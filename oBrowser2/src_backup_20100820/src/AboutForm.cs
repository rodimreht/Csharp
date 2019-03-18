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
			Process.Start("http://blog.daum.net/_blog/ArticleCateList.do?blogid=0LQ3o&CATEGORYID=567039&dispkind=B2203");
		}

		private void btnSMSTest_Click(object sender, EventArgs e)
		{
			string testmsg = "flight attack'>xxx�� <a href='#' onmouseover='return overlib(\"&lt;font color=white&gt;&lt;b&gt;������ 1&lt;br&gt;&lt;/b&gt;&lt;/font&gt;\");' onmouseout='return nd();' class='attack'>�Դ�</a><a href='#' title='������ 1'></a>�� xxx <a href=\"javascript:showGalaxy(0,0,0)\" attack>[0:0:0]</a>�༺���� xxx <a href=# onclick=showGalaxy(0,0,0); >[0:0:0]</a>�༺���� �������Դϴ�. �ӹ� : ����</span>";
			string hash = "012345678901234567890";
			if (string.IsNullOrEmpty(SettingsHelper.Current.AttackHash) ||
				(SettingsHelper.Current.AttackHash != hash))
			{
#if INTERNAL_USE
				// SMS �׽�Ʈ
				// SendSMS.Send();
				SendSMTP.Send("��x����", hash, testmsg);
				MessageBoxEx.Show("SMS+SMTP�� �۽��߽��ϴ�.", "SMS+SMTP �۽�", MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
#else
				// SMTP �׽�Ʈ
				SendSMTP.Send("��x����", hash, testmsg);
				MessageBoxEx.Show("SMTP�� �۽��߽��ϴ�.", "SMTP �۽�", MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
#endif
			}
		}

		private void AboutForm_Load(object sender, EventArgs e)
		{
			btnSMSTest.Text = "SMTP �۽� �׽�Ʈ";
#if !INTERNAL_USE
#endif
		}
	}
}