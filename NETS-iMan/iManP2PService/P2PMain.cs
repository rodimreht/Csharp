using System;
using System.Collections;
using System.Configuration;
using System.Net.Sockets;
using System.Text;

namespace iManP2PService
{
	public class P2PMain
	{
		private static readonly ArrayList pairList = ArrayList.Synchronized(new ArrayList());
		private ServerSocket mainSocket;

		private void mainSocket_OnError(string errorMessage, int index, Socket socket, int errorCode)
		{
			try
			{
				if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
				{
					Logger.Log("[ServerSocket_OnError] 연결 끊김[" + index + "]: " + socket.RemoteEndPoint);
					if (index != -1) cleanUp(index);
					return;
				}

				if (socket == null)
				{
					Logger.Log("[ServerSocket_OnError] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
				}
				else
				{
					Logger.Log("[ServerSocket_OnError] 알 수 없는 에러 발생[" + index + "]: " + socket.RemoteEndPoint + "(에러코드: " + errorCode +
							   ", 메시지: " + errorMessage + ")");
				}

				if (index != -1) cleanUp(index);

			}
			catch (Exception ex)
			{
				Logger.Log("[ServerSocket_OnError] " + ex);
			}
		}

		private void mainSocket_OnRead(int index, Socket socket, byte[] bytes)
		{
			try
			{
				if ((bytes == null) || (bytes.Length == 0))
				{
					if (index != -1) cleanUp(index);
					return;
				}

				if (bytes.Length < 128)
				{
					// 플래그 확인
					// bytes=<file>RECEIVER|{userid}: 수신자 등록
					// bytes=<file>SENDER|{target_userid}: 송신자 등록
					string s = Encoding.Default.GetString(bytes);
					if (s.StartsWith("<file>"))
					{
						Logger.Log("[ServerSocket] 받은 메시지: " + s);

						s = s.Substring(6);
						if (s.StartsWith("RECEIVER"))
						{
							string[] arr = s.Split(new char[] { '|' });
							P2PPair pair = new P2PPair(arr[1], index);
							lock (pairList)
							{
								pairList.Add(pair);
							}
						}
						else if (s.StartsWith("SENDER"))
						{
							bool pairFound = false;
							string[] arr = s.Split(new char[] { '|' });
							lock (pairList)
							{
								foreach (P2PPair pair in pairList)
								{
									if ((pair.Receiver != arr[1]) || (pair.SenderIndex != -1)) continue;

									pair.SenderIndex = index;
									pairFound = true;

									mainSocket.SendText("<file>OK", index);
									Logger.Log("서버 경유 채널 연결 완료([S:" + index + "] --> [R:" + pair.ReceiverIndex + "])");
									break;
								}
							}

							// 수신자가 없는 경우 송신자 연결을 바로 끊는다.
							if (!pairFound) mainSocket.CloseConnection(index);
						}
						return;
					}
				}

				foreach (P2PPair pair in pairList)
				{
					if (pair.SenderIndex != index) continue;

					// 송신자의 패킷을 수신자의 소켓으로 그대로 쏜다.
					if (mainSocket.Connected(pair.ReceiverIndex))
						mainSocket.Send(bytes, pair.ReceiverIndex);
					break;
				}

			}
			catch (Exception ex)
			{
				Logger.Log("[ServerSocket_OnRead] " + ex);
			}
		}

		private void mainSocket_OnConnect(int index, Socket socket)
		{
			Logger.Log("[ServerSocket] 연결됨[" + index + "]: " + socket.RemoteEndPoint);
		}

		private void mainSocket_OnDisconnect(int index, Socket socket)
		{
			/*
			Logger.Log("[ServerSocket_OnDisconnect] 연결 끊김[" + index + "]: " + socket.RemoteEndPoint);
			*/
			if (index != -1) cleanUp(index);
		}

		public void Start()
		{
			Logger.Log("서비스가 시작되었습니다.");

			string port = ConfigurationManager.AppSettings["listenPort"];
			int iPort = string.IsNullOrEmpty(port) ? 443 : int.Parse(port);

			mainSocket = new ServerSocket(iPort);
			mainSocket.OnConnect += mainSocket_OnConnect;
			mainSocket.OnDisconnect += mainSocket_OnDisconnect;
			mainSocket.OnRead += mainSocket_OnRead;
			mainSocket.OnError += mainSocket_OnError;
			mainSocket.Listen();
		}

		public void Stop()
		{
			try
			{
				for (int i = 0; i < mainSocket.ActiveConnections; i++)
				{
					if (mainSocket.Connected(i)) mainSocket.CloseConnection(i);
				}
				mainSocket.Close();
				mainSocket = null;
			}
			catch (Exception)
			{
			}
			Logger.Log("서비스가 중지되었습니다. 실행 중인 프로세스는 모두 중지되었습니다.");
		}

		private void cleanUp(int index)
		{
			try
			{
				lock (pairList)
				{
					bool removed;
					do
					{
						removed = false;
						foreach (P2PPair pair in pairList)
						{
							if (pair.ReceiverIndex == index)
							{
								if ((pair.SenderIndex != -1) && mainSocket.Connected(pair.SenderIndex))
									mainSocket.CloseConnection(pair.SenderIndex);

								pairList.Remove(pair);
								Logger.Log("서버 경유 채널 해제 완료([S:" + pair.SenderIndex + "] <-- [R:" + index + "])");
								removed = true;
								break;
							}
							
							if (pair.SenderIndex == index)
							{
								if (mainSocket.Connected(pair.ReceiverIndex))
									mainSocket.SendText("<file>DISCONNECT", pair.ReceiverIndex);
								else
								{
									pairList.Remove(pair);
									Logger.Log("서버 경유 채널 해제 완료([S:" + index + "] --> [R:" + pair.ReceiverIndex + "])");
									removed = true;
									break;
								}
							}
						}
					} while (removed); 
				}
			}
			catch (Exception ex)
			{
				Logger.Log("cleanUp(" + index + ") - " + ex);
			}
		}

		#region Nested type: P2PPair

		private class P2PPair
		{
			private readonly string m_receiver;
			private readonly int m_to = -1;
			private int m_from = -1;

			public P2PPair(string userid, int index)
			{
				m_receiver = userid;
				m_to = index;
			}

			public string Receiver
			{
				get { return m_receiver; }
			}

			public int SenderIndex
			{
				get { return m_from; }
				set { m_from = value; }
			}

			public int ReceiverIndex
			{
				get { return m_to; }
			}
		}

		#endregion
	}
}
