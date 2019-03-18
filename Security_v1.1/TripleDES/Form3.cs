using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace TripleDES
{
	/// <summary>
	/// TripleDES ��ȣȭ �˰��� (��Ʈ��) ��
	/// </summary>
	public class Form3 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label lblResult;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOriginal;
		private System.Windows.Forms.Button cmdDecrypt;
		private System.Windows.Forms.Button cmdEncrypt;
		private System.Windows.Forms.TextBox txtIV;
		private System.Windows.Forms.TextBox txtKey;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form3()
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
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblResult = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOriginal = new System.Windows.Forms.TextBox();
			this.cmdDecrypt = new System.Windows.Forms.Button();
			this.cmdEncrypt = new System.Windows.Forms.Button();
			this.txtIV = new System.Windows.Forms.TextBox();
			this.txtKey = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(100, 347);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(392, 16);
			this.lblStatus.TabIndex = 34;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblResult
			// 
			this.lblResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblResult.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.lblResult.Location = new System.Drawing.Point(312, 99);
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(272, 232);
			this.lblResult.TabIndex = 33;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(312, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 14);
			this.label2.TabIndex = 32;
			this.label2.Text = "��ȣȭ�� ����Ʈ�迭";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 83);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 14);
			this.label1.TabIndex = 31;
			this.label1.Text = "���� ���ڿ�";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(8, 99);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new System.Drawing.Size(272, 232);
			this.txtOriginal.TabIndex = 30;
			this.txtOriginal.Text = "";
			// 
			// cmdDecrypt
			// 
			this.cmdDecrypt.Location = new System.Drawing.Point(311, 379);
			this.cmdDecrypt.Name = "cmdDecrypt";
			this.cmdDecrypt.TabIndex = 25;
			this.cmdDecrypt.Text = "��ȣȭ";
			this.cmdDecrypt.Click += new System.EventHandler(this.cmdDecrypt_Click);
			// 
			// cmdEncrypt
			// 
			this.cmdEncrypt.Location = new System.Drawing.Point(207, 379);
			this.cmdEncrypt.Name = "cmdEncrypt";
			this.cmdEncrypt.TabIndex = 24;
			this.cmdEncrypt.Text = "��ȣȭ";
			this.cmdEncrypt.Click += new System.EventHandler(this.cmdEncrypt_Click);
			// 
			// txtIV
			// 
			this.txtIV.Location = new System.Drawing.Point(192, 35);
			this.txtIV.MaxLength = 24;
			this.txtIV.Name = "txtIV";
			this.txtIV.Size = new System.Drawing.Size(304, 21);
			this.txtIV.TabIndex = 29;
			this.txtIV.Tag = "";
			this.txtIV.Text = "";
			// 
			// txtKey
			// 
			this.txtKey.Location = new System.Drawing.Point(192, 3);
			this.txtKey.MaxLength = 24;
			this.txtKey.Name = "txtKey";
			this.txtKey.Size = new System.Drawing.Size(304, 21);
			this.txtKey.TabIndex = 28;
			this.txtKey.Tag = "";
			this.txtKey.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(104, 43);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 14);
			this.label4.TabIndex = 27;
			this.label4.Text = "�ʱ�ȭ ����";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(104, 11);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 14);
			this.label3.TabIndex = 26;
			this.label3.Text = "���Ű";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// Form3
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(592, 413);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lblStatus,
																		  this.lblResult,
																		  this.label2,
																		  this.label1,
																		  this.txtOriginal,
																		  this.cmdDecrypt,
																		  this.cmdEncrypt,
																		  this.txtIV,
																		  this.txtKey,
																		  this.label4,
																		  this.label3});
			this.Name = "Form3";
			this.Text = "TripleDES ��ȣȭ (��Ʈ��)";
			this.Load += new System.EventHandler(this.Form3_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdEncrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CTripleDES ctdes = new CTripleDES();
				
				byte[] byteKey;
				byteKey = ctdes.GetKeyByteArray(txtKey.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = ctdes.GetKeyByteArray(txtIV.Text);

				byte[] bTemp = Encoding.Unicode.GetBytes(txtOriginal.Text);
				MemoryStream msInput = new MemoryStream(bTemp, false);
				MemoryStream msOutput = new MemoryStream();
				msOutput.SetLength(0);

				ctdes.EncryptDecryptStream(msInput, out msOutput, byteKey, byteInitializationVector, "E");

				msOutput.Position = 0;
				byte[] byteBuffer2 = new byte[msOutput.Length];
				msOutput.Read(byteBuffer2, 0, (int) msOutput.Length);
				
				lblResult.Text = ctdes.GetHexFromByte(byteBuffer2);
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

				string[] sTemp = lblResult.Text.ToUpper().Split(':');
				byte[] bTemp = new byte[sTemp.Length];

				for (int i = 0; i < sTemp.Length; i++)
				{
					bTemp[i] = ctdes.GetByteFromHex(sTemp[i]);
				}
				MemoryStream msInput = new MemoryStream(bTemp, false);
				MemoryStream msOutput;

				ctdes.EncryptDecryptStream(msInput, out msOutput, byteKey, byteInitializationVector, "D");

				msOutput.Position = 0;
				byte[] byteBuffer2 = new byte[msOutput.Length];
				msOutput.Read(byteBuffer2, 0, (int) msOutput.Length);

				txtOriginal.Text = Encoding.Unicode.GetString(byteBuffer2);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void Form3_Load(object sender, System.EventArgs e)
		{
			txtKey.Text = "thermidor";
			txtIV.Text = "thermidor";
		}
	}
}
