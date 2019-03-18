using System;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class OptionsForm : Form
	{
		public OptionsForm()
		{
			InitializeComponent();
		}

		private void OptionsForm_Load(object sender, EventArgs e)
		{
			SettingsHelper helper = SettingsHelper.Current;
			this.txtRate.Text = helper.RefreshRate.ToString();
			this.txtRateMax.Text = helper.RefreshRateMax.ToString();
			this.txtSMSphoneNum.Text = helper.SMSphoneNum;
			this.applySummerTime.Checked = helper.ApplySummerTime;
			this.useFireFox.Checked = helper.UseFireFox;
			this.txtFireFoxDir.Text = helper.FireFoxDirectory;
			this.chkShowInLeftBottom.Checked = !helper.ShowInLeftBottom;

#if !INTERNAL_USE
			txtSMSphoneNum.Visible = false;
			label4.Visible = false;
#endif

			SmtpMailInfo info = SmtpMailInfo.ParseInfo(helper.SmtpMail);
			if (info != null)
			{
				txtMailAddress.Text = info.MailAddress;
				txtMailServer.Text = info.MailServer;
				txtMailUserID.Text = info.UserID;
				txtMailPassword.Text = info.Pwd;
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			SettingsHelper helper = SettingsHelper.Current;
			helper.RefreshRate = int.Parse(this.txtRate.Text);
			helper.RefreshRateMax = int.Parse(this.txtRateMax.Text);
#if INTERNAL_USE
			helper.SMSphoneNum = this.txtSMSphoneNum.Text;
#endif
			helper.ApplySummerTime = this.applySummerTime.Checked;
			helper.UseFireFox = this.useFireFox.Checked;
			helper.FireFoxDirectory = this.txtFireFoxDir.Text;
			helper.ShowInLeftBottom = !this.chkShowInLeftBottom.Checked;

			SmtpMailInfo info = new SmtpMailInfo();
			info.MailAddress = txtMailAddress.Text;
			info.MailServer = txtMailServer.Text;
			info.UserID = txtMailUserID.Text;
			info.Pwd = txtMailPassword.Text;

			helper.SmtpMail = SmtpMailInfo.ToString(info);

			helper.Save();
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cmdOpenFireFox_Click(object sender, EventArgs e)
		{
			openFileDialog1.Title = "파이어폭스";
			openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			openFileDialog1.Filter = "파이어폭스 실행파일|FireFox.exe";
			if (DialogResult.OK == openFileDialog1.ShowDialog())
			{
				txtFireFoxDir.Text = openFileDialog1.FileName;
			}
		}

		private void useFireFox_CheckedChanged(object sender, EventArgs e)
		{
			txtFireFoxDir.Enabled = useFireFox.Checked;
			cmdOpenFireFox.Enabled = useFireFox.Checked;
		}

	}
}