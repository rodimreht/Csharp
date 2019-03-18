using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HeroTCPRelay
{
	public class ClientSocket
	{
		#region Delegates
		public delegate void ConnectionEventHandler(Socket socket);
		public delegate void ErrorEventHandler(string errorMessage, Socket socket, int errorCode);
		#endregion

		#region Events
		public event ConnectionEventHandler OnConnect;
		public event ConnectionEventHandler OnDisconnect;
		public event ConnectionEventHandler OnRead;
		public event ConnectionEventHandler OnWrite;
		public event ErrorEventHandler OnError;
		public event ConnectionEventHandler OnSendFile;
		#endregion

		#region Variables
		private AsyncCallback WorkerCallBack;
		private Socket mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private byte[] dataBuffer = new byte[65535];
		
		private byte[] mBytesReceived;
		private byte[] mBytesSent;
		private string mTextReceived = "";
		private string mTextSent = "";
		private string mRemoteAddress = "";
		private string mRemoteHost = "";
		private int mPortTarget = 0;

		private bool mConnecting = false;
		#endregion

		#region Properties
		/// <summary>
		/// 통신포트
		/// </summary>
		public int Port
		{
			get { return mPortTarget; }
			set { mPortTarget = value; }
		}

		/// <summary>
		/// 원격서버 IP주소
		/// </summary>
		/// <value>The IP.</value>
		public string IP
		{
			get { return mRemoteAddress; }
			set
			{
				mRemoteAddress = value;
				IPHostEntry ipss = Dns.GetHostEntry(mRemoteAddress);
				mRemoteHost = ipss.HostName;
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
		/// 소켓으로 보낸 혹은 보낼 데이터
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

			set { mBytesSent = value; }
		}

		/// <summary>
		/// 서버 IP주소
		/// </summary>
		public string RemoteAddress
		{
			get
			{
				if (mainSocket.Connected)
					return (mRemoteAddress);
				else
					return "";
			}
		}

		/// <summary>
		/// 서버 호스트명
		/// </summary>
		public string RemoteHost
		{
			get
			{
				if (mainSocket.Connected)
					return (mRemoteHost);
				else
					return "";
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:HeroTCPRelay.ClientSocket"/> is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected
		{
			get
			{
				return (mainSocket.Connected);
			}
		}

		public bool Connecting
		{
			get
			{
				return mConnecting;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="T:HeroTCPRelay.ClientSocket"/> class.
		/// </summary>
		public ClientSocket()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:HeroTCPRelay.ClientSocket"/> class.
		/// </summary>
		/// <param name="IP">The IP.</param>
		/// <param name="port">The port.</param>
		public ClientSocket(string IP, int port)
		{
			try
			{
				mainSocket.NoDelay = true;

				mPortTarget = port;
				IPAddress ipAddress = IPAddress.Parse(IP);
				mRemoteAddress = ipAddress.ToString();

				// 원격호스트 이름을 얻어오는 부분: 동작하지 않음 (15초 타임아웃)
				//IPHostEntry ipss = Dns.GetHostEntry(mRemoteAddress);
				//mRemoteHost = ipss.HostName;
				mRemoteHost = mRemoteAddress;
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
		/// Connects this instance.
		/// </summary>
		/// <returns></returns>
		public bool Connect()
		{
			try
			{
				IPAddress ipAddress = IPAddress.Parse(mRemoteAddress);
				IPEndPoint ipTarget = new IPEndPoint(ipAddress, mPortTarget);

				//Connect to the server
				Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "ClientSocket.Connect()-TCP 연결중: " + ipTarget + "...");

				mConnecting = true;
				mainSocket.BeginConnect(ipTarget, new AsyncCallback(connectCallback), null);
				return true;
			}
			catch (ArgumentException se)
			{
				mConnecting = false;
				if (OnError != null)
					OnError(se.Message, null, 0);
				return false;
			}
			catch (InvalidOperationException se)
			{
				mConnecting = false;
				if (OnError != null)
					OnError(se.Message, null, 0);
				return false;
			}
			catch (SocketException se)
			{
				mConnecting = false;
				if (OnError != null)
					OnError(se.Message, mainSocket, se.ErrorCode);
				return false;
			}
		}

		private void connectCallback(IAsyncResult asyn)
		{
			try
			{
				mainSocket.EndConnect(asyn);
				mConnecting = false;

				WaitForData(mainSocket);
				if (OnConnect != null)
					OnConnect(mainSocket);
			}
			catch (ObjectDisposedException se)
			{
				mConnecting = false;
				if (OnError != null)
					OnError(se.Message, null, 0);
			}
			catch (SocketException se)
			{
				mConnecting = false;
				if (OnError != null)
					OnError(se.Message, null, 0);
			}
		}

		private void WaitForData(Socket soc)
		{
			try
			{
				if (WorkerCallBack == null)
					WorkerCallBack = new AsyncCallback(receiveCallback);

				soc.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, WorkerCallBack, null);
			}
			catch (SocketException se)
			{
				if (OnError != null)
					OnError(se.Message, soc, se.ErrorCode);
			}
		}

		private void receiveCallback(IAsyncResult asyn)
		{
			try
			{
				int iRx = mainSocket.EndReceive(asyn);
				if (iRx < 1)
				{
					mainSocket.Close();
					if (!mainSocket.Connected)
						if (OnDisconnect != null)
							OnDisconnect(mainSocket);
				}
				else
				{
					//Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", iRx + " 바이트 받음.");

					byte[] bytes = new byte[iRx];
					Array.Copy(dataBuffer, bytes, iRx);
					mBytesReceived = bytes;

					if (OnRead != null)
						OnRead(mainSocket);

					WaitForData(mainSocket);
				}
			}
			catch (ArgumentException se)
			{
				if (OnError != null)
					OnError(se.Message, null, 0);
			}
			catch (InvalidOperationException se)
			{
				mainSocket.Close();
				if (!mainSocket.Connected)
					if (OnDisconnect != null)
						OnDisconnect(mainSocket);
					else
						if (OnError != null)
							OnError(se.Message, null, 0);
			}
			catch (SocketException se)
			{
				if (OnError != null)
					OnError(se.Message, mainSocket, se.ErrorCode);

				if (!mainSocket.Connected)
					if (OnDisconnect != null)
						OnDisconnect(mainSocket);
			}
		}

		/// <summary>
		/// Sends the text.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <returns></returns>
		public bool SendText(string msg)
		{
			byte[] byData = Encoding.Default.GetBytes(msg);
			return Send(byData);
		}

		/// <summary>
		/// Sends the bytes.
		/// </summary>
		/// <returns></returns>
		public bool Send()
		{
			return Send(mBytesSent);
		}

		/// <summary>
		/// Sends the bytes.
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <returns></returns>
		public bool Send(byte[] bytes)
		{
			try
			{
				int NumBytes = mainSocket.Send(bytes);
				//Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "ClientSocket.Send()-" + mainSocket.RemoteEndPoint + "로 " + NumBytes + " 바이트 보냄.");

				if (NumBytes == bytes.Length)
				{
					if (OnWrite != null)
					{
						mBytesSent = bytes;
						OnWrite(mainSocket);
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
					OnError(se.Message, mainSocket, se.ErrorCode);

				return false;
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public bool SendFile(string fileName)
		{
			try
			{
				mainSocket.BeginSendFile(fileName, new AsyncCallback(fileSendCallback), mainSocket);
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
					OnError(se.Message, mainSocket, se.ErrorCode);
				return false;
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="PreString">The pre string.</param>
		/// <param name="PosString">The pos string.</param>
		/// <returns></returns>
		public bool SendFile(string FileName, string PreString, string PosString)
		{
			try
			{
				byte[] preBuf = Encoding.Default.GetBytes(PreString);
				byte[] postBuf = Encoding.Default.GetBytes(PosString);
				mainSocket.BeginSendFile(FileName, preBuf, postBuf, 0, new AsyncCallback(fileSendCallback), mainSocket);
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
					OnError(se.Message, mainSocket, se.ErrorCode);
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
		/// Disconnects this instance.
		/// </summary>
		/// <returns></returns>
		public bool Disconnect()
		{
			mainSocket.Close();
			if (!mainSocket.Connected)
				return true;
			else
				return false;
		}
		#endregion
	}
}
