using System;
using System.Collections;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using NETS_iMan.chatWebsvc;
using NETS_iMan.iManWebsvc;
using NETS_iMan.Win32Imports;
using Timer=System.Windows.Forms.Timer;

namespace NETS_iMan
{
	public partial class frmMain : Form
	{
		private static readonly ArrayList wndArray = ArrayList.Synchronized(new ArrayList());
		private static volatile int ERROR_COUNT;
		
		internal static string CACHED_USERS = string.Empty;
		internal static ArrayList CHATTING_GROUPS = ArrayList.Synchronized(new ArrayList());
		internal static ArrayList CHATTING_USERS = ArrayList.Synchronized(new ArrayList());
		internal static int GROUP_COUNT;
		internal static string IN_MESSAGE = string.Empty;
		internal static object LOCK_OBJECT = new object();
		internal static LoginInfo LOGIN_INFO;
		internal static ServerSocket SERVER_SOCKET;

		internal static double FORM_OPACITY = 1.0;
		internal static double FADE_RATE = 0.01; //how fast the form fades in/out

		private readonly ExtendedWebBrowser _browser;
		private readonly ChatService chatSvc;
		private readonly DirService dirSvc;
		private readonly ChatService groupChatSvc;
		private readonly StickyWindow sticky;
		private bool alwaysTopMost;

		private int chatTickCount;
		private bool doExit;
		private bool firstLoading;
		private DateTime LAST_ACT_TIME;
		private Uri loopURL;
		private int statusTickCount;
		private int updateTickCount;
		private int msgClearCount;

		private volatile bool chatProcessing = false;
		private volatile bool statusChanging = false;
		private volatile bool isFading = false, stopFading = false;
		private volatile bool abortFading = false;

		private Thread fadeInThread = null;
		private Thread fadeOutThread = null;

		private bool forceBusy = false;
		private bool forceAbsent = false;

		public frmMain()
		{
			InitializeComponent();

			Logger.Log("NETS-ⓘMan 시작");
			firstLoading = true;
			LAST_ACT_TIME = DateTime.Now;

			sticky = new StickyWindow(this);
			sticky.StickOnMove = true;
			sticky.StickOnResize = false;
			sticky.StickToOther = false;
			sticky.StickToScreen = true;

			_browser = new ExtendedWebBrowser();
			_browser.Dock = DockStyle.Fill;
			_browser.DownloadComplete += _browser_DownloadComplete;
			_browser.Navigated += _browser_Navigated;
			_browser.DocumentCompleted += _browser_DocumentCompleted;
			_browser.PreviewKeyDown += _browser_PreviewKeyDown;
			_browser.ScriptErrorsSuppressed = true;
			containerPanel.Controls.Add(_browser);

			dirSvc = new DirService();

			chatSvc = new ChatService();
			chatSvc.GetLoginUsersCompleted += chatSvc_GetLoginUsersCompleted;
			chatSvc.ReceiveMessageCompleted += chatSvc_ReceiveMessageCompleted;
			chatSvc.ReceiveOfflineMessageCompleted += chatSvc_ReceiveOfflineMessageCompleted;

			groupChatSvc = new ChatService();

			SERVER_SOCKET = new ServerSocket(6421);
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// 버전이 변경되었는지 확인하여 이전 버전의 설정을 읽어온다.
			NIUtil.LoadPrevSettings();

			SettingsHelper settings = SettingsHelper.Current;
			if (string.IsNullOrEmpty(settings.MainWindowSize))
				Size = new Size(270, 500);
			else
			{
				string[] arr = settings.MainWindowSize.Split(new char[] {'|'});
				Size = new Size(int.Parse(arr[0]), int.Parse(arr[1]));
			}
			if (string.IsNullOrEmpty(settings.MainWindowPosition))
			{
				Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width,
				                     Screen.PrimaryScreen.WorkingArea.Bottom - Height);
			}
			else
			{
				string[] arr = settings.MainWindowPosition.Split(new char[] {'|'});
				int left = int.Parse(arr[0]);
				int top = int.Parse(arr[1]);
				if (left < 0 || top < 0)
				{
					left = Screen.PrimaryScreen.WorkingArea.Width - Width;
					top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
					Location = new Point(left, top);

					settings.MainWindowPosition = left + "|" + top;
					settings.Save();
				}
				else
					Location = new Point(left, top);
			}

			// 개인 이미지 숨기기
			picSplitContainer.Panel1Collapsed = true;


			// 투명도 조정
			FORM_OPACITY = settings.FormOpacity/100.0;
			if (FORM_OPACITY < 1.0)
			{
				this.Opacity = FORM_OPACITY;
				this.DoubleBuffered = true;
				this.TransparencyKey = Color.Lime;

				ThreadStart fadeInStart = fadeIn;
				fadeInThread = new Thread(fadeInStart);

				ThreadStart fadeOutStart = fadeOut;
				fadeOutThread = new Thread(fadeOutStart);

				opacityTimer.Enabled = true;
			}
			
			// 타이머 시작
			statusTimer.Enabled = true;

			SERVER_SOCKET.Listen();
		}

		private void frmMain_Activated(object sender, EventArgs e)
		{
			if (firstLoading)
			{
				firstLoading = false;

				SettingsHelper settings = SettingsHelper.Current;

				loginToolStripMenuItem.Visible = true;
				logoutToolStripMenuItem.Visible = false;
				msgLogToolStripMenuItem.Visible = false;
				toolStripMenuItem4.Visible = false;
				changeStatusToolStripMenuItem2.Enabled = false;
				showOnlyOnlineToolStripMenuItem.Checked = settings.ShowOnline;
				topMostToolStripMenuItem.Checked = TopMost = alwaysTopMost = settings.AlwaysTopMost;
				offLoginAlarmToolStripMenuItem.Checked = settings.OffLoginAlarm;

#if TEST
				testToolStripMenuItem.Visible = true;
#else
				testToolStripMenuItem.Visible = false;
#endif

				toolTip.SetToolTip(pictureBox1, "넷츠 커뮤니티 사이트 접속");
				toolTip2.SetToolTip(pictureBox2, "");

				loginToolStripMenuItem_Click(null, null);
			}
		}

		private void showOnlyOnlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				SettingsHelper help = SettingsHelper.Current;
				help.ShowOnline = showOnlyOnlineToolStripMenuItem.Checked;
				help.Save();

				// 사용자 소속 조직 찾기
				string deptName = dirSvc.GetUserDepartment(LOGIN_INFO.LoginID);

				// 조직도 읽기
				string groups = dirSvc.GetGroupTree();
				TreeNode group;
				fillGroupTreefromXml(groups, deptName, out group);

				// 임시/계약직 조직 추가
				TreeNode group2 = treeUpdate("임시/계약직 ");
				if (group2 == null)
				{
					MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
					                  "오류: NETS-ⓘMan",
					                  MessageBoxButtons.OK,
					                  MessageBoxIcon.Exclamation,
					                  60*1000);
					return;
				}
				group2.Tag = "TEMP";
				group2.ImageIndex = 0;
				group2.SelectedImageIndex = 0;

				// 사용자 소속 조직원 목록 얻기
				TreeNode user;
				if (deptName == "") // 임시/계약직 구분
				{
					string members = dirSvc.GetTempUsers();
					fillUserTreefromXml(members, group2, out user);
				}
				else
				{
					string members = dirSvc.GetUsers((string) group.Tag);
					fillUserTreefromXml(members, group, out user);
				}

				if (user == null)
				{
					Logger.Log(Logger.LogLevel.WARNING, "로그인 실패 - 정규직 또는 임시/계약직에 없음");
					treeView.Nodes.Clear();
					return;
				}

				user.ImageIndex = 2;
				user.SelectedImageIndex = 2;

				string uName = user.Text;
				if (uName.IndexOf("(") < 0)
					LOGIN_INFO.UserName = uName;
				else
					LOGIN_INFO.UserName = uName.Substring(0, uName.IndexOf("("));

				treeView.Focus();
				treeView.SelectedNode = user;

				string users = chatSvc.GetLoginUsers();
				showUsers(users);
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("showOnlyOnlineToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "showOnlyOnlineToolStripMenuItem_Click(): " + ex);
			}
		}

		private void offLoginAlarmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SettingsHelper help = SettingsHelper.Current;
			help.OffLoginAlarm = offLoginAlarmToolStripMenuItem.Checked;
			help.Save();
		}

		private void topMostToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SettingsHelper help = SettingsHelper.Current;
			help.AlwaysTopMost = alwaysTopMost = topMostToolStripMenuItem.Checked;
			help.Save();

			TopMost = alwaysTopMost;
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool topMost = TopMost;
			TopMost = false;

			stopFading = true;
			frmOption opt = new frmOption();
			if (DialogResult.OK == opt.ShowDialog())
			{
				if (opt.ChangeDir)
				{
					Logger.ChangeDir();
					ChatLogger.ChangeDir();
				}

				SettingsHelper helper = SettingsHelper.Current;
				offLoginAlarmToolStripMenuItem.Checked = helper.OffLoginAlarm;
				showOnlyOnlineToolStripMenuItem.Checked = helper.ShowOnline;
				if (opt.TreeRefresh)
				{
					showOnlyOnlineToolStripMenuItem_Click(null, null);
				}

				double opacity = (double)helper.FormOpacity / 100;
				if (opacity < 0.1) opacity = 0.1;
				FORM_OPACITY = opacity;
				if (FORM_OPACITY < 1.0)
				{
					this.Opacity = FORM_OPACITY;
					this.DoubleBuffered = true;
					this.TransparencyKey = Color.Lime;

					ThreadStart fadeInStart = fadeIn;
					fadeInThread = new Thread(fadeInStart);

					ThreadStart fadeOutStart = fadeOut;
					fadeOutThread = new Thread(fadeOutStart);

					opacityTimer.Enabled = true;
				}
				else
				{
					opacityTimer.Enabled = false;

					abortFading = true;

					if ((fadeInThread != null) && (fadeOutThread != null))
					{
						//Cancel running fading threads to avoid overflowing the stack
						while ((fadeInThread.ThreadState == System.Threading.ThreadState.Running) ||
						       (fadeOutThread.ThreadState == System.Threading.ThreadState.Running)) Thread.Sleep(10);
					}

					fadeInThread = null;
					fadeOutThread = null;
					abortFading = false;

					this.DoubleBuffered = false;
					this.Opacity = FORM_OPACITY;
				}
			}
			stopFading = false;
			TopMost = topMost;
		}

		private bool loginCheck(LoginInfo info)
		{
			string encPwd;
			if (NISecurity.IsStrongKey(info.LoginID))
			{
				encPwd = "enc:" + NISecurity.Encrypt(info.LoginID, info.Password);
			}
			else
			{
				string key = info.LoginID + "12345678";
				encPwd = "enc:" + NISecurity.Encrypt(key, info.Password);
			}
			bool b = dirSvc.UserLogin(info.LoginID, encPwd);
			if (b) return b;

			Logger.Log(Logger.LogLevel.WARNING, "로그인을 실패했습니다.");
			MessageBoxEx.Show("로그인을 실패했습니다. 다시 시도하십시오.",
			                  "접속 실패: NETS-ⓘMan",
			                  MessageBoxButtons.OK,
			                  MessageBoxIcon.Warning,
			                  50*1000);
			Stop();
			return false;
		}

		private void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				bool topMost = TopMost;
				TopMost = false;

				using (LoginForm loginForm = new LoginForm())
				{
					if (loginForm.ShowDialog() == DialogResult.OK)
					{
						if (loginCheck(loginForm.LoginInformation))
						{
							notifyIcon1.Text = Text;
							LOGIN_INFO = loginForm.LoginInformation;
							LAST_ACT_TIME = DateTime.Now; // 강제 로그아웃을 방지하기 위해

							_browser.Navigate(LOGIN_INFO.URL, "", LOGIN_INFO.PostData,
							                  "Accept-Language: ko\r\nContent-Type: application/x-www-form-urlencoded\r\nAccept-Encoding: gzip, deflate\r\nProxy-Connection: Keep-Alive\r\n");

							treeView.Nodes.Clear();
							loginToolStripMenuItem.Visible = false;
							logoutToolStripMenuItem.Visible = true;
							msgLogToolStripMenuItem.Visible = true;
							toolStripMenuItem4.Visible = true;
							changeStatusToolStripMenuItem2.Enabled = true;
						}
					}
				}

				TopMost = topMost;
			}
			catch (WebException ex)
			{
				// 네트워크 또는 DNS 에러
				MessageBoxEx.Show("로그인 중 다음의 오류가 발생했습니다: " + ex.Message,
				                  "접속 실패: NETS-ⓘMan",
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Warning,
				                  50*1000);
				Logger.Log(Logger.LogLevel.ERROR, "loginToolStripMenuItem_Click(): " + ex);
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("loginToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "loginToolStripMenuItem_Click(): " + ex);
			}
		}

		private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				// 열려있는 대화창을 모두 닫는다.
				foreach (Form wnd in wndArray)
					if ((wnd != null) && (!wnd.IsDisposed)) wnd.Close();

				Stop();
				treeView.Nodes.Clear();
				TreeNode node = treeUpdate("로그아웃 시각 ");
				if (node == null)
				{
					MessageBoxEx.Show("화면 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
									  "오류: NETS-ⓘMan",
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Exclamation,
									  60 * 1000);
					return;
				}
				treeUpdate(node, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0xCC3333));
				treeExpand(node);
				ERROR_COUNT = 0;
			}
			catch (WebException ex)
			{
				// 네트워크 또는 DNS 에러
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[logoutToolStripMenuItem_Click()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[logoutToolStripMenuItem_Click()]: " + ex);
					}
				}
				else
				{
					ErrorReport.SendReport("logoutToolStripMenuItem_Click", ex);
					Logger.Log(Logger.LogLevel.ERROR, "logoutToolStripMenuItem_Click(): " + ex);
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("logoutToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "logoutToolStripMenuItem_Click(): " + ex);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			doExit = true;

			foreach (Form wnd in wndArray)
				if ((wnd != null) && (!wnd.IsDisposed)) wnd.Close();

			SERVER_SOCKET.Close();

			Logger.Log("NETS-ⓘMan 종료");
			Logger.Close();
			ChatLogger.Close();

			Application.Exit();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			About();
		}

		/// <summary>
		/// Shows the AboutForm
		/// </summary>
		private void About()
		{
			using (AboutForm af = new AboutForm())
			{
				af.ShowDialog(this);
			}
		}

		private void showMenuItem_Click(object sender, EventArgs e)
		{
			showMainWindow();
		}

		private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			showMainWindow();
		}

		private void showMainWindow()
		{
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
			notifyIcon1.Visible = false;

			Show();
			WindowState = FormWindowState.Normal;
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				// 이미 창이 최소화된 상태에서 종료신호를 받으면 정상 종료한다.
				if (doExit || notifyIcon1.Visible)
				{
					if (LOGIN_INFO != null)
					{
						dirSvc.Timeout = 2500;
						dirSvc.Logout(LOGIN_INFO.LoginID);
						chatSvc.Timeout = 2500;
						chatSvc.RemoveUser(LOGIN_INFO.LoginID);
					}
					return;
				}

				notifyIcon1.Icon = new Icon(getIconResourceStream());
				notifyIcon1.Visible = true;
				e.Cancel = true;
				Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
				WindowState = FormWindowState.Minimized;
				Thread.Sleep(500);
				Hide();
			}
			catch (WebException ex)
			{
				// 네트워크 또는 DNS 에러
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[frmMain_FormClosing()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[frmMain_FormClosing()]: " + ex);
					}
				}
				else
				{
					ErrorReport.SendReport("frmMain_FormClosing", ex);
					Logger.Log(Logger.LogLevel.ERROR, "frmMain_FormClosing(): " + ex);
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("frmMain_FormClosing", ex);
				Logger.Log(Logger.LogLevel.ERROR, "frmMain_FormClosing(): " + ex);
			}
		}

		private static Stream getIconResourceStream()
		{
			string loginIcon;
			if (LOGIN_INFO != null)
				loginIcon = LOGIN_INFO.IsAbsent
				            	? "NETS_iMan.logoi3.ico"
				            	: (LOGIN_INFO.IsBusy ? "NETS_iMan.logoi4.ico" : "NETS_iMan.logoi1.ico");
			else
				loginIcon = "NETS_iMan.offline.ico";

			string resourceName = loginIcon;
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
		}

		private static Stream getIconResourceStream2()
		{
			string loginIcon;
			if (LOGIN_INFO != null)
				loginIcon = LOGIN_INFO.IsAbsent
				            	? "NETS_iMan.logoi3.ico"
				            	: (LOGIN_INFO.IsBusy ? "NETS_iMan.logoi4.ico" : "NETS_iMan.logoi2.ico");
			else
				loginIcon = "NETS_iMan.offline.ico";

			string resourceName = loginIcon;
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			doExit = true;

			foreach (Form wnd in wndArray)
				if ((wnd != null) && (!wnd.IsDisposed)) wnd.Close();

			SERVER_SOCKET.Close();

			Logger.Log("NETS-ⓘMan 종료");
			Logger.Close();
			ChatLogger.Close();

			Application.Exit();
		}

		private void _browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.Control && _browser.Focused) _browser.Parent.Focus();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			if (LOGIN_INFO != null)
			{
				dirSvc.Timeout = 2500;
				dirSvc.Logout(LOGIN_INFO.LoginID);
				chatSvc.Timeout = 2500;
				chatSvc.RemoveUser(LOGIN_INFO.LoginID);
			}

			lock (CHATTING_USERS)
			{
				CHATTING_USERS.Clear();
			}

			_browser.Stop();
			loopURL = null;
			LOGIN_INFO = null;

			loginToolStripMenuItem.Visible = true;
			logoutToolStripMenuItem.Visible = false;
			msgLogToolStripMenuItem.Visible = false;
			toolStripMenuItem4.Visible = false;
			changeStatusToolStripMenuItem2.Enabled = false;

			// 개인 이미지 숨기기
			picSplitContainer.Panel1Collapsed = true;

			chatTimer.Enabled = false;
		}

		private void _browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			if ((_browser.Document != null) && (_browser.Document.Window != null))
			{
				_browser.Document.Window.Error += window_Error;
			}
		}

		private void _browser_DownloadComplete(object sender, EventArgs e)
		{
			if ((_browser.Document != null) && (_browser.Document.Window != null))
			{
				_browser.Document.Window.Error += window_Error;
			}
		}

		private static void window_Error(object sender, HtmlElementErrorEventArgs e)
		{
			e.Handled = true;
			Logger.Log(e.Url + " : " + e.Description + " (줄 번호: " + e.LineNumber + ")");
		}

		private void _browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			HtmlDocument doc = ((ExtendedWebBrowser) sender).Document;
			if ((doc != null) && (doc.Window != null) && (doc.Window.Document != null))
			{
				string targetURL = e.Url.AbsoluteUri.ToLower();
				if (targetURL.IndexOf("/access/fail.aspx") > 0)
				{
					Stop();
				}
				else if (targetURL.IndexOf("/webadmin/default.aspx") > 0)
				{
					if (loopURL == null) loopURL = e.Url;

					string sHtml = (doc.Window.Document.Body != null) ? doc.Window.Document.Body.OuterHtml : "";
					if (!processOverview(sHtml))
					{
						Logger.Log(Logger.LogLevel.WARNING, "로그인을 실패했습니다.");
						DialogResult dr = MessageBoxEx.Show("로그인을 실패했습니다. 웹 브라우저로 확인하시겠습니까?",
						                                    "접속 실패: NETS-ⓘMan",
						                                    MessageBoxButtons.YesNo,
						                                    MessageBoxIcon.Information,
						                                    MessageBoxDefaultButton.Button2,
						                                    60*1000);

						if (dr == DialogResult.Yes)
						{
							if (loopURL != null)
							{
								string url = loopURL.ToString();
								Process.Start(url);
								//_browser.Navigate(url, true);
							}
						}
						Stop();
					}
				}
				else if (targetURL.IndexOf("dev.nets.co.kr") < 0)
				{
					// 엉뚱한 URL로 가버린 경우
					Logger.Log(Logger.LogLevel.WARNING, "로그인을 실패했습니다: " + targetURL);
					DialogResult dr = MessageBoxEx.Show("로그인을 실패했습니다. 웹 브라우저로 확인하시겠습니까?",
														"접속 실패: NETS-ⓘMan",
														MessageBoxButtons.YesNo,
														MessageBoxIcon.Information,
														MessageBoxDefaultButton.Button2,
														60 * 1000);

					if (dr == DialogResult.Yes) Process.Start(targetURL);
					Stop();
				}
			}
		}

		private bool processOverview(string sHtml)
		{
			try
			{
				// 메인 페이지가 맞는지 다시 확인
				int pMain = sHtml.ToLower().IndexOf("/support/menu.aspx");
				if (pMain >= 0)
				{
					dirSvc.Timeout = 5000;
					dirSvc.Login(LOGIN_INFO.LoginID);
					chatSvc.Timeout = 5000;
					chatSvc.AddUser(LOGIN_INFO.LoginID);

					LAST_ACT_TIME = DateTime.Now;
					TreeNode node = treeUpdate("로그인 시각 ");
					if (node == null)
					{
						MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
										  "오류: NETS-ⓘMan",
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  60 * 1000);
						return true;
					}
					treeUpdate(node, LAST_ACT_TIME.ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x33CC33));
					treeExpand(node);

					// 사용자 소속 조직 찾기
					string deptName = dirSvc.GetUserDepartment(LOGIN_INFO.LoginID);

					// 조직도 읽기
					string groups = dirSvc.GetGroupTree();
					TreeNode group;
					fillGroupTreefromXml(groups, deptName, out group);

					// 임시/계약직 조직 추가
					TreeNode group2 = treeUpdate("임시/계약직 ");
					if (group2 == null)
					{
						MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
										  "오류: NETS-ⓘMan",
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  60 * 1000);
						return true;
					}
					group2.Tag = "TEMP";
					group2.ImageIndex = 0;
					group2.SelectedImageIndex = 0;
					group2.ContextMenuStrip = popupMenuStrip3;

					// 사용자 소속 조직원 목록 얻기
					TreeNode user;
					if (deptName == "") // 임시/계약직 구분
					{
						string members = dirSvc.GetTempUsers();
						fillUserTreefromXml(members, group2, out user);
					}
					else
					{
						string members = dirSvc.GetUsers((string) group.Tag);
						fillUserTreefromXml(members, group, out user);

						// 상위 조직 목록을 자동으로 얻는다.
						TreeNode parent = group.Parent;
						if (parent != null)
						{
							do
							{
								members = dirSvc.GetUsers((string) parent.Tag);
								TreeNode temp;
								fillUserTreefromXml2(members, parent, out temp);
								parent = parent.Parent;
							} while (parent != null);
						}
					}

					if (user == null)
					{
						Logger.Log(Logger.LogLevel.WARNING, "로그인 실패 - 정규직 또는 임시/계약직에 없음");
						treeView.Nodes.Clear();
						return false;
					}

					// 개인 이미지 보이기
					if (deptName == "") // 임시/계약직 구분
					{
						using (
							Stream stream =
								WebCall.GetStream(new Uri("http://sso.nets.co.kr/iManService/getPhoto.aspx?tempid=" + LOGIN_INFO.LoginID),
								                  ""))
						{
							Image img = Image.FromStream(stream);
							stream.Close();

							try
							{
								Image.GetThumbnailImageAbort dummyAbort = thumbnail_abortCallBack;
								Image thumbNail = img.GetThumbnailImage(42, 45, dummyAbort, IntPtr.Zero);
								pictureBox2.Tag = img;
								pictureBox2.Image = thumbNail;
							}
							catch (AccessViolationException)
							{
								pictureBox2.Tag = img;
								pictureBox2.Image = img;
							}
						}
					}
					else
					{
						using (
							Stream stream =
								WebCall.GetStream(new Uri("http://sso.nets.co.kr/iManService/getPhoto.aspx?uid=" + LOGIN_INFO.LoginID), ""))
						{
							Image img = Image.FromStream(stream);
							stream.Close();

							try
							{
								Image.GetThumbnailImageAbort dummyAbort = thumbnail_abortCallBack;
								Image thumbNail = img.GetThumbnailImage(42, 45, dummyAbort, IntPtr.Zero);
								pictureBox2.Tag = img;
								pictureBox2.Image = thumbNail;
							}
							catch (AccessViolationException)
							{
								pictureBox2.Tag = img;
								pictureBox2.Image = img;
							}
						}
					}

					try
					{
						picSplitContainer.Panel1Collapsed = false;
						picSplitContainer.Refresh();
					}
					catch
					{
					}

					user.ImageIndex = 2;
					user.SelectedImageIndex = 2;
					string uName = user.Text;
					if (uName.IndexOf("(") < 0)
						LOGIN_INFO.UserName = uName;
					else
						LOGIN_INFO.UserName = uName.Substring(0, uName.IndexOf("("));
					toolTip2.SetToolTip(pictureBox2, LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")");

					treeView.Focus();
					treeView.SelectedNode = user;

					string users = chatSvc.GetLoginUsers();
					showUsers(users);

					chatSvc.ReceiveOfflineMessageAsync(LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")");

					// 타이머 시작
					if (!chatTimer.Enabled) chatTimer.Enabled = true;
					return true;
				}

				Logger.Log(Logger.LogLevel.WARNING, "페이지 읽기 에러 - " + loopURL);
				return false;
			}
			catch (WebException ex)
			{
				Logger.Log(Logger.LogLevel.ERROR, "processOverview(): " + ex);
				return false;
			}
			catch (NullReferenceException ex)
			{
				if (LOGIN_INFO != null) ErrorReport.SendReport("processOverview", ex);

				return false;
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("processOverview", ex);
				Logger.Log(Logger.LogLevel.ERROR, "processOverview(): " + ex);
				return false;
			}
		}

		private static bool thumbnail_abortCallBack()
		{
			return false;
		}

		private void chatTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				if (chatProcessing) return;
				if (LOGIN_INFO == null) return;

				// 2분 이상 서버와 통신하지 않았으면 아무 것도 하지 않는다: 로그아웃 처리...
				if (LAST_ACT_TIME < DateTime.Now.AddMinutes(-2)) return;

				chatProcessing = true;
				if (IN_MESSAGE.Trim().Length == 0)
				{
					msgClearCount = 0;
					chatSvc.ReceiveMessageAsync(LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")");
				}
				else // 이전 메시지가 처리되지 않고 남아 있으면 건너뛴다.
				{
					msgClearCount++;
					if (msgClearCount > 3) // 3회 이상 메시지가 그대로 남아있으면 강제로 지운다.
					{
						lock (LOCK_OBJECT)
						{
							IN_MESSAGE = string.Empty;
						}
						msgClearCount = 0;
					}
					else
					{
						Logger.Log(Logger.LogLevel.WARNING,
						           "chatTimer_Tick(): 남은 메시지[" + IN_MESSAGE.Trim().Replace("\r\n", "").Replace("\n", "") + "]");
					}

					// 8번에 한 번(1.5*6=9초)씩 로그인 직원 상태 조회
					chatTickCount++;
					if (chatTickCount%6 == 0)
					{
						chatSvc.GetLoginUsersAsync();
					}

					// 39번에 한 번(1.5*39=58.5초)씩 쪽지 조회
					if (chatTickCount%39 == 0)
					{
						chatSvc.ReceiveOfflineMessageAsync(LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")");
						chatTickCount = 0;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("chatTimer_Tick", ex);
				Logger.Log(Logger.LogLevel.ERROR, "chatTimer_Tick(): " + ex);
			}
			chatProcessing = false;
		}

		private void chatSvc_GetLoginUsersCompleted(object sender, GetLoginUsersCompletedEventArgs e)
		{
			try
			{
				string strTemp = string.Empty;
				try
				{
					if (!e.Cancelled)
					{
						strTemp = e.Result;
						LAST_ACT_TIME = DateTime.Now;
					}
				}
				catch // 비동기 처리에 실패하면 건너뛴다.
				{
				}
				if (strTemp.Length == 0) return;
				if (LOGIN_INFO == null) return;

				showUsers(strTemp);
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("chatSvc_GetLoginUsersCompleted", ex);
				Logger.Log(Logger.LogLevel.ERROR, "chatSvc_GetLoginUsersCompleted(): " + ex);
			}
		}

		private void showUsers(string users)
		{
			string strUList = "|" + users + "|";
			int cmp = string.Compare(strUList, CACHED_USERS);
			if (cmp != 0)
			{
				CACHED_USERS = strUList;
				foreach (TreeNode node in treeView.Nodes)
					showUsersRecursive(node, strUList);
			}
		}

		private static void showUsersRecursive(TreeNode parent, string loginList)
		{
			foreach (TreeNode node in parent.Nodes)
			{
				if ((node.ImageIndex == 1) || (node.ImageIndex == 2) || (node.ImageIndex == 3) || (node.ImageIndex == 9))
				{
					string id = (string) node.Tag;
					if (loginList.IndexOf("|" + id + "|") >= 0) // 로그인
					{
						node.ImageIndex = 2;
						node.SelectedImageIndex = 2;
					}
					else if (loginList.IndexOf("|*" + id + "|") >= 0) // 자리 비움
					{
						node.ImageIndex = 3;
						node.SelectedImageIndex = 3;
					}
					else if (loginList.IndexOf("|/" + id + "|") >= 0) // 다른 용무 중
					{
						node.ImageIndex = 9;
						node.SelectedImageIndex = 9;
					}
					else // 로그아웃
					{
						node.ImageIndex = 1;
						node.SelectedImageIndex = 1;
					}
				}
				else
				{
					showUsersRecursive(node, loginList);
				}
			}
		}

		private void chatSvc_ReceiveMessageCompleted(object sender, ReceiveMessageCompletedEventArgs e)
		{
			try
			{
				string strTemp = string.Empty;
				try
				{
					if (!e.Cancelled)
					{
						strTemp = e.Result;
						LAST_ACT_TIME = DateTime.Now;
					}
				}
				catch // 비동기 처리에 실패하면 건너뛴다.
				{
				}

				if (LOGIN_INFO == null) return;

				// 8번에 한 번(1.5*6=9초)씩 로그인 직원 상태 조회
				chatTickCount++;
				if (chatTickCount%6 == 0)
				{
					chatSvc.GetLoginUsersAsync();
				}

				// 39번에 한 번(1.5*39=58.5초)씩 쪽지 조회
				if (chatTickCount%39 == 0)
				{
					chatSvc.ReceiveOfflineMessageAsync(LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")");
					chatTickCount = 0;
				}

				if (strTemp.Trim().Length > 0)
				{
					// 다른 컴퓨터에서 로그인 여부 체크
					if (strTemp.StartsWith("LOGOUT:"))
					{
						LOGIN_INFO = null;
						logoutToolStripMenuItem_Click(null, null);

						SoundPlayer.PlaySoundEvent("SystemHand");
						MessageBoxEx.Show("다른 컴퓨터(IP주소: " + strTemp.Substring(7) + ")에서 로그인하여 로그아웃되었습니다.",
						                  "중복 로그인",
						                  MessageBoxButtons.OK,
						                  MessageBoxIcon.Warning,
						                  30*1000);
						return;
					}

					// 서버 메시지 체크
					string[] msgs = strTemp.Split(new string[] {"\n\n\t\n"}, StringSplitOptions.RemoveEmptyEntries);

					bool showNotifyWnd = !offLoginAlarmToolStripMenuItem.Checked;
					int alarmCount = 0;
					foreach (string s in msgs)
					{
						if (s.Substring(0, 7) != "Ser@ver") continue;

						if (showNotifyWnd)
						{
							NotifyWindow nw = new NotifyWindow(s.Substring(s.IndexOf(":", 8) + 1));
							nw.TextClicked += notifyWindow_TextClicked;
							nw.Font = new Font("굴림", 9F);
							nw.SetDimensions(s.Length*2 + 240, 55);
							nw.Notify(alarmCount++);
						}

						// 메시지 삭제
						strTemp = strTemp.Replace(s, "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
					}

					lock (LOCK_OBJECT)
					{
						IN_MESSAGE = strTemp;
					}

					msgs = strTemp.Split(new string[] {"\n\n\t\n"}, StringSplitOptions.RemoveEmptyEntries);
					for (int cnt = 0; cnt < msgs.Length; cnt++)
					{
						string remoteUser = msgs[cnt].Substring(0, msgs[cnt].IndexOf(':'));

						bool userFlag = true;
						for (int i = 0; i < CHATTING_USERS.Count; i++)
						{
							if (remoteUser == CHATTING_USERS[i].ToString())
							{
								userFlag = false;
								break;
							}
						}

						string userid = remoteUser.IndexOf("(") >= 0
						                	? remoteUser.Substring(remoteUser.IndexOf("(")).Replace("(", "").Replace(")", "")
						                	: remoteUser;

						if (msgs[cnt].IndexOf("<file>JOIN") >= 0)
						{
							lock (LOCK_OBJECT)
							{
								IN_MESSAGE = IN_MESSAGE.Replace(msgs[cnt], "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
							}

							string[] arr = msgs[cnt].Split(new char[] {':'});
							RichTextBox rtbTemp = new RichTextBox();
							rtbTemp.Rtf = arr[2];

							string s = rtbTemp.Text.Substring(6);
							string[] arr2 = s.Split(new char[] {'|'});
							string groupID = arr2[1];

							bool groupFlag = true;
							for (int i = 0; i < CHATTING_GROUPS.Count; i++)
							{
								GroupItem group = (GroupItem) CHATTING_GROUPS[i];
								if (groupID == group.GroupID)
								{
									groupFlag = false;
									break;
								}
							}

							if (groupFlag)
							{
								SoundPlayer.PlaySoundEvent("SystemExclamation");
								DialogResult dr = MessageBoxEx.Show(remoteUser + "님이 회의실에 초대했습니다. 수락하시겠습니까?\r\n(회의실 ID={" + groupID + "})",
								                                    "회의실 초대",
								                                    MessageBoxButtons.YesNo,
								                                    MessageBoxIcon.Question,
								                                    MessageBoxDefaultButton.Button2,
								                                    60*1000);

								if (dr == DialogResult.Yes)
								{
									lock (CHATTING_GROUPS)
									{
										GroupItem group = new GroupItem(groupID);
										group.GroupNo = ++GROUP_COUNT;
										CHATTING_GROUPS.Add(group);
									}

									groupChatSvc.AddUserGroup(LOGIN_INFO.LoginID, groupID);

									GroupMessage f = new GroupMessage(groupID, remoteUser, true);
									wndArray.Add(f);
									f.Show();
								}
								else
								{
									groupChatSvc.SendGroupMessage("Gro@up", groupID,
									                              LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")님이 회의실 입장을 거부했습니다.");
								}
							}
						}
						else if (userFlag)
						{
							// 메시지를 보낸 상대가 오프라인이면 쪽지로 보여준다.
							if ((CACHED_USERS.IndexOf("|" + userid + "|") < 0) &&
							    (CACHED_USERS.IndexOf("|*" + userid + "|") < 0))
							{
								lock (LOCK_OBJECT)
								{
									IN_MESSAGE = IN_MESSAGE.Replace(msgs[cnt], "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
								}
								if (strTemp.IndexOf("<file>") < 0)
								{
									OfflineMessage f = new OfflineMessage(strTemp);
									wndArray.Add(f);
									f.Show();
								}
							}
							else
							{
								if (msgs[cnt].IndexOf("<file>STATUS") < 0)
								{
									lock (CHATTING_USERS)
									{
										CHATTING_USERS.Add(remoteUser);
									}
									PrivateMessage f = new PrivateMessage(userid, remoteUser, msgs[cnt]);
									wndArray.Add(f);
									f.Show();
								}
								else
								{
									// 대화창 없이 상태 메시지만 온 경우
									lock (LOCK_OBJECT)
									{
										IN_MESSAGE = IN_MESSAGE.Replace(msgs[cnt], "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("chatSvc_ReceiveMessageCompleted", ex);
				Logger.Log(Logger.LogLevel.ERROR, "chatSvc_ReceiveMessageCompleted(): " + ex);
			}
		}

		private void nw_TextClicked(object sender, TextClickedEventArgs e)
		{
			DialogResult dr = MessageBoxEx.Show("프로그램 업데이트를 위해 다시 시작하시겠습니까?",
			                                    "NETS-ⓘMan",
			                                    MessageBoxButtons.YesNo,
			                                    MessageBoxIcon.Question,
			                                    MessageBoxDefaultButton.Button2,
			                                    30*1000);

			if (dr == DialogResult.Yes)
			{
				string s = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
				s += @"\NETS\NETS-ⓘMan v1.0.appref-ms";
				Process.Start(new ProcessStartInfo(s));

				exitToolStripMenuItem_Click(null, null);
			}
		}

		private void notifyWindow_TextClicked(object sender, TextClickedEventArgs e)
		{
			string txt = e.Text;
			string id = txt.Substring(0, txt.IndexOf("님"));
			TreeNode theUser = findRemoteUser(id);
			if (theUser != null) treeView.SelectedNode = theUser;
			chatSvc.GetLoginUsersAsync();

			if (notifyIcon1.Visible)
			{
				showMainWindow();
			}
			else
			{
				TopMost = true;
				Thread.Sleep(50);
				TopMost = alwaysTopMost;
			}
		}

		private TreeNode findRemoteUser(string user)
		{
			TreeNode theNode = null;
			if (user.IndexOf("(") >= 0)
			{
				string id = user.Substring(user.IndexOf("(")).Replace("(", "").Replace(")", "");
				foreach (TreeNode node in treeView.Nodes)
					findRemoteUserRecursive(node, id, ref theNode);
			}
			else
			{
				string id = user;
				foreach (TreeNode node in treeView.Nodes)
					findRemoteUserRecursive(node, id, ref theNode);
			}
			return theNode;
		}

		private void findRemoteUserRecursive(TreeNode node, string loginID, ref TreeNode theNode)
		{
			Application.DoEvents();

			if ((node.ImageIndex == 1) || (node.ImageIndex == 2) || (node.ImageIndex == 3) || (node.ImageIndex == 9))
			{
				string id = (string) node.Tag;
				if (loginID == id)
				{
					theNode = node;
					return;
				}
			}
			else if (node.ImageIndex == 0)
			{
				bool userExist = false;
				foreach (TreeNode child in node.Nodes)
				{
					if ((child.ImageIndex == 1) || (child.ImageIndex == 2) || (child.ImageIndex == 3) || (child.ImageIndex == 9))
					{
						userExist = true;
						break;
					}
				}
				if (!userExist)
				{
					string groupID = (string) node.Tag;

					// 사용자 소속 조직원 목록 얻기
					string members = (groupID == "TEMP") ? dirSvc.GetTempUsers() : dirSvc.GetUsers(groupID);
					if (fillUserTreefromXml(members, node, loginID, ref theNode)) return;
				}
			}

			foreach (TreeNode child in node.Nodes)
				findRemoteUserRecursive(child, loginID, ref theNode);
		}

		private void chatSvc_ReceiveOfflineMessageCompleted(object sender, ReceiveOfflineMessageCompletedEventArgs e)
		{
			try
			{
				string strTemp = string.Empty;
				try
				{
					if (!e.Cancelled)
						strTemp = e.Result;
				}
				catch // 비동기 처리에 실패하면 건너뛴다.
				{
				}

				if (LOGIN_INFO == null) return;

				if (strTemp.Trim().Length > 0)
				{
					// 다른 컴퓨터에서 로그인 여부 체크
					if (strTemp.StartsWith("LOGOUT:"))
					{
						LOGIN_INFO = null;
						logoutToolStripMenuItem_Click(null, null);

						MessageBoxEx.Show("다른 컴퓨터(IP주소: " + strTemp.Substring(7) + ")에서 로그인하여 로그아웃되었습니다.",
						                  "중복 로그인",
						                  MessageBoxButtons.OK,
						                  MessageBoxIcon.Warning,
						                  30*1000);
						return;
					}

					OfflineMessage f = new OfflineMessage(strTemp);
					wndArray.Add(f);
					f.StartPosition = FormStartPosition.CenterScreen;
					f.Show();
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("chatSvc_ReceiveOfflineMessageCompleted", ex);
				Logger.Log(Logger.LogLevel.ERROR, "chatSvc_ReceiveOfflineMessageCompleted(): " + ex);
			}
		}

		private void fillGroupTreefromXml(string groups, string deptName, out TreeNode theGroup)
		{
			TreeNode root = treeUpdate("넷츠 ");
			if (root == null)
			{
				MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
								  "오류: NETS-ⓘMan",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  60 * 1000);
				theGroup = null;
				return;
			}
			root.Tag = "NETSALL";
			root.ImageIndex = 0;
			root.SelectedImageIndex = 0;
			root.ContextMenuStrip = popupMenuStrip3;
			theGroup = root;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(groups);
			XmlNodeList nodes = doc.SelectNodes("/root/group");
			treeAddRecursive(nodes, root, deptName, ref theGroup);
			treeExpand(root);
		}

		private void treeAddRecursive(XmlNodeList xmlNodes, TreeNode treeNode, string deptName, ref TreeNode theGroup)
		{
			Application.DoEvents();

			if (xmlNodes == null) return;
			foreach (XmlNode node in xmlNodes)
			{
				string name = node.Attributes["name"].InnerText;
				TreeNode tree = treeUpdate(treeNode, name);
				if (tree == null)
				{
					MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
									  "오류: NETS-ⓘMan",
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Exclamation,
									  60 * 1000);
					return;
				}
				tree.Tag = node.Attributes["id"].InnerText;
				tree.ImageIndex = 0;
				tree.SelectedImageIndex = 0;
				tree.ContextMenuStrip = popupMenuStrip3;
				if (name.Equals(deptName)) theGroup = tree;

				if (node.ChildNodes.Count > 0)
				{
					XmlNodeList nodes = node.ChildNodes;
					treeAddRecursive(nodes, tree, deptName, ref theGroup);
				}
			}
		}

		private void fillUserTreefromXml(string members, TreeNode group, out TreeNode theUser)
		{
			theUser = null;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(members);
			XmlNodeList xmlNodes = doc.SelectNodes("/group/user");
			if (xmlNodes == null) return;

			bool showOnlyOnline = SettingsHelper.Current.ShowOnline;
			foreach (XmlNode node in xmlNodes)
			{
				string id = node.Attributes["id"].InnerText;
				string status = node.Attributes["status"].InnerText;
				if (showOnlyOnline && (status == "0") && !id.Equals(LOGIN_INFO.LoginID, StringComparison.CurrentCultureIgnoreCase))
					continue;

				string name = node.Attributes["name"].InnerText;
				string title = node.Attributes["title"].InnerText;
				if (title.Trim().Length == 0) title = "사원";

				string empID;
				TreeNode tree;
				try
				{
					empID = node.Attributes["employeeID"].InnerText;
				}
				catch
				{
					empID = "";
				}

				try
				{
					tree = treeUpdate(group, title + " " + name);
				}
				catch
				{
					tree = treeUpdate(group, title + " " + name + "(" + empID + ")");
				}
				if (tree == null)
				{
					MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
									  "오류: NETS-ⓘMan",
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Exclamation,
									  60 * 1000);
					return;
				}
				tree.Tag = id;
				tree.ContextMenuStrip = popupMenuStrip2;

				if (status == "1") // 로그인
				{
					tree.ImageIndex = 2;
					tree.SelectedImageIndex = 2;
				}
				else if (status == "9") // 자리비움
				{
					tree.ImageIndex = 3;
					tree.SelectedImageIndex = 3;
				}
				else if (status == "2") // 다른 용무 중
				{
					tree.ImageIndex = 9;
					tree.SelectedImageIndex = 9;
				}
				else // 로그아웃
				{
					tree.ImageIndex = 1;
					tree.SelectedImageIndex = 1;
				}
				if (LOGIN_INFO.LoginID.Equals(id)) theUser = tree;
				Application.DoEvents();
			}
			treeExpand(group);
		}

		private void fillUserTreefromXml2(string members, TreeNode group, out TreeNode theUser)
		{
			theUser = null;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(members);
			XmlNodeList xmlNodes = doc.SelectNodes("/group/user");
			if (xmlNodes == null) return;

			bool showOnlyOnline = SettingsHelper.Current.ShowOnline;
			for (int i = xmlNodes.Count - 1; i >= 0; i--)
			{
				XmlNode node = xmlNodes[i];
				string id = node.Attributes["id"].InnerText;
				string status = node.Attributes["status"].InnerText;
				if (showOnlyOnline && (status == "0") && !id.Equals(LOGIN_INFO.LoginID, StringComparison.CurrentCultureIgnoreCase))
					continue;

				string name = node.Attributes["name"].InnerText;
				string title = node.Attributes["title"].InnerText;
				if (title.Trim().Length == 0) title = "사원";
				try
				{
					TreeNode tree = treeInsert(group, title + " " + name);

					tree.Tag = id;
					tree.ContextMenuStrip = popupMenuStrip2;

					if (status == "1") // 로그인
					{
						tree.ImageIndex = 2;
						tree.SelectedImageIndex = 2;
					}
					else if (status == "9") // 자리비움
					{
						tree.ImageIndex = 3;
						tree.SelectedImageIndex = 3;
					}
					else if (status == "2") // 다른 용무 중
					{
						tree.ImageIndex = 9;
						tree.SelectedImageIndex = 9;
					}
					else // 로그아웃
					{
						tree.ImageIndex = 1;
						tree.SelectedImageIndex = 1;
					}
					if (LOGIN_INFO.LoginID.Equals(id)) theUser = tree;
				}
				catch
				{
				}
				Application.DoEvents();
			}
			treeExpand(group);
		}

		private bool fillUserTreefromXml(string members, TreeNode group, string loginID, ref TreeNode theUser)
		{
			bool result = false;
			bool isExpand = group.IsExpanded;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(members);
			XmlNodeList xmlNodes = doc.SelectNodes("/group/user");
			if (xmlNodes == null) return result;

			bool showOnlyOnline = SettingsHelper.Current.ShowOnline;
			foreach (XmlNode node in xmlNodes)
			{
				string id = node.Attributes["id"].InnerText;
				string status = node.Attributes["status"].InnerText;
				if (showOnlyOnline && (status == "0") && !id.Equals(LOGIN_INFO.LoginID, StringComparison.CurrentCultureIgnoreCase))
					continue;

				string name = node.Attributes["name"].InnerText;
				string title = node.Attributes["title"].InnerText;
				if (title.Trim().Length == 0) title = "사원";
				try
				{
					TreeNode tree = treeUpdate(group, title + " " + name);
					if (tree == null)
					{
						MessageBoxEx.Show("조직도 갱신에 실패했습니다. 잠시 후 다시 시도해보시기 바랍니다.",
										  "오류: NETS-ⓘMan",
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  60 * 1000);
						return true;
					}

					tree.Tag = id;
					tree.ContextMenuStrip = popupMenuStrip2;

					if (status == "1")
					{
						tree.ImageIndex = 2;
						tree.SelectedImageIndex = 2;
					}
					else if (status == "9") // 자리비움
					{
						tree.ImageIndex = 3;
						tree.SelectedImageIndex = 3;
					}
					else if (status == "2") // 다른 용무 중
					{
						tree.ImageIndex = 9;
						tree.SelectedImageIndex = 9;
					}
					else
					{
						tree.ImageIndex = 1;
						tree.SelectedImageIndex = 1;
					}
					if (loginID == id)
					{
						theUser = tree;
						result = true;
					}
				}
				catch
				{
				}
				Application.DoEvents();
			}

			if (isExpand)
				treeExpand(group);
			else
				treeCollapse(group);

			return result;
		}

		private TreeNode treeInsert(TreeNode node, string text)
		{
			return treeInsert(node, text, Color.Black, false);
		}

		private TreeNode treeInsert(TreeNode node, string text, Color textColor, bool doChildClear)
		{
			try
			{
				if (treeView.InvokeRequired)
				{
					TreeUpdateCallback callback = treeInsert;
					return (TreeNode)Invoke(callback, new object[] { node, text, textColor, doChildClear });
				}

				if (node == null)
				{
					for (int i = 0; i < treeView.Nodes.Count; i++)
					{
						TreeNode existNode = treeView.Nodes[i];
						if (existNode.Text.Equals(text))
						{
							if (doChildClear) existNode.Nodes.Clear();
							return existNode;
						}
					}
					TreeNode newNode = treeView.Nodes.Insert(0, text);
					newNode.NodeFont = new Font(treeView.Font, FontStyle.Bold);
					newNode.ForeColor = textColor;
					newNode.ImageIndex = 5;
					newNode.SelectedImageIndex = 6;
					return newNode;
				}
				else
				{
					for (int i = 0; i < node.Nodes.Count; i++)
					{
						TreeNode existNode = node.Nodes[i];
						if (existNode.Text.Equals(text))
						{
							if (doChildClear) existNode.Nodes.Clear();
							return existNode;
						}
					}
					TreeNode newNode = node.Nodes.Insert(0, text);
					newNode.ForeColor = textColor;
					newNode.ImageIndex = 8;
					newNode.SelectedImageIndex = 8;
					return newNode;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private TreeNode treeUpdate(string text)
		{
			return treeUpdate(text, Color.Black);
		}

		private TreeNode treeUpdate(string text, bool doChildClear)
		{
			return treeUpdate(null, text, doChildClear);
		}

		private TreeNode treeUpdate(string text, Color textColor)
		{
			return treeUpdate(null, text, textColor, true);
		}

		private TreeNode treeUpdate(TreeNode node, string text)
		{
			return treeUpdate(node, text, false);
		}

		private TreeNode treeUpdate(TreeNode node, string text, bool doChildClear)
		{
			return treeUpdate(node, text, Color.Black, doChildClear);
		}

		private TreeNode treeUpdate(TreeNode node, string text, Color textColor)
		{
			return treeUpdate(node, text, textColor, false);
		}

		private TreeNode treeUpdate(TreeNode node, string text, Color textColor, bool doChildClear)
		{
			try
			{
				if (treeView.InvokeRequired)
				{
					TreeUpdateCallback callback = treeUpdate;
					return (TreeNode)Invoke(callback, new object[] { node, text, textColor, doChildClear });
				}

				if (node == null)
				{
					for (int i = 0; i < treeView.Nodes.Count; i++)
					{
						TreeNode existNode = treeView.Nodes[i];
						if (existNode.Text.Equals(text))
						{
							if (doChildClear) existNode.Nodes.Clear();
							return existNode;
						}
					}
					TreeNode newNode = treeView.Nodes.Add(text);
					newNode.NodeFont = new Font(treeView.Font, FontStyle.Bold);
					newNode.ForeColor = textColor;
					newNode.ImageIndex = 5;
					newNode.SelectedImageIndex = 6;
					return newNode;
				}
				else
				{
					for (int i = 0; i < node.Nodes.Count; i++)
					{
						TreeNode existNode = node.Nodes[i];
						if (existNode.Text.Equals(text))
						{
							if (doChildClear) existNode.Nodes.Clear();
							return existNode;
						}
					}
					TreeNode newNode = node.Nodes.Add(text);
					newNode.ForeColor = textColor;
					newNode.ImageIndex = 8;
					newNode.SelectedImageIndex = 8;
					return newNode;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void treeExpand(TreeNode node)
		{
			try
			{
				if (treeView.InvokeRequired)
				{
					TreeExpandeCallback callback = treeExpand;
					Invoke(callback, new object[] { node });
				}
				else
				{
					if (node != null) node.Expand();
				}
			}
			catch (Exception)
			{
			}
		}

		private void treeCollapse(TreeNode node)
		{
			try
			{
				if (treeView.InvokeRequired)
				{
					TreeCollapseCallback callback = treeCollapse;
					Invoke(callback, new object[] { node });
				}
				else
				{
					if (node != null) node.Collapse(true);
				}
			}
			catch (Exception)
			{
			}
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{
#if TEST
			/*
			string text = "123\r\t\n456\r\t\n789";
			string[] msgs = text.Split(new string[] { "\r\t\n" }, StringSplitOptions.RemoveEmptyEntries);
			MessageBox.Show("테스트: " + msgs.Length);
			*/
			//notifyWindow_TextClicked(null, new TextClickedEventArgs("test님이 로그인했습니다."));
			//checkUpdateOnline();
			frmChatHistory frm = new frmChatHistory();
			frm.Show();
#endif
		}

		private void pictureBox1_DoubleClick(object sender, EventArgs e)
		{
			if ((loopURL != null) && (LOGIN_INFO != null))
				openWebBrowser(LOGIN_INFO.PostData);
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			bool t = TopMost;
			TopMost = false;

			frmPicture pic = new frmPicture();
			pic.Img = (Image) pictureBox2.Tag;
			pic.Left = Left - 5;
			if (pic.Left < 0) pic.Left = 0;
			pic.Top = Top - 5;
			if (pic.Top < 0) pic.Top = 0;
			pic.ShowDialog();

			TopMost = t;
		}

		private void devSiteBrowseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((loopURL != null) && (LOGIN_INFO != null))
				openWebBrowser(LOGIN_INFO.PostData);
		}

		private void mListBrowseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((loopURL != null) && (LOGIN_INFO != null))
				openWebBrowser(LOGIN_INFO.GetMListPostData());
		}

		private void dListBrowseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((loopURL != null) && (LOGIN_INFO != null))
				openWebBrowser(LOGIN_INFO.GetDListPostData());
		}

		private void openWebBrowser(byte[] postData)
		{
			if ((loopURL != null) && (LOGIN_INFO != null))
			{
				//_browser.Navigate("http://dev.nets.co.kr/IM25/WebAdmin/Default.aspx", true);
				_browser.Navigate(LOGIN_INFO.URL, "_BLANK", postData,
				                  "Accept-Language: ko\r\nContent-Type: application/x-www-form-urlencoded\r\nAccept-Encoding: gzip, deflate\r\nProxy-Connection: Keep-Alive\r\n");
			}
		}

		private void statusTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				if (statusChanging) return;

				statusChanging = true;
				if (LOGIN_INFO != null)
				{
					// 2분 이상 서버와 통신하지 않았으면 로그아웃
					if (LAST_ACT_TIME < DateTime.Now.AddMinutes(-2))
					{
						logoutToolStripMenuItem_Click(null, null);
						statusChanging = false;
						return;
					}

					// 자리비움이나 다른 용무 중이면 건너뛴다
					if (!forceBusy && !forceAbsent)
					{
#if DEBUG
						const int timeOut = 1*60*1000; // 10분 이상 자리를 비우면
#else
						const int timeOut = 10*60*1000; // 10분 이상 자리를 비우면
#endif
						if (Win32.GetIdleTime() > timeOut)
						{
							if (!LOGIN_INFO.IsAbsent) // 로그인 상태를 자리비움으로 바꾼다.
							{
								LOGIN_INFO.IsAbsent = true;
								try
								{
									dirSvc.Absent(LOGIN_INFO.LoginID);
									chatSvc.AbsentUser(LOGIN_INFO.LoginID, true);
									ERROR_COUNT = 0;
								}
								catch (WebException ex)
								{
									// 네트워크 또는 DNS 에러
									if (ERROR_COUNT < 5)
									{
										ERROR_COUNT++;
										Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
									}
								}
								catch (InvalidOperationException ex)
								{
									if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
									{
										if (ERROR_COUNT < 5)
										{
											ERROR_COUNT++;
											Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
										}
									}
									else
										throw;
								}
							}
						}
						else
						{
							if (LOGIN_INFO.IsAbsent) // 자리비움 상태를 로그인으로 바꾼다.
							{
								LOGIN_INFO.IsAbsent = false;
								try
								{
									dirSvc.Login(LOGIN_INFO.LoginID);
									chatSvc.AbsentUser(LOGIN_INFO.LoginID, false);
									ERROR_COUNT = 0;
								}
								catch (WebException ex)
								{
									// 네트워크 또는 DNS 에러
									if (ERROR_COUNT < 5)
									{
										ERROR_COUNT++;
										Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
									}
								}
								catch (InvalidOperationException ex)
								{
									if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
									{
										if (ERROR_COUNT < 5)
										{
											ERROR_COUNT++;
											Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
										}
									}
									else
										throw;
								}
							}

							bool isTop = this.TopMost && !notifyIcon1.Visible;
							bool wasBusy = LOGIN_INFO.IsBusy;

							if (NIUtil.IsMaxWindow() && !isTop) // 다른 용무 중 체크
							{
								LOGIN_INFO.IsBusy = true;
								if (!wasBusy)
								{
									try
									{
										dirSvc.Busy(LOGIN_INFO.LoginID);
										chatSvc.BusyUser(LOGIN_INFO.LoginID, true);
										ERROR_COUNT = 0;
									}
									catch (WebException ex)
									{
										// 네트워크 또는 DNS 에러
										if (ERROR_COUNT < 5)
										{
											ERROR_COUNT++;
											Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
										}
									}
									catch (InvalidOperationException ex)
									{
										if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
										{
											if (ERROR_COUNT < 5)
											{
												ERROR_COUNT++;
												Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
											}
										}
										else
											throw;
									}
								}
							}
							else
							{
								LOGIN_INFO.IsBusy = false;
								if (wasBusy)
								{
									try
									{
										dirSvc.Login(LOGIN_INFO.LoginID);
										chatSvc.BusyUser(LOGIN_INFO.LoginID, false);
										ERROR_COUNT = 0;
									}
									catch (WebException ex)
									{
										// 네트워크 또는 DNS 에러
										if (ERROR_COUNT < 5)
										{
											ERROR_COUNT++;
											Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
										}
									}
									catch (InvalidOperationException ex)
									{
										if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
										{
											if (ERROR_COUNT < 5)
											{
												ERROR_COUNT++;
												Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[statusTimer_Tick()]: " + ex);
											}
										}
										else
											throw;
									}
								}
							}
						}
					}
				}

				if (notifyIcon1.Visible)
				{
					statusTickCount++;
					switch (statusTickCount%6) // 6초에 한번씩 트레이 아이콘 모양을 변화시킨다.
					{
						case 5:
							notifyIcon1.Icon = new Icon(getIconResourceStream2());
							break;
						case 0:
							notifyIcon1.Icon = new Icon(getIconResourceStream());
							statusTickCount = 0;
							if (LOGIN_INFO == null)
								notifyIcon1.Text = Text;
							else
								notifyIcon1.Text = Text + ": " + (LOGIN_INFO.IsAbsent ? "자리 비움" : (LOGIN_INFO.IsBusy ? "다른 용무 중" : "온라인"));
							break;
					}
				}

				updateTickCount++;
				if (updateTickCount > 3636) // 1시간에 1번씩 체크한다. (0.99초마다 반복이므로 1시간=3636)
				{
					updateTickCount = 0;
					checkUpdateOnline();
				}
			}
			catch (WebException ex)
			{
				Logger.Log(Logger.LogLevel.ERROR, "statusTimer_Tick(): " + ex);
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("statusTimer_Tick", ex);
				Logger.Log(Logger.LogLevel.ERROR, "statusTimer_Tick(): " + ex);
			}
			statusChanging = false;
		}

		private void checkUpdateOnline()
		{
			try
			{
				string manifest = WebCall.GetHtml(new Uri("http://sso.nets.co.kr/iman/NETS-iMan.application"), "");
				if (manifest.StartsWith("ERROR:")) return;
				if (manifest.StartsWith("<?")) manifest = manifest.Substring(manifest.IndexOf("\n") + 1);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(manifest);
				if (doc.DocumentElement == null) return;

				XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
				nsMgr.AddNamespace("asmv1", doc.DocumentElement.NamespaceURI);

				XmlNode node = doc.SelectSingleNode("//asmv1:assemblyIdentity", nsMgr);
				if ((node == null) || (node.Attributes["version"] == null)) return;
				{
					string newVer = node.Attributes["version"].InnerText;
#if DEBUG
					string curVer = "1.0.0.49";
#else
					string curVer = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
#endif
					if (!curVer.Equals(newVer))
					{
						string title = "[업데이트 발견]";
						string text = "NETS-ⓘMan 최신버전(" + newVer + ")이 나왔습니다.\r\n종료 후 다시 실행하시면 최신버전으로 업데이트하실 수 있습니다.";
						NotifyWindow nw = new NotifyWindow(title, text);
						nw.Font = new Font("굴림", 9F);
						nw.SetDimensions(440, 85);
						nw.TextClicked += nw_TextClicked;
						nw.Notify(0);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.WARNING, "checkUpdateOnline(): " + ex);
			}
		}

		private void treeView_MouseDown(object sender, MouseEventArgs e)
		{
			treeView.SelectedNode = treeView.GetNodeAt(e.X, e.Y);
		}

		private void treeView_MouseUp(object sender, MouseEventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;
			if (LOGIN_INFO == null) return; // 로그아웃이면

			if (popupMenuStrip2.Items.Count > 8)
			{
				for (int i = 8; i < popupMenuStrip2.Items.Count; i++)
					popupMenuStrip2.Items.RemoveAt(i--);
			}

			if (LOGIN_INFO.LoginID == (string) theNode.Tag)
			{
				viewProfileToolStripMenuItem.Text = "내 프로필 보기(&V)";
				netsQAToolStripMenuItem.Visible = true;
				changeStatusToolStripMenuItem.Visible = true;
				toolStripMenuItem2.Visible = false;
				privateMessageToolStripMenuItem.Visible = false;
				offlineMessageToolStripMenuItem.Visible = false;
				newGroupToolStripMenuItem.Visible = false;
			}
			else
			{
				viewProfileToolStripMenuItem.Text = "프로필 보기(&V)";
				netsQAToolStripMenuItem.Visible = false;
				changeStatusToolStripMenuItem.Visible = false;
				toolStripMenuItem2.Visible = true;

				if ((theNode.ImageIndex == 2) || (theNode.ImageIndex == 3) || (theNode.ImageIndex == 9)) // 로그인 사용자 or 자리비움 사용자 or 다른 용무 중 사용자
				{
					privateMessageToolStripMenuItem.Visible = true;
					offlineMessageToolStripMenuItem.Visible = true;
					newGroupToolStripMenuItem.Visible = true;

					for (int i = 0; i < CHATTING_GROUPS.Count; i++)
					{
						GroupItem group = (GroupItem) CHATTING_GROUPS[i];
						string groupID = group.GroupID;

						bool isExist = false;
						foreach (string id in group.UserList)
						{
							if (id == (string) theNode.Tag)
							{
								isExist = true;
								break;
							}
						}

						if (!isExist)
						{
							ToolStripMenuItem groupToolStripMenuItem = new ToolStripMenuItem();
							groupToolStripMenuItem.Text = "제" + group.GroupNo + " 회의실에 초대";
							groupToolStripMenuItem.ToolTipText = "선택한 직원을 제" + group.GroupNo + " 회의실{" + groupID + "}에 초대(포함)합니다.";
							groupToolStripMenuItem.Tag = groupID;
							groupToolStripMenuItem.Click += groupToolStripMenuItem_Click;
							popupMenuStrip2.Items.Add(groupToolStripMenuItem);
						}
					}
				}
				else if (theNode.ImageIndex == 1) // 로그아웃 사용자
				{
					privateMessageToolStripMenuItem.Visible = false;
					offlineMessageToolStripMenuItem.Visible = true;
					newGroupToolStripMenuItem.Visible = false;
				}
			}
		}

		private void treeView_DoubleClick(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			if (theNode.ImageIndex == 0)
			{
				// 더블클릭을 하는 순간 이미 펼침상태는 바뀌어 있다.
				bool goExpanding = theNode.IsExpanded;
				if (!goExpanding && (theNode.Nodes.Count > 0)) return;

				bool removed;
				do
				{
					removed = false;
					foreach (TreeNode node in theNode.Nodes)
					{
						if ((node.ImageIndex == 1) || (node.ImageIndex == 2) || (node.ImageIndex == 3) || (node.ImageIndex == 9))
						{
							node.Remove();
							removed = true;
						}
					}
				} while (removed);

				string groupID = (string) theNode.Tag;

				// 사용자 소속 조직원 목록 얻기
				string members = (groupID == "TEMP") ? dirSvc.GetTempUsers() : dirSvc.GetUsers(groupID);
				TreeNode user = null;
				try
				{
					fillUserTreefromXml2(members, theNode, out user);
				}
				catch (Exception)
				{
					theNode.Remove();
				}

				if (user != null)
				{
					user.ImageIndex = 2;
					user.SelectedImageIndex = 2;
					treeView.SelectedNode = user;

					string uName = user.Text;
					if (uName.IndexOf("(") < 0)
						LOGIN_INFO.UserName = uName;
					else
						LOGIN_INFO.UserName = uName.Substring(0, uName.IndexOf("("));
				}
			}
			else if (theNode.ImageIndex == 1) // 로그아웃 사용자
			{
				try
				{
					// 오프라인 메시지 보내기
					string userid = (string) theNode.Tag;
					if (userid == LOGIN_INFO.LoginID) return;

					string uName = theNode.Text;
					if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

					OfflineMessage f = new OfflineMessage(userid, uName + "(" + userid + ")", null);
					wndArray.Add(f);
					f.StartPosition = FormStartPosition.CenterScreen;
					f.Show();
				}
				catch (Exception ex)
				{
					ErrorReport.SendReport("treeView_DoubleClick-쪽지 보내기", ex);
					Logger.Log(Logger.LogLevel.ERROR, "treeView_DoubleClick(): " + ex);
				}
			}
			else if ((theNode.ImageIndex == 2) || (theNode.ImageIndex == 3) || (theNode.ImageIndex == 9)) // 로그인 사용자
			{
				try
				{
					string userid = (string) theNode.Tag;
					if (userid == LOGIN_INFO.LoginID) return;

					bool userFlag = true;
					for (int i = 0; i < CHATTING_USERS.Count; i++)
					{
						if (CHATTING_USERS[i].ToString().IndexOf("(" + userid + ")") >= 0)
							userFlag = false;
					}

					string uName = theNode.Text;
					if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

					if (userFlag)
					{
						lock (CHATTING_USERS)
						{
							CHATTING_USERS.Add(uName + "(" + userid + ")");
						}
						PrivateMessage f = new PrivateMessage(userid, uName + "(" + userid + ")");
						wndArray.Add(f);
						f.Show();
					}
					else
					{
						foreach (Form f in wndArray)
						{
							if (f.Text.IndexOf(uName + "(" + userid + ")") >= 0)
							{
								f.TopMost = true;
								f.Focus();
								f.TopMost = false;
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					ErrorReport.SendReport("treeView_DoubleClick-대화창 실행", ex);
					Logger.Log(Logger.LogLevel.ERROR, "treeView_DoubleClick(): " + ex);
				}
			}
		}

		private void privateMessageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			try
			{
				string userid = (string) theNode.Tag;
				if (userid == LOGIN_INFO.LoginID) return;

				bool userFlag = true;
				for (int i = 0; i < CHATTING_USERS.Count; i++)
				{
					if (CHATTING_USERS[i].ToString().IndexOf("(" + userid + ")") >= 0)
						userFlag = false;
				}

				string uName = theNode.Text;
				if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

				if (userFlag)
				{
					lock (CHATTING_USERS)
					{
						CHATTING_USERS.Add(uName + "(" + userid + ")");
					}
					PrivateMessage f = new PrivateMessage(userid, uName + "(" + userid + ")");
					wndArray.Add(f);
					f.Show();
				}
				else
				{
					foreach (Form f in wndArray)
					{
						if (f.Text.IndexOf(uName + "(" + userid + ")") >= 0)
						{
							f.TopMost = true;
							f.Focus();
							f.TopMost = false;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("privateMessageToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "privateMessageToolStripMenuItem_Click(): " + ex);
			}
		}

		private void offlineMessageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			try
			{
				// 오프라인 메시지 보내기
				string userid = (string) theNode.Tag;
				if (userid == LOGIN_INFO.LoginID) return;

				string uName = theNode.Text;
				if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

				OfflineMessage f = new OfflineMessage(userid, uName + "(" + userid + ")", null);
				wndArray.Add(f);
				f.StartPosition = FormStartPosition.CenterScreen;
				f.Show();
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("offlineMessageToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "offlineMessageToolStripMenuItem_Click(): " + ex);
			}
		}

		private void viewProfileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string id = (string) treeView.SelectedNode.Tag;
			if ((loopURL != null) && (LOGIN_INFO != null))
			{
				openWebBrowser(LOGIN_INFO.GetProfilePostData(id));
			}
		}

		private void openPrvHomePageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				TreeNode theNode = treeView.SelectedNode;
				string id = (string) theNode.Tag;

				string userXml = ((string) theNode.Parent.Tag) == "TEMP" ? dirSvc.GetTempUserInfo(id) : dirSvc.GetUserInfo(id);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(userXml);
				XmlNode node = doc.SelectSingleNode("/user");
				if (node == null) return;

				string www = node.Attributes["homePage"].InnerText;
				if (!string.IsNullOrEmpty(www) && www.StartsWith("http"))
				{
					Process.Start(www);
				}
				else
				{
					MessageBoxEx.Show("홈페이지가 없거나 잘못된 주소입니다.", "개인 홈페이지 연결",
					                  MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
				}
				//_browser.Navigate(www, "_BLANK", null, "");
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.WARNING, "openPrvHomePageToolStripMenuItem_Click(): " + ex);
			}
		}

		private void netsQAToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			try
			{
				// 오프라인 메시지 보내기
				string userid = (string) theNode.Tag;
				if (userid != LOGIN_INFO.LoginID) return;

				Uri qaUrl = new Uri("http://netsqa.nets.co.kr/qa/login.do");

				SettingsHelper setting = SettingsHelper.Current;
				string decPwd;
				if (string.IsNullOrEmpty(setting.NETSQAPassword))
					decPwd = LOGIN_INFO.Password;
				else
				{
					if (NISecurity.IsStrongKey(LOGIN_INFO.LoginID))
					{
						string pwd = setting.NETSQAPassword.Replace("enc:", "");
						decPwd = NISecurity.Decrypt(LOGIN_INFO.LoginID, pwd);
					}
					else
					{
						string key = LOGIN_INFO.LoginID + "12345678";
						string pwd = setting.NETSQAPassword.Replace("enc:", "");
						decPwd = NISecurity.Decrypt(key, pwd);
					}
				}
				_browser.Navigate(qaUrl, "_BLANK", LOGIN_INFO.GetQAPostData(decPwd),
				                  "Accept-Language: ko\r\nContent-Type: application/x-www-form-urlencoded\r\nAccept-Encoding: gzip, deflate\r\nProxy-Connection: Keep-Alive\r\n");
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("netsQAToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "netsQAToolStripMenuItem_Click(): " + ex);
			}
		}

		private void changeStatus1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool wasBusy = LOGIN_INFO.IsBusy;
			bool wasAbsent = LOGIN_INFO.IsAbsent;
			LOGIN_INFO.IsBusy = forceBusy = false;
			LOGIN_INFO.IsAbsent = forceAbsent = false;
			if (wasBusy) chatSvc.BusyUser(LOGIN_INFO.LoginID, false);
			if (wasAbsent) chatSvc.AbsentUser(LOGIN_INFO.LoginID, false);
		}

		private void changeStatus2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool wasBusy = LOGIN_INFO.IsBusy;
			LOGIN_INFO.IsBusy = forceBusy = true;
			LOGIN_INFO.IsAbsent = forceAbsent = false;
			if (!wasBusy) chatSvc.BusyUser(LOGIN_INFO.LoginID, true);
		}

		private void changeStatus3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool wasAbsent = LOGIN_INFO.IsAbsent;
			LOGIN_INFO.IsBusy = forceBusy = false;
			LOGIN_INFO.IsAbsent = forceAbsent = true;
			if (!wasAbsent) chatSvc.AbsentUser(LOGIN_INFO.LoginID, true);
		}

		private void newGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			// 회의실 생성
			string groupID = groupChatSvc.CreateGroup();

			lock (CHATTING_GROUPS)
			{
				GroupItem group = new GroupItem(groupID);
				group.GroupNo = ++GROUP_COUNT;
				CHATTING_GROUPS.Add(group);
			}
			// 회의실에 입장
			groupChatSvc.AddUserGroup(LOGIN_INFO.LoginID, groupID);

			// 회의실에 초대
			string userid = (string) theNode.Tag;
			RichTextBox rtbTemp = new RichTextBox();
			rtbTemp.Text = "<file>JOIN|" + groupID;

			string uName = theNode.Text;
			if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

			chatSvc.SendMessage(LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")", uName + "(" + userid + ")", rtbTemp.Rtf);

			// 회의실 창 실행
			GroupMessage frm = new GroupMessage(groupID, uName + "(" + userid + ")");
			wndArray.Add(frm);
			frm.Show();
		}

		private void groupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			string groupID = (string) ((ToolStripMenuItem) sender).Tag;
			RichTextBox rtbTemp = new RichTextBox();
			rtbTemp.Text = "<file>JOIN|" + groupID;

			string uName = theNode.Text;
			if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

			chatSvc.SendMessage(LOGIN_INFO.UserName + "(" + LOGIN_INFO.LoginID + ")", uName + "(" + theNode.Tag + ")",
			                    rtbTemp.Rtf);

			foreach (GroupItem group in CHATTING_GROUPS)
			{
				if (group.GroupID == groupID)
				{
					MessageBoxEx.Show(uName + "님을 제" + group.GroupNo + " 회의실에 초대했습니다.", "회의실 초대",
					                  MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
					break;
				}
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			// 항상 펼친다.
			if (theNode.ImageIndex == 0)
			{
				bool removed;
				do
				{
					removed = false;
					foreach (TreeNode node in theNode.Nodes)
					{
						if ((node.ImageIndex == 1) || (node.ImageIndex == 2) || (node.ImageIndex == 3) || (node.ImageIndex == 9))
						{
							node.Remove();
							removed = true;
						}
					}
				} while (removed);

				string groupID = (string) theNode.Tag;

				// 사용자 소속 조직원 목록 얻기
				string members = (groupID == "TEMP") ? dirSvc.GetTempUsers() : dirSvc.GetUsers(groupID);
				TreeNode user = null;
				try
				{
					fillUserTreefromXml2(members, theNode, out user);
				}
				catch (Exception)
				{
					theNode.Remove();
				}

				if (user != null)
				{
					user.ImageIndex = 2;
					user.SelectedImageIndex = 2;
					treeView.SelectedNode = user;

					string uName = user.Text;
					if (uName.IndexOf("(") < 0)
						LOGIN_INFO.UserName = uName;
					else
						LOGIN_INFO.UserName = uName.Substring(0, uName.IndexOf("("));
				}
			}
		}

		private void offlineMessageToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode == null) return;

			string groupID = (string) theNode.Tag;

			// 사용자 소속 조직원 목록 얻기
			string members = (groupID == "TEMP") ? dirSvc.GetTempUsers() : dirSvc.GetUsers(groupID);
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(getMemberList(members));

				if (sb.Length > 0) sb.Append("|");
				sb.Append(getMemberListRecursive(theNode));

				string list = sb.ToString();

				string uName = theNode.Text;
				if (uName.IndexOf("(") >= 0) uName = uName.Substring(0, uName.IndexOf("("));

				OfflineMessage f = new OfflineMessage("GROUP:" + uName, list, null);
				wndArray.Add(f);
				f.StartPosition = FormStartPosition.CenterScreen;
				f.Show();
			}
			catch (Exception)
			{
			}
		}

		private string getMemberListRecursive(TreeNode node)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < node.Nodes.Count; i++)
			{
				if (node.Nodes[i].ImageIndex != 0) continue;

				string groupID = (string) node.Nodes[i].Tag;
				string members = (groupID == "TEMP") ? dirSvc.GetTempUsers() : dirSvc.GetUsers(groupID);

				if (sb.Length > 0) sb.Append("|");
				sb.Append(getMemberList(members));

				if (sb.Length > 0) sb.Append("|");
				sb.Append(getMemberListRecursive(node.Nodes[i]));
			}
			return sb.ToString();
		}

		private string getMemberList(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList xmlNodes = doc.SelectNodes("/group/user");
			if (xmlNodes == null) return "";

			StringBuilder sb = new StringBuilder();
			foreach (XmlNode node in xmlNodes)
			{
				string id = node.Attributes["id"].InnerText;
				string name = node.Attributes["name"].InnerText;
				string title = node.Attributes["title"].InnerText;
				if (title.Trim().Length == 0) title = "사원";

				if (sb.Length > 0) sb.Append("|");
				sb.Append(title).Append(" ").Append(name).Append("(").Append(id).Append(")");
			}
			return sb.ToString();
		}

		private void frmMain_Resize(object sender, EventArgs e)
		{
			/*
			if (resizeBegin)
			{
				this.Width = 270; // 폭은 고정
				this.Left = fixedLeft; // 윈도우 위치도 고정
			}
			*/
		}

		private void frmMain_ResizeBegin(object sender, EventArgs e)
		{
			/*
			resizeBegin = true;
			fixedLeft = this.Left;
			*/
		}

		private void frmMain_ResizeEnd(object sender, EventArgs e)
		{
			//resizeBegin = false;
			SettingsHelper settings = SettingsHelper.Current;
			settings.MainWindowSize = Width + "|" + Height;
			settings.Save();
		}

		private void frmMain_LocationChanged(object sender, EventArgs e)
		{
			if (!notifyIcon1.Visible)
			{
				SettingsHelper settings = SettingsHelper.Current;
				settings.MainWindowPosition = Left + "|" + Top;
				settings.Save();
			}
		}

		private void msgLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				frmChatHistory frm = new frmChatHistory();
				frm.Show();
#if DEBUG
				string logPath = SettingsHelper.Current.LogPath;
				if (string.IsNullOrEmpty(logPath))
				{
					logPath = Application.ExecutablePath;
					logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
				}
				logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "NETS-iMan_chat.Log";
				Process.Start("notepad.exe", logPath);
#endif
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("msgLogToolStripMenuItem_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "msgLogToolStripMenuItem_Click(): " + ex);
			}
		}

		private void frmMain_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) Close();
		}


		#region ----------Fade Behavior-------------

		void opacityTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				if (isFading || stopFading) return;

				bool inForm = this.Bounds.Contains(Cursor.Position);
				if (inForm)
				{
					if (!ContainsFocus) return;
					if (this.Opacity != 1.00)
					{
						isFading = true;
						abortFading = true;

						//Cancel running fading threads to avoid overflowing the stack
						while ((fadeInThread.ThreadState == System.Threading.ThreadState.Running) ||
							(fadeOutThread.ThreadState == System.Threading.ThreadState.Running))
						{
							Thread.Sleep(10);
							Application.DoEvents();
						}

						abortFading = false;

						//Begin fading in
						ThreadStart fadeInStart = fadeIn;
						fadeInThread = new Thread(fadeInStart);
						fadeInThread.Start();
					}
				}
				else
				{
					if (this.Opacity == 1.00)
					{
						isFading = true;
						abortFading = true;

						//Cancel running fading threads to avoid overflowing the stack
						while ((fadeInThread.ThreadState == System.Threading.ThreadState.Running) ||
							(fadeOutThread.ThreadState == System.Threading.ThreadState.Running))
						{
							Thread.Sleep(10);
							Application.DoEvents();
						}

						abortFading = false;

						//Begin fading out
						ThreadStart fadeOutStart = fadeOut;
						fadeOutThread = new Thread(fadeOutStart);
						fadeOutThread.Start();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.WARNING, "opacityTImer_Tick(): " + ex.Message);
			}
			isFading = false;
		}

		//Changing opacity will be a cross-thread operation, delegates are in order
		private delegate void ChangeOpacityDelegate(double value);
		private void changeOpacity(double value)
		{
			if (this.InvokeRequired)
			{
				ChangeOpacityDelegate del = changeOpacity;
				this.Invoke(del, value);
			}
			else
			{
				this.Opacity = value;
			}
		}

		private void fadeIn()
		{
			try
			{
				for (double i = this.Opacity; i <= 1; i += FADE_RATE)
				{
					if (abortFading) return;

					changeOpacity(i);
					Application.DoEvents();
				}

				if (this.Opacity != 1.00)
					changeOpacity(1.00);
			}
			catch (Exception)
			{
			}
		}

		private void fadeOut()
		{
			try
			{
				for (double i = this.Opacity; i >= FORM_OPACITY; i -= FADE_RATE)
				{
					if (abortFading) return;

					changeOpacity(i);
					Application.DoEvents();
				}

				if (this.Opacity != FORM_OPACITY)
					changeOpacity(FORM_OPACITY);
			}
			catch (Exception)
			{
			}
		}

		#endregion

		#region Nested type: TreeCollapseCallback

		private delegate void TreeCollapseCallback(TreeNode node);

		#endregion

		#region Nested type: TreeExpandeCallback

		private delegate void TreeExpandeCallback(TreeNode node);

		#endregion

		#region Nested type: TreeUpdateCallback

		private delegate TreeNode TreeUpdateCallback(TreeNode node, string text, Color textColor, bool doChildClear);

		#endregion
	}

	public class GroupItem
	{
		private readonly ArrayList m_userList = ArrayList.Synchronized(new ArrayList());
		private string m_id;
		private int m_num;

		public GroupItem(string id)
		{
			m_id = id;
		}

		public string GroupID
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public int GroupNo
		{
			get { return m_num; }
			set { m_num = value; }
		}

		public ArrayList UserList
		{
			get { return m_userList; }
		}
	}
}
