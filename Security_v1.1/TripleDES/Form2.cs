using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace TripleDES
{
	/// <summary>
	/// TripleDES ��ȣȭ �˰��� (����) ��
	/// </summary>
	public class Form2 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.StatusBar sbEncryptionStatus;
		private System.Windows.Forms.Button btnDecrypt;
		private System.Windows.Forms.Button btnEncrypt;
		private System.Windows.Forms.TextBox txtIVPassword;
		private System.Windows.Forms.TextBox txtKeyPassword;
		private System.Windows.Forms.TextBox txtEncryptedFile;
		private System.Windows.Forms.TextBox txtUnencryptedFile;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;

		private const int TDES_BIT_LENGTH = 192;	// 192��Ʈ ��ȣȭ
		//private const int TDES_BIT_LENGTH = 128;

		/// <summary>
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form2()
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
				if(components != null)
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
			this.sbEncryptionStatus = new System.Windows.Forms.StatusBar();
			this.btnDecrypt = new System.Windows.Forms.Button();
			this.btnEncrypt = new System.Windows.Forms.Button();
			this.txtIVPassword = new System.Windows.Forms.TextBox();
			this.txtKeyPassword = new System.Windows.Forms.TextBox();
			this.txtEncryptedFile = new System.Windows.Forms.TextBox();
			this.txtUnencryptedFile = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(256, 200);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 32);
			this.button1.TabIndex = 23;
			this.button1.Text = "�ݱ�";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// sbEncryptionStatus
			// 
			this.sbEncryptionStatus.Location = new System.Drawing.Point(0, 247);
			this.sbEncryptionStatus.Name = "sbEncryptionStatus";
			this.sbEncryptionStatus.Size = new System.Drawing.Size(392, 22);
			this.sbEncryptionStatus.TabIndex = 22;
			// 
			// btnDecrypt
			// 
			this.btnDecrypt.Location = new System.Drawing.Point(152, 200);
			this.btnDecrypt.Name = "btnDecrypt";
			this.btnDecrypt.Size = new System.Drawing.Size(75, 32);
			this.btnDecrypt.TabIndex = 21;
			this.btnDecrypt.Text = "��ȣȭ";
			this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
			// 
			// btnEncrypt
			// 
			this.btnEncrypt.Location = new System.Drawing.Point(48, 200);
			this.btnEncrypt.Name = "btnEncrypt";
			this.btnEncrypt.Size = new System.Drawing.Size(75, 32);
			this.btnEncrypt.TabIndex = 20;
			this.btnEncrypt.Text = "��ȣȭ";
			this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
			// 
			// txtIVPassword
			// 
			this.txtIVPassword.Location = new System.Drawing.Point(160, 136);
			this.txtIVPassword.MaxLength = 24;
			this.txtIVPassword.Name = "txtIVPassword";
			this.txtIVPassword.Size = new System.Drawing.Size(128, 21);
			this.txtIVPassword.TabIndex = 19;
			this.txtIVPassword.Text = "";
			// 
			// txtKeyPassword
			// 
			this.txtKeyPassword.Location = new System.Drawing.Point(160, 104);
			this.txtKeyPassword.MaxLength = 24;
			this.txtKeyPassword.Name = "txtKeyPassword";
			this.txtKeyPassword.Size = new System.Drawing.Size(128, 21);
			this.txtKeyPassword.TabIndex = 18;
			this.txtKeyPassword.Text = "";
			// 
			// txtEncryptedFile
			// 
			this.txtEncryptedFile.Location = new System.Drawing.Point(160, 48);
			this.txtEncryptedFile.Name = "txtEncryptedFile";
			this.txtEncryptedFile.Size = new System.Drawing.Size(208, 21);
			this.txtEncryptedFile.TabIndex = 17;
			this.txtEncryptedFile.Text = "";
			// 
			// txtUnencryptedFile
			// 
			this.txtUnencryptedFile.Location = new System.Drawing.Point(160, 8);
			this.txtUnencryptedFile.Name = "txtUnencryptedFile";
			this.txtUnencryptedFile.Size = new System.Drawing.Size(208, 21);
			this.txtUnencryptedFile.TabIndex = 16;
			this.txtUnencryptedFile.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 14);
			this.label4.TabIndex = 15;
			this.label4.Text = "�ʱ�ȭ ����";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 14);
			this.label3.TabIndex = 14;
			this.label3.Text = "���Ű";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 14);
			this.label2.TabIndex = 13;
			this.label2.Text = "��ȣȭ �� ����";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 14);
			this.label1.TabIndex = 12;
			this.label1.Text = "��ȣȭ ���� ���� ����";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Form2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(392, 269);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button1,
																		  this.sbEncryptionStatus,
																		  this.btnDecrypt,
																		  this.btnEncrypt,
																		  this.txtIVPassword,
																		  this.txtKeyPassword,
																		  this.txtEncryptedFile,
																		  this.txtUnencryptedFile,
																		  this.label4,
																		  this.label3,
																		  this.label2,
																		  this.label1});
			this.Name = "Form2";
			this.Text = "TripleDES ��ȣȭ (����)";
			this.Load += new System.EventHandler(this.Form2_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnEncrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CTripleDES ctdes = new CTripleDES();

				byte[] byteKey;
				byteKey = ctdes.GetKeyByteArray(txtKeyPassword.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = ctdes.GetKeyByteArray(txtIVPassword.Text);

				ctdes.EncryptDecryptFile(txtUnencryptedFile.Text, txtEncryptedFile.Text, byteKey, byteInitializationVector, "E");
			}
			catch (Exception er)
			{
				MessageBox.Show(er.Message);
			}
		}

		private void btnDecrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CTripleDES ctdes = new CTripleDES();

				byte[] byteKey;
				byteKey = ctdes.GetKeyByteArray(txtKeyPassword.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = ctdes.GetKeyByteArray(txtIVPassword.Text);

				ctdes.EncryptDecryptFile(txtEncryptedFile.Text, txtUnencryptedFile.Text, byteKey, byteInitializationVector, "D");
			}
			catch (Exception er)
			{
				MessageBox.Show(er.Message);
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void Form2_Load(object sender, System.EventArgs e)
		{
			txtUnencryptedFile.Text = @"E:\Temp\test1.txt";
			txtEncryptedFile.Text = @"E:\Temp\test3.txt";
			txtKeyPassword.Text = "thermidor";
			txtIVPassword.Text = "thermidor";
		}
	}
}
