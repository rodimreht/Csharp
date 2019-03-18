using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ex_Security
{
	/// <summary>
	/// Form2에 대한 요약 설명입니다.
	/// </summary>
	public class Form2 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOriginal;
		private System.Windows.Forms.Button cmdDecrypt;
		private System.Windows.Forms.Button cmdEncrypt;
		private System.Windows.Forms.TextBox txtIV;
		private System.Windows.Forms.TextBox txtKey;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox lblResult;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form2()
		{
			//
			// Windows Form 디자이너 지원에 필요합니다.
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
			//
		}

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
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

		#region Windows Form 디자이너에서 생성한 코드
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblStatus = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOriginal = new System.Windows.Forms.TextBox();
			this.cmdDecrypt = new System.Windows.Forms.Button();
			this.cmdEncrypt = new System.Windows.Forms.Button();
			this.txtIV = new System.Windows.Forms.TextBox();
			this.txtKey = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblResult = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(108, 351);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(392, 16);
			this.lblStatus.TabIndex = 34;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(320, 87);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 17);
			this.label2.TabIndex = 32;
			this.label2.Text = "암호화된 바이트배열";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 87);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 17);
			this.label1.TabIndex = 31;
			this.label1.Text = "원본 문자열";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("굴림체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(16, 103);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new System.Drawing.Size(272, 232);
			this.txtOriginal.TabIndex = 30;
			this.txtOriginal.Text = "";
			// 
			// cmdDecrypt
			// 
			this.cmdDecrypt.Location = new System.Drawing.Point(319, 383);
			this.cmdDecrypt.Name = "cmdDecrypt";
			this.cmdDecrypt.TabIndex = 25;
			this.cmdDecrypt.Text = "복호화";
			this.cmdDecrypt.Click += new System.EventHandler(this.cmdDecrypt_Click);
			// 
			// cmdEncrypt
			// 
			this.cmdEncrypt.Location = new System.Drawing.Point(215, 383);
			this.cmdEncrypt.Name = "cmdEncrypt";
			this.cmdEncrypt.TabIndex = 24;
			this.cmdEncrypt.Text = "암호화";
			this.cmdEncrypt.Click += new System.EventHandler(this.cmdEncrypt_Click);
			// 
			// txtIV
			// 
			this.txtIV.Location = new System.Drawing.Point(200, 39);
			this.txtIV.MaxLength = 24;
			this.txtIV.Name = "txtIV";
			this.txtIV.Size = new System.Drawing.Size(304, 21);
			this.txtIV.TabIndex = 29;
			this.txtIV.Tag = "";
			this.txtIV.Text = "";
			// 
			// txtKey
			// 
			this.txtKey.Location = new System.Drawing.Point(200, 7);
			this.txtKey.MaxLength = 24;
			this.txtKey.Name = "txtKey";
			this.txtKey.Size = new System.Drawing.Size(304, 21);
			this.txtKey.TabIndex = 28;
			this.txtKey.Tag = "";
			this.txtKey.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(112, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 14);
			this.label4.TabIndex = 27;
			this.label4.Text = "초기화 벡터";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(112, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 14);
			this.label3.TabIndex = 26;
			this.label3.Text = "비밀키";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblResult
			// 
			this.lblResult.Font = new System.Drawing.Font("굴림체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.lblResult.Location = new System.Drawing.Point(320, 104);
			this.lblResult.Multiline = true;
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(272, 232);
			this.lblResult.TabIndex = 35;
			this.lblResult.Text = "";
			// 
			// Form2
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
			this.Name = "Form2";
			this.Text = "DES 암호화 (문자열)";
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdEncrypt_Click(object sender, System.EventArgs e)
		{
			try
			{
				CDES cdes = new CDES();
				
				byte[] byteKey;
				byteKey = cdes.GetKeyByteArray(txtKey.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = cdes.GetKeyByteArray(txtIV.Text);

				string sTemp;

				cdes.EncryptDecryptString(txtOriginal.Text, out sTemp, byteKey, byteInitializationVector, "E");

				lblResult.Text = sTemp;
				lblStatus.Text = "DES(56bit) 암호화 알고리즘";
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
				CDES cdes = new CDES();

				byte[] byteKey;
				byteKey = cdes.GetKeyByteArray(txtKey.Text);

				byte[] byteInitializationVector;
				byteInitializationVector = cdes.GetKeyByteArray(txtIV.Text);

				string sTemp;

				cdes.EncryptDecryptString(lblResult.Text, out sTemp, byteKey, byteInitializationVector, "D");

				txtOriginal.Text = sTemp;
				lblStatus.Text = "DES 복호화되었습니다.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
