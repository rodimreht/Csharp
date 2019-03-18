using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ex_Security
{
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtUnencryptedFile;
		private System.Windows.Forms.TextBox txtEncryptedFile;
		private System.Windows.Forms.TextBox txtKeyPassword;
		private System.Windows.Forms.TextBox txtIVPassword;
		private System.Windows.Forms.Button btnEncrypt;
		private System.Windows.Forms.Button btnDecrypt;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.StatusBar sbEncryptionStatus;

		private System.ComponentModel.Container components = null;

		public Form1()
		{
			InitializeComponent();
		}
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
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtUnencryptedFile = new System.Windows.Forms.TextBox();
			this.txtEncryptedFile = new System.Windows.Forms.TextBox();
			this.txtKeyPassword = new System.Windows.Forms.TextBox();
			this.txtIVPassword = new System.Windows.Forms.TextBox();
			this.btnEncrypt = new System.Windows.Forms.Button();
			this.btnDecrypt = new System.Windows.Forms.Button();
			this.sbEncryptionStatus = new System.Windows.Forms.StatusBar();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "암호화 되지 않은 파일";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 14);
			this.label2.TabIndex = 1;
			this.label2.Text = "암호화 된 파일";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 14);
			this.label3.TabIndex = 2;
			this.label3.Text = "Key 암호";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 152);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(44, 14);
			this.label4.TabIndex = 3;
			this.label4.Text = "IV 암호";
			// 
			// txtUnencryptedFile
			// 
			this.txtUnencryptedFile.Location = new System.Drawing.Point(160, 16);
			this.txtUnencryptedFile.Name = "txtUnencryptedFile";
			this.txtUnencryptedFile.Size = new System.Drawing.Size(208, 21);
			this.txtUnencryptedFile.TabIndex = 4;
			this.txtUnencryptedFile.Text = "";
			// 
			// txtEncryptedFile
			// 
			this.txtEncryptedFile.Location = new System.Drawing.Point(160, 56);
			this.txtEncryptedFile.Name = "txtEncryptedFile";
			this.txtEncryptedFile.Size = new System.Drawing.Size(208, 21);
			this.txtEncryptedFile.TabIndex = 5;
			this.txtEncryptedFile.Text = "";
			// 
			// txtKeyPassword
			// 
			this.txtKeyPassword.Location = new System.Drawing.Point(160, 112);
			this.txtKeyPassword.MaxLength = 8;
			this.txtKeyPassword.Name = "txtKeyPassword";
			this.txtKeyPassword.Size = new System.Drawing.Size(128, 21);
			this.txtKeyPassword.TabIndex = 6;
			this.txtKeyPassword.Text = "";
			// 
			// txtIVPassword
			// 
			this.txtIVPassword.Location = new System.Drawing.Point(160, 144);
			this.txtIVPassword.MaxLength = 8;
			this.txtIVPassword.Name = "txtIVPassword";
			this.txtIVPassword.Size = new System.Drawing.Size(128, 21);
			this.txtIVPassword.TabIndex = 7;
			this.txtIVPassword.Text = "";
			// 
			// btnEncrypt
			// 
			this.btnEncrypt.Location = new System.Drawing.Point(48, 216);
			this.btnEncrypt.Name = "btnEncrypt";
			this.btnEncrypt.TabIndex = 8;
			this.btnEncrypt.Text = "암호화";
			this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
			// 
			// btnDecrypt
			// 
			this.btnDecrypt.Location = new System.Drawing.Point(152, 216);
			this.btnDecrypt.Name = "btnDecrypt";
			this.btnDecrypt.TabIndex = 9;
			this.btnDecrypt.Text = "암호 해독";
			this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
			// 
			// sbEncryptionStatus
			// 
			this.sbEncryptionStatus.Location = new System.Drawing.Point(0, 256);
			this.sbEncryptionStatus.Name = "sbEncryptionStatus";
			this.sbEncryptionStatus.Size = new System.Drawing.Size(384, 22);
			this.sbEncryptionStatus.TabIndex = 10;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(256, 216);
			this.button1.Name = "button1";
			this.button1.TabIndex = 11;
			this.button1.Text = "CLOSE";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(384, 278);
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
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form2());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			txtUnencryptedFile.Text = @"E:\Temp\test1.txt";
			txtEncryptedFile.Text = @"E:\Temp\test2.txt";
			txtKeyPassword.Text = "thermido";
			txtIVPassword.Text = "thermido";
		}

		private void btnEncrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CDES cdes = new CDES();

				byte[] byteKey;
				byteKey = cdes.GetKeyByteArray(txtKeyPassword.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = cdes.GetKeyByteArray(txtIVPassword.Text);

				cdes.EncryptDecryptFile(txtUnencryptedFile.Text, txtEncryptedFile.Text, byteKey, byteInitializationVector, "E");

				sbEncryptionStatus.Text = "64bit(56bit) 비밀키를 사용한 대칭키 암호화 알고리즘";
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
				CDES cdes = new CDES();

				byte[] byteKey;
				byteKey = cdes.GetKeyByteArray(txtKeyPassword.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = cdes.GetKeyByteArray(txtIVPassword.Text);

				cdes.EncryptDecryptFile(txtEncryptedFile.Text, txtUnencryptedFile.Text, byteKey, byteInitializationVector, "D");

				sbEncryptionStatus.Text = "DES 복호화된 내용이 파일로 저장되었습니다.";
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

	}
}
