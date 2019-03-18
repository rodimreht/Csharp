using System;
using System.Windows.Forms;

namespace HeroMiner
{
	static class Program
	{
		public static KeyboardHook2 kh;

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (kh = new KeyboardHook2("PassAllKeysToNextApp"))
			{
				Application.Run(new frmMain());
			}
		}
	}
}