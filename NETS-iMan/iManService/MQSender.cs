using System;
using System.Configuration;
using System.Messaging;
using System.Text;
using Nets.IM.Common;

namespace iManService
{
	public class MQSender
	{
		private const string delim = ":";
		private static readonly ActiveXMessageFormatter formatter;
		private static readonly MessageQueue sMQ;
		private static readonly MessageQueue rMQ;

		static MQSender()
		{
			string path = ConfigurationManager.AppSettings["mqpath"] ?? @"FormatName:Direct=OS:.\Private$\iManQueue";
			sMQ = new MessageQueue(path);
			rMQ = new MessageQueue(path);
			formatter = new ActiveXMessageFormatter();
		}

		public static void Send(DateTime time, string from, string to, string msg)
		{
			lock (typeof (MQSender))
			{
				try
				{
					using (MessageQueueTransaction mqTran = new MessageQueueTransaction())
					{
						mqTran.Begin();

						if ((sMQ.Formatter == null) || (sMQ.Formatter.GetType() != formatter.GetType()))
							sMQ.Formatter = formatter;

						sMQ.Send(time.ToString("yyyy-MM-dd HH:mm:ss") + delim + from + delim + msg, to, mqTran);

						mqTran.Commit();
					}
				}
				catch (Exception ex)
				{
					Logger.Log("iManService", LogLevel.WARNING, "ChatService", "MQSender-Send(): " + ex);
				}
			}
		}

		public static string Receive(string strUser)
		{
			//Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "MQSender-Receive(): MQ receiving... " + strUser);

			StringBuilder sb = new StringBuilder();
			try
			{
				MessageEnumerator mEnum = rMQ.GetMessageEnumerator2();
				bool msgExist = mEnum.MoveNext();
				mEnum.Close();
				//Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "MQSender-Receive(): MQ msgExist: " + msgExist);

				if (msgExist)
				{
					bool loopOption;
					do
					{
						loopOption = false;
						using (Cursor cs = rMQ.CreateCursor())
						{
							try
							{
								Message msg = rMQ.Peek(new TimeSpan(TimeSpan.TicksPerMillisecond), cs, PeekAction.Current);
								while (msg != null)
								{
									try
									{
										//Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "MQSender-Receive(): MQ msg.Label: " + msg.Label);
										if ((msg.Label == strUser) || (strUser.IndexOf("(" + msg.Label + ")") >= 0))
										{
											if (sb.Length > 0) sb.Append("\n\n\t\n");
											using (MessageQueueTransaction mqTran = new MessageQueueTransaction())
											{
												mqTran.Begin();

												if ((rMQ.Formatter == null) || (rMQ.Formatter.GetType() != formatter.GetType()))
													rMQ.Formatter = formatter;

												msg = rMQ.Receive(new TimeSpan(TimeSpan.TicksPerSecond), cs, mqTran);
												string body = msg != null ? (string) msg.Body : "";
												sb.Append(body);

												mqTran.Commit();
												Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "MQSender-Receive(): MQ msg received: " + body);
											}
										}
										if (msg != null) msg.Dispose();
										msg = rMQ.Peek(new TimeSpan(TimeSpan.TicksPerMillisecond), cs, PeekAction.Next);
									}
									catch (MessageQueueException e)
									{
										// 큐에 메시지가 없으면 끝낸다.
										if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) break;

										// 메시지를 받았으면 처음부터 다시 검색한다.
										if (e.MessageQueueErrorCode == MessageQueueErrorCode.IllegalCursorAction)
										{
											loopOption = true;
											break;
										}

										Logger.Log("iManService", LogLevel.WARNING, "ChatService",
										           "MQSender-Receive(): " + e.MessageQueueErrorCode + ": " + e.Message);
									}
								}
								if (msg != null) msg.Dispose();
							}
							catch (Exception ex)
							{
								Logger.Log("iManService", LogLevel.WARNING, "ChatService", "MQSender-Receive(): " + ex);
							}
							cs.Close();
						}
					} while (loopOption);
				}
			}
			catch (Exception ex)
			{
				Logger.Log("iManService", LogLevel.WARNING, "ChatService", "MQSender-Receive(): " + ex);
			}
			return sb.ToString();
		}
	}
}