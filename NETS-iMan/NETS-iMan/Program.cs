using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NETS_iMan
{
	internal static class Program
	{
		// For Windows Mobile, replace user32.dll with coredll.dll 
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetForegroundWindow(IntPtr hWnd); 
		
		private static Mutex gMutex;

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				bool createdNew;
#if DEBUG
				gMutex = new Mutex(true, @"Global\NETS-iMan_DEBUG", out createdNew);
#else
				gMutex = new Mutex(true, @"Global\NETS-iMan", out createdNew);
#endif
				if (createdNew)
				{
					try
					{
						RegistryKey key1 = Registry.LocalMachine.OpenSubKey(
							@"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\StandardProfile\GloballyOpenPorts\List",
							true);
						if (key1 != null) key1.SetValue("6421:TCP", "6421:TCP:*:Enabled:NETS-iMan (TCP: 6421)", RegistryValueKind.String);
						RegistryKey key2 = Registry.LocalMachine.OpenSubKey(
							@"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\DomainProfile\GloballyOpenPorts\List",
							true);
						if (key2 != null) key2.SetValue("6421:TCP", "6421:TCP:*:Enabled:NETS-iMan (TCP: 6421)", RegistryValueKind.String);
					}
					catch
					{
					}

					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new frmMain());
				}
				else
				{
					Process currProcess = Process.GetCurrentProcess();
					foreach (Process temp in Process.GetProcessesByName("NETS-iMan"))
					{
						if (currProcess.Id != temp.Id)
						{
							if (currProcess.SessionId != temp.SessionId)
								MessageBox.Show("NETS-ⓘMan이 이미 실행되어 있습니다.", "중복실행", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							else
								SetForegroundWindow(temp.MainWindowHandle);
						}
					}
				}
				gMutex.ReleaseMutex();
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.WARNING, "프로세스 생성 실패: " + ex);
			}
		}
	}
}