using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HeroTCPRelay
{
	[Serializable, StructLayout(LayoutKind.Sequential)]
	struct SERVER_CONNECT
	{
		public Int16 header;
		public Int16 block_size;
		public Int16 seq_no;
		public byte flag;
		public byte str_size;
		public string server_ip;
		public Int32 server_port;
		public Int16 tail;
	}

	class HeroCommon
	{
		[DllImport("wsock32.dll")]
		public static extern int htonl(int hLong);

		[DllImport("wsock32.dll")]
		public static extern int ntohl(int nLong);

		[DllImport("wsock32.dll")]
		public static extern Int16 htons(Int16 hLong);

		[DllImport("wsock32.dll")]
		public static extern Int16 ntohs(Int16 nLong);
		
		public static int HeaderSize(SERVER_CONNECT header)
		{
			return 6 + header.block_size;
		}

		public static byte[] ConvertToBytes(SERVER_CONNECT header)
		{
			byte[] bytes = new byte[HeaderSize(header)];
			byte[] b = BitConverter.GetBytes(header.header);
			Array.Copy(b, 0, bytes, 0, 2);
			b = BitConverter.GetBytes(header.block_size);
			Array.Copy(b, 0, bytes, 2, 2);
			b = BitConverter.GetBytes(header.seq_no);
			Array.Copy(b, 0, bytes, 4, 2);
			bytes[6] = header.flag;
			bytes[7] = header.str_size;

			IntPtr ptr; // = Marshal.AllocHGlobal(header.str_size);
			ptr = Marshal.StringToHGlobalAnsi(header.server_ip);
			Marshal.Copy(ptr, bytes, 8, header.str_size);
			Marshal.FreeHGlobal(ptr);

			b = BitConverter.GetBytes(header.server_port);
			Array.Copy(b, 0, bytes, 7 + header.str_size + 1, 4);
			b = BitConverter.GetBytes(header.tail);
			Array.Copy(b, 0, bytes, bytes.Length - 2, 2);
			return bytes;
		}

		public static SERVER_CONNECT ConvertToHeader(byte[] bytes)
		{
			SERVER_CONNECT header = new SERVER_CONNECT();
			header.header = BitConverter.ToInt16(bytes, 0);
			header.block_size = BitConverter.ToInt16(bytes, 2);
			header.seq_no = BitConverter.ToInt16(bytes, 4);
			header.flag = bytes[6];
			header.str_size = bytes[7];

			IntPtr ptr = Marshal.AllocHGlobal(header.str_size);
			Marshal.Copy(bytes, 8, ptr, header.str_size);
			header.server_ip = Marshal.PtrToStringAnsi(ptr, header.str_size);
			Marshal.FreeHGlobal(ptr);

			header.server_port = BitConverter.ToInt32(bytes, 7 + header.str_size + 1);
			header.tail = BitConverter.ToInt16(bytes, bytes.Length - 2);
			return header;
		}

		public static void GetHostFixed(ref SERVER_CONNECT header)
		{
			header.header = htons(header.header);
			header.seq_no = htons(header.seq_no);
			header.tail = htons(header.tail);
		}

		public static void GetNetworkFixed(ref SERVER_CONNECT header)
		{
			header.header = ntohs(header.header);
			header.seq_no = ntohs(header.seq_no);
			header.tail = ntohs(header.tail);
		}

		public static int CheckMainConnect(byte[] bytes)
		{
			try
			{
				Int16 seq_no = BitConverter.ToInt16(bytes, 4);
				seq_no = htons(seq_no);
				byte str_size = bytes[7];

				IntPtr ptr = Marshal.AllocHGlobal(str_size);
				Marshal.Copy(bytes, 8, ptr, str_size);
				string server_ip = Marshal.PtrToStringAnsi(ptr, str_size);
				Marshal.FreeHGlobal(ptr);
				
				int server_port = BitConverter.ToInt32(bytes, 7 + str_size + 1);

				// 초기접속은 seq_no가 5
				// 중간 서버 변경은 seq_no가 다름
				if (seq_no == 5 || (server_ip.StartsWith("222.235.") && (server_port == 15030)))
					return 0;
				else
				{
					int tmpPos = 0;
					int nextPos = GetNextPacketPos(bytes, tmpPos);
					while (nextPos > 0)
					{
						Logger.Log(LogLevel.DEBUG, "HeroTCPRelay", "다음 연속되는 패킷 위치 찾음: 인덱스=" + nextPos);

						// 패킷 크기가 10바이트 이하이면 건너뜀
						if (nextPos < tmpPos + 10)
						{
							tmpPos = nextPos;
							nextPos = GetNextPacketPos(bytes, tmpPos);
							continue;
						}

						seq_no = BitConverter.ToInt16(bytes, 4 + nextPos);
						seq_no = htons(seq_no);
						str_size = bytes[7 + nextPos];

						// IP주소 항목이 없으면(15바이트 이상) 건너뜀
						if ((str_size > 15) || (str_size < 7))
						{
							tmpPos = nextPos;
							nextPos = GetNextPacketPos(bytes, tmpPos);
							continue;
						}

						ptr = Marshal.AllocHGlobal(str_size);
						Marshal.Copy(bytes, 8 + nextPos, ptr, str_size);
						server_ip = Marshal.PtrToStringAnsi(ptr, str_size);
						Marshal.FreeHGlobal(ptr);

						server_port = BitConverter.ToInt32(bytes, 7 + str_size + 1 + nextPos);

						if (seq_no == 5 || (server_ip.StartsWith("222.235.") && (server_port == 15030)))
							return nextPos;

						tmpPos = nextPos;
						nextPos = GetNextPacketPos(bytes, tmpPos);
					}
					return -1;
				}
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static int GetNextPacketPos(byte[] bytes, int pos)
		{
			try
			{
				if (pos + 2 < bytes.Length)
				{
					Int16 block_size = BitConverter.ToInt16(bytes, pos + 2);
					if (bytes.Length > (pos + block_size + 6))
						return (pos + block_size + 6);
				}
				return 0;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public static bool IsLogFiltered(byte[] bytes)
		{
			return false;
			try
			{
				Int16 block_size = BitConverter.ToInt16(bytes, 2);
				Int16 seq_no = BitConverter.ToInt16(bytes, 4);
				seq_no = htons(seq_no);
				
				// 일반공격
				if ((seq_no == 0x020B) && (block_size == 0x0E))
					return false;
				// ???
				else if ((seq_no == 0x020D) && (block_size == 0x10))
					return false;
				// 무공공격
				else if ((seq_no == 0x021A) && (block_size == 0x1D))
					return false;
				else
					return true;
			}
			catch (Exception)
			{
				return true;
			}
		}

		public static byte[] DuplicateMessage(byte[] bytes, int times)
		{
			try
			{
				Int16 block_size = BitConverter.ToInt16(bytes, 2);
				Int16 seq_no = BitConverter.ToInt16(bytes, 4);
				seq_no = htons(seq_no);

				// 무공공격
				if ((seq_no == 0x021A) && (block_size == 0x1D))
				{
					byte[] newBytes = new byte[bytes.Length * times];
					for (int i = 0; i < times; i++)
						Array.Copy(bytes, 0, newBytes, bytes.Length * i, bytes.Length);
					return newBytes;
				}
				return bytes;
			}
			catch (Exception)
			{
				return bytes;
			}
		}

		public static bool IsDuplicatable(byte[] bytes)
		{
			try
			{
				Int16 block_size = BitConverter.ToInt16(bytes, 2);
				Int16 seq_no = BitConverter.ToInt16(bytes, 4);
				seq_no = htons(seq_no);

				// 무공공격
				if ((seq_no == 0x021A) && (block_size == 0x1D))
					return true;
				else
					return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsLogFilteredR(byte[] bytes)
		{
			return false;
			try
			{
				Int16 block_size = BitConverter.ToInt16(bytes, 2);
				Int16 seq_no = BitConverter.ToInt16(bytes, 4);
				seq_no = htons(seq_no);

				// 상태1
				if (((seq_no == 0x4100) || (seq_no == 0x4101)) && (block_size == 0x0A))
					return false;
				// 상태2
				else if ((seq_no == 0x4101) && (block_size == 0x0C))
					return false;
				else
					return true;
			}
			catch (Exception)
			{
				return true;
			}
		}

		public static void SetLocalServer()
		{
			string path = ConfigurationManager.AppSettings["HeroPath"];
			if (path == null) path = @"D:\Mgame\HeroOnline";

			// 이미 로컬 설정이 되어 있음
			if (File.Exists(path + "\\Server.ini.bak")) return;

			if (!File.Exists(path + "\\Server.ini.filter"))
			{
				using (FileStream fs = File.Create(path + "\\Server.ini.filter"))
				{
					using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
					{
						sw.Write(@"[LOGIN_SERVER]
COUNT=12
IP_0=127.0.0.1
Port_0=12382
IP_1=127.0.0.1
Port_1=12383
IP_2=127.0.0.1
Port_2=12384
IP_3=127.0.0.1
Port_3=12385
IP_4=127.0.0.1
Port_4=12386
IP_5=127.0.0.1
Port_5=12387
IP_6=127.0.0.1
Port_6=12388
IP_7=127.0.0.1
Port_7=12389
IP_8=127.0.0.1
Port_8=12390
IP_9=127.0.0.1
Port_9=12391
IP_10=127.0.0.1
Port_10=12392
IP_11=127.0.0.1
Port_11=12393
");
						sw.Flush();
						sw.Close();
					}
				}
			}

			File.Move(path + "\\Server.ini", path + "\\Server.ini.bak");
			File.Move(path + "\\Server.ini.filter", path + "\\Server.ini");
		}

		public static void SetOriginalServer()
		{
			string path = ConfigurationManager.AppSettings["HeroPath"];
			if (path == null) path = @"D:\Mgame\HeroOnline";

			if (!File.Exists(path + "\\Server.ini.bak")) return;

			File.Move(path + "\\Server.ini", path + "\\Server.ini.filter");
			File.Move(path + "\\Server.ini.bak", path + "\\Server.ini");
		}
	}
}
