using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HeroTCPRelay
{
	public class ServerSocket
	{
		#region Delegates
		public delegate void ConnectionEventHandler(Socket socket);
		public delegate void ErrorEventHandler(string errorMessage, Socket socket, int errorCode);
		public delegate void ListenEventHandler();
		#endregion

		#region Events
		public event ConnectionEventHandler OnConnect;
		public event ConnectionEventHandler OnDisconnect;
		public event ConnectionEventHandler OnRead;
		public event ConnectionEventHandler OnWrite;
		public event ErrorEventHandler OnError;
		public event ListenEventHandler OnListen;
		public event ConnectionEventHandler OnSendFile;
		#endregion

		#region Variables
		private ArrayList clients = ArrayList.Synchronized(new ArrayList());
		private AsyncCallback WorkerCallBack;
		private Socket mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private IPEndPoint ipLocal;
		private int mPort = 0;
		private byte[] mBytesReceived;
		private byte[] mBytesSent;
		private string mTextReceived = "";
		private string mTextSent = "";
		#endregion

		#region Properties
		/// <summary>
		/// 통신 포트
		/// </summary>
		public int Port
		{
			get
			{
				return (mPort);
			}
		}

		/// <summary>
		/// 소켓으로 읽은 데이터
		/// </summary>
		public byte[] ReceivedBytes
		{
			get
			{
				byte[] temp = null;
				if (mBytesReceived != null)
				{
					temp = mBytesReceived;
					mBytesReceived = null;
				}
				return (temp);
			}
		}

		/// <summary>
		/// 소켓으로 읽은 텍스트 데이터
		/// </summary>
		public string ReceivedText
		{
			get
			{
				mTextReceived = Encoding.Default.GetString(mBytesReceived);
				mBytesReceived = null;

				string temp = mTextReceived;
				mTextReceived = "";
				return (temp);
			}
		}

		/// <summary>
		/// 소켓으로 보낸 텍스트 데이터
		/// </summary>
		public string WriteText
		{
			get
			{
				mTextSent = Encoding.Default.GetString(mBytesSent);
				mBytesSent = null;

				string temp = mTextSent;
				mTextSent = "";
				return (temp);
			}
		}

		/// <summary>
		/// 소켓으로 보낸 데이터
		/// </summary>
		public byte[] WriteBytes
		{
			get
			{
				byte[] temp = null;
				if (mBytesSent != null)
				{
					temp = mBytesSent;
					mBytesSent = null;
				}
				return (temp);
			}
		}

		/// <summary>
		/// Gets the active connections.
		/// </summary>
		/// <value>The active connections.</value>
		public int ActiveConnections
		{
			get
			{
				return (clients.Count);
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="T:HeroTCPRelay.ServerSocket"/> class.
		/// </summary>
		/// <param name="port">The port.</param>
		public ServerSocket(int port)
		{
			try
			{
				mPort = port;
				ipLocal = new IPEndPoint(IPAddress.Any, mPort);
				Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "ServerSocket.ServerSocket()-로컬 연결 대기 TCP 주소: " + ipLocal);
			}
			catch (Exception se)
			{
				if (OnError != null)
					OnError(se.Message, null, 0);
			}
		}
		#endregion

		#region Functions & Events
		/// <summary>
		/// Actives this instance.
		/// </summary>
		/// <returns></returns>
		public bool Listen()
		{
			try
			{
				mainSocket.Bind(ipLocal);
				mainSocket.Listen(10);

				if (OnListen != null)
					OnListen();

				mainSocket.BeginAccept(new AsyncCallback(acceptCallback), null);
				return true;
			}
			catch (SocketException se)
			{
				if (OnError != null)
					OnError(se.Message, mainSocket, se.ErrorCode);

				return false;
			}
		}

		private void acceptCallback(IAsyncResult asyn)
		{
			try
			{
				Socket workSocket = mainSocket.EndAccept(asyn);
				try
				{
					WaitForData(workSocket);
					lock (this)
					{
						clients.Add(workSocket);
					}
					if (OnConnect != null)
						OnConnect(workSocket);

					mainSocket.BeginAccept(new AsyncCallback(acceptCallback), null);
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, workSocket, se.ErrorCode);
				}
			}
			catch (ObjectDisposedException)
			{
				if (OnDisconnect != null)
					OnDisconnect(null);
			}
		}

		private void WaitForData(Socket socket)
		{
			try
			{
				if (WorkerCallBack == null)
					WorkerCallBack = new AsyncCallback(receiveCallback);

				StateObject state = new StateObject();
				state.workSocket = socket;

				socket.BeginReceive(state.buffer, 0, StateObject.BufferSize,
				                 SocketFlags.None, WorkerCallBack, state);
			}
			catch (SocketException se)
			{
				if (OnError != null)
					OnError(se.Message, socket, se.ErrorCode);
			}
		}

		private void receiveCallback(IAsyncResult asyn)
		{
			StateObject state = (StateObject)asyn.AsyncState;
			try
			{
				Socket handler = state.workSocket;
				int iRx = handler.EndReceive(asyn);
				if (iRx < 1)
				{
					handler.Close();
					if (!handler.Connected)
					{
						if (OnDisconnect != null)
							OnDisconnect(handler);

						clients.Remove(handler);
						state.workSocket = null;
					}
				}
				else
				{
					mBytesReceived = new byte[iRx];
					//Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", iRx + " 바이트 보냄.");

					Array.Copy(state.buffer, mBytesReceived, iRx);
					if (OnRead != null)
						OnRead(handler);

					WaitForData(handler);
				}
			}
			catch (InvalidOperationException se)
			{
				Socket handler = state.workSocket;
				if (handler.Connected)
					handler.Close();

				if (!handler.Connected)
				{
					if (OnDisconnect != null)
						OnDisconnect(handler);

					clients.Remove(handler);
					state.workSocket = null;
				}
				else
					if (OnError != null)
						OnError(se.Message, null, 0);
			}
			catch (SocketException se)
			{
				Socket handler = state.workSocket;
				if (OnError != null)
					OnError(se.Message, handler, se.ErrorCode);

				if (!handler.Connected)
				{
					if (OnDisconnect != null)
						OnDisconnect(handler);

					clients.Remove(handler);
					state.workSocket = null;
				}
			}
		}

		/// <summary>
		/// Sends the text.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public bool SendText(string msg, int SocketIndex)
		{
			byte[] byData = Encoding.Default.GetBytes(msg);
			return Send(byData, SocketIndex);
		}

		/// <summary>
		/// Sends the bytes.
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public bool Send(byte[] bytes, int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket workerSocket = (Socket)clients[SocketIndex];
				try
				{
					int NumBytes = workerSocket.Send(bytes);
					if (NumBytes == bytes.Length)
					{
						if (OnWrite != null)
						{
							mBytesSent = bytes;
							OnWrite(workerSocket);
						}
						return true;
					}
					else
						return false;
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + SocketIndex + ")가 범위를 벗어났습니다.", null, 0);

				return false;
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public bool SendFile(string FileName, int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket workerSocket = (Socket)clients[SocketIndex];
				try
				{
					workerSocket.BeginSendFile(FileName, new AsyncCallback(fileSendCallback), workerSocket);
					return true;
				}
				catch (FileNotFoundException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, workerSocket, se.ErrorCode);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + SocketIndex + ")가 범위를 벗어났습니다.", null, 0);

				return false;
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="PreString">The pre string.</param>
		/// <param name="PosString">The pos string.</param>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public bool SendFile(string FileName, string PreString, string PosString, int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket workerSocket = (Socket)clients[SocketIndex];
				try
				{
					byte[] preBuf = Encoding.Default.GetBytes(PreString);
					byte[] postBuf = Encoding.Default.GetBytes(PosString);
					workerSocket.BeginSendFile(FileName, preBuf, postBuf, 0, new AsyncCallback(fileSendCallback), workerSocket);
					return true;
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, workerSocket, se.ErrorCode);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + SocketIndex + ")가 범위를 벗어났습니다.", null, 0);

				return false;
			}
		}

		private void fileSendCallback(IAsyncResult ar)
		{
			Socket workerSocket = (Socket)ar.AsyncState;
			workerSocket.EndSendFile(ar);

			if (OnSendFile != null)
				OnSendFile(workerSocket);
		}

		/// <summary>
		/// Deactives this instance.
		/// </summary>
		/// <returns></returns>
		public bool Close()
		{
			bool err = true;
			if (mainSocket != null)
				mainSocket.Close();

			Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "ServerSocket.Close()-소켓 연결 끊김.");

			int total = clients.Count;
			for (int i = 0; i < total; i++)
			{
				Socket workerSocket = (Socket)clients[i];
				if (workerSocket != null)
				{
					if (OnDisconnect != null)
						OnDisconnect(workerSocket);

					workerSocket.Close();
					err = err && workerSocket.Connected;
				}
			}
			return err;
		}

		/// <summary>
		/// Closes the connection.
		/// </summary>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public bool CloseConnection(int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket workerSocket = (Socket)clients[SocketIndex];
				if (workerSocket != null)
					workerSocket.Close();

				if (workerSocket == null || !workerSocket.Connected)
					return true;
				else
					return false;
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + SocketIndex + ")가 범위를 벗어났습니다.", null, 0);
				return false;
			}
		}

		/// <summary>
		/// Connecteds the specified socket index.
		/// </summary>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public bool Connected(int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket soc = (Socket)clients[SocketIndex];
				return soc.Connected;
			}
			else
				return false;
		}

		/// <summary>
		/// Get the remote address.
		/// </summary>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public string RemoteAddress(int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket soc = (Socket)clients[SocketIndex];
				try
				{

					string temp = soc.RemoteEndPoint.ToString();
					return temp.Substring(0, temp.IndexOf(":"));
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return "";
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);
					
					return "";
				}
			}
			else
				return "";
		}

		/// <summary>
		/// Get the remote host.
		/// </summary>
		/// <param name="SocketIndex">Index of the socket.</param>
		/// <returns></returns>
		public string RemoteHost(int SocketIndex)
		{
			if ((clients.Count - 1) >= SocketIndex)
			{
				Socket soc = (Socket)clients[SocketIndex];
				try
				{
					string temp = soc.RemoteEndPoint.ToString();
					temp = temp.Substring(0, temp.IndexOf(":"));
					IPHostEntry entry = Dns.GetHostEntry(temp);
					return entry.HostName;
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return "";
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, null, 0);

					return "";
				}
			}
			else
				return "";
		}

		/// <summary>
		/// Indexes of the socket.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <returns></returns>
		public int IndexOf(Socket socket)
		{
			return clients.IndexOf(socket);
		}
		#endregion
	}
}
