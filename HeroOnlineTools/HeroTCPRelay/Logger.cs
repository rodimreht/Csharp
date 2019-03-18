using System;
using System.Collections;
using System.Configuration;

namespace HeroTCPRelay
{
	/// <summary>
	/// 로그를 남기기 위해 사용되는 클래스 (copy from NETS*IM 2.6)
	/// </summary>
	/// <remarks>
	/// 파일 혹은 MQ를 사용하여 로그를 남길수 있다.
	/// </remarks>	
	public class Logger
	{
		/// <summary>
		/// 로그를 남기는데 필요한 최소 레벨
		/// </summary>
		public static LogLevel defaultLevel = LogLevel.INFORMATION;
		private static Hashtable driverTbl = Hashtable.Synchronized(new Hashtable());

		static Logger()
		{
			// 로그 드라이버들 로딩
			lock (typeof(Logger))
			{
				LogDriver driver = new FileLogDriver();
				driver.initialize(ConfigurationManager.AppSettings["LogLevel"],
								  ConfigurationManager.AppSettings["fileLogPath"],
								  "Log.txt",
								  5,
								  "day",
								  "utf8",
								  false);
				driverTbl.Add("FILE", driver);
				
				driver = new MQLogDriver();
				driver.initialize(ConfigurationManager.AppSettings["LogLevel"],
								  ConfigurationManager.AppSettings["MQLogPath"],
								  null,
								  0,
								  null,
								  null,
								  true);
				driverTbl.Add("MQ", driver);

				driver = new ConsoleLogDriver();
				driver.initialize(ConfigurationManager.AppSettings["LogLevel"],
								  null,
								  null,
								  0,
								  null,
								  null,
								  false);
				driverTbl.Add("CONSOLE", driver);
			}
		}

		/// <summary>
		/// Gets the default channel.
		/// </summary>
		/// <returns></returns>
		static string getDefaultChannel()
		{
			string channel = ConfigurationManager.AppSettings["LogChannel"];
			if ((channel == null) || (channel.Length == 0)) channel = "FILE";
			return channel;
		}

		/// <summary>
		/// 문자열을 LogLevel 형으로 변환한다.
		/// </summary>
		/// <remarks>
		/// 대소문자 구분없이 "NOTSET", "INFORMATION", "DEBUG", "WARNING", "ERROR"을 해당하는 LogLevel의 값으로 반환
		/// </remarks>
		/// <param name="levelStr">로그 레벨 문자열. information/debug/warning/error/notset 중의 하나이다.</param>
		/// <returns>LogLevel 상수</returns>
		public static LogLevel ToLogLevel(string levelStr)
		{
			try
			{
				return (LogLevel)Enum.Parse(typeof(LogLevel), levelStr, true);
			}
			catch
			{
				return LogLevel.NOTSET;
			}
		}

		/// <summary>
		/// 디폴트 채널을 사용하여 현재 시각의 로그를 남김
		/// </summary>
		/// <remarks>
		/// 로그 시간은 함수 호출 시간으로 자동 설정한다.
		/// </remarks>
		/// <param name="level">LogLevel</param>
		/// <param name="systemID">로그를 남기는 시스템의 이름</param>
		/// <param name="msg">로그 메세지</param>
		public static void Log(LogLevel level, string systemID, string msg)
		{
			Log(getDefaultChannel(), level, systemID, DateTime.Now, msg);
		}

		/// <summary>
		/// 디폴트 채널을 사용하여 주어진 시각의 로그를 남김
		/// </summary>
		/// <example>
		/// <code>
		/// Logger.Log(LogLevel.WARNING, "SSO", DateTime.Now, "Unindentifyed user");		
		/// </code>
		/// </example>
		/// <param name="level">LogLevel</param>
		/// <param name="systemID">로그를 남기는 시스템의 이름</param>
		/// <param name="time">로그 시간</param>
		/// <param name="msg">로그 메세지</param>
		public static void Log(LogLevel level, string systemID, DateTime time, string msg)
		{
			Log(getDefaultChannel(), level, systemID, time, msg);
		}

		/// <summary>
		/// 채널을 명시해서 현재 시각의 로그를 남김
		/// </summary>
		/// <remarks>
		/// 로그 시간은 함수 호출 시간으로 자동 설정한다.
		/// </remarks>
		/// <param name="channelName">채널 이름</param>
		/// <param name="level">LogLevel</param>
		/// <param name="systemID">로그를 남기는 시스템의 이름</param>
		/// <param name="msg">로그 메세지</param>
		public static void Log(string channelName, LogLevel level, string systemID, string msg)
		{
			GetLogDriver(channelName).Log(channelName, level, systemID, DateTime.Now, msg);
		}

		/// <summary>
		/// 채널을 선택하여 주어진 시각의 로그를 남긴다.
		/// </summary>
		/// <example>
		/// <code>
		/// Logger.Log("ssofile", LogLevel.WARNING, "SSO", DateTime.Now, "Unindentifyed user");
		/// Logger.Log("ssomq", LogLevel.INFORMATION,"SSO", DateTime.Now, "SSO succeeded");		
		/// </code>
		/// </example>
		/// <param name="channelName">로그 채널 이름</param>
		/// <param name="level">LogLevel</param>
		/// <param name="systemID">로그를 남기는 시스템의 이름, 시스템 ID 별로 별도의 로그파일 유지</param>
		/// <param name="time">로그 시각</param>
		/// <param name="msg">로그 메세지</param>
		public static void Log(string channelName, LogLevel level, string systemID, DateTime time, string msg)
		{
			GetLogDriver(channelName).Log(channelName, level, systemID, time, msg);
		}

		/// <summary>
		/// 특정 채널의 LogLevel을 리턴한다.
		/// </summary>
		/// <remarks>
		/// nets.config에 설정된 채널의 최소 로그 레벨을 반환한다.
		/// 각 채널은 최소 로그 레벨 이상의 로그메세지만 기록한다.
		/// </remarks>
		/// <param name="channelName">CHANNEL_FILE or CHANNEL_CONSOLE ....</param>
		/// <returns>채널의 최소 LogLevel</returns>
		public static LogLevel GetLogLevel(string channelName)
		{
			return GetLogDriver(channelName).Level;
		}

		public static string GetPath(string systemID)
		{
			return GetLogDriver(getDefaultChannel()).GetPath(systemID);
		}

		public static string GetPath(string channelName, string systemID)
		{
			return GetLogDriver(channelName).GetPath(systemID);
		}
		
		/// <summary>
		/// 로그 기능을 종료한다.
		/// </summary>
		/// <remarks>
		/// CANNEL_FILE 과 CHANNEL_QUERY의 경우는 로그 파일을 닫아준다. 그외의 채널에서는 아무일도 하지 않는다.
		/// </remarks>
		public static void Close()
		{
			foreach (LogDriver driver in driverTbl.Values)
			{
				driver.Close();
			}
		}

		private static LogDriver GetLogDriver(string channelName)
		{
			LogDriver driver = (LogDriver)driverTbl[channelName];
			if (driver == null)
				throw new Exception("Logger:GetLogDriver() No such log channel " + channelName);
			return driver;
		}
	}
}
