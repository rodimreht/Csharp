using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NETS_iMan
{
	class ChatLogger
	{
		private static StreamWriter sw = null;
		private static string logPath = null;
		private static bool errorFlag = false;

		static ChatLogger()
		{
			logPath = SettingsHelper.Current.LogPath;
			if (string.IsNullOrEmpty(logPath))
			{
				logPath = Application.ExecutablePath;
				logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
			}
			logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "NETS-iMan_chat";
		}

		public static void Log(string text)
		{
			try
			{
				if (errorFlag) return;

				if (sw == null)
				{
					string path = logPath + ".Log";

					// 로그파일 크기가 1MB이상 되면 백업하고 새로 만든다.
					FileInfo fi = new FileInfo(path);
					if (fi.Directory == null) throw new Exception(path + " 경로의 폴더가 없습니다.");
					if (!fi.Directory.Exists) fi.Directory.Create();
					if (fi.Exists && fi.Length > 1024 * 1024)
					{
						if (!mergeOldfiles(fi)) fi.MoveTo(logPath + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + ".Log");
					}

					sw = new StreamWriter(new FileStream(path,
														 FileMode.Append,
														 FileAccess.Write,
														 FileShare.ReadWrite),
										  Encoding.Default);
				}
				sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
				sw.Flush();

			}
			catch (Exception exx)
			{
				errorFlag = true;
				MessageBoxEx.Show(
					"다음과 같은 오류가 발생했습니다: [" + exx.Message.Replace("\r\n", "").Replace("\n", "") + "]\r\n\r\n도구-옵션에서 로그파일 경로를 조정하십시오.",
					"대화로그 오류",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error,
					30*1000);
			}
		}

		private static bool mergeOldfiles(FileInfo fi)
		{
			DirectoryInfo di = fi.Directory;
			if (di == null) return false;

			FileInfo[] fis = di.GetFiles("NETS-iMan_chat.*.Log");
			if (fis.Length == 0) return false;

			Stream stream = fis[0].Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			StreamWriter writer = new StreamWriter(stream, Encoding.Default);

			if (fis.Length > 1)
			{
				for (int i = 1; i < fis.Length; i++)
				{
					Stream s = fis[i].Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					StreamReader sr = new StreamReader(s, Encoding.Default);
					string text = sr.ReadToEnd();
					sr.Close();
					fis[i].Delete();

					writer.Write(text);
					writer.Flush();
				}
			}

			Stream s2 = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader sr2 = new StreamReader(s2, Encoding.Default);
			string text2 = sr2.ReadToEnd();
			sr2.Close();
			fi.Delete();

			writer.Write(text2);
			writer.Flush();
			writer.Close();
			fis[0].MoveTo(logPath + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + ".Log");
			return true;
		}

		public static void ChangeDir()
		{
			logPath = SettingsHelper.Current.LogPath;
			if (string.IsNullOrEmpty(logPath))
			{
				logPath = Application.ExecutablePath;
				logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
			}
			Log("경로바뀜: " + logPath);
			logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "NETS-iMan_chat";
			
			Close();
		}

		public static void Close()
		{
			errorFlag = false;
			if (sw != null) sw.Close();
			sw = null;
		}
	}
}
