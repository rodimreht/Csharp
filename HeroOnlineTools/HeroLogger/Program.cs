using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeroLogger
{
	static class Program
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string str = Application.ExecutablePath + " ";
			foreach (string s in args)
			{
				str += s + " ";
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1(str));
		}
	}
}