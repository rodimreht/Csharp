using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace oBrowser2
{
	class FirefoxControl
	{
		public static bool setFFSession(string cookies)
		{
			// Profile 읽기
			string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\";
			IniFile ini = new IniFile(iniPath + "profiles.ini");
			string path = ini.IniReadValue("Profile0", "Path");

			// Modify Headers Add-on 설치여부 검사
			string extDir = iniPath + path + @"\extensions\{b749fc7c-e949-447f-926c-3f4eed6accfe}";
			if (!Directory.Exists(extDir)) return false;

			// prefs.js 파일 읽기
			string prefPath = iniPath + path + @"\prefs.js";
			prefPath = prefPath.Replace("/", @"\");

			FileStream fsRead = new FileStream(prefPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader reader = new StreamReader(fsRead, Encoding.UTF8);
			string text = reader.ReadToEnd();

			// 이미 등록된 경우
			if ((text.IndexOf("modifyheaders.config") > 0) && (text.IndexOf("__utma=") > 0))
			{
				fsRead.Position = 0;
				StringBuilder sb = new StringBuilder(20000);
				while (!reader.EndOfStream)
				{
					string sTemp = reader.ReadLine();
					if (sTemp.IndexOf("__utma=") > 0) // 쿠키 내용만 교체
					{
						int pos1 = sTemp.IndexOf(", \"");
						int pos2 = sTemp.IndexOf("\");", pos1);
						string org = sTemp.Substring(pos1 + 3, pos2 - pos1 - 3);
						sTemp = sTemp.Replace(org, cookies);
					}
					
					sb.Append(sTemp + "\r\n");
				}
				reader.Close();
				text = sb.ToString();
			}
			else // 등록된 적이 없으면 초기화한다.
			{
				fsRead.Position = 0;
				StringBuilder sb = new StringBuilder(20000);
				while (!reader.EndOfStream)
				{
					string sTemp = reader.ReadLine();
					if (sTemp.IndexOf(@"user_pref(""modifyheaders") >= 0) continue;
					sb.Append(sTemp + "\r\n");
				}
				reader.Close();
				text = sb.ToString();

				int pos = text.IndexOf("user_pref(\"network.cookie");
				if (pos > 0)
				{
					string insText = @"user_pref(""modifyheaders.config.alwaysOn"", true);
user_pref(""modifyheaders.config.logMsgs"", false);
user_pref(""modifyheaders.config.openNewTab"", false);
user_pref(""modifyheaders.headers.action0"", ""Filter"");
user_pref(""modifyheaders.headers.action1"", ""Add"");
user_pref(""modifyheaders.headers.action2"", ""Modify"");
user_pref(""modifyheaders.headers.count"", 3);
user_pref(""modifyheaders.headers.enabled0"", true);
user_pref(""modifyheaders.headers.enabled1"", true);
user_pref(""modifyheaders.headers.enabled2"", true);
user_pref(""modifyheaders.headers.name0"", ""Cookie"");
user_pref(""modifyheaders.headers.name1"", ""Cookie"");
user_pref(""modifyheaders.headers.name2"", ""User-Agent"");
user_pref(""modifyheaders.headers.value0"", """");
user_pref(""modifyheaders.headers.value1"", """ + cookies + @""");
user_pref(""modifyheaders.headers.value2"", ""Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"");
";
					text = text.Substring(0, pos) + insText + text.Substring(pos);
				}
			}

			FileStream fsWrite = new FileStream(prefPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
			StreamWriter sw = new StreamWriter(fsWrite, Encoding.UTF8);
			sw.Write(text);
			sw.Flush();
			sw.Close();

			return true;
		}

		private static string arrangeCookie(string cookies)
		{
			string newCookies;
			string a = "PHPSESSID=";
			int pos1 = cookies.IndexOf(a);
			if (pos1 >= 0)
			{
				int pos2 = cookies.IndexOf("; ", pos1);
				if (pos2 > 0)
				{
					string sTemp = cookies.Substring(pos1, pos2 - pos1);
					newCookies = cookies.Replace(sTemp, "");
				}
				else
					newCookies = cookies.Substring(0, pos1);
			}
			else
				newCookies = cookies;

			return newCookies;
		}
	}
}
