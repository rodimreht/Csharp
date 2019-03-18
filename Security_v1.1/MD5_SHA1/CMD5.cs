using System;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace MD5
{
	/// <summary>
	/// MD5 ��ȣȭ �˰��� Ŭ����
	/// </summary>
	public class CMD5
	{
		public CMD5()
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
		public void EncryptDecryptString(string sIn, out string sOut)
		{
			byte[] byteSource;
			byteSource = Encoding.Default.GetBytes(sIn);

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] byteBuffer = md5.ComputeHash(byteSource);

			sOut = GetHexFromByte(byteBuffer);
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
