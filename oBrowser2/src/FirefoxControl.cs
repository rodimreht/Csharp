using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace oBrowser2
{
	class FirefoxControl
	{
		private const string _FIND = "{\"action\":\"Filter\",\"name\":\"Cookie\"";
		private const string _MHJSON1 = "{{\"headers\":[{0}]}}";
		private const string _MHJSON2 = 
			"{{\"action\":\"Filter\",\"name\":\"Cookie\",\"value\":\"\",\"comment\":\"\",\"enabled\":true}}," +
			"{{\"action\":\"Add\",\"name\":\"Cookie\",\"value\":\"{0}\",\"comment\":\"\",\"enabled\":true}}," +
			"{{\"action\":\"Modify\",\"name\":\"User-Agent\",\"value\":\"{1}\",\"comment\":\"\",\"enabled\":true}}";

		public static bool SetLoginSession(string uniName, string ffPath, string cookies)
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

				// Modify Headers Add-on 설치여부 검사: .xpi 파일로 변경 since 2012
				string extDir = path + @"\extensions\{b749fc7c-e949-447f-926c-3f4eed6accfe}";
				if (!Directory.Exists(extDir) && !File.Exists(extDir + ".xpi")) return true;

				// 보정작업: invalidprefs.js 파일 삭제하기
				string invalidprefPath = path + @"\invalidprefs.js";
				invalidprefPath = invalidprefPath.Replace("/", @"\");
				if (File.Exists(invalidprefPath)) File.Delete(invalidprefPath);

				// prefs.js 파일 읽기
				string prefPath = path + @"\prefs.js";
				prefPath = prefPath.Replace("/", @"\");

				string text;
				using (FileStream fsRead = new FileStream(prefPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader reader = new StreamReader(fsRead, Encoding.UTF8))
					{
						text = reader.ReadToEnd();

						// 이미 등록된 경우
						if (text.IndexOf("modifyheaders.config") > 0)
						{
							fsRead.Position = 0;
							StringBuilder sb = new StringBuilder(20000);
							while (!reader.EndOfStream)
							{
								string sTemp = reader.ReadLine();

								// 보정작업: user_pref(로 시작하지 않는 행 제거
								if (!sTemp.StartsWith("user_pref(")) continue;

								if (sTemp.IndexOf("modifyheaders.config.active") > 0 && sTemp.IndexOf("false") > 0)
									sTemp = sTemp.Replace("false", "true");

								sb.Append(sTemp + "\r\n");
							}
							reader.Close();
							text = sb.ToString();
						}
						else // 등록된 적이 없으면 새로 등록한다.
						{
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
								string insText = @"user_pref(""modifyheaders.config.active"", true);
user_pref(""modifyheaders.config.migrated"", true);
user_pref(""modifyheaders.config.openNewTab"", false);
user_pref(""modifyheaders.headers.count"", 0);
";
								text = text.Substring(0, pos) + insText + text.Substring(pos);
							}
						}
					}
				}

				using (StreamWriter sw = new StreamWriter(
					new FileStream(prefPath, FileMode.Truncate, FileAccess.Write, FileShare.Read),
					Encoding.UTF8))
				{
					sw.Write(text);
					sw.Flush();
					sw.Close();
				}

				// ModifyHeaders 기능 변경됨: 외부 modifyheaders.conf 파일 사용 since 2012
				string confPath = path + @"\modifyheaders.conf";
				confPath = confPath.Replace("/", @"\");
				if (File.Exists(confPath))
				{
					using (StreamReader reader = new StreamReader(
						new FileStream(confPath, FileMode.Open, FileAccess.Read, FileShare.Read),
						Encoding.Default))
					{
						text = reader.ReadToEnd();

						// 이미 등록된 경우
						int pos = text.IndexOf(_FIND);
						if (pos < 0) pos = text.LastIndexOf("]");
						if (pos < 0)
						{
							string insText = string.Format(_MHJSON2, cookies, WebCall.UserAgent);
							text = string.Format(_MHJSON1, insText);
						}
						else
						{
							string insText = string.Format(_MHJSON2, cookies, WebCall.UserAgent);
							text = text.Substring(0, pos) + insText + "]}";
						}
						reader.Close();
					}
				}
				else
				{
					string insText = string.Format(_MHJSON2, cookies, WebCall.UserAgent);
					text = string.Format(_MHJSON1, insText);
				}

				using (StreamWriter sw = new StreamWriter(
					new FileStream(confPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read),
					Encoding.Default))
				{
					sw.Write(text);
					sw.Flush();
					sw.Close();
				}
				return true;

			}
			catch (Exception ex)
			{
				Logger.Log("FirefoxControl 로그인 설정 실패: " + ex.Message);
				return false;
			}
		}
	}
}
