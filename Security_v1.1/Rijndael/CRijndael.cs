using System;
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

			Rijndael r = RijndaelManaged.Create();
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

				return GetHexFromByte(byteBuffer2);
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

			byte[] bTemp = GetHexArray(sOrg);
			msIn = new MemoryStream(bTemp, false);

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			byte[] bRjnKey = GetKey(sKey);
			byte[] bRjnIV = GetIV(sKey); // �ʱ�ȭ ���͵� �Ȱ��� Ű�� ����Ѵ�.

			Rijndael r = RijndaelManaged.Create();
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

		/// <summary>
		/// ��ȣȭ�� ���ڿ��� ����Ʈ �迭�� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="sOrg"></param>
		/// <returns></returns>
		private byte[] GetHexArray(string sOrg)
		{
			byte[] bArray = null;

			// ���� '+' ���� �빮�� ��ȣȭ ���ڿ����� ȣȯ���� ���� ����
			if (sOrg.ToUpper().Replace(" ", "+").Replace("%2B", "+").IndexOf("+") > 0)
			{
				string[] sArray = sOrg.ToUpper().Split('+');
				bArray = new byte[sArray.Length];

				for (int i = 0; i < sArray.Length; i++)
				{
					bArray[i] = GetByteFromHex(sArray[i]);
				}
			}
			else
			{
				int len = sOrg.Length / 2;
				bArray = new byte[len];
				for (int i = 0; i < len; i++)
				{
					bArray[i] = GetByteFromHex(sOrg.Substring(i * 2, 2));
				}
			}
			return bArray;
		}

		/// <summary>
		/// ����Ʈ �迭�� 16���� ���ڿ��� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="bBytes">�Է� ����Ʈ �迭</param>
		/// <returns>':'�� ���еǴ� 16���� ���ڿ�</returns>
		public string GetHexFromByte(byte[] bBytes)
		{
			StringBuilder sb = new StringBuilder(bBytes.Length);
			for (int i = 0; i < bBytes.Length; i++)
			{
				if (bBytes[i].ToString("x").Length < 2)
					sb.Append("0" + bBytes[i].ToString("x"));
				else
					sb.Append(bBytes[i].ToString("x"));
			}
			//sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		/// <summary>
		/// 16���� ���ڿ�(2�ڸ� ��)�� byte�� ��ȯ
		/// </summary>
		/// <param name="sHex">16���� ���ڿ� ��</param>
		/// <returns>����Ʈ ��</returns>
		public byte GetByteFromHex(string sHex)
		{
			char[] cTemp = sHex.ToUpper().ToCharArray();
			byte bTemp = 0;
			for (int k = 0; k < cTemp.Length; k++)
			{
				switch (k)
				{
					case 0:
						if (cTemp[k] >= 'A')
						{
							bTemp += (byte) ((cTemp[k] - 'A' + 10) * 16);
						}
						else
						{
							bTemp += (byte) (int.Parse(cTemp[k].ToString()) * 16);
						}
						break;

					case 1:
						if (cTemp[k] >= 'A')
						{
							bTemp += (byte) (cTemp[k] - 'A' + 10);
						}
						else
						{
							bTemp += (byte) int.Parse(cTemp[k].ToString());
						}
						break;

					default:
						bTemp = 0;
						break;
				}
			}
			return bTemp;
		}
	}
}
