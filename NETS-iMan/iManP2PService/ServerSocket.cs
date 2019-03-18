using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace iManP2PService
{
	public class ServerSocket
	{
		#region Delegates

		public delegate void ConnectionEventHandler(int index, Socket socket);
		public delegate void ErrorEventHandler(string errorMessage, int index, Socket socket, int errorCode);
		public delegate void ListenEventHandler();
		public delegate void SocketDataEventHandler(int index, Socket socket, byte[] bytes);

		#endregion

		#region Events

		public event ConnectionEventHandler OnConnect;
		public event ConnectionEventHandler OnDisconnect;
		public event SocketDataEventHandler OnRead;
		public event SocketDataEventHandler OnWrite;
		public event ErrorEventHandler OnError;
		public event ListenEventHandler OnListen;
		public event ConnectionEventHandler OnSendFile;

		#endregion

		#region Variables

		private readonly NameObjectCollection clients = new NameObjectCollection();
		private int clientIndex = 0;
		private readonly IPEndPoint ipLocal;
		private readonly Socket mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly NameValueCollection mFiles = new NameValueCollection();
		private readonly int mPort;
		private AsyncCallback WorkerCallBack;

		#endregion

		#region Properties

		/// <summary>
		/// 통신 포트
		/// </summary>
		public int Port
		{
			get { return (mPort); }
		}

		/// <summary>
		/// Gets the active connections.
		/// </summary>
		/// <value>The active connections.</value>
		public int ActiveConnections
		{
			get { return (clients.Count); }
		}

		public void SetFileName(int index, string path)
		{
			lock (this)
			{
				if (mFiles[index.ToString()] == null)
					mFiles.Add(index.ToString(), path);
				else
					mFiles[index.ToString()] = path;
			}
		}
		
		#endregion

		#region Constructor

		public ServerSocket(int port)
		{
			try
			{
				mPort = port;
				ipLocal = new IPEndPoint(IPAddress.Any, mPort);
				Logger.Log("[ServerSocket] TCP Local address and port: " + ipLocal);
			}
			catch (Exception se)
			{
				if (OnError != null)
					OnError(se.Message, -1, null, 0);
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
					OnError(se.Message, -1, mainSocket, se.ErrorCode);

				return false;
			}
		}

		private void acceptCallback(IAsyncResult asyn)
		{
			try
			{
				if (clients.Count == 0)
					clientIndex = 0;
				else
					clientIndex++;

				Socket workSocket = mainSocket.EndAccept(asyn);
				try
				{
					WaitForData(clientIndex, workSocket);
					lock (this)
					{
						clients.Add(clientIndex.ToString(), workSocket);
					}
					if (OnConnect != null)
						OnConnect(clientIndex, workSocket);

					mainSocket.BeginAccept(new AsyncCallback(acceptCallback), null);
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, clientIndex, workSocket, se.ErrorCode);
				}
			}
			catch (ObjectDisposedException)
			{
				if (OnDisconnect != null)
					OnDisconnect(-1, null);
			}
		}

		private void WaitForData(int index, Socket socket)
		{
			try
			{
				if (WorkerCallBack == null)
					WorkerCallBack = receiveCallback;

				StateObject state = new StateObject();
				state.index = index;
				state.workSocket = socket;

				socket.BeginReceive(state.buffer, 0, StateObject.BufferSize,
				                    SocketFlags.None, WorkerCallBack, state);
			}
			catch (SocketException se)
			{
				if (OnError != null)
					OnError(se.Message, index, socket, se.ErrorCode);
			}
		}

		private void receiveCallback(IAsyncResult asyn)
		{
			StateObject state = (StateObject) asyn.AsyncState;
			try
			{
				int index = state.index;
				Socket handler = state.workSocket;
				int iRx = handler.EndReceive(asyn);
				if (iRx < 1)
				{
					handler.Close();
					if (!handler.Connected)
					{
						if (OnDisconnect != null)
							OnDisconnect(index, handler);

						clients.Remove(index.ToString());
						state.index = -1;
						state.workSocket = null;
					}
				}
				else
				{
					byte[] bytes = new byte[iRx];
					//Logger.Log("[ServerSocket] Read " + iRx + " bytes from socket[" + index + "]");

					Array.Copy(state.buffer, bytes, iRx);

					if (OnRead != null)
						OnRead(index, handler, bytes);

					WaitForData(index, handler);
				}
			}
			catch (InvalidOperationException se)
			{
				int index = state.index;
				Socket handler = state.workSocket;
				if (handler.Connected)
					handler.Close();

				if (!handler.Connected)
				{
					if (OnDisconnect != null)
						OnDisconnect(index, handler);

					clients.Remove(index.ToString());
					state.index = -1;
					state.workSocket = null;
				}
				else if (OnError != null)
					OnError(se.Message, index, null, 0);
			}
			catch (SocketException se)
			{
				int index = state.index;
				Socket handler = state.workSocket;
				if (OnError != null)
					OnError(se.Message, index, handler, se.ErrorCode);

				if (!handler.Connected)
				{
					if (OnDisconnect != null)
						OnDisconnect(index, handler);

					clients.Remove(index.ToString());
					state.index = -1;
					state.workSocket = null;
				}
			}
		}

		/// <summary>
		/// Sends the text.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool SendText(string msg, int index)
		{
			byte[] byData = Encoding.Default.GetBytes(msg);
			return Send(byData, index);
		}

		/// <summary>
		/// Sends the bytes.
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool Send(byte[] bytes, int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket workerSocket = (Socket)clients[index.ToString()];
				try
				{
					int NumBytes = workerSocket.Send(bytes);
					if (NumBytes == bytes.Length)
					{
						if (OnWrite != null)
						{
							OnWrite(index, workerSocket, bytes);
						}
						return true;
					}
					else
						return false;
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + index + ")가 범위를 벗어났습니다.", -1, null, 0);

				return false;
			}
		}

		/// <summary>
		/// Sends the bytes.
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="len">The len.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool Send(byte[] bytes, int len, int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket workerSocket = (Socket)clients[index.ToString()];
				try
				{
					int numBytes = workerSocket.Send(bytes, 0, len, SocketFlags.None);
					if (numBytes == len)
					{
						if (OnWrite != null)
						{
							byte[] nBytes = new byte[len];
							Array.Copy(bytes, nBytes, len);

							OnWrite(index, workerSocket, nBytes);
						}
						return true;
					}
					else
						return false;
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + index + ")가 범위를 벗어났습니다.", -1, null, 0);

				return false;
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool SendFile(int index)
		{
			return SendFile(mFiles[index.ToString()], index);
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="PreString">The pre string.</param>
		/// <param name="PosString">The pos string.</param>
		/// <returns></returns>
		public bool SendFile(int index, string PreString, string PosString)
		{
			return SendFile(mFiles[index.ToString()], PreString, PosString, index);
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool SendFile(string FileName, int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket workerSocket = (Socket)clients[index.ToString()];
				try
				{
					StateObject state = new StateObject();
					state.index = index;
					state.workSocket = workerSocket;

					workerSocket.BeginSendFile(FileName, fileSendCallback, state);
					return true;
				}
				catch (FileNotFoundException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, index, workerSocket, se.ErrorCode);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + index + ")가 범위를 벗어났습니다.", -1, null, 0);

				return false;
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="PreString">The pre string.</param>
		/// <param name="PosString">The pos string.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool SendFile(string FileName, string PreString, string PosString, int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket workerSocket = (Socket)clients[index.ToString()];
				try
				{
					StateObject state = new StateObject();
					state.index = index;
					state.workSocket = workerSocket;

					byte[] preBuf = Encoding.Default.GetBytes(PreString);
					byte[] postBuf = Encoding.Default.GetBytes(PosString);
					workerSocket.BeginSendFile(FileName, preBuf, postBuf, 0, fileSendCallback, state);
					return true;
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (ObjectDisposedException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return false;
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, index, workerSocket, se.ErrorCode);

					return false;
				}
			}
			else
			{
				if (OnError != null)
					OnError("[ServerSocket] 소켓 인덱스(" + index + ")가 범위를 벗어났습니다.", -1, null, 0);

				return false;
			}
		}

		private void fileSendCallback(IAsyncResult ar)
		{
			StateObject state = (StateObject) ar.AsyncState;
			int index = state.index;
			Socket workerSocket = state.workSocket;
			workerSocket.EndSendFile(ar);

			if (OnSendFile != null)
				OnSendFile(index, workerSocket);
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

			Logger.Log("[ServerSocket] Socket closed.");

			int total = clients.Count;
			for (int i = 0; i < total; i++)
			{
				Socket workerSocket = (Socket) clients[i.ToString()];
				if (workerSocket != null)
				{
					if (OnDisconnect != null)
						OnDisconnect(i, workerSocket);

					workerSocket.Close();
					err = err && workerSocket.Connected;
				}
			}
			return err;
		}

		/// <summary>
		/// Closes the connection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool CloseConnection(int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket workerSocket = (Socket)clients[index.ToString()];
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
					OnError("[ServerSocket] 소켓 인덱스(" + index + ")가 범위를 벗어났습니다.", -1, null, 0);
				return false;
			}
		}

		/// <summary>
		/// Connecteds the specified socket index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public bool Connected(int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket soc = (Socket)clients[index.ToString()];
				return soc.Connected;
			}
			else
				return false;
		}

		/// <summary>
		/// Get the remote address.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public string RemoteAddress(int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket soc = (Socket)clients[index.ToString()];
				try
				{
					string temp = soc.RemoteEndPoint.ToString();
					return temp.Substring(0, temp.IndexOf(":"));
				}
				catch (ArgumentException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return "";
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return "";
				}
			}
			else
				return "";
		}

		/// <summary>
		/// Get the remote host.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public string RemoteHost(int index)
		{
			if ((clients.Count > 0) && (clients.ContainsKey(index.ToString())))
			{
				Socket soc = (Socket)clients[index.ToString()];
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
						OnError(se.Message, -1, null, 0);

					return "";
				}
				catch (SocketException se)
				{
					if (OnError != null)
						OnError(se.Message, -1, null, 0);

					return "";
				}
			}
			else
				return "";
		}

		/// <summary>
		/// Names of the socket.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <returns></returns>
		public string NameOf(Socket socket)
		{
			for (int i = 0; i < clients.Count; i++)
			{
				if (clients.Get(i) == socket) return clients.GetKey(i);
			}
			return null;
		}

		#endregion
	}
}