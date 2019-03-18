using System;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace MD5
{
	/// <summary>
	/// CSHA256�� ���� ��� �����Դϴ�.
	/// </summary>
	public class CSHA256
	{
		public CSHA256()
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

			SHA256 sha256 = new SHA256Managed();
			byte[] byteBuffer = sha256.ComputeHash(byteSource);

			sOut = GetHexFromByte(byteBuffer);
		}

		/// <summary>
		/// ����Ʈ �迭�� 16���� ���ڿ��� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="bBytes">�Է� ����Ʈ �迭</param>
		/// <returns>2�ڸ� 16���� ���ڿ��� ������ ���ڿ�</returns>
		private string GetHexFromByte(byte[] bBytes)
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
		private byte GetByteFromHex(string sHex)
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
