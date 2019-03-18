using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace LDIFCreator
{
	/// <summary>
	/// Form1�� ���� ��� �����Դϴ�.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label lblStatus;
		/// <summary>
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Windows Form �����̳� ������ �ʿ��մϴ�.
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent�� ȣ���� ���� ������ �ڵ带 �߰��մϴ�.
			//
		}

		/// <summary>
		/// ��� ���� ��� ���ҽ��� �����մϴ�.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(109, 56);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "����";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(22, 16);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(248, 23);
			this.lblStatus.TabIndex = 1;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(292, 93);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lblStatus,
																		  this.button1});
			this.Name = "Form1";
			this.Text = "LDIFCreator";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			// @, *, |, \ : can not in AD
			// % : can not in DB
			String[] sSpecial = {"!", "#", "~", "^", "&", "."};

			FileInfo fi = new FileInfo("export.ldf");
			FileStream fs = fi.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
			StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

			int nCnt = 31000000;

			for (int i = 0; i < 100000; i++)
			{
				Random r = new Random();
				String sID = "nu" + sSpecial[r.Next(0, 5)] 
					+ String.Format("{0:00000000}", ++nCnt) + sSpecial[r.Next(0, 4)];	// cn

				String sText = String.Format("dn: CN={0},OU=Members,DC=bmt,DC=net\r\n"
					+ "changetype: add\r\n"
					+ "objectClass: top\r\n"
					+ "objectClass: netmarbleuser\r\n"
					+ "cn: {0}\r\n"
					+ "displayName: �׽���\r\n"
					+ "mail: {0}@netmarble.net\r\n"
					+ "uno: {1:00000000}\r\n"
					+ "upwd: pw{1:00000000}\r\n"
					+ "ugrade: 01\r\n"
					+ "uprofile: �׽���{1:00000000}�� �������Դϴ�.\r\n"
					+ "uzipcode: 135712\r\n"
					+ "uaddr: ����Ư���� ������ ��ġ4�� ������������ B�� 204ȣ\r\n"
					+ "uregdate: {2}\r\n"
					+ "resRegNum: 83051{3:00000000}"
					+ "\r\n", sID, nCnt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), nCnt + 1000000);
				sw.WriteLine(sText);
				lblStatus.Text = nCnt.ToString() + " written...";
			}
			sw.Close();
			fs.Close();
			lblStatus.Text = nCnt.ToString() + " written complete!";
		}
	}
}
