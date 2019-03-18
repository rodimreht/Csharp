using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace oBrowser2
{
	class FirefoxControl
	{
		public static bool setFFSession(string uniNum, string ffPath, string cookies)
		{
			try
			{
				string path = "";

				// FirefoxPartable인 경우 체크
				if (ffPath.ToLower().IndexOf("firefoxportable.exe") > 0)
				{
					path = ffPath.Substring(0, ffPath.LastIndexOf("\\")) + @"\Data\profile";
				}
				else
				{
					// Profile 읽기
					string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\";
					IniFile ini = new IniFile(iniPath + "profiles.ini");
					path = iniPath + ini.IniReadValue("Profile0", "Path");
				}

				// Modify Headers Add-on 설치여부 검사
				string extDir = path + @"\extensions\{b749fc7c-e949-447f-926c-3f4eed6accfe}";
				if (!Directory.Exists(extDir)) return true;

				// 보정작업: invalidprefs.js 파일 삭제하기
				string invalidprefPath = path + @"\invalidprefs.js";
				invalidprefPath = invalidprefPath.Replace("/", @"\");
				if (File.Exists(invalidprefPath)) File.Delete(invalidprefPath);

				// prefs.js 파일 읽기
				string prefPath = path + @"\prefs.js";
				prefPath = prefPath.Replace("/", @"\");

				FileStream fsRead = new FileStream(prefPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				StreamReader reader = new StreamReader(fsRead, Encoding.UTF8);
				string text = reader.ReadToEnd();

				// 이미 등록된 경우
				if ((text.IndexOf("modifyheaders.config") > 0) && (text.IndexOf("prsess_") > 0))
				{
					fsRead.Position = 0;
					StringBuilder sb = new StringBuilder(20000);
					while (!reader.EndOfStream)
					{
						string sTemp = reader.ReadLine();

						// 보정작업: user_pref(로 시작하지 않는 행 제거
						if (!sTemp.StartsWith("user_pref(")) continue;

						// ModifyHeaders 기능 사용 못함 2009-12-14: for Redesign
						/*
						if (sTemp.IndexOf("prsess_") > 0) // 쿠키 내용만 교체
						{
							int pos1 = sTemp.IndexOf(", \"");
							int pos2 = sTemp.IndexOf("\");", pos1);
							string org = sTemp.Substring(pos1 + 3, pos2 - pos1 - 3);
							sTemp = sTemp.Replace(org, replaceCookie(org, uniNum, cookies));
						}
						*/

						if (sTemp.IndexOf("modifyheaders.headers.enabled") > 0) // ModifyHeaders 기능 중지
						{
							sTemp = sTemp.Replace("true", "false");
						}

						sb.Append(sTemp + "\r\n");
					}
					reader.Close();
					text = sb.ToString();

					FileStream fsWrite = new FileStream(prefPath, FileMode.Truncate, FileAccess.Write, FileShare.Read);
					StreamWriter sw = new StreamWriter(fsWrite, Encoding.UTF8);
					sw.Write(text);
					sw.Flush();
					sw.Close();
				}
				else // 등록된 적이 없으면 무시한다.
				{
					/*
					fsRead.Position = 0;
					StringBuilder sb = new StringBuilder(20000);
					while (!reader.EndOfStream)
					{
						string sTemp = reader.ReadLine();

						// 보정작업: user_pref(로 시작하지 않는 행 제거
						if (!sTemp.StartsWith("user_pref(")) continue;
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
	user_pref(""modifyheaders.headers.value2"", ""Mozilla/5.0 (Windows; U; Windows NT 5.1; ko; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.5 (.NET CLR 3.5.30729)"");
	";
						text = text.Substring(0, pos) + insText + text.Substring(pos);
					}
					*/
				}

				return true;

			}
			catch (Exception)
			{
				return false;
			}
		}

		/*
		private static string replaceCookie(string existCookie, string uniNum, string newCookie)
		{
			// 해당 우주 쿠키가 있으면 교체한다
			string cookie = "";
			if (existCookie.IndexOf("=U_en" + uniNum + "%3A") > 0)
			{
				int lastPos = existCookie.IndexOf("=U_en" + uniNum + "%3A");
				int uNumPos = existCookie.Substring(0, lastPos).LastIndexOf("login_");
				string uNum = existCookie.Substring(uNumPos + 6, lastPos - uNumPos - 6);
				cookie = arrangeCookie(existCookie, "login_" + uNum);
				cookie = arrangeCookie(cookie, "prsess_" + uNum);
				cookie = arrangeCookie(cookie, "PHPSESSID").Trim();
			}
			else // 없으면 추가한다
			{
				cookie = arrangeCookie(existCookie, "PHPSESSID").Trim();
			}
			return cookie.Length > 0 ? cookie + "; " + newCookie : newCookie;
		}

		// 쿠키에서 removeVal값을 제거한다.
		private static string arrangeCookie(string cookies, string removeVal)
		{
			string newCookies;
			string a = removeVal + "=";
			int pos1 = cookies.IndexOf(a);
			if (pos1 >= 0)
			{
				int pos2 = cookies.IndexOf("; ", pos1);
				if (pos2 > 0)
				{
					string sTemp = cookies.Substring(pos1, pos2 + 2 - pos1);
					newCookies = cookies.Replace(sTemp, "");
				}
				else
					newCookies = cookies.Substring(0, pos1);
			}
			else
				newCookies = cookies;

			return newCookies;
		}
		*/
	}
}
