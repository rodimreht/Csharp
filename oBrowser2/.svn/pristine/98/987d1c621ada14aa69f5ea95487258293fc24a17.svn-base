using System;
using System.Collections.Generic;
//using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace oBrowser2
{
	class ChromeControl
	{
		/*
		private const string _FIND = "\"action\":\"Filter\",\"name\":\"Cookie\"";
		private const string _MHJSON1 = "{{\"__jstorage_meta\":{{\"CRC32\":{{\"CMH.HEADERS\":\"2.2450418687\",\"CMH.CTRL\":\"2.3245496982\"}},\"TTL\":{{}}}},\"CMH.HEADERS\":[{0}],\"CMH.CTRL\":\"STARTED\"}}";
		private const string _MHJSON2 = 
			"{{\"index\":{0},\"action\":\"Filter\",\"name\":\"Cookie\",\"value\":\"\",\"description\":\"\",\"state\":\"ENABLED\"}}," +
			"{{\"index\":{1},\"action\":\"Add\",\"name\":\"Cookie\",\"value\":\"{3}\",\"description\":\"\",\"state\":\"ENABLED\"}}," +
			"{{\"index\":{2},\"action\":\"Modify\",\"name\":\"User-Agent\",\"value\":\"{4}\",\"description\":\"\",\"state\":\"ENABLED\"}}";

		/// <summary>
		/// Sets the login session.
		/// </summary>
		/// <param name="uniName">Name of the uni.</param>
		/// <param name="chromePath">The chrome path.</param>
		/// <param name="cookies">The cookies.</param>
		/// <returns></returns>
		public static bool SetLoginSession(string uniName, string chromePath, string cookies)
		{
			try
			{
				string path = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\Google\Chrome\User Data\Default";

				// Modify Headers Add-on 설치여부 검사
				string extDir = path + @"\Extensions\innpjfdalfhpcoinfnehdnbkglpmogdi";
				if (!Directory.Exists(extDir)) return true;

				// sqlite3 localstorage 파일 읽기
				string confPath = path + @"\Local Storage\chrome-extension_innpjfdalfhpcoinfnehdnbkglpmogdi_0.localstorage";

				// Data Source=c:\Users\Thermidor\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_innpjfdalfhpcoinfnehdnbkglpmogdi_0.localstorage;Pooling=true;FailIfMissing=false
				using (var conn = new SQLiteConnection("Data Source=" + confPath + ";Version=3;Pooling=true;FailIfMissing=false"))
				using (var cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText = "SELECT key, value FROM ItemTable WHERE key = 'jStorage'";

					string jStorage = string.Empty;
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							jStorage = reader.GetString(reader.GetOrdinal("value"));
							Logger.Log(string.Format("Chrome localstorage - jStorage: {0}", jStorage));
						}
						reader.Close();
					}

					string text = string.Empty;
					if (string.IsNullOrEmpty(jStorage))
					{
						string insText = string.Format(_MHJSON2, 1, 2, 3, cookies, WebCall.UserAgent);
						text = string.Format(_MHJSON1, insText);

						cmd.CommandText = "INSERT INTO ItemTable(key, value) VALUES('jStorage', '" + text.Replace("'", "''") + "')";
						cmd.ExecuteNonQuery();
						cmd.CommandText = "INSERT INTO ItemTable(key, value) VALUES('jStorage_update', '1400294892485')";
						cmd.ExecuteNonQuery();
					}
					else
					{
						int pos = jStorage.IndexOf(_FIND);
						if (pos > 0)
						{
							string s = jStorage.Substring(0, pos);
							int iPos = s.LastIndexOf("{\"index\":");
							int iPos2 = s.LastIndexOf(",");
							if (iPos < 0 || iPos2 < 0)
							{
								string insText = string.Format(_MHJSON2, 1, 2, 3, cookies, WebCall.UserAgent);
								text = string.Format(_MHJSON1, insText);

								cmd.CommandText = "UPDATE ItemTable SET value = '" + text.Replace("'", "''") + "' WHERE key = 'jStorage'";
								cmd.ExecuteNonQuery();
							}
							else
							{
								int lastIndex;
								if (!int.TryParse(s.Substring(iPos + 9, iPos2 - iPos - 9), out lastIndex)) lastIndex = 1;
								string insText = string.Format(_MHJSON2, lastIndex, lastIndex + 1, lastIndex + 2, cookies, WebCall.UserAgent);
								text = string.Format(_MHJSON1, insText);

								cmd.CommandText = "UPDATE ItemTable SET value = '" + text.Replace("'", "''") + "' WHERE key = 'jStorage'";
								cmd.ExecuteNonQuery();
							}
						}
						else
						{
							string insText = string.Format(_MHJSON2, 1, 2, 3, cookies, WebCall.UserAgent);
							text = string.Format(_MHJSON1, insText);

							cmd.CommandText = "UPDATE ItemTable SET value = '" + text.Replace("'", "''") + "' WHERE key = 'jStorage'";
							cmd.ExecuteNonQuery();
						}
					}

					conn.Close();
				}
				return true;

			}
			catch (Exception ex)
			{
				Logger.Log("ChromeControl 로그인 설정 실패: " + ex.Message);
				return false;
			}
		}
		*/
	}
}
