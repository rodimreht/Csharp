using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace MD5
{
	/// <summary>
	/// Form1�� ���� ��� �����Դϴ�.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label lblResult;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOriginal;
		private System.Windows.Forms.Button cmdDecrypt;
		private System.Windows.Forms.Button cmdMD5;
		private System.Windows.Forms.Button cmdSHA;
		private System.Windows.Forms.Button cmdSHA256;
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
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblResult = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOriginal = new System.Windows.Forms.TextBox();
			this.cmdDecrypt = new System.Windows.Forms.Button();
			this.cmdMD5 = new System.Windows.Forms.Button();
			this.cmdSHA = new System.Windows.Forms.Button();
			this.cmdSHA256 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(104, 112);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(392, 16);
			this.lblStatus.TabIndex = 41;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblResult
			// 
			this.lblResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblResult.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblResult.Location = new System.Drawing.Point(312, 40);
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(272, 48);
			this.lblResult.TabIndex = 40;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(312, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 12);
			this.label2.TabIndex = 39;
			this.label2.Text = "��ȣȭ�� ����Ʈ�迭";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 12);
			this.label1.TabIndex = 38;
			this.label1.Text = "���� ���ڿ�";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(16, 40);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new System.Drawing.Size(272, 48);
			this.txtOriginal.TabIndex = 37;
			// 
			// cmdDecrypt
			// 
			this.cmdDecrypt.Enabled = false;
			this.cmdDecrypt.Location = new System.Drawing.Point(360, 144);
			this.cmdDecrypt.Name = "cmdDecrypt";
			this.cmdDecrypt.Size = new System.Drawing.Size(200, 23);
			this.cmdDecrypt.TabIndex = 36;
			this.cmdDecrypt.Text = "��ȣȭ�� �������� �ʽ��ϴ�.";
			// 
			// cmdMD5
			// 
			this.cmdMD5.Location = new System.Drawing.Point(40, 144);
			this.cmdMD5.Name = "cmdMD5";
			this.cmdMD5.Size = new System.Drawing.Size(75, 23);
			this.cmdMD5.TabIndex = 35;
			this.cmdMD5.Text = "MD5 �ؽ�";
			this.cmdMD5.Click += new System.EventHandler(this.cmdMD5_Click);
			// 
			// cmdSHA
			// 
			this.cmdSHA.Location = new System.Drawing.Point(136, 144);
			this.cmdSHA.Name = "cmdSHA";
			this.cmdSHA.Size = new System.Drawing.Size(88, 23);
			this.cmdSHA.TabIndex = 42;
			this.cmdSHA.Text = "SHA1 �ؽ�";
			this.cmdSHA.Click += new System.EventHandler(this.cmdSHA_Click);
			// 
			// cmdSHA256
			// 
			this.cmdSHA256.Location = new System.Drawing.Point(248, 144);
			this.cmdSHA256.Name = "cmdSHA256";
			this.cmdSHA256.Size = new System.Drawing.Size(88, 23);
			this.cmdSHA256.TabIndex = 43;
			this.cmdSHA256.Text = "SHA256 �ؽ�";
			this.cmdSHA256.Click += new System.EventHandler(this.cmdSHA256_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(600, 181);
			this.Controls.Add(this.cmdSHA256);
			this.Controls.Add(this.cmdSHA);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.cmdDecrypt);
			this.Controls.Add(this.cmdMD5);
			this.Name = "Form1";
			this.Text = "MD5-SHA1 �ؽ� (���ڿ�)";
			this.ResumeLayout(false);
			this.PerformLayout();

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

		private void cmdMD5_Click(object sender, System.EventArgs e)
		{
			string sTemp;

			CMD5 cmd5 = new CMD5();
			cmd5.EncryptDecryptString(txtOriginal.Text, out sTemp);

			Clipboard.SetText(sTemp);

			lblResult.Text = sTemp;
			lblStatus.Text = "MD5 �ؽ� : �Է� ���ڿ��� ���� 128bit�� �ؽ��ڵ� ����";
		}

		private void cmdSHA_Click(object sender, System.EventArgs e)
		{
			string sTemp;

			CSHA1 csha1 = new CSHA1();
			csha1.EncryptDecryptString(txtOriginal.Text, out sTemp);

			Clipboard.SetText(sTemp);

			lblResult.Text = sTemp;
			lblStatus.Text = "SHA1 �ؽ� : �Է� ���ڿ��� ���� 160bit�� �ؽ��ڵ� ����";
		}

		private void cmdSHA256_Click(object sender, System.EventArgs e)
		{
			string sTemp;

			CSHA256 csha256 = new CSHA256();
			csha256.EncryptDecryptString(txtOriginal.Text, out sTemp);

			Clipboard.SetText(sTemp);

			lblResult.Text = sTemp;
			lblStatus.Text = "SHA1 �ؽ� : �Է� ���ڿ��� ���� 160bit�� �ؽ��ڵ� ����";
		}
	}
}
