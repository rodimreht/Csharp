using System;
using System.Security.Cryptography;
using System.Text;

namespace PublicKey
{
	/// <summary>
	/// TestMain에 대한 요약 설명입니다.
	/// </summary>
	public class TestMain
	{
		private static void Main()
		{
            // 512비트 RSA 키쌍 생성
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512);
			rsa.PersistKeyInCsp = true;

            // 공개키(e, n) 추출
            RSAParameters param = rsa.ExportParameters(false);
			string e = CryptUtil.GetHexFromByte(param.Exponent);
			string n = CryptUtil.GetHexFromByte(param.Modulus);
			/*
			string e = "10001";
			string n = "30db31542ace0f7d37a629ee5eba28cb";
			*/

			string s = "test:1111";

            // 공개키로 암호화
			nRSA nRsa = new nRSA(e, n);
			string encTemp = nRsa.Encrypt(s);
			Console.WriteLine("encTemp: " + encTemp);

            // 개인키(d, n) 추출
            RSAParameters param2 = rsa.ExportParameters(true);
			string d = CryptUtil.GetHexFromByte(param2.D);
			/*
			string d = "202700adbd85e2d7182720c3a0ee19c1";
			*/

            // 개인키로 복호화
			nRSA nRsa2 = new nRSA(e, d, n);
			string decTemp = nRsa2.Decrypt(encTemp);
			Console.WriteLine("decTemp: " + decTemp);


            // 개인키로 암호화(전자서명)
		    nRSA nRsa3 = new nRSA(d, n);
		    encTemp = nRsa3.Encrypt(s);
		    Console.WriteLine("encTemp: " + encTemp);

		    // 공개키로 복호화(서명 검증)
		    nRSA nRsa4 = new nRSA(null, e, n);
		    decTemp = nRsa4.Decrypt(encTemp);
		    Console.WriteLine("decTemp: " + decTemp);

            // rsa 자체 테스트
            byte[] enc = rsa.Encrypt(Encoding.Default.GetBytes(s), false);
			byte[] dec = rsa.Decrypt(enc, false);
			decTemp = Encoding.Default.GetString(dec);
		}
	}
}