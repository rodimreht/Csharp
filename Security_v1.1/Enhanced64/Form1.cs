using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Nets.IM.Common;

namespace Enhanced64
{
	/// <summary>
	/// Form1에 대한 요약 설명입니다.
	/// </summary>
	public class Form1 : Form
	{
		private Label lblStatus;
		private Label label2;
		private Label label1;
		private TextBox txtOriginal;
		private TextBox txtResult;
		private System.Windows.Forms.TextBox txtKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button cmdDecrypt;
		private System.Windows.Forms.Button cmdEncrypt;

		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private Container components = null;

		public Form1()
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
				if (components != null) 
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
			this.txtResult = new System.Windows.Forms.TextBox();
			this.txtKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(108, 312);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(392, 16);
			this.lblStatus.TabIndex = 34;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(320, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 17);
			this.label2.TabIndex = 32;
			this.label2.Text = "암호화된 바이트배열";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 17);
			this.label1.TabIndex = 31;
			this.label1.Text = "원본 문자열";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("굴림체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(16, 64);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new System.Drawing.Size(272, 232);
			this.txtOriginal.TabIndex = 30;
			this.txtOriginal.Text = "";
			// 
			// cmdDecrypt
			// 
			this.cmdDecrypt.Location = new System.Drawing.Point(319, 344);
			this.cmdDecrypt.Name = "cmdDecrypt";
			this.cmdDecrypt.TabIndex = 25;
			this.cmdDecrypt.Text = "복호화";
			this.cmdDecrypt.Click += new System.EventHandler(this.cmdDecrypt2_Click);
			// 
			// cmdEncrypt
			// 
			this.cmdEncrypt.Location = new System.Drawing.Point(215, 344);
			this.cmdEncrypt.Name = "cmdEncrypt";
			this.cmdEncrypt.TabIndex = 36;
			this.cmdEncrypt.Text = "암호화";
			this.cmdEncrypt.Click += new System.EventHandler(this.cmdEncrypt2_Click);
			// 
			// txtResult
			// 
			this.txtResult.Font = new System.Drawing.Font("굴림체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.txtResult.Location = new System.Drawing.Point(320, 64);
			this.txtResult.Multiline = true;
			this.txtResult.Name = "txtResult";
			this.txtResult.Size = new System.Drawing.Size(272, 232);
			this.txtResult.TabIndex = 35;
			this.txtResult.Text = "";
			// 
			// txtKey
			// 
			this.txtKey.Location = new System.Drawing.Point(196, 16);
			this.txtKey.MaxLength = 24;
			this.txtKey.Name = "txtKey";
			this.txtKey.Size = new System.Drawing.Size(304, 21);
			this.txtKey.TabIndex = 38;
			this.txtKey.Tag = "";
			this.txtKey.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(108, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 14);
			this.label3.TabIndex = 37;
			this.label3.Text = "비밀키";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(608, 379);
			this.Controls.Add(this.txtKey);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.cmdDecrypt);
			this.Controls.Add(this.cmdEncrypt);
			this.Name = "Form1";
			this.Text = "Enhanced64 암호화 (문자열)";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void cmdEncrypt_Click(object sender, EventArgs e)
		{
			try
			{
				CEnhanced64 en64 = new CEnhanced64();
				string sTemp = en64.Encrypt(txtKey.Text, txtOriginal.Text);

				txtResult.Text = sTemp;
				lblStatus.Text = "Enhanced64 Encrypted.(" + sTemp.Length + " bytes)";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdDecrypt_Click(object sender, EventArgs e)
		{
			try
			{
				CEnhanced64 en64 = new CEnhanced64();
				string sTemp = en64.Decrypt(txtKey.Text, txtResult.Text);

				txtOriginal.Text = sTemp;
				lblStatus.Text = "Enhanced64 Decrypted.(" + Encoding.Default.GetBytes(sTemp).Length + " bytes)";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdEncrypt2_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = Security.GetInstance("enhanced").Encrypt(txtKey.Text, txtOriginal.Text);

				txtResult.Text = sTemp;
				lblStatus.Text = "Enhanced64 Encrypted.(" + sTemp.Length + " bytes)";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdDecrypt2_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = Security.GetInstance("enhanced").Decrypt(txtKey.Text, txtResult.Text);

				txtOriginal.Text = sTemp;
				lblStatus.Text = "Enhanced64 Decrypted.(" + Encoding.Default.GetBytes(sTemp).Length + " bytes)";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
