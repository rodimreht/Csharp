using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace nAES
{
	/// <summary>
	/// CAES�� ���� ��� �����Դϴ�.
	/// </summary>
	public class CAES
	{
		private const int AES128_BIT_LENGTH = 128;
		private const int AES192_BIT_LENGTH = 192;
		private const int AES256_BIT_LENGTH = 256;

		// AES Ŭ���������� �ʱ�ȭ ����(IV)�� �ڵ����� �����Ͽ� ��ȣȭ �� ���� 16����Ʈ��
		// �����ϹǷ� ���� Rijndael Ŭ������ ����� �ٸ��� ���´�.
		private Rijndael getRijndael(string sKey, int bitLength)
		{
			byte[] bRjnKey = GetKey(sKey, bitLength);
			byte[] bRjnIV = GetIV(sKey, bitLength); // �ʱ�ȭ ���͵� �Ȱ��� Ű�� ����Ѵ�.

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
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ���� ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string aesEncryptString(string sKey, string sOrg)
		{
			return aes128EncryptString(sKey, sOrg);
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string aesDecryptString(string sKey, string sOrg)
		{
			return aes128DecryptString(sKey, sOrg);
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ���� ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
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

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
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
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ���� ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
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

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
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
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ���� ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
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

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
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
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
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

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
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
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
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

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
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
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
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

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
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
        /// ���ڿ� Ű ���� ������ ������ ����Ʈ �迭�� ��ȯ�Ѵ�.
        /// </summary>
        /// <param name="sKey">�Է� ���ڿ�</param>
        /// <param name="bitLength">Length of the bit.</param>
        /// <returns>
        /// ��� ����Ʈ �迭
        /// </returns>
        public byte[] GetKey(string sKey, int bitLength)
		{
            int size = bitLength / 8;
            byte[] byteTemp = new byte[size];

			// ���̰� 16�ڰ� �ƴϸ� �������� ������ ä���� 16��(128��Ʈ)�� ����.
			if (sKey.Length > size) sKey = sKey.Substring(0, size);
			sKey = sKey.PadRight(size);

			// sPassword�� ASCII �ڵ忡 �ش��ϴ� Integer�� Encoding ����, byteTemp ����.
			byteTemp = Encoding.ASCII.GetBytes(sKey);

			return byteTemp;
		}

        /// <summary>
        /// ���ڿ� Ű ������ �ʱ�ȭ ���� ���� ��´�.
        /// </summary>
        /// <param name="sKey">�Է� ���ڿ�</param>
        /// <param name="bitLength">Length of the bit.</param>
        /// <returns>
        /// ��� ����Ʈ �迭
        /// </returns>
        public byte[] GetIV(string sKey, int bitLength)
        {
            int size = bitLength / 8;
            byte[] byteTemp = new byte[size];

			// ���̰� 16�ڰ� �ƴϸ� �������� ������ ä���� 16��(128��Ʈ)�� ����.
			if (sKey.Length > size) sKey = sKey.Substring(0, size);
			sKey = sKey.PadRight(size);

			// sPassword�� ASCII �ڵ忡 �ش��ϴ� Integer�� Encoding ����, byteTemp ����.
			byteTemp = Encoding.ASCII.GetBytes(sKey);

			return byteTemp;
		}
	}
}