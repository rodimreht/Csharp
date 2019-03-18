using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace TripleDES
{
	/// <summary>
	/// TripleDES ��ȣȭ �˰��� (���ڿ�) ��
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtIV;
		private System.Windows.Forms.TextBox txtKey;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button cmdDecrypt;
		private System.Windows.Forms.Button cmdEncrypt;
		private System.Windows.Forms.TextBox txtOriginal;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.TextBox lblResult;

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
			this.cmdDecrypt = new System.Windows.Forms.Button();
			this.cmdEncrypt = new System.Windows.Forms.Button();
			this.txtIV = new System.Windows.Forms.TextBox();
			this.txtKey = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtOriginal = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblResult = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// cmdDecrypt
			// 
			this.cmdDecrypt.Location = new System.Drawing.Point(319, 384);
			this.cmdDecrypt.Name = "cmdDecrypt";
			this.cmdDecrypt.TabIndex = 1;
			this.cmdDecrypt.Text = "��ȣȭ";
			this.cmdDecrypt.Click += new System.EventHandler(this.cmdDecrypt_Click);
			// 
			// cmdEncrypt
			// 
			this.cmdEncrypt.Location = new System.Drawing.Point(215, 384);
			this.cmdEncrypt.Name = "cmdEncrypt";
			this.cmdEncrypt.TabIndex = 0;
			this.cmdEncrypt.Text = "��ȣȭ";
			this.cmdEncrypt.Click += new System.EventHandler(this.cmdEncrypt_Click);
			// 
			// txtIV
			// 
			this.txtIV.Location = new System.Drawing.Point(200, 40);
			this.txtIV.MaxLength = 24;
			this.txtIV.Name = "txtIV";
			this.txtIV.Size = new System.Drawing.Size(304, 21);
			this.txtIV.TabIndex = 15;
			this.txtIV.Tag = "";
			this.txtIV.Text = "";
			// 
			// txtKey
			// 
			this.txtKey.Location = new System.Drawing.Point(200, 8);
			this.txtKey.MaxLength = 24;
			this.txtKey.Name = "txtKey";
			this.txtKey.Size = new System.Drawing.Size(304, 21);
			this.txtKey.TabIndex = 14;
			this.txtKey.Tag = "";
			this.txtKey.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(112, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 14);
			this.label4.TabIndex = 13;
			this.label4.Text = "�ʱ�ȭ ����";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(112, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 14);
			this.label3.TabIndex = 12;
			this.label3.Text = "���Ű";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(16, 104);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new System.Drawing.Size(272, 232);
			this.txtOriginal.TabIndex = 18;
			this.txtOriginal.Text = "";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 88);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 17);
			this.label1.TabIndex = 19;
			this.label1.Text = "���� ���ڿ�";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(320, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 17);
			this.label2.TabIndex = 21;
			this.label2.Text = "��ȣȭ�� ����Ʈ�迭";
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(108, 352);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(392, 16);
			this.lblStatus.TabIndex = 23;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblResult
			// 
			this.lblResult.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.lblResult.Location = new System.Drawing.Point(320, 104);
			this.lblResult.Multiline = true;
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(272, 232);
			this.lblResult.TabIndex = 24;
			this.lblResult.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(608, 413);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.txtIV);
			this.Controls.Add(this.txtKey);
			this.Controls.Add(this.cmdDecrypt);
			this.Controls.Add(this.cmdEncrypt);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Name = "Form1";
			this.Text = "TripleDES ��ȣȭ (���ڿ�)";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());	// ���ڿ� ��ȣȭ / ��ȣȭ
			//Application.Run(new Form2());	// ���� ��ȣȭ / ��ȣȭ
			//Application.Run(new Form3());	// ��Ʈ�� ��ȣȭ / ��ȣȭ
		}

		private void cmdEncrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CTripleDES ctdes = new CTripleDES();
				
				byte[] byteKey;
				byteKey = ctdes.GetKeyByteArray(txtKey.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = ctdes.GetKeyByteArray(txtIV.Text);

				string sTemp;

				ctdes.EncryptDecryptString(txtOriginal.Text, out sTemp, byteKey, byteInitializationVector, "E");

				lblResult.Text = sTemp;
				lblStatus.Text = "DES(56bit)�� ���� �����ؼ� ����ϴ� ��ȣȭ �˰���";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdDecrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CTripleDES ctdes = new CTripleDES();

				byte[] byteKey;
				byteKey = ctdes.GetKeyByteArray(txtKey.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = ctdes.GetKeyByteArray(txtIV.Text);

				string sTemp;

				ctdes.EncryptDecryptString(lblResult.Text, out sTemp, byteKey, byteInitializationVector, "D");

				txtOriginal.Text = sTemp;
				lblStatus.Text = "TripleDES ��ȣȭ�Ǿ����ϴ�.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			txtKey.Text = "thermidor";
			txtIV.Text = "thermidor";
		}
	}
}
