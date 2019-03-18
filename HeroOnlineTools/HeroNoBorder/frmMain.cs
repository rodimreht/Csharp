using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HeroNoBorder
{
	public partial class frmMain : Form
	{
		[DllImport("user32.dll")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		public static extern int SendMessage(int hwnd, int wMsg, int wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(int hwnd, int nIndex);
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(int hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll")]
		public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

		public const int GWL_EXSTYLE = (-20);
		public const int GWL_STYLE = (-16);
		public const int GWL_WNDPROC = (-4);

		public const int HWND_NOTOPMOST = -2;
		public const int HWND_TOPMOST = -1;
		public const int HWND_TOP = 0;

		public const int WS_POPUP = unchecked((int)0x80000000);
		public const int WS_THICKFRAME = 0x40000;
		public const int WS_VISIBLE = 0x10000000;
		public const int WS_BORDER = 0x800000;
		public const int WS_DLGFRAME = 0x400000;
		public const int WS_CAPTION = 0xC00000;                 //  WS_BORDER | WS_DLGFRAME;
		public const int WS_SYSMENU = 0x80000;
		public const int WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);
		public const int WS_EX_DLGMODALFRAME = 0x1;

		public const int SWP_NOSIZE = 0x1;
		public const int SWP_NOMOVE = 0x2;
		public const int SWP_NOZORDER = 0x4;
		public const int SWP_FRAMECHANGED = 0x20;        //  The frame changed: send WM_NCCALCSIZE;
		public const int SWP_SHOWWINDOW = 0x0040;

		private int m_style = 0;
		private int m_exstyle = 0;

		public frmMain()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			int hWnd = FindWindow(null, "Hero OnLine");
			Debug.WriteLine("FindWindow: " + hWnd);

			int style = GetWindowLong(hWnd, GWL_STYLE);
			m_style = style;
			style &= ~WS_DLGFRAME;
			style &= ~WS_BORDER;
			style &= ~WS_THICKFRAME;

			int exstyle = GetWindowLong(hWnd, GWL_EXSTYLE);
			m_exstyle = exstyle;
			exstyle &= ~WS_EX_DLGMODALFRAME;

			SetWindowLong(hWnd, GWL_STYLE, style);
			SetWindowLong(hWnd, GWL_EXSTYLE, exstyle);
			SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
			SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			int hWnd = FindWindow(null, "Hero OnLine");
			Debug.WriteLine("FindWindow: " + hWnd);

			int style = m_style;
			if (style == 0)
			{
				style = GetWindowLong(hWnd, GWL_STYLE);
				style |= WS_DLGFRAME;
				style |= WS_BORDER;
				style |= WS_THICKFRAME;
			}

			int exstyle = m_exstyle;
			if (exstyle == 0)
			{
				exstyle = GetWindowLong(hWnd, GWL_EXSTYLE);
				exstyle |= WS_EX_DLGMODALFRAME;
			}

			SetWindowLong(hWnd, GWL_STYLE, style);
			SetWindowLong(hWnd, GWL_EXSTYLE, exstyle);
			SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
			SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			comboBox1.SelectedText = "1024x768";
		}

		private void button3_Click(object sender, EventArgs e)
		{
			int hWnd = Process.GetProcessById(int.Parse(txtPID.Text)).MainWindowHandle.ToInt32();
			//int hWnd = FindWindow(null, "Hero OnLine");

			string[] wh = comboBox1.Text.Split('x');
			int w = int.Parse(wh[0]) + SystemInformation.FixedFrameBorderSize.Width * 2;
			int h = int.Parse(wh[1]) + SystemInformation.CaptionHeight + SystemInformation.FixedFrameBorderSize.Height * 2;
			SetWindowPos(hWnd, HWND_TOP, 0, 0, w, h, SWP_NOZORDER | SWP_NOMOVE | SWP_SHOWWINDOW);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			int hWnd = Process.GetProcessById(int.Parse(txtPID.Text)).MainWindowHandle.ToInt32();
			//int hWnd = FindWindow(null, "Hero OnLine");

			int x = int.Parse(txtX.Text);
			int y = int.Parse(txtY.Text);
			SetWindowPos(hWnd, HWND_TOP, x, y, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
		}
	}
}