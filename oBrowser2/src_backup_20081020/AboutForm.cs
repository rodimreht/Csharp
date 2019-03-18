using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();
		}

		private void licenseButton_Click(object sender, EventArgs e)
		{
			Process.Start("http://creativecommons.org/licenses/by-sa/2.5/");
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void theWheelLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("mailto:thermidor@chol.com");
		}

		private void btnSMSTest_Click(object sender, EventArgs e)
		{
			string testmsg = "flight attack'>xxx의 <a href='#' onmouseover='return overlib(\"&lt;font color=white&gt;&lt;b&gt;전투기 1&lt;br&gt;&lt;/b&gt;&lt;/font&gt;\");' onmouseout='return nd();' class='attack'>함대</a><a href='#' title='전투기 1'></a>가 xxx <a href=\"javascript:showGalaxy(0,0,0)\" attack>[0:0:0]</a>행성에서 xxx <a href=# onclick=showGalaxy(0,0,0); >[0:0:0]</a>행성으로 접근중입니다. 임무 : 공격</span>";
			string hash = "012345678901234567890";
			if (string.IsNullOrEmpty(SettingsHelper.Current.AttackHash) ||
				(SettingsHelper.Current.AttackHash != hash))
			{
#if INTERNAL_USE
				// SMS 테스트
				SendSMS.Send();
				SendSMTP.Send(hash, testmsg);
				MessageBoxEx.Show("SMS+SMTP를 송신했습니다.", "SMS+SMTP 송신", MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
#else
				// SMTP 테스트
				SendSMTP.Send(hash, testmsg);
				MessageBoxEx.Show("SMTP를 송신했습니다.", "SMTP 송신", MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
#endif
			}
		}

		private void AboutForm_Load(object sender, EventArgs e)
		{
#if !INTERNAL_USE
			btnSMSTest.Text = "SMTP 송신 테스트";
#endif
		}
	}
}