using System;

namespace nAES
{
	/// <summary>
	/// CMain�� ���� ��� �����Դϴ�.
	/// </summary>
	class CMain
	{
		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.WriteLine("Rijndael �˰��� �׽�Ʈ:");
			CRijndael rijndaelEnc = new CRijndael();
			string encData = rijndaelEnc.rijndaelEncryptString("testkey123", "testdatatestdata");
			Console.WriteLine(encData);
			string decData = rijndaelEnc.rijndaelDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = rijndaelEnc.rijndaelEncryptString("testkey123", "�Ϲ̶� ���� �� �ǳ�???");
			Console.WriteLine(encData);
			decData = rijndaelEnc.rijndaelDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			Console.WriteLine("AES128 �˰��� �׽�Ʈ:");
			CAES aesEnc = new CAES();
			encData = aesEnc.aesEncryptString("testkey123", "testdata");
			Console.WriteLine(encData);
			decData = aesEnc.aesDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = aesEnc.aesEncryptString("testkey123", "�Ϲ̶� ���� �� �ǳ�???");
			Console.WriteLine(encData);
			decData = aesEnc.aesDecryptString("testkey123", encData);
			Console.WriteLine(decData);

			Console.WriteLine("AES192 �˰��� �׽�Ʈ:");
			encData = aesEnc.aes192EncryptString("testkey123", "testdata");
			Console.WriteLine(encData);
			decData = aesEnc.aes192DecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = aesEnc.aes192EncryptString("testkey123", "�Ϲ̶� ���� �� �ǳ�???");
			Console.WriteLine(encData);
			decData = aesEnc.aes192DecryptString("testkey123", encData);
			Console.WriteLine(decData);

			Console.WriteLine("AES256 �˰��� �׽�Ʈ:");
			encData = aesEnc.aes256EncryptString("testkey123", "");
			Console.WriteLine(encData);
			decData = aesEnc.aes256DecryptString("testkey123", encData);
			Console.WriteLine(decData);

			encData = aesEnc.aes256EncryptString("testkey123", "�Ϲ̶� ���� �� �ǳ�???");
			Console.WriteLine(encData);
			decData = aesEnc.aes256DecryptString("testkey123", encData);
			Console.WriteLine(decData);
		}
	}
}
