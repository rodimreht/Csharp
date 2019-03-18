using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeroTCPRelay
{
	static class Program
	{
		public static bool AutoStart = false;

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if ((args.Length == 1) && (args[0].Equals("/S"))) AutoStart = true;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}