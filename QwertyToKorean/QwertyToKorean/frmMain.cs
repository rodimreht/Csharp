using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QwertyToKorean
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void txtQwerty_KeyUp(object sender, KeyEventArgs e)
		{
			txtKorean.Text = QwertyToKorean.Convert(txtQwerty.Text);
		}

		private void txtKorean_KeyUp(object sender, KeyEventArgs e)
		{
			txtQwerty.Text = QwertyToKorean.Revert(txtKorean.Text);
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			this.Left = 0;
			this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
		}
	}
}