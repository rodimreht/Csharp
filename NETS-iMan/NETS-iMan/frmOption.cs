using System;
using System.Windows.Forms;

namespace NETS_iMan
{
	public partial class frmOption : Form
	{
		private ToolTip toolTip;
		private ToolTip qaTooltip;
		private bool m_changedir;
		private bool m_refresh;

		public frmOption()
		{
			InitializeComponent();
		}

		public bool ChangeDir
		{
			get { return m_changedir; }
		}

		public bool TreeRefresh
		{
			get { return m_refresh; }
		}

		private void frmOption_Load(object sender, EventArgs e)
		{
			m_changedir = false;
			m_refresh = false;

			SettingsHelper setting = SettingsHelper.Current;
			txtLogPath.Text = setting.LogPath ?? "";
			chkAutoStart.Checked = setting.AutoStart;
			chkRemPwd.Checked = setting.RememberPwd;
			chkShowOnline.Checked = setting.ShowOnline;
			chkLoginAlarm.Checked = setting.OffLoginAlarm;

			if (!string.IsNullOrEmpty(setting.NETSQAPassword))
			{
				if (NISecurity.IsStrongKey(setting.UserID))
				{
					string pwd = setting.NETSQAPassword.Replace("enc:", "");
					txtQAPwd.Text = NISecurity.Decrypt(setting.UserID, pwd);
				}
				else
				{
					string key = setting.UserID + "12345678";
					string pwd = setting.NETSQAPassword.Replace("enc:", "");
					txtQAPwd.Text = NISecurity.Decrypt(key, pwd);
				}
			}
			int opacity = setting.FormOpacity;
			if (opacity < 10) opacity = 10;
			txtOpacity.Value = opacity;
			trackBar.Value = opacity;

			toolTip = new ToolTip();
			toolTip.SetToolTip(chkShowOnline, "조직도 트리에 온라인 직원 목록만 표시합니다.(현재 선택시점 기준; 이후 상태가 변경된 직원은 목록에 자동으로 반영되지 않습니다.)");
			qaTooltip = new ToolTip();
			qaTooltip.SetToolTip(lblQA, "NETS*QA 사이트에 곧바로 접속하기 위한 패스워드를 설정합니다.(기본 패스워드와 동일할 경우에는 따로 설정하지 않아도 됩니다.)");
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
			{
				txtLogPath.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			SettingsHelper setting = SettingsHelper.Current;
			if (setting.LogPath != txtLogPath.Text)
			{
				m_changedir = true;
				setting.LogPath = txtLogPath.Text;
			}
			setting.AutoStart = chkAutoStart.Checked;
			setting.RememberPwd = chkRemPwd.Checked;
			if (!setting.RememberPwd)
			{
				setting.UserID = "";
				setting.Password = "";
			}

			if (setting.ShowOnline != chkShowOnline.Checked)
			{
				m_refresh = true;
				setting.ShowOnline = chkShowOnline.Checked;
			}
			setting.OffLoginAlarm = chkLoginAlarm.Checked;

			string pwd = txtQAPwd.Text;
			if (NISecurity.IsStrongKey(setting.UserID))
			{
				setting.NETSQAPassword = "enc:" + NISecurity.Encrypt(setting.UserID, pwd);
			}
			else
			{
				string key = setting.UserID + "12345678";
				setting.NETSQAPassword = "enc:" + NISecurity.Encrypt(key, pwd);
			}

			int opacity = (int)txtOpacity.Value;
			if (opacity < 10) opacity = 10;
			setting.FormOpacity = opacity;

			setting.Save();
			Close();
		}

		private void trackBar_Scroll(object sender, EventArgs e)
		{
			txtOpacity.Value = trackBar.Value;
		}

		private void txtOpacity_ValueChanged(object sender, EventArgs e)
		{
			trackBar.Value = (int)txtOpacity.Value;

			double opacity = (double)txtOpacity.Value / 100;
			if (opacity < 0.1) opacity = 0.1;

			foreach (Form frm in Application.OpenForms)
			{
				if (!(frm is frmOption) && !(frm is OfflineMessage)) frm.Opacity = opacity;
			}
		}
	}
}
