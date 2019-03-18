using System;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NETS_iMan
{
	internal partial class LoginForm : Form
	{
		private LoginInfo m_info;
		
		public LoginForm()
		{
			InitializeComponent();
		}

		public LoginInfo LoginInformation
		{
			get { return m_info; }
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			SettingsHelper settings = SettingsHelper.Current;

			// 자동실행 옵션이 있으면 레지스트리 업데이트
			if (settings.AutoStart)
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
				if (key != null)
				{
					string s = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
					s += @"\NETS\NETS-ⓘMan v1.0.appref-ms";
					key.SetValue("NETS_iMan", "\"" + s + "\"");
					key.Close();
				}
				else
				{
					settings.AutoStart = false;
					settings.Save();
				}
			}

			if (!settings.AutoStart) chkAutoStart.Checked = false;
			if (!settings.RememberPwd)
			{
				chkRemPwd.Checked = false;
				return;
			}

			string userID = settings.UserID;
			if (!string.IsNullOrEmpty(userID)) txtLoginID.Text = userID;

			if (!string.IsNullOrEmpty(settings.Password))
			{
				if (NISecurity.IsStrongKey(userID))
				{
					string pwd = settings.Password.Replace("enc:", "");
					txtPassword.Text = NISecurity.Decrypt(userID, pwd);
				}
				else
				{
					string key = userID + "12345678";
					string pwd = settings.Password.Replace("enc:", "");
					txtPassword.Text = NISecurity.Decrypt(key, pwd);
				}
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (txtLoginID.Text.Length == 0)
			{
				MessageBoxEx.Show("로그인ID를 입력하세요.",
								  "로그인",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning,
								  5000);
				txtLoginID.Focus();
				return;
			}
			if (txtPassword.Text.Length == 0)
			{
				MessageBoxEx.Show("패스워드를 입력하세요.",
								  "로그인",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning,
								  5000);
				txtPassword.Focus();
				return;
			}
			
			string id = txtLoginID.Text;
			string newID = txtLoginID.Text.Replace("|", "&#7C;");
			string pwd = txtPassword.Text;

			SettingsHelper settings = SettingsHelper.Current;
			if (chkRemPwd.Checked)
			{
				settings.RememberPwd = true;

				settings.UserID = newID;
				if (NISecurity.IsStrongKey(id))
				{
					settings.Password = "enc:" + NISecurity.Encrypt(id, pwd);
				}
				else
				{
					string key = id + "12345678";
					settings.Password = "enc:" + NISecurity.Encrypt(key, pwd);
				}
			}
			else if (settings.RememberPwd)
			{
				settings.RememberPwd = false;

				settings.UserID = "";
				settings.Password = "";
			}
			settings.Save(); //-- 환경설정 저장

			string url = "http://dev.nets.co.kr/im25/webservice/Access/Logon.aspx";
			Uri u = new Uri(url);
			
			string s = "tokens=&url=http%3A%2F%2Fdev.nets.co.kr%2FIM25%2FWebAdmin%2FLogin%2FLogin.aspx%3Ffrom_url%3Dhttp%253a%252f%252fdev.nets.co.kr%252fIM25%252fWebAdmin%252fdefault.aspx&userid=" + id + "&passwd=" + pwd + "&x=38&y=34";
			byte[] b = Encoding.Default.GetBytes(s);

			m_info = new LoginInfo(id, pwd, u, b);

			DialogResult = DialogResult.OK;
			Close();
		}
		
		private void txtPassword_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				okButton_Click(null, null);
		}

		private void LoginForm_Activated(object sender, EventArgs e)
		{
			if (((txtLoginID.Text.Length > 0)) && (txtPassword.Text.Length > 0))
				okButton.Focus();
		}

		private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
			if (key != null)
			{
				SettingsHelper help = SettingsHelper.Current;
				if (!help.AutoStart)
				{
					if (chkAutoStart.Checked)
					{
						string s = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
						s += @"\NETS\NETS-ⓘMan v1.0.appref-ms";
						key.SetValue("NETS_iMan", "\"" + s + "\"");
						help.AutoStart = true;
						help.Save();
					}
				}
				else
				{
					if (!chkAutoStart.Checked)
					{
						if (key.GetValue("NETS_iMan") != null) key.DeleteValue("NETS_iMan");
						help.AutoStart = false;
						help.Save();
					}
				}
			}
			else
			{
				MessageBoxEx.Show("자동실행 등록상태를 변경할 수 없습니다.",
								  "자동실행 등록",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning,
								  5000);
				SettingsHelper.Current.AutoStart = false;
				SettingsHelper.Current.Save();
			}
		}
	}

	internal class LoginInfo
	{
		private readonly string _loginID;
		private readonly string _password;
		private readonly Uri _uri;
		private readonly byte[] _postData;
		private string _userName;
		private bool _absent;
		private bool _busy;

		public LoginInfo(string id, string pwd, Uri uri, byte[] pData)
		{
			_loginID = id;
			_password = pwd;
			_uri = uri;
			_postData = pData;
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

		public byte[] PostData
		{
			get { return _postData; }
		}

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		public bool IsAbsent
		{
			get { return _absent; }
			set { _absent = value; }
		}

		public bool IsBusy
		{
			get { return _busy; }
			set { _busy = value; }
		}

		public byte[] GetProfilePostData(string s)
		{
			string ps = string.Format("tokens=&url=http%3A%2F%2Fdev.nets.co.kr%2FIM25%2FWebAdmin%2FLogin%2FLogin.aspx%3Ffrom_url%3Dhttp%253a%252f%252fdev.nets.co.kr%252fIM25%252fWebAdmin%252fDefault.aspx%253fsuburl%253dhttp%25253a%25252f%25252fsso.nets.co.kr%25252fJoin%25252fv2%25252fMemberView.aspx%25253fuid%25253d{0}&userid={1}&passwd={2}&x=35&y=42",
				s,
				_loginID,
				_password);
			byte[] pb = Encoding.Default.GetBytes(ps);
			return pb;
		}

		public byte[] GetMListPostData()
		{
			string ps = string.Format("tokens=&url=http%3A%2F%2Fdev.nets.co.kr%2FIM25%2FWebAdmin%2FLogin%2FLogin.aspx%3Ffrom_url%3Dhttp%253a%252f%252fdev.nets.co.kr%252fIM25%252fWebAdmin%252fDefault.aspx%253fsuburl%253dhttp%25253a%25252f%25252fsso.nets.co.kr%25252fJoin%25252fv2%25252fMemberList.aspx&userid={0}&passwd={1}&x=35&y=42",
				_loginID,
				_password);
			byte[] pb = Encoding.Default.GetBytes(ps);
			return pb;
		}

		public byte[] GetDListPostData()
		{
			string ps = string.Format("tokens=&url=http%3A%2F%2Fdev.nets.co.kr%2FIM25%2FWebAdmin%2FLogin%2FLogin.aspx%3Ffrom_url%3Dhttp%253a%252f%252fdev.nets.co.kr%252fIM25%252fWebAdmin%252fDefault.aspx%253fsuburl%253dhttp%25253a%25252f%25252fsso.nets.co.kr%25252fJoin%25252fv2%25252fNetsDList.aspx&userid={0}&passwd={1}&x=35&y=42",
				_loginID,
				_password);
			byte[] pb = Encoding.Default.GetBytes(ps);
			return pb;
		}

		public byte[] GetQAPostData(string pwd)
		{
			string password = string.IsNullOrEmpty(pwd) ? _password : pwd;
			string ps = string.Format("loginID={0}&password={1}",
				_loginID,
				password);
			byte[] pb = Encoding.Default.GetBytes(ps);
			return pb;
		}
	}
}