using System;
using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace TripleDES
{
	/// <summary>
	/// TripleDES ��ȣȭ �˰��� Ŭ����
	/// </summary>
	public class CTripleDES
	{
		private const int TDES_BIT_LENGTH = 192;	// 192��Ʈ ��ȣȭ (56 * 3 = 168bit)
		//private const int TDES_BIT_LENGTH = 128;

		public CTripleDES()
		{
			//
			// TODO: ���⿡ ������ ���� �߰��մϴ�.
			//
		}

		/// <summary>
		/// ���ڿ� ��ȣȭ / ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sIn">�Է� ���ڿ�</param>
		/// <param name="sOut">��� ���ڿ�</param>
		/// <param name="bDESKey">��� Ű</param>
		/// <param name="bDESIV">�ʱ�ȭ ����</param>
		/// <param name="sMethod">��ȣȭ / ��ȣȭ ����</param>
		public void EncryptDecryptString(string sIn, out string sOut, byte[] bDESKey, byte[] bDESIV, string sMethod)
		{
			// ��ȣȭ/��ȣȭ ���μ��� �� �ʿ��� ����
			MemoryStream msIn;
			MemoryStream msOut = new MemoryStream();
			msOut.SetLength(0);

			if (sMethod == "E")
			{
				byte[] bTemp = Encoding.Default.GetBytes(sIn);
				msIn = new MemoryStream(bTemp, false);
			}
			else
			{
				string[] sTemp = sIn.ToUpper().Split('+');
				byte[] bTemp = new byte[sTemp.Length];

				for (int i = 0; i < sTemp.Length; i++)
				{
					bTemp[i] = GetByteFromHex(sTemp[i]);
				}
				msIn = new MemoryStream(bTemp, false);
			}

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			CryptoStream cs = null;
			
			switch (sMethod)
			{
				case "E":
					// ��ȣȭ�� ���� ����
					cs = new CryptoStream(msOut, tdes.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D":
					// ��ȣȭ�� ���� ����
					cs = new CryptoStream(msOut, tdes.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				default:
					break;
			}
			
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

				if (sMethod == "E")
				{
					msOut.Position = 0;
					byte[] byteBuffer2 = new byte[msOut.Length];
					msOut.Read(byteBuffer2, 0, (int) msOut.Length);
				
					sOut = GetHexFromByte(byteBuffer2);
				}
				else
				{
					msOut.Position = 0;
					byte[] byteBuffer2 = new byte[msOut.Length];
					msOut.Read(byteBuffer2, 0, (int) msOut.Length);

					sOut = Encoding.Default.GetString(byteBuffer2);
				}
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// �޸� ��Ʈ�� ��ȣȭ / ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="msIn">�Է� ��Ʈ��</param>
		/// <param name="msOut">��� ��Ʈ��</param>
		/// <param name="bDESKey">��� Ű</param>
		/// <param name="bDESIV">�ʱ�ȭ ����</param>
		/// <param name="sMethod">��ȣȭ / ��ȣȭ ����</param>
		public void EncryptDecryptStream(MemoryStream msIn, out MemoryStream msOut, byte[] bDESKey, byte[] bDESIV, string sMethod)
		{
			// ��ȣȭ/��ȣȭ ���μ��� �� �ʿ��� ����
			MemoryStream msTemp = new MemoryStream();
			msTemp.SetLength(0);

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			CryptoStream cs = null;
			
			switch (sMethod)
			{
				case "E":
					// ��ȣȭ�� ���� ����
					cs = new CryptoStream(msTemp, tdes.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D":
					// ��ȣȭ�� ���� ����
					cs = new CryptoStream(msTemp, tdes.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				default:
					break;
			}
			
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

				msOut = new MemoryStream();
				msTemp.WriteTo(msOut);
			}
			finally
			{
				cs.Close();
			}
		}

		/// <summary>
		/// ���� ��ȣȭ / ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sInputFile">�Է� ���� ���</param>
		/// <param name="sOutputFile">��� ���� ���</param>
		/// <param name="bDESKey">��� Ű</param>
		/// <param name="bDESIV">�ʱ�ȭ ����</param>
		/// <param name="strDirection">��ȣȭ / ��ȣȭ ����</param>
		public void EncryptDecryptFile(string sInputFile, string sOutputFile, byte[] bDESKey, byte[] bDESIV, string strDirection)
		{
			//���� ��Ʈ���� ����� �Է� �� ��� ������ ó��
			FileStream fsInput = new FileStream(sInputFile, FileMode.Open, FileAccess.Read);
			FileStream fsOutput = new FileStream(sOutputFile, FileMode.OpenOrCreate, FileAccess.Write);
			fsOutput.SetLength(0);

			//��ȣȭ/��ȣȭ ���μ��� �� �ʿ��� ����
			byte[] byteBuffer = new byte[4096];
			long nBytesProcessed = 0;
			long nFileLength = fsInput.Length;
			int iBytesInCurrentBlock;
			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			CryptoStream cs = null;
			
			// ��ȣȭ�� ��ȣȭ�� ���� ����
			switch (strDirection)
			{
				case "E" :
					cs = new CryptoStream(fsOutput, tdes.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D" :
					cs = new CryptoStream(fsOutput, tdes.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;
			}
			
			try
			{
				//�Է� ���Ͽ��� ���� ���� ��ȣȭ�ϰų� ��ȣ�� �ص��ϰ� ��� ���Ͽ� ��.
				do
				{
					iBytesInCurrentBlock = fsInput.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());

				}while (nBytesProcessed < nFileLength);
			}
			finally
			{
				cs.Close();
				fsInput.Close();
				fsOutput.Close();
			}
		}

		/// <summary>
		/// ���ڿ� Ű ���� ������ ������ ����Ʈ �迭�� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="sKey">�Է� ���ڿ�</param>
		/// <returns>��� ����Ʈ �迭</returns>
		public byte[] GetKeyByteArray(string sKey)
		{
			byte[] byteTemp = new byte[(int) (TDES_BIT_LENGTH / 8)];

			// ���̰� 24�ڰ� �ƴϸ� �������� ������ ä���� 24��(192��Ʈ)�� ����.
			if (sKey.Length > 24) sKey = sKey.Substring(0, 24);
			sKey = sKey.PadRight((int) (TDES_BIT_LENGTH / 8));

			// sPassword�� ASCII �ڵ忡 �ش��ϴ� Integer�� Encoding ����, byteTemp ����.
			byteTemp = Encoding.ASCII.GetBytes(sKey);
		
			return byteTemp;
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
				if (bBytes[i].ToString("X").Length < 2)
					sb.Append("0" + bBytes[i].ToString("X") + "+");
				else
					sb.Append(bBytes[i].ToString("X") + "+");
			}
			sb.Remove(sb.Length - 1, 1);
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
