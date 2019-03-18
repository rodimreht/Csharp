using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace HeroTCPRelay
{
	/// <summary>
	/// 로그파일의 이름은 "systemID"-"날짜"-"프로세스ID"-"nets.config에 명시된 파일이름"
	/// (copy from NETS*IM 2.6)
	/// </summary>
	public class FileLogDriver : LogDriver
	{
		protected string logDirectory; // 백업정책에 따른 디렉토리
		protected Hashtable fileTbl = new Hashtable(); // systemID별 LogFile객체를 보관

		/// <summary>
		/// 로그 정책(시간/일/월)에 따라 로그 파일이 위치할 디렉토리 절대 경로를 반환한다("\"로 시작).
		/// </summary>
		/// <returns></returns>
		protected string GetLogDirectory()
		{
			string ret = path + @"\";

			DateTime now = DateTime.Now;
			if (period == "hour")
				ret += now.ToString("yyyy-MM-dd") + "-" + now.Hour;
			else if (period == "day")
				ret += now.ToString("yyyy-MM-dd");
			else
				ret += now.Year + "-" + now.Month;

			return ret;
		}

		public override string GetPath(string systemID)
		{
			LogFile file = getFileHandle(systemID);
			return file.Path;
		}
		
		/// <summary>
		/// 디렉토리를 만들고 새로운 로그파일을 생성한다.
		/// </summary>
		/// <param name="systemID"></param>
		/// <returns></returns>
		public LogFile CreateLogFile(string systemID)
		{
			string logDir = GetLogDirectory();

			if (!Directory.Exists(logDir))
				Directory.CreateDirectory(logDir);

			string s = logDir + @"\" + createFileName(systemID);
			
			StreamWriter sw = new StreamWriter(
				new FileStream(s, FileMode.Append, FileAccess.Write, FileShare.ReadWrite),
				encoding);

			return new LogFile(systemID, s, sw, 0);
		}

		/// <summary>
		/// 로그 파일이름을 반환
		/// </summary>
		/// <param name="systemID"></param>
		/// <returns></returns>
		private string createFileName(string systemID)
		{
			return String.Format("{0}-{1}-{2}-{3}",
				systemID,
				DateTime.Now.ToString("yyyyMMddHHmmss"),
				Process.GetCurrentProcess().Id.ToString("x"), //Thread.CurrentThread.GetHashCode().ToString("x"),
				fileName);
		}

		#region LogDriver Members

		public override void Log(string channelName, LogLevel level, string systemID, DateTime time, string msg)
		{
			// 최소 로그레벨 이하면 반환
			if (!CheckLevel(level))
				return;
			lock (this)
			{
				// 시스템 ID에 따른 
				LogFile handle = getFileHandle(systemID);
				writeLog(handle, level, systemID, time, msg);
				checkMaxSize(handle);
			}
		}

		public override void Close()
		{
			if (fileTbl.Count == 0)
				return;

			ArrayList array = new ArrayList(fileTbl.Keys);

			//	close 내부에서 fileTbl.Remove()를 호출하기 때문에 아래 코드로 하면 에러가 발생한다.
			//  fileTbl.Keys가 내부 코드에서 변경되기 때문!!			
			//			foreach(string systemID in fileTbl.Keys)
			for (int i = 0; i < array.Count; i++)
			{
				string systemID = (string)array[i];
				close(systemID);
			}
		}

		#endregion

		/// <summary>
		/// 해당 systemID의 로그파일 핸들을 반환한다.
		/// </summary>
		/// <param name="systemID"></param>
		/// <returns></returns>
		private LogFile getFileHandle(string systemID)
		{
			LogFile entry = (LogFile)fileTbl[systemID];

			// 새로운 시스템일 경우			
			if (entry == null)
			{
				entry = CreateLogFile(systemID);
				// 새로운 항목 등록
				fileTbl.Add(systemID, entry);
			}

			// 정책에의해 경로명이 바뀌었으면 기존 파일을 닫고 새로운 경로에 파일을 만든다.						
			if (!entry.Path.StartsWith(GetLogDirectory()))
			{
				close(systemID);

				entry = CreateLogFile(systemID);
				// 기존 항목을 대체
				fileTbl[systemID] = entry;
			}

			return entry;
		}

		/// <summary>
		/// systemID의 파일을 닫는다.
		/// </summary>
		/// <param name="systemID"></param>
		private void close(string systemID)
		{
			LogFile entry = (LogFile)fileTbl[systemID];
			entry.SW.Close();
			// 닫은 파일은 파일테이블에서 제거한다.
			fileTbl.Remove(systemID);
		}

		/// <summary>
		/// 해당 로그파일의 사이즈가 최대치를 넘었는지 검사한다.
		/// 최대치를 넘을 경우는 기존 파일을 백업하고 로그 파일을 다시 연다.
		/// </summary>
		/// <param name="handle"></param>
		private void checkMaxSize(LogFile handle)
		{
			if (maxSize == 0)
				return;

			// 파일 사이즈 검사			
			if (handle.CurrentBytes >= maxSize)
			{
				//lock added by Kim Hanyoung, 2005-06-13
				lock (this)
				{
					// 기존 파일 닫기
					close(handle.SystemID);

					// 파일 백업
					FileInfo info = new FileInfo(handle.Path);
					info.MoveTo(getBackupFilePath(handle.Path));

					// 파일 다시 열기
					LogFile entry = CreateLogFile(handle.SystemID);

					// 새로운 파일로 기존 핸들 변경
					fileTbl.Remove(entry.SystemID);
					fileTbl.Add(entry.SystemID, entry);
				}
			}
		}

		/// <summary>
		/// 로그파일의 백업이름을 반환한다.
		/// ex) systemID-im.log -> systemID-im2002-08-12-12-21-23.log
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string getBackupFilePath(string path)
		{
			string postFix = "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
			int pos = path.LastIndexOf(".");
			return path.Insert(pos, postFix);
		}

		/// <summary>
		/// 파일에 실제로 로그를 쓴다.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="level"></param>
		/// <param name="systemID"></param>
		/// <param name="time"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private void writeLog(LogFile handle, LogLevel level, string systemID, DateTime time, string message)
		{
			//lock added by Kim Hanyoung, 2005-06-13
			lock (this)
			{
				string msg = FormatMessage(level, systemID, time, message);
				handle.SW.WriteLine(msg);
				handle.SW.Flush();
				handle.CurrentBytes += (msg.Length + 2); // \r\n추가			
			}
		}
	}
}
