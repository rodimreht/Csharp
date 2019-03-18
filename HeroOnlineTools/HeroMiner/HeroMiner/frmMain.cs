using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
#if _VK_MAPPING || _VK_MAPPING2
using ManagedHooks;
#elif _VK_MAPPING_DX
using Microsoft.DirectX.DirectInput;
#endif

namespace HeroMiner
{
	public partial class frmMain : Form
	{
#if _VK_MAPPING || _VK_MAPPING2
		private KeyboardHook keyboardHook = null;

		private const int KEYEVENTF_EXTENDEDKEY = 1;
		private const int KEYEVENTF_KEYUP = 2;
#elif _VK_MAPPING_DX
		private Device keyb;
		
		private const ushort KEYEVENTF_KEYUP = 0x0002;
		private const ushort KEYEVENTF_SCANCODE = 0x0008;
		private const ushort INPUT_KEYBOARD = 0x0001;
		
		private struct KEYBD_INPUT
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public long time;
			public uint dwExtraInfo;
		}

		[StructLayout(LayoutKind.Explicit, Size = 28)]
		private struct INPUT
		{
			[FieldOffset(0)]
			public uint type;
			
			[FieldOffset(4)]
			public KEYBD_INPUT ki;
		}
		
		[DllImport("user32.dll")]
		private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
#endif
		
		private double ticks;
		private Mappings[] maps;
		private IniFile _iniFile;

#if _VK_MAPPING
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		[DllImport("user32.dll")]
		private static extern int MapVirtualKey(int wCode, int wMapType);
#elif _VK_MAPPING2
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int HWND_BROADCAST = 0xFFFF;
		
		[DllImport("user32.dll")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);
		
		[DllImport("user32.dll")]
		public static extern int PostMessage(int hwnd, int wMsg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
#endif

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		private const int CHAR_256 = 256;

		private const int SW_SHOWNORMAL = 1;
		private const int SW_SHOWMINIMIZED = 2;
		private const int SW_SHOWMAXIMIZED = 3;
		private const int SW_SHOWNOACTIVATE = 4;
		private const int SW_SHOW = 5;
		private const int SW_MINIMIZE = 6;
		private const int SW_SHOWMINNOACTIVE = 7;
		private const int SW_SHOWNA = 8;
		private const int SW_RESTORE = 9;
		private const int SW_SHOWDEFAULT = 10;
		private const int SW_FORCEMINIMIZE = 11;
		private const int SW_MAX = 11;

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		private struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}

		public frmMain()
		{
			InitializeComponent();

#if _VK_MAPPING || _VK_MAPPING2
			keyboardHook = new KeyboardHook();
			keyboardHook.KeyboardEvent += new KeyboardHook.KeyboardEventHandler(keyboardHook_KeyboardEvent);
#elif _VK_MAPPING_LOCAL || _VK_MAPPING_DX
			Program.kh.KeyIntercepted += new KeyboardHook2.KeyboardHookEventHandler(kh_KeyIntercepted);
#endif

#if _VK_MAPPING_DX
			initializeKeyboard();
#endif
			_iniFile = new IniFile(Application.ExecutablePath.ToLower().Replace(".exe", ".ini"));
		}

#if _VK_MAPPING_DX
		private void initializeKeyboard()
		{
			keyb = new Device(SystemGuid.Keyboard);
			keyb.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
			keyb.SetDataFormat(DeviceDataFormat.Keyboard);
			keyb.Acquire();
		}

		/*
		private bool readKeyboard()
		{
			bool ret = false;
			KeyboardState keys = keyb.GetCurrentKeyboardState();
			if (keys[Key.Insert])
			{
				cmdStart_Click(null, null);
				ret = true;
			}
			else if (keys[Key.Delete])
			{
				cmdEnd_Click(null, null);
				ret = true;
			}

			string msg = string.Format("{0} is pressed.", keys);
			lblStatus.Text = msg;
			lblStatus.Refresh();
			return ret;
		}
		*/
#endif

		private void frmMain_Load(object sender, EventArgs e)
		{
#if _VK_MAPPING || _VK_MAPPING2
			keyboardHook.InstallHook();
#endif
			loadIni();
			cmdStart.Focus();
		}

		private void loadIni()
		{
			string val = _iniFile.IniReadValue("Keymap1", "Key11");
			if (!string.IsNullOrEmpty(val)) cboKey11.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Key12");
			if (!string.IsNullOrEmpty(val)) cboKey12.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Time1");
			if (!string.IsNullOrEmpty(val)) txtTime1.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Key21");
			if (!string.IsNullOrEmpty(val)) cboKey21.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Key22");
			if (!string.IsNullOrEmpty(val)) cboKey22.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Time2");
			if (!string.IsNullOrEmpty(val)) txtTime2.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Key31");
			if (!string.IsNullOrEmpty(val)) cboKey31.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Key32");
			if (!string.IsNullOrEmpty(val)) cboKey32.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Time3");
			if (!string.IsNullOrEmpty(val)) txtTime3.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Key41");
			if (!string.IsNullOrEmpty(val)) cboKey41.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Key42");
			if (!string.IsNullOrEmpty(val)) cboKey42.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Time4");
			if (!string.IsNullOrEmpty(val)) txtTime4.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Key51");
			if (!string.IsNullOrEmpty(val)) cboKey51.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Key52");
			if (!string.IsNullOrEmpty(val)) cboKey52.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Time5");
			if (!string.IsNullOrEmpty(val)) txtTime5.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Key61");
			if (!string.IsNullOrEmpty(val)) cboKey61.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Key62");
			if (!string.IsNullOrEmpty(val)) cboKey62.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Time6");
			if (!string.IsNullOrEmpty(val)) txtTime6.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Custkey11");
			if (!string.IsNullOrEmpty(val)) txtCustKey1.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Custkey12");
			if (!string.IsNullOrEmpty(val)) txtCustKey2.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Custtime1");
			if (!string.IsNullOrEmpty(val)) txtCustTime.Text = val;

			val = _iniFile.IniReadValue("Keymap1", "Custkey21");
			if (!string.IsNullOrEmpty(val)) txtCustKey3.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Custkey22");
			if (!string.IsNullOrEmpty(val)) txtCustKey4.Text = val;
			val = _iniFile.IniReadValue("Keymap1", "Custtime2");
			if (!string.IsNullOrEmpty(val)) txtCustTime2.Text = val;
		}

#if _VK_MAPPING || _VK_MAPPING2
		private void keyboardHook_KeyboardEvent(KeyboardEvents kEvent, Keys key)
		{
			if (key.Equals(Keys.Insert))
				cmdStart_Click(null, null);
			else if (key.Equals(Keys.Delete))
				cmdEnd_Click(null, null);

			string msg = string.Format("{0} is pressed.", key);
			lblStatus.Text = msg;
			lblStatus.Refresh();

		}
#elif _VK_MAPPING_LOCAL || _VK_MAPPING_DX
		private void kh_KeyIntercepted(KeyboardHook2.KeyboardHookEventArgs e)
		{
			if (e.KeyCode.Equals((int)Keys.Insert))
			{
				cmdStart_Click(null, null);
				e.PassThrough = !isHeroOnlineOrThis();
			}
			else if (e.KeyCode.Equals((int)Keys.Delete))
			{
				cmdEnd_Click(null, null);
				e.PassThrough = !isHeroOnlineOrThis();
			}

			string msg = string.Format("{0} is pressed.", e.KeyName);
			lblStatus.Text = msg;
			lblStatus.Refresh();
		}
#endif

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
#if _VK_MAPPING || _VK_MAPPING2
			keyboardHook.UninstallHook();
#elif _VK_MAPPING_DX
			keyb.Unacquire();
#endif
		}

		private void cmdStart_Click(object sender, EventArgs e)
		{
			ticks = 0;
			maps = getMappings();
			cmdStart.Enabled = false;
			cmdEnd.Enabled = true;
			
			timer1.Interval = 500;
			timer1.Start();
		}

		private void cmdEnd_Click(object sender, EventArgs e)
		{
			timer1.Stop();
			
			cmdStart.Enabled = true;
			cmdEnd.Enabled = false;
		}

		private bool isHeroOnlineOrThis()
		{
			IntPtr hWndThis = Process.GetCurrentProcess().MainWindowHandle;

			StringBuilder buff = new StringBuilder(CHAR_256);
			IntPtr hWnd = GetForegroundWindow();
			if (hWndThis == hWnd) return true;

			if (GetWindowText(hWnd, buff, CHAR_256) > 0)
			{
				return buff.ToString().Equals("Hero OnLine");
			}
			return false;
		}

		private bool isHeroOnline(ref IntPtr hWnd)
		{
			StringBuilder buff = new StringBuilder(CHAR_256);
			hWnd = GetForegroundWindow();
			if (GetWindowText(hWnd, buff, CHAR_256) > 0)
			{
				return buff.ToString().Equals("Hero OnLine");
			}
			return false;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			IntPtr hWndOrg = IntPtr.Zero;
			IntPtr hWndHero = IntPtr.Zero;
			int heroState = 0;

			bool doneProcess = false;
			bool isHero = isHeroOnline(ref hWndOrg);
			if (!isHero)
			{
				string pid = txtPID.Text;
				if (string.IsNullOrEmpty(pid) || int.Parse(pid) == 0) return;
			}

			ticks += 0.5;
			if (ticks >= double.MaxValue) ticks = 0.0;
			
#if _VK_MAPPING2
			string progName = "notepad";
			Process[] procs = Process.GetProcessesByName(progName);
			if (procs.Length == 0)
			{
				cmdEnd_Click(null, null);
				MessageBox.Show(progName + "이(가) 실행되고 있지 않습니다.");
				return;
			}

			int thWnd = procs[0].Handle.ToInt32();
#endif
			
			foreach (Mappings map in maps)
			{
				if ((ticks % map.ticks) == 0.0)
				{
					lock (this)
					{
						if (!isHero && !doneProcess)
						{
							int pidHero = int.Parse(txtPID.Text);
							hWndHero = Process.GetProcessById(pidHero).MainWindowHandle;

							WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
							GetWindowPlacement(hWndHero, ref placement);
							heroState = placement.showCmd; // 1: Normal, 2: Minimized, 3: Maximized

							SetForegroundWindow(hWndHero);
							if (heroState != 1) ShowWindow(hWndHero, SW_RESTORE);
							doneProcess = true;
						}
					}
#if _VK_MAPPING
					if (map.key1 > 0)
					{
						if ((map.key1 >= 0x70) && (map.key1 <= 0x73))
							keybd_event(map.key1, (byte)MapVirtualKey(map.key1, 0), KEYEVENTF_EXTENDEDKEY | 0, 0);
						else
							keybd_event(map.key1, (byte)MapVirtualKey(map.key1, 0), 0, 0);

						Thread.Sleep(50);
						keybd_event(map.key1, (byte)MapVirtualKey(map.key1, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
						System.Threading.Thread.Sleep(120);
					}

					if (map.key2 > 0)
					{
						keybd_event(map.key2, (byte) MapVirtualKey(map.key2, 0), 0, 0);
						Thread.Sleep(50);
						keybd_event(map.key2, (byte)MapVirtualKey(map.key2, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
					}
					System.Threading.Thread.Sleep(50);
#elif _VK_MAPPING2
					if (map.key1 > 0)
					{
						PostMessage(thWnd, WM_KEYDOWN, map.key1, 0);
						Thread.Sleep(50);
						PostMessage(thWnd, WM_KEYUP, map.key1, 0);
						Thread.Sleep(50);
					}
					if (map.key2 > 0)
					{
						PostMessage(thWnd, WM_KEYDOWN, map.key2, 0);
						Thread.Sleep(50);
						PostMessage(thWnd, WM_KEYUP, map.key2, 0);
					}
#elif _VK_MAPPING_DX
					diKeyPress(map.key1, map.key2);
#elif _STR_MAPPING
					if (map.key1.Length > 0)
					{
						SendKeys.Send(map.key1);
						Thread.Sleep(50);
					}
					if (map.key2.Length > 0)
						SendKeys.Send(map.key2);

					SendKeys.Flush();
#endif
				}
			}

			lock (this)
			{
				if (!isHero && doneProcess)
				{
					ShowWindow(hWndHero, heroState);
					SetForegroundWindow(hWndOrg);
				}
			}
		}

		private void diKeyPress(ushort key1, ushort key2)
		{
			if (key2 <= 0) return;

			INPUT inp1 = new INPUT();
			inp1.type = INPUT_KEYBOARD;
			inp1.ki.dwFlags = KEYEVENTF_SCANCODE;

			INPUT inp2 = inp1;
			inp2.ki.dwFlags |= KEYEVENTF_KEYUP;

			bool pressWith = false;

			// key1 입력
			if (key1 > 0)
			{
				if (key1 == getDIKey("LSHIFT") || key1 == getDIKey("RSHIFT") || key1 == getDIKey("LCTRL") || key1 == getDIKey("RCTRL"))
					pressWith = true;

				if (pressWith)
				{
					inp1.ki.wScan = key1;

					SendInput(1, ref inp1, Marshal.SizeOf(inp1));
					Thread.Sleep(50);
				}
				else
				{
					inp1.ki.wScan = key1;
					inp2.ki.wScan = key1;

					SendInput(1, ref inp1, Marshal.SizeOf(inp1));
					Thread.Sleep(50);
					SendInput(1, ref inp2, Marshal.SizeOf(inp2));
					Thread.Sleep(50);
				}
			}

			// key2 입력
			inp1.ki.wScan = key2;
			inp2.ki.wScan = key2;

			SendInput(1, ref inp1, Marshal.SizeOf(inp1));
			Thread.Sleep(50);
			SendInput(1, ref inp2, Marshal.SizeOf(inp2));
			Thread.Sleep(50);

			if (pressWith)
			{
				inp2.ki.wScan = key1;

				SendInput(1, ref inp2, Marshal.SizeOf(inp2));
				Thread.Sleep(50);
			}

			// 마지막으로 F1을 한번 더 눌러줌
			inp1.ki.wScan = getDIKey("F1");
			inp2.ki.wScan = inp1.ki.wScan;

			SendInput(1, ref inp1, Marshal.SizeOf(inp1));
			Thread.Sleep(50);
			SendInput(1, ref inp2, Marshal.SizeOf(inp2));
			Thread.Sleep(50);
		}

		private Mappings getMapping(ComboBox key1, ComboBox key2, TextBox time)
		{
			Mappings map = new Mappings();
			
#if _VK_MAPPING || _VK_MAPPING2
			string k1 = key1.Text.Trim();
			if (k1.Equals("F1"))
				map.key1 = 0x70;
			else if (k1.Equals("F2"))
				map.key1 = 0x71;
			else if (k1.Equals("F3"))
				map.key1 = 0x72;
			else if (k1.Equals("F4"))
				map.key1 = 0x73;
			else
				map.key1 = 0;

			string k2 = key2.Text.Trim();
			if (k2.Length > 0)
			{
				if (k2.Equals("-"))
					map.key2 = 0xBD;
				else if (k2.Equals("="))
					map.key2 = 0xBB;
				else
					map.key2 = (byte)char.Parse(k2);
			}
			else
				map.key2 = 0;
#elif _VK_MAPPING_DX
			string k1 = key1.Text.Trim();
			map.key1 = getDIKey(k1);

			string k2 = key2.Text.Trim();
			map.key2 = getDIKey(k2);
#elif _STR_MAPPING
			map.key1 = "{" + key1.Text.Trim() + "}";
			map.key2 = key2.Text.Trim();
#endif
			map.ticks = int.Parse(time.Text.Trim());
			return map;
		}

		private Mappings getMapping(TextBox key1, TextBox key2, TextBox time)
		{
			Mappings map = new Mappings();
			
#if _VK_MAPPING || _VK_MAPPING2 || _VK_MAPPING_DX
			string k1 = key1.Text.Trim();
			map.key1 = (byte)getDIKey(k1);

			string k2 = key2.Text.Trim();
			map.key2 = (byte)getDIKey(k2);
#elif _STR_MAPPING
			map.key1 = key1.Text.Trim();
			map.key2 = key2.Text.Trim();
#endif
			map.ticks = int.Parse(time.Text.Trim());
			return map;
		}
	
		private ushort getDIKey(string key)
		{
			switch (key.ToUpper())
			{
				case "F1":
					return 0x3B; // (byte)Key.F1;
				case "F2":
					return 0x3C; // (byte)Key.F2;
				case "F3":
					return 0x3D; // (byte)Key.F3;
				case "F4":
					return 0x3E; // (byte)Key.F4;
					
				case "1":
					return 0x02; // (byte)Key.D1;
				case "2":
					return 0x03; // (byte)Key.D2;
				case "3":
					return 0x04; // (byte)Key.D3;
				case "4":
					return 0x05; // (byte)Key.D4;
				case "5":
					return 0x06; // (byte)Key.D5;
				case "6":
					return 0x07; // (byte)Key.D6;
				case "7":
					return 0x08; // (byte)Key.D7;
				case "8":
					return 0x09; // (byte)Key.D8;
				case "9":
					return 0x0A; // (byte)Key.D9;
				case "0":
					return 0x0B; // (byte)Key.D0;
					
				case "-":
					return 0x0C; // (byte)Key.Minus;
				case "=":
					return 0x0D; // (byte)Key.Equals;

				case "Q":
					return 0x10; // (byte)Key.Q;
				case "W":
					return 0x11; // (byte)Key.W;
				case "A":
					return 0x1E; // (byte)Key.A;
				case "S":
					return 0x1F; // (byte)Key.S;
				case "V":
					return 0x2F; // (byte)Key.V;
				case "Z":
					return 0x2C; // (byte)Key.Z;
				case "X":
					return 0x2D; // (byte)Key.X;

				case "LCTRL":
					return 0x1D; // (byte)Key.LeftControl;
				case "RCTRL":
					return 0x9D; // (byte)Key.RightControl;

				case "LSHIFT":
					return 0x2A; // (byte)Key.LeftShift;
				case "RSHIFT":
					return 0x36; // (byte)Key.RightShift;
				default:
					return 0;
			}
		}
		
		private Mappings[] getMappings()
		{
			ArrayList arr = new ArrayList();
			
			if (txtTime1.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(cboKey11, cboKey12, txtTime1));
				_iniFile.IniWriteValue("Keymap1", "Key11", cboKey11.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Key12", cboKey12.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Time1", txtTime1.Text.Trim());
			}

			if (txtTime2.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(cboKey21, cboKey22, txtTime2));
				_iniFile.IniWriteValue("Keymap1", "Key21", cboKey21.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Key22", cboKey22.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Time2", txtTime2.Text.Trim());
			}

			if (txtTime3.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(cboKey31, cboKey32, txtTime3));
				_iniFile.IniWriteValue("Keymap1", "Key31", cboKey31.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Key32", cboKey32.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Time3", txtTime3.Text.Trim());
			}

			if (txtTime4.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(cboKey41, cboKey42, txtTime4));
				_iniFile.IniWriteValue("Keymap1", "Key41", cboKey41.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Key42", cboKey42.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Time4", txtTime4.Text.Trim());
			}

			if (txtTime5.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(cboKey51, cboKey52, txtTime5));
				_iniFile.IniWriteValue("Keymap1", "Key51", cboKey51.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Key52", cboKey52.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Time5", txtTime5.Text.Trim());
			}

			if (txtTime6.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(cboKey61, cboKey62, txtTime6));
				_iniFile.IniWriteValue("Keymap1", "Key61", cboKey61.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Key62", cboKey62.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Time6", txtTime6.Text.Trim());
			}

			if (txtCustTime.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(txtCustKey1, txtCustKey2, txtCustTime));
				_iniFile.IniWriteValue("Keymap1", "Custkey11", txtCustKey1.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Custkey12", txtCustKey2.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Custtime1", txtCustTime.Text.Trim());
			}

			if (txtCustTime2.Text.Trim().Length > 0)
			{
				arr.Add(getMapping(txtCustKey3, txtCustKey4, txtCustTime2));
				_iniFile.IniWriteValue("Keymap1", "Custkey21", txtCustKey3.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Custkey22", txtCustKey4.Text.Trim());
				_iniFile.IniWriteValue("Keymap1", "Custtime2", txtCustTime2.Text.Trim());
			}

			return (Mappings[]) arr.ToArray(typeof(Mappings));
		}

#if _VK_MAPPING || _VK_MAPPING2
		private class Mappings
		{
			public byte key1;
			public byte key2;
			public double ticks;
		}
#elif _VK_MAPPING_DX
		private class Mappings
		{
			public ushort key1;
			public ushort key2;
			public double ticks;
		}
#elif _STR_MAPPING
		private class Mappings
		{
			public string key1;
			public string key2;
			public double ticks;
		}
#endif
	}
}