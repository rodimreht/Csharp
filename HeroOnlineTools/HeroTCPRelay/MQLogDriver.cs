using System;
using System.Messaging;

namespace HeroTCPRelay
{
	/// <summary>
	/// Summary description for MQLogDriver. (from NETS*IM 2.6)
	/// </summary>
	public class MQLogDriver : LogDriver
	{
		private MessageQueue mq;
		private ActiveXMessageFormatter formatter;

		public override void initialize(string level, string path, string filename, int maxsize, string period, string encoding, bool useTransaction)
		{
			base.initialize( level, path, filename, maxsize, period, encoding, useTransaction );
			mq = new MessageQueue( path );
			formatter = new ActiveXMessageFormatter();
		}

		#region LogDriver Members

		/// <summary>
		/// 지정된 MQ를 통해 메시지를 전송한다.
		/// </summary>
		/// <remarks>
		/// nets.config의 /config/log/channel/{채널 이름}에 정의된 메시지큐를 통해 로그를 전송한다.
		/// 메시지는 구분이 필요한 경우 '|' 문자를 구분자로 사용한다.
		/// 
		/// 기본적으로 전송되는 메시지의 앞에 다음과 같은 헤더정보가 붙는다.
		/// {로그 레벨} | {전송 시각} | {시스템 ID}
		/// 
		/// 예) 로그온 성공 메시지 "IT|test|127.0.0.1"를 전송하는 경우
		///     : INFORMATION|2003-09-08 10:20:22|SSO|IT|test|127.0.0.1
		/// </remarks>
		/// <param name="channelName">로그 채널 이름</param>
		/// <param name="level">로그 레벨</param>
		/// <param name="systemID">로그를 전송하는 원본 시스템</param>
		/// <param name="time">로그 발생 시각</param>
		/// <param name="msg">전송 메시지</param>
		public override void Log(string channelName, LogLevel level, string systemID, DateTime time, string msg)
		{
			// 최소 로그레벨 이하면 반환
			if ( !CheckLevel( level ) )
				return;

			lock (this)
			{
				try
				{
					if ( useTransaction )
					{
						using (MessageQueueTransaction mqTran = new MessageQueueTransaction())
						{
							mqTran.Begin();

							if ( (mq.Formatter==null) || (mq.Formatter.GetType()!=formatter.GetType()) )
								mq.Formatter = formatter;

							mq.Send( level.ToString() + delim +
								time.ToString( "yyyy-MM-dd HH:mm:ss" ) + delim + systemID + delim + msg, systemID, mqTran );

							mqTran.Commit();
						}
					}
					else
					{
						if ( (mq.Formatter==null) || (mq.Formatter.GetType()!=formatter.GetType()) )
							mq.Formatter = formatter;

						mq.Send( level.ToString() + delim + systemID + delim +
							time.ToString("yyyy-MM-dd HH:mm:ss") + delim + msg, systemID);
					}
				}
				catch (Exception e)
				{
					Logger.Log( "FILE", LogLevel.ERROR, systemID, time,
					            FormatMessage( level, systemID, time, msg + " ==> " + e.ToString() ) );
				}
			}
		}

		public override string GetPath(string systemID)
		{
			return mq.Path;
		}

		/// <summary>
		/// 로그 채널을 닫는다. (MQ에서는 사용안함)
		/// </summary>
		public override void Close()
		{
		}

		#endregion
	}
}