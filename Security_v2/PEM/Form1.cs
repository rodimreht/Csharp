using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace PEM
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
		private Button cmdDecode;
		private Button cmdEncode;
		private TextBox txtKey;
		private Label label3;
		private TextBox lblResult;
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
			this.lblStatus = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.txtOriginal = new TextBox();
			this.cmdDecode = new Button();
			this.cmdEncode = new Button();
			this.txtKey = new TextBox();
			this.label3 = new Label();
			this.lblResult = new TextBox();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new Point(108, 312);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new Size(392, 16);
			this.lblStatus.TabIndex = 34;
			this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new Point(320, 48);
			this.label2.Name = "label2";
			this.label2.Size = new Size(120, 17);
			this.label2.TabIndex = 32;
			this.label2.Text = "인코딩된 바이트배열";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(16, 48);
			this.label1.Name = "label1";
			this.label1.Size = new Size(70, 17);
			this.label1.TabIndex = 31;
			this.label1.Text = "원본 문자열";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new Font("굴림체", 8F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(129)));
			this.txtOriginal.Location = new Point(16, 64);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new Size(272, 232);
			this.txtOriginal.TabIndex = 30;
			this.txtOriginal.Text = "";
			// 
			// cmdDecode
			// 
			this.cmdDecode.Location = new Point(319, 344);
			this.cmdDecode.Name = "cmdDecode";
			this.cmdDecode.TabIndex = 25;
			this.cmdDecode.Text = "디코딩";
			this.cmdDecode.Click += new EventHandler(this.cmdDecode_Click);
			// 
			// cmdEncode
			// 
			this.cmdEncode.Location = new Point(215, 344);
			this.cmdEncode.Name = "cmdEncode";
			this.cmdEncode.TabIndex = 24;
			this.cmdEncode.Text = "인코딩";
			this.cmdEncode.Click += new EventHandler(this.cmdEncode_Click);
			// 
			// txtKey
			// 
			this.txtKey.Location = new Point(200, 7);
			this.txtKey.MaxLength = 24;
			this.txtKey.Name = "txtKey";
			this.txtKey.Size = new Size(304, 21);
			this.txtKey.TabIndex = 28;
			this.txtKey.Tag = "";
			this.txtKey.Text = "CERTIFICATE";
			// 
			// label3
			// 
			this.label3.Location = new Point(112, 15);
			this.label3.Name = "label3";
			this.label3.Size = new Size(70, 14);
			this.label3.TabIndex = 26;
			this.label3.Text = "형식 문자열";
			this.label3.TextAlign = ContentAlignment.TopRight;
			// 
			// lblResult
			// 
			this.lblResult.Font = new Font("굴림체", 8F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(129)));
			this.lblResult.Location = new Point(320, 64);
			this.lblResult.Multiline = true;
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new Size(272, 232);
			this.lblResult.TabIndex = 35;
			this.lblResult.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new Size(6, 14);
			this.ClientSize = new Size(608, 379);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.txtKey);
			this.Controls.Add(this.cmdDecode);
			this.Controls.Add(this.cmdEncode);
			this.Controls.Add(this.label3);
			this.Name = "Form1";
			this.Text = "PEM Encoding (문자열)";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void cmdEncode_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = CPEM.ToPEM(txtKey.Text, txtOriginal.Text);

				lblResult.Text = sTemp;
				lblStatus.Text = "PEM Encoded.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdDecode_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = "";
				byte[] data = Encoding.ASCII.GetBytes(lblResult.Text);
				byte[] result = CPEM.FromPEM(txtKey.Text, data);
				bool isBinary = false;
				for (int i = 0; i < result.Length; i++)
					if (result[i] < 0x0A)
					{
						isBinary = true;
						break;
					}

				if (isBinary)
					sTemp = CryptUtil.GetHexFromByte(result);
				else
					sTemp = Encoding.ASCII.GetString(result);

				txtOriginal.Text = sTemp;
				lblStatus.Text = "PEM Decoded.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
