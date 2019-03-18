using System;
using System.Net.Sockets;
using System.Text;

namespace iManP2PService
{
	public class StateObject
	{
		public bool connected = false;	// ID received flag
		public int index = -1;
		public Socket workSocket = null;	// Client socket.
		public Socket partnerSocket = null;	// Partner socket.
		public const int BufferSize = 65535;	// Size of receive buffer.
		public byte[] buffer = new byte[BufferSize];// Receive buffer.
		public StringBuilder sb = new StringBuilder();//Received data String.
		public string id = String.Empty;	// Host or conversation ID
		public DateTime TimeStamp;
	}
}
