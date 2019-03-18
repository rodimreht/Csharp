using System;
using System.Data;
using System.Globalization;
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
				byte[] bTemp = CryptUtil.GetHexArray(sIn);
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

					sOut = CryptUtil.GetHexFromByte(byteBuffer2);
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
	}
}
