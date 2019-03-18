using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HeroLogger
{
	public partial class Form1 : Form
	{
		private string m_execInfo;

		public Form1(string arg)
		{
			m_execInfo = arg;
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			textBox1.Text = m_execInfo;
		}
	}
}