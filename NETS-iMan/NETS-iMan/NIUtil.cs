using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using NETS_iMan.Properties;

namespace NETS_iMan
{
	public class NIUtil
	{
		//Flash both the window caption and taskbar button.
		//This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
		public const UInt32 FLASHW_ALL = 3;
		public const UInt32 FLASHW_CAPTION = 1;
		public const UInt32 FLASHW_STOP = 0;
		//Flash continuously, until the FLASHW_STOP flag is set. 
		public const UInt32 FLASHW_TIMER = 4;
		//Flash continuously until the window comes to the foreground. 
		public const UInt32 FLASHW_TIMERNOFG = 12;
		public const UInt32 FLASHW_TRAY = 2;

		[DllImport("user32.dll")]
		private static extern Int32 FlashWindowEx(ref FLASHWINFO pwfi);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[DllImport("user32.dll")]
		private static extern int GetWindowRect(IntPtr hwnd, [MarshalAs(UnmanagedType.Struct)] ref RECT lpRect);

		/// <summary>
		/// 창을 깜박깜박하게 한다.
		/// </summary>
		/// <param name="frm">대상 창</param>
		/// <returns></returns>
		public static bool FlashWindowEx(Form frm)
		{
			IntPtr hWnd = frm.Handle;
			FLASHWINFO fInfo = new FLASHWINFO();

			fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
			fInfo.hwnd = hWnd;
			fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
			fInfo.uCount = UInt32.MaxValue;
			fInfo.dwTimeout = 0;

			return (FlashWindowEx(ref fInfo) != 0);
		}

		public static void LoadPrevSettings()
		{
			try
			{
				SettingsHelper settings = SettingsHelper.Current;
				if (!string.IsNullOrEmpty(settings.UserID)) return;

				Settings prevSettings = Settings.Default;
				string prevID = (string) prevSettings.GetPreviousVersion("userid");
				if (!string.IsNullOrEmpty(prevID)) settings.UserID = prevID;

				// 패스워드 암호화
				string pwd = (string) prevSettings.GetPreviousVersion("pwd");
				if ((pwd.Length > 0) && (!pwd.StartsWith("enc:")))
				{
					if (NISecurity.IsStrongKey(prevID))
					{
						pwd = "enc:" + NISecurity.Encrypt(prevID, pwd);
					}
					else
					{
						string key = prevID + "12345678";
						pwd = "enc:" + NISecurity.Encrypt(key, pwd);
					}
				}
				settings.Password = pwd;

				string val = (string) prevSettings.GetPreviousVersion("font");
				if (!string.IsNullOrEmpty(val)) settings.Font = val;

				val = (string) prevSettings.GetPreviousVersion("color");
				if (!string.IsNullOrEmpty(val)) settings.Color = val;

				if (prevSettings.GetPreviousVersion("autoStart") != null)
					settings.AutoStart = (bool) prevSettings.GetPreviousVersion("autoStart");

				// 자동실행 옵션이 있으면 레지스트리 업데이트
				if (settings.AutoStart)
				{
					RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
					if (key != null)
					{
						string s = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
						s += @"\NETS\NETS-ⓘMan v1.0.appref-ms";
						key.SetValue("NETS_iMan", "\"" + s + "\"");
						key.Close();
					}
					else
					{
						settings.AutoStart = false;
					}
				}

				if (prevSettings.GetPreviousVersion("remPwd") != null)
					settings.RememberPwd = (bool) prevSettings.GetPreviousVersion("remPwd");

				val = (string) prevSettings.GetPreviousVersion("mainWndPos");
				if (!string.IsNullOrEmpty(val)) settings.MainWindowPosition = val;

				val = (string) prevSettings.GetPreviousVersion("mainWndSize");
				if (!string.IsNullOrEmpty(val)) settings.MainWindowSize = val;

				if (prevSettings.GetPreviousVersion("showOnline") != null)
					settings.ShowOnline = (bool) prevSettings.GetPreviousVersion("showOnline");

				if (prevSettings.GetPreviousVersion("alwaysTopMost") != null)
					settings.AlwaysTopMost = (bool) prevSettings.GetPreviousVersion("alwaysTopMost");

				if (prevSettings.GetPreviousVersion("offLoginAlarm") != null)
					settings.OffLoginAlarm = (bool) prevSettings.GetPreviousVersion("offLoginAlarm");

				string qaPwd = (string)prevSettings.GetPreviousVersion("qaPwd");
				if (qaPwd != null)
				{
					if ((qaPwd.Length > 0) && (!qaPwd.StartsWith("enc:")))
					{
						if (NISecurity.IsStrongKey(prevID))
						{
							qaPwd = "enc:" + NISecurity.Encrypt(prevID, qaPwd);
						}
						else
						{
							string key = prevID + "12345678";
							qaPwd = "enc:" + NISecurity.Encrypt(key, qaPwd);
						}
					}
					settings.NETSQAPassword = qaPwd;
				}

				if (prevSettings.GetPreviousVersion("formOpacity") != null)
					settings.FormOpacity = (int)prevSettings.GetPreviousVersion("formOpacity");

				if (prevSettings.GetPreviousVersion("chatOpacity") != null)
					settings.ChatOpacity = (int)prevSettings.GetPreviousVersion("chatOpacity");
				else
					settings.ChatOpacity = settings.FormOpacity;

				settings.Save(); // -- 이전 버전 환경설정에서 읽을 때는 바로 저장
			}
			catch
			{
			}
		}

		public static string GetTime(int dur)
		{
			string theTime = "";
			int dd = dur/(3600*24);
			if (dd > 0) theTime += dd + "일 ";
			int hh = (dur - dd*3600*24)/3600;
			if (hh > 0) theTime += hh + "시간 ";
			int mm = (dur - hh*3600)/60;
			if (mm > 0) theTime += mm + "분 ";
			int ss = dur - hh*3600 - mm*60;
			theTime += ss + "초";
			return theTime;
		}

		public static string GetThisHostIP()
		{
			ArrayList ipArray = new ArrayList();
			foreach (NetworkInterface nif in NetworkInterface.GetAllNetworkInterfaces())
			{
				foreach (GatewayIPAddressInformation gateway in nif.GetIPProperties().GatewayAddresses)
				{
					ipArray.Add(gateway.Address.ToString());
				}
			}

			// IP주소가 여러개이면 게이트웨이 주소와 C클래스까지 같은 첫번째 IP주소를 사용한다.
			IPAddress[] ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress ip in ips)
			{
				string hostIP = ip.ToString();
				for (int i = 0; i < ipArray.Count; i++)
				{
					string gw = (string) ipArray[i];
					gw = gw.Substring(0, gw.LastIndexOf("."));
					if (ip.ToString().StartsWith(gw))
						return hostIP;
				}
			}
			return ips[0].ToString();
		}

		public static bool IsSameNetworkWithMine(string remoteIP)
		{
			ArrayList ipArray = new ArrayList();
			foreach (NetworkInterface nif in NetworkInterface.GetAllNetworkInterfaces())
			{
				foreach (GatewayIPAddressInformation gateway in nif.GetIPProperties().GatewayAddresses)
				{
					ipArray.Add(gateway.Address.ToString());
				}
			}

			// IP주소가 여러개이면 게이트웨이 주소와 C클래스까지 같은 첫번째 IP주소를 사용한다.
			IPAddress[] ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress ip in ips)
			{
				for (int i = 0; i < ipArray.Count; i++)
				{
					string gw = (string)ipArray[i];
					gw = gw.Substring(0, gw.LastIndexOf("."));
					if (ip.ToString().StartsWith(gw)) return remoteIP.StartsWith(gw);
				}
			}
			return false;
		}

		public static bool IsMaxWindow()
		{
			IntPtr hWnd = GetForegroundWindow();
			if (hWnd == IntPtr.Zero) return false;

			// 바탕화면인 경우 -->
			IntPtr hWnd2 = GetDesktopWindow();
			if (hWnd2 == IntPtr.Zero) return false;
			if (hWnd == hWnd2) return false;

			StringBuilder sb = new StringBuilder(256);
			GetClassName(hWnd, sb, 255);

			string className = sb.ToString().ToLower();
            if ((className == "progman") || (className == "workerw")) return false;
			// <--

			foreach (Form frm in Application.OpenForms)
				if (frm.Handle == hWnd) return false;

			RECT rect = new RECT();
			GetWindowRect(hWnd, ref rect);

			if (rect.Left > 0 || rect.Top > 0) return false;
			if (rect.Right < Screen.PrimaryScreen.Bounds.Width || rect.Bottom < Screen.PrimaryScreen.Bounds.Height) return false;

			return true;
		}

		#region Nested type: FLASHWINFO

		[StructLayout(LayoutKind.Sequential)]
		public struct FLASHWINFO
		{
			public UInt32 cbSize;
			public IntPtr hwnd;
			public UInt32 dwFlags;
			public UInt32 uCount;
			public UInt32 dwTimeout;
		}

		#endregion
	}
}