using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NETS_iMan.chatWebsvc;
using Timer=System.Windows.Forms.Timer;

namespace NETS_iMan
{
	public partial class GroupMessage : Form
	{
		private static string IN_GROUP_MESSAGE = string.Empty;
		private static readonly object LOCK_OBJECT = new object();

		private readonly ChatService chatSvc = new ChatService();
		private readonly ChatService groupChatSvc = new ChatService();
		private readonly RichTextBox rtbTemp = new RichTextBox();

		private readonly ToolTip toolTip1 = new ToolTip();
		private readonly ToolTip toolTip2 = new ToolTip();
		private readonly ToolTip toolTip3 = new ToolTip();
		private string groupID;
		private bool isLoaded;

		private bool isWriting;

		private string[] prev_messages;
		private int prev_msgidx;
		private int view_msgidx;

		private volatile int ERROR_COUNT;
		private int msgClearCount;

		private enum SEND_FLAG
		{
			STATUS,
			MESSAGE
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NETS_iMan.GroupMessage"/> class.
		/// </summary>
		/// <param name="strID">The Group ID.</param>
		/// <param name="strUserName">Name + ( ID )</param>
		public GroupMessage(string strID, string strUserName)
		{
			InitializeComponent();
			init(strID, strUserName, false);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NETS_iMan.GroupMessage"/> class.
		/// </summary>
		/// <param name="strID">The Group ID.</param>
		/// <param name="strUserName">Name + ( ID )</param>
		/// <param name="autoJoin">if set to <c>true</c> [auto join].</param>
		public GroupMessage(string strID, string strUserName, bool autoJoin)
		{
			InitializeComponent();
			init(strID, strUserName, autoJoin);
		}

		private void init(string strID, string strUserName, bool autoJoin)
		{
			groupID = strID;

			lstUsers.Items.Clear();

			rtbMessage.Clear();
			rtbMessage.SelectionColor = Color.LightGray;
			rtbMessage.SelectedText = "*회의실에서는 이미지나 파일 첨부를 가급적 자제해주시기 바랍니다.\r\n*모든 참가자들의 컴퓨터가 느려질 수 있습니다.\r\n\r\n";
			
			if (!autoJoin)
			{
				rtbMessage.SelectionColor = Color.Violet;
				rtbMessage.SelectedText = strUserName + "님이 입장하기를 기다리는 중입니다...\r\n\r\n";
			}

			// 현 회의실 직원 목록 구하기
			string users = groupChatSvc.GetGroupUsers(groupID);
			string gID = users.Substring(0, users.IndexOf("^"));
			if (gID == groupID)
			{
				string s2 = users.Substring(users.IndexOf("^") + 1);
				string[] user = s2.Split(new char[] {'&'});
				foreach (string userName in user)
				{
					string uID = userName.IndexOf("(") >= 0
					             	? userName.Substring(userName.IndexOf("(")).Replace("(", "").Replace(")", "")
					             	: userName;
					NVItem item = new NVItem(userName, uID);
					lstUsers.Items.Add(item);

					lock (frmMain.CHATTING_GROUPS)
					{
						foreach (GroupItem group in frmMain.CHATTING_GROUPS)
						{
							if (group.GroupID == groupID)
							{
								group.UserList.Add(uID);
								break;
							}
						}
					}
				}
			}

			prev_messages = new string[] { null, null, null, null, null, null, null, null, null, null };
			view_msgidx = prev_msgidx = 9;

			toolTip1.SetToolTip(btnEmotion, "이모티콘 삽입");
			toolTip2.SetToolTip(btnColor, "글 색깔 설정");
			toolTip3.SetToolTip(btnFont, "글꼴 설정");

			rtbWriteMsg.DragDrop += Object_DragDrop;

			groupChatSvc.ReceiveGroupMessageCompleted += groupChatSvc_ReceiveGroupMessageCompleted;
			groupChatSvc.SendGroupMessageCompleted += groupChatSvc_SendGroupMessageCompleted;
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			try
			{
				processMsgSend();
				isWriting = false;
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::btnSend_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "btnSend_Click() - " + ex);
			}
		}

		private static bool isObjectEmbeded(string rtf)
		{
			return (rtf.IndexOf(@"\pic") >= 0) || (rtf.IndexOf(@"\objdata") >= 0);
		}

		private void GroupMessage_Load(object sender, EventArgs e)
		{
			isLoaded = false;
			timer1.Enabled = true;
			for (int i = 0; i < frmMain.CHATTING_GROUPS.Count; i++)
			{
				GroupItem group = (GroupItem)frmMain.CHATTING_GROUPS[i];
				if (groupID == group.GroupID)
				{
					Text = "제" + group.GroupNo + " 회의실";
					break;
				}
			}
			panel1.Visible = false;


			SettingsHelper helper = SettingsHelper.Current;

			if (!string.IsNullOrEmpty(helper.Font))
				rtbWriteMsg.Font = deserialize(helper.Font);

			if (!string.IsNullOrEmpty(helper.Color))
				rtbWriteMsg.ForeColor = Color.FromArgb(int.Parse(helper.Color));


			// 투명도 조정
			this.Opacity = helper.ChatOpacity / 100.0;
			trackBar.Value = helper.ChatOpacity;

			rtbWriteMsg.Focus();
		}

		private void Object_DragDrop(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
			processFileSend(e.Data, e);
		}

		private void GroupMessage_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				DialogResult dr = MessageBoxEx.Show("[" + Text + " 퇴장]\r\n\r\n이 회의실에서 나가시겠습니까?", Text, MessageBoxButtons.YesNo,
									MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 20 * 1000);
				if (dr == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}

				SettingsHelper.Current.Save();
				this.Hide();

				// 상태를 업데이트한다.
				rtbTemp.Text = "<file>STATUS|0";
				groupChatSvc.SendGroupMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
											  rtbTemp.Rtf);
				isWriting = false;

				// 회의실 제거
				lock (frmMain.CHATTING_GROUPS)
				{
					foreach (GroupItem group in frmMain.CHATTING_GROUPS)
					{
						if (groupID == group.GroupID)
						{
							frmMain.CHATTING_GROUPS.Remove(group);
							break;
						}
					}

					if (frmMain.CHATTING_GROUPS.Count == 0) frmMain.GROUP_COUNT = 0;
				}

				// 회의실 퇴장
				groupChatSvc.RemoveUserGroup(frmMain.LOGIN_INFO.LoginID, groupID);

				Dispose();

			}
			catch (System.Net.WebException ex)
			{
				// 네트워크 또는 DNS 에러
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[GroupMessage_FormClosing()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[GroupMessage_FormClosing()]: " + ex);
					}
				}
				else
				{
					ErrorReport.SendReport("GroupMessage_FormClosing", ex);
					Logger.Log(Logger.LogLevel.ERROR, "GroupMessage_FormClosing(): " + ex);
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage_FormClosing", ex);
				Logger.Log(Logger.LogLevel.ERROR, "GroupMessage_FormClosing(): " + ex);
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				if (frmMain.LOGIN_INFO == null) return;

				// 본인의 상태 표시
				if (frmMain.LOGIN_INFO.IsAbsent)
				{
					lblSelfStatus.Text = "*현재 자리 비움 상태입니다.";
					lblSelfStatus.Visible = true;
				}
				else if (frmMain.LOGIN_INFO.IsBusy)
				{
					lblSelfStatus.Text = "*현재 '다른 용무 중' 상태입니다.";
					lblSelfStatus.Visible = true;
				}
				else
				{
					lblSelfStatus.Visible = false;
				}

				// 상태 전송
				processStatusSend();

				for (int i = 0; i < lstUsers.Items.Count; i++)
				{
					NVItem item = (NVItem) lstUsers.Items[i];

					// 대화상대가 오프라인이면
					if ((frmMain.CACHED_USERS.IndexOf("|" + item.Value + "|") < 0) &&
						(frmMain.CACHED_USERS.IndexOf("|*" + item.Value + "|") < 0) &&
						(frmMain.CACHED_USERS.IndexOf("|/" + item.Value + "|") < 0))
					{
						lstUsers.Items.Remove(item);
						i--;
					}
					else if (frmMain.CACHED_USERS.IndexOf("|*" + item.Value + "|") >= 0)
					{
						item.Text = item.Text + "(*자리 비움)";
					}
					else if (frmMain.CACHED_USERS.IndexOf("|/" + item.Value + "|") >= 0)
					{
						item.Text = item.Text + "(*다른 용무 중)";
					}
					else
					{
						item.Text = item.Text.Replace("(*자리 비움)", "").Replace("(*다른 용무 중)", "");
					}
				}
				lstUsers.Refresh();

				// 메시지 수신
				if (IN_GROUP_MESSAGE.Trim().Length == 0)
				{
					msgClearCount = 0;
					groupChatSvc.ReceiveGroupMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID);
				}
				else // 이전 메시지가 처리되지 않고 남아 있으면 건너뛴다.
				{
					msgClearCount++;
					if (msgClearCount > 3) // 3회 이상 메시지가 그대로 남아있으면 강제로 지운다.
					{
						lock (LOCK_OBJECT)
						{
							IN_GROUP_MESSAGE = string.Empty;
						}
						msgClearCount = 0;
					}
					else
					{
						Logger.Log(Logger.LogLevel.WARNING,
								   "timer1_Tick(): 남은 메시지[" + IN_GROUP_MESSAGE.Trim().Replace("\r\n", "").Replace("\n", "") + "]");
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::timer1_Tick", ex);
				Logger.Log(Logger.LogLevel.ERROR, "GroupMessage::timer1_Tick() - " + ex);
			}
		}

		void groupChatSvc_ReceiveGroupMessageCompleted(object sender, ReceiveGroupMessageCompletedEventArgs e)
		{
			try
			{
				string strTemp = string.Empty;
				try
				{
					if (!e.Cancelled)
					{
						strTemp = e.Result;
					}
				}
				catch // 비동기 처리에 실패하면 건너뛴다.
				{
				}

				if (strTemp.Trim().Length > 0)
				{
					// 다른 컴퓨터에서 로그인 여부 체크
					if (strTemp.StartsWith("LOGOUT:")) return;

					lock (LOCK_OBJECT)
					{
						IN_GROUP_MESSAGE = strTemp;
					}

					if (IN_GROUP_MESSAGE.Trim().Length > 0)
					{
						string[] msgs = IN_GROUP_MESSAGE.Split(new string[] { "\n\n\t\n" }, StringSplitOptions.RemoveEmptyEntries);
						for (int cnt = 0; cnt < msgs.Length; cnt++)
						{
							string[] arr = msgs[cnt].Split(new char[] { ':' });
							string grpID = arr[0];

							bool grpFlag = true;
							for (int i = 0; i < frmMain.CHATTING_GROUPS.Count; i++)
							{
								GroupItem group = (GroupItem)frmMain.CHATTING_GROUPS[i];
								if (grpID == group.GroupID)
								{
									grpFlag = false;
									break;
								}
							}
							if (grpFlag)
							{
								GroupItem group = new GroupItem(grpID);
								for (int i = 0; i < lstUsers.Items.Count; i++)
								{
									NVItem item = (NVItem)lstUsers.Items[i];
									if (item.Value == frmMain.LOGIN_INFO.LoginID) continue;

									group.UserList.Add(item.Value);
								}

								lock (frmMain.CHATTING_GROUPS)
								{
									group.GroupNo = ++frmMain.GROUP_COUNT;
									frmMain.CHATTING_GROUPS.Add(group);
								}
							}
						}

						bool statusOnly = true;
						for (int k = 0; k < msgs.Length; k++)
						{
							processReceive(msgs[k], true);
							if (msgs[k].IndexOf("<file>STATUS") < 0) statusOnly = false;
						}

						if (!statusOnly)
						{
							int cnt = 0;
							while (cnt < 3)
							{
								if (!chkOffSound.Checked) SoundPlayer.PlaySound("newmsg.wav");
								if (NIUtil.FlashWindowEx(this)) break;
								cnt++;
								Thread.Sleep(500);
							}
						}
					}
					ERROR_COUNT = 0;
				}
			}
			catch (System.Net.WebException ex)
			{
				// 네트워크 또는 DNS 에러
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // 메시지 해석 오류
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "네트워크 오류[GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted()]: " + ex);
					}
				}
				else
				{
					ErrorReport.SendReport("GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted", ex);
					Logger.Log(Logger.LogLevel.ERROR, "GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted(): " + ex);
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted", ex);
				Logger.Log(Logger.LogLevel.ERROR, "GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted(): " + ex);
			}
		}

		void groupChatSvc_SendGroupMessageCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			try
			{
				if (!e.Cancelled)
				{
					if (e.Error == null) return;
				}

				UserState state = (UserState)e.UserState;
				if (state.Flag == SEND_FLAG.STATUS) return;
				string msg = state.Msg;

				// 비동기로 보내다가 에러가 발생하면 동기로 전환한다.
				Thread.Sleep(200);
				try
				{
					groupChatSvc.SendGroupMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
					                              msg);
				}
				catch (Exception) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
				{
					try
					{
						int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
						rtbMessage.Select(pos, 0);
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}

					try
					{
						rtbMessage.SelectionColor = Color.DarkRed;
						rtbMessage.SelectedText = "메시지 송신에 실패했습니다. 다시 시도해보십시오.\r\n\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.WARNING, "GroupMessage::groupChatSvc_SendGroupMessageCompleted() - " + ex);
			}
		}

		private void processStatusSend()
		{
			if (rtbWriteMsg.TextLength > 0)
			{
				if (!isWriting)
				{
					rtbTemp.Text = "<file>STATUS|1";
					string msg = rtbTemp.Rtf;
					groupChatSvc.SendGroupMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
													   msg, new UserState(SEND_FLAG.STATUS, msg));
					isWriting = true;
				}
			}
			else
			{
				if (isWriting)
				{
					rtbTemp.Text = "<file>STATUS|0";
					string msg = rtbTemp.Rtf;
					groupChatSvc.SendGroupMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
													   msg, new UserState(SEND_FLAG.STATUS, msg));
					isWriting = false;
				}
			}
		}

		private void processMsgSend()
		{
			if (rtbWriteMsg.Text.Trim().Length > 0 || isObjectEmbeded(rtbWriteMsg.Rtf))
			{
				prev_messages[prev_msgidx--] = rtbWriteMsg.Rtf;
				if (prev_msgidx < 0) prev_msgidx = 9;
				view_msgidx = prev_msgidx;

				try
				{
					int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
					rtbMessage.Select(pos, 0);
					rtbMessage.ScrollToCaret();
				}
				catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
				{
				}

				if (rtbWriteMsg.Text == "/allusers") // 예약어 실행
				{
					showUserList(true);
				}
				else if (rtbWriteMsg.Text == "/loginusers") // 예약어 실행
				{
					showUserList(false);
				}
				else if (rtbWriteMsg.Text == "/allgroups") // 예약어 실행
				{
					showGroupList();
				}
				else if ((rtbWriteMsg.Text == "/cls") || (rtbWriteMsg.Text == "/clear"))
				{
					rtbMessage.Clear();
				}
				else if (rtbWriteMsg.Text == "/help")
				{
					try
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = "/cls 또는 /clear : 메시지 창 지움\r\n";
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = "방향키 상/하 : 이전 입력 메시지 반복\r\n";
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
				}
				else if ((rtbWriteMsg.Rtf.IndexOf("<file>") < 0) && (rtbWriteMsg.Rtf.IndexOf(@"<\f1 file>") < 0))
				{
					string msg = rtbWriteMsg.Rtf.Replace(":", "%3A");
					groupChatSvc.SendGroupMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
													   msg, new UserState(SEND_FLAG.MESSAGE, msg));

					// 자리비움 경고
					bool isIt = false;
					for (int i = 0; i < lstUsers.Items.Count; i++)
					{
						NVItem item = (NVItem) lstUsers.Items[i];
						if (item.Text.IndexOf("(*자리 비움)") > 0)
						{
							rtbMessage.SelectionColor = Color.DarkRed;
							rtbMessage.SelectedText = item.Text.Replace("(*자리 비움)", "") + "님은 현재 \"자리 비움\" 상태라 응답하지 않을 수 있습니다.\r\n";
							isIt = true;
						}
						else if (item.Text.IndexOf("(*다른 용무 중)") > 0)
						{
							rtbMessage.SelectionColor = Color.DarkRed;
							rtbMessage.SelectedText = item.Text.Replace("(*다른 용무 중)", "") + "님은 현재 \"다른 용무 중\" 상태라 응답하지 않을 수 있습니다.\r\n";
							isIt = true;
						}
					}
					if (isIt)
					{
						rtbMessage.SelectedText = "\r\n";
					}

					rtbMessage.SelectionColor = Color.Black;
					rtbMessage.SelectionFont = new Font("굴림", 9F);
					rtbMessage.SelectedText = frmMain.LOGIN_INFO.UserName + "님의 말:\r\n";
					rtbTemp.Rtf = rtbWriteMsg.Rtf;
					ChatLogger.Log("회의실{" + groupID + "} " + frmMain.LOGIN_INFO.UserName + "님의 말: " + rtbTemp.Text);

					const string rep = @"\fs";
					string rtf = rtbWriteMsg.Rtf;
					for (int size = 1; size < 255; size++)
					{
						if (rtf.IndexOf(rep + size) >= 0)
							rtf = rtf.Replace(rep + size + " ", rep + size + "  ").Replace(rep + size + @"\'", rep + size + @"  \'");
					}
					rtf = rtf.Replace("\\par\r\n", "\\par\r\n ").Replace("\r\n }", "\r\n}"); // '\n' --> '\n '
					rtbWriteMsg.Rtf = rtf;

					try
					{
						rtbMessage.SelectedRtf = rtbWriteMsg.Rtf;
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
				}

				rtbWriteMsg.Clear();
				rtbWriteMsg.Focus();
			}
		}

		private void showUserList(bool bFlag)
		{
			try
			{
				string users = chatSvc.GetAllUsers(frmMain.LOGIN_INFO.LoginID);
				string[] list = users.Split(new char[] {'^'});
				if (list.Length > 0)
				{
					if (bFlag)
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = "===== 전체 목록 =====\r\n";
					}
					else
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = "===== 로그인 목록 =====\r\n";
					}

					foreach (string s in list)
					{
						string[] user = s.Split(new char[] {'&'});
						if (user.Length < 4) continue;

						int status = int.Parse(user[3]);
						if (bFlag || (status == 1) || (status == 9) || (status == 2))
						{
							rtbMessage.SelectionColor = Color.Violet;
							rtbMessage.SelectionFont = new Font("돋움체", 9F);
							rtbMessage.SelectedText = string.Format("{0}({1})\t{2}\t상태: {3}\r\n",
							                                        user[0].PadLeft(6),
							                                        user[1].PadRight(9),
							                                        user[2].PadRight(15),
																	(status == 1) ? "로그인" : ((status == 9) ? "자리비움" : ((status == 2) ? "다른 용무 중" : "로그아웃")));
						}
					}

					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::showUserList", ex);
				Logger.Log(Logger.LogLevel.ERROR, "사용자 목록 출력 중 에러가 발생했습니다: " + ex);
			}
		}

		private void showGroupList()
		{
			try
			{
				string users = chatSvc.GetAllGroups(frmMain.LOGIN_INFO.LoginID);
				string[] list = users.Split(new char[] { '|' });
				if (list.Length > 0)
				{
					rtbMessage.SelectionColor = Color.Violet;
					rtbMessage.SelectionFont = new Font("돋움체", 9F);
					rtbMessage.SelectedText = "===== 회의실 목록 =====\r\n";

					foreach (string s in list)
					{
						int ind = s.IndexOf("^");
						if (ind < 0) continue;
						string gID = s.Substring(0, ind);
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectionFont = new Font("돋움체", 9F);
						rtbMessage.SelectedText = "회의실 ID={" + gID + "}\r\n";

						string s2 = s.Substring(ind + 1);
						string[] user = s2.Split(new char[] { '&' });
						foreach (string u in user)
						{
							rtbMessage.SelectionColor = Color.Violet;
							rtbMessage.SelectionFont = new Font("돋움체", 9F);
							rtbMessage.SelectedText = string.Format(" - {0}\r\n", u);
						}
					}

					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::showGroupList", ex);
				Logger.Log(Logger.LogLevel.ERROR, "회의실 목록 출력 중 에러가 발생했습니다: " + ex);
			}
		}

		private void processFileSend(IDataObject data, object e)
		{
			// 이미지 삽입인 경우 크기 체크
			if ((data.GetData("Bitmap") is Bitmap))
			{
				MemoryStream ms = data.GetData("DeviceIndependentBitmap") as MemoryStream;
				if ((ms != null) && (ms.Length > 640*480*3))
				{
					if (e is DragEventArgs) ((DragEventArgs) e).Effect = DragDropEffects.None;
					if (e is KeyEventArgs) ((KeyEventArgs) e).Handled = true;

					try
					{
						rtbMessage.SelectionColor = Color.DarkRed;
						rtbMessage.SelectedText = "* 640x480 보다 큰 이미지는 파일로 첨부하시기 바랍니다.\r\n";
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
					return;
				}
			}

			string[] paths = (string[]) data.GetData("FileDrop");
			if (paths == null) return;

			string filePath = paths[0];
			FileInfo fi = new FileInfo(filePath);

			bool isOffice = false;
			byte[] bytes = new byte[4];
			using (FileStream fsTemp = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				fsTemp.Read(bytes, 0, 4);
				fsTemp.Close();
			}

			// MS 오피스 문서(워드, 액셀, 파워포인트 등) 처리
			if ((bytes[0] == 0xd0) && (bytes[1] == 0xcf) && (bytes[2] == 0x11) && (bytes[3] == 0xe0))
				isOffice = true;

			// 파일 크기가 512KB를 넘거나 오피스 문서이면
			if ((fi.Length >= 1024 * 1024 / 2) // > 512KB
				|| isOffice)
			{
				if (e is DragEventArgs) ((DragEventArgs) e).Effect = DragDropEffects.None;
				if (e is KeyEventArgs) ((KeyEventArgs) e).Handled = true;

				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* 회의실에서는 512KB가 넘는 파일 전송은 지원하지 않습니다.\r\n";
				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* 파일을 분할하여 전송하시기 바랍니다.\r\n";
				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* 오피스 통합문서도 전송되지 않으므로 압축하여 전송하시기 바랍니다.\r\n";
				try
				{
					rtbMessage.SelectedText = "\r\n";
					rtbMessage.ScrollToCaret();
				}
				catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
				{
				}

				rtbWriteMsg.Clear();
				rtbWriteMsg.Focus();
			}
			else
			{
				string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
				rtbWriteMsg.SelectionFont = new Font("굴림", 9F);
				rtbWriteMsg.SelectionColor = Color.Violet;
				rtbWriteMsg.SelectedText = fileName + "\r\n(아래 아이콘을 클릭하여 저장하십시오. // 더블클릭: 바로 열기)\r\n";
			}
		}

		private void processReceive(string msg, bool loaded)
		{
			string[] strU = msg.Split(':'); // 0: groupID, 1: remoteUser, 2: remoteIP, 3: msg
			string strM = string.Empty;
			if (strU[0] == groupID)
			{
				for (int i = 3; i < strU.Length; i++)
				{
					strM += strU[i];
				}

				if (strU[1] == "Gro@up") // 회의실 상태(멤버 추가/삭제) 메시지
				{
					string strUserName = strM.Substring(0, strM.IndexOf(")") + 1);
					string strUserID = strUserName.IndexOf("(") >= 0
					                   	? strUserName.Substring(strUserName.IndexOf("(")).Replace("(", "").Replace(")", "")
					                   	: strUserName;

					try
					{
						int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
						rtbMessage.Select(pos, 0);
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}

					if ((strM.IndexOf("나가셨습니다") > 0) || // 퇴장
					    (strM.IndexOf("거부했습니다") > 0)) // 초대 거부
					{
						for (int i = 0; i < lstUsers.Items.Count; i++)
						{
							NVItem item = (NVItem) lstUsers.Items[i];
							if (item.Value == strUserID)
							{
								lstUsers.Items.Remove(item);
								break;
							}
						}
						lstUsers.Refresh();

						lock (frmMain.CHATTING_GROUPS)
						{
							foreach (GroupItem group in frmMain.CHATTING_GROUPS)
							{
								if (group.GroupID != groupID) continue;

								foreach (string uID in group.UserList)
								{
									if (uID != strUserID) continue;

									group.UserList.Remove(uID);
									break;
								}
								break;
							}
						}
					}
					else // 입장
					{
						NVItem item = new NVItem(strUserName, strUserID);
						lstUsers.Items.Add(item);
						lstUsers.Refresh();

						lock (frmMain.CHATTING_GROUPS)
						{
							foreach (GroupItem group in frmMain.CHATTING_GROUPS)
							{
								if (group.GroupID != groupID) continue;

								bool isFound = false;
								foreach (string uID in group.UserList)
								{
									if (uID == strUserID)
									{
										isFound = true;
										break;
									}
								}
								if (!isFound) group.UserList.Add(strUserID);
								break;
							}
						}
					}

					rtbMessage.SelectionColor = Color.Violet;
					rtbMessage.SelectedText = strM + "\r\n";

					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
				}
				else
				{
					rtbTemp.Rtf = strM.Replace("%3A", ":");
					string temp = rtbTemp.Text;
					if (temp.StartsWith("<file>STATUS"))
					{
						string[] strs = temp.Substring(6).Split(new char[] {'|'});
						// 상대방 대화 입력 상태
						lock (statusLabel)
						{
							if (strs[1] == "1")
							{
								statusLabel.Tag = statusLabel.Text;
								statusLabel.Text = strU[1] + "님이 메시지를 작성하고 있습니다...";
							}
							else
							{
								if (statusLabel.Tag != null)
									statusLabel.Text = (string) statusLabel.Tag;
								else
									statusLabel.Text = "";
							}
						}
					}
					else
					{
						try
						{
							int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
							rtbMessage.Select(pos, 0);
							rtbMessage.ScrollToCaret();
						}
						catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
						{
						}

						processMsgReceive(strU[1]);

						try
						{
							rtbMessage.SelectedText = "\r\n";
							rtbMessage.ScrollToCaret();
						}
						catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
						{
						}

						if (loaded && (strU[1] != "Gro@up"))
						{
							try
							{
								if (WindowState == FormWindowState.Minimized)
								{
									NotifyWindow nw;
									RichTextBox rtb = new RichTextBox();
									rtb.Rtf = strM;
									if (rtb.Text.Trim().Length == 0)
									{
										nw = new NotifyWindow(strU[1], "Emotions");
										nw.SetDimensions(240, 77);
									}
									else
									{
										int iRw = (rtb.Text.Length / 30);
										iRw++;
										int iHt = (iRw * 25) + 52;
										nw = new NotifyWindow(strU[1], rtb.Text);
										nw.SetDimensions(240, iHt);
									}
									nw.TextClicked += notifyWindow_TextClicked;
									nw.Font = new Font("굴림", 9F);
									nw.Notify();
									rtb.Dispose();
								}
							}
							catch
							{
							}
						}
					}
				}

				lock (LOCK_OBJECT)
				{
					IN_GROUP_MESSAGE = IN_GROUP_MESSAGE.Replace(msg, "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
				}
			}
		}

		private void notifyWindow_TextClicked(object sender, TextClickedEventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		private void processMsgReceive(string remoteUser)
		{
			lock (statusLabel)
			{
				statusLabel.Text = "메시지를 받고 있습니다...(" + rtbTemp.Rtf.Length + "바이트)";
			}
			rtbMessage.SelectionColor = Color.Black;
			rtbMessage.SelectionFont = new Font("굴림", 9F);
			rtbMessage.SelectedText = remoteUser + "님의 말:\r\n";
			ChatLogger.Log("회의실{" + groupID + "} " + remoteUser + "님의 말: " + rtbTemp.Text);

			const string rep = @"\fs";
			string rtf = rtbTemp.Rtf;
			for (int size = 1; size < 255; size++)
			{
				if (rtf.IndexOf(rep + size) >= 0)
					rtf = rtf.Replace(rep + size + " ", rep + size + "  ").Replace(rep + size + @"\'", rep + size + @"  \'");
			}
			rtf = rtf.Replace("\\par\r\n", "\\par\r\n ").Replace("\r\n }", "\r\n}"); // '\n' --> '\n '
			rtbTemp.Rtf = rtf;

			rtbMessage.SelectedRtf = rtbTemp.Rtf;
			Application.DoEvents();
			lock (statusLabel)
			{
				statusLabel.Text = "마지막으로 메시지를 받은 시각: " + DateTime.Now.ToString("yyyy년 M월 d일 H시 m분 s초");
				statusLabel.Tag = statusLabel.Text;
			}
		}

		private void rtbWriteMsg_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.Escape)
				{
					e.Handled = true;
					Close();
				}
				else if (e.KeyCode == Keys.Enter)
				{
					e.Handled = true;

					if (e.Shift)
					{
						rtbWriteMsg.SelectedText = "\r\n";
					}
					else
					{
						processMsgSend();
					}
					isWriting = false;
				}
				else if ((e.KeyCode == Keys.V) && e.Control)
				{
					IDataObject obj = Clipboard.GetDataObject();
					if (obj != null)
					{
						processFileSend(obj, e);
					}
					isWriting = false;
				}
				else if (e.KeyCode == Keys.Up)
				{
					if (++view_msgidx > 9) view_msgidx = 0;
					string msg = prev_messages[view_msgidx];
					int cnt = 0;
					while ((msg == null) && (cnt < 10))
					{
						if (++view_msgidx > 9) view_msgidx = 0;
						msg = prev_messages[view_msgidx];
						cnt++;
					}
					if (msg != null) rtbWriteMsg.Rtf = msg;
				}
				else if (e.KeyCode == Keys.Down)
				{
					if (--view_msgidx < 0) view_msgidx = 9;
					string msg = prev_messages[view_msgidx];
					int cnt = 0;
					while ((msg == null) && (cnt < 10))
					{
						if (--view_msgidx < 0) view_msgidx = 9;
						msg = prev_messages[view_msgidx];
						cnt++;
					}
					if (msg != null) rtbWriteMsg.Rtf = msg;
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::rtbWriteMsg_KeyDown", ex);
				Logger.Log(Logger.LogLevel.ERROR, "rtbWriteMsg_KeyDown() - " + ex);
			}
		}

		private void rtbWriteMsg_TextChanged(object sender, EventArgs e)
		{
			processStatusSend();
		}

		private void btnFont_Click(object sender, EventArgs e)
		{
			DialogResult dr = fontDialog1.ShowDialog();
			if (dr == DialogResult.OK)
			{
				Font font = new Font(fontDialog1.Font.Name, fontDialog1.Font.Size, fontDialog1.Font.Style);
				string f = serialize(font);
				SettingsHelper.Current.Font = f;
				SettingsHelper.Current.Save();
				rtbWriteMsg.Font = font;
				rtbWriteMsg.Focus();
				font.Dispose();
			}
		}

		private void btnColor_Click(object sender, EventArgs e)
		{
			DialogResult dr = colorDialog1.ShowDialog();
			if (dr == DialogResult.OK)
			{
				string c = colorDialog1.Color.ToArgb().ToString();
				SettingsHelper.Current.Color = c;
				SettingsHelper.Current.Save();
				rtbWriteMsg.ForeColor = colorDialog1.Color;
				rtbWriteMsg.Focus();
			}
		}

		private static string serialize(Font theFont)
		{
			string name = theFont.FontFamily.Name;
			float size = theFont.Size;
			int style = (int) theFont.Style;

			StringBuilder sb = new StringBuilder();
			sb.Append(name).Append("|");
			sb.Append(size).Append("|");
			sb.Append(style);
			return sb.ToString();
		}

		private static Font deserialize(string fontString)
		{
			string[] parts = fontString.Split(new char[] {'|'});
			if (parts.Length < 3) return null;
			Font theFont = new Font(parts[0], float.Parse(parts[1]), (FontStyle) int.Parse(parts[2]));
			return theFont;
		}

		private void rtbMessage_SelectionChanged(object sender, EventArgs e)
		{
			string rtf = rtbMessage.SelectedRtf;
			// 보내온 파일 선택
			if (rtf.IndexOf("\\objdata") >= 0)
			{
				// 텍스트와 함께 선택(드래그)한 경우는 제외
				int pos = rtf.IndexOf(@"\fs");
				while (pos >= 0)
				{
					int num;
					if (int.TryParse(rtf.Substring(pos + 3, 2), out num))
					{
						string rep = rtf.Substring(pos, 5); // \fs18, \fs20, ...
						if ((rtf.IndexOf(rep + " ") >= 0) || (rtf.IndexOf(rep + @"\'") >= 0) || (rtf.IndexOf("\\par\r\n") >= 0)) return;
					}
					pos = rtf.IndexOf(@"\fs", ++pos);
				}

				Clipboard.Clear();
				rtbMessage.Copy();

				if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				{
					MemoryStream ms = Clipboard.GetData("Native") as MemoryStream;
					clpNativeObject obj = NativeObject.GetObject(ms);

					string filename;
					if (string.IsNullOrEmpty(obj.m_fileName))
						filename = obj.m_fullPath.Substring(obj.m_fullPath.LastIndexOf(@"\") + 1);
					else
						filename = obj.m_fileName;

					FileInfo fi = new FileInfo(folderBrowserDialog1.SelectedPath + @"\" + filename);
					using (FileStream fs = fi.Open(FileMode.Create, FileAccess.Write, FileShare.Read))
					{
						fs.Write(obj.m_data, 0, obj.m_fileSize);
						fs.Flush();
						fs.Close();
					}
				}
			}
		}

		private void GroupMessage_Resize(object sender, EventArgs e)
		{
			if (isLoaded)
			{
				/*
				rtbMessage.Width = this.Width - 11;
				rtbMessage.Height = this.Height - 138;

				rtbWriteMsg.Width = this.Width - 76;
				rtbWriteMsg.Top = this.Height - 112;

				btnEmotion.Top = btnColor.Top = btnFont.Top = this.Height - 134;
				panel1.Top = this.Height - 207;

				btnSend.Location = new Point(this.Width - 73, this.Height - 87);
				*/
				int top = splitContainer1.Height - 145;
				panel1.Top = (top < 0) ? 0 : top;
			}
		}

		private void GroupMessage_Activated(object sender, EventArgs e)
		{
			isLoaded = true;
			rtbWriteMsg.Focus();
		}

		private void rtbMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.Handled = true;
				Close();
			}
		}

		#region 이모티콘

		private void btnEmotion_Click(object sender, EventArgs e)
		{
			panel1.Visible = !panel1.Visible;
		}

		private void btnSmile_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btnSmile.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btnSad_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btnSad.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			panel1.Visible = false;
		}

		private void btn3_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn3.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn4_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn4.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn5_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn5.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn6_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn6.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn7_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn7.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn8_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn8.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn9_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn9.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn10_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn10.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn11_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn11.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn12_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn12.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn13_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn13.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn14_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn14.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn15_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn15.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn16_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn16.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn17_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn17.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn18_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn18.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn19_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn19.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn20_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn20.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn21_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn21.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		private void btn22_Click(object sender, EventArgs e)
		{
			Bitmap img = new Bitmap(btn22.BackgroundImage);
			Clipboard.Clear();
			Clipboard.SetDataObject(img);
			DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
			rtbWriteMsg.Paste(myFormat);
			panel1.Visible = false;
			img.Dispose();
		}

		#endregion

		private void trackBar_Scroll(object sender, EventArgs e)
		{
			this.Opacity = trackBar.Value / 100.0;
			SettingsHelper.Current.ChatOpacity = trackBar.Value;
		}


		#region UserState

		private class UserState
		{
			private readonly SEND_FLAG m_flag;
			private readonly string m_msg;

			public UserState(SEND_FLAG flag, string msg)
			{
				m_flag = flag;
				m_msg = msg;
			}

			public SEND_FLAG Flag
			{
				get { return m_flag; }
			}

			public string Msg
			{
				get { return m_msg; }
			}
		}

		#endregion
	}
}