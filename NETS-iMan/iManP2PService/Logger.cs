using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace iManP2PService
{
	class Logger
	{
		private static StreamWriter sw;
		private static readonly string logPath;

		static Logger()
		{
			logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrEmpty(logPath))
			{
				logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
				logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
			}
			logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "iManP2PService";
		}

		public static void Log(string text)
		{
#if DEBUG
			if (sw == null)
			{
				string path = logPath + ".Log";

				// 로그파일 크기가 1MB이상 되면 백업하고 새로 만든다.
				FileInfo fi = new FileInfo(path);
				if (!fi.Directory.Exists) fi.Directory.Create();
				if (fi.Exists && fi.Length > 1024 * 1024)
					fi.MoveTo(logPath + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + ".Log");

				sw = new StreamWriter(new FileStream(path,
				                                     FileMode.Append,
				                                     FileAccess.Write,
				                                     FileShare.ReadWrite),
				                      Encoding.Default);
			}
			sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
			sw.Flush();
#endif
		}

		public static void Close()
		{
#if DEBUG
			if (sw != null) sw.Close();
			sw = null;
#endif
		}
	}
}
