using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NETS_iMan
{
	internal class Logger
	{
		internal enum LogLevel
		{
			NOTSET = 0,
			DEBUG = 1,
			INFORMATION = 2,
			WARNING = 3,
			ERROR = 4
		}

		private static bool errorFlag;
		private static string logPath;
		private static StreamWriter sw;

		static Logger()
		{
			logPath = SettingsHelper.Current.LogPath;
			if (string.IsNullOrEmpty(logPath))
			{
				logPath = Application.ExecutablePath;
				logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
			}
			logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "NETS-iMan";
		}

		public static void Log(string text)
		{
			Log(LogLevel.DEBUG, text);
		}

		public static void Log(LogLevel lvl, string text)
		{
#if DEBUG
			if (lvl < LogLevel.DEBUG) return;
#else
			if (lvl < LogLevel.INFORMATION) return;
#endif
			try
			{
				if (errorFlag) return;

				if (sw == null)
				{
					string path = logPath + ".Log";

					// 로그파일 크기가 1MB이상 되면 삭제하고 새로 만든다.
					FileInfo fi = new FileInfo(path);
					if (fi.Directory == null) throw new Exception(path + " 경로의 폴더가 없습니다.");
					if (!fi.Directory.Exists) fi.Directory.Create();

					if (fi.Exists && fi.Length > 1024 * 1024) fi.Delete();

					sw = new StreamWriter(new FileStream(path,
														 FileMode.Append,
														 FileAccess.Write,
														 FileShare.ReadWrite),
										  Encoding.Default);
				}
				sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss ") + lvl + "] " + text);
				sw.Flush();
			}
			catch (Exception exx)
			{
				errorFlag = true;
				MessageBoxEx.Show(
					"다음과 같은 오류가 발생했습니다: [" + exx.Message.Replace("\r\n", "").Replace("\n", "") + "]\r\n\r\n도구-옵션에서 로그파일 경로를 조정하십시오.",
					"로그 오류",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error,
					30 * 1000);
			}
		}

		public static void ChangeDir()
		{
			logPath = SettingsHelper.Current.LogPath;
			if (string.IsNullOrEmpty(logPath))
			{
				logPath = Application.ExecutablePath;
				logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
			}
			Log(LogLevel.WARNING, "경로바뀜: " + logPath);
			logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "NETS-iMan";

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