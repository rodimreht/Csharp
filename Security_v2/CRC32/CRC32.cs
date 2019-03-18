using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CRC32App
{
	/// <summary>
	/// 파일 CRC 체크 클래스
	/// </summary>
	public class CRC32 : HashAlgorithm
	{
		protected static uint AllOnes = 0xffffffff;
		protected static Hashtable cachedCRC32Tables;
		protected static bool autoCache;

		protected uint[] crc32Table;
		private uint m_crc;
		private string m_systemID;

		// 기본 버퍼 크기: 2MB
		private const int DEFAULT_BUFF_SIZE = 8 * 1024 * 1024;

		/// <summary>
		/// Returns the default polynomial (used in WinZip, Ethernet, etc)
		/// </summary>
		public static uint DefaultPolynomial
		{
			// This is the official polynomial used by CRC32 in PKZip.
			// Often times the polynomial shown reversed as 0x04C11DB7.
			get { return 0xEDB88320; }
		}

		/// <summary>
		/// Gets or sets the auto-cache setting of this class.
		/// </summary>
		public static bool AutoCache
		{
			get { return autoCache; }
			set { autoCache = value; }
		}

		/// <summary>
		/// Initialize the cache
		/// </summary>
		static CRC32()
		{
			cachedCRC32Tables = Hashtable.Synchronized(new Hashtable());
			autoCache = true;
		}

		public static void ClearCache()
		{
			cachedCRC32Tables.Clear();
		}


		/// <summary>
		/// Builds a crc32 table given a polynomial
		/// </summary>
		/// <param name="ulPolynomial"></param>
		/// <returns></returns>
		protected static uint[] BuildCRC32Table(uint ulPolynomial)
		{
			uint[] table = new uint[256];

			// 256 values representing ASCII character codes. 
			for (int i = 0; i < 256; i++)
			{
				uint dwCrc = (uint)i;
				for (int j = 8; j > 0; j--)
				{
					if ((dwCrc & 1) == 1)
						dwCrc = (dwCrc >> 1) ^ ulPolynomial;
					else
						dwCrc >>= 1;
				}
				table[i] = dwCrc;
			}

			return table;
		}


		/// <summary>
		/// Creates a CRC32 object using the DefaultPolynomial
		/// </summary>
		public CRC32()
			: this(DefaultPolynomial)
		{
			m_systemID = null;
		}

		/// <summary>
		/// Creates a CRC32 object using the DefaultPolynomial
		/// </summary>
		public CRC32(string systemID)
			: this(DefaultPolynomial)
		{
			m_systemID = systemID;
		}

		/// <summary>
		/// Creates a CRC32 object using the specified Creates a CRC32 object 
		/// </summary>
		public CRC32(uint aPolynomial)
			: this(aPolynomial, AutoCache)
		{
		}

		/// <summary>
		/// Construct the 
		/// </summary>
		public CRC32(uint aPolynomial, bool cacheTable)
		{
			this.HashSizeValue = 32;

			crc32Table = (uint[])cachedCRC32Tables[aPolynomial];
			if (crc32Table == null)
			{
				crc32Table = BuildCRC32Table(aPolynomial);
				if (cacheTable)
					cachedCRC32Tables.Add(aPolynomial, crc32Table);
			}
			Initialize();
		}

		/// <summary>
		/// Initializes an implementation of HashAlgorithm.
		/// </summary>
		public override void Initialize()
		{
			m_crc = AllOnes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		protected override void HashCore(byte[] buffer, int offset, int count)
		{
			// Save the text in the buffer. 
			for (int i = offset; i < count; i++)
			{
				ulong tabPtr = (m_crc & 0xFF) ^ buffer[i];
				m_crc >>= 8;
				m_crc ^= crc32Table[tabPtr];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override byte[] HashFinal()
		{
			byte[] finalHash = new byte[4];
			ulong finalCRC = m_crc ^ AllOnes;

			finalHash[0] = (byte)((finalCRC >> 24) & 0xFF);
			finalHash[1] = (byte)((finalCRC >> 16) & 0xFF);
			finalHash[2] = (byte)((finalCRC >> 8) & 0xFF);
			finalHash[3] = (byte)((finalCRC >> 0) & 0xFF);

			return finalHash;
		}

		/// <summary>
		/// Computes the hash value for the specified Stream.
		/// </summary>
		public new byte[] ComputeHash(Stream inputStream)
		{
			long size = inputStream.Length;
			long elapsedSize = 0;
			long percentSeg = 0, prevPercent = 0;

			byte[] buffer = new byte[DEFAULT_BUFF_SIZE];
			int bytesRead;
			while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				HashCore(buffer, 0, bytesRead);
				elapsedSize += bytesRead;

				// 전체크기의 5% 마다 로그 기록
				long percent = elapsedSize * 100 / size;

				if (percent > prevPercent)	// 디버그 모드에서는 1% 마다 기록
				{
					if (percent / 5 > percentSeg)
					{
						if (m_systemID != null)
							Console.WriteLine(m_systemID + " [CRC] creating crc32...(" + percent + "% done.)");
						percentSeg = percent / 5;
					}
					else
					{
						if (m_systemID != null)
							Console.WriteLine(m_systemID + " [CRC] creating crc32...(" + percent + "% done.)");
					}
					prevPercent = percent;
				}
			}
			return HashFinal();
		}

		/// <summary>
		/// Overloaded. Computes the hash value for the input data.
		/// </summary>
		public new byte[] ComputeHash(byte[] buffer)
		{
			return ComputeHash(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Overloaded. Computes the hash value for the input data.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public new byte[] ComputeHash(byte[] buffer, int offset, int count)
		{
			HashCore(buffer, offset, count);
			return HashFinal();
		}

		/// <summary>
		/// Creates the CRC32.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="systemID">The system ID.</param>
		/// <returns></returns>
		public static string CreateCRC32(string filePath, string systemID)
		{
			FileStream fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read);

			// 먼저 CRC 파일을 생성한다.
			CRC32 crc = new CRC32(systemID);
			crc.Initialize();
			byte[] hash = crc.ComputeHash(fsIn);
			string sHash = CryptUtil.GetHexFromByte(hash);
			fsIn.Close();
			Console.WriteLine(systemID + " [CRC] CRC32: " + sHash);

			return sHash;
		}

		/// <summary>
		/// Creates the CRC32.
		/// </summary>
		/// <param name="orgString">The org string.</param>
		/// <returns></returns>
		public static string CreateCRC32(string orgString)
		{
			CRC32 crc = new CRC32("{String}");
			crc.Initialize();
			byte[] hash = crc.ComputeHash(Encoding.Default.GetBytes(orgString));
			string sHash = CryptUtil.GetHexFromByte(hash);
			Console.WriteLine("{String} [CRC] CRC32: " + sHash);

			return sHash;
		}

		/// <summary>
		/// Creates the CRC32.
		/// </summary>
		/// <param name="orgBytes">The org bytes.</param>
		/// <returns></returns>
		public static string CreateCRC32(byte[] orgBytes)
		{
			CRC32 crc = new CRC32("{Bytes}");
			crc.Initialize();
			byte[] hash = crc.ComputeHash(orgBytes);
			string sHash = CryptUtil.GetHexFromByte(hash);
			Console.WriteLine("{Bytes} [CRC] CRC32: " + sHash);

			return sHash;
		}

		static void Main()
		{
			//string filePath = @"\\Ns80gdm3\ufs01\Content\sdcp02\138\wav_2d304834-e863-cf4d-925d-4f4a78f79053_audio.mxf";
			string filePath = @".\CRC32.exe";
			DateTime start = DateTime.Now;
			Console.WriteLine("Start... " + start.ToString("yyyy-MM-dd HH:mm:ss"));
			CreateCRC32(filePath, "Distribution");
			DateTime end = DateTime.Now;
			Console.WriteLine("End... " + end.ToString("yyyy-MM-dd HH:mm:ss"));
			Console.WriteLine("Elapsed time: " + end.Subtract(start).TotalSeconds + " sec(s).");
			
			//Console.WriteLine("Start... " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			//FileStream fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			//HashAlgorithm md5 = new MD5CryptoServiceProvider();
			//byte[] hash = md5.ComputeHash(fsIn);
			//string sHash = CryptUtil.GetHexFromByte(hash);
			//fsIn.Close();
			//Console.WriteLine("[MD5] MD5: " + sHash);
			//Console.WriteLine("End... " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

			//DateTime start = DateTime.Now;
			//int count = 0;
			//FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			//byte[] buffer = new byte[DEFAULT_BUFF_SIZE];
			//int bytesRead;
			//while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
			//{
			//    count++;
			//    Console.WriteLine(count);
			//}
			//inputStream.Close();
			//DateTime end = DateTime.Now;
			//Console.WriteLine("End... " + end.ToString("yyyy-MM-dd HH:mm:ss"));
			//Console.WriteLine("Elapsed time: " + end.Subtract(start).TotalSeconds + " sec(s).");
		}
	}
}
