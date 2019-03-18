using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace oBrowser2
{
	class Logger
	{
		private static StreamWriter sw = null;

		public static void Log(string text)
		{
			if (sw == null)
			{
				string path = Application.ExecutablePath + ".Log";
				
				// 로그파일 크기가 1MB이상 되면 삭제하고 새로 만든다.
				FileInfo fi = new FileInfo(path);
				if (fi.Exists && fi.Length > 1024 * 1024) fi.Delete();

				sw = new StreamWriter(new FileStream(path,
				                                     FileMode.Append,
				                                     FileAccess.Write,
				                                     FileShare.ReadWrite),
				                      Encoding.Default);
			}
			sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
			sw.Flush();
		}

		public static void Close()
		{
			if (sw != null) sw.Close();
			sw = null;
		}
	}
}
