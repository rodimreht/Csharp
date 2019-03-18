using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HeroTCPRelay
{
	public partial class frmMain : Form
	{
		private const int BASE_PORT = 12382;

		private ServerSocket[] serverSocks;
		private ClientSocket[] clientSocks;

		private object lockObj = new object();
		private int SOCK_COUNT = 0;

		private volatile bool isfreezing = false;
		private volatile bool isfreezingR = false;

		private static string[] SERVERS = {
		                                  	"222.235.66.16",
		                                  	"222.235.66.17",
		                                  	"222.235.66.18",
		                                  	"222.235.66.19"
		                                  };

		private static int[] PORTS = {
		                             	15000,
		                             	15010,
		                             	15020
		                             };

		private ServerSocket SESS_SSock;
		private ClientSocket SESS_CSock;
		private static string SESS_SERVER = "222.235.66.43";
		private static int SESS_PORT = 15001;

		private static string TEMP_CSockIP = null;
		private static int TEMP_CSockPort = 0;
		private static string TEMP_MCSockIP = null;
		private static int TEMP_MCSockPort = 0;
		private static int TEMP_SSockPort = 0;
		private static int TEMP_MSSockPort = 0;

		public frmMain()
		{
			InitializeComponent();
		}

		#region SessionServerSocket 이벤트

		void SESS_SSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
			{
				Logger.Log(LogLevel.WARNING, "HeroTCPRelay2", "[로컬세션] 연결 끊김: " + socket.RemoteEndPoint);
				return;
			}

			if (socket == null || !socket.Connected)
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay2", "[로컬세션] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
			}
			else
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay2", "[로컬세션] 알 수 없는 에러 발생: " + socket.RemoteEndPoint + "(에러코드: " + errorCode + ", 메시지: " + errorMessage + ")");
			}
		}

		void SESS_SSock_OnRead(Socket socket)
		{
			byte[] bytes = SESS_SSock.ReceivedBytes;

			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay2", "[로컬세션] 보낸 메시지: " + getStringHex(bytes));

			if (SESS_CSock.Connected)
			{
				lock (clientSocks)
				{
					SESS_CSock.WriteBytes = bytes;
					SESS_CSock.Send();
				}
			}
			else
			{
				if (SESS_CSock.Connecting)
				{
					while (!SESS_CSock.Connected)
						Thread.Sleep(50);

					lock (clientSocks)
					{
						SESS_CSock.WriteBytes = bytes;
						SESS_CSock.Send();
					}
				}
				else
				{
					lock (clientSocks)
					{
						SESS_CSock.WriteBytes = bytes;
						SESS_CSock.Connect();
					}
				}
			}
		}

		void SESS_SSock_OnConnect(Socket socket)
		{
			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay2", "[로컬세션] TCP 연결됨: " + socket.LocalEndPoint);
		}

		void SESS_SSock_OnDisconnect(Socket socket)
		{
			if (SESS_SSock == null) return;

			bool found = false;
			for (int k = 0; k < SESS_SSock.ActiveConnections; k++)
			{
				if (!SESS_SSock.Connected(k) && (SESS_SSock.Port > 0))
				{
					Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay2", "[로컬세션] 연결끊김: 127.0.0.1:" + SESS_SSock.Port);
					if (SESS_CSock != null) SESS_CSock.Disconnect();
					found = true;
					break;
				}
			}
			if (found)
			{
				SESS_CSock = new ClientSocket(SESS_SERVER, SESS_PORT);
				SESS_CSock.OnConnect += new ClientSocket.ConnectionEventHandler(clientSock_OnConnect);
				SESS_CSock.OnError += new ClientSocket.ErrorEventHandler(clientSock_OnError);
				SESS_CSock.OnRead += new ClientSocket.ConnectionEventHandler(clientSock_OnRead);
				SESS_CSock.OnDisconnect += new ClientSocket.ConnectionEventHandler(clientSock_OnDisconnect);
			}
		}

		#endregion


		#region ServerSocket 이벤트

		void serverSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
			{
				Logger.Log(LogLevel.WARNING, "HeroTCPRelay", "[로컬연결] 연결 끊김: " + socket.RemoteEndPoint);
				return;
			}

			if (socket == null || !socket.Connected)
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay", "[로컬연결] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
			}
			else
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay", "[로컬연결] 알 수 없는 에러 발생: " + socket.RemoteEndPoint + "(에러코드: " + errorCode + ", 메시지: " + errorMessage + ")");
			}
		}

		void serverSock_OnRead(Socket socket)
		{
			int bindPort = ((IPEndPoint)socket.LocalEndPoint).Port;
			int cnt = bindPort - BASE_PORT;
			byte[] bytes = serverSocks[cnt].ReceivedBytes;

			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "[로컬연결] 보낸 메시지: " + getStringHex(bytes));

			if (clientSocks[cnt].Connected)
			{
				lock (clientSocks)
				{
					clientSocks[cnt].WriteBytes = bytes;
					clientSocks[cnt].Send();
				}
			}
			else
			{
				if (clientSocks[cnt].Connecting)
				{
					while (!clientSocks[cnt].Connected)
						Thread.Sleep(50);

					lock (clientSocks)
					{
						clientSocks[cnt].WriteBytes = bytes;
						clientSocks[cnt].Send();
					}
				}
				else
				{
					lock (clientSocks)
					{
						clientSocks[cnt].WriteBytes = bytes;
						clientSocks[cnt].Connect();
					}
				}
			}
		}

		void serverSock_OnConnect(Socket socket)
		{
			TEMP_SSockPort = ((IPEndPoint) socket.LocalEndPoint).Port;
			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "[로컬연결] TCP 연결됨: " + socket.LocalEndPoint);
		}

		void serverSock_OnDisconnect(Socket socket)
		{
			for (int i = 0; i < serverSocks.Length; i++)
			{
				ServerSocket sock = serverSocks[i];
				if (sock == null) continue;

				bool found = false;
				for (int k = 0; k < sock.ActiveConnections; k++)
				{
					if (!sock.Connected(k) && (sock.Port == TEMP_SSockPort))
					{
						TEMP_SSockPort = 0;
						Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "[로컬연결] 연결끊김: 127.0.0.1:" + sock.Port);
						if (clientSocks[i] != null) clientSocks[i].Disconnect();
						found = true;
						break;
					}
				}
				if (found)
				{
					clientSocks[i] = new ClientSocket(SERVERS[i / 3], PORTS[i % 3]);
					clientSocks[i].OnConnect += new ClientSocket.ConnectionEventHandler(clientSock_OnConnect);
					clientSocks[i].OnError += new ClientSocket.ErrorEventHandler(clientSock_OnError);
					clientSocks[i].OnRead += new ClientSocket.ConnectionEventHandler(clientSock_OnRead);
					clientSocks[i].OnDisconnect += new ClientSocket.ConnectionEventHandler(clientSock_OnDisconnect);
					break;
				}
			}
		}

		#endregion

		#region MainServerSocket 이벤트

		void mainServerSock_OnRead(Socket socket)
		{
			int bindPort = ((IPEndPoint)socket.LocalEndPoint).Port;
			int cnt = bindPort - BASE_PORT;
			byte[] bytes = serverSocks[cnt].ReceivedBytes;

			// 로그 생략 체크 (화면에 표시하기에 별 의미없다고 판단되는 패킷 데이터 필터링)
			if (!HeroCommon.IsLogFiltered(bytes))
			{
				if (!isfreezing)
				{
					lblPacket.Text = getStringHex(bytes, 48);
					lblPacket.Refresh();
				}

				// 무공공격 반복횟수 조정
				//bytes = HeroCommon.DuplicateMessage(bytes, (int)attackCounter.Value);
				if (HeroCommon.IsDuplicatable(bytes))
					Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "보낸 메시지: " + ToString(bytes));
			}
			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "보낸 메시지: " + getStringHex(bytes));

			// 무공공격 반복횟수 조정
			//int times = HeroCommon.IsDuplicatable(bytes) ? (int)attackCounter.Value : 1;
			//for (int i = 0; i < times; i++)
			{
				if (clientSocks[cnt].Connected)
				{
					lock (clientSocks)
					{
						clientSocks[cnt].WriteBytes = bytes;
						clientSocks[cnt].Send();
					}
				}
				else
				{
					if (clientSocks[cnt].Connecting)
					{
						while (!clientSocks[cnt].Connected)
							Thread.Sleep(50);

						lock (clientSocks)
						{
							clientSocks[cnt].WriteBytes = bytes;
							clientSocks[cnt].Send();
						}
					}
					else
					{
						lock (clientSocks)
						{
							clientSocks[cnt].WriteBytes = bytes;
							clientSocks[cnt].Connect();
						}
					}
				}
			}
		}

		void mainServerSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
			{
				Logger.Log(LogLevel.WARNING, "HeroTCPRelay", "[주로컬연결] 연결 끊김: " + socket.RemoteEndPoint);
				return;
			}

			if (socket == null || !socket.Connected)
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay", "[주로컬연결] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
			}
			else
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay", "[주로컬연결] 알 수 없는 에러 발생: " + socket.RemoteEndPoint + "(에러코드: " + errorCode + ", 메시지: " + errorMessage + ")");
			}
		}

		void mainServerSock_OnConnect(Socket socket)
		{
			TEMP_MSSockPort = ((IPEndPoint)socket.LocalEndPoint).Port;
			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "[주로컬연결] TCP 연결됨: " + socket.LocalEndPoint);
		}

		void mainServerSock_OnDisconnect(Socket socket)
		{
			for (int i = 0; i < serverSocks.Length; i++)
			{
				ServerSocket sock = serverSocks[i];
				if (sock == null) continue;

				for (int k = 0; k < sock.ActiveConnections; k++)
				{
					if (!sock.Connected(k) && (sock.Port == TEMP_MSSockPort))
					{
						TEMP_MSSockPort = 0;
						Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "[주로컬연결] 연결끊김: 127.0.0.1:" + sock.Port);
						if (clientSocks[i] != null) clientSocks[i].Disconnect();
						break;
					}
				}
			}
		}

		#endregion

		#region SessionClientSocket 이벤트

		void SESS_CSock_OnRead(Socket socket)
		{
			IPEndPoint endPoint = (IPEndPoint) socket.RemoteEndPoint;
			string server = endPoint.Address.ToString();
			int port = endPoint.Port;

			if (SESS_CSock.IP.Equals(server) && SESS_CSock.Port.Equals(port))
			{
				byte[] bytes = SESS_CSock.ReceivedBytes;
				Logger.Log(LogLevel.DEBUG, "HeroTCPRelay2", "[원격세션] 받은 메시지: " + getStringHex(bytes));

				for (int k = 0; k < SESS_SSock.ActiveConnections; k++)
					if (SESS_SSock.Connected(k)) SESS_SSock.Send(bytes, k);
			}
		}

		void SESS_CSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			try
			{
				if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
				{
					Logger.Log(LogLevel.WARNING, "HeroTCPRelay2", "[원격세션] 연결 끊김: " + socket.RemoteEndPoint);
					return;
				}

				if (socket == null || !socket.Connected)
				{
					Logger.Log(LogLevel.ERROR, "HeroTCPRelay2",
							   "[원격세션] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
				}
				else
				{
					Logger.Log(LogLevel.ERROR, "HeroTCPRelay2",
							   "[원격세션] 알 수 없는 에러 발생: " + socket.RemoteEndPoint + "(에러코드: " + errorCode + ", 메시지: " +
							   errorMessage + ")");
				}
			}
			catch (Exception ex)
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay2",
						   "[원격세션] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + "): " + ex);
			}
		}

		void SESS_CSock_OnConnect(Socket socket)
		{
			IPEndPoint endPoint = (IPEndPoint)socket.RemoteEndPoint;
			string server = endPoint.Address.ToString();
			int port = endPoint.Port;

			if (SESS_CSock.IP.Equals(server) && SESS_CSock.Port.Equals(port))
			{
				Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay2", "[원격세션] TCP 연결됨: " + socket.RemoteEndPoint);

				SESS_CSock.Send();
			}
		}

		void SESS_CSock_OnDisconnect(Socket socket)
		{
			if (!SESS_CSock.Connected && (SESS_CSock.IP.Length > 0))
			{
				Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay2", "[원격세션] 연결끊김: " + SESS_CSock.IP + ":" + SESS_CSock.Port);
				SESS_SSock.Close();

				SESS_SSock = new ServerSocket(SESS_PORT);
				SESS_SSock.OnConnect += new ServerSocket.ConnectionEventHandler(serverSock_OnConnect);
				SESS_SSock.OnRead += new ServerSocket.ConnectionEventHandler(serverSock_OnRead);
				SESS_SSock.OnError += new ServerSocket.ErrorEventHandler(serverSock_OnError);
				SESS_SSock.OnDisconnect += new ServerSocket.ConnectionEventHandler(serverSock_OnDisconnect);
				SESS_SSock.Listen();
			}
		}

		#endregion

		#region ClientSocket 이벤트

		void clientSock_OnRead(Socket socket)
		{
			IPEndPoint endPoint = (IPEndPoint)socket.RemoteEndPoint;
			string server = endPoint.Address.ToString();
			int port = endPoint.Port;
			for (int i = 0; i < clientSocks.Length; i++)
			{
				ClientSocket sock = clientSocks[i];
				if (sock == null) continue;

				if (sock.IP.Equals(server) && sock.Port.Equals(port))
				{
					byte[] bytes = sock.ReceivedBytes;
					Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "[원격연결] 받은 메시지: " + getStringHex(bytes));

					int pos = HeroCommon.CheckMainConnect(bytes);
					if (pos >= 0)
					{
						changeServer(serverSocks[i], bytes, pos);

						lock (lockObj)
						{
							SOCK_COUNT++;
							serverSocks[SOCK_COUNT] = new ServerSocket(BASE_PORT + SOCK_COUNT);
							serverSocks[SOCK_COUNT].OnConnect += new ServerSocket.ConnectionEventHandler(mainServerSock_OnConnect);
							serverSocks[SOCK_COUNT].OnRead += new ServerSocket.ConnectionEventHandler(mainServerSock_OnRead);
							serverSocks[SOCK_COUNT].OnError += new ServerSocket.ErrorEventHandler(mainServerSock_OnError);
							serverSocks[SOCK_COUNT].OnDisconnect += new ServerSocket.ConnectionEventHandler(mainServerSock_OnDisconnect);
							serverSocks[SOCK_COUNT].Listen();
						}
					}
					else
					{
						for (int k = 0; k < serverSocks[i].ActiveConnections; k++)
							if (serverSocks[i].Connected(k)) serverSocks[i].Send(bytes, k);
					}

					break;
				}
			}
		}

		void clientSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			try
			{
				if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
				{
					Logger.Log(LogLevel.WARNING, "HeroTCPRelay", "[원격연결] 연결 끊김: " + socket.RemoteEndPoint);
					return;
				}

				if (socket == null || !socket.Connected)
				{
					Logger.Log(LogLevel.ERROR, "HeroTCPRelay",
							   "[원격연결] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
				}
				else
				{
					Logger.Log(LogLevel.ERROR, "HeroTCPRelay",
							   "[원격연결] 알 수 없는 에러 발생: " + socket.RemoteEndPoint + "(에러코드: " + errorCode + ", 메시지: " +
							   errorMessage + ")");
				}
			}
			catch (Exception ex)
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay",
						   "[원격연결] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + "): " + ex);
			}
		}

		void clientSock_OnConnect(Socket socket)
		{
			IPEndPoint endPoint = (IPEndPoint)socket.RemoteEndPoint;
			string server = endPoint.Address.ToString();
			int port = endPoint.Port;
			TEMP_CSockIP = server;
			TEMP_CSockPort = port;
			for (int i = 0; i < clientSocks.Length; i++)
			{
				ClientSocket sock = clientSocks[i];
				if (sock == null) continue;

				if (sock.IP.Equals(server) && sock.Port.Equals(port))
				{
					Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "[원격연결] TCP 연결됨: " + socket.RemoteEndPoint);

					sock.Send();

					break;
				}
			}
		}

		void clientSock_OnDisconnect(Socket socket)
		{
			for (int i = 0; i < clientSocks.Length; i++)
			{
				ClientSocket sock = clientSocks[i];
				if (sock == null) continue;

				if (!sock.Connected && (sock.IP == TEMP_CSockIP) && (sock.Port == TEMP_CSockPort))
				{
					TEMP_CSockIP = null;
					TEMP_CSockPort = 0;

					Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "[원격연결] 연결끊김: " + sock.IP + ":" + sock.Port);
					serverSocks[i].Close();

					serverSocks[i] = new ServerSocket(BASE_PORT + i);
					serverSocks[i].OnConnect += new ServerSocket.ConnectionEventHandler(serverSock_OnConnect);
					serverSocks[i].OnRead += new ServerSocket.ConnectionEventHandler(serverSock_OnRead);
					serverSocks[i].OnError += new ServerSocket.ErrorEventHandler(serverSock_OnError);
					serverSocks[i].OnDisconnect += new ServerSocket.ConnectionEventHandler(serverSock_OnDisconnect);
					serverSocks[i].Listen();
					break;
				}
			}
		}

		#endregion

		#region MainClientSocket 이벤트

		void mainClientSock_OnRead(Socket socket)
		{
			IPEndPoint endPoint = (IPEndPoint) socket.RemoteEndPoint;
			string server = endPoint.Address.ToString();
			int port = endPoint.Port;
			for (int i = 0; i < clientSocks.Length; i++)
			{
				ClientSocket sock = clientSocks[i];
				if (sock == null) continue;

				if (sock.IP.Equals(server) && sock.Port.Equals(port))
				{
					byte[] bytes = sock.ReceivedBytes;
					// 로그 생략 체크 (화면에 표시하기에 별 의미없다고 판단되는 패킷 데이터 필터링)
					if (!HeroCommon.IsLogFilteredR(bytes))
					{
						if (!isfreezingR)
						{
							lblPacketR.Text = getStringHex(bytes, 48);
							lblPacketR.Refresh();
						}
					}
					Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "받은 메시지: " + getStringHex(bytes));

					int pos = HeroCommon.CheckMainConnect(bytes);
					if (pos >= 0)
					{
						changeServer(serverSocks[i], bytes, pos);

						lock (lockObj)
						{
							SOCK_COUNT++;
							serverSocks[SOCK_COUNT] = new ServerSocket(BASE_PORT + SOCK_COUNT);
							serverSocks[SOCK_COUNT].OnConnect += new ServerSocket.ConnectionEventHandler(mainServerSock_OnConnect);
							serverSocks[SOCK_COUNT].OnRead += new ServerSocket.ConnectionEventHandler(mainServerSock_OnRead);
							serverSocks[SOCK_COUNT].OnError += new ServerSocket.ErrorEventHandler(mainServerSock_OnError);
							serverSocks[SOCK_COUNT].OnDisconnect += new ServerSocket.ConnectionEventHandler(mainServerSock_OnDisconnect);
							serverSocks[SOCK_COUNT].Listen();
						}
					}
					else
					{
						for (int k = 0; k < serverSocks[i].ActiveConnections; k++)
							if (serverSocks[i].Connected(k)) serverSocks[i].Send(bytes, k);
					}
					break;
				}
			}
		}

		void mainClientSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			try
			{
				if (errorCode == 10054) // 상대방이 연결을 강제로 끊은 경우
				{
					Logger.Log(LogLevel.WARNING, "HeroTCPRelay", "[주원격연결] 연결 끊김: " + socket.RemoteEndPoint);
					return;
				}

				if (socket == null || !socket.Connected)
				{
					Logger.Log(LogLevel.ERROR, "HeroTCPRelay",
							   "[주원격연결] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + ").");
				}
				else
				{
					Logger.Log(LogLevel.ERROR, "HeroTCPRelay",
							   "[주원격연결] 알 수 없는 에러 발생: " + socket.RemoteEndPoint + "(에러코드: " + errorCode + ", 메시지: " +
							   errorMessage + ")");
				}
			}
			catch (Exception ex)
			{
				Logger.Log(LogLevel.ERROR, "HeroTCPRelay",
						   "[주원격연결] 알 수 없는 에러 발생(에러코드: " + errorCode + ", 메시지: " + errorMessage + "): " + ex);
			}
		}

		void mainClientSock_OnConnect(Socket socket)
		{
			IPEndPoint endPoint = (IPEndPoint)socket.RemoteEndPoint;
			string server = endPoint.Address.ToString();
			int port = endPoint.Port;
			TEMP_MCSockIP = server;
			TEMP_MCSockPort = port;
			for (int i = 0; i < clientSocks.Length; i++)
			{
				ClientSocket sock = clientSocks[i];
				if (sock == null) continue;

				if (sock.IP.Equals(server) && sock.Port.Equals(port))
				{
					Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "[주원격연결] TCP 연결됨: " + socket.RemoteEndPoint);

					sock.Send();

					break;
				}
			}
		}

		void mainClientSock_OnDisconnect(Socket socket)
		{
			for (int i = 0; i < clientSocks.Length; i++)
			{
				ClientSocket sock = clientSocks[i];
				if (sock == null) continue;

				if (!sock.Connected && (sock.IP == TEMP_MCSockIP) && (sock.Port == TEMP_MCSockPort))
				{
					TEMP_MCSockIP = null;
					TEMP_MCSockPort = 0;

					Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "[주원격연결] 연결끊김: " + sock.IP + ":" + sock.Port);
					serverSocks[i].Close();

					clientSocks[i] = null;
					serverSocks[i] = null;
					break;
				}
			}
		}

		#endregion

		private void changeServer(ServerSocket serverSock, byte[] bytes, int pos)
		{
			byte[] newbytes = new byte[bytes.Length - pos];
			Array.Copy(bytes, pos, newbytes, 0, newbytes.Length);

			SERVER_CONNECT header = HeroCommon.ConvertToHeader(newbytes);
			HeroCommon.GetHostFixed(ref header);

			string serverIP = header.server_ip;
			int strLen = header.str_size;
			int serverPort = header.server_port;

			header.server_ip = "127.0.0.1";
			header.server_port = BASE_PORT + SOCK_COUNT;
			header.str_size = (byte)header.server_ip.Length;
			header.block_size = (byte)(header.block_size - strLen + header.str_size);
			Logger.Log(LogLevel.INFORMATION, "HeroTCPRelay", "changeServer()-서버 새 연결 설정: " + serverIP + ":" + serverPort + "를 127.0.0.1:" + header.server_port + "로 변경.");

			HeroCommon.GetNetworkFixed(ref header);
			newbytes = HeroCommon.ConvertToBytes(header);

			byte[] bytes2 = new byte[newbytes.Length + pos];
			if (pos > 0) Array.Copy(bytes, bytes2, pos);
			Array.Copy(newbytes, 0, bytes, pos, newbytes.Length);

			for (int k = 0; k < serverSock.ActiveConnections; k++)
				if (serverSock.Connected(k)) serverSock.Send(newbytes, k);

			clientSocks[SOCK_COUNT] = new ClientSocket(serverIP, serverPort);
			clientSocks[SOCK_COUNT].OnConnect += new ClientSocket.ConnectionEventHandler(mainClientSock_OnConnect);
			clientSocks[SOCK_COUNT].OnError += new ClientSocket.ErrorEventHandler(mainClientSock_OnError);
			clientSocks[SOCK_COUNT].OnRead += new ClientSocket.ConnectionEventHandler(mainClientSock_OnRead);
			clientSocks[SOCK_COUNT].OnDisconnect += new ClientSocket.ConnectionEventHandler(mainClientSock_OnDisconnect);
		}

		private void cmdGo_Click(object sender, EventArgs e)
		{
			//test();
			//return;

			if (cmdGo.Text.Equals("시작"))
			{
				cmdGo.Text = "중지";
				svrCounter.Enabled = false;

				HeroCommon.SetLocalServer();

				// 세션서버#1
				SESS_SSock = new ServerSocket(SESS_PORT);
				SESS_SSock.OnConnect += new ServerSocket.ConnectionEventHandler(SESS_SSock_OnConnect);
				SESS_SSock.OnRead += new ServerSocket.ConnectionEventHandler(SESS_SSock_OnRead);
				SESS_SSock.OnError += new ServerSocket.ErrorEventHandler(SESS_SSock_OnError);
				SESS_SSock.OnDisconnect += new ServerSocket.ConnectionEventHandler(SESS_SSock_OnDisconnect);
				SESS_SSock.Listen();

				SESS_CSock = new ClientSocket(SESS_SERVER, SESS_PORT);
				SESS_CSock.OnConnect += new ClientSocket.ConnectionEventHandler(SESS_CSock_OnConnect);
				SESS_CSock.OnError += new ClientSocket.ErrorEventHandler(SESS_CSock_OnError);
				SESS_CSock.OnRead += new ClientSocket.ConnectionEventHandler(SESS_CSock_OnRead);
				SESS_CSock.OnDisconnect += new ClientSocket.ConnectionEventHandler(SESS_CSock_OnDisconnect);

				int cnt = (int) svrCounter.Value;
				serverSocks = new ServerSocket[cnt + 20];
				clientSocks = new ClientSocket[cnt + 20];
				for (int i = 0; i < cnt; i++)
				{
					serverSocks[i] = new ServerSocket(BASE_PORT + i);
					serverSocks[i].OnConnect += new ServerSocket.ConnectionEventHandler(serverSock_OnConnect);
					serverSocks[i].OnRead += new ServerSocket.ConnectionEventHandler(serverSock_OnRead);
					serverSocks[i].OnError += new ServerSocket.ErrorEventHandler(serverSock_OnError);
					serverSocks[i].OnDisconnect += new ServerSocket.ConnectionEventHandler(serverSock_OnDisconnect);
					serverSocks[i].Listen();

					clientSocks[i] = new ClientSocket(SERVERS[i/3], PORTS[i%3]);
					clientSocks[i].OnConnect += new ClientSocket.ConnectionEventHandler(clientSock_OnConnect);
					clientSocks[i].OnError += new ClientSocket.ErrorEventHandler(clientSock_OnError);
					clientSocks[i].OnRead += new ClientSocket.ConnectionEventHandler(clientSock_OnRead);
					clientSocks[i].OnDisconnect += new ClientSocket.ConnectionEventHandler(clientSock_OnDisconnect);
				}

				lock (lockObj)
				{
					// 메인 게임서버
					SOCK_COUNT = cnt;
					serverSocks[SOCK_COUNT] = new ServerSocket(BASE_PORT + SOCK_COUNT);
					serverSocks[SOCK_COUNT].OnConnect += new ServerSocket.ConnectionEventHandler(mainServerSock_OnConnect);
					serverSocks[SOCK_COUNT].OnRead += new ServerSocket.ConnectionEventHandler(mainServerSock_OnRead);
					serverSocks[SOCK_COUNT].OnError += new ServerSocket.ErrorEventHandler(mainServerSock_OnError);
					serverSocks[SOCK_COUNT].OnDisconnect += new ServerSocket.ConnectionEventHandler(mainServerSock_OnDisconnect);
					serverSocks[SOCK_COUNT].Listen();
				}
			}
			else
			{
				if (SESS_CSock.Connected) SESS_CSock.Disconnect();
				SESS_CSock = null;
				SESS_SSock.Close();
				SESS_SSock = null;

				lock (clientSocks)
				{
					for (int i = 0; i < clientSocks.Length; i++)
					{
						if (clientSocks[i] == null) continue;
						if (clientSocks[i].Connected) clientSocks[i].Disconnect();
						clientSocks[i] = null;
					}
				}

				lock (lockObj)
				{
					for (int i = 0; i < serverSocks.Length; i++)
					{
						if (serverSocks[i] == null) continue;
						serverSocks[i].Close();
						serverSocks[i] = null;
					}
				}

				cmdGo.Text = "시작";
				svrCounter.Enabled = true;

				HeroCommon.SetOriginalServer();
			}
		}

		private static string getStringHex(byte[] bytes)
		{
			return "\r\n" + getStringHex(bytes, bytes.Length);
		}

		private static string getStringHex(byte[] bytes, int length)
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();

			byte prevB = 0x00;
			bool isContinued = false;
			int contPos = -1;
			for (int i = 0; i < bytes.Length; i++)
			{
				if (i >= length) break;
				if (i > 0) prevB = bytes[i - 1];

				// 16진수 표현
				if (i % 16 != 0) sb.Append(" ");
				sb.Append(bytes[i].ToString("X2"));

				// 문자열 표현
				if ((bytes[i] >= 0x20) && (bytes[i] <= 0x7e))
					sb2.Append(Encoding.Default.GetString(bytes, i, 1));
				else
					sb2.Append(".");

				if ((i > 0) && ((i + 1) % 16 == 0))
				{
					sb.Append("\t" + sb2);
					if (i < length - 1) sb.AppendLine();
					sb2 = new StringBuilder();
				}

				if ((prevB == 0x55) && (bytes[i] == 0xAA))
				{
					isContinued = true;
					contPos = i + 1;
					break;
				}
			}

			if (sb2.Length > 0)
			{
				for (int i = sb2.Length; i < 16; i++)
					sb.Append("   ");

				sb.Append("\t" + sb2);
			}

			if (isContinued && (bytes.Length - contPos > 0))
			{
				byte[] newBytes = new byte[bytes.Length - contPos];
				Array.Copy(bytes, contPos, newBytes, 0, newBytes.Length);
				sb.AppendLine();
				sb.Append(getStringHex(newBytes, length - contPos));
			}

			return sb.ToString();
		}

		public static string ToString(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				// 16진수 표현
				if (i > 0) sb.Append(" ");
				sb.Append(bytes[i].ToString("X2"));
			}
			return sb.ToString();
		}

		#region 테스트 코드

		private void test()
		{
			/*
			byte[] bb = {
			            	0xAA, 0x55, 0x15, 0x00, 0x00, 0x05, 0x01, 0x0D,
			            	0x32, 0x32, 0x32, 0x2E, 0x32, 0x33, 0x35, 0x2E,
			            	0x36, 0x36, 0x2E, 0x33, 0x33, 0xB6, 0x3A, 0x00,
			            	0x00, 0x55, 0xAA
			            };
			SERVER_CONNECT header = HeroCommon.ConvertToHeader(bb);
			HeroCommon.GetHostFixed(ref header);
			MessageBox.Show(header.server_ip + ":" + header.server_port);

			HeroCommon.GetNetworkFixed(ref header);
			byte[] bb2 = HeroCommon.ConvertToBytes(header);
			MessageBox.Show(ToString(bb2));
			*/
			/*
			byte[] bb2 = {
							0xAA, 0x55, 0x04, 0x00, 0x51, 0x02, 0x0A, 0x00, 0x55, 0xAA,
							0xAA, 0x55, 0x15, 0x00, 0x09, 0x07, 0x00, 0x0D,
			            	0x32, 0x32, 0x32, 0x2E, 0x32, 0x33, 0x35, 0x2E,
			            	0x36, 0x36, 0x2E, 0x32, 0x39, 0xB6, 0x3A, 0x00,
			            	0x00, 0x55, 0xAA
			            };
			int pos = HeroCommon.CheckMainConnect(bb2);
			MessageBox.Show(pos.ToString());
			*/
			DateTime theDate = DateTime.Now;
			long lngDate = theDate.ToFileTime();
			byte[] bb = BitConverter.GetBytes(lngDate);
			MessageBox.Show(ToString(bb));
		}

		#endregion

		private void frmMain_Load(object sender, EventArgs e)
		{
			int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
			this.Location = new Point(0, screenHeight - this.Height);

			if (Program.AutoStart)
				cmdGo_Click(this, null);
		}

		private void lblPacket_Click(object sender, EventArgs e)
		{
			if (!lblPacket.ForeColor.Equals(Color.LightPink))
			{
				isfreezing = true;
				lblPacket.ForeColor = Color.LightPink;
				lblPacket.Refresh();
				Thread.Sleep(50);
				isfreezingR = true;
				lblPacketR.ForeColor = Color.LightPink;
				lblPacketR.Refresh();
			}
			else
			{
				isfreezingR = false;
				isfreezing = false;
				lblPacketR.ForeColor = Color.White;
				lblPacketR.Refresh();
				lblPacket.ForeColor = Color.White;
				lblPacket.Refresh();
			}
		}
	}
}