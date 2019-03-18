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
		/// ������ MQ�� ���� �޽����� �����Ѵ�.
		/// </summary>
		/// <remarks>
		/// nets.config�� /config/log/channel/{ä�� �̸�}�� ���ǵ� �޽���ť�� ���� �α׸� �����Ѵ�.
		/// �޽����� ������ �ʿ��� ��� '|' ���ڸ� �����ڷ� ����Ѵ�.
		/// 
		/// �⺻������ ���۵Ǵ� �޽����� �տ� ������ ���� ��������� �ٴ´�.
		/// {�α� ����} | {���� �ð�} | {�ý��� ID}
		/// 
		/// ��) �α׿� ���� �޽��� "IT|test|127.0.0.1"�� �����ϴ� ���
		///     : INFORMATION|2003-09-08 10:20:22|SSO|IT|test|127.0.0.1
		/// </remarks>
		/// <param name="channelName">�α� ä�� �̸�</param>
		/// <param name="level">�α� ����</param>
		/// <param name="systemID">�α׸� �����ϴ� ���� �ý���</param>
		/// <param name="time">�α� �߻� �ð�</param>
		/// <param name="msg">���� �޽���</param>
		public override void Log(string channelName, LogLevel level, string systemID, DateTime time, string msg)
		{
			// �ּ� �α׷��� ���ϸ� ��ȯ
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
		/// �α� ä���� �ݴ´�. (MQ������ ������)
		/// </summary>
		public override void Close()
		{
		}

		#endregion
	}
}