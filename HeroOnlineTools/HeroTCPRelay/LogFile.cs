using System.IO;

namespace HeroTCPRelay
{
	/// <summary>
	/// 각 로그 파일에 대한 정보를 보관한다. (copy from NETS*IM 2.6)
	/// </summary>
	public class LogFile
	{
		private StreamWriter sw; // 파일의 writer
		private int currentBytes; // 파일 크기
		private string systemID; // 로그파일을 사용하는 system 아이디
		private string logFileAbsolutePath; // 현재 로그파일의 절대 경로

		public LogFile(string systemID, string path, StreamWriter sw, int bytes)
		{
			this.systemID = systemID;
			this.sw = sw;
			this.logFileAbsolutePath = path;
			currentBytes = bytes;
		}

		public string SystemID
		{
			get { return systemID; }
			set { systemID = value; }
		}

		public StreamWriter SW
		{
			get { return sw; }
			set { sw = value; }
		}

		public int CurrentBytes
		{
			get { return currentBytes; }
			set { currentBytes = value; }
		}

		public string Path
		{
			get { return logFileAbsolutePath; }
			set { logFileAbsolutePath = value; }
		}
	}
}
