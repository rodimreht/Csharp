using System;
using System.Net;
using System.Net.Sockets;

namespace Ping
{
	class Program
	{
		const int SOCKET_ERROR = -1;
		const int ICMP_ECHO = 8;

		public static void Main(string[] argv)
		{
			if (argv.Length == 0)
			{
				Console.WriteLine("Usage:Ping <hostname> [-t] [-l <packet size>]");
				Console.WriteLine("<hostname> The name of the Host who you want to ping");
				Console.WriteLine("-t	Ping the host continuously");
				Console.WriteLine("-l	Ping the host with <packet size> in bytes");
				return;
			}
			else if (argv.Length == 1)
			{
				PingHost(argv[0]);
				return;
			}
			else if (argv.Length > 1)
			{
				if (argv.Length == 2)
				{
					if (argv[1] == "-t")
					{
						//loop the ping program
						while (true)
							PingHost(argv[0]);
					}
				}
				else if (argv.Length == 3)
				{
					if (argv[1] == "-l")
					{
						PingHost(argv[0], int.Parse(argv[2]));
						return;
					}
				}
				else if (argv.Length == 4)
				{
					if (((argv[1] == "-t") && (argv[2] == "-l")) ||
					    ((argv[1] == "-l") && (argv[3] == "-t")))
					{
						while (true)
							PingHost(argv[0], int.Parse(argv[2]));
					}
				}
			}
			
			Console.WriteLine("Error in Arguments");
		}

		/// <summary>
		///		This method takes the "hostname" of the server
		///		and then it ping's it and shows the response time
		/// </summary>
		public static void PingHost(string host)
		{
			PingHost(host, 32);
		}

		/// <summary>
		///		This method takes the "hostname" of the server
		///		and then it ping's it and shows the response time
		/// </summary>
		public static void PingHost(string host, int pingData)
		{
			//Declare the IPHostEntry 
			IPHostEntry serverHE, fromHE;
			int nBytes, dwStart, dwStop;

			//Initilize a Socket of the Type ICMP
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);

			// Get the server endpoint
			try
			{
				serverHE = Dns.GetHostEntry(host);
			}
			catch (Exception)
			{
				Console.WriteLine("Host not found"); // fail
				return;
			}

			// Convert the server IP_EndPoint to an EndPoint
			IPEndPoint ipepServer = new IPEndPoint(serverHE.AddressList[0], 0);
			EndPoint epServer = (ipepServer);

			// Set the receiving endpoint to the client machine
			fromHE = Dns.GetHostEntry(Dns.GetHostName());
			IPEndPoint ipEndPointFrom = new IPEndPoint(fromHE.AddressList[0], 0);
			EndPoint EndPointFrom = (ipEndPointFrom);

			int PacketSize;
			IcmpPacket packet = new IcmpPacket();
			// Construct the packet to send
			packet.Type = ICMP_ECHO; //8
			packet.SubCode = 0;
			packet.CheckSum = UInt16.Parse("0");
			packet.Identifier = UInt16.Parse("45");
			packet.SequenceNumber = UInt16.Parse("0");

			packet.Data = new Byte[pingData];
			//Initilize the Packet.Data
			for (int i = 0; i < pingData; i++)
				packet.Data[i] = (byte)'#';

			//Variable to hold the total Packet size
			PacketSize = pingData + 8;
			Byte[] icmp_pkt_buffer = new Byte[PacketSize];
			//Call a Method Serialize which counts
			//The total number of Bytes in the Packet
			int Index = Serialize(packet,
								  icmp_pkt_buffer,
								  PacketSize,
								  pingData);
			//Error in Packet Size
			if (Index == -1)
			{
				Console.WriteLine("Error in Making Packet");
				return;
			}

			// now get this critter into a UInt16 array

			//Get the Half size of the Packet
			Double double_length = Convert.ToDouble(Index);
			Double dtemp = Math.Ceiling(double_length / 2);
			int cksum_buffer_length = Convert.ToInt32(dtemp);
			//Create a Byte Array
			UInt16[] cksum_buffer = new UInt16[cksum_buffer_length];
			//Code to initialize the Uint16 array 
			int icmp_header_buffer_index = 0;
			for (int i = 0; i < cksum_buffer_length; i++)
			{
				cksum_buffer[i] =
					  BitConverter.ToUInt16(icmp_pkt_buffer, icmp_header_buffer_index);
				icmp_header_buffer_index += 2;
			}
			//Call a method which will return a checksum             
			UInt16 u_cksum = checksum(cksum_buffer, cksum_buffer_length);
			//Save the checksum to the Packet
			packet.CheckSum = u_cksum;

			// Now that we have the checksum, serialize the packet again
			Byte[] sendbuf = new Byte[PacketSize];
			//again check the packet size
			Index = Serialize(packet,
							  sendbuf,
							  PacketSize,
							  pingData);
			//if there is a error report it
			if (Index == -1)
			{
				Console.WriteLine("Error in Making Packet");
				return;
			}


			dwStart = Environment.TickCount; // Start timing
			//send the Pack over the socket
			if (socket.SendTo(sendbuf, PacketSize, 0, epServer) == SOCKET_ERROR)
			{
				Console.WriteLine("Socket Error cannot Send Packet");
			}
			// Initialize the buffers. The receive buffer is the size of the
			// ICMP header plus the IP header (20 bytes)
			Byte[] ReceiveBuffer = new Byte[PacketSize + 32];
			//Receive the bytes
			bool recd = false;
			int timeout;

			//loop for checking the time of the server responding 
			while (!recd)
			{
				nBytes = socket.ReceiveFrom(ReceiveBuffer, ReceiveBuffer.Length, 0, ref EndPointFrom);
				if (nBytes == SOCKET_ERROR)
				{
					Console.WriteLine("Host not Responding");
					recd = true;
					continue;
				}
				else if (nBytes > 0)
				{
					dwStop = Environment.TickCount - dwStart; // stop timing
					Console.WriteLine("Reply from " + epServer.ToString() + " in "
					+ dwStop + " ms: " + nBytes + " Bytes Received");
					recd = true;
					continue;
				}

				timeout = Environment.TickCount - dwStart;
				if (timeout > 1000)
				{
					Console.WriteLine("Time Out");
					recd = true;
				}
			}

			//close the socket
			socket.Close();
		}
		
		/// <summary>
		///  This method get the Packet and calculates the total size 
		///  of the Pack by converting it to byte array
		/// </summary>
		public static int Serialize(IcmpPacket packet, Byte[] Buffer,
			  int PacketSize, int PingData)
		{
			int cbReturn;
			// serialize the struct into the array
			int Index = 0;

			Byte[] b_type = new Byte[1];
			b_type[0] = (packet.Type);

			Byte[] b_code = new Byte[1];
			b_code[0] = (packet.SubCode);

			Byte[] b_cksum = BitConverter.GetBytes(packet.CheckSum);
			Byte[] b_id = BitConverter.GetBytes(packet.Identifier);
			Byte[] b_seq = BitConverter.GetBytes(packet.SequenceNumber);

			// Console.WriteLine("Serialize type ");
			Array.Copy(b_type, 0, Buffer, Index, b_type.Length);
			Index += b_type.Length;

			// Console.WriteLine("Serialize code ");
			Array.Copy(b_code, 0, Buffer, Index, b_code.Length);
			Index += b_code.Length;

			// Console.WriteLine("Serialize cksum ");
			Array.Copy(b_cksum, 0, Buffer, Index, b_cksum.Length);
			Index += b_cksum.Length;

			// Console.WriteLine("Serialize id ");
			Array.Copy(b_id, 0, Buffer, Index, b_id.Length);
			Index += b_id.Length;

			Array.Copy(b_seq, 0, Buffer, Index, b_seq.Length);
			Index += b_seq.Length;

			// copy the data	        
			Array.Copy(packet.Data, 0, Buffer, Index, PingData);
			Index += PingData;
			if (Index != PacketSize/* sizeof(IcmpPacket)  */)
			{
				cbReturn = -1;
				return cbReturn;
			}

			cbReturn = Index;
			return cbReturn;
		}
		
		/// <summary>
		///		This Method has the algorithm to make a checksum 
		/// </summary>
		public static UInt16 checksum(UInt16[] buffer, int size)
		{
			int cksum = 0;
			int counter;
			counter = 0;

			while (size > 0)
			{
				UInt16 val = buffer[counter];
				cksum += Convert.ToInt32(val);
				counter += 1;
				size -= 1;
			}

			cksum = (cksum >> 16) + (cksum & 0xffff);
			cksum += (cksum >> 16);
			return (UInt16)(~cksum);
		}
	} // class ping
	
	/// <summary>
	///		Class that holds the Pack information
	/// </summary>
	public class IcmpPacket
	{
		public Byte Type;    // type of message
		public Byte SubCode;    // type of sub code
		public UInt16 CheckSum;   // ones complement checksum of struct
		public UInt16 Identifier;      // identifier
		public UInt16 SequenceNumber;     // sequence number  
		public Byte[] Data;

	} // class IcmpPacket
}
