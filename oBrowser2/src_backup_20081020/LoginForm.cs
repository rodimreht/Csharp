using System;
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
			cboUniverse.Items.Clear();
			for (int i = 1; i <= 9; i++)
				cboUniverse.Items.Add(new UniverseItem("제" + i + "우주", "uni" + i + ".ogame2.co.kr"));
			
			selOption1.Checked = true;
			txtSessionID.Enabled = true;
			
			selOption2.Checked = false;

			SettingsHelper settings = SettingsHelper.Current;
			if (settings.UserID != null)
				txtLoginID.Text = settings.UserID;
			if (settings.Password != null)
			{
				if (OB2Security.IsStrongKey(settings.UserID))
				{
					string pwd = settings.Password.Replace("enc:", "");
					txtPassword.Text = OB2Security.Decrypt(settings.UserID, pwd);
				}
				else
				{
					string key = settings.UserID + "12345678";
					string pwd = settings.Password.Replace("enc:", "");
					txtPassword.Text = OB2Security.Decrypt(key, pwd);
				}
			}

			txtLoginID.Enabled = false;
			txtPassword.Enabled = false;

			cboUniverse.SelectedIndex = 0;
			cboUniverse.Focus();
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
				_password = txtPassword.Text;

				SettingsHelper settings = SettingsHelper.Current;
				settings.UserID = _loginID;
				
				if (OB2Security.IsStrongKey(_loginID))
				{
					settings.Password = "enc:" + OB2Security.Encrypt(_loginID, _password);
				}
				else
				{
					string key = _loginID + "12345678";
					settings.Password = "enc:" + OB2Security.Encrypt(key, _password);
				}
				
				settings.Save();

				string url = "http://" + item.Value + "/game/reg/login2.php?v=2" +
					"&login=" + _loginID + "&pass=" + _password;
				_uri = new Uri(url);
			}
			
			DialogResult = DialogResult.OK;
			Close();
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
			}
			else
			{
				txtSessionID.Enabled = false;
				txtLoginID.Enabled = true;
				txtPassword.Enabled = true;
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