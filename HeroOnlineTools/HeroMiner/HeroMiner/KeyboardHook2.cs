using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HeroMiner
{
	/// <summary>
	/// Low-level keyboard intercept class to trap and suppress system keys.
	/// </summary>
	public class KeyboardHook2 : IDisposable
	{
		/// <summary>
		/// Parameters accepted by the KeyboardHook constructor.
		/// </summary>
		public enum Parameters
		{
			None,
			AllowAltTab,
			AllowWindowsKey,
			AllowAltTabAndWindows,
			PassAllKeysToNextApp
		}

		//Internal parameters
		private bool PassAllKeysToNextApp = false;
		private bool AllowAltTab = false;
		private bool AllowWindowsKey = false;

		//Keyboard API constants
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_SYSKEYDOWN = 0x0104;

		//Variables used in the call to SetWindowsHookEx
		private HookHandlerDelegate proc;
		private IntPtr hookID = IntPtr.Zero;
		private delegate IntPtr HookHandlerDelegate(
			int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		/// <summary>
		/// Event triggered when a keystroke is intercepted by the 
		/// low-level hook.
		/// </summary>
		public event KeyboardHookEventHandler KeyIntercepted;

		// Structure returned by the hook whenever a key is pressed
		private struct KBDLLHOOKSTRUCT
		{
			public int vkCode;
			int scanCode;
			public int flags;
			int time;
			int dwExtraInfo;
		}

		#region Constructors
		/// <summary>
		/// Sets up a keyboard hook to trap all keystrokes without 
		/// passing any to other applications.
		/// </summary>
		public KeyboardHook2()
		{
			proc = new HookHandlerDelegate(HookCallback);
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc,
					GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		/// <summary>
		/// Sets up a keyboard hook with custom parameters.
		/// </summary>
		/// <param name="param">A valid name from the Parameter enum; otherwise, the 
		/// default parameter Parameter.None will be used.</param>
		public KeyboardHook2(string param)
			: this()
		{
			if (!String.IsNullOrEmpty(param) && Enum.IsDefined(typeof(Parameters), param))
			{
				SetParameters((Parameters)Enum.Parse(typeof(Parameters), param));
			}
		}

		/// <summary>
		/// Sets up a keyboard hook with custom parameters.
		/// </summary>
		/// <param name="param">A value from the Parameters enum.</param>
		public KeyboardHook2(Parameters param)
			: this()
		{
			SetParameters(param);
		}

		private void SetParameters(Parameters param)
		{
			switch (param)
			{
				case Parameters.None:
					break;
				case Parameters.AllowAltTab:
					AllowAltTab = true;
					break;
				case Parameters.AllowWindowsKey:
					AllowWindowsKey = true;
					break;
				case Parameters.AllowAltTabAndWindows:
					AllowAltTab = true;
					AllowWindowsKey = true;
					break;
				case Parameters.PassAllKeysToNextApp:
					PassAllKeysToNextApp = true;
					break;
			}
		}
		#endregion

		#region Hook Callback Method
		/// <summary>
		/// Processes the key event captured by the hook.
		/// </summary>
		private IntPtr HookCallback(
			int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
		{
			bool AllowKey = PassAllKeysToNextApp;

			//Filter wParam for KeyDown events only
			if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				//Ctrl+Esc or Windows key
				if (AllowWindowsKey)
				{
					switch (lParam.flags)
					{
						//Ctrl+Esc
						case 0:
							if (lParam.vkCode == 27)
								AllowKey = true;
							break;

						//Windows keys
						case 1:
							if ((lParam.vkCode == 91) || (lParam.vkCode == 92))
								AllowKey = true;
							break;
					}
				}
				//Alt+Tab
				if (AllowAltTab)
				{
					if ((lParam.flags == 32) && (lParam.vkCode == 9))
						AllowKey = true;
				}

				KeyboardHookEventArgs args = new KeyboardHookEventArgs(lParam.vkCode, AllowKey);
				OnKeyIntercepted(args);

				//If this key is being suppressed, return a dummy value
				if (args.PassThrough == false)
					return (IntPtr)1;
			}
			//Pass key to next application
			return CallNextHookEx(hookID, nCode, wParam, ref lParam);

		}
		#endregion

		#region Event Handling
		/// <summary>
		/// Raises the KeyIntercepted event.
		/// </summary>
		/// <param name="e">An instance of KeyboardHookEventArgs</param>
		public void OnKeyIntercepted(KeyboardHookEventArgs e)
		{
			if (KeyIntercepted != null)
				KeyIntercepted(e);
		}

		/// <summary>
		/// Delegate for KeyboardHook event handling.
		/// </summary>
		/// <param name="e">An instance of InterceptKeysEventArgs.</param>
		public delegate void KeyboardHookEventHandler(KeyboardHookEventArgs e);

		/// <summary>
		/// Event arguments for the KeyboardHook class's KeyIntercepted event.
		/// </summary>
		public class KeyboardHookEventArgs : EventArgs
		{

			private string keyName;
			private int keyCode;
			private bool passThrough;

			/// <summary>
			/// The name of the key that was pressed.
			/// </summary>
			public string KeyName
			{
				get { return keyName; }
			}

			/// <summary>
			/// The virtual key code of the key that was pressed.
			/// </summary>
			public int KeyCode
			{
				get { return keyCode; }
			}

			/// <summary>
			/// True if this key combination was passed to other applications,
			/// false if it was trapped.
			/// </summary>
			public bool PassThrough
			{
				get { return passThrough; }
				set { passThrough = value; }
			}

			public KeyboardHookEventArgs(int evtKeyCode, bool evtPassThrough)
			{
				keyName = ((Keys)evtKeyCode).ToString();
				keyCode = evtKeyCode;
				passThrough = evtPassThrough;
			}

		}

		#endregion

		#region IDisposable Members
		/// <summary>
		/// Releases the keyboard hook.
		/// </summary>
		public void Dispose()
		{
			UnhookWindowsHookEx(hookID);
		}
		#endregion

		#region DllImports
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook,
			HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
			IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);
		#endregion
	}
}
