using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NETS_iMan
{
	public partial class frmPicture : Form
	{
		private Image img;

		public Image Img
		{
			set { img = value; }
		}

		public frmPicture()
		{
			InitializeComponent();
		}

		private void frmPicture_Load(object sender, EventArgs e)
		{
			if (img == null)
			{
				Close();
				return;
			}

			this.ClientSize = img.Size;
			pictureBox1.Image = img;
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}