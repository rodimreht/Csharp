using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]
namespace NETS_iMan
{
	public class MessageBoxEx
	{
		public enum Position
		{
			NotSet,
			CenterScreen,
			CenterParent,
			BottomLeft,
			BottomRight
		}

		public static DialogResult Show(string text, uint uTimeout)
		{
			Setup("", uTimeout);
			return MessageBox.Show(text);
		}
		
		public static DialogResult Show(string text, string caption, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(text, caption);
		}
		
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(text, caption, buttons);
		}
		
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(text, caption, buttons, icon);
		}

		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, uint uTimeout, Position pos)
		{
			Setup(caption, uTimeout, pos);
			return MessageBox.Show(text, caption, buttons, icon);
		}
		
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(text, caption, buttons, icon, defButton);
		}

		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, uint uTimeout, Position pos)
		{
			Setup(caption, uTimeout, pos);
			return MessageBox.Show(text, caption, buttons, icon, defButton);
		}
		
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(text, caption, buttons, icon, defButton, options);
		}
		
		public static DialogResult Show(IWin32Window owner, string text, uint uTimeout)
		{
			Setup("", uTimeout);
			return MessageBox.Show(owner, text);
		}
		
		public static DialogResult Show(IWin32Window owner, string text, string caption, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(owner, text, caption);
		}
		
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(owner, text, caption, buttons);
		}
		
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(owner, text, caption, buttons, icon);
		}
		
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(owner, text, caption, buttons, icon, defButton);
		}
		
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options, uint uTimeout)
		{
			Setup(caption, uTimeout);
			return MessageBox.Show(owner, text, caption, buttons, icon, defButton, options);
		}

		public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
		public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

		public const int WH_CALLWNDPROCRET = 12;
		public const int WM_DESTROY = 0x0002;
		public const int WM_INITDIALOG = 0x0110;
		public const int WM_TIMER = 0x0113;
		public const int WM_USER = 0x400;
		public const int DM_GETDEFID = WM_USER + 0;

		[DllImport("User32.dll")]
		public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);

		[DllImport("User32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

		[DllImport("user32.dll")]
		public static extern int UnhookWindowsHookEx(IntPtr idHook);
			
		[DllImport("user32.dll")]
		public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

		[DllImport("user32.dll")]
		public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);


		#region 메시지창 위치를 위한 옵션

		public const int WH_CBT = 5;
		public const int HCBT_ACTIVATE = 5;
		public const int SWP_NOSIZE = 0x01;
		public const int SWP_NOZORDER = 0x04;
		public const int SWP_NOACTIVATE = 0x10;

		[DllImport("user32.dll")]
		public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[DllImport("user32.dll")]
		public static extern int GetWindowRect(IntPtr hWnd, [MarshalAs(UnmanagedType.Struct)] ref RECT lpRect);

		#endregion


		[StructLayout(LayoutKind.Sequential)]
		public struct CWPRETSTRUCT
		{
			public IntPtr lResult;
			public IntPtr lParam;
			public IntPtr wParam;
			public uint   message;
			public IntPtr hwnd;
		};

		private const int TimerID = 42;
		private static HookProc hookProc;
		private static TimerProc hookTimer;
		private static uint hookTimeout;
		private static string hookCaption;
		private static IntPtr hHook;
		private static Position posMsgBox;
		private static HookProc hookProc2;
		private static IntPtr hHook2;
		
		static MessageBoxEx()
		{
			hookProc = new HookProc(MessageBoxHookProc);
			hookProc2 = new HookProc(MessageBoxHookProc2);
			hookTimer = new TimerProc(MessageBoxTimerProc);
			hookTimeout = 0;
			hookCaption = null;
			hHook = IntPtr.Zero;
			hHook2 = IntPtr.Zero;
			posMsgBox = Position.NotSet;
		}
		
		private static void Setup(string caption, uint uTimeout)
		{
			if (hHook != IntPtr.Zero)
				throw new NotSupportedException("multiple calls are not supported");

			hookTimeout = uTimeout;
			hookCaption = caption ?? "";
			hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, hookProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
		}

		private static void Setup(string caption, uint uTimeout, Position pos)
		{
			if (hHook != IntPtr.Zero)
				throw new NotSupportedException("multiple calls are not supported");

			hookTimeout = uTimeout;
			hookCaption = caption ?? "";
			posMsgBox = pos;
			hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, hookProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
			hHook2 = SetWindowsHookEx(WH_CBT, hookProc2, IntPtr.Zero, AppDomain.GetCurrentThreadId());
		}
		
		private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode < 0)
				return CallNextHookEx(hHook, nCode, wParam, lParam);

			CWPRETSTRUCT msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
			IntPtr hook = hHook;

			if (hookCaption != null && msg.message == WM_INITDIALOG)
			{
				int nLength = GetWindowTextLength(msg.hwnd);
				StringBuilder text = new StringBuilder(nLength + 1);

				GetWindowText(msg.hwnd, text, text.Capacity);

				if (hookCaption == text.ToString())
				{
					hookCaption = null;
					UIntPtr ptr = new UIntPtr(TimerID);
					SetTimer(msg.hwnd, ptr, hookTimeout, hookTimer);
					UnhookWindowsHookEx(hHook);
					hHook = IntPtr.Zero;
				}
			}

			return CallNextHookEx(hook, nCode, wParam, lParam);
		}

		private static IntPtr MessageBoxHookProc2(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (posMsgBox != Position.NotSet && nCode == HCBT_ACTIVATE)
			{
				RECT rectMsg = new RECT();
				int x, y;
				GetWindowRect(wParam, ref rectMsg);

				switch (posMsgBox)
				{
					case Position.CenterParent: // 부모창이 있을 때만; 없으면 화면 가운데
						if (frmMain.ActiveForm != null && frmMain.ActiveForm.Visible)
						{
							RECT rectParent = new RECT();
							GetWindowRect(frmMain.ActiveForm.Handle, ref rectParent);

							x = (rectParent.Left + (rectParent.Right - rectParent.Left) / 2) -
									((rectMsg.Right - rectMsg.Left) / 2);
							y = (rectParent.Top + (rectParent.Bottom - rectParent.Top) / 2) -
									((rectMsg.Bottom - rectMsg.Top) / 2);
							SetWindowPos(wParam, 0, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
						}
						break;

					case Position.BottomLeft: // 부모창이 없을 때만; 있으면 화면 가운데
						if (frmMain.ActiveForm == null || !frmMain.ActiveForm.Visible)
						{
							x = 0;
							y = Screen.PrimaryScreen.WorkingArea.Bottom - (rectMsg.Bottom - rectMsg.Top);
							SetWindowPos(wParam, 0, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
						}
						break;

					case Position.BottomRight: // 부모창이 없을 때만; 있으면 화면 가운데
						if (frmMain.ActiveForm == null || !frmMain.ActiveForm.Visible)
						{
							x = Screen.PrimaryScreen.WorkingArea.Right - (rectMsg.Right - rectMsg.Left);
							y = Screen.PrimaryScreen.WorkingArea.Bottom - (rectMsg.Bottom - rectMsg.Top);
							SetWindowPos(wParam, 0, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
						}
						break;

					default:
						break;
				}
				UnhookWindowsHookEx(hHook2);
				hHook2 = IntPtr.Zero;
			}
			return IntPtr.Zero;
		}

		private static void MessageBoxTimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime)
		{
			UIntPtr ptr = new UIntPtr(TimerID);
			if (nIDEvent.ToUInt32() == ptr.ToUInt32())
			{
				short dw = (short)SendMessage(hWnd, DM_GETDEFID, IntPtr.Zero, IntPtr.Zero);
			
				EndDialog(hWnd, (IntPtr)dw);
			}
		}
	}
}
