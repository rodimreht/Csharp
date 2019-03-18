using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using oBrowser2.Properties;
using Timer=System.Windows.Forms.Timer;

namespace oBrowser2
{
	public partial class frmMain : Form
	{
		private bool firstLoading = false;
		private bool doExit = false;

		private StickyWindow sticky;

		private ExtendedWebBrowser _browser;
		private ResourceCollector _collector;
		private Timer tickTimer = null;
		private int dontShowAgain = 3;

		delegate TreeNode TreeUpdateCallback(TreeNode node, string text, Color textColor, bool doChildClear);
		delegate void TreeExpandeCallback(TreeNode node);
		delegate void TreeCollapseCallback(TreeNode node);

		private string _uniName = "";
		private Uri loopURL;
		private string loopCookie;

		// SMS 송신 플래그
		private bool isSentSMS = false;
		// SMTP 송신 플래그
		private bool isSentSMTP = false;
		private int sendCount = 0;

		// 최근 HTML
		private string currentHTML = "";

		// 이동함대 자원
		private int fleetMetal = 0;
		private int fleetCrystal = 0;
		private int fleetDuterium = 0;

		// 자동 갱신 시각
		private DateTime autoRefreshTime = DateTime.MinValue;

		private frmEventAlarm alarmForm = null;
		private SortedList<DateTime, string> m_alarmList = new SortedList<DateTime, string>();
		private Timer alarmTimer = null;

		private readonly DateTime m_daily = DateTime.Parse("2000-01-01");

		public frmMain()
		{
			InitializeComponent();

			Logger.Log("oBrowser 시작");
			firstLoading = true;

			sticky = new StickyWindow(this);
			sticky.StickOnMove = true;
			sticky.StickOnResize = false;
			sticky.StickToOther = false;
			sticky.StickToScreen = true;

			_browser = new ExtendedWebBrowser();
			_browser.Dock = DockStyle.Fill;
			_browser.DownloadComplete += new EventHandler(_browser_DownloadComplete);
			_browser.Navigated += new WebBrowserNavigatedEventHandler(_browser_Navigated);
			_browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_browser_DocumentCompleted);
			_browser.PreviewKeyDown += new PreviewKeyDownEventHandler(_browser_PreviewKeyDown);
			this.containerPanel.Controls.Add(_browser);
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			this.Size = new Size(270, 500);

			SettingsHelper settings = SettingsHelper.Current;
			if (settings.ShowInLeftBottom)
				this.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);
			else
				this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width,
					Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);

			loginToolStripMenuItem.Enabled = true;
			loginRetryToolStripMenuItem.Enabled = false;
			refreshToolStripMenuItem.Enabled = false;
			resRefreshToolStripMenuItem.Enabled = false;
			openbrowserToolStripMenuItem.Enabled = false;
			alarmToolStripMenuItem.Enabled = false;
			expeditionToolStripMenuItem.Enabled = false;
			expeditionToolStripMenuItem2.Enabled = false;
			resMoveAllToolStripMenuItem.Enabled = false;
			resMoveToolStripMenuItem.Enabled = false;
			resMoveToolStripMenuItem2.Enabled = false;
			fleetSavingToolStripMenuItem.Enabled = false;

			// 버전이 변경되었는지 확인하여 이전 버전의 설정을 읽어온다.
			OB2Util.LoadPrevSettings();

			toolTip.SetToolTip(pictureBox1, "웹 브라우저로 O-Game 열기");

			// 타이머 시작
			tickTimer = new Timer();
			tickTimer.Tick += new EventHandler(tickTimer_Tick);

			// 이벤트 알람 타이머 시작
			alarmTimer = new Timer();
			alarmTimer.Tick += new EventHandler(alarmTimer_Tick);
			alarmTimer.Interval = 60 * 1000;
			alarmTimer.Start();
		}

		private void frmMain_Activated(object sender, EventArgs e)
		{
			if (firstLoading)
			{
				firstLoading = false;
				SettingsHelper settings = SettingsHelper.Current;
#if INTERNAL_USE
				if (settings.SMSphoneNum.Length == 0)
					optionsToolStripMenuItem_Click(null, null);
				else if (settings.UseFireFox && (settings.FireFoxDirectory.Length == 0))
					optionsToolStripMenuItem_Click(null, null);
				else if (settings.SmtpMail.Length == 0)
					optionsToolStripMenuItem_Click(null, null);
#else
				if (settings.UseFireFox && (settings.FireFoxDirectory.Length == 0))
					optionsToolStripMenuItem_Click(null, null);
				else if (settings.SmtpMail.Length == 0)
					optionsToolStripMenuItem_Click(null, null);
#endif

				loginToolStripMenuItem_Click(null, null);
			}
		}

		private void pictureBox1_DoubleClick(object sender, EventArgs e)
		{
			openBrowser();
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool topMost = this.TopMost;
			this.TopMost = false;

			using (OptionsForm of = new OptionsForm())
			{
				of.ShowDialog(this);
			}

			this.TopMost = topMost;
		}

		private void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool topMost = this.TopMost;
			this.TopMost = false;

			using (LoginForm loginForm = new LoginForm())
			{
				if (loginForm.ShowDialog(this) == DialogResult.OK)
				{
					_uniName = loginForm.UniverseName;
					this.Text = "oBrowser2: " + _uniName;
					notifyIcon1.Text = this.Text;

					_browser.Navigate(loginForm.URL);
					loginToolStripMenuItem.Enabled = false;
					loginRetryToolStripMenuItem.Enabled = true;
					refreshToolStripMenuItem.Enabled = true;
					resRefreshToolStripMenuItem.Enabled = true;
					openbrowserToolStripMenuItem.Enabled = true;
				}
			}

			this.TopMost = topMost;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			doExit = true;
			Logger.Log("oBrowser 종료");
			Logger.Close();
			this.Close();
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode dnode = treeUpdate("!!! 경고 !!! ", true);
			treeView.Nodes.Remove(dnode);
			dnode = treeUpdate("알림", true);
			treeView.Nodes.Remove(dnode);

			if (!processOverview(WebCall.GetHtml(loopURL, loopCookie)))
			{
				TreeNode node = treeUpdate("현재시각");
				treeUpdate(node, "(페이지 에러발생!!)", Color.Red);
				Application.DoEvents();
			}
		}

		private void resRefreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode dnode = treeUpdate("!!! 경고 !!! ", true);
			treeView.Nodes.Remove(dnode);
			dnode = treeUpdate("알림", true);
			treeView.Nodes.Remove(dnode);

			updateResource();
		}

		private void openbrowserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			openBrowser();
		}

		private void openBrowser()
		{
			if (loopURL != null)
			{
				string url = loopURL.ToString();
				SettingsHelper settings = SettingsHelper.Current;
				if (settings.UseFireFox && settings.FireFoxDirectory.Length > 0)
				{
					if (!FirefoxControl.setFFSession(loopCookie))
					{
						// 파이어폭스 Modify Headers Add-on없음!!
						MessageBoxEx.Show("다음 파이어폭스 Add-on 기능이 설치되어 있지 않습니다.\r\n\r\n" +
										  "- Modify Headers v0.6.4 버전 이상\r\n\r\n" +
										  "Add-on을 설치하신 후에 정상 로그인됩니다.",
										  "oBrowser2: 파이어폭스 오류",
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  10 * 1000);
					}
					ProcessStartInfo psi = new ProcessStartInfo(settings.FireFoxDirectory, url);
					Process.Start(psi);
				}
				else
				{
					ProcessStartInfo psi = new ProcessStartInfo(url);
					Process.Start(psi);
				}
			}
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
			bool topMost = this.TopMost;
			this.TopMost = false;

			using (AboutForm af = new AboutForm())
			{
				af.ShowDialog(this);
			}

			this.TopMost = topMost;
		}

		private void treeView_MouseDown(object sender, MouseEventArgs e)
		{
			treeView.SelectedNode = treeView.GetNodeAt(e.X, e.Y);
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

			this.Show();
			this.WindowState = FormWindowState.Normal;

			if (this.Size.Width != 270 && this.Size.Height != 500)
			{
				// 크기 및 위치를 재조정한다.
				this.Size = new Size(270, 500);

				SettingsHelper settings = SettingsHelper.Current;
				if (settings.ShowInLeftBottom)
					this.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);
				else
					this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width,
					                          Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);
			}
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!doExit)
			{
				// 이미 창이 최소화된 상태에서 종료신호를 받으면 정상 종료한다.
				if (notifyIcon1.Visible) return;

				notifyIcon1.Visible = true;
				e.Cancel = true;
				Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
				this.WindowState = FormWindowState.Minimized;
				Thread.Sleep(500);
				this.Hide();
			}
		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			doExit = true;
			Logger.Log("oBrowser 종료");
			Logger.Close();
			Application.Exit();
		}

		void _browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.Control && _browser.Focused) _browser.Parent.Focus();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			_browser.Stop();
			if (tickTimer.Enabled) tickTimer.Stop();
			tickTimer.Enabled = false;
			_collector = null;
			loopURL = null;

			loginToolStripMenuItem.Enabled = true;
			loginRetryToolStripMenuItem.Enabled = false;
			refreshToolStripMenuItem.Enabled = false;
			resRefreshToolStripMenuItem.Enabled = false;
			openbrowserToolStripMenuItem.Enabled = false;
		}

		// 초 단위로 갱신 주기를 얻는다.
		private static int getRefreshRate()
		{
			SettingsHelper settings = SettingsHelper.Current;
			int min = settings.RefreshRate;
			int max = settings.RefreshRateMax;

			// 새벽 1시 ~ 7시 사이에는 갱신 주기를 4배로 늘인다. (최대 15분 ~ 60분)
			if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 7)
			{
				min *= 4;
				if (min > 15) min = 15;
				max *= 4;
				if (max > 60) max = 60;
			}

			Random r = new Random();
			int newVal = r.Next(min * 60, max * 60);
			return newVal;
		}

		private void _browser_DownloadComplete(object sender, EventArgs e)
		{
			// Check wheter the document is available (it should be)
			if ((_browser.Document != null) && (_browser.Document.Window != null))
			{
				// Subscribe to the Error event
				_browser.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
			}
		}

		private void Window_Error(object sender, HtmlElementErrorEventArgs e)
		{
			// We got a script error, record it
			Logger.Log(e.Url + " : " + e.Description + " (줄 번호: " + e.LineNumber + ")");
			// Let the browser know we handled this error.
			e.Handled = true;
		}

		private void _browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if ((((ExtendedWebBrowser)sender).Document.Window != null))
			{
				if (e.Url.AbsoluteUri.ToLower().IndexOf("/index.php?page=overview&session=") >= 0)
				{
					if (loopURL == null) loopURL = e.Url;

					string sHtml = ((ExtendedWebBrowser)sender).Document.Window.Document.Body.OuterHtml;
					loopCookie = ((ExtendedWebBrowser)sender).Document.Window.Document.Cookie;
					if (!processOverview(sHtml))
					{
						DialogResult dr = MessageBoxEx.Show("오게임 접속에 실패했습니다. 웹 브라우저로 확인하시겠습니까?",
										  "oBrowser2: 접속 실패",
										  MessageBoxButtons.YesNo,
										  MessageBoxIcon.Information,
										  MessageBoxDefaultButton.Button2,
										  50 * 1000);

						if (dr == DialogResult.Yes)
						{
							if (loopURL != null)
							{
								string url = loopURL.ToString();
								ProcessStartInfo psi = new ProcessStartInfo(url);
								Process.Start(psi);
							}
						}
						Stop();
					}
				}
				else if ((e.Url.AbsoluteUri.ToLower().IndexOf("/game/reg/errorpage.php") >= 0) ||
						 (e.Url.OriginalString.IndexOf("%2Fgame%2Freg%2Ferrorpage.php") >= 0))
				{
					// 페이지 요청 중 에러가 발생해 에러 페이지로 간 경우 다시 페이지 요청
					if (dontShowAgain > 0)
					{
						dontShowAgain--;
						if (loopURL != null)
						{
							TreeNode node = treeUpdate("현재시각");
							treeUpdate(node, "(페이지 에러발생!!)", Color.Red);
							Logger.Log("ERROR: 실행 중 오류 발생 - " + e.Url);

							_browser.Navigate(loopURL);
						}
					}
					else
						Stop();
				}
			}
		}

		private bool processOverview(string sHtml)
		{
			// 메인 페이지가 맞는지 다시 확인
			int pMain = sHtml.ToLower().IndexOf("행성 매뉴");
			if (pMain >= 0)
			{
				currentHTML = sHtml;
				string addMsg = "";

				if (sHtml.ToLower().IndexOf("flight attack") >= 0)	// 공격당하는 중
				{
					int attPos = sHtml.ToLower().IndexOf("flight attack");
					int attPos2 = sHtml.IndexOf("\n", attPos + 1);
					string val = sHtml.Substring(attPos, attPos2 - attPos);
					string hash = OB2Security.Hash(val);
					if (string.IsNullOrEmpty(SettingsHelper.Current.AttackHash) ||
						(SettingsHelper.Current.AttackHash != hash))
					{
						// SMS 송신
						if (!isSentSMS)
						{
							addMsg = DateTime.Now.ToString("HH:mm") + " 공격함대 발견!!";
							SendSMS.Send();
							isSentSMS = true;
						}

						// SMTP 송신
						if (!isSentSMTP)
						{
							addMsg = DateTime.Now.ToString("HH:mm") + " 공격함대 발견!!";
							SendSMTP.Send(hash, val);
							isSentSMTP = true;
						}
					}

					// 메시지창은 3회에 걸쳐 3번만 출력한다.
					if (sendCount < 3)
					{
						this.TopMost = true;
						showMainWindow();

						Logger.Log("WARNING: O-Game 침입 경고 - 공격당하고 있습니다!!");

						SoundPlayer.PlaySound(Application.StartupPath + @"\malfound.wav");
						MessageBoxEx.Show("공격당하고 있습니다!!",
										  "oBrowser2: 침입 경고",
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  50 * 1000);
						sendCount++;
					}
				}
				else
				{
					isSentSMS = false;
					isSentSMTP = false;
					if (this.TopMost) this.TopMost = false;
				}

				string s1 = "flight ownattack";
				string s2 = "return own";
				string s3 = "flight ownespionage";
				string s4 = "flight ownharvest";
				string s5 = "flight owntransport";
				string s6 = "flight owndeploy";
				string s7 = "flight owncolony";
				string s8 = "holding owntransport";

				int aCount = 0, rCount = 0, eCount = 0, hCount = 0, tCount = 0, dCount = 0, cCount = 0, t2Count = 0;
				string rank = "";
				if (sHtml.ToLower().IndexOf("flight espionage") >= 0)	// 정탐당하는 중
				{
					Logger.Log("WARNING: O-Game 침입 경고 - 정찰위성 발견!!");
					addMsg = DateTime.Now.ToString("HH:mm") + " 정찰위성 발견!!";
				}

				calcCount(sHtml, s1, ref aCount); // 공격하는 중
				calcCount(sHtml, s2, ref rCount); // 귀환하는 중
				calcCount(sHtml, s3, ref eCount); // 정탐하는 중
				calcCount(sHtml, s4, ref hCount); // 수확하는 중
				calcCount(sHtml, s5, ref tCount); // 운송하는 중
				calcCountRev(sHtml, s5, ref tCount); // 운송함대에서 원정함대를 찾아 빼준다.

				// 배치는 귀환이 없다
				calcCount(sHtml, s6, ref dCount); // 배치하는 중
				calcCount(sHtml, s7, ref cCount); // 식민하는 중
				calcCount(sHtml, s8, ref t2Count);// 원정하는 중

				// 귀환은 전체 귀환 함대 수에서 배치를 제외한 다른 함대 수만큼 빼준다
				rCount -= aCount + tCount + hCount + eCount + cCount + t2Count;

				if (sHtml.IndexOf("점수") >= 0) // 점수 표시
				{
					int pos = sHtml.IndexOf("점수");
					string sTemp = "=3>";
					pos = sHtml.IndexOf(sTemp, pos + 1);
					int pos2 = sHtml.IndexOf("<", pos + 1);
					rank = sHtml.Substring(pos + sTemp.Length, pos2 - pos - sTemp.Length);

					sTemp = ">";
					pos = sHtml.IndexOf(sTemp, pos2);
					pos2 = sHtml.IndexOf("</", pos + 1);
					rank += sHtml.Substring(pos + sTemp.Length, pos2 - pos - sTemp.Length);

					sTemp = ">";
					pos = sHtml.IndexOf(sTemp, pos2);
					pos2 = sHtml.IndexOf("</", pos + 1);
					rank += sHtml.Substring(pos + sTemp.Length, pos2 - pos - sTemp.Length);
				}

				TreeNode node = treeUpdate("현재시각");
				if (SettingsHelper.Current.ApplySummerTime)
					treeUpdate(node, "독일: " + DateTime.Now.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"));
				else
					treeUpdate(node, "독일: " + DateTime.Now.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"));
				treeUpdate(node, "한국: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				treeExpand(node);

				if (!tickTimer.Enabled)
				{
					int nextInterval = getRefreshRate();
					Logger.Log("페이지 읽음(다음 갱신 시각: " + nextInterval + "초 후)");
					tickTimer.Interval = nextInterval * 1000;
					autoRefreshTime = DateTime.Now.AddSeconds(nextInterval);
				}

				node = treeUpdate("다음 자동 갱신시각 ");
				if (SettingsHelper.Current.ApplySummerTime)
					treeUpdate(node, "독일: " + autoRefreshTime.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"));
				else
					treeUpdate(node, "독일: " + autoRefreshTime.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"));
				treeUpdate(node, "한국: " + autoRefreshTime.ToString("yyyy-MM-dd HH:mm:ss"));
				treeExpand(node);

				node = treeUpdate("점수");
				treeUpdate(node, rank);
				treeExpand(node);

				node = treeUpdate("함대현황");

				// 함대 이동 메뉴 연결
				if (node != null) node.ContextMenuStrip = popupMenuStrip3;

				treeUpdate(node, "공격함대: " + aCount + "함대", Color.FromArgb(0xBD, 0xB1, 0x3A));
				treeUpdate(node, "운송함대: " + tCount + "함대", Color.FromArgb(0x00, 0xDF, 0x00));
				treeUpdate(node, "배치함대: " + dCount + "함대", Color.FromArgb(0x5E, 0xC9, 0x60));
				treeUpdate(node, "수확함대: " + hCount + "함대", Color.FromArgb(0x2E, 0x8F, 0xDA));
				treeUpdate(node, "정탐함대: " + eCount + "함대", Color.FromArgb(0xDC, 0x6E, 0x00));
				treeUpdate(node, "식민함대: " + cCount + "함대", Color.FromArgb(0x60, 0x60, 0xDF));
				treeUpdate(node, "원정함대: " + t2Count + "함대", Color.FromArgb(0xCF, 0x6E, 0xFA));
				treeUpdate(node, "귀환함대: " + rCount + "함대");
				treeUpdate(node, "----- 총 이동함대: " + (aCount + tCount + dCount + hCount + eCount + cCount + rCount + t2Count) + "함대");
				treeExpand(node);

				// 자원 수집기를 초기화한다.
				if (_collector == null)
				{
					_collector = new ResourceCollector();
					_collector.CollectCompleted += new EventHandler<EventArgs>(_collector_CollectCompleted);
					_collector.PartialCollectCompleted += new EventHandler<ColonyEventArgs>(_collector_PartialCollectCompleted);
				}

				// 최초 1회 자원수집은 자동으로 한다.
				// 이후 자원수집은 트리메뉴에서 수동으로 진행한다.
				if (!tickTimer.Enabled)
				{
					tickTimer.Start();

					node = treeUpdate("자원현황");
					if (node != null) node.ContextMenuStrip = null;

					getResource();
					_collector.StartCollect(loopURL, sHtml, loopCookie);
				}

				if (addMsg.Length > 0)
				{
					node = treeUpdate("!!! 경고 !!! ", Color.FromArgb(0xFF, 0x00, 0x00));
					treeUpdate(node, addMsg);
					treeExpand(node);

					this.TopMost = false;
					showMainWindow();
				}

				if (sHtml.IndexOf("새 메시지") >= 0) // 새 메시지 도착
				{
					DialogResult dr = MessageBoxEx.Show("확인하지 않은 새 메시지가 있습니다. 확인하시겠습니까?",
									  "oBrowser2: 새 메시지 도착",
									  MessageBoxButtons.YesNo,
									  MessageBoxIcon.Information,
									  MessageBoxDefaultButton.Button2,
									  50 * 1000,
									  MessageBoxEx.Position.BottomRight);

					if (dr == DialogResult.Yes)
					{
						if (loopURL != null)
						{
							string url = loopURL.ToString();

							// 메시지창으로 변경 표시
							url = url.Replace("=overview", "=messages&dsp=1");
							
							SettingsHelper settings = SettingsHelper.Current;
							if (settings.UseFireFox && settings.FireFoxDirectory.Length > 0)
							{
								if (!FirefoxControl.setFFSession(loopCookie))
								{
									// 파이어폭스 Modify Headers Add-on없음!!
									MessageBoxEx.Show("다음 파이어폭스 Add-on 기능이 설치되어 있지 않습니다.\r\n\r\n" +
													  "- Modify Headers v0.6.4 버전 이상\r\n\r\n" +
													  "Add-on을 설치하신 후에 정상 로그인됩니다.",
													  "oBrowser2: 파이어폭스 오류",
													  MessageBoxButtons.OK,
													  MessageBoxIcon.Exclamation,
													  10 * 1000);
								}
								ProcessStartInfo psi = new ProcessStartInfo(settings.FireFoxDirectory, url);
								Process.Start(psi);
							}
							else
							{
								ProcessStartInfo psi = new ProcessStartInfo(url);
								Process.Start(psi);
							}
						}
					}
				}

				return true;
			}
			else
			{
				Logger.Log("ERROR: 페이지 읽기 에러 - " + loopURL);
				return false;
			}
		}

		static void calcCount(string str1, string str2, ref int count)
		{
			if (str1.IndexOf(str2) < 0) return;
			int pos = str1.IndexOf(str2);
			while (pos >= 0)
			{
				count++;
				pos = str1.IndexOf(str2, pos + str2.Length);
			}
		}

		// 원정함대는 flight owntransport 미션으로 나가므로... 보정한다.
		static void calcCountRev(string str1, string str2, ref int count)
		{
			if (str1.IndexOf(str2) < 0) return;
			int pos = str1.IndexOf(str2);
			while (pos >= 0)
			{
				int pos2 = str1.IndexOf("\n", pos + str2.Length);
				string sTemp = str1.Substring(pos, pos2 - pos);
				if (sTemp.IndexOf("임무 : 원정") >= 0)
					count--;

				pos = str1.IndexOf(str2, pos + str2.Length);
			}
		}

		void _collector_PartialCollectCompleted(object sender, ColonyEventArgs e)
		{
			// 기존 목록을 지우지 않고 유지한다.
			TreeNode node = treeUpdate("자원현황", false);
			if (node != null) node.ContextMenuStrip = null;

			ResourceInfo[] infos = _collector.ResourceInfos;
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i] != null) && infos[i].ColonyName.Equals(e.ColonyName))
				{
					TreeNode child = treeUpdate(node, infos[i].ColonyName + " [" + infos[i].Location + "]");
					treeUpdate(child, "필 드 수: " + infos[i].FieldsDeveloped);
					treeUpdate(child, "메    탈: " + infos[i].ResourceList["M"], Color.DarkBlue);
					treeUpdate(child, "크리스탈: " + infos[i].ResourceList["C"], Color.DeepPink);
					treeUpdate(child, "듀 테 륨: " + infos[i].ResourceList["D"], Color.DarkViolet);
					treeExpand(child);
				}
			}
			treeExpand(node);
			Logger.Log("자원 수집 완료: " + e.ColonyName);
		}

		void _collector_CollectCompleted(object sender, EventArgs e)
		{
			int[] mTotal = new int[10] { 0, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			int[] cTotal = new int[10] { 0, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			int[] dTotal = new int[10] { 0, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

			// 목록을 지우고 새로 게시한다.
			TreeNode node = treeUpdate("자원현황");
			if (node != null) node.ContextMenuStrip = popupMenuStrip2;
			TreeNode resTime = treeUpdate(node, "기준시각");
			if (SettingsHelper.Current.ApplySummerTime)
				treeUpdate(resTime, "독일: " + DateTime.Now.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"));
			else
				treeUpdate(resTime, "독일: " + DateTime.Now.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"));
			treeUpdate(resTime, "한국: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			treeExpand(resTime);

			// 함대현황에 이동 자원량 표시
			if ((fleetMetal + fleetCrystal + fleetDuterium) > 0)
			{
				mTotal[0] += fleetMetal;
				cTotal[0] += fleetCrystal;
				dTotal[0] += fleetDuterium;

				TreeNode fleet = treeUpdate(node, "함대 이동자원");
				treeUpdate(fleet, "메    탈: " + fleetMetal.ToString("###,###,##0"), Color.DarkBlue);
				treeUpdate(fleet, "크리스탈: " + fleetCrystal.ToString("###,###,##0"), Color.DeepPink);
				treeUpdate(fleet, "듀 테 륨: " + fleetDuterium.ToString("###,###,##0"), Color.DarkViolet);
				treeCollapse(fleet);
			}

			ResourceInfo[] infos = _collector.ResourceInfos;
			for (int i = 0; i < infos.Length; i++)
			{
				if (infos[i] != null)
				{
					int galaxy = int.Parse(infos[i].Location.Substring(0, 1));
					TreeNode child = treeUpdate(node, galaxy + "은하");

					// 함대 이동 메뉴 연결
					if (child != null) child.ContextMenuStrip = popupMenuStrip3;

					if (mTotal[galaxy] == -1) mTotal[galaxy] = 0;
					if (cTotal[galaxy] == -1) cTotal[galaxy] = 0;
					if (dTotal[galaxy] == -1) dTotal[galaxy] = 0;

					TreeNode child2 = treeUpdate(child, infos[i].ColonyName + " [" + infos[i].Location + "]");

					// 함대 이동 메뉴 연결
					if (child2 != null)
					{
						child2.Tag = infos[i].Location;
						child2.ContextMenuStrip = popupMenuStrip3;
					}

					if (infos[i].ResourceList["M"] == null)
					{
						treeUpdate(child2, "(타임아웃! 자원수집 실패)");
						treeExpand(child2);
					}
					else
					{
						mTotal[galaxy] += int.Parse(infos[i].ResourceList["M"].Replace(",", "").Replace("(*)", ""));
						cTotal[galaxy] += int.Parse(infos[i].ResourceList["C"].Replace(",", "").Replace("(*)", ""));
						dTotal[galaxy] += int.Parse(infos[i].ResourceList["D"].Replace(",", "").Replace("(*)", ""));
						mTotal[0] += int.Parse(infos[i].ResourceList["M"].Replace(",", "").Replace("(*)", ""));
						cTotal[0] += int.Parse(infos[i].ResourceList["C"].Replace(",", "").Replace("(*)", ""));
						dTotal[0] += int.Parse(infos[i].ResourceList["D"].Replace(",", "").Replace("(*)", ""));

						treeUpdate(child2, "필 드 수: " + infos[i].FieldsDeveloped);
						treeUpdate(child2, "메    탈: " + infos[i].ResourceList["M"], Color.DarkBlue);
						treeUpdate(child2, "크리스탈: " + infos[i].ResourceList["C"], Color.DeepPink);
						treeUpdate(child2, "듀 테 륨: " + infos[i].ResourceList["D"], Color.DarkViolet);
						treeExpand(child2);
					}
					treeCollapse(child);
				}
			}

			for (int i = 1; i <= 9; i++)
			{
				if (mTotal[i] != -1)
				{
					TreeNode child = treeUpdate(node, i.ToString() + "은하");
					TreeNode childSum = treeUpdate(child, "은하합계");
					treeUpdate(childSum, "메    탈: " + mTotal[i].ToString("###,###,##0"), Color.DarkBlue);
					treeUpdate(childSum, "크리스탈: " + cTotal[i].ToString("###,###,##0"), Color.DeepPink);
					treeUpdate(childSum, "듀 테 륨: " + dTotal[i].ToString("###,###,##0"), Color.DarkViolet);
					treeCollapse(childSum);
				}
			}

			TreeNode summary = treeUpdate(node, "전체합계(함대포함)");
			treeUpdate(summary, "메    탈: " + mTotal[0].ToString("###,###,##0"), Color.DarkBlue);
			treeUpdate(summary, "크리스탈: " + cTotal[0].ToString("###,###,##0"), Color.DeepPink);
			treeUpdate(summary, "듀 테 륨: " + dTotal[0].ToString("###,###,##0"), Color.DarkViolet);
			treeCollapse(summary);

			treeExpand(node);

			alarmToolStripMenuItem.Enabled = true;
			expeditionToolStripMenuItem.Enabled = true;
			expeditionToolStripMenuItem2.Enabled = true;
			resMoveAllToolStripMenuItem.Enabled = true;
			resMoveToolStripMenuItem.Enabled = true;
			resMoveToolStripMenuItem2.Enabled = true;
			fleetSavingToolStripMenuItem.Enabled = true;
		}

		TreeNode treeUpdate(string text)
		{
			return treeUpdate(text, Color.Black);
		}

		TreeNode treeUpdate(string text, bool doChildClear)
		{
			return treeUpdate(null, text, doChildClear);
		}

		TreeNode treeUpdate(string text, Color textColor)
		{
			return treeUpdate(null, text, textColor, true);
		}

		TreeNode treeUpdate(TreeNode node, string text)
		{
			return treeUpdate(node, text, false);
		}

		TreeNode treeUpdate(TreeNode node, string text, bool doChildClear)
		{
			return treeUpdate(node, text, Color.Black, doChildClear);
		}

		TreeNode treeUpdate(TreeNode node, string text, Color textColor)
		{
			return treeUpdate(node, text, textColor, false);
		}

		TreeNode treeUpdate(TreeNode node, string text, Color textColor, bool doChildClear)
		{
			if (this.treeView.InvokeRequired)
			{
				TreeUpdateCallback callback = new TreeUpdateCallback(treeUpdate);
				return (TreeNode)this.Invoke(callback, new object[] { node, text, textColor, doChildClear });
			}
			else
			{
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
					newNode.ImageIndex = 0;
					newNode.SelectedImageIndex = 1;
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
					return newNode;
				}
			}
		}

		void treeExpand(TreeNode node)
		{
			if (this.treeView.InvokeRequired)
			{
				TreeExpandeCallback callback = new TreeExpandeCallback(treeExpand);
				this.Invoke(callback, new object[] { node });
			}
			else
			{
				if (node != null) node.Expand();
			}
		}

		void treeCollapse(TreeNode node)
		{
			if (this.treeView.InvokeRequired)
			{
				TreeCollapseCallback callback = new TreeCollapseCallback(treeCollapse);
				this.Invoke(callback, new object[] { node });
			}
			else
			{
				if (node != null) node.Collapse(true);
			}
		}

		void tickTimer_Tick(object sender, EventArgs e)
		{
			if ((_browser != null) && (loopURL != null))
			{
				SettingsHelper settings = SettingsHelper.Current;

				int errCount = 0;
				while (!processOverview(WebCall.GetHtml(loopURL, loopCookie)))
				{
					TreeNode node = treeUpdate("현재시각");
					treeUpdate(node, "(페이지 에러발생!!)", Color.Red);
					Application.DoEvents();
					Thread.Sleep(500);
					errCount++;

					if (errCount >= 5)
					{
						showMainWindow();

						if ((settings.UserID != null) && (settings.UserID.Trim().Length > 0) &&
							(settings.Password != null) && (settings.Password.Trim().Length > 0))
						{
							string encPwd;
							if (OB2Security.IsStrongKey(settings.UserID))
							{
								string pwd = settings.Password.Replace("enc:", "");
								encPwd = OB2Security.Decrypt(settings.UserID, pwd);
							}
							else
							{
								string key = settings.UserID + "12345678";
								string pwd = settings.Password.Replace("enc:", "");
								encPwd = OB2Security.Decrypt(key, pwd);
							}

							Logger.Log("자동 로그인 시도: userid=" + settings.UserID);
							Uri uri = new Uri("http://" + loopURL.Host + "/game/reg/login2.php?v=2" +
											  "&login=" + settings.UserID + "&pass=" + encPwd);
							Stop();
							Thread.Sleep(500);
							_browser.Navigate(uri);
							loginToolStripMenuItem.Enabled = false;
							loginRetryToolStripMenuItem.Enabled = true;
							refreshToolStripMenuItem.Enabled = true;
							resRefreshToolStripMenuItem.Enabled = true;
							openbrowserToolStripMenuItem.Enabled = true;
						}
						else
						{
							Logger.Log("ERROR: 페이지 읽기 에러 - 페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.");
							MessageBoxEx.Show("페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.\r\n다시 접속해보시기 바랍니다.",
											  "oBrowser2: 장애 발생",
							                  MessageBoxButtons.OK,
							                  MessageBoxIcon.Exclamation,
							                  50 * 1000);
							Stop();
						}
						return;
					}
				}
				int nextInterval = getRefreshRate();
				Logger.Log("페이지 갱신됨(다음 갱신 시각: " + nextInterval + "초 후)");
				tickTimer.Interval = nextInterval*1000;
				autoRefreshTime = DateTime.Now.AddSeconds(nextInterval);

				TreeNode node2 = treeUpdate("다음 자동 갱신시각 ");
				if (SettingsHelper.Current.ApplySummerTime)
					treeUpdate(node2, "독일: " + autoRefreshTime.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"));
				else
					treeUpdate(node2, "독일: " + autoRefreshTime.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"));
				treeUpdate(node2, "한국: " + autoRefreshTime.ToString("yyyy-MM-dd HH:mm:ss"));
				treeExpand(node2);
			}
			else
			{
				tickTimer.Stop();
			}
		}

		void alarmTimer_Tick(object sender, EventArgs e)
		{
			DateTime key1 = DateTime.Today.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
			DateTime key2 = m_daily.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);

			// 최대 10초간 쉰다: 일정한 시각에 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 200);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			if ((m_alarmList != null) && (m_alarmList.Count > 0))
			{
				for (int i = 0; i < m_alarmList.Count; i++)
				{
					DateTime key = m_alarmList.Keys[i];
					if (key.Equals(key1) || key.Equals(key2)) // 매일 같은 시각
					{
						this.TopMost = true;
						showMainWindow();

						string val = m_alarmList.Values[i];
						if (val.StartsWith("[R]"))	// 예약
						{
							val = val.Substring(3);
							string[] vals = val.Split(new char[] { '^' });
							string sVal = vals[1];
							if (sVal.Length > 0)
							{
								if (sVal.Length == 2) // = 2: 원정(새 버전) 횟수
								{
									treeView.SelectedNode = treeView.TopNode;

									int iTmp = int.Parse(sVal.Replace("E", "")) + 1;
									for (int cnt = 0; cnt < iTmp; cnt++)
									{
										goExpedition(false);

										// 최대 3초간 쉰 후 다시 보낸다.
										num = r.Next(0, 60);
										for (int kk = 0; kk < num; kk++)
										{
											Thread.Sleep(50);
											Application.DoEvents();
										}
									}

									SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
									MessageBoxEx.Show("원정 함대(총 " + iTmp + "함대)가 출발했습니다.",
													  "oBrowser2: 원정",
													  MessageBoxButtons.OK,
													  MessageBoxIcon.Information,
													  3 * 1000);
								}
								else if (sVal.Length > 1)	// > 1: 행성 좌표(플릿 세이빙)
								{
									string coords = sVal;
									int planetType = 1;
									if (coords.IndexOf("(달)") > 0)
									{
										coords = coords.Replace("(달)", "").Trim();
										planetType = 3;
									}
									goFleetsaving(coords, planetType);
								}
								else // = 1: 은하 인덱스(자원 모으기)
									goResourceMoving(sVal);
							}
							else	// 원정(기존 버전)
							{
								treeView.SelectedNode = treeView.TopNode;

								goExpedition(true);
							}
						}
						else
						{
							string dd = key1.ToString("HH:mm");

							TreeNode node = treeUpdate("알림", false);
							TreeNode child = treeUpdate(node, "이벤트", false);
							treeUpdate(child, "[" + dd + "] " + val, Color.FromArgb(0x00, 0x00, 0xFF));
							treeExpand(child);
							treeExpand(node);

							SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
							MessageBoxEx.Show("설정된 이벤트 시각이 되었습니다: [" + dd + "] " + val,
											  "oBrowser2: 이벤트 알람",
											  MessageBoxButtons.OK,
											  MessageBoxIcon.Question,
											  10 * 1000);
						}

						this.TopMost = false;
					}
				}
			}
		}

		private void getResource()
		{
			fleetMetal = 0;
			fleetCrystal = 0;
			fleetDuterium = 0;
			try
			{
				int pos = currentHTML.IndexOf("\"운송: ");
				while (pos > 0)
				{
					int p1 = currentHTML.IndexOf("메탈: ", pos + 1);
					int p2 = currentHTML.IndexOf("크리스탈: ", p1 + 1);
					int p3 = currentHTML.IndexOf("듀테륨: ", p2 + 1);
					int pEnd = currentHTML.IndexOf("\"", p3 + 1);

					fleetMetal += int.Parse(currentHTML.Substring(p1 + 4, p2 - p1 - 4).Replace("&lt;br /&gt;", "").Replace(".", "").Trim());
					fleetCrystal += int.Parse(currentHTML.Substring(p2 + 6, p3 - p2 - 6).Replace("&lt;br /&gt;", "").Replace(".", "").Trim());
					fleetDuterium += int.Parse(currentHTML.Substring(p3 + 5, pEnd - p3 - 5).Replace("&lt;br /&gt;", "").Replace(".", "").Trim());

					pos = currentHTML.IndexOf("\"운송: ", pos + 1);
				}

				pos = currentHTML.IndexOf("'운송: ");
				while (pos > 0)
				{
					int p1 = currentHTML.IndexOf("메탈: ", pos + 1);
					int p2 = currentHTML.IndexOf("크리스탈: ", p1 + 1);
					int p3 = currentHTML.IndexOf("듀테륨: ", p2 + 1);
					int pEnd = currentHTML.IndexOf("'", p3 + 1);

					fleetMetal += int.Parse(currentHTML.Substring(p1 + 4, p2 - p1 - 4).Replace("&lt;br /&gt;", "").Replace(".", "").Trim());
					fleetCrystal += int.Parse(currentHTML.Substring(p2 + 6, p3 - p2 - 6).Replace("&lt;br /&gt;", "").Replace(".", "").Trim());
					fleetDuterium += int.Parse(currentHTML.Substring(p3 + 5, pEnd - p3 - 5).Replace("&lt;br /&gt;", "").Replace(".", "").Trim());

					pos = currentHTML.IndexOf("'운송: ", pos + 1);
				}
			}
			catch (Exception e)
			{
				Logger.Log("getResource()-예외 발생: " + e);
			}
		}

		private void _browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			// Check wheter the document is available (it should be)
			if ((_browser.Document != null) && (_browser.Document.Window != null))
			{
				// Subscribe to the Error event
				_browser.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
			}
		}

		private void updateMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode dnode = treeUpdate("!!! 경고 !!! ", true);
			treeView.Nodes.Remove(dnode);
			dnode = treeUpdate("알림", true);
			treeView.Nodes.Remove(dnode);

			updateResource();
		}

		private void updateResource()
		{
			TreeNode node = treeUpdate("자원현황");
			if (node != null) node.ContextMenuStrip = null;

			getResource();
			_collector.StartCollect(loopURL, currentHTML, loopCookie);
		}

		private void loginRetryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool topMost = this.TopMost;
			this.TopMost = false;

			using (LoginForm loginForm = new LoginForm())
			{
				if (loginForm.ShowDialog(this) == DialogResult.OK)
				{
					Stop();

					_uniName = loginForm.UniverseName;
					this.Text = "oBrowser2: " + _uniName;
					notifyIcon1.Text = this.Text;

					_browser.Navigate(loginForm.URL);
					loginToolStripMenuItem.Enabled = false;
					loginRetryToolStripMenuItem.Enabled = true;
					refreshToolStripMenuItem.Enabled = true;
					resRefreshToolStripMenuItem.Enabled = true;
					openbrowserToolStripMenuItem.Enabled = true;
				}
			}

			this.TopMost = topMost;
		}

		private void alarmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 이벤트 설정을 할 수 없는 상태입니다.",
								  "oBrowser2: 이벤트 설정 장애",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			loadAlarmSettings();

			bool topMost = this.TopMost;
			this.TopMost = false;

			alarmForm = (m_alarmList == null) ? new frmEventAlarm() : new frmEventAlarm(m_alarmList);
			alarmForm.ResourceInfos = _collector.ResourceInfos;
			alarmForm.ShowDialog(this);
			lock (m_alarmList)
			{
				m_alarmList = alarmForm.AlarmList;
			}
			alarmForm.Dispose();
			alarmForm = null;

			saveAlarmSettings();

			this.TopMost = topMost;
		}

		private void saveAlarmSettings()
		{
			if ((m_alarmList != null) && (m_alarmList.Count > 0))
			{
				OB2Util.SaveAlarmSettings(m_alarmList);
			}
		}

		private void loadAlarmSettings()
		{
			lock (m_alarmList)
			{
				m_alarmList = OB2Util.LoadAlarmSettings();
			}
		}

		private void openbrowserToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			openBrowser();
		}

		private void expeditionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정 설정을 할 수 없는 상태입니다.",
								  "oBrowser2: 원정 설정 장애",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			bool topMost = this.TopMost;
			this.TopMost = false;

			frmMission missionForm = new frmMission();
			missionForm.NaviURL = loopURL;
			missionForm.ContextCookies = loopCookie;
			missionForm.ResourceInfos = _collector.ResourceInfos;
			missionForm.ShowDialog(this);

			this.TopMost = topMost;
		}

		private void resMoveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 모으기 설정을 할 수 없는 상태입니다.",
								  "oBrowser2: 자원 모으기 설정 장애",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			bool topMost = this.TopMost;
			this.TopMost = false;

			frmResCollect resMoveForm = new frmResCollect();
			resMoveForm.NaviURL = loopURL;
			resMoveForm.ContextCookies = loopCookie;
			resMoveForm.ResourceInfos = _collector.ResourceInfos;
			resMoveForm.ShowDialog(this);

			this.TopMost = topMost;
		}

		private void expeditionToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			goExpedition(true);
		}

		private void goExpedition(bool showMsg)
		{
			frmMission missionForm = new frmMission();
			missionForm.NaviURL = loopURL;
			missionForm.ContextCookies = loopCookie;
			missionForm.ResourceInfos = _collector.ResourceInfos;

			TreeNode theNode = treeView.SelectedNode;
			if (theNode.Tag != null)
			{
				missionForm.FromLocation = (string)theNode.Tag;
				missionForm.PlanetType = (theNode.Text.IndexOf("(달)") > 0) ? 3 : 1;
			}

			if (alarmForm != null)
			{
				lock (m_alarmList)
				{
					m_alarmList = alarmForm.AlarmList;
				}
				saveAlarmSettings();
			}

			if (missionForm.GoMission())
			{
				string dd = DateTime.Now.ToString("HH:mm");

				TreeNode node = treeUpdate("알림", false);
				TreeNode child = treeUpdate(node, "원정", false);
				treeUpdate(child, "[" + dd + "] 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
				treeExpand(child);
				treeExpand(node);

				if (showMsg)
				{
					SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
					MessageBoxEx.Show("원정 함대가 출발했습니다.",
									  "oBrowser2: 원정",
					                  MessageBoxButtons.OK,
					                  MessageBoxIcon.Information,
					                  3*1000);
				}
			}

			if (alarmForm != null)
			{
				loadAlarmSettings();
				alarmForm.AlarmList = m_alarmList;
				alarmForm.RefreshList();
			}
		}

		private void resMoveToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			frmResCollect resMoveForm = new frmResCollect();
			resMoveForm.NaviURL = loopURL;
			resMoveForm.ContextCookies = loopCookie;
			resMoveForm.ResourceInfos = _collector.ResourceInfos;

			TreeNode theNode = treeView.SelectedNode;
			if (theNode.Tag != null)
			{
				resMoveForm.FromLocation = (string)theNode.Tag;
				resMoveForm.PlanetType = (theNode.Text.IndexOf("(달)") > 0) ? 3 : 1;
			}
			
			if (resMoveForm.MoveResource())
			{
				string dd = DateTime.Now.ToString("HH:mm");

				TreeNode node = treeUpdate("알림", false);
				TreeNode child = treeUpdate(node, "자원운송", false);
				treeUpdate(child, "[" + dd + "] " + (string)theNode.Tag + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
				treeExpand(child);
				treeExpand(node);

				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("운송 함대가 출발했습니다.",
								  "oBrowser2: 자원 운송",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Information,
								  3 * 1000);
			}
		}

		private void popupMenuStrip3_Opening(object sender, CancelEventArgs e)
		{
			string text = treeView.SelectedNode.Text;
			if (text.IndexOf("함대현황") >= 0)
			{
				expeditionToolStripMenuItem2.Text = "원정함대 출발";
				expeditionToolStripMenuItem2.ToolTipText = "원정 옵션에서 설정된 내용대로 즉시 원정을 보냅니다.";
				expeditionToolStripMenuItem2.Enabled = true;
				resMoveAllToolStripMenuItem.Enabled = false;
				resMoveToolStripMenuItem2.Enabled = false;
				fleetSavingToolStripMenuItem.Enabled = false;
			}
			else if (text.IndexOf("은하") >= 0)
			{
				expeditionToolStripMenuItem2.Enabled = false;
				resMoveAllToolStripMenuItem.Enabled = true;
				resMoveToolStripMenuItem2.Enabled = false;
				fleetSavingToolStripMenuItem.Enabled = false;
			}
			else
			{
				expeditionToolStripMenuItem2.Text = "이 행성에서 원정함대 출발";
				expeditionToolStripMenuItem2.ToolTipText = "원정 옵션에서 설정된 함대 구성으로 이 행성에서 즉시 원정을 보냅니다.";
				expeditionToolStripMenuItem2.Enabled = true;
				resMoveAllToolStripMenuItem.Enabled = false;
				resMoveToolStripMenuItem2.Enabled = true;
				fleetSavingToolStripMenuItem.Enabled = true;
			}
		}

		private void flSavSettingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙 설정을 할 수 없는 상태입니다.",
								  "oBrowser2: 플릿 세이빙 설정 장애",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			bool topMost = this.TopMost;
			this.TopMost = false;

			frmFleetSaving resMoveForm = new frmFleetSaving();
			resMoveForm.NaviURL = loopURL;
			resMoveForm.ContextCookies = loopCookie;
			resMoveForm.ResourceInfos = _collector.ResourceInfos;
			resMoveForm.ShowDialog(this);

			this.TopMost = topMost;
		}

		private void fleetSavingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			if (theNode.Tag != null)
			{
				string coords = (string)theNode.Tag;
				int planetType = (theNode.Text.IndexOf("(달)") > 0) ? 3 : 1;
				goFleetsaving(coords, planetType);
			}
		}

		private void goFleetsaving(string coords, int planetType)
		{
			frmFleetSaving flSavForm = new frmFleetSaving();
			flSavForm.NaviURL = loopURL;
			flSavForm.ContextCookies = loopCookie;
			flSavForm.ResourceInfos = _collector.ResourceInfos;
			flSavForm.FromLocation = coords;
			flSavForm.PlanetType = planetType;

			if (flSavForm.FleetSaving())
			{
				string dd = DateTime.Now.ToString("HH:mm");

				TreeNode node = treeUpdate("알림", false);
				TreeNode child = treeUpdate(node, "플릿세이빙", false);
				treeUpdate(child, "[" + dd + "] " + coords + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
				treeExpand(child);
				treeExpand(node);

				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("해당 행성의 모든 함대와 자원을 안전하게 이동시켰습니다.",
								  "oBrowser2: 플릿 세이빙!!",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Information,
								  3 * 1000);
			}
		}

		private void resMoveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode theNode = treeView.SelectedNode;
			string galaxy = theNode.Text.Substring(0, 1);
			goResourceMoving(galaxy);
		}

		private void goResourceMoving(string galaxy)
		{
			int count = 0;
			int successCount = 0;

			frmResCollect resMoveForm = new frmResCollect();
			resMoveForm.NaviURL = loopURL;
			resMoveForm.ContextCookies = loopCookie;
			resMoveForm.ResourceInfos = _collector.ResourceInfos;
			resMoveForm.GalaxyMethod = true;

			foreach (ResourceInfo info in _collector.ResourceInfos)
			{
				try
				{
					if ((info == null) || string.IsNullOrEmpty(info.Location)) continue;

					if ((galaxy == "0") || info.Location.StartsWith(galaxy))
					{
						count++;

						resMoveForm.FromLocation = info.Location;
						resMoveForm.PlanetType = (info.ColonyName.IndexOf("(달)") > 0) ? 3 : 1;

						if (alarmForm != null)
						{
							lock (m_alarmList)
							{
								m_alarmList = alarmForm.AlarmList;
							}
							saveAlarmSettings();
						}

						if (resMoveForm.MoveResource())
						{
							string dd = DateTime.Now.ToString("HH:mm");

							TreeNode node = treeUpdate("알림", false);
							TreeNode child = treeUpdate(node, "자원운송", false);
							treeUpdate(child, "[" + dd + "] " + info.Location + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
							treeExpand(child);
							treeExpand(node);

							successCount++;
						}

						if (alarmForm != null)
						{
							loadAlarmSettings();
							alarmForm.AlarmList = m_alarmList;
							alarmForm.RefreshList();
						}

						// 최대 5초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
						Random r = new Random();
						int num = r.Next(0, 100);
						for (int i = 0; i < num; i++)
						{
							Thread.Sleep(50);
							Application.DoEvents();
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Log("ERROR: " + ex);
				}
			}

			if (count > 0 && count == successCount)
			{
				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("해당 은하의 모든 행성에서 운송 함대가 출발했습니다.",
								  "oBrowser2: 자원 운송",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Information,
								  3 * 1000);
			}
		}
	}
}
