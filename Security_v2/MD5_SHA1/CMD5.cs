using System;
using System.Globalization;
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

			sOut = CryptUtil.GetHexFromByte(byteBuffer);
		}
	}
}
