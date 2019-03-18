﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using oBrowser2.oBStatSvc;
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
		private delegate void ToolStripCallback(bool value);

		private string _uniName = "";
		private string _loginUrl = "";
		private Uri loopURL;
		private string loopCookie;

		// SMS 송신 플래그
		private bool isSentSMS = false;
		// SMTP 송신 플래그
		private bool isSentSMTP = false;
		private int sendCount = 0;

		// 최근 HTML
		private string m_currentHTML = "";
		private string m_currentEventHTML = "";

		// 이동함대 자원
		private int m_fleetMetal = 0;
		private int m_fleetCrystal = 0;
		private int m_fleetDeuterium = 0;

		// 자동 갱신 시각
		private DateTime autoRefreshTime = DateTime.MinValue;

		private frmEventAlarm alarmForm;
		private frmOption optionForm;
		private SortedList<DateTime, string> m_alarmList;
		private Timer alarmTimer;
		private DateTime triggerTime = DateTime.MinValue;

		// 이벤트 알림: "매일" 기준일자
		private readonly DateTime m_daily = DateTime.Parse("2000-01-01");

		// 환경설정 저장 주기 카운트: 60분 마다 저장
		private int settingsCount = 0;

		// 플릿세이빙 예약 체크
		private bool flSavAfterResCollection = false;

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

			alarmForm = null;
			optionForm = null;
			m_alarmList = new SortedList<DateTime, string>();
			alarmTimer = null;
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

			expeditionToolStripMenuItem.Visible = false;
			expeditionToolStripMenuItem.Enabled = false;
			expeditionToolStripMenuItem2.Enabled = false;

			resMoveAllToolStripMenuItem.Enabled = false;
			resMoveToolStripMenuItem.Visible = false;
			resMoveToolStripMenuItem.Enabled = false;
			resMoveToolStripMenuItem2.Enabled = false;

			flSavSettingToolStripMenuItem.Visible = false;
			flSavSettingToolStripMenuItem.Enabled = false;
			fleetSavingToolStripMenuItem.Enabled = false;

			fleetMoveToolStripMenuItem.Visible = false;
			fleetMoveToolStripMenuItem.Enabled = false;
			fleetMove2ToolStripMenuItem.Enabled = false;

#if TEST
			testToolStripMenuItem.Visible = true;
#else
			testToolStripMenuItem.Visible = false;
#endif
			toolTip.SetToolTip(pictureBox1, "웹 브라우저로 O-Game 열기");

			// 버전이 변경되었는지 확인하여 이전 버전의 설정을 읽어온다.
			OB2Util.LoadPrevSettings();

			// 최초 이벤트 알림 및 예약 설정을 읽어온다.
			loadAlarmSettings();

			// 타이머 시작
			tickTimer = new Timer();
			tickTimer.Tick += new EventHandler(tickTimer_Tick);

			// 이벤트 알람 타이머 시작
			alarmTimer = new Timer();
			alarmTimer.Tick += new EventHandler(alarmTimer_Tick);
			alarmTimer.Interval = 10 * 1000;
			alarmTimer.Start();
		}

		private void frmMain_Activated(object sender, EventArgs e)
		{
			if (firstLoading)
			{
				firstLoading = false;
				SettingsHelper settings = SettingsHelper.Current;

				if (string.IsNullOrEmpty(settings.OGameDomain))
					optionsToolStripMenuItem_Click(null, null);
				else if (settings.UseFireFox && (settings.FireFoxDirectory.Length == 0))
					optionsToolStripMenuItem_Click(null, null);
				else if (settings.SmtpMail.Length == 0)
					optionsToolStripMenuItem_Click(null, null);

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

			if (optionForm == null) optionForm = new frmOption(); //OptionsForm of = new OptionsForm();
			if (_collector != null)
			{
				optionForm.NaviURL = loopURL;
				optionForm.ContextCookies = loopCookie;
				optionForm.ResourceInfos = _collector.ResourceInfos;
				optionForm.UniverseName = _uniName;
			}
			optionForm.ShowDialog();
			if (optionForm.ContextCookies != loopCookie) loopCookie = optionForm.ContextCookies;

			this.TopMost = topMost;
		}

		private void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool topMost = this.TopMost;
			this.TopMost = false;

			using (LoginForm loginForm = new LoginForm())
			{
				if (loginForm.ShowDialog() == DialogResult.OK)
				{
					_uniName = loginForm.UniverseName;
					_loginUrl = loginForm.URL.AbsoluteUri;

#if AUTOFS
					const string title = "oBrowser2+";
#else
					const string title = "oBrowser2";
#endif
					this.Text = title + ": " + _uniName;
					notifyIcon1.Text = this.Text;
					//Logger.Log("[로그인 시도] " + loginForm.URL.OriginalString);

					_browser.Navigate(_loginUrl);
					loginToolStripMenuItem.Enabled = false;
					loginRetryToolStripMenuItem.Enabled = true;
					refreshToolStripMenuItem.Enabled = true;
					resRefreshToolStripMenuItem.Enabled = true;
					openbrowserToolStripMenuItem.Enabled = true;

					try
					{
						// 통계정보 전송
						oBStatService svc = new oBStatService();
						svc.Timeout = 5000;
						if (string.IsNullOrEmpty(loginForm.LoginID))
							svc.StatLog(_uniName, "SESSION USE", title + " v." + Application.ProductVersion);
						else
							svc.StatLog(_uniName, LoginForm.GetOriginalID(loginForm.LoginID), title + " v." + Application.ProductVersion);
					}
					catch (Exception)
					{
					}
				}
			}

			this.TopMost = topMost;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			doExit = true;
			Logger.Log("oBrowser 종료");
			Logger.Close();
			SettingsHelper.Current.Save();
			if (alarmForm != null)
			{
				alarmForm.DoClose = true;
				alarmForm.Close();
				alarmForm.Dispose();
			}
			if (optionForm != null)
			{
				optionForm.DoClose = true;
				optionForm.Close();
				optionForm.Dispose();
			}
			Application.Exit();
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode dnode = treeUpdate("!!! 경고 !!! ", true);
			treeView.Nodes.Remove(dnode);
			dnode = treeUpdate("알림", true);
			treeView.Nodes.Remove(dnode);

			string preCookie = string.Copy(loopCookie);
			string sHtml = WebCall.GetHtml(loopURL, ref loopCookie);
			if (loopCookie != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + loopCookie);

			if (!processOverview(sHtml))
			{
				if (sHtml.Length > 1000)
					Logger.Log(sHtml.Substring(0, 1000).Replace("\r\n", "\n"));
				else
					Logger.Log(sHtml.Replace("\r\n", "\n"));

				if (sHtml.IndexOf("Please log in again") > 0)
				{
					loginRetry();
					return;
				}

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
					string uniNum = _uniName.Replace("우주", "").Split(':')[1];

					if (!FirefoxControl.setFFSession(uniNum, settings.FireFoxDirectory, loopCookie))
					{
						// ModifyHeaders 기능 사용 못함 2009-12-14: for Redesign
						// 파이어폭스 Modify Headers Add-on없음!!
						/*
						MessageBoxEx.Show("다음 파이어폭스 Add-on 기능이 설치되어 있지 않습니다.\r\n\r\n" +
										  "- Modify Headers v0.6.4 버전 이상\r\n\r\n" +
										  "Add-on을 설치하신 후에 정상 로그인됩니다.",
										  "파이어폭스 오류 - oBrowser2: " + _uniName,
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  10 * 1000);
						*/
						MessageBoxEx.Show("파이어폭스를 실행할 수 없습니다.",
										  "파이어폭스 오류 - oBrowser2: " + _uniName,
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Exclamation,
										  10 * 1000);
						return;
					}
					
					//Clipboard.SetText(url);

					ProcessStartInfo psi = new ProcessStartInfo(settings.FireFoxDirectory, _loginUrl);
					Process.Start(psi);
				}
				else
				{
					ProcessStartInfo psi = new ProcessStartInfo(_loginUrl);
					Process.Start(psi);
				}

				minimizeForm();
				if (tickTimer.Enabled) tickTimer.Stop();
				tickTimer.Enabled = false;
				if (alarmTimer.Enabled) alarmTimer.Stop();
				alarmTimer.Enabled = false;

				MessageBoxEx.Show("웹 브라우저 대기중...\r\n(웹 브라우저 종료 후 아래 '확인' 단추를 누르세요.)",
				                  "oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
								  MessageBoxIcon.None,
								  uint.MaxValue,
								  MessageBoxEx.Position.BottomRight);

				alarmTimer.Start();
				loginRetry();
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
				e.Cancel = true;
				minimizeForm();
			}
		}

		private void minimizeForm()
		{
			// 이미 창이 최소화된 상태에서 종료신호를 받으면 정상 종료한다.
			if (notifyIcon1.Visible) return;

			if (_uniName.Length > 0)
				notifyIcon1.Icon = new Icon(getIconResourceStream());
			else
				notifyIcon1.Icon = this.Icon;

			notifyIcon1.Visible = true;
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
			this.WindowState = FormWindowState.Minimized;
			Thread.Sleep(500);
			this.Hide();
		}

		private Stream getIconResourceStream()
		{
			string uniName = _uniName.Replace("우주", "");
			string uniNum = "0";
			SettingsHelper settings = SettingsHelper.Current;
			for (int i = 0; i < settings.OGameUniNames.Length; i++)
			{
				if (uniName.Equals(settings.OGameUniNames[i]))
				{
					uniNum = (i + 1).ToString();
					break;
				}
			}
			string resourceName = "oBrowser2.og" + uniNum + ".ico";
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			doExit = true;
			Logger.Log("oBrowser 종료");
			Logger.Close();
			SettingsHelper.Current.Save();
			if (alarmForm != null)
			{
				alarmForm.DoClose = true;
				alarmForm.Close();
				alarmForm.Dispose();
			}
			if (optionForm != null)
			{
				optionForm.DoClose = true;
				optionForm.Close();
				optionForm.Dispose();
			}
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

			// 새벽 1시 ~ 7시 사이에는 갱신 주기를 4배로 늘인다. (최소 15분 ~ 60분)
			if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 7)
			{
				min *= 4;
				if (min < 15) min = 15;
				max *= 4;
				if (max < 60) max = 60;
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
						Logger.Log("ERROR: 오게임 접속에 실패했습니다.");
						DialogResult dr = MessageBoxEx.Show("오게임 접속에 실패했습니다. 웹 브라우저로 확인하시겠습니까?",
										  "접속 실패 - oBrowser2: " + _uniName,
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

					// 페이지를 읽은 후에는 브라우저를 중지시킨다.
					_browser.Stop();
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

							_browser.Navigate(loopURL.AbsoluteUri);
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
			int pMain = sHtml.IndexOf("Overview -");
			if (pMain >= 0)
			{
				m_currentHTML = sHtml;

				string rank = "";

				if (sHtml.IndexOf("Points:") >= 0) // 점수 표시
				{
					int pos = sHtml.IndexOf("Points:");
					string sTemp = "a href=";
					pos = sHtml.IndexOf(sTemp, pos + 1);
					sTemp = ">";
					pos = sHtml.IndexOf(sTemp, pos + 1);
					int pos2 = sHtml.IndexOf(" (", pos + 1);
					rank = sHtml.Substring(pos + 1, pos2 - pos - 1);

					sTemp = "Place ";
					pos = sHtml.IndexOf(sTemp, pos2);
					pos2 = sHtml.IndexOf(" of ", pos + 1);
					rank += " (순위: " + sHtml.Substring(pos + sTemp.Length, pos2 - pos - sTemp.Length);

					pos = pos2 + 4;
					pos2 = sHtml.IndexOf(")", pos + 1);
					rank += "/" + sHtml.Substring(pos, pos2 - pos) + ")";
				}

				TreeNode node = treeUpdate("현재시각");
				if (SettingsHelper.Current.ApplySummerTime)
					treeUpdate(node, "독일: " + DateTime.Now.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
				else
					treeUpdate(node, "독일: " + DateTime.Now.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
				treeUpdate(node, "한국: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x33CC33));
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
					treeUpdate(node, "독일: " + autoRefreshTime.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
				else
					treeUpdate(node, "독일: " + autoRefreshTime.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
				treeUpdate(node, "한국: " + autoRefreshTime.ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x33CC33));
				treeExpand(node);

				node = treeUpdate("점수");
				treeUpdate(node, rank);
				treeExpand(node);


				//********** 함대 이벤트 처리 **********/
				node = treeUpdate("함대현황");

				// 함대 이동 메뉴 연결
				if (node != null) node.ContextMenuStrip = popupMenuStrip3;

				// 이벤트 목록창으로 변경 표시
				string eventUrl = loopURL.ToString();
				eventUrl = eventUrl.Replace("=overview", "=eventList&pro=1");

				string preCookie = string.Copy(loopCookie);
				string eventHtml = WebCall.GetHtml(new Uri(eventUrl), ref loopCookie).ToLower();
				if (loopCookie != preCookie)
					Logger.Log("쿠키변경: " + preCookie + " --> " + loopCookie);
                
                m_currentEventHTML = eventHtml;
				
				string addMsg = "";

				if (checkEnemyMission(eventHtml, "<span>attack</span>"))	// 공격당하는 중
				{
					addMsg = DateTime.Now.ToString("HH:mm") + " 공격함대 발견!!";

					// SMS, SMTP 송신 및 자동 플릿 세이빙 기능
					if (_collector == null)
						flSavAfterResCollection = true;
					else
						checkFleetSaving(eventHtml);

					// 메시지창은 3회에 걸쳐 3번만 출력한다.
					if (sendCount < 3)
					{
						this.TopMost = true;
						showMainWindow();

						Logger.Log("WARNING: O-Game 침입 경고 - 공격당하고 있습니다!!");

						SoundPlayer.PlaySound(Application.StartupPath + @"\malfound.wav");
						MessageBoxEx.Show("공격당하고 있습니다!!",
										  "침입 경고 - oBrowser2: " + _uniName,
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

					SettingsHelper helper = SettingsHelper.Current;
					if (!string.IsNullOrEmpty(helper.AttackHash))
					{
						helper.AttackHash = "";
						helper.Changed = true;
					}

					if (this.TopMost) this.TopMost = false;
				}

				if (checkEnemyMission(eventHtml, "<span>espionage</span>"))	// 정탐당하는 중
				{
					Logger.Log("WARNING: O-Game 침입 경고 - 정찰위성 발견!!");
					addMsg = DateTime.Now.ToString("HH:mm") + " 정찰위성 발견!!";
				}

				string s1 = "<span>attack</span>";
				string s2 = "(r)</span>";
				string s3 = "<span>espionage</span>";
				string s4 = "<span>harvest</span>";
				string s5 = "<span>transport</span>";
				string s6 = "<span>deployment</span>";
				string s7 = "<span>colonization</span>";
				string s8 = "<span>expedition</span>";
				string s9 = "<span>moon destruction</span>";
				string s10 = "<span>acs attack</span>";
				string s11 = "<span>acs defend</span>";

				int aCount = 0, rCount = 0, eCount = 0, hCount = 0, tCount = 0,
					dCount = 0, cCount = 0, t2Count = 0, mCount = 0, aaCount = 0, adCount = 0;

				calcCount(eventHtml, s1, ref aCount); // 공격하는 중
				calcCount(eventHtml, s2, ref rCount); // 귀환하는 중
				calcCount(eventHtml, s3, ref eCount); // 정탐하는 중
				calcCount(eventHtml, s4, ref hCount); // 수확하는 중
				calcCount(eventHtml, s5, ref tCount); // 운송하는 중

				// 배치는 귀환이 없다
				calcCount(eventHtml, s6, ref dCount); // 배치하는 중
				calcCount(eventHtml, s7, ref cCount); // 식민하는 중
				calcCount(eventHtml, s8, ref t2Count);// 원정하는 중
				calcCountFix(eventHtml, s8, ref t2Count);// 원정함대 보정

				calcCount(eventHtml, s9, ref mCount);// 달 파괴하는 중
				calcCount(eventHtml, s10, ref aaCount);// ACS 공격하는 중
				calcCount(eventHtml, s11, ref adCount);// ACS 방어하는 중

				// 귀환은 전체 귀환 함대 수에서 배치를 제외한 다른 함대 수만큼 빼준다
				rCount -= aCount + tCount + hCount + eCount + cCount + t2Count + mCount + aaCount + adCount;

				treeUpdate(node, "공   격: " + aCount + "함대", Color.FromArgb(0xBD, 0xB1, 0x3A));
				treeUpdate(node, "운   송: " + tCount + "함대", Color.FromArgb(0x00, 0xDF, 0x00));
				treeUpdate(node, "배   치: " + dCount + "함대", Color.FromArgb(0x5E, 0xC9, 0x60));
				treeUpdate(node, "수   확: " + hCount + "함대", Color.FromArgb(0x2E, 0x8F, 0xDA));
				treeUpdate(node, "정   탐: " + eCount + "함대", Color.FromArgb(0xDC, 0x6E, 0x00));
				treeUpdate(node, "식   민: " + cCount + "함대", Color.FromArgb(0x60, 0x60, 0xDF));
				treeUpdate(node, "원   정: " + t2Count + "함대", Color.FromArgb(0xCF, 0x6E, 0xFA));
				treeUpdate(node, "달 파괴: " + mCount + "함대", Color.FromArgb(0x3A, 0x3A, 0x3A));
				treeUpdate(node, "ACS공격: " + aaCount + "함대", Color.FromArgb(0x66, 0x66, 0x66));
				treeUpdate(node, "ACS방어: " + adCount + "함대", Color.FromArgb(0x9C, 0x9C, 0x9C));
				treeUpdate(node, "귀   환: " + rCount + "함대");
				treeUpdate(node, "----- 총 이동함대: " + (aCount + tCount + dCount + hCount + eCount + cCount + rCount + t2Count + mCount + aaCount + adCount) + "함대");
				treeExpand(node);
				//********** 함대 이벤트 처리 끝 **********/


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

				if ((sHtml.IndexOf("unread message(s)") > 0) && (sHtml.IndexOf("|0 unread message(s)") < 0)) // 새 메시지 도착
				{
					DialogResult dr = MessageBoxEx.Show("확인하지 않은 새 메시지가 있습니다. 확인하시겠습니까?",
									  "새 메시지 도착 - oBrowser2: " + _uniName,
									  MessageBoxButtons.YesNo,
									  MessageBoxIcon.Information,
									  MessageBoxDefaultButton.Button2,
									  50 * 1000,
									  MessageBoxEx.Position.BottomRight);

					if (dr == DialogResult.Yes)
					{
						if (loopURL != null)
						{
							SettingsHelper settings = SettingsHelper.Current;
							if (settings.UseFireFox && settings.FireFoxDirectory.Length > 0)
							{
								string uniNum = _uniName.Replace("우주", "").Split(':')[1];

								if (!FirefoxControl.setFFSession(uniNum, settings.FireFoxDirectory, loopCookie))
								{
									// ModifyHeaders 기능 사용 못함 2009-12-14: for Redesign
									// 파이어폭스 Modify Headers Add-on없음!!
									/*
									MessageBoxEx.Show("다음 파이어폭스 Add-on 기능이 설치되어 있지 않습니다.\r\n\r\n" +
													  "- Modify Headers v0.6.4 버전 이상\r\n\r\n" +
													  "Add-on을 설치하신 후에 정상 로그인됩니다.",
													  "파이어폭스 오류 - oBrowser2: " + _uniName,
													  MessageBoxButtons.OK,
													  MessageBoxIcon.Exclamation,
													  10 * 1000);
									*/
									MessageBoxEx.Show("파이어폭스를 실행할 수 없습니다.",
													  "파이어폭스 오류 - oBrowser2: " + _uniName,
													  MessageBoxButtons.OK,
													  MessageBoxIcon.Exclamation,
													  10 * 1000);
								}
								ProcessStartInfo psi = new ProcessStartInfo(settings.FireFoxDirectory, _loginUrl);
								Process.Start(psi);
							}
							else
							{
								ProcessStartInfo psi = new ProcessStartInfo(_loginUrl);
								Process.Start(psi);
							}

							minimizeForm();
							if (tickTimer.Enabled) tickTimer.Stop();
							tickTimer.Enabled = false;
							if (alarmTimer.Enabled) alarmTimer.Stop();
							alarmTimer.Enabled = false;

							MessageBoxEx.Show("웹 브라우저 대기중...\r\n(웹 브라우저 종료 후 아래 '확인' 단추를 누르세요.)",
											  "oBrowser2: " + _uniName,
											  MessageBoxButtons.OK,
											  MessageBoxIcon.None,
											  uint.MaxValue,
											  MessageBoxEx.Position.BottomRight);

							alarmTimer.Start();
							loginRetry();
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

		static bool checkEnemyMission(string str1, string str2)
		{
			int pos = str1.IndexOf(str2);
			if (pos < 0) return false;

			string strTemp = str1.Substring(0, pos);

			int posStart = strTemp.LastIndexOf("descfleet\">");
			if (posStart < 0)
			{
				posStart = strTemp.LastIndexOf("descfleet>");
				if (posStart < 0) return false;
				posStart += 10;
			}
			else
				posStart += 11;

			int posEnd = strTemp.IndexOf("</li>", posStart);
			if (posEnd < 0) return false;
			string enemyMission = strTemp.Substring(posStart, posEnd - posStart);
			if (enemyMission.Equals("enemy fleet")) return true;
			return false;
		}

		static void calcCount(string str1, string str2, ref int count)
		{
			int pos = str1.IndexOf(str2);
			while (pos >= 0)
			{
				string strTemp = str1.Substring(0, pos);

				int posStart = strTemp.LastIndexOf("descfleet\">");
				if (posStart < 0)
				{
					posStart = strTemp.LastIndexOf("descfleet>");
					if (posStart < 0) break;
					posStart += 10;
				}
				else
					posStart += 11;

				int posEnd = strTemp.IndexOf("</li>", posStart);
				if (posEnd < 0) break;
				string ownMission = strTemp.Substring(posStart, posEnd - posStart);
				if (ownMission.Equals("own fleet")) count++;
				pos = str1.IndexOf(str2, pos + str2.Length);
			}
		}

		// 원정함대는 두개씩 나가므로... 보정한다.
		// eventid --> eventid + 1 --> eventid + 2 (귀환) 임을 고려하여
		// eventid + 2가 있으면 원정함대 개수를 하나 줄여준다.
		static void calcCountFix(string str1, string str2, ref int count)
		{
			int pos = str1.IndexOf(str2);
			while (pos >= 0)
			{
				int pos2 = str1.IndexOf("eventid=", pos + str2.Length);
				int pos3 = str1.IndexOf("\"", pos2 + 1);
				if (pos3 < 0) break;

				string eventID = str1.Substring(pos2 + 8, pos3 - pos2 - 8);
				int nEventID;
				if (int.TryParse(eventID, out nEventID))
				{
					if (str1.IndexOf((nEventID + 2).ToString()) > 0) count--;
				}
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
				treeUpdate(resTime, "서버: " + DateTime.Now.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
			else
				treeUpdate(resTime, "서버: " + DateTime.Now.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
			treeUpdate(resTime, "한국: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x33CC33));
			treeExpand(resTime);

			// 함대현황에 이동 자원량 표시
			if ((m_fleetMetal + m_fleetCrystal + m_fleetDeuterium) > 0)
			{
				mTotal[0] += m_fleetMetal;
				cTotal[0] += m_fleetCrystal;
				dTotal[0] += m_fleetDeuterium;

				TreeNode fleet = treeUpdate(node, "함대 이동자원");
				treeUpdate(fleet, "메    탈: " + m_fleetMetal.ToString("###,###,##0"), Color.DarkBlue);
				treeUpdate(fleet, "크리스탈: " + m_fleetCrystal.ToString("###,###,##0"), Color.DeepPink);
				treeUpdate(fleet, "듀 테 륨: " + m_fleetDeuterium.ToString("###,###,##0"), Color.DarkViolet);
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

			invokeToolStripChanges(true);

			if (flSavAfterResCollection)
			{
				flSavAfterResCollection = false;
				checkFleetSaving(m_currentHTML);
			}
		}

		private void invokeToolStripChanges(bool value)
		{
			if (this.InvokeRequired)
			{
				ToolStripCallback callback = invokeToolStripChanges;
				this.Invoke(callback, value);
				return;
			}

			alarmToolStripMenuItem.Enabled = value;
			//expeditionToolStripMenuItem.Enabled = value;
			expeditionToolStripMenuItem2.Enabled = value;
			resMoveAllToolStripMenuItem.Enabled = value;
			//resMoveToolStripMenuItem.Enabled = value;
			resMoveToolStripMenuItem2.Enabled = value;
			//flSavSettingToolStripMenuItem.Enabled = value;
			fleetSavingToolStripMenuItem.Enabled = value;
			//fleetMoveToolStripMenuItem.Enabled = value;
			fleetMove2ToolStripMenuItem.Enabled = value;
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
				int errCount = 0;

				string preCookie = string.Copy(loopCookie);
				string sHtml = WebCall.GetHtml(loopURL, ref loopCookie);
				if (loopCookie != preCookie)
					Logger.Log("쿠키변경: " + preCookie + " --> " + loopCookie);
                
                while (!processOverview(sHtml))
				{
					if (sHtml.Length > 1000)
						Logger.Log(sHtml.Substring(0, 1000).Replace("\r\n", "\n"));
					else
						Logger.Log(sHtml.Replace("\r\n", "\n"));

					if (sHtml.IndexOf("Please log in again") > 0)
					{
						loginRetry();
						return;
					}

					TreeNode node = treeUpdate("현재시각");
					treeUpdate(node, "(페이지 에러발생!!)", Color.Red);
					Application.DoEvents();
					Thread.Sleep(500);
					errCount++;

					if (errCount >= 5)
					{
						loginRetry();
						return;
					}
				}
				int nextInterval = getRefreshRate();
				Logger.Log("페이지 갱신됨(다음 갱신 시각: " + nextInterval + "초 후)");
				tickTimer.Interval = nextInterval*1000;
				autoRefreshTime = DateTime.Now.AddSeconds(nextInterval);

				TreeNode node2 = treeUpdate("다음 자동 갱신시각 ");
				if (SettingsHelper.Current.ApplySummerTime)
					treeUpdate(node2, "독일: " + autoRefreshTime.AddHours(-7).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
				else
					treeUpdate(node2, "독일: " + autoRefreshTime.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x999900));
				treeUpdate(node2, "한국: " + autoRefreshTime.ToString("yyyy-MM-dd HH:mm:ss"), Color.FromArgb(0x33CC33));
				treeExpand(node2);
			}
			else
			{
				tickTimer.Stop();
				Logger.Log("자동 갱신 멈춤");
			}
		}

		void loginRetry()
		{
			showMainWindow();

			SettingsHelper settings = SettingsHelper.Current;
			string userID = settings.UserID;
			if ((userID != null) && (userID.Trim().Length > 0) &&
				(settings.Password != null) && (settings.Password.Trim().Length > 0))
			{
				string loginID;
				if (userID.IndexOf("|") >= 0) // 아이디에 접속우주 표시
				{
					string[] uids = userID.Split(new char[] { '|' });
					loginID = uids[1].Replace("&#7C;", "|");
				}
				else
					loginID = userID;

				string decPwd;
				if (OB2Security.IsStrongKey(loginID))
				{
					string pwd = settings.Password.Replace("enc:", "");
					decPwd = OB2Security.Decrypt(loginID, pwd);
				}
				else
				{
					string key = loginID + "12345678";
					string pwd = settings.Password.Replace("enc:", "");
					decPwd = OB2Security.Decrypt(key, pwd);
				}

#if AUTOFS
				const string title = "oBrowser2+";
#else
				const string title = "oBrowser2";
#endif
				this.Text = title + ": " + _uniName;

				Logger.Log("자동 재로그인 시도[" + _uniName + "]: userid=" + loginID);
				Uri uri = new Uri("http://" + loopURL.Host + "/game/reg/login2.php?v=2" +
								  "&login=" + loginID + "&pass=" + decPwd);
				Stop();
				Thread.Sleep(500);
				_browser.Navigate(uri.AbsoluteUri);
				loginToolStripMenuItem.Enabled = false;
				loginRetryToolStripMenuItem.Enabled = true;
				refreshToolStripMenuItem.Enabled = true;
				resRefreshToolStripMenuItem.Enabled = true;
				openbrowserToolStripMenuItem.Enabled = true;

				try
				{
					// 통계정보 전송
					oBStatService svc = new oBStatService();
					svc.Timeout = 5000;
					svc.StatLog(_uniName, LoginForm.GetOriginalID(loginID), "자동 재로그인, " + title + " v." + Application.ProductVersion);

				}
				catch (Exception)
				{
				}
			}
			else
			{
				Logger.Log("ERROR: 페이지 읽기 에러 - 페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.");
				MessageBoxEx.Show("페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.\r\n다시 접속해보시기 바랍니다.",
								  "장애 발생 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  50 * 1000);
				Stop();
			}
		}

		void alarmTimer_Tick(object sender, EventArgs e)
		{
			DateTime key1 = DateTime.Today.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
			DateTime key2 = m_daily.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);

			if (triggerTime == key1) return;
			triggerTime = key1;

			try
			{
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
								if (_uniName.Length == 0) return;
								if (_collector == null)
								{
									triggerTime = key1.AddMinutes(-1);
									loginRetry();
									return;
								}

								string preCookie = string.Copy(loopCookie);
								string sHtml = WebCall.GetHtml(loopURL, ref loopCookie);
								if (loopCookie != preCookie)
									Logger.Log("쿠키변경: " + preCookie + " --> " + loopCookie);

								if (!processOverview(sHtml))
								{
									if (sHtml.Length > 1000)
										Logger.Log(sHtml.Substring(0, 1000).Replace("\r\n", "\n"));
									else
										Logger.Log(sHtml.Replace("\r\n", "\n"));

									if (sHtml.IndexOf("Please log in again") > 0)
									{
										triggerTime = key1.AddMinutes(-1);
										loginRetry();
										return;
									}
								}

								// 최대 10초간 쉰다: 일정한 시각에 발생하는 이벤트 회피(봇 탐지 회피)
								Random r = new Random();
								int num = r.Next(0, 200);
								for (int k = 0; k < num; k++)
								{
									Thread.Sleep(50);
									Application.DoEvents();
								}

								string newVal = val.Substring(3);
								string[] vals = newVal.Split(new char[] { '^' });
								string sVal = vals[1];
								if (sVal.Length > 0)
								{
									if (sVal.Length == 2) // sVal.Length == 2: 원정(새 버전) 횟수 또는 함대 기동 횟수
									{
										treeView.SelectedNode = treeView.TopNode;

										if (sVal.StartsWith("E"))	// 원정
										{
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
											MessageBoxEx.Show("원정함대(총 " + iTmp + "함대)가 출발했습니다.",
															  "원정 - oBrowser2: " + _uniName,
															  MessageBoxButtons.OK,
															  MessageBoxIcon.Information,
															  3 * 1000);
										}
										else if (sVal.StartsWith("M"))	// 함대 기동
										{
											int iTmp = int.Parse(sVal.Replace("M", "")) + 1;
											for (int cnt = 0; cnt < iTmp; cnt++)
											{
												goFleetMoving(false);

												// 최대 10초간 쉰 후 다시 보낸다.
												num = r.Next(0, 200);
												for (int kk = 0; kk < num; kk++)
												{
													Thread.Sleep(50);
													Application.DoEvents();
												}
											}

											SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
											MessageBoxEx.Show("기동함대(총 " + iTmp + "함대)가 출발했습니다.",
															  "함대 기동 - oBrowser2: " + _uniName,
															  MessageBoxButtons.OK,
															  MessageBoxIcon.Information,
															  3 * 1000);
										}
									}
									else if (sVal.Length > 1)	// sVal.Length > 1: 행성 좌표(플릿 세이빙) 또는 함대 기동 또는 건설/연구
									{
										if (sVal.StartsWith("M"))	// 함대 기동
										{
											string[] ss = sVal.Split(new char[] { ':' });
											if (ss.Length != 2)
											{
												MessageBoxEx.Show("함대 기동을 할 수 없습니다.",
																  "함대 기동 - oBrowser2: " + _uniName,
																  MessageBoxButtons.OK,
																  MessageBoxIcon.Exclamation,
																  10 * 1000);
												return;
											}

											int iTmp = int.Parse(ss[0].Replace("M", "")) + 1;
											for (int cnt = 0; cnt < iTmp; cnt++)
											{
												goFleetMoving2(ss[1]);

												// 최대 10초간 쉰 후 다시 보낸다.
												num = r.Next(0, 200);
												for (int kk = 0; kk < num; kk++)
												{
													Thread.Sleep(50);
													Application.DoEvents();
												}
											}

											SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
											MessageBoxEx.Show("기동함대(총 " + iTmp + "함대)가 출발했습니다.",
															  "함대 기동 - oBrowser2: " + _uniName,
															  MessageBoxButtons.OK,
															  MessageBoxIcon.Information,
															  3 * 1000);
										}
										else if (sVal.StartsWith("B") || sVal.StartsWith("R"))	// 건설 or 연구
										{
											string[] ss = sVal.Split(new char[] { '-' });
											string action = ss[0].Substring(0, 2);
											string coords = ss[0].Substring(2);
											string brID = action.Equals("BC") ? null : ss[1];
											int planetType = 1;
											if (coords.IndexOf("(달)") > 0)
											{
												coords = coords.Replace("(달)", "").Trim();
												planetType = 3;
											}

											doPlanetJob(action, coords, planetType, brID);
										}
										else // 플릿 세이빙
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
									}
									else // sVal.Length == 1: 은하 인덱스(자원 모으기)
									{
										goResourceMoving(sVal);
									}
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
												  "이벤트 알림 - oBrowser2: " + _uniName,
												  MessageBoxButtons.OK,
												  MessageBoxIcon.Question,
												  10 * 1000);
							}

							this.TopMost = false;
						}
					}
				}

			}
			catch (Exception ex)
			{
				Logger.Log("ERROR: 타이머 동작 오류 - " + ex);
				triggerTime = key1.AddMinutes(-1);
				loginRetry();
			}

			// 환경설정 주기적으로 저장: 60분 간격
			if (++settingsCount >= 60)
			{
				SettingsHelper.Current.Save();
				settingsCount = 0;
			}
		}

		private void getResource()
		{
			m_fleetMetal = 0;
			m_fleetCrystal = 0;
			m_fleetDeuterium = 0;
			try
			{
				int pos = m_currentEventHTML.IndexOf("eventid=");
				while (pos > 0)
				{
					int pos2 = m_currentEventHTML.IndexOf("\"", pos + 1);
					if (pos2 < 0) break;
					string eventID = m_currentEventHTML.Substring(pos + 8, pos2 - pos - 8);

					string eventUrl = loopURL.ToString();
					eventUrl = eventUrl.Replace("=overview", "=eventListTooltip&ajax=1&eventID=" + eventID);

					int attPos1 = m_currentEventHTML.Substring(0, pos).LastIndexOf("<ul>");
					int attPos2 = m_currentEventHTML.IndexOf("</ul>", attPos1 + 1);
					if (attPos1 < 0 || attPos2 < 0) break;

					string s2 = m_currentEventHTML.Substring(0, pos);

					int posStart = s2.LastIndexOf("descfleet\">");
					if (posStart < 0) break;
					posStart += 11;
					int posEnd = s2.IndexOf("</li>", posStart);
					if (posEnd < 0) break;

					string ownMission = s2.Substring(posStart, posEnd - posStart);
					if (ownMission.Equals("own fleet"))
					{
						string sInfo = m_currentEventHTML.Substring(attPos1, attPos2 + 5 - attPos1); // <ul> ~ </ul>
						
						// 귀환하는 함대의 자원은 무시한다.
						// (예외: 원정의 경우는 귀환하는 함대의 자원만 계산한다.)
						if ((sInfo.IndexOf("<span>expedition (r)</span>") > 0) ||
							((sInfo.IndexOf("(r)</span>") < 0) && (sInfo.IndexOf("<span>expedition") < 0)))
						{
							string preCookie = string.Copy(loopCookie);
							string checkText = WebCall.GetHtml(new Uri(eventUrl), ref loopCookie).ToLower();
							if (loopCookie != preCookie)
								Logger.Log("쿠키변경: " + preCookie + " --> " + loopCookie);

							int p1 = checkText.IndexOf("metal:");
							if (p1 < 0) break;
							int p1start = checkText.IndexOf("value\">", p1 + 1) + 7;
							int p1end = checkText.IndexOf("<", p1start);
							int p2 = checkText.IndexOf("crystal:");
							int p2start = checkText.IndexOf("value\">", p2 + 1) + 7;
							int p2end = checkText.IndexOf("<", p2start);
							int p3 = checkText.IndexOf("deuterium:");
							int p3start = checkText.IndexOf("value\">", p3 + 1) + 7;
							int p3end = checkText.IndexOf("<", p3start);

							m_fleetMetal += int.Parse(checkText.Substring(p1start, p1end - p1start).Replace(".", "").Trim());
							m_fleetCrystal += int.Parse(checkText.Substring(p2start, p2end - p2start).Replace(".", "").Trim());
							m_fleetDeuterium += int.Parse(checkText.Substring(p3start, p3end - p3start).Replace(".", "").Trim());
						}
					}

					pos = m_currentEventHTML.IndexOf("</ul>", pos + 1);
					if (pos < 0) break;
					pos = m_currentEventHTML.IndexOf("eventid=", pos + 1);
				}
			}
			catch (Exception e)
			{
				Logger.Log("getResource()-예외 발생: " + e);
			}
		}

		private void _browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			/*
			// Check wheter the document is available (it should be)
			if ((_browser.Document != null) && (_browser.Document.Window != null))
			{
				// Subscribe to the Error event
				_browser.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
			}
			*/
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

			if (_collector == null) return;

			getResource();
			_collector.StartCollect(loopURL, m_currentHTML, loopCookie);
		}

		private void loginRetryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool topMost = this.TopMost;
			this.TopMost = false;

			using (LoginForm loginForm = new LoginForm())
			{
				if (loginForm.ShowDialog() == DialogResult.OK)
				{
					Stop();

					_uniName = loginForm.UniverseName;
					_loginUrl = loginForm.URL.AbsoluteUri;

#if AUTOFS
					const string title = "oBrowser2+";
#else
					const string title = "oBrowser2";
#endif
					this.Text = title + ": " + _uniName;
					notifyIcon1.Text = this.Text;

					_browser.Navigate(_loginUrl);
					loginToolStripMenuItem.Enabled = false;
					loginRetryToolStripMenuItem.Enabled = true;
					refreshToolStripMenuItem.Enabled = true;
					resRefreshToolStripMenuItem.Enabled = true;
					openbrowserToolStripMenuItem.Enabled = true;

					try
					{
						// 통계정보 전송
						oBStatService svc = new oBStatService();
						svc.Timeout = 5000;
						if (string.IsNullOrEmpty(loginForm.LoginID))
							svc.StatLog(_uniName, "SESSION USE", title + " v." + Application.ProductVersion);
						else
							svc.StatLog(_uniName, LoginForm.GetOriginalID(loginForm.LoginID), title + " v." + Application.ProductVersion);

					}
					catch (Exception)
					{
					}
				}
			}

			this.TopMost = topMost;
		}

		private void alarmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 이벤트 설정을 할 수 없는 상태입니다.",
								  "이벤트 설정 장애 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			loadAlarmSettings();

			bool topMost = this.TopMost;
			this.TopMost = false;

			if (alarmForm == null) alarmForm = (m_alarmList == null) ? new frmEventAlarm() : new frmEventAlarm(m_alarmList);
			alarmForm.ResourceInfos = _collector.ResourceInfos;
			alarmForm.UniverseName = _uniName;
			//DialogResult re =
			alarmForm.ShowDialog();
			//Logger.Log("결과: " + re);
			
			saveAlarmSettings(alarmForm.AlarmList);

			this.TopMost = topMost;
		}

		private static bool isEqual(SortedList<DateTime, string> list1, SortedList<DateTime, string> list2)
		{
			if (list1.Count != list2.Count) return false;

			for (int i = 0; i < list1.Count; i++)
			{
				if ((list1.Keys[i] != list2.Keys[i]) || (list1.Values[i] != list2.Values[i])) return false;
			}
			return true;
		}

		private void saveAlarmSettings(SortedList<DateTime, string> compareList)
		{
			bool saveFlag = false;
			lock (m_alarmList)
			{
				// 목록이 변경된 경우에만 업데이트
				if (!isEqual(m_alarmList, compareList))
				{
					saveFlag = true;
					m_alarmList = new SortedList<DateTime, string>(compareList);
				}
			}

			if (saveFlag) OB2Util.SaveAlarmSettings(m_alarmList);
		}

		private void loadAlarmSettings()
		{
			lock (m_alarmList)
			{
				m_alarmList = OB2Util.LoadAlarmSettings() ?? new SortedList<DateTime, string>();
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
								  "원정 설정 장애 - oBrowser2: " + _uniName,
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
			missionForm.UniverseName = _uniName;
			missionForm.ShowDialog(this);

			if (missionForm.ContextCookies != loopCookie) loopCookie = missionForm.ContextCookies;
			this.TopMost = topMost;
		}

		private void resMoveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 모으기 설정을 할 수 없는 상태입니다.",
								  "자원 모으기 설정 장애 - oBrowser2: " + _uniName,
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
			resMoveForm.UniverseName = _uniName;
			resMoveForm.ShowDialog(this);

			this.TopMost = topMost;
		}

		private void expeditionToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			goExpedition(true);
		}

		private void goExpedition(bool showMsg)
		{
			//frmMission missionForm = new frmMission();
			frmOption missionForm = new frmOption();
			missionForm.NaviURL = loopURL;
			missionForm.ContextCookies = loopCookie;
			missionForm.ResourceInfos = _collector.ResourceInfos;
			missionForm.UniverseName = _uniName;

			TreeNode theNode = treeView.SelectedNode;
			if (theNode.Tag != null)
			{
				missionForm.FromLocation = (string)theNode.Tag;
				missionForm.PlanetType = (theNode.Text.IndexOf("(달)") > 0) ? 3 : 1;
			}

			// 이벤트 알림 창이 열려 있는 상태 체크
			if (alarmForm != null && alarmForm.Visible) saveAlarmSettings(alarmForm.AlarmList);

			for (int reCount = 0; reCount < 3; reCount++)
			{
				bool retry = false;
				if (missionForm.GoExpedition(ref retry))
				{
					loadAlarmSettings();

					string dd = DateTime.Now.ToString("HH:mm");

					TreeNode node = treeUpdate("알림", false);
					TreeNode child = treeUpdate(node, "원정", false);
					treeUpdate(child, "[" + dd + "] 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
					treeExpand(child);
					treeExpand(node);

					if (showMsg)
					{
						SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
						MessageBoxEx.Show("원정함대가 출발했습니다.",
										  "원정 - oBrowser2: " + _uniName,
						                  MessageBoxButtons.OK,
						                  MessageBoxIcon.Information,
						                  3*1000);
					}
					break;
				}

				if (missionForm.ContextCookies != loopCookie) loopCookie = missionForm.ContextCookies;
				if (!retry) break;
			}

			if (alarmForm != null)
			{
				alarmForm.AlarmList = m_alarmList;
				if (alarmForm.Visible) alarmForm.RefreshList();
			}
		}

		private void resMoveToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			//frmResCollect resMoveForm = new frmResCollect();
			frmOption resMoveForm = new frmOption();
			resMoveForm.NaviURL = loopURL;
			resMoveForm.ContextCookies = loopCookie;
			resMoveForm.ResourceInfos = _collector.ResourceInfos;
			resMoveForm.UniverseName = _uniName;

			TreeNode theNode = treeView.SelectedNode;
			if (theNode.Tag != null)
			{
				resMoveForm.FromLocation = (string)theNode.Tag;
				resMoveForm.PlanetType = (theNode.Text.IndexOf("(달)") > 0) ? 3 : 1;
			}

			bool retry = false;
			if (resMoveForm.MoveResource(ref retry))
			{
				string dd = DateTime.Now.ToString("HH:mm");

				TreeNode node = treeUpdate("알림", false);
				TreeNode child = treeUpdate(node, "자원운송", false);
				treeUpdate(child, "[" + dd + "] " + (string)theNode.Tag + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
				treeExpand(child);
				treeExpand(node);

				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("운송 함대가 출발했습니다.",
								  "자원 운송 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Information,
								  3 * 1000);
			}
			if (resMoveForm.ContextCookies != loopCookie) loopCookie = resMoveForm.ContextCookies;
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
				fleetMove2ToolStripMenuItem.Text = "기동함대 출발";
				fleetMove2ToolStripMenuItem.ToolTipText = "함대 기동 옵션에서 설정된 내용대로 즉시 기동함대를 보냅니다.";
				fleetMove2ToolStripMenuItem.Enabled = true;
			}
			else if (text.IndexOf("은하") >= 0)
			{
				expeditionToolStripMenuItem2.Enabled = false;
				resMoveAllToolStripMenuItem.Enabled = true;
				resMoveToolStripMenuItem2.Enabled = false;
				fleetSavingToolStripMenuItem.Enabled = false;
				fleetMove2ToolStripMenuItem.Enabled = false;
			}
			else
			{
				expeditionToolStripMenuItem2.Text = "이 행성에서 원정함대 출발";
				expeditionToolStripMenuItem2.ToolTipText = "원정 옵션에서 설정된 함대 구성으로 이 행성에서 즉시 원정을 보냅니다.";
				expeditionToolStripMenuItem2.Enabled = true;
				resMoveAllToolStripMenuItem.Enabled = false;
				resMoveToolStripMenuItem2.Enabled = true;
				fleetSavingToolStripMenuItem.Enabled = true;
				fleetMove2ToolStripMenuItem.Text = "이 행성에서 기동함대 출발";
				fleetMove2ToolStripMenuItem.ToolTipText = "함대 기동 옵션에서 설정된 함대 구성으로 이 행성에서 즉시 기동함대를 보냅니다.";
				fleetMove2ToolStripMenuItem.Enabled = true;
			}
		}

		private void flSavSettingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙 설정을 할 수 없는 상태입니다.",
								  "플릿 세이빙 설정 장애 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			bool topMost = this.TopMost;
			this.TopMost = false;

			frmFleetSaving flSavForm = new frmFleetSaving();
			flSavForm.NaviURL = loopURL;
			flSavForm.ContextCookies = loopCookie;
			flSavForm.ResourceInfos = _collector.ResourceInfos;
			flSavForm.UniverseName = _uniName;
			flSavForm.ShowDialog(this);

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

		private void checkFleetSaving(string sHtml)
		{
			SettingsHelper helper = SettingsHelper.Current;

			string s = sHtml;
			string attackHash = helper.AttackHash ?? "";
			while (true)
			{
				int attPos = s.IndexOf("<span>attack</span>");
				if (attPos < 0) break;

				int attPos1 = s.Substring(0, attPos).LastIndexOf("<ul>");
				int attPos2 = s.IndexOf("</ul>", attPos1 + 1);
				if (attPos1 < 0 || attPos2 < 0) break;
				
				string s2 = s.Substring(0, attPos);

				int posStart = s2.LastIndexOf("descfleet\">");
				if (posStart < 0) continue;
				posStart += 11;
				int posEnd = s2.IndexOf("</li>", posStart);
				if (posEnd < 0) continue;

				string enemyMission = s2.Substring(posStart, posEnd - posStart);
				if (!enemyMission.Equals("enemy fleet")) continue;

				string hashVal = s.Substring(attPos1, attPos2 + 5 - attPos1); // <ul> ~ </ul>
				string hash = OB2Security.Hash(hashVal);

				if ((attackHash.Length == 0) || (attackHash.IndexOf(hash) < 0))
				{
					// SMS 송신
					if (!isSentSMS) SendSMS.Send();

					// SMTP 송신
					if (!isSentSMTP) SendSMTP.Send(_uniName, hash, hashVal);

					// AttackHash 값 저장
					if (attackHash.Length > 0) attackHash += "|";
					attackHash += hash;

#if AUTOFS
					// 플릿세이빙 예약 저장
					if (!string.IsNullOrEmpty(helper.AutoFleetSaving) && helper.AutoFleetSaving == "Y")
					{
						int ePos = s.IndexOf("eventid=", attPos);
						if (ePos < 0) continue;
						int ePos2 = s.IndexOf("\"", ePos + 1);
						if (ePos2 < 0) continue;
						string eventID = s.Substring(ePos + 8, ePos2 - ePos - 8);

						string eventUrl = loopURL.ToString();
						eventUrl = eventUrl.Replace("=overview", "=eventListTooltip&ajax=1&eventID=" + eventID);
						string preCookie = string.Copy(loopCookie);
						string checkText = WebCall.GetHtml(new Uri(eventUrl), ref loopCookie).ToLower();
						if (loopCookie != preCookie)
							Logger.Log("쿠키변경: " + preCookie + " --> " + loopCookie);
						Logger.Log("공격 탐지: [..." + checkText.Replace("\r\n", "\n") + "...]");

						int cPos = s.IndexOf("detailsfleet\">", attPos) + 14;
						if (cPos < 14) continue;
						int cPos2 = s.IndexOf("<a", cPos);
						if (cPos2 < 0) continue;
						string enemyNum = s.Substring(cPos, cPos2 - cPos).Trim();

						// 기준 함대 미만의 공격은 무시함
						if (!checkDummyAttack(int.Parse(enemyNum), checkText))
						{
							Logger.Log("기준 함대(순양-5, 구축-10) 미만... 무시함.");
							break;
						}

						int pos = s2.LastIndexOf("arrivaltime\">") + 13;
						if (pos < 13) continue;
						int pos2 = s2.IndexOf("clock", pos + 1);
						if (pos2 < 0) continue;

						int diff = SettingsHelper.Current.ApplySummerTime ? -7 : -8;

						// 도착시각(독일시각: HH:mm:ss Clock)
						string arrivalTime = s2.Substring(pos, pos2 - pos);
						string arrTimeDE;
						if (arrivalTime.Split('.').Length == 3) // 일.월.년 시:분:초
						{
							string part1 = arrivalTime.Split(' ')[0];
							string part2 = arrivalTime.Split(' ')[1];
							arrTimeDE = part1.Split('.')[2] + "-" + part1.Split('.')[1] + "-" + part1.Split('.')[0] + " " + part2;
						}
						else if (arrivalTime.Split('.').Length == 2) // 일.월 시:분:초
						{
							string part1 = arrivalTime.Split(' ')[0];
							string part2 = arrivalTime.Split(' ')[1];
							arrTimeDE = DateTime.Now.AddHours(diff).Year + "-" + part1.Split('.')[1] + "-" + part1.Split('.')[0] + " " + part2;
						}
						else // 시:분:초
						{
							arrTimeDE = DateTime.Now.AddHours(diff).ToString("yyyy-MM-dd") + " " + arrivalTime;
						}

						// 도착행성
						string val = "";
						pos = 0;
						string destText = hashVal.Substring(hashVal.IndexOf("destfleet\">"));
						for (int i = 0; i < _collector.ResourceInfos.Length; i++)
						{
							pos = destText.IndexOf(_collector.ResourceInfos[i].Location);
							if (pos >= 0)
							{
								val = _collector.ResourceInfos[i].Location;
								break;
							}
						}
						if (pos < 0) return;

//---------------------->???? 행성 형태
						if ((destText.Substring(pos).IndexOf("(moon)") >= 0) || (destText.Substring(pos).Trim() == "moon <")) val += " (달)"; // 달

						// 도착 시각 - 1분
						DateTime picker = DateTime.Parse(arrTimeDE).AddHours(-diff);
						DateTime key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute).AddMinutes(-1);

						DialogResult r = MessageBoxEx.Show("공격탐지: 도착 1분전에 자동으로 플릿세이빙을 할 수 있습니다. 하시겠습니까?\r\n" +
						                                   " - 행성: " + val + "\r\n" +
						                                   " - 도착시각: " + picker.ToString("yyyy-MM-dd HH:mm") + "\r\n\r\n" +
														   "[..." + hashVal + "...]",
						                                   "공격탐지 - oBrowser2: " + _uniName,
						                                   MessageBoxButtons.YesNo,
						                                   MessageBoxIcon.Question,
						                                   MessageBoxDefaultButton.Button1,
						                                   10*1000);
						if (r == DialogResult.Yes)
						{
							lock (m_alarmList)
							{
								if (!m_alarmList.ContainsKey(key))
									m_alarmList.Add(key, "[R]2^" + val);
								else
									m_alarmList[key] = "[R]2^" + val;
							}
							OB2Util.SaveAlarmSettings(m_alarmList);

							if (alarmForm != null)
							{
								alarmForm.AlarmList = m_alarmList;
								if (alarmForm.Visible) alarmForm.RefreshList();
							}
							Logger.Log("공격탐지 - 플릿세이빙 설정 저장: 행성=" + val + ", 시각=" + key.ToString("yyyy-MM-dd HH:mm"));
						}
					}
#endif
				}
				s = s.Substring(attPos2);
			}

			if (attackHash.Length > 0)
			{
				helper.AttackHash = attackHash;
				helper.Changed = true;
				// helper.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로

				isSentSMS = true;
				isSentSMTP = true;
			}
		}

		private bool checkDummyAttack(int num, string checkText)
		{
			// 기준 함대(순양함 5대 또는 구축함 10대) 미만의 공격은 플릿하지 않음
			try
			{
				const string check1 = "ships:";
				int pos = checkText.IndexOf(check1);
				int pos3 = checkText.IndexOf("shipment:", pos + 1);
				string text = checkText.Substring(pos, pos3 - pos);

				int noDummy = 0;

				pos = text.IndexOf("light");
				if (pos >= 0) noDummy++;
				pos = text.IndexOf("heavy");
				if (pos >= 0) noDummy++;
				pos = text.IndexOf("cruiser");
				if (pos >= 0) noDummy++;
				pos = text.IndexOf("battleship");
				if (pos >= 0) noDummy += 2;
				pos = text.IndexOf("battlecruiser");
				if (pos >= 0) noDummy += 5;
				pos = text.IndexOf("destroyer");
				if (pos >= 0) noDummy += 5;
				pos = text.IndexOf("bomber");
				if (pos >= 0) noDummy += 5;
				pos = text.IndexOf("deathstar");
				if (pos >= 0) noDummy += 10;

				// 5종류 이상의 함대를 보냈으면 진짜 공격
				if (noDummy >= 5) return true;

				// 2종류 이상의 함대를 5대 이상 보낸 공격이면 진짜 공격
				if ((noDummy >= 2) && (num >= 5)) return true;

				// 1종류의 함대를 10대 이상 보낸 공격이면 진짜 공격
				if ((noDummy == 1) && (num >= 10)) return true;

				// 나머지는 모두 가짜 공격
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void goFleetsaving(string coords, int planetType)
		{
			//frmFleetSaving flSavForm = new frmFleetSaving();
			frmOption flSavForm = new frmOption();
			flSavForm.NaviURL = loopURL;
			flSavForm.ContextCookies = loopCookie;
			flSavForm.ResourceInfos = _collector.ResourceInfos;
			flSavForm.FromLocation = coords;
			flSavForm.PlanetType = planetType;
			flSavForm.UniverseName = _uniName;

			for (int reCount = 0; reCount < 3; reCount++)
			{
				bool retry = false;
				if (flSavForm.FleetSaving(ref retry))
				{
					string dd = DateTime.Now.ToString("HH:mm");

					TreeNode node = treeUpdate("알림", false);
					TreeNode child = treeUpdate(node, "플릿세이빙", false);
					treeUpdate(child, "[" + dd + "] " + coords + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
					treeExpand(child);
					treeExpand(node);

					SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
					MessageBoxEx.Show("해당 행성의 모든 함대와 자원을 안전하게 이동시켰습니다.",
									  "플릿 세이빙!! - oBrowser2: " + _uniName,
					                  MessageBoxButtons.OK,
					                  MessageBoxIcon.Information,
					                  3*1000);
					break;
				}

				if (flSavForm.ContextCookies != loopCookie) loopCookie = flSavForm.ContextCookies;
				if (!retry) break;
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

			//frmResCollect resMoveForm = new frmResCollect();
			frmOption resMoveForm = new frmOption();
			resMoveForm.NaviURL = loopURL;
			resMoveForm.ContextCookies = loopCookie;
			resMoveForm.ResourceInfos = _collector.ResourceInfos;
			resMoveForm.GalaxyMethod = true;
			resMoveForm.UniverseName = _uniName;

			ArrayList retryList = new ArrayList();
			foreach (ResourceInfo info in _collector.ResourceInfos)
			{
				try
				{
					if ((info == null) || string.IsNullOrEmpty(info.Location)) continue;

					if ((galaxy == "0") || info.Location.StartsWith(galaxy))
					{
						count++;

						resMoveForm.FromLocation = info.Location;
//---------------------->?????
						resMoveForm.PlanetType = ((info.ColonyName.IndexOf("(달)") > 0) || (info.ColonyName.Trim() == "달")) ? 3 : 1;

						// 이벤트 알림 창이 열려 있는 상태 체크
						if (alarmForm != null && alarmForm.Visible) saveAlarmSettings(alarmForm.AlarmList);

						bool retry = false;
						if (resMoveForm.MoveResource(ref retry))
						{
							loadAlarmSettings();

							string dd = DateTime.Now.ToString("HH:mm");

							TreeNode node = treeUpdate("알림", false);
							TreeNode child = treeUpdate(node, "자원운송", false);
							treeUpdate(child, "[" + dd + "] " + info.Location + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
							treeExpand(child);
							treeExpand(node);

							successCount++;
							if (resMoveForm.ContextCookies != loopCookie) loopCookie = resMoveForm.ContextCookies;
						}
						else
						{
							// 재시도 플래그
							if (retry) retryList.Add(info);
							continue;
						}

						if (alarmForm != null)
						{
							alarmForm.AlarmList = m_alarmList;
							if (alarmForm.Visible) alarmForm.RefreshList();
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

			// 재시도: 2회
			for (int reCount = 0; reCount < 2; reCount++)
			{
				if (retryList.Count == 0) break;

				ResourceInfo[] infos = (ResourceInfo[]) retryList.ToArray(typeof (ResourceInfo));
				foreach (ResourceInfo info in infos)
				{
					try
					{
						if ((info == null) || string.IsNullOrEmpty(info.Location)) continue;

						if ((galaxy == "0") || info.Location.StartsWith(galaxy))
						{
							resMoveForm.FromLocation = info.Location;
//---------------------->?????
							resMoveForm.PlanetType = ((info.ColonyName.IndexOf("(달)") > 0) || (info.ColonyName.Trim() == "달")) ? 3 : 1;

							// 이벤트 알림 창이 열려 있는 상태 체크
							if (alarmForm != null && alarmForm.Visible) saveAlarmSettings(alarmForm.AlarmList);

							bool retry = false;
							if (resMoveForm.MoveResource(ref retry))
							{
								loadAlarmSettings();

								string dd = DateTime.Now.ToString("HH:mm");

								TreeNode node = treeUpdate("알림", false);
								TreeNode child = treeUpdate(node, "자원운송", false);
								treeUpdate(child, "[" + dd + "] " + info.Location + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
								treeExpand(child);
								treeExpand(node);

								successCount++;
								retryList.Remove(info);
								if (resMoveForm.ContextCookies != loopCookie) loopCookie = resMoveForm.ContextCookies;
							}
							else
							{
								continue;
							}

							if (alarmForm != null)
							{
								alarmForm.AlarmList = m_alarmList;
								if (alarmForm.Visible) alarmForm.RefreshList();
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
			}

			if (count > 0 && count == successCount)
			{
				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("해당 은하의 모든 행성에서 운송 함대가 출발했습니다.",
								  "자원 운송 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Information,
								  3 * 1000);
			}
		}

		private void goFleetMoving(bool showMsg)
		{
			//frmFleetAttack flMovForm = new frmFleetAttack();
			frmOption flMovForm = new frmOption();
			flMovForm.NaviURL = loopURL;
			flMovForm.ContextCookies = loopCookie;
			flMovForm.ResourceInfos = _collector.ResourceInfos;
			flMovForm.UniverseName = _uniName;

			TreeNode theNode = treeView.SelectedNode;
			if (theNode.Tag != null)
			{
				flMovForm.FromLocation = (string)theNode.Tag;
				flMovForm.PlanetType = (theNode.Text.IndexOf("(달)") > 0) ? 3 : 1;
			}

			// 이벤트 알림 창이 열려 있는 상태 체크
			if (alarmForm != null && alarmForm.Visible) saveAlarmSettings(alarmForm.AlarmList);

			for (int reCount = 0; reCount < 3; reCount++)
			{
				bool retry = false;
				if (flMovForm.MoveFleet(ref retry))
				{
					loadAlarmSettings();

					string dd = DateTime.Now.ToString("HH:mm");

					TreeNode node = treeUpdate("알림", false);
					TreeNode child = treeUpdate(node, "함대 기동", false);
					treeUpdate(child, "[" + dd + "] 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
					treeExpand(child);
					treeExpand(node);

					if (showMsg)
					{
						SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
						MessageBoxEx.Show("기동함대가 출발했습니다.",
										  "함대 기동 - oBrowser2: " + _uniName,
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Information,
										  3 * 1000);
					}
					break;
				}

				if (flMovForm.ContextCookies != loopCookie) loopCookie = flMovForm.ContextCookies;
				if (!retry) break;
			}

			if (alarmForm != null)
			{
				alarmForm.AlarmList = m_alarmList;
				if (alarmForm.Visible) alarmForm.RefreshList();
			}
		}

		private void goFleetMoving2(string id)
		{
			//frmFleetAttack flMovForm = new frmFleetAttack();
			frmOption flMovForm = new frmOption();
			flMovForm.NaviURL = loopURL;
			flMovForm.ContextCookies = loopCookie;
			flMovForm.ResourceInfos = _collector.ResourceInfos;
			flMovForm.UniverseName = _uniName;

			FleetMoveInfo info = null;
			NameObjectCollection fleetInfo = OB2Util.GetFleetMoveInfo2();
			if (fleetInfo != null && fleetInfo.ContainsKey(id))
				info = (FleetMoveInfo) fleetInfo[id];

			// 이벤트 알림 창이 열려 있는 상태 체크
			if (alarmForm != null && alarmForm.Visible) saveAlarmSettings(alarmForm.AlarmList);

			for (int reCount = 0; reCount < 3; reCount++)
			{
				bool retry = false;
				if (flMovForm.MoveFleet2(info, ref retry))
				{
					loadAlarmSettings();

					string dd = DateTime.Now.ToString("HH:mm");

					TreeNode node = treeUpdate("알림", false);
					TreeNode child = treeUpdate(node, "함대 기동", false);
					treeUpdate(child, "[" + dd + "] " + getMoveType(info.MoveType) + " 함대 출발", Color.FromArgb(0x00, 0x00, 0xFF));
					treeExpand(child);
					treeExpand(node);
					break;
				}

				if (flMovForm.ContextCookies != loopCookie) loopCookie = flMovForm.ContextCookies;
				if (!retry) break;
			}

			if (alarmForm != null)
			{
				alarmForm.AlarmList = m_alarmList;
				if (alarmForm.Visible) alarmForm.RefreshList();
			}
		}

		private void fleetMoveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_collector == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대 기동 설정을 할 수 없는 상태입니다.",
								  "함대 기동 설정 장애 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return;
			}

			bool topMost = this.TopMost;
			this.TopMost = false;

			frmFleetAttack flMoveForm = new frmFleetAttack();
			flMoveForm.ResourceInfos = _collector.ResourceInfos;
			flMoveForm.UniverseName = _uniName;
			flMoveForm.ShowDialog(this);

			this.TopMost = topMost;
		}

		private void fleetMove2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			goFleetMoving(true);
		}

		private static string getMoveType(int moveType)
		{
			switch (moveType)
			{
				case 1:
					return "공격";
				case 3:
					return "운송";
				case 4:
					return "배치";
				case 6:
					return "정탐";
				case 7:
					return "식민";
				case 8:
					return "수확";
				case 15:
					return "원정";
			}
			return "(잘못됨)";
		}

		private void doPlanetJob(string action, string coords, int planetType, string bID)
		{
			frmOption jobForm = new frmOption();
			jobForm.NaviURL = loopURL;
			jobForm.ContextCookies = loopCookie;
			jobForm.ResourceInfos = _collector.ResourceInfos;
			jobForm.UniverseName = _uniName;
			jobForm.FromLocation = coords;
			jobForm.PlanetType = planetType;

			for (int reCount = 0; reCount < 3; reCount++)
			{
				bool retry = false;
				if (jobForm.DoPlanetJob(action, bID, ref retry))
				{
					string dd = DateTime.Now.ToString("HH:mm");

					TreeNode node = treeUpdate("알림", false);
					string actName = OB2Util.GetActionName(action);
					TreeNode child = treeUpdate(node, actName, false);
					treeUpdate(child, "[" + dd + "] " + actName + " 시작", Color.FromArgb(0x00, 0x00, 0xFF));
					treeExpand(child);
					treeExpand(node);

					SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
					MessageBoxEx.Show(actName + " 작업을 시작했습니다.",
									  actName + " - oBrowser2: " + _uniName,
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Information,
									  3 * 1000);
					break;
				}

				if (jobForm.ContextCookies != loopCookie) loopCookie = jobForm.ContextCookies;
				if (!retry) break;
			}
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{
#if TEST
			string text = "title=579 star=\"1227764689\">0:09:40</DIV></TH>\r\n<TH colSpan=3><SPAN class=\"flight attack\">Udie <A onclick=showMessageMenu(101433) href=\"#\"><IMG title=\"메시지 작성\" alt=\"메시지 작성\" src=\"http://ogame2.thermidor.net/skin/img/m.gif\"></A>의 <A class=attack onmouseover='return overlib(\"<font color=white><b>함선 수 100 <br>순양함 <br></b></font>\");' onmouseout=\"return nd();\" href=\"#\">함대</A><A title=\"함선 수 100 순양함 \" href=\"#\"></A>가 Udie <A href=\"javascript:showGalaxy(1,207,9)\" attack>[1:207:9]</A> 행성에서 planet-33621610 <A href=\"javascript:showGalaxy(1,207,11)\" attack>[1:207:11]</A> 행성으로 접근중입니다. 임무: 공격</SPAN> </TH></TR>\n";
			bool result = checkDummyAttack(text);
			MessageBox.Show("테스트: " + result);
			text = "title='106'star='1227764689'></div></th>\n<th colspan='3'><span class='flight attack'>Udie <a href='#' onclick='showMessageMenu(101433)'><img src='http://ogame2.thermidor.net/skin/img/m.gif' title='메시지 작성' alt='메시지 작성'></a>의 <a href='#' onmouseover='return overlib(\"&lt;font color=white&gt;&lt;b&gt;함선 수 100 &lt;br&gt;순양함 &lt;br&gt;&lt;/b&gt;&lt;/font&gt;\");' onmouseout='return nd();' class='attack'>함대</a><a href='#' title='함선 수 100 순양함 '></a>가 Udie <a href=\"javascript:showGalaxy(1,207,9)\" attack>[1:207:9]</a> 행성에서 planet-33621610 <a href=\"javascript:showGalaxy(1,207,11)\" attack>[1:207:11]</a> 행성으로 접근중입니다. 임무: 공격</span>";
			result = checkDummyAttack(text);
			MessageBox.Show("테스트: " + result);
#endif
		}
	}
}
