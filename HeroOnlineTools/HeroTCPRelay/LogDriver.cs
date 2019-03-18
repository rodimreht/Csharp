using System;
using System.Text;

namespace HeroTCPRelay
{
	/// <summary>
	/// Summary description for LogDriver. (copy from NETS*IM 2.6)
	/// </summary>
	public abstract class LogDriver
	{
		protected LogLevel level; // 각 채널의 로그 레벨
		protected string path; // 로그를 남길 디렉토리
		protected string fileName; // 로그 파일명		
		protected int maxSize; // 하나의 로그 파일이 가질수 있는 최대크기
		protected string period; // 로그파일 갱신주기(주기별로 paht및에 디렉토리가 생성된다.)
		protected Encoding encoding; // 로그파일의 인코딩 타입
		protected bool useTransaction;

		/// <summary>
		/// 로그 메시지에 사용되는 구분자
		/// </summary>
		/// <remarks>
		/// 로그 메시지에 사용되는 구분자.
		/// '|' 문자가 사용된다.
		/// </remarks>
		public static string delim = "|"; // 로그파일 구분자

		/// <summary>
		/// 로그 레벨
		/// </summary>
		/// <remarks>
		/// 로그 레벨. 읽기 전용
		/// </remarks>
		public LogLevel Level
		{
			get { return level; }
		}

		public virtual void initialize(string level, string path, string filename, int maxsize, string period, string encoding, bool useTransaction)
		{
			this.level = (level == null || level.Length < 1) ? Logger.defaultLevel : Logger.ToLogLevel(level);
			this.path = (path != null && path.EndsWith(@"\")) ? path.Remove(path.Length - 1, 1) : path;
			this.fileName = filename;
			this.maxSize = maxsize * 1024 * 1024;
			this.period = (period != null) ? period.ToLower() : "";
			this.encoding = GetEncoding(encoding);
			this.useTransaction = useTransaction;
		}

		private Encoding GetEncoding(string encoding)
		{
			switch (encoding)
			{
				case "utf8":
					return Encoding.UTF8;
				case "utf7":
					return Encoding.UTF7;
				case "unicode":
					return Encoding.Unicode;
				default:
					return Encoding.Default;
			}
		}

		/// <summary>
		/// 지정한 로그 레벨이 최소 로그 레벨 이상인지 반환
		/// </summary>
		/// <param name="userLevel">지정 로그 레벨</param>
		/// <returns>조건을 만족하면 true</returns>
		public bool CheckLevel(LogLevel userLevel)
		{
			return userLevel >= level;
		}

		public abstract void Log(string channelName, LogLevel level, string systemID, DateTime time, string msg);
		public abstract void Close();

		public virtual string GetPath(string systemID) 
		{
			return null;
		}
		
		public static string FormatMessage(LogLevel level, string systemID, DateTime time, string message)
		{
			return level + delim + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + delim + message;
		}
	}
}
