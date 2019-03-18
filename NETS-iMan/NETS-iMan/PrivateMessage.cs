using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using NETS_iMan.chatWebsvc;
using NETS_iMan.iManWebsvc;
using Timer = System.Windows.Forms.Timer;

namespace NETS_iMan
{
	public partial class PrivateMessage : Form
	{
		private readonly ChatService chatSvc = new ChatService();
		private readonly object lockObject = new object();
		private readonly RichTextBox rtbTemp = new RichTextBox();

		private readonly ToolTip toolTip1 = new ToolTip();
		private readonly ToolTip toolTip2 = new ToolTip();
		private readonly ToolTip toolTip3 = new ToolTip();
		private readonly ToolTip toolTip4 = new ToolTip();

		private ArrayList arrList;
		private ArrayList arrMsgs;

		private ClientSocket p2pSock;
		private ClientSocket relaySock;
		private bool isLoaded;
		private bool isWriting;
		private long m_elapsedSize;
		private FileInfo m_fileReceive;
		private FileInfo m_fileSend;
		private FileStream m_fsReceive;
		private FileStream m_fsSend;
		private SOCKET_MODE m_mode;
		private long m_prevSndStatus;
		private int m_retryCount;
		private long m_totalSizeReceive;
		private long m_totalSizeSend;
		private long m_SndStatus;
		private int m_PercentElapsed;
		private long m_Speed;
		private Timer msgTimer;
		private string remoteID;
		private string remoteIP = string.Empty;
		private string remoteUserName;
		private Timer statusTimer;

		private string[] prev_messages;
		private int prev_msgidx;
		private int view_msgidx;

		private volatile int ERROR_COUNT;

		private enum SEND_FLAG
		{
			STATUS,
			MESSAGE,
			FILE
		}

		private DateTime m_rcvTime;
		private int m_rcvTimeCount;
		private long m_rcvTimeSeg;
		private long m_rcvSize1, m_rcvSize2, m_rcvSize3, m_rcvSize4, m_rcvSize5, m_rcvSize6, m_rcvSize7, m_rcvSize8, m_rcvSize9, m_rcvSize10;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NETS_iMan.PrivateMessage"/> class.
		/// </summary>
		/// <param name="strID">The STR ID.</param>
		/// <param name="strUserName">Name + ( ID )</param>
		public PrivateMessage(string strID, string strUserName)
		{
			InitializeComponent();
			init(strID, strUserName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NETS_iMan.PrivateMessage"/> class.
		/// </summary>
		/// <param name="strID">The STR ID.</param>
		/// <param name="strUserName">Name + ( ID )</param>
		/// <param name="strMsg">The STR MSG.</param>
		public PrivateMessage(string strID, string strUserName, string strMsg)
		{
			try
			{
				InitializeComponent();
				init(strID, strUserName);

				processReceive(strMsg, false);
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("PrivateMessage ������", ex);
				Logger.Log(Logger.LogLevel.ERROR, "PrivateMessage() - " + ex);
			}
		}

		private void init(string strID, string strUserName)
		{
			remoteID = strID;
			remoteUserName = strUserName;

			picUser.Visible = true;
			toolTip4.SetToolTip(picUser, strUserName + " ������ ����");

			ThreadStart showPhoto = showPhotoProc;
			Thread showPhotoThread = new Thread(showPhoto);
			showPhotoThread.Start();

			arrList = new ArrayList();
			arrMsgs = new ArrayList();

			prev_messages = new string[] { null, null, null, null, null, null, null, null, null, null };
			view_msgidx = prev_msgidx = 9;

			toolTip1.SetToolTip(btnEmotion, "�̸�Ƽ�� ����");
			toolTip2.SetToolTip(btnColor, "�� ���� ����");
			toolTip3.SetToolTip(btnFont, "�۲� ����");

			rtbWriteMsg.DragDrop += Object_DragDrop;

			chatSvc.SendMessageCompleted += chatSvc_SendMessageCompleted;
		}

		private delegate void showPhotoDelegate();
		private void showPhotoProc()
		{
			if (this.InvokeRequired)
			{
				showPhotoDelegate d = showPhotoProc;
				this.Invoke(d);
			}
			else
			{
				DirService dirSvc = new DirService();
				try
				{
					string userXml = dirSvc.GetUserInfo(remoteID);

					XmlDocument doc = new XmlDocument();
					doc.LoadXml(userXml);
					XmlNode node = doc.SelectSingleNode("/user");
					if (node != null)
					{
						string title = node.Attributes["title"].InnerText;
						if (title.Trim().Length == 0) title = "���";
						string name = node.Attributes["name"].InnerText;
						lblName.Text = (name.Trim().Length > 0) ? title + " " + name : "(��٣)";
						lblMobile.Text = node.Attributes["mobile"].InnerText;
					}

					using (
						Stream stream = WebCall.GetStream(new Uri("http://sso.nets.co.kr/iManService/getPhoto.aspx?uid=" + remoteID), ""))
					{
						Image img = Image.FromStream(stream);
						picUser.Tag = img;
						picUser.Image = setImageOpacity(img, 0.85F);
						stream.Close();
					}
				}
				catch
				{
					try
					{
						string userXml = dirSvc.GetTempUserInfo(remoteID);

						XmlDocument doc = new XmlDocument();
						doc.LoadXml(userXml);
						XmlNode node = doc.SelectSingleNode("/user");
						if (node != null)
						{
							string title = node.Attributes["title"].InnerText;
							if (title.Trim().Length == 0) title = "���";
							string name = node.Attributes["name"].InnerText;
							lblName.Text = (name.Trim().Length > 0) ? title + " " + name : "(��٣)";
							lblMobile.Text = node.Attributes["mobile"].InnerText;
						}

						using (
							Stream stream = WebCall.GetStream(new Uri("http://sso.nets.co.kr/iManService/getPhoto.aspx?tempid=" + remoteID),
							                                  ""))
						{
							Image img = Image.FromStream(stream);
							picUser.Tag = img;
							picUser.Image = setImageOpacity(img, 0.85F);
							stream.Close();
						}
					}
					catch
					{
						picUser.Visible = false;
					}
				}
			}
		}

		private static Image setImageOpacity(Image imgPic, float imgOpac)
		{
			Bitmap bmpPic = new Bitmap(imgPic.Width, imgPic.Height);
			Graphics gfxPic = Graphics.FromImage(bmpPic);
			ColorMatrix cmxPic = new ColorMatrix();
			cmxPic.Matrix33 = imgOpac;

			ImageAttributes iaPic = new ImageAttributes();
			iaPic.SetColorMatrix(cmxPic, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			gfxPic.DrawImage(imgPic, new Rectangle(0, 0, bmpPic.Width, bmpPic.Height), 0, 0, imgPic.Width, imgPic.Height,
							 GraphicsUnit.Pixel, iaPic);
			gfxPic.Dispose();

			return bmpPic;
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
				ErrorReport.SendReport("PrivateMessage::btnSend_Click", ex);
				Logger.Log(Logger.LogLevel.ERROR, "btnSend_Click() - " + ex);
			}
		}

		private static bool isObjectEmbeded(string rtf)
		{
			return (rtf.IndexOf(@"\pic") >= 0) || (rtf.IndexOf(@"\objdata") >= 0);
		}

		private void PrivateMessage_Load(object sender, EventArgs e)
		{
			isLoaded = false;
			timer1.Enabled = true;
			Text = remoteUserName + "�԰��� ��ȭ";
			panel1.Visible = false;


			SettingsHelper helper = SettingsHelper.Current;

			if (!string.IsNullOrEmpty(helper.Font))
				rtbWriteMsg.Font = deserialize(helper.Font);

			if (!string.IsNullOrEmpty(helper.Color))
				rtbWriteMsg.ForeColor = Color.FromArgb(int.Parse(helper.Color));


			// ���� ����
			this.Opacity = helper.ChatOpacity/100.0;
			trackBar.Value = helper.ChatOpacity;

			rtbWriteMsg.Focus();
		}

		private void Object_DragDrop(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
			processFileSend(e.Data, e);
		}

		private void PrivateMessage_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				// ���� ���� ��⸦ ��� ����Ѵ�.
				cancelFileTranfer(null, true);

				SettingsHelper.Current.Save();
				this.Hide();

				// ���¸� ������Ʈ�Ѵ�.
				rtbTemp.Text = "<file>STATUS|0";
				chatSvc.SendMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
									rtbTemp.Rtf);
				isWriting = false; // Ű �Է��� ������

				// â�� �ݱ� ���� ��ȭ���κ��� �� �޽����� �� �����.
				if (frmMain.IN_MESSAGE.Trim().Length > 0)
				{
					string[] msgs = frmMain.IN_MESSAGE.Split(new string[] { "\n\n\t\n" }, StringSplitOptions.RemoveEmptyEntries);

					for (int k = 0; k < msgs.Length; k++)
					{
						processClear(msgs[k]);
					}
				}

				// ���� ����
				cleanUp();

				// �޽��� �ޱ� ��
				if (msgTimer != null) msgTimer.Stop();
				msgTimer = null;

				// ��ȭ ��� ����
				lock (frmMain.CHATTING_USERS)
				{
					frmMain.CHATTING_USERS.Remove(remoteUserName);
				}
				Dispose();
			}
			catch (System.Net.WebException ex)
			{
				// ��Ʈ��ũ �Ǵ� DNS ����
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[PrivateMessage_FormClosing()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // �޽��� �ؼ� ����
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[PrivateMessage_FormClosing()]: " + ex);
					}
				}
				else
				{
					ErrorReport.SendReport("PrivateMessage_FormClosing", ex);
					Logger.Log(Logger.LogLevel.ERROR, "PrivateMessage_FormClosing(): " + ex);
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("PrivateMessage_FormClosing", ex);
				Logger.Log(Logger.LogLevel.ERROR, "PrivateMessage_FormClosing(): " + ex);
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
					lblSelfStatus.Text = "*���� '�ڸ� ���' �����Դϴ�.";
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

				// ��ȭ��밡 ���������̸�
				if ((frmMain.CACHED_USERS.IndexOf("|" + remoteID + "|") < 0) &&
					(frmMain.CACHED_USERS.IndexOf("|*" + remoteID + "|") < 0) &&
					(frmMain.CACHED_USERS.IndexOf("|/" + remoteID + "|") < 0))
				{
					Text = remoteUserName + "��(*��������)���� ��ȭ";
					rtbWriteMsg.Enabled = false;
					btnSend.Enabled = false;
				}
				else if (frmMain.CACHED_USERS.IndexOf("|*" + remoteID + "|") >= 0)
				{
					Text = remoteUserName + "��(*�ڸ� ���)���� ��ȭ";
					rtbWriteMsg.Enabled = true;
					btnSend.Enabled = true;
				}
				else if (frmMain.CACHED_USERS.IndexOf("|/" + remoteID + "|") >= 0)
				{
					Text = remoteUserName + "��(*�ٸ� �빫 ��)���� ��ȭ";
					rtbWriteMsg.Enabled = true;
					btnSend.Enabled = true;
				}
				else
				{
					Text = remoteUserName + "�԰��� ��ȭ";
					if (!btnSend.Enabled)
					{
						rtbWriteMsg.Enabled = true;
						btnSend.Enabled = true;
					}
				}

				if (frmMain.IN_MESSAGE.Trim().Length > 0)
				{
					string[] msgs = frmMain.IN_MESSAGE.Split(new string[] { "\n\n\t\n" }, StringSplitOptions.RemoveEmptyEntries);

					for (int k = 0; k < msgs.Length; k++)
					{
						processReceive(msgs[k], true);
					}
				}
				ERROR_COUNT = 0;
			}
			catch (System.Net.WebException ex)
			{
				// ��Ʈ��ũ �Ǵ� DNS ����
				if (ERROR_COUNT < 5)
				{
					ERROR_COUNT++;
					Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[PrivateMessage::timer1_Tick()]: " + ex);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.IndexOf("text/html") >= 0) // �޽��� �ؼ� ����
				{
					if (ERROR_COUNT < 5)
					{
						ERROR_COUNT++;
						Logger.Log(Logger.LogLevel.WARNING, "��Ʈ��ũ ����[PrivateMessage::timer1_Tick()]: " + ex);
					}
				}
				else
				{
					ErrorReport.SendReport("PrivateMessage::timer1_Tick", ex);
					Logger.Log(Logger.LogLevel.ERROR, "PrivateMessage::timer1_Tick(): " + ex);
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("PrivateMessage::timer1_Tick", ex);
				Logger.Log(Logger.LogLevel.ERROR, "PrivateMessage::timer1_Tick() - " + ex);
			}
		}

		void chatSvc_SendMessageCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			try
			{
				if (!e.Cancelled)
				{
					if (e.Error == null) return;
				}

				UserState state = (UserState) e.UserState;
				if (state.Flag == SEND_FLAG.STATUS) return;
				string msg = state.Msg;

				// �񵿱�� �����ٰ� ������ �߻��ϸ� ����� ��ȯ�Ѵ�.
				Thread.Sleep(200);
				try
				{
					chatSvc.SendMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
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
				Logger.Log(Logger.LogLevel.WARNING, "PrivateMessage::chatSvc_SendMessageCompleted() - " + ex);
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
					chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
					                         msg, new UserState(SEND_FLAG.STATUS, msg));
					isWriting = true; // Ű �Է��� ���۵Ǿ���
				}
			}
			else
			{
				if (isWriting)
				{
					rtbTemp.Text = "<file>STATUS|0";
					string msg = rtbTemp.Rtf;
					chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
					                         msg, new UserState(SEND_FLAG.STATUS, msg));
					isWriting = false; // Ű �Է��� ������
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

				if (rtbWriteMsg.Text == "/queryip") // ����� ����
				{
					try
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = remoteUserName + "���� IP�ּ�: " + (string.IsNullOrEmpty(remoteIP) ? "(�� �� ����)" : remoteIP) +
												  "\r\n\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}
				}
				else if (rtbWriteMsg.Text == "/allusers") // ����� ����
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
						rtbMessage.SelectedText = "/queryip : ���� IP�ּ� ��ȸ\r\n";
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
					chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
										msg, new UserState(SEND_FLAG.MESSAGE, msg));

					// �ڸ���� ���
					if (Text.IndexOf("(*�ڸ� ���)") > 0)
					{
						rtbMessage.SelectionColor = Color.DarkRed;
						rtbMessage.SelectedText = remoteUserName + "���� ���� \"�ڸ� ���\" ���¶� �������� ���� �� �ֽ��ϴ�.\r\n\r\n";
					}
					else if (Text.IndexOf("(*�ٸ� �빫 ��)") > 0)
					{
						rtbMessage.SelectionColor = Color.DarkRed;
						rtbMessage.SelectedText = remoteUserName + "���� ���� \"�ٸ� �빫 ��\" ���¶� �������� ���� �� �ֽ��ϴ�.\r\n\r\n";
					}

					rtbMessage.SelectionColor = Color.Black;
					rtbMessage.SelectionFont = new Font("����", 9F);
					rtbMessage.SelectedText = frmMain.LOGIN_INFO.UserName + "���� ��:\r\n";
					rtbTemp.Rtf = rtbWriteMsg.Rtf;
					ChatLogger.Log(remoteUserName + "�Կ��� �� ��: " + rtbTemp.Text);

					const string rep = @"\fs";
					string rtf = rtbWriteMsg.Rtf;
					for (int size = 1; size < 255; size++)
					{
						if (rtf.IndexOf(rep + size) >= 0)
							rtf = rtf.Replace(rep + size + " ", rep + size + "  ").Replace(rep + size + @"\'", rep + size + @"  \'");
					}
					rtf = rtf.Replace("\\par\r\n", "\\par\r\n ").Replace("\r\n }", "\r\n}"); // '\n' --> '\n '
					rtbWriteMsg.Rtf = rtf;
					rtbMessage.SelectedRtf = rtbWriteMsg.Rtf;

					try
					{
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
				string[] list = users.Split(new char[] { '^' });
				if (list.Length > 0)
				{
					if (bFlag)
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectionFont = new Font("����ü", 9F);
						rtbMessage.SelectedText = "===== ��ü ��� =====\r\n";
					}
					else
					{
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectionFont = new Font("����ü", 9F);
						rtbMessage.SelectedText = "===== �α��� ��� =====\r\n";
					}

					foreach (string s in list)
					{
						string[] user = s.Split(new char[] { '&' });
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
				ErrorReport.SendReport("PrivateMessage::showUserList", ex);
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
				ErrorReport.SendReport("PrivateMessage::showGroupList", ex);
				Logger.Log(Logger.LogLevel.ERROR, "ȸ�ǽ� ��� ��� �� ������ �߻��߽��ϴ�: " + ex);
			}
		}

		private void processFileSend(IDataObject data, object e)
		{
			// �̹��� ������ ��� ũ�� üũ
			if ((data.GetData("Bitmap") is Bitmap))
			{
				MemoryStream ms = data.GetData("DeviceIndependentBitmap") as MemoryStream;
				if ((ms != null) && (ms.Length > 800 * 600 * 3))
				{
					if (e is DragEventArgs) ((DragEventArgs)e).Effect = DragDropEffects.None;
					if (e is KeyEventArgs) ((KeyEventArgs)e).Handled = true;

					try
					{
						rtbMessage.SelectionColor = Color.DarkRed;
						rtbMessage.SelectedText = "* 800x600 ���� ū �̹����� ���Ϸ� ÷���Ͻñ� �ٶ��ϴ�.\r\n";
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}
					return;
				}
			}

			string[] paths = (string[])data.GetData("FileDrop");
			if (paths == null) return;

			// ���� ������ �ϳ����� ����Ѵ�.
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

			// ���� ũ�Ⱑ 1MB�� �Ѱų� ���ǽ� �����̸� ��Ʈ������ ��ٷ� ����
			if ((fi.Length >= 1 * 1024 * 1024) // > 1MB
				|| isOffice)
			{
				if (e is DragEventArgs) ((DragEventArgs)e).Effect = DragDropEffects.None;
				if (e is KeyEventArgs) ((KeyEventArgs)e).Handled = true;

				rtbTemp.Text = "<file>QUERY|" + fi.FullName.Replace(":", "%3A") + "|" + fi.Length;
				string msg = rtbTemp.Rtf;
				chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
				                         msg, new UserState(SEND_FLAG.FILE, msg));
				try
				{
					int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
					rtbMessage.Select(pos, 0);
					rtbMessage.ScrollToCaret();
				}
				catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
				{
				}
				rtbMessage.SelectedText = remoteUserName + "�Կ��� " + fi.FullName + " ������ �����Ϸ��� �մϴ�...";
				ChatLogger.Log(remoteUserName + "�Կ��� ���Ϻ�����: " + fi.FullName + "|" + fi.Length);

				rtbMessage.InsertLink("���", rtbMessage.TextLength + "|" + fi.FullName.Replace(@"\", @"\\") + "|");

				if (msgTimer == null)
				{
					msgTimer = new Timer();
					msgTimer.Interval = 250;
					msgTimer.Tick += msgTimer_Tick;
					msgTimer.Start();
				}

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
				string fileName = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
				rtbWriteMsg.SelectionFont = new Font("����", 9F);
				rtbWriteMsg.SelectionColor = Color.Violet;
				rtbWriteMsg.SelectedText = fileName + "\r\n(�Ʒ� �������� Ŭ���Ͽ� �����Ͻʽÿ�. // ����Ŭ��: �ٷ� ����)\r\n";
			}
		}

		private void processReceive(string msg, bool loaded)
		{
			string[] strU = msg.Split(':'); // 0: remoteuser, 1: remoteIP, 2: msg
			string strM = string.Empty;
			if (strU[0] == remoteUserName)
			{
				if (remoteIP != strU[1]) remoteIP = strU[1];
				for (int i = 2; i < strU.Length; i++)
				{
					strM += strU[i];
				}

				rtbTemp.Rtf = strM.Replace("%3A", ":");
				string temp = rtbTemp.Text;
				if (temp.StartsWith("<file>STATUS"))
				{
					string[] strs = temp.Substring(6).Split(new char[] { '|' });
					// ���� ��ȭ �Է� ����
					lock (statusLabel)
					{
						if (strs[1] == "1")
						{
							statusLabel.Tag = statusLabel.Text;
							statusLabel.Text = strU[0] + "���� �޽����� �ۼ��ϰ� �ֽ��ϴ�...";
						}
						else
						{
							if (statusLabel.Tag != null)
								statusLabel.Text = (string)statusLabel.Tag;
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

					if (temp.StartsWith("<file>"))
					{
						processFileReceive(strU[0]);
					}
					else
					{
						processMsgReceive(strU[0]);
					}

					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}

					if (loaded)
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
									nw = new NotifyWindow(strU[0], "Emotions");
									nw.SetDimensions(240, 77);
								}
								else
								{
									int iRw = (rtb.Text.Length / 30);
									iRw++;
									int iHt = (iRw * 25) + 52;
									nw = new NotifyWindow(strU[0], rtb.Text);
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

					int cnt = 0;
					while (cnt < 3)
					{
						if (!chkOffSound.Checked) SoundPlayer.PlaySound("newmsg.wav");
						if (NIUtil.FlashWindowEx(this)) break;
						cnt++;
						Thread.Sleep(500);
					}
				}

				lock (frmMain.LOCK_OBJECT)
				{
					frmMain.IN_MESSAGE = frmMain.IN_MESSAGE.Replace(msg, "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
				}
			}
		}

		private void processClear(string msg)
		{
			string[] strU = msg.Split(':'); // 0: remoteuser, 1: remoteIP, 2: msg
			if (strU[0] == remoteUserName)
			{
				lock (frmMain.LOCK_OBJECT)
				{
					frmMain.IN_MESSAGE = frmMain.IN_MESSAGE.Replace(msg, "<bof>").Replace("<bof>\n\n\t\n", "").Replace("<bof>", "");
				}
			}
		}

		private void notifyWindow_TextClicked(object sender, TextClickedEventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		private void processFileReceive(string remoteUser)
		{
			// <file>{code}|{filename}|{filesize}�Ǵ�{IP�ּ�}
			string[] strs = rtbTemp.Text.Substring(6).Split(new char[] { '|' });
			if (strs.Length < 2)
			{
				frmMain.IN_MESSAGE = "";
				return;
			}

			string fullName = strs[1];
			string fileName = fullName.Substring(fullName.LastIndexOf(@"\") + 1);
			if (strs[0] == "QUERY") // ���� ���� ��û ����
			{
				rtbMessage.SelectionFont = new Font("����", 9F);
				rtbMessage.SelectionColor = Color.Violet;
				rtbMessage.SelectedText = remoteUser + "���� " + fileName + " ����(" + long.Parse(strs[2]).ToString("###,###,###,###") +
										  "����Ʈ)�� �����Ϸ��� �մϴ�. �����Ͻðڽ��ϱ�? ";
				ChatLogger.Log(remoteUser + "���� " + fileName + " ����(" + long.Parse(strs[2]).ToString("###,###,###,###") +
							   "����Ʈ)�� �����Ϸ��� �մϴ�. �����Ͻðڽ��ϱ�?");

				int pos = rtbMessage.TextLength;
				rtbMessage.InsertLink("����", pos + "|" + fullName.Replace(@"\", @"\\") + "|" + strs[2]);
				rtbMessage.SelectedText = " ";
				rtbMessage.InsertLink("�ź�", pos + "|" + fullName.Replace(@"\", @"\\") + "|");

				lock (statusLabel)
				{
					statusLabel.Text = "���������� �޽����� ���� �ð�: " + DateTime.Now.ToString("yyyy�� M�� d�� H�� m�� s��");
					statusLabel.Tag = statusLabel.Text;
				}
			}
			else if (strs[0] == "OK") // ������ ���ϼ��� ����
			{
				if (strs.Length == 4)
				{
					string prvIP = strs[2];
					if (NIUtil.IsSameNetworkWithMine(prvIP))
						startFileSend(strs[2], fullName);
					else
						startFileSend(strs[3], fullName);
				}
				else
					startFileSend(strs[2], fullName);
			}
			else if (strs[0] == "DENY") // ������ ���ϼ��� �ź�
			{
				remoteCancel(fullName);
				rtbMessage.SelectionFont = new Font("����", 9F);
				rtbMessage.SelectionColor = Color.Violet;
				rtbMessage.SelectedText = remoteUser + "���� ���������� �ź��߽��ϴ�.\r\n";
				lock (statusLabel)
				{
					statusLabel.Text = "���������� �޽����� ���� �ð�: " + DateTime.Now.ToString("yyyy�� M�� d�� H�� m�� s��");
					statusLabel.Tag = statusLabel.Text;
				}
				ChatLogger.Log(remoteUser + "���� ���������� �ź��߽��ϴ�.");
			}
			else if (strs[0] == "CANCEL") // ������ ���ϼ۽� ���
			{
				remoteCancel(fullName);
				rtbMessage.SelectionFont = new Font("����", 9F);
				rtbMessage.SelectionColor = Color.Violet;
				rtbMessage.SelectedText = remoteUser + "���� ���������� ����߽��ϴ�.\r\n";
				lock (statusLabel)
				{
					statusLabel.Text = "���������� �޽����� ���� �ð�: " + DateTime.Now.ToString("yyyy�� M�� d�� H�� m�� s��");
					statusLabel.Tag = statusLabel.Text;
				}
				ChatLogger.Log(remoteUser + "���� ���������� ����߽��ϴ�.");
			}
			else if (strs[0] == "REDIRECTION") // ���Ϲޱ� ���� ���� ����
			{
				m_mode = SOCKET_MODE.FILE_RECEIVE_SERVER;

				relaySock = new ClientSocket("61.74.137.10", 443);
				relaySock.OnConnect += relaySock_OnConnect;
				relaySock.OnRead += relaySock_OnRead;
				relaySock.OnError += relaySock_OnError;
				relaySock.Connect();
			}

			if (msgTimer == null)
			{
				msgTimer = new Timer();
				msgTimer.Interval = 250;
				msgTimer.Tick += msgTimer_Tick;
				msgTimer.Start();
			}
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
			ChatLogger.Log(remoteUser + "���� ��: " + rtbTemp.Text);

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
				if (e.KeyCode == Keys.Escape) // â �ݱ�
				{
					e.Handled = true;
					Close();
				}
				else if (e.KeyCode == Keys.Enter) // ����Ű
				{
					e.Handled = true;

					if (e.Shift) // �ٹٲ�
					{
						rtbWriteMsg.SelectedText = "\r\n";
					}
					else
					{
						processMsgSend();
					}
					isWriting = false;
				}
				else if ((e.KeyCode == Keys.V) && e.Control) // �ٿ��ֱ�
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
				ErrorReport.SendReport("PrivateMessage::rtbWriteMsg_KeyDown", ex);
				Logger.Log(Logger.LogLevel.ERROR, "rtbWriteMsg_KeyDown() - " + ex);
			}
		}

		private void rtbWriteMsg_TextChanged(object sender, EventArgs e)
		{
			processStatusSend();
		}

		private void msgTimer_Tick(object sender, EventArgs e)
		{
			if (arrList.Count == 0) return;

			for (int i = 0; i < arrList.Count; i++)
			{
				object obj = arrList[i];
				if (obj is ProgressBar)
				{
					ProgressBar pb = (ProgressBar)obj;
					pb.Value = m_PercentElapsed;
					if (pb.Value >= 100)
					{
						pb.Width = 1;
						pb.Height = 1;
						pb.Tag = null;
						rtbMessage.UpdateObjects();

						lock (arrList)
						{
							arrList.RemoveAt(i--);
						}
						pb.Dispose();

						rtbMessage.SelectedText = " ������ �Ϸ�Ǿ����ϴ�.\r\n";
						rtbMessage.Select(rtbMessage.TextLength, 0);
						try
						{
							rtbMessage.SelectedText = "\r\n";
							rtbMessage.ScrollToCaret();
						}
						catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
						{
						}

						lock (statusLabel)
						{
							statusLabel.Text = "���������� �޽����� ���� �ð�: " + DateTime.Now.ToString("yyyy�� M�� d�� H�� m�� s��");
							statusLabel.Tag = statusLabel.Text;
						}
					}
				}
				else if (obj is Label)
				{
					Label label = (Label)obj;
					label.Text = m_PercentElapsed + "% (" + m_Speed.ToString("###,##0") + " KB/s)";
					if (m_PercentElapsed >= 100)
					{
						label.Text = "";
						label.Width = 1;
						label.Height = 1;
						label.Tag = null;
						rtbMessage.UpdateObjects();

						lock (arrList)
						{
							arrList.RemoveAt(i--);
						}
						label.Dispose();
					}
				}
			}
			rtbMessage.UpdateObjects();
		}

		/// <summary>
		/// ���� ���� ����
		/// </summary>
		private void startFileSend(string targetIP, string filePath)
		{
			remoteCancel(filePath);

			m_mode = SOCKET_MODE.FILE_SEND;
			m_fileSend = new FileInfo(filePath);
			m_totalSizeSend = m_fileSend.Length;

			try
			{
				lock (lockObject)
				{
					m_fsSend = m_fileSend.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				}
			}
			catch (IOException ex)
			{
				cancelFileTranfer(filePath, true);

				m_mode = SOCKET_MODE.NONE;
				m_fileSend = null;
				m_totalSizeSend = -1;

				Logger.Log(Logger.LogLevel.ERROR, "startFileSend() - ��ҵ�: " + ex);
				MessageBoxEx.Show("���� ���� �� ������ ������ �߻��߽��ϴ�: " + ex.Message + "\r\n�ٽ� Ȯ���Ͻð� �����Ͻñ� �ٶ��ϴ�.",
								  "���� ����",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  30000);
				return;
			}

			rtbMessage.SelectionFont = new Font("����", 9F);
			rtbMessage.SelectionColor = Color.Violet;
			rtbMessage.SelectedText = " ���ϸ�: " + filePath.Substring(filePath.LastIndexOf(@"\") + 1) + " ";

			// ���� �۽���
			ProgressBar pb = new ProgressBar();
			pb.Maximum = 100;
			pb.Minimum = 0;
			pb.Style = ProgressBarStyle.Continuous;
			pb.Height = 15;
			pb.Value = 0;
			pb.Tag = filePath;
			lock (arrList)
			{
				arrList.Add(pb);
			}
			rtbMessage.InsertControl(pb);

			Label label = new Label();
			label.TextAlign = ContentAlignment.MiddleLeft;
			label.BackColor = Color.White;
			label.ForeColor = Color.Blue;
			label.Text = "0% (0 KB/s)";
			label.Width = 130;
			label.Height = 15;
			label.Tag = filePath;
			lock (arrList)
			{
				arrList.Add(label);
			}
			rtbMessage.InsertControl(label);
			rtbMessage.UpdateObjects();
			rtbMessage.SelectedText = "\r\n";

			// ���°�: ���=0, ������=���۹���Ʈ��
			m_SndStatus = m_prevSndStatus = 0;
			statusTimer = new Timer();
			statusTimer.Interval = 5000;
			statusTimer.Tick += statusTimer_Tick;
			statusTimer.Start();

			p2pSock = new ClientSocket(targetIP, 6421);
			p2pSock.OnConnect += clientSock_OnConnect;
			p2pSock.OnError += clientSock_OnError;
			p2pSock.Connect();
		}

		private void sendviaServer()
		{
			if (m_SndStatus == m_prevSndStatus)
			{
				if ((m_SndStatus == 0) && (m_mode == SOCKET_MODE.FILE_SEND)) // ������ Ÿ�Ӿƿ�
				{
					rtbTemp.Text = "<file>REDIRECTION|" + m_fileSend.FullName.Replace(":", "%3A");
					string msg = rtbTemp.Rtf;
					chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
					                         msg, new UserState(SEND_FLAG.FILE, msg));

					m_mode = SOCKET_MODE.FILE_SEND_SERVER;
					m_retryCount = 0;

					if (p2pSock != null)
					{
						p2pSock.Disconnect();
						p2pSock = null;
					}

					// ������ ���� ������ �Ϸ��� ������ 2.5�ʰ� ����Ѵ�.
					Thread.Sleep(2500);

					// ������ �����Ѵ�.
					relaySock = new ClientSocket("61.74.137.10", 443);
					relaySock.OnConnect += relaySock_OnConnect;
					relaySock.OnRead += relaySock_OnRead;
					relaySock.OnError += relaySock_OnError;
					relaySock.Connect();

					return;
				}

				if ((m_SndStatus == 1) && (m_mode == SOCKET_MODE.FILE_SEND_SERVER))
				{
					m_retryCount++;

					if (m_retryCount < 3)
					{
						if (p2pSock != null)
						{
							p2pSock.Disconnect();
							p2pSock = null;
						}

						// ������ ���� ������ �Ϸ��� ������ 1.5�ʰ� ����Ѵ�.
						Thread.Sleep(1500);

						// ������ �����Ѵ�.
						relaySock = new ClientSocket("61.74.137.10", 443);
						relaySock.OnConnect += relaySock_OnConnect;
						relaySock.OnRead += relaySock_OnRead;
						relaySock.OnError += relaySock_OnError;
						relaySock.Connect();
						return;
					}
				}

				// ���� ó���Ѵ�.
				for (int i = 0; i < arrList.Count; i++)
				{
					object obj = arrList[i];
					// ���� �۽���
					if (obj is ProgressBar)
					{
						ProgressBar pb = (ProgressBar)obj;
						if (m_fileSend.FullName == (string)pb.Tag)
						{
							rtbTemp.Text = "<file>CANCEL|" + ((string)pb.Tag).Replace(":", "%3A");
							string msg = rtbTemp.Rtf;
							chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
							                         msg, new UserState(SEND_FLAG.FILE, msg));

							lock (arrList)
							{
								arrList.RemoveAt(i);
							}
							pb.Dispose();
							rtbMessage.UpdateObjects();

							rtbMessage.SelectedText = " ���������� �����߽��ϴ�.\r\n";
							rtbMessage.Select(rtbMessage.TextLength, 0);
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
					else if (obj is Label)
					{
						Label label = (Label)obj;
						if (m_fileSend.FullName == (string)label.Tag)
						{
							lock (arrList)
							{
								arrList.RemoveAt(i);
							}
							label.Dispose();
							rtbMessage.UpdateObjects();
						}
					}
				}
				cleanUp();
			}
			else
			{
				m_prevSndStatus = m_SndStatus;
			}
		}

		private void statusTimer_Tick(object sender, EventArgs e)
		{
			sendviaServer();
		}

		/// <summary>
		/// ���� ������ ��� - ���ݿ� ����
		/// </summary>
		private void remoteCancel(string filePath)
		{
			cancelFileTranfer(filePath, false);
			cleanUp();
		}

		private void cancelFileTranfer(string filePath, bool cancelMsg)
		{
			// filePath�� ã�´�.
			string text = rtbMessage.Text;
			int scanPos = text.IndexOf("����#");
			while (scanPos >= 0)
			{
				int startPos = scanPos;
				scanPos += 3;
				int endPos1 = text.IndexOf(" ", scanPos);
				if (endPos1 > scanPos)
				{
					string str = text.Substring(scanPos, endPos1 - scanPos);
					string[] arr = str.Split(new char[] { '|' });
					if (arr.Length != 3) continue;

					string filePath2 = arr[1].Replace(@"\\", @"\");
					if (filePath2.StartsWith(@"\")) filePath2 = @"\" + filePath2;
					if ((filePath != null) && (filePath != filePath2)) continue;

					int endPos = text.IndexOf("\n", startPos);
					if (endPos <= scanPos) endPos = rtbMessage.TextLength;
					rtbMessage.Select(startPos, endPos - startPos);
					rtbMessage.SelectedText = cancelMsg ? "\r\n ��ҵǾ����ϴ�.\r\n" : "\r\n";
					text = rtbMessage.Text;
					scanPos = -1;

					if (cancelMsg)
					{
						rtbTemp.Text = "<file>DENY|" + filePath2.Replace(":", "%3A");
						string msg = rtbTemp.Rtf;
						chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
						                         msg, new UserState(SEND_FLAG.FILE, msg));
					}
				}
				scanPos = text.IndexOf("����#", ++scanPos);
			}

			text = rtbMessage.Text;
			scanPos = text.IndexOf("���#");
			while (scanPos >= 0)
			{
				int startPos = scanPos;
				scanPos += 3;
				int endPos1 = text.IndexOf("\n", scanPos);
				if (endPos1 > scanPos)
				{
					string str = text.Substring(scanPos, endPos1 - scanPos);
					string[] arr = str.Split(new char[] { '|' });
					if (arr.Length != 3) continue;

					string filePath2 = arr[1].Replace(@"\\", @"\");
					if (filePath2.StartsWith(@"\")) filePath2 = @"\" + filePath2;
					if ((filePath != null) && (filePath != filePath2)) continue;

					int endPos = text.IndexOf("\n", startPos);
					if (endPos <= scanPos) endPos = rtbMessage.TextLength;
					rtbMessage.Select(startPos, endPos - startPos);
					rtbMessage.SelectedText = cancelMsg ? "\r\n ��ҵǾ����ϴ�.\r\n" : "\r\n";
					text = rtbMessage.Text;
					scanPos = -1;

					if (cancelMsg)
					{
						rtbTemp.Text = "<file>CANCEL|" + filePath2.Replace(":", "%3A");
						string msg = rtbTemp.Rtf;
						chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
						                         msg, new UserState(SEND_FLAG.FILE, msg));
					}
				}
				scanPos = text.IndexOf("���#", ++scanPos);
			}
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
			int style = (int)theFont.Style;

			StringBuilder sb = new StringBuilder();
			sb.Append(name).Append("|");
			sb.Append(size).Append("|");
			sb.Append(style);
			return sb.ToString();
		}

		private static Font deserialize(string fontString)
		{
			string[] parts = fontString.Split(new char[] { '|' });
			if (parts.Length < 3) return null;
			Font theFont = new Font(parts[0], float.Parse(parts[1]), (FontStyle)int.Parse(parts[2]));
			return theFont;
		}

		private void rtbMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.Handled = true;
				Close();
			}
		}

		private void rtbMessage_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			string linkText = e.LinkText;
			int pos = linkText.IndexOf("#");
			if (pos > 0)
			{
				string[] arr = linkText.Substring(pos + 1).Split(new char[] { '|' });
				int startPos = int.Parse(arr[0]);
				string filePath = arr[1].Replace(@"\\", @"\");
				if (filePath.StartsWith(@"\")) filePath = @"\" + filePath;

				if (linkText.StartsWith("����"))
				{
					if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
					{
						int endPos = rtbMessage.Text.IndexOf("\n", startPos);
						if (endPos <= startPos) endPos = rtbMessage.TextLength;
						rtbMessage.Select(startPos, endPos - startPos);
						rtbMessage.SelectedText = "\r\n";

						string fileName = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
						m_fileReceive = new FileInfo(folderBrowserDialog1.SelectedPath + @"\" + fileName);
						m_totalSizeReceive = long.Parse(arr[2]);
						lock (lockObject)
						{
							m_fsReceive = m_fileReceive.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
						}
						m_elapsedSize = 0;
						m_mode = SOCKET_MODE.FILE_RECEIVE;

						// ���� ���� �̺�Ʈ ó���� ����
						frmMain.SERVER_SOCKET.OnConnect += serverSock_OnConnect;
						frmMain.SERVER_SOCKET.OnRead += serverSock_OnRead;
						frmMain.SERVER_SOCKET.OnError += serverSock_OnError;

						string ip = NIUtil.GetThisHostIP();
						rtbTemp.Text = "<file>OK|" + filePath.Replace(":", "%3A") + "|" + ip;
						string msg = rtbTemp.Rtf;
						chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
						                         msg, new UserState(SEND_FLAG.FILE, msg));

						rtbMessage.SelectionFont = new Font("����", 9F);
						rtbMessage.SelectionColor = Color.Violet;
						rtbMessage.SelectedText = " ���ϸ�: " + fileName + " ";

						// ���� ������
						ProgressBar pb = new ProgressBar();
						pb.Maximum = 100;
						pb.Minimum = 0;
						pb.Style = ProgressBarStyle.Continuous;
						pb.Height = 15;
						pb.Value = 0;
						pb.Tag = fileName;
						lock (arrList)
						{
							arrList.Add(pb);
						}
						rtbMessage.InsertControl(pb);

						Label label = new Label();
						label.TextAlign = ContentAlignment.MiddleLeft;
						label.BackColor = Color.White;
						label.ForeColor = Color.Blue;
						label.Text = "0% (0 KB/s)";
						label.Width = 130;
						label.Height = 15;
						label.Tag = fileName;
						lock (arrList)
						{
							arrList.Add(label);
						}
						rtbMessage.InsertControl(label);
						rtbMessage.UpdateObjects();
						rtbMessage.SelectedText = "\r\n";
					}
				}
				else if (linkText.StartsWith("�ź�")) // �޴� ���� "�ź�"
				{
					string text = rtbMessage.Text;
					int scanPos = text.Substring(0, startPos).LastIndexOf("\n");
					scanPos = text.IndexOf("����#", ++scanPos);
					while ((scanPos >= 0) && (scanPos <= startPos))
					{
						scanPos += 3;
						int endPos1 = text.IndexOf(" ", scanPos);
						if (endPos1 > scanPos)
						{
							string str = text.Substring(scanPos, endPos1 - scanPos);
							string[] arr2 = str.Split(new char[] { '|' });
							if (arr2.Length != 3) continue;

							string filePath2 = arr2[1].Replace(@"\\", @"\");
							if (filePath2.StartsWith(@"\")) filePath2 = @"\" + filePath2;
							if (filePath == filePath2)
							{
								int endPos = text.IndexOf("\n", startPos);
								if (endPos <= startPos) endPos = rtbMessage.TextLength;
								rtbMessage.Select(startPos, endPos - startPos);
								rtbMessage.SelectedText = "\r\n ��ҵǾ����ϴ�.\r\n";
								rtbMessage.Select(rtbMessage.TextLength, 0);

								rtbTemp.Text = "<file>DENY|" + filePath.Replace(":", "%3A");
								string msg = rtbTemp.Rtf;
								chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
								                         msg, new UserState(SEND_FLAG.FILE, msg));
								break;
							}
						}
						scanPos = rtbMessage.Text.IndexOf("����#", ++scanPos);
					}
				}
				else if (linkText.StartsWith("���")) // ������ ���� "���"
				{
					int endPos = rtbMessage.Text.IndexOf("\n", startPos);
					if (endPos <= startPos) endPos = rtbMessage.TextLength;
					rtbMessage.Select(startPos, endPos - startPos);
					rtbMessage.SelectedText = "\r\n ��ҵǾ����ϴ�.\r\n";
					rtbMessage.Select(rtbMessage.TextLength, 0);

					rtbTemp.Text = "<file>CANCEL|" + filePath.Replace(":", "%3A");
					string msg = rtbTemp.Rtf;
					chatSvc.SendMessageAsync(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
					                         msg, new UserState(SEND_FLAG.FILE, msg));
				}
			}
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

				/*
				MessageBoxEx.Show("Ŭ�����忡 ����Ǿ����ϴ�. ���Ͻô� ������ �ٿ������ʽÿ�.",
				                  "���� ����",
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Information,
				                  30000,
				                  MessageBoxEx.Position.CenterParent);
				*/

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
			/*
			else if (rtf.IndexOf("\\wmetafile") >= 0) // ��� ��ư
			{
				// �ؽ�Ʈ�� �Բ� ����(�巡��)�� ���� ����
				bool includeText = false;
				int pos = rtf.IndexOf(@"\fs");
				while (pos >= 0)
				{
					int num;
					if (int.TryParse(rtf.Substring(pos + 3, 2), out num))
					{
						string rep = rtf.Substring(pos, 5); // \fs18, \fs20, ...
						if ((rtf.IndexOf(rep + " ") >= 0) || (rtf.IndexOf(rep + @"\'") >= 0) || (rtf.IndexOf("\\par\r\n") >= 0))
						{
							includeText = true;
							break;
						}
					}
					pos = rtf.IndexOf(@"\fs", ++pos);
				}
				rtbMessage.Cursor = includeText ? Cursors.IBeam : Cursors.Hand;
			}
			else if (rtbMessage.Cursor == Cursors.Hand)
			{
				rtbMessage.Cursor = Cursors.IBeam;
			}
			*/
		}

		// ���� ������ ������ �ִ�ӵ�(rate == -1)�� �����Ѵ�.
		/// <summary>
		/// ���� ����
		/// </summary>
		/// <param name="rate"></param>
		private bool doTransfer(int rate)
		{
			const int weight = 3000;
			const int min_time = 10;
			int buffSize = 51200;
			int est_time, weight_time, max_time;

			long wait = 0;

			if (rate == -1)
			{
				// use 100Mbps for purpose of calculating weight
				est_time = (int)Math.Floor(m_totalSizeSend / (100.0 * 1024 / 8) / 1024);
				weight_time = (int)Math.Floor(((double)weight / 100) * (m_totalSizeSend / (100.0 * 1024 / 8) / 1024));
				max_time = (weight_time > min_time) ? weight_time : min_time;
				Logger.Log("[���Ϻ�����] ��Ʈ��ũ ��� �ִ� �ӵ��� ����!");
			}
			else
			{
				buffSize = 2048;
				wait = (long)(buffSize / ((float)rate * 1024 / 8) * 1000000);

				est_time = (int)Math.Floor(m_totalSizeSend / (rate / 8.0) / 1024);
				weight_time = (int)Math.Floor(((double)weight / 100) * (m_totalSizeSend / (rate / 8.0) / 1024));
				max_time = (weight_time > min_time) ? weight_time : min_time;
				Logger.Log("[���Ϻ�����] ���� �ӵ�: " + rate + " Kbps (" + (rate / 8) + " KB/s), " +
						   "��Ŷ �� ���ð�: " + wait + " ��s");
			}
			Logger.Log("[���Ϻ�����] ���� ���� �ð�: " + est_time + "��, " +
					   "�ִ� ���� ��� �ð�: " + max_time + "��");

			long elapsedSize = 0;
			long timeSeg = 0; // ����ð�(��)
			byte[] data = new byte[buffSize];

			if (wait > 0) uSleep(wait);
			long t1 = DateTime.Now.Ticks;

			long overage = 0;
			long avg = 0;
			int count = 0;
			long w1 = DateTime.Now.Ticks;

			int timeCount = 0;
			long spdSize1 = 0, spdSize2 = 0, spdSize3 = 0, spdSize4 = 0, spdSize5 = 0, spdSize6 = 0, spdSize7 = 0, spdSize8 = 0, spdSize9 = 0, spdSize10 = 0;

			lock (lockObject)
			{
				m_fsSend.Position = 0;
			}
			do
			{
				int numbytes = 0;
				lock (lockObject)
				{
					numbytes = m_fsSend.Read(data, 0, buffSize);
				}
				elapsedSize += numbytes;
				m_SndStatus = elapsedSize;

				// ���� ���۽ð��� �ִ� ���ġ(max_time)�� �ʰ��ϸ� ������ ����Ѵ�.
				if ((w1 - t1) / 10000000 > max_time)
				{
					Logger.Log(Logger.LogLevel.WARNING, "[���Ϻ�����] �ִ� ���� ���ð� �ʰ���!");
					return false;
				}

				// ���ۼӵ��� �����ϰ� �����ϱ� ���� ������ ���� �ð��� �����Ѵ�.
				if (wait > overage) uSleep(wait - overage);
				long w2 = DateTime.Now.Ticks;
				long tdiff = (w2 - w1) / 10;
				avg += tdiff;
				count++;
				if (wait > 0) overage += tdiff - wait;
				w1 = w2;

				int percentElapsed = (m_totalSizeSend > 0) ? (int)(((double)elapsedSize / m_totalSizeSend) * 100) : 0;
				m_PercentElapsed = percentElapsed;

				long timeElapsed = (w1 - t1) / 10000000;
				if (timeElapsed > timeSeg)
				{
					timeSeg = timeElapsed;

					timeCount++;
					if (timeCount > 10) timeCount = 10;

					// �ֱ� 10�ʰ� ��ռӵ� üũ
					spdSize1 = spdSize2;
					spdSize2 = spdSize3;
					spdSize3 = spdSize4;
					spdSize4 = spdSize5;
					spdSize5 = spdSize6;
					spdSize6 = spdSize7;
					spdSize7 = spdSize8;
					spdSize8 = spdSize9;
					spdSize9 = spdSize10;
					spdSize10 = elapsedSize;
					m_Speed = (spdSize10 - spdSize1) / timeCount / 1024;
					if (m_Speed == 0) return false; // �ֱ� 10�ʰ� ������ ���� ������ �ڵ� ���

					// 30�� �������� �α� ���
					if (timeElapsed % 30 == 0)
						Logger.Log(Logger.LogLevel.INFORMATION, "[���Ϻ�����] ���� ��..." + elapsedSize + " ����Ʈ (" + percentElapsed + "%) ���۵�, ����ð�: " +
								   timeElapsed + "��");
				}

				// ������ �����Ѵ�.
				if (m_mode == SOCKET_MODE.FILE_SEND)
				{
					if (!p2pSock.Connected) return false;
					p2pSock.Send(data, numbytes);
				}
				else
				{
					if (!relaySock.Connected) return false;
					relaySock.Send(data, numbytes);
				}
			} while (elapsedSize < m_totalSizeSend);

			Logger.Log("[���Ϻ�����] ��� ���ð� = " + ((count == 0) ? 0 : (float)avg / count) + " ��s");

			lock (lockObject)
			{
				m_fsSend.Close();
				m_fsSend = null;
			}
			if (statusTimer != null) statusTimer.Stop();
			statusTimer = null;
			m_SndStatus = m_prevSndStatus = 0;

			long t2 = DateTime.Now.Ticks;
			double t3 = ((double)(t2 - t1)) / 10000000;

			Logger.Log(Logger.LogLevel.INFORMATION, "[���Ϻ�����] �� ����ð�: " + t3.ToString("###,##0.##") + "��");
			Logger.Log(Logger.LogLevel.INFORMATION, "[���Ϻ�����] ��� ���ۼӵ�: " + ((t3 != 0) ? (m_totalSizeSend / t3 / 1024) : 0).ToString("###,###,##0.##") + " KB/s");
			return true;
		}

		/// <summary>
		/// ����ũ����(��) ������ �����带 �����Ѵ�.
		/// </summary>
		/// <param name="milliseconds">The milliseconds.</param>
		private static void uSleep(long milliseconds)
		{
			int microSec = (int)(milliseconds / 1000);
			Thread.Sleep(microSec);
		}

		private void cleanUp()
		{
			try
			{
				if (p2pSock != null)
				{
					p2pSock.Disconnect();
					p2pSock = null;
				}

				if (relaySock != null)
				{
					relaySock.Disconnect();
					relaySock = null;
				}

				lock (lockObject)
				{
					if (m_fsSend != null)
					{
						m_fsSend.Flush();
						m_fsSend.Close();
						m_fsSend = null;
					}

					if (m_fsReceive != null)
					{
						if (m_elapsedSize == 0)
						{
							m_fsReceive.Close();
							m_fsReceive = null;
							m_fileReceive.Delete();
						}
						else
						{
							m_fsReceive.Flush();
							m_fsReceive.Close();
							m_fsReceive = null;
						}
					}
				}
				// ���� ���� �̺�Ʈ ó���� ���� ����
				frmMain.SERVER_SOCKET.OnConnect -= serverSock_OnConnect;
				frmMain.SERVER_SOCKET.OnRead -= serverSock_OnRead;
				frmMain.SERVER_SOCKET.OnError -= serverSock_OnError;

				if (statusTimer != null) statusTimer.Stop();
				statusTimer = null;

				m_SndStatus = m_prevSndStatus = 0;
				m_mode = SOCKET_MODE.NONE;
				m_retryCount = 0;
				m_elapsedSize = m_totalSizeSend = m_totalSizeReceive = 0;
				
				// ���ื�� �ʱ�ȭ�� ���� 250�и��� ���� ����Ѵ�.
				Thread.Sleep(250);

				m_PercentElapsed = 0;

				m_rcvTimeCount = 0;
				m_rcvTimeSeg = 0;
				m_rcvSize1 = m_rcvSize2 = m_rcvSize3 = m_rcvSize4 = m_rcvSize5 = m_rcvSize6 = m_rcvSize7 = m_rcvSize8 = m_rcvSize9 = m_rcvSize10 = 0;
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("PrivateMessage::cleanUp", ex);
				Logger.Log(Logger.LogLevel.ERROR, "cleanUp() - " + ex);
			}
		}

		private void PrivateMessage_Resize(object sender, EventArgs e)
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

		private void PrivateMessage_Activated(object sender, EventArgs e)
		{
			isLoaded = true;
			rtbWriteMsg.Focus();
		}

		private void picUser_Click(object sender, EventArgs e)
		{
			ExtendedWebBrowser _browser = new ExtendedWebBrowser();
			_browser.Navigate(frmMain.LOGIN_INFO.URL, "_BLANK", frmMain.LOGIN_INFO.GetProfilePostData(remoteID),
							  "Accept-Language: ko\r\nContent-Type: application/x-www-form-urlencoded\r\nAccept-Encoding: gzip, deflate\r\nProxy-Connection: Keep-Alive\r\n");
		}

		private void picUser_MouseEnter(object sender, EventArgs e)
		{
			picUser.Image = setImageOpacity((Image)picUser.Tag, 1.0F);
		}

		private void picUser_MouseLeave(object sender, EventArgs e)
		{
			picUser.Image = setImageOpacity((Image)picUser.Tag, 0.85F);
		}

		#region ServerSocket �̺�Ʈ

		private void serverSock_OnError(string errorMessage, string name, Socket socket, int errorCode)
		{
			if (errorCode == 10054) // ������ ������ ������ ���� ���
			{
				Logger.Log("[ServerSocket] ���� ����: " + socket.RemoteEndPoint);

				if (arrList.Count == 0) return;
				if ((m_PercentElapsed > 0) && (m_PercentElapsed < 100))
				{
					for (int i = 0; i < arrList.Count; i++)
					{
						object obj = arrList[i];
						// ���� ������
						if (obj is ProgressBar)
						{
							ProgressBar pb = (ProgressBar)obj;
							pb.Width = 1;
							pb.Height = 1;
							pb.Tag = null;
							rtbMessage.UpdateObjects();

							lock (arrList)
							{
								arrList.RemoveAt(i--);
							}
							pb.Dispose();
						}
						else if (obj is Label)
						{
							Label label = (Label)obj;
							label.Text = "";
							label.Width = 1;
							label.Height = 1;
							label.Tag = null;
							rtbMessage.UpdateObjects();

							lock (arrList)
							{
								arrList.RemoveAt(i--);
							}
							label.Dispose();
						}
					}
					rtbMessage.UpdateObjects();

					rtbMessage.SelectedText = " ��ҵǾ����ϴ�.\r\n";
					rtbMessage.Select(rtbMessage.TextLength, 0);
					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // Ŀ�� ��ġ�� �̵��ϴ� ������ �߻��ϸ� �����Ѵ�.
					{
					}

					lock (statusLabel)
					{
						statusLabel.Text = "���������� �޽����� ���� �ð�: " + DateTime.Now.ToString("yyyy�� M�� d�� H�� m�� s��");
						statusLabel.Tag = statusLabel.Text;
					}
				}
				cleanUp();
				return;
			}

			if (socket == null)
			{
				Logger.Log("[ServerSocket] �� �� ���� ���� �߻�(�����ڵ�: " + errorCode + ", �޽���: " + errorMessage + ").");
			}
			else
			{
				Logger.Log("[ServerSocket] �� �� ���� ���� �߻�: " + socket.RemoteEndPoint + "(�����ڵ�: " + errorCode + ", �޽���: " +
						   errorMessage + ")");
			}
			cleanUp();
		}

		private void serverSock_OnRead(string name, Socket socket, byte[] bytes)
		{
			if ((bytes == null) || (bytes.Length == 0)) return;
			if (m_fsReceive == null) return;
			int len = bytes.Length;

			try
			{
				lock (lockObject)
				{
					m_fsReceive.Write(bytes, 0, len);
				}
				m_elapsedSize += len;
				m_PercentElapsed = (int)(((double)m_elapsedSize / m_totalSizeReceive) * 100);

				long timeElapsed = DateTime.Now.Subtract(m_rcvTime).Ticks / 10000000;
				if (timeElapsed > m_rcvTimeSeg)
				{
					m_rcvTimeSeg = timeElapsed;

					m_rcvTimeCount++;
					if (m_rcvTimeCount > 10) m_rcvTimeCount = 10;

					// �ֱ� 10�ʰ� ��ռӵ� üũ
					m_rcvSize1 = m_rcvSize2;
					m_rcvSize2 = m_rcvSize3;
					m_rcvSize3 = m_rcvSize4;
					m_rcvSize4 = m_rcvSize5;
					m_rcvSize5 = m_rcvSize6;
					m_rcvSize6 = m_rcvSize7;
					m_rcvSize7 = m_rcvSize8;
					m_rcvSize8 = m_rcvSize9;
					m_rcvSize9 = m_rcvSize10;
					m_rcvSize10 = m_elapsedSize;
					m_Speed = (m_rcvSize10 - m_rcvSize1) / m_rcvTimeCount / 1024;
				}

				if (m_elapsedSize >= m_totalSizeReceive)
				{
					frmMain.SERVER_SOCKET.CloseConnection(name);
					Thread.Sleep(500);
					cleanUp();
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("PrivateMessage::serverSock_OnRead", ex);
				Logger.Log(Logger.LogLevel.ERROR, "���� ���⿡ �����߽��ϴ�, ����: " + ex);
				cleanUp();
			}
		}

		private void serverSock_OnConnect(string name, Socket socket)
		{
			m_PercentElapsed = 0;
			m_rcvTime = DateTime.Now;
			m_rcvTimeCount = 0;
			m_rcvTimeSeg = 0;
			m_rcvSize1 = m_rcvSize2 = m_rcvSize3 = m_rcvSize4 = m_rcvSize5 = m_rcvSize6 = m_rcvSize7 = m_rcvSize8 = m_rcvSize9 = m_rcvSize10 = 0;
			Logger.Log("[ServerSocket] TCP �����: " + socket.RemoteEndPoint);
		}

		#endregion

		#region ClientSocket �̺�Ʈ

		private void clientSock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			if (errorCode == 10054) // ������ ������ ������ ���� ���
			{
				// ���� ����� ��ȯ�� ���Ŀ� ������ �߻��� ��쿡�� �����Ѵ�.
				if (m_mode == SOCKET_MODE.FILE_SEND_SERVER) return;

				Logger.Log("[ClientSocket_OnError] ���� ����: " + socket.RemoteEndPoint);
				cleanUp();
				return;
			}

			if (socket == null)
			{
				if ((m_SndStatus == 0) && (m_mode == SOCKET_MODE.FILE_SEND)) // ���� �� Ÿ�Ӿƿ�
				{
					sendviaServer();
					return;
				}

				// ���� ����� ��ȯ�� ���Ŀ� ������ �߻��� ��쿡�� �����Ѵ�.
				if (m_mode == SOCKET_MODE.FILE_SEND_SERVER) return;

				Logger.Log("[ClientSocket_OnError] �� �� ���� ���� �߻�(�����ڵ�: " + errorCode + ", �޽���: " + errorMessage + ").");
			}
			else
			{
				Logger.Log("[ClientSocket_OnError] �� �� ���� ���� �߻�: " + socket.RemoteEndPoint + "(�����ڵ�: " + errorCode + ", �޽���: " +
						   errorMessage + ")");
			}
			cleanUp();
		}

		private void clientSock_OnConnect(Socket socket)
		{
			m_SndStatus = 1;

			// ����Ǹ� �״�� �����͸� ������.
			if (!doTransfer(-1))
			{
				// filePath�� ã�´�.
				string text = rtbMessage.Text;
				int scanPos = text.IndexOf("���#");
				while (scanPos >= 0)
				{
					scanPos += 3;
					int endPos1 = text.IndexOf("\n", scanPos);
					if (endPos1 > scanPos)
					{
						string str = text.Substring(scanPos, endPos1 - scanPos);
						string[] arr = str.Split(new char[] {'|'});
						if (arr.Length != 3) continue;

						string filePath = arr[1].Replace(@"\\", @"\");
						if (filePath.StartsWith(@"\")) filePath = @"\" + filePath;
						if (filePath == m_fileSend.FullName)
						{
							int endPos = text.IndexOf("\n", scanPos);
							if (endPos <= scanPos) endPos = rtbMessage.TextLength;
							rtbMessage.Select(scanPos, endPos - scanPos);
							rtbMessage.SelectedText = "\r\n";
							text = rtbMessage.Text;
						}
					}
					scanPos = text.IndexOf("���#", ++scanPos);
				}
			}
			else
			{
				while (p2pSock.Connected) Thread.Sleep(50);
			}
			cleanUp();
		}

		private void relaySock_OnError(string errorMessage, Socket socket, int errorCode)
		{
			if (errorCode == 10054) // ������ ������ ������ ���� ���
			{
				Logger.Log("[ClientSocket_OnError] ���� ����: " + socket.RemoteEndPoint);

				// ���� �������: �������� ��쿡�� Ÿ�̸ӿ� ���� ������ �����Ѵ�.
				if (m_SndStatus == 1) return;

				cleanUp();
				return;
			}

			if (socket == null)
			{
				Logger.Log("[ClientSocket_OnError] �� �� ���� ���� �߻�(�����ڵ�: " + errorCode + ", �޽���: " + errorMessage + ").");
			}
			else
			{
				Logger.Log("[ClientSocket_OnError] �� �� ���� ���� �߻�: " + socket.RemoteEndPoint + "(�����ڵ�: " + errorCode + ", �޽���: " +
						   errorMessage + ")");
			}
			cleanUp();
		}

		private void relaySock_OnConnect(Socket socket)
		{
			if (m_mode == SOCKET_MODE.FILE_RECEIVE_SERVER)
			{
				// �����κ��� ������ �ޱ� ���� ����ϴ� ���
				relaySock.SendText("<file>RECEIVER|" + frmMain.LOGIN_INFO.LoginID);

				m_PercentElapsed = 0;
				m_rcvTime = DateTime.Now;
				m_rcvTimeCount = 0;
				m_rcvTimeSeg = 0;
				m_rcvSize1 = m_rcvSize2 = m_rcvSize3 = m_rcvSize4 = m_rcvSize5 = m_rcvSize6 = m_rcvSize7 = m_rcvSize8 = m_rcvSize9 = m_rcvSize10 = 0;
			}
			else if (m_mode == SOCKET_MODE.FILE_SEND_SERVER)
			{
				m_SndStatus = 1;

				// ���� ���� ����
				relaySock.SendText("<file>SENDER|" + remoteID);

				// ������ �Ϸ�� ������ ����Ѵ�.
				while ((relaySock != null) && relaySock.Connected && (m_SndStatus == 1)) Thread.Sleep(50);

				if (m_SndStatus > 1)
				{
					// ������ �����ϴ� ��쿡�� 3Mbps�� �����Ѵ�.
					if (!doTransfer(3 * 1024))
					{
						// filePath�� ã�´�.
						string text = rtbMessage.Text;
						int scanPos = text.IndexOf("���#");
						while (scanPos >= 0)
						{
							scanPos += 3;
							int endPos1 = text.IndexOf("\n", scanPos);
							if (endPos1 > scanPos)
							{
								string str = text.Substring(scanPos, endPos1 - scanPos);
								string[] arr = str.Split(new char[] { '|' });
								if (arr.Length != 3) continue;

								string filePath = arr[1].Replace(@"\\", @"\");
								if (filePath.StartsWith(@"\")) filePath = @"\" + filePath;
								if (filePath == m_fileSend.FullName)
								{
									int endPos = text.IndexOf("\n", scanPos);
									if (endPos <= scanPos) endPos = rtbMessage.TextLength;
									rtbMessage.Select(scanPos, endPos - scanPos);
									rtbMessage.SelectedText = "\r\n";
									text = rtbMessage.Text;
								}
							}
							scanPos = text.IndexOf("���#", ++scanPos);
						}
					}
					else
					{
						while ((relaySock != null) && relaySock.Connected) Thread.Sleep(50);
					}
				}
				cleanUp();
			}
		}

		private void relaySock_OnRead(Socket socket, byte[] bytes)
		{
			if ((bytes == null) || (bytes.Length == 0)) return;
			int len = bytes.Length;

			try
			{
				if (bytes.Length < 128)
				{
					// �÷��� Ȯ��
					// bytes=<file>DISCONNECT: ���ϼ��� ����
					// bytes=<file>OK: ���� ����
					string s = Encoding.Default.GetString(bytes);
					if (s.StartsWith("<file>DISCONNECT"))
					{
						if ((relaySock != null) && relaySock.Connected) relaySock.Disconnect();
						Thread.Sleep(500);
						cleanUp();
						return;
					}
					if (s.StartsWith("<file>OK")) // ���� �۽� ���� �÷���
					{
						m_SndStatus = 2;
						return;
					}
				}

				if (m_fsReceive == null) return;

				lock (lockObject)
				{
					m_fsReceive.Write(bytes, 0, len);
				}
				m_elapsedSize += len;
				m_PercentElapsed = (m_totalSizeReceive > 0) ? (int)(((double)m_elapsedSize / m_totalSizeReceive) * 100) : 0;

				long timeElapsed = DateTime.Now.Subtract(m_rcvTime).Ticks / 10000000;
				if (timeElapsed > m_rcvTimeSeg)
				{
					m_rcvTimeSeg = timeElapsed;

					m_rcvTimeCount++;
					if (m_rcvTimeCount > 10) m_rcvTimeCount = 10;

					// �ֱ� 10�ʰ� ��ռӵ� üũ
					m_rcvSize1 = m_rcvSize2;
					m_rcvSize2 = m_rcvSize3;
					m_rcvSize3 = m_rcvSize4;
					m_rcvSize4 = m_rcvSize5;
					m_rcvSize5 = m_rcvSize6;
					m_rcvSize6 = m_rcvSize7;
					m_rcvSize7 = m_rcvSize8;
					m_rcvSize8 = m_rcvSize9;
					m_rcvSize9 = m_rcvSize10;
					m_rcvSize10 = m_elapsedSize;
					m_Speed = (m_rcvSize10 - m_rcvSize1) / m_rcvTimeCount / 1024;
				}

				if (m_elapsedSize >= m_totalSizeReceive)
				{
					if ((relaySock != null) && relaySock.Connected) relaySock.Disconnect();
					Thread.Sleep(500);
					cleanUp();
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("PrivateMessage::clientSock_OnRead", ex);
				Logger.Log(Logger.LogLevel.ERROR, "[���Ϲޱ�] ���� ���⿡ �����߽��ϴ�, ����: " + ex);
				cleanUp();
			}
		}

		#endregion

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

		#region Nested type: SOCKET_MODE

		private enum SOCKET_MODE
		{
			NONE = 0,
			FILE_SEND = 1,
			FILE_RECEIVE = 2,
			FILE_SEND_SERVER = 3,
			FILE_RECEIVE_SERVER = 4
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
