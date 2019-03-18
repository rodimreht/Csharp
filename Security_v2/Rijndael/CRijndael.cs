using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace nAES
{
	/// <summary>
	/// CRijndael�� ���� ��� �����Դϴ�.
	/// </summary>
	public class CRijndael
	{
		private const int RIJNDAEL_BIT_LENGTH = 128;

		public CRijndael()
		{
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ���� ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string rijndaelEncryptString(string sKey, string sOrg)
		{
			// ��ȣȭ ���μ��� �� �ʿ��� ����
			MemoryStream msIn;
			MemoryStream msOut = new MemoryStream();
			msOut.SetLength(0);

			byte[] bTemp = Encoding.Default.GetBytes(sOrg);
			msIn = new MemoryStream(bTemp, false);

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			byte[] bRjnKey = GetKey(sKey);
			byte[] bRjnIV = GetIV(sKey); // �ʱ�ȭ ���͵� �Ȱ��� Ű�� ����Ѵ�.

			Rijndael r = Rijndael.Create();
			r.BlockSize = RIJNDAEL_BIT_LENGTH;
			r.KeySize = RIJNDAEL_BIT_LENGTH;
			r.Mode = CipherMode.CBC;
			r.Padding = PaddingMode.PKCS7;
			CryptoStream cs = new CryptoStream(msOut, r.CreateEncryptor(bRjnKey, bRjnIV), CryptoStreamMode.Write);
			
			try
			{
				do
				{
					byteBuffer = new byte[4096];
					iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());

				}while (nBytesProcessed < nLength);

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);
				
				cs.Close();
				msIn.Close();
				msOut.Close();

				return CryptUtil.GetHexFromByte(byteBuffer2);
			}
			catch(Exception ex)
			{
				cs.Close();
				msIn.Close();
				msOut.Close();

				throw ex;
			}
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string rijndaelDecryptString(string sKey, string sOrg)
		{
			// ��ȣȭ ���μ��� �� �ʿ��� ����
			MemoryStream msIn;
			MemoryStream msOut = new MemoryStream();
			msOut.SetLength(0);

			byte[] bTemp = CryptUtil.GetHexArray(sOrg);
			msIn = new MemoryStream(bTemp, false);

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			byte[] bRjnKey = GetKey(sKey);
			byte[] bRjnIV = GetIV(sKey); // �ʱ�ȭ ���͵� �Ȱ��� Ű�� ����Ѵ�.

			Rijndael r = Rijndael.Create();
			r.BlockSize = RIJNDAEL_BIT_LENGTH;
			r.KeySize = RIJNDAEL_BIT_LENGTH;
			r.Mode = CipherMode.CBC;
			r.Padding = PaddingMode.PKCS7;
			CryptoStream cs = new CryptoStream(msOut, r.CreateDecryptor(bRjnKey, bRjnIV), CryptoStreamMode.Write);
			
			try
			{
				do
				{
					byteBuffer = new byte[4096];
					iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());

				}while (nBytesProcessed < nLength);

				// ���� ���۸��� ���ڵ� ����Ʈ�� ��Ʈ���� ����Ѵ�.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				cs.Close();
				msIn.Close();
				msOut.Close();

				return Encoding.Default.GetString(byteBuffer2);
			}
			catch(Exception ex)
			{
				cs.Close();
				msIn.Close();
				msOut.Close();

				throw ex;
			}
		}

		/// <summary>
		/// ���ڿ� Ű ���� ������ ������ ����Ʈ �迭�� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="sKey">�Է� ���ڿ�</param>
		/// <returns>��� ����Ʈ �迭</returns>
		public byte[] GetKey(string sKey)
		{
			int size = RIJNDAEL_BIT_LENGTH / 8;
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
		/// <returns>��� ����Ʈ �迭</returns>
		private byte[] GetIV(string sKey)
		{
			int size = 16;
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
