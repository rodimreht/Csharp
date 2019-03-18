using System;

namespace nAES
{
	/// <summary>
	/// CMain에 대한 요약 설명입니다.
	/// </summary>
	class CMain
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.WriteLine("Rijndael 알고리즘 테스트:");
			CRijndael rijndaelEnc = new CRijndael();
			string encData = rijndaelEnc.rijndaelEncryptString("testkey123", "testdatatestdata");
			Console.WriteLine(encData);
			string decData = rijndaelEnc.rijndaelDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = rijndaelEnc.rijndaelEncryptString("testkey123", "니미랄 조또 잘 되넹???");
			Console.WriteLine(encData);
			decData = rijndaelEnc.rijndaelDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			Console.WriteLine("AES128 알고리즘 테스트:");
			CAES aesEnc = new CAES();
			encData = aesEnc.aesEncryptString("testkey123", "testdata");
			Console.WriteLine(encData);
			decData = aesEnc.aesDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = aesEnc.aesEncryptString("testkey123", "니미랄 조또 잘 되넹???");
			Console.WriteLine(encData);
			decData = aesEnc.aesDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			Console.WriteLine("AES192 알고리즘 테스트:");
			encData = aesEnc.aes192EncryptString("testkey123", "testdata");
			Console.WriteLine(encData);
			decData = aesEnc.aes192DecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = aesEnc.aes192EncryptString("testkey123", "니미랄 조또 잘 되넹???");
			Console.WriteLine(encData);
			decData = aesEnc.aes192DecryptString("testkey123", encData);
			Console.WriteLine(decData);

			Console.WriteLine("AES256 알고리즘 테스트:");
			encData = aesEnc.aes256EncryptString("testkey123", "");
			Console.WriteLine(encData);
			decData = aesEnc.aes256DecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = aesEnc.aes256EncryptString("testkey123", "니미랄 조또 잘 되넹???");
			Console.WriteLine(encData);
			decData = aesEnc.aes256DecryptString("testkey123", encData);
			Console.WriteLine(decData);
		}
	}
}
