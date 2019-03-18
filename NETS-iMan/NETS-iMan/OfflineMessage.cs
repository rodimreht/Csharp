using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using NETS_iMan.chatWebsvc;
using NETS_iMan.iManWebsvc;

namespace NETS_iMan
{
	public partial class OfflineMessage : Form
	{
		private readonly string remoteID;
		private readonly string remoteUserName;
		private readonly RichTextBox rtbTemp = new RichTextBox();
		private readonly ToolTip toolTip1;
		private readonly ToolTip toolTip2;
		private readonly ToolTip toolTip3;
		private readonly ToolTip toolTip4;
		private readonly int windowMode;
		private readonly ChatService chatSvc = new ChatService();

		public OfflineMessage(string strMsg)
		{
			try
			{
				InitializeComponent();

				windowMode = 0;
				rtbMessage.Visible = true;
				splitContainer1.Visible = false;
				splitContainer2.Visible = false;
				rtbWriteMsg.Visible = false;
				btnSend.Visible = false;
				btnEmotion.Visible = false;
				btnColor.Visible = false;
				btnFont.Visible = false;
				panel1.Visible = false;

				string[] msgs = strMsg.Split(new string[] {"\n\n\t\n"}, StringSplitOptions.RemoveEmptyEntries);
				for (int k = 0; k < msgs.Length; k++)
				{
					string sendDate = msgs[k].Substring(0, 19);
					DateTime dt;
					string[] strU;
					string strM = string.Empty;
					if (!DateTime.TryParse(sendDate, out dt))
					{
						sendDate = "(현재 로그인 중)";
						strU = msgs[k].Split(':');
						for (int i = 2; i < strU.Length; i++)
						{
							strM += strU[i];
						}
					}
					else
					{
						strU = msgs[k].Substring(20).Split(':');
						for (int i = 1; i < strU.Length; i++)
						{
							strM += strU[i];
						}
					}
					try
					{
						int pos = rtbMessage.SelectionStart = rtbMessage.TextLength;
						rtbMessage.Select(pos, 0);
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
					rtbMessage.SelectedText = strU[0] + "님이 " + sendDate + "에 남긴 쪽지:\r\n ";
					rtbTemp.Rtf = strM;
					ChatLogger.Log(strU[0] + "님이 " + sendDate + "에 남긴 쪽지: " + rtbTemp.Text);

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
				ErrorReport.SendReport("OfflineMessage 생성자", ex);
			}
		}

		public OfflineMessage(string strUserID, string strUserName, string strMsg)
		{
			try
			{
				InitializeComponent();

				toolTip1 = new ToolTip();
				toolTip2 = new ToolTip();
				toolTip3 = new ToolTip();
				toolTip4 = new ToolTip();

				windowMode = 1;
				remoteID = strUserID;
				remoteUserName = strUserName;

				rtbMessage.Visible = false;
				splitContainer1.Visible = true;
				splitContainer2.Visible = true;
				rtbWriteMsg.Visible = true;
				if (strMsg != null) rtbWriteMsg.Text = strMsg;
				btnSend.Visible = true;
				btnEmotion.Visible = true;
				btnColor.Visible = true;
				btnFont.Visible = true;
				panel1.Visible = false;

				picUser.Visible = true;
				toolTip4.SetToolTip(picUser, strUserName + " 프로필 보기");

				if (remoteID.StartsWith("GROUP:"))
				{
					picUser.Visible = false;
					toolTip4.RemoveAll();
				}
				else
				{
					ThreadStart showPhoto = showPhotoProc;
					Thread showPhotoThread = new Thread(showPhoto);
					showPhotoThread.Start();
				}

				rtbWriteMsg.DragDrop += Object_DragDrop;
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("OfflineMessage 생성자2", ex);
			}
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
						if (title.Trim().Length == 0) title = "사원";
						string name = node.Attributes["name"].InnerText;
						lblName.Text = (name.Trim().Length > 0) ? title + " " + name : "(無名)";
						lblMobile.Text = node.Attributes["mobile"].InnerText;
					}

					using (Stream stream = WebCall.GetStream(new Uri("http://sso.nets.co.kr/iManService/getPhoto.aspx?uid=" + remoteID), ""))
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
							if (title.Trim().Length == 0) title = "사원";
							string name = node.Attributes["name"].InnerText;
							lblName.Text = (name.Trim().Length > 0) ? title + " " + name : "(無名)";
							lblMobile.Text = node.Attributes["mobile"].InnerText;
						}

						using (Stream stream = WebCall.GetStream(new Uri("http://sso.nets.co.kr/iManService/getPhoto.aspx?tempid=" + remoteID), ""))
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

		private void Object_DragDrop(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
			processFileSend(e.Data, e);
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

		private static Image setImageOpacity(Image imgPic, float imgOpac)
		{
			Bitmap bmpPic = new Bitmap(imgPic.Width, imgPic.Height);
			Graphics gfxPic = Graphics.FromImage(bmpPic);
			ColorMatrix cmxPic = new ColorMatrix();
			cmxPic.Matrix33 = imgOpac;

			ImageAttributes iaPic = new ImageAttributes();
			iaPic.SetColorMatrix(cmxPic, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			gfxPic.DrawImage(imgPic, new Rectangle(0, 0, bmpPic.Width, bmpPic.Height), 0, 0, imgPic.Width, imgPic.Height, GraphicsUnit.Pixel, iaPic);
			gfxPic.Dispose();

			return bmpPic;
		}

		private void processFileSend(IDataObject data, object e)
		{
			// 이미지 삽입인 경우 크기 체크
			if ((data.GetData("Bitmap") is Bitmap))
			{
				MemoryStream ms = data.GetData("DeviceIndependentBitmap") as MemoryStream;
				if ((ms != null) && (ms.Length > 800 * 600 * 3))
				{
					if (e is DragEventArgs) ((DragEventArgs)e).Effect = DragDropEffects.None;
					if (e is KeyEventArgs) ((KeyEventArgs)e).Handled = true;

					rtbMessage.SelectionColor = Color.DarkRed;
					rtbMessage.SelectedText = "* 800x600 보다 큰 이미지는 파일로 첨부하시기 바랍니다.\r\n";
					try
					{
						rtbMessage.SelectedText = "\r\n";
						rtbMessage.ScrollToCaret();
					}
					catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
					{
					}
					return;
				}
			}

			string[] paths = (string[])data.GetData("FileDrop");
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
				if (e is DragEventArgs) ((DragEventArgs)e).Effect = DragDropEffects.None;
				if (e is KeyEventArgs) ((KeyEventArgs)e).Handled = true;

				rtbWriteMsg.SelectionColor = Color.DarkRed;
				rtbWriteMsg.SelectedText = "* 쪽지로 512KB가 넘는 파일 전송은 지원하지 않습니다.\r\n";
				rtbWriteMsg.SelectionColor = Color.DarkRed;
				rtbWriteMsg.SelectedText = "* 파일을 분할하여 전송하시기 바랍니다.\r\n";
				rtbMessage.SelectionColor = Color.DarkRed;
				rtbMessage.SelectedText = "* 오피스 통합문서도 전송되지 않으므로 압축하여 전송하시기 바랍니다.\r\n";
				try
				{
					rtbWriteMsg.SelectedText = "\r\n";
					rtbWriteMsg.ScrollToCaret();
				}
				catch (COMException) // 커서 위치를 이동하다 에러가 발생하면 무시한다.
				{
				}
			}
			else
			{
				string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
				rtbWriteMsg.SelectionFont = new Font("굴림", 9F);
				rtbWriteMsg.SelectionColor = Color.Violet;
				rtbWriteMsg.SelectedText = fileName + "\r\n(아래 아이콘을 클릭하여 저장하십시오. // 더블클릭: 바로 열기)\r\n";
			}
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			try
			{
				if (rtbWriteMsg.Text.Trim().Length > 0 || isObjectEmbeded(rtbWriteMsg.Rtf))
				{
					if (remoteID.StartsWith("GROUP:"))
					{
						string[] users = remoteUserName.Split(new char[] {'|'});
						foreach (string user in users)
						{
							if (user.Trim().Length == 0) continue;

							chatSvc.SendOfflineMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", user,
							                           rtbWriteMsg.Rtf);
						}
						rtbTemp.Rtf = rtbWriteMsg.Rtf;
						ChatLogger.Log(frmMain.LOGIN_INFO.UserName + "님이 " + remoteID.Substring(6) + " 구성원 모두에게 보낸 쪽지: " + rtbTemp.Text);
					}
					else
					{
						chatSvc.SendOfflineMessage(frmMain.LOGIN_INFO.UserName + "(" + frmMain.LOGIN_INFO.LoginID + ")", remoteUserName,
												   rtbWriteMsg.Rtf);
						rtbTemp.Rtf = rtbWriteMsg.Rtf;
						ChatLogger.Log(frmMain.LOGIN_INFO.UserName + "님이 " + remoteUserName + "님에게 보낸 쪽지: " + rtbTemp.Text);
					}
					MessageBoxEx.Show("쪽지가 전송되었습니다.", "쪽지", MessageBoxButtons.OK, 3000);
					Close();
				}
			}
			catch (Exception ex)
			{
				ErrorReport.SendReport("OfflineMessage::btnSend_Click", ex);
			}
		}

		private static bool isObjectEmbeded(string rtf)
		{
			return (rtf.IndexOf(@"\pic") >= 0) || (rtf.IndexOf(@"\objdata") >= 0);
		}

		private void OfflineMessage_Load(object sender, EventArgs e)
		{
			if (windowMode == 1)
			{
				if (remoteID.StartsWith("GROUP:"))
					Text = remoteID.Substring(6) + " 구성원 모두에게 보내는 쪽지";
				else
					Text = remoteUserName + "님에게 보내는 쪽지";

				toolTip1.SetToolTip(btnEmotion, "이모티콘 삽입");
				toolTip2.SetToolTip(btnColor, "글 색깔 설정");
				toolTip3.SetToolTip(btnFont, "글꼴 설정");

				SettingsHelper helper = SettingsHelper.Current;

				if (!string.IsNullOrEmpty(helper.Font))
					rtbWriteMsg.Font = deserialize(helper.Font);

				if (!string.IsNullOrEmpty(helper.Color))
					rtbWriteMsg.ForeColor = Color.FromArgb(int.Parse(helper.Color));
			}
		}

		private void OfflineMessage_FormClosing(object sender, FormClosingEventArgs e)
		{
			Dispose();
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
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
			rtbWriteMsg.Tag = img;
			panel1.Visible = false;
			img.Dispose();
		}

		private void OfflineMessage_Activated(object sender, EventArgs e)
		{
			if (windowMode == 1)
				rtbWriteMsg.Focus();
			else
			{
				int cnt = 0;
				while (cnt < 3)
				{
					SoundPlayer.PlaySound("newoffmsg.wav");
					if (NIUtil.FlashWindowEx(this)) break;
					cnt++;
					Thread.Sleep(500);
				}
			}
		}

		private void rtbWriteMsg_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.Handled = true;
				Close();
			}
			else if ((e.KeyCode == Keys.V) && e.Control)
			{
				IDataObject obj = Clipboard.GetDataObject();
				if (obj != null)
				{
					processFileSend(obj, e);
				}
			}
		}

		private void rtbMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.Handled = true;
				Close();
			}
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
	}
}