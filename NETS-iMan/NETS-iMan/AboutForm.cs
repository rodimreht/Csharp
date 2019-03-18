using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Windows.Forms;

namespace NETS_iMan
{
	public partial class AboutForm : Form
	{
		private int count;
		private Timer timer;

		public AboutForm()
		{
			InitializeComponent();
		}

		private void licenseButton_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.nets.co.kr/");
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void AboutForm_Load(object sender, EventArgs e)
		{
			lblVersion.Text = Application.ProductVersion;
#if !DEBUG
			lblDistVersion.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
#endif
			timer = new Timer();
			timer.Interval = 1000;
			timer.Tick += timer_Tick;
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			count++;

			if (count >= 30)
			{
				timer.Stop();
				Close();
			}
		}
	}
}