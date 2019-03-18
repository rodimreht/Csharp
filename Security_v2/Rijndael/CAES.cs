using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace nAES
{
	/// <summary>
	/// CAES에 대한 요약 설명입니다.
	/// </summary>
	public class CAES
	{
		private const int AES128_BIT_LENGTH = 128;
		private const int AES192_BIT_LENGTH = 192;
		private const int AES256_BIT_LENGTH = 256;

		// AES 클래스에서는 초기화 벡터(IV)를 자동으로 생성하여 암호화 블럭 최초 16바이트에
		// 삽입하므로 앞의 Rijndael 클래스의 결과와 다르게 나온다.
		private Rijndael getRijndael(string sKey, int bitLength)
		{
			byte[] bRjnKey = GetKey(sKey, bitLength);
			byte[] bRjnIV = GetIV(sKey, bitLength); // 초기화 벡터도 똑같은 키를 사용한다.

			Rijndael r = Rijndael.Create();
			r.BlockSize = bitLength;
			r.KeySize = bitLength;
			r.Mode = CipherMode.CBC;
			r.Padding = PaddingMode.PKCS7;
			r.Key = bRjnKey;
			r.IV = bRjnIV;

			return r;
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aesEncryptString(string sKey, string sOrg)
		{
			return aes128EncryptString(sKey, sOrg);
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aesDecryptString(string sKey, string sOrg)
		{
			return aes128DecryptString(sKey, sOrg);
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aes128EncryptString(string sKey, string sOrg)
		{
			byte[] bTemp = Encoding.Default.GetBytes(sOrg);
			MemoryStream msIn = new MemoryStream(bTemp, false);
			long nLength = msIn.Length;

			Rijndael aes = getRijndael(sKey, AES128_BIT_LENGTH);
			MemoryStream msOut = new MemoryStream();
			CryptoStream cs = new CryptoStream(msOut, aes.CreateEncryptor(), CryptoStreamMode.Write);

			long nBytesProcessed = 0;
			try
			{
				do
				{
					byte[] byteBuffer = new byte[4096];
					int iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());
				} while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				return CryptUtil.GetHexFromByte(byteBuffer2);
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aes192EncryptString(string sKey, string sOrg)
		{
			byte[] bTemp = Encoding.Default.GetBytes(sOrg);
			MemoryStream msIn = new MemoryStream(bTemp, false);
			long nLength = msIn.Length;

			Rijndael aes = getRijndael(sKey, AES192_BIT_LENGTH);
			MemoryStream msOut = new MemoryStream();
			CryptoStream cs = new CryptoStream(msOut, aes.CreateEncryptor(), CryptoStreamMode.Write);

			long nBytesProcessed = 0;
			try
			{
				do
				{
					byte[] byteBuffer = new byte[4096];
					int iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());
				} while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				return CryptUtil.GetHexFromByte(byteBuffer2);
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aes256EncryptString(string sKey, string sOrg)
		{
			byte[] bTemp = Encoding.Default.GetBytes(sOrg);
			MemoryStream msIn = new MemoryStream(bTemp, false);
			long nLength = msIn.Length;

			Rijndael aes = getRijndael(sKey, AES256_BIT_LENGTH);
			MemoryStream msOut = new MemoryStream();
			CryptoStream cs = new CryptoStream(msOut, aes.CreateEncryptor(), CryptoStreamMode.Write);

			long nBytesProcessed = 0;
			try
			{
				do
				{
					byte[] byteBuffer = new byte[4096];
					int iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());
				} while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				return CryptUtil.GetHexFromByte(byteBuffer2);
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aes128DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = CryptUtil.GetHexArray(sOrg);
			MemoryStream msIn = new MemoryStream(bTemp, false);
			long nLength = msIn.Length;

			Rijndael aes = getRijndael(sKey, AES128_BIT_LENGTH);
			MemoryStream msOut = new MemoryStream();
			CryptoStream cs = new CryptoStream(msOut, aes.CreateDecryptor(), CryptoStreamMode.Write);

			long nBytesProcessed = 0;
			try
			{
				do
				{
					byte[] byteBuffer = new byte[4096];
					int iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());
				} while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				return Encoding.Default.GetString(byteBuffer2);
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aes192DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = CryptUtil.GetHexArray(sOrg);
			MemoryStream msIn = new MemoryStream(bTemp, false);
			long nLength = msIn.Length;

			Rijndael aes = getRijndael(sKey, AES192_BIT_LENGTH);
			MemoryStream msOut = new MemoryStream();
			CryptoStream cs = new CryptoStream(msOut, aes.CreateDecryptor(), CryptoStreamMode.Write);

			long nBytesProcessed = 0;
			try
			{
				do
				{
					byte[] byteBuffer = new byte[4096];
					int iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());
				} while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				return Encoding.Default.GetString(byteBuffer2);
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aes256DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = CryptUtil.GetHexArray(sOrg);
			MemoryStream msIn = new MemoryStream(bTemp, false);
			long nLength = msIn.Length;

			Rijndael aes = getRijndael(sKey, AES256_BIT_LENGTH);
			MemoryStream msOut = new MemoryStream();
			CryptoStream cs = new CryptoStream(msOut, aes.CreateDecryptor(), CryptoStreamMode.Write);

			long nBytesProcessed = 0;
			try
			{
				do
				{
					byte[] byteBuffer = new byte[4096];
					int iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());
				} while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				return Encoding.Default.GetString(byteBuffer2);
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

        /// <summary>
        /// 문자열 키 값을 정해진 길이의 바이트 배열로 변환한다.
        /// </summary>
        /// <param name="sKey">입력 문자열</param>
        /// <param name="bitLength">Length of the bit.</param>
        /// <returns>
        /// 출력 바이트 배열
        /// </returns>
        public byte[] GetKey(string sKey, int bitLength)
		{
            int size = bitLength / 8;
            byte[] byteTemp = new byte[size];

			// 길이가 16자가 아니면 오른쪽을 여백을 채워서 16자(128비트)를 맞춤.
			if (sKey.Length > size) sKey = sKey.Substring(0, size);
			sKey = sKey.PadRight(size);

			// sPassword를 ASCII 코드에 해당하는 Integer로 Encoding 한후, byteTemp 대입.
			byteTemp = Encoding.ASCII.GetBytes(sKey);

			return byteTemp;
		}

        /// <summary>
        /// 문자열 키 값에서 초기화 벡터 값을 얻는다.
        /// </summary>
        /// <param name="sKey">입력 문자열</param>
        /// <param name="bitLength">Length of the bit.</param>
        /// <returns>
        /// 출력 바이트 배열
        /// </returns>
        public byte[] GetIV(string sKey, int bitLength)
        {
            int size = bitLength / 8;
            byte[] byteTemp = new byte[size];

			// 길이가 16자가 아니면 오른쪽을 여백을 채워서 16자(128비트)를 맞춤.
			if (sKey.Length > size) sKey = sKey.Substring(0, size);
			sKey = sKey.PadRight(size);

			// sPassword를 ASCII 코드에 해당하는 Integer로 Encoding 한후, byteTemp 대입.
			byteTemp = Encoding.ASCII.GetBytes(sKey);

			return byteTemp;
		}
	}
}