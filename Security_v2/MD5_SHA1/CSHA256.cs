using System;
using System.Globalization;
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

			sOut = CryptUtil.GetHexFromByte(byteBuffer);
		}
	}
}
