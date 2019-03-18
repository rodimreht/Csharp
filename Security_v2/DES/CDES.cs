using System;
using System.Data;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace ex_Security
{
	/// <summary>
	/// DES ��ȣȭ �˰��� Ŭ����
	/// </summary>
	public class CDES
	{
		private const int DES_BIT_LENGTH = 64;	// 64��Ʈ ��ȣȭ (56bit)

		public CDES()
		{
			//
			// TODO: ���⿡ ������ ���� �߰��մϴ�.
			//
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
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			CryptoStream cs = null;
			
			// ��ȣȭ�� ��ȣȭ�� ���� ����
			switch (strDirection)
			{
				case "E" :
					cs = new CryptoStream(fsOutput, des.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D" :
					cs = new CryptoStream(fsOutput, des.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
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
					bTemp[i] = CryptUtil.GetByteFromHex(sTemp[i]);
				}
				msIn = new MemoryStream(bTemp, false);
			}

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			CryptoStream cs = null;
			
			switch (sMethod)
			{
				case "E":
					// ��ȣȭ�� ���� ����
					cs = new CryptoStream(msOut, des.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D":
					// ��ȣȭ�� ���� ����
					cs = new CryptoStream(msOut, des.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
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
		/// ���ڿ� Ű ���� ������ ������ ����Ʈ �迭�� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="sKey">�Է� ���ڿ�</param>
		/// <returns>��� ����Ʈ �迭</returns>
		public byte[] GetKeyByteArray(string sKey)
		{
			byte[] byteTemp = new byte[(int) (DES_BIT_LENGTH / 8)];

			// ���̰� 8�ڰ� �ƴϸ� ������ �ڸ��ų�,
			// �������� ������ ä���� 8��(64��Ʈ)�� ����.
			if (sKey.Length > 8) sKey = sKey.Substring(0, 8);
			sKey = sKey.PadRight((int) (DES_BIT_LENGTH / 8));

			// sPassword�� ASCII �ڵ忡 �ش��ϴ� Integer�� Encoding ����, byteTemp ����.
			byteTemp = Encoding.ASCII.GetBytes(sKey);
		
			return byteTemp;
		}
	}
}
