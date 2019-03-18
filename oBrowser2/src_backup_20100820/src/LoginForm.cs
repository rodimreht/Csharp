using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using oBrowser2.Properties;

namespace oBrowser2
{
	public partial class LoginForm : Form
	{
		private string _sessionID;
		private string _loginID;
		private string _password;
		private Uri _uri;
		private string _universeName;
		
		public LoginForm()
		{
			InitializeComponent();
		}

		public string SessionID
		{
			get { return _sessionID; }
		}

		public string LoginID
		{
			get { return _loginID; }
		}

		public string Password
		{
			get { return _password; }
		}

		public Uri URL
		{
			get { return _uri; }
		}

		public string UniverseName
		{
			get { return _universeName; }
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			int selectedIndex = -1;

			SettingsHelper settings = SettingsHelper.Current;
			string ogDomain = settings.OGameDomain;
			if (string.IsNullOrEmpty(ogDomain)) ogDomain = "ogame.org";

			// 도메인 설정 적용
			cboUniverse.Items.Clear();
			foreach (string s in settings.OGameUniNames) // capella:103
			{
				cboUniverse.Items.Add(new UniverseItem(s + "우주", s.Split(':')[0] + "." + ogDomain));
			}

			string userID = settings.UserID;
			if (userID != null)
			{
				if (userID.IndexOf("|") >= 0) // 아이디에 접속우주 표시
				{
					string[] uids = userID.Split(new char[] { '|' });
					cboUniverse.SelectedIndex = selectedIndex = int.Parse(uids[0]);
					userID = uids[1].Replace("&#7C;", "|");
				}

				txtLoginID.Text = GetOriginalID(userID);
			}

			if (settings.Password != null)
			{
				if (OB2Security.IsStrongKey(userID))
				{
					string pwd = settings.Password.Replace("enc:", "");
					txtPassword.Text = OB2Security.Decrypt(userID, pwd);
				}
				else
				{
					string key = userID + "12345678";
					string pwd = settings.Password.Replace("enc:", "");
					txtPassword.Text = OB2Security.Decrypt(key, pwd);
				}
			}

			if (selectedIndex == -1)
			{
				selOption1.Checked = true;

				cboUniverse.SelectedIndex = 0;
				cboUniverse.Focus();
			}
			else
			{
				selOption2.Checked = true;
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (cboUniverse.SelectedIndex < 0)
			{
				MessageBoxEx.Show("접속할 서버를 선택하세요.",
				                  "로그인",
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Warning,
				                  5000);
				return;
			}
			
			if (selOption1.Checked && (txtSessionID.Text.Length == 0))
			{
				MessageBoxEx.Show("세션ID를 입력하세요.",
								  "로그인",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning,
								  5000);
				return;
			}
			else if (selOption2.Checked && (txtLoginID.Text.Length == 0))
			{
				MessageBoxEx.Show("로그인ID를 입력하세요.",
								  "로그인",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning,
								  5000);
				return;
			}
			else if (selOption2.Checked && (txtPassword.Text.Length == 0))
			{
				MessageBoxEx.Show("패스워드를 입력하세요.",
								  "로그인",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning,
								  5000);
				return;
			}
			
			UniverseItem item = (UniverseItem)cboUniverse.SelectedItem;
			
			if (selOption1.Checked)
			{
				_universeName = item.Text;
				_sessionID = txtSessionID.Text;
				string url = "http://" + item.Value + "/game/index.php?page=overview&session=" + _sessionID;
				_uri = new Uri(url);
			}
			else
			{
				_universeName = item.Text;
				_loginID = txtLoginID.Text;
				_loginID = GetFixedID(_loginID);
				string newID = cboUniverse.SelectedIndex + "|" + _loginID.Replace("|", "&#7C;");
				_password = txtPassword.Text;

				SettingsHelper settings = SettingsHelper.Current;
				settings.UserID = newID;

				if (OB2Security.IsStrongKey(_loginID))
				{
					settings.Password = "enc:" + OB2Security.Encrypt(_loginID, _password);
				}
				else
				{
					string key = _loginID + "12345678";
					settings.Password = "enc:" + OB2Security.Encrypt(key, _password);
				}
				settings.Changed = true;
				// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로

				string url = "http://" + item.Value + "/game/reg/login2.php?v=2" +
					"&login=" + _loginID + "&pass=" + _password;
				_uri = new Uri(url);
			}
			
			DialogResult = DialogResult.OK;
			Close();
		}
		
		public static string GetFixedID(string id)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(id);
			if (bytes.Length != id.Length)
			{
				StringBuilder sb = new StringBuilder(bytes.Length);
				for (int i = 0; i < bytes.Length; i++)
				{
					sb.Append("%" + bytes[i].ToString("X2"));
				}
				return sb.ToString();
			}
			return id;
		}

		public static string GetOriginalID(string id)
		{
			string[] arr = id.Split(new char[] {'%'});
			if (arr.Length < 2) return id;

			byte[] bytes = new byte[arr.Length - 1];
			for (int i = 1; i < arr.Length; i++)
				bytes[i - 1] = getByteFromHex(arr[i]);

			return Encoding.UTF8.GetString(bytes);
		}

		private static byte getByteFromHex(string sHex)
		{
			char[] cTemp = sHex.ToUpper().ToCharArray();
			byte bTemp = 0;
			for (int k = 0; k < cTemp.Length; k++)
			{
				switch (k)
				{
					case 0:
						if (cTemp[k] >= 'A')
							bTemp += (byte)((cTemp[k] - 'A' + 10) * 16);
						else
							bTemp += (byte)(int.Parse(cTemp[k].ToString()) * 16);

						break;

					case 1:
						if (cTemp[k] >= 'A')
							bTemp += (byte)(cTemp[k] - 'A' + 10);
						else
							bTemp += (byte)int.Parse(cTemp[k].ToString());

						break;

					default:
						bTemp = 0;
						break;
				}
			}
			return bTemp;
		}

		class UniverseItem
		{
			private string text;
			private string value;

			public UniverseItem(string text, string val)
			{
				this.text = text;
				this.value = val;
			}
			
			public string Text
			{
				get { return text; }
				set { text = value; }
			}

			public string Value
			{
				get { return value; }
				set { this.value = value; }
			}
			
			public override string ToString()
			{
				return this.text;
			}
		}

		private void selOption1_CheckedChanged(object sender, EventArgs e)
		{
			toggleCheckBoxes(true);
		}

		private void selOption2_CheckedChanged(object sender, EventArgs e)
		{
			toggleCheckBoxes(false);
		}
		
		private void toggleCheckBoxes(bool selectOption1)
		{
			if (selectOption1)
			{
				txtSessionID.Enabled = true;
				txtLoginID.Enabled = false;
				txtPassword.Enabled = false;
				
				txtSessionID.SelectAll();
				txtSessionID.Focus();
			}
			else
			{
				txtSessionID.Enabled = false;
				txtLoginID.Enabled = true;
				txtPassword.Enabled = true;

				if (txtPassword.Text.Length > 0)
				{
					okButton.Select();
					okButton.Focus();
				}
				else
				{
					txtLoginID.SelectAll();
					txtLoginID.Focus();
				}
			}
		}

		private void txtSessionID_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				okButton_Click(null, null);
		}

		private void txtPassword_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				okButton_Click(null, null);
		}
	}
}