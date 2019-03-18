using System;

namespace Nets.Security
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
			SEED seedEnc = new SEED();
			string encData = seedEnc.seedEncryptString("", "테스트 문자열임돠...");
//			string encData = seedEnc.seedEncryptString("1234", "abcdefghijklmnopqrstuvwxyz");
			Console.WriteLine(encData);
			string decData = seedEnc.seedDecryptString("", encData);
			Console.WriteLine(decData);

			encData = seedEnc.seedEncryptString("testkey123", "니미랄 조또 잘 되넹???");
			Console.WriteLine(encData);
			decData = seedEnc.seedDecryptString("testkey123", encData);
			Console.WriteLine(decData);
		}
	}
}
