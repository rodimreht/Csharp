using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace oBrowser2Stat
{
	class Logger
	{
		private static StreamWriter sw = null;

		public static void Log(string text)
		{
			try
			{
				if (sw == null)
				{
					string path = ConfigurationManager.AppSettings["path"] + @"\" + DateTime.Today.ToString("yyyyMMdd") + ".Log";
					sw = new StreamWriter(new FileStream(path,
														 FileMode.Append,
														 FileAccess.Write,
														 FileShare.ReadWrite),
										  Encoding.Default);
				}
				sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
				sw.Flush();
			}
			catch (Exception)
			{
			}
		}

		public static void Close()
		{
			if (sw != null) sw.Close();
			sw = null;
		}
	}
}
