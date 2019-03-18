using System;

namespace Nets.Security
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
			SEED seedEnc = new SEED();
			string encData = seedEnc.seedEncryptString("", "�׽�Ʈ ���ڿ��ӵ�...");
//			string encData = seedEnc.seedEncryptString("1234", "abcdefghijklmnopqrstuvwxyz");
			Console.WriteLine(encData);
			string decData = seedEnc.seedDecryptString("", encData);
			Console.WriteLine(decData);

			encData = seedEnc.seedEncryptString("testkey123", "�Ϲ̶� ���� �� �ǳ�???");
			Console.WriteLine(encData);
			decData = seedEnc.seedDecryptString("testkey123", encData);
			Console.WriteLine(decData);
		}
	}
}
