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
			rtbMessage.SelectedText = "*ȸ�ǽǿ����� �̹����� ���� ÷�θ� ������ �������ֽñ� �ٶ��ϴ�.\r\n*��� �����ڵ��� ��ǻ�Ͱ� ������ �� �ֽ��ϴ�.\r\n\r\n";
			
			if (!autoJoin)
			{
				rtbMessage.SelectionColor = Color.Violet;
				rtbMessage.SelectedText = strUserName + "���� �����ϱ⸦ ��ٸ��� ���Դϴ�...\r\n\r\n";
			}

			// �� ȸ�ǽ� ���� ��� ���ϱ�
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

			toolTip1.SetToolTip(btnEmotion, "�̸�Ƽ�� ����");
			toolTip2.SetToolTip(btnColor, "�� ���� ����");
			toolTip3.SetToolTip(btnFont, "�۲� ����");

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
					Text = "��" + group.GroupNo + " ȸ�ǽ�";
					break;
				}
			}
			panel1.Visible = false;


			SettingsHelper helper = SettingsHelper.Current;

			if (!string.IsNullOrEmpty(helper.Font))
				rtbWriteMsg.Font = deserialize(helper.Font);

			if (!string.IsNullOrEmpty(helper.Color))
				rtbWriteMsg.ForeColor = Color.FromArgb(int.Parse(helper.Color));


			// ���� ����
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
				DialogResult dr = MessageBoxEx.Show("[" + Text + " ����]\r\n\r\n�� ȸ�ǽǿ��� �����ðڽ��ϱ�?", Text, MessageBoxButtons.YesNo,
									MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 20 * 1000);
				if (dr == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}

				SettingsHelper.Current.Save();
				this.Hide();

				// ���¸� ������Ʈ�Ѵ�.
				rtbTemp.Text = "<file>STATUS|0";
				groupChatSvc.SendGroupMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
											  rtbTemp.Rtf);
				isWriting = false;

				// ȸ�ǽ� ����
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

				// ȸ�ǽ� ����
				groupChatSvc.RemoveUserGroup(frmMain.LOGIN_INFO.LoginID, groupID);

				Dispose();

			}
			catch (System.Net.WebException ex)
			{
				// ��Ʈ��ũ �Ǵ� DNS ����
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[GroupMessage_FormClosing()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // �޽��� �ؼ� ����
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[GroupMessage_FormClosing()]: " + ex);
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

				// ������ ���� ǥ��
				if (frmMain.LOGIN_INFO.IsAbsent)
				{
					lblSelfStatus.Text = "*���� �ڸ� ��� �����Դϴ�.";
					lblSelfStatus.Visible = true;
				}
				else if (frmMain.LOGIN_INFO.IsBusy)
				{
					lblSelfStatus.Text = "*���� '�ٸ� �빫 ��' �����Դϴ�.";
					lblSelfStatus.Visible = true;
				}
				else
				{
					lblSelfStatus.Visible = false;
				}

				// ���� ����
				processStatusSend();

				for (int i = 0; i < lstUsers.Items.Count; i++)
				{
					NVItem item = (NVItem) lstUsers.Items[i];

					// ��ȭ��밡 ���������̸�
					if ((frmMain.CACHED_USERS.IndexOf("|" + item.Value + "|") < 0) &&
						(frmMain.CACHED_USERS.IndexOf("|*" + item.Value + "|") < 0) &&
						(frmMain.CACHED_USERS.IndexOf("|/" + item.Value + "|") < 0))
					{
						lstUsers.Items.Remove(item);
						i--;
					}
					else if (frmMain.CACHED_USERS.IndexOf("|*" + item.Value + "|") >= 0)
					{
						item.Text = item.Text + "(*�ڸ� ���)";
					}
					else if (frmMain.CACHED_USERS.IndexOf("|/" + item.Value + "|") >= 0)
					{
						item.Text = item.Text + "(*�ٸ� �빫 ��)";
					}
					else
					{
						item.Text = item.Text.Replace("(*�ڸ� ���)", "").Replace("(*�ٸ� �빫 ��)", "");
					}
				}
				lstUsers.Refresh();

				// �޽��� ����
				if (IN_GROUP_MESSAGE.Trim().Length == 0)
				{
					msgClearCount = 0;
					groupChatSvc.ReceiveGroupMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID);
				}
				else // ���� �޽����� ó������ �ʰ� ���� ������ �ǳʶڴ�.
				{
					msgClearCount++;
					if (msgClearCount > 3) // 3ȸ �̻� �޽����� �״�� ���������� ������ �����.
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
								   "timer1_Tick(): ���� �޽���[" + IN_GROUP_MESSAGE.Trim().Replace("\r\n", "").Replace("\n", "") + "]");
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
				catch // �񵿱� ó���� �����ϸ� �ǳʶڴ�.
				{
				}

				if (strTemp.Trim().Length > 0)
				{
					// �ٸ� ��ǻ�Ϳ��� �α��� ���� üũ
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
				// ��Ʈ��ũ �Ǵ� DNS ����
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // �޽��� �ؼ� ����
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[GroupMessage::groupChatSvc_ReceiveGroupMessageCompleted()]: " + ex);
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

				// �񵿱�� �����ٰ� ������ �߻��ϸ� ����� ��ȯ�Ѵ�.
				Thread.Sleep(200);
				try
				{
					groupChatSvc.SendGroupMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
					                              msg);
				}
				catch (Exception) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
				{
					try
					{
						int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
						rtbMessage.Select(pos, 0);
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}

					try
					{
						rtbMessage.SelectionColor = Color.DarkRed;
						rtbMessage.SelectedText = "�޽��� �۽ſ� �����߽��ϴ�. �ٽ� �õ��غ��ʽÿ�.\r\n\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
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
				catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
				{
				}

				if (rtbWriteMsg.Text == "/allusers") // ����� ����
				{
					showUserList(true);
				}
				else if (rtbWriteMsg.Text == "/loginusers") // ����� ����
				{
					showUserList(false);
				}
				else if (rtbWriteMsg.Text == "/allgroups") // ����� ����
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
						rtbMessage.SelectedText = "/cls �Ǵ� /clear : �޽��� â ����\r\n";
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = "����Ű ��/�� : ���� �Է� �޽��� �ݺ�\r\n";
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}
				}
				else if ((rtbWriteMsg.Rtf.IndexOf("<file>") < 0) && (rtbWriteMsg.Rtf.IndexOf(@"<\f1 file>") < 0))
				{
					string msg = rtbWriteMsg.Rtf.Replace(":", "%3A");
					groupChatSvc.SendGroupMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", groupID,
													   msg, new UserState(SEND_FLAG.MESSAGE, msg));

					// �ڸ���� ���
					bool isIt = false;
					for (int i = 0; i < lstUsers.Items.Count; i++)
					{
						NVItem item = (NVItem) lstUsers.Items[i];
						if (item.Text.IndexOf("(*�ڸ� ���)") > 0)
						{
							rtbMessage.SelectionColor = Color.DarkRed;
							rtbMessage.SelectedText = item.Text.Replace("(*�ڸ� ���)", "") + "���� ���� \"�ڸ� ���\" ���¶� �������� ���� �� �ֽ��ϴ�.\r\n";
							isIt = true;
						}
						else if (item.Text.IndexOf("(*�ٸ� �빫 ��)") > 0)
						{
							rtbMessage.SelectionColor = Color.DarkRed;
							rtbMessage.SelectedText = item.Text.Replace("(*�ٸ� �빫 ��)", "") + "���� ���� \"�ٸ� �빫 ��\" ���¶� �������� ���� �� �ֽ��ϴ�.\r\n";
							isIt = true;
						}
					}
					if (isIt)
					{
						rtbMessage.SelectedText = "\r\n";
					}

					rtbMessage.SelectionColor = Color.Black;
					rtbMessage.SelectionFont = new Font("����", 9F);
					rtbMessage.SelectedText = frmMain.LOGIN_INFO.UserName + "���� ��:\r\n";
					rtbTemp.Rtf = rtbWriteMsg.Rtf;
					ChatLogger.Log("ȸ�ǽ�{" + groupID + "} " + frmMain.LOGIN_INFO.UserName + "���� ��: " + rtbTemp.Text);

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
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
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
						rtbMessage.SelectedText = "===== ��ü ��� =====\r\n";
					}
					else
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = "===== �α��� ��� =====\r\n";
					}

					foreach (string s in list)
					{
						string[] user = s.Split(new char[] {'&'});
						if (user.Length < 4) continue;

						int status = int.Parse(user[3]);
						if (bFlag || (status == 1) || (status == 9) || (status == 2))
						{
							rtbMessage.SelectionColor = Color.Violet;
							rtbMessage.SelectionFont = new Font("����ü", 9F);
							rtbMessage.SelectedText = string.Format("{0}({1})\t{2}\t����: {3}\r\n",
							                                        user[0].PadLeft(6),
							                                        user[1].PadRight(9),
							                                        user[2].PadRight(15),
																	(status == 1) ? "�α���" : ((status == 9) ? "�ڸ����" : ((status == 2) ? "�ٸ� �빫 ��" : "�α׾ƿ�")));
						}
					}

					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::showUserList", ex);
				Logger.Log(Logger.LogLevel.ERROR, "����� ��� ��� �� ������ �߻��߽��ϴ�: " + ex);
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
					rtbMessage.SelectionFont = new Font("����ü", 9F);
					rtbMessage.SelectedText = "===== ȸ�ǽ� ��� =====\r\n";

					foreach (string s in list)
					{
						int ind = s.IndexOf("^");
						if (ind < 0) continue;
						string gID = s.Substring(0, ind);
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectionFont = new Font("����ü", 9F);
						rtbMessage.SelectedText = "ȸ�ǽ� ID={" + gID + "}\r\n";

						string s2 = s.Substring(ind + 1);
						string[] user = s2.Split(new char[] { '&' });
						foreach (string u in user)
						{
							rtbMessage.SelectionColor = Color.Violet;
							rtbMessage.SelectionFont = new Font("����ü", 9F);
							rtbMessage.SelectedText = string.Format(" - {0}\r\n", u);
						}
					}

					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("GroupMessage::showGroupList", ex);
				Logger.Log(Logger.LogLevel.ERROR, "ȸ�ǽ� ��� ��� �� ������ �߻��߽��ϴ�: " + ex);
			}
		}

		private void processFileSend(IDataObject data, object e)
		{
			// �̹��� ������ ��� ũ�� üũ
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
						rtbMessage.SelectedText = "* 640x480 ���� ū �̹����� ���Ϸ� ÷���Ͻñ� �ٶ��ϴ�.\r\n";
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
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

			// MS ���ǽ� ����(����, �׼�, �Ŀ�����Ʈ ��) ó��
			if ((bytes[0] == 0xd0) && (bytes[1] == 0xcf) && (bytes[2] == 0x11) && (bytes[3] == 0xe0))
				isOffice = true;

			// ���� ũ�Ⱑ 512KB�� �Ѱų� ���ǽ� �����̸�
			if ((fi.Length >= 1024 * 1024 / 2) // > 512KB
				|| isOffice)
			{
				if (e is DragEventArgs) ((DragEventArgs) e).Effect = DragDropEffects.None;
				if (e is KeyEventArgs) ((KeyEventArgs) e).Handled = true;

				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* ȸ�ǽǿ����� 512KB�� �Ѵ� ���� ������ �������� �ʽ��ϴ�.\r\n";
				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* ������ �����Ͽ� �����Ͻñ� �ٶ��ϴ�.\r\n";
				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* ���ǽ� ���չ����� ���۵��� �����Ƿ� �����Ͽ� �����Ͻñ� �ٶ��ϴ�.\r\n";
				try
				{
					rtbMessage.SelectedText = "\r\n";
					rtbMessage.ScrollToCaret();
				}
				catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
				{
				}

				rtbWriteMsg.Clear();
				rtbWriteMsg.Focus();
			}
			else
			{
				string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
				rtbWriteMsg.SelectionFont = new Font("����", 9F);
				rtbWriteMsg.SelectionColor = Color.Violet;
				rtbWriteMsg.SelectedText = fileName + "\r\n(�Ʒ� �������� Ŭ���Ͽ� �����Ͻʽÿ�. // ����Ŭ��: �ٷ� ����)\r\n";
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

				if (strU[1] == "Gro@up") // ȸ�ǽ� ����(��� �߰�/����) �޽���
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
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}

					if ((strM.IndexOf("�����̽��ϴ�") > 0) || // ����
					    (strM.IndexOf("�ź��߽��ϴ�") > 0)) // �ʴ� �ź�
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
					else // ����
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
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
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
						// ���� ��ȭ �Է� ����
						lock (statusLabel)
						{
							if (strs[1] == "1")
							{
								statusLabel.Tag = statusLabel.Text;
								statusLabel.Text = strU[1] + "���� �޽����� �ۼ��ϰ� �ֽ��ϴ�...";
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
						catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
						{
						}

						processMsgReceive(strU[1]);

						try
						{
							rtbMessage.SelectedText = "\r\n";
							rtbMessage.ScrollToCaret();
						}
						catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
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
									nw.Font = new Font("����", 9F);
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
				statusLabel.Text = "�޽����� �ް� �ֽ��ϴ�...(" + rtbTemp.Rtf.Length + "����Ʈ)";
			}
			rtbMessage.SelectionColor = Color.Black;
			rtbMessage.SelectionFont = new Font("����", 9F);
			rtbMessage.SelectedText = remoteUser + "���� ��:\r\n";
			ChatLogger.Log("ȸ�ǽ�{" + groupID + "} " + remoteUser + "���� ��: " + rtbTemp.Text);

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
				statusLabel.Text = "���������� �޽����� ���� �ð�: " + DateTime.Now.ToString("yyyy�� M�� d�� H�� m�� s��");
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
			// ������ ���� ����
			if (rtf.IndexOf("\\objdata") >= 0)
			{
				// �ؽ�Ʈ�� �Բ� ����(�巡��)�� ���� ����
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

		#region �̸�Ƽ��

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