using System;
using System.Security.Cryptography;
using System.Text;

namespace PublicKey
{
	/// <summary>
	/// TestMain�� ���� ��� �����Դϴ�.
	/// </summary>
	public class TestMain
	{
		private static void Main()
		{
            // 512��Ʈ RSA Ű�� ����
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512);
			rsa.PersistKeyInCsp = true;

            // ����Ű(e, n) ����
            RSAParameters param = rsa.ExportParameters(false);
			string e = CryptUtil.GetHexFromByte(param.Exponent);
			string n = CryptUtil.GetHexFromByte(param.Modulus);
			/*
			string e = "10001";
			string n = "30db31542ace0f7d37a629ee5eba28cb";
			*/

			string s = "test:1111";

            // ����Ű�� ��ȣȭ
			nRSA nRsa = new nRSA(e, n);
			string encTemp = nRsa.Encrypt(s);
			Console.WriteLine("encTemp: " + encTemp);

            // ����Ű(d, n) ����
            RSAParameters param2 = rsa.ExportParameters(true);
			string d = CryptUtil.GetHexFromByte(param2.D);
			/*
			string d = "202700adbd85e2d7182720c3a0ee19c1";
			*/

            // ����Ű�� ��ȣȭ
			nRSA nRsa2 = new nRSA(e, d, n);
			string decTemp = nRsa2.Decrypt(encTemp);
			Console.WriteLine("decTemp: " + decTemp);


            // ����Ű�� ��ȣȭ(���ڼ���)
		    nRSA nRsa3 = new nRSA(d, n);
		    encTemp = nRsa3.Encrypt(s);
		    Console.WriteLine("encTemp: " + encTemp);

		    // ����Ű�� ��ȣȭ(���� ����)
		    nRSA nRsa4 = new nRSA(null, e, n);
		    decTemp = nRsa4.Decrypt(encTemp);
		    Console.WriteLine("decTemp: " + decTemp);

            // rsa ��ü �׽�Ʈ
            byte[] enc = rsa.Encrypt(Encoding.Default.GetBytes(s), false);
			byte[] dec = rsa.Decrypt(enc, false);
			decTemp = Encoding.Default.GetString(dec);
		}
	}
}