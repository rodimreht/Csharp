using System.Security.Cryptography;
using System.Text;
using Microsoft.Web.Services2.Security.Cryptography;

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

		public CAES()
		{
		}

		// AES Ŭ���������� �ʱ�ȭ ����(IV)�� �ڵ����� �����Ͽ� ��ȣȭ �� ���� 16����Ʈ��
		// �����ϹǷ� ���� Rijndael Ŭ������ ����� �ٸ��� ���´�.
		private Rijndael getRijndael(string sKey, int bitLength)
		{
			byte[] bRjnKey = GetKey(sKey, bitLength);
			byte[] bRjnIV = GetIV(sKey); // �ʱ�ȭ ���͵� �Ȱ��� Ű�� ����Ѵ�.

			Rijndael r = RijndaelManaged.Create();
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

			AES128 aes = new AES128();
			aes.Key = getRijndael(sKey, AES128_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Encrypt(bTemp);

			return GetHexFromByte(buffer);
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string aes128DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = GetHexArray(sOrg);

			AES128 aes = new AES128();
			aes.Key = getRijndael(sKey, AES128_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Decrypt(bTemp);

			return Encoding.Default.GetString(buffer);
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

			AES192 aes = new AES192();
			aes.Key = getRijndael(sKey, AES192_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Encrypt(bTemp);

			return GetHexFromByte(buffer);
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string aes192DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = GetHexArray(sOrg);

			AES192 aes = new AES192();
			aes.Key = getRijndael(sKey, AES192_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Decrypt(bTemp);

			return Encoding.Default.GetString(buffer);
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

			AES256 aes = new AES256();
			aes.Key = getRijndael(sKey, AES256_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Encrypt(bTemp);

			return GetHexFromByte(buffer);
		}

		/// <summary>
		/// ��ȣȭ ó�� �Լ�
		/// </summary>
		/// <param name="sKey">���Ű</param>
		/// <param name="sOrg">�Է� ��ȣȭ ���ڿ�</param>
		/// <returns>��ȣȭ�� ���ڿ�</returns>
		public string aes256DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = GetHexArray(sOrg);

			AES256 aes = new AES256();
			aes.Key = getRijndael(sKey, AES256_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Decrypt(bTemp);

			return Encoding.Default.GetString(buffer);
		}

		/// <summary>
		/// ���ڿ� Ű ���� ������ ������ ����Ʈ �迭�� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="sKey">�Է� ���ڿ�</param>
		/// <returns>��� ����Ʈ �迭</returns>
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
		/// <returns>��� ����Ʈ �迭</returns>
		public byte[] GetIV(string sKey)
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
