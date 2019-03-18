using System;

namespace PnPeople.Security
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
			//SEED seedEnc = new SEED();
			//string enc = seedEnc.seedEncryptString("1234567890123456", "AbcdefghijklmnoP");
			//Console.WriteLine(enc);
			//string dec = seedEnc.seedDecryptString("1234567890123456", enc);
			//Console.WriteLine(dec);
			SEED256 seedEnc = new SEED256();
			string enc = seedEnc.seedEncryptString("12345678901234567890123456789012", "AbcdefghijklmnoP");
			Console.WriteLine(enc);
			string dec = seedEnc.seedDecryptString("12345678901234567890123456789012", enc);
			Console.WriteLine(dec);
		}
	}
}
