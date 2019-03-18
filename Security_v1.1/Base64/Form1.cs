using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Base64
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
		private TextBox txtResult;
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
			this.lblStatus = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.txtOriginal = new TextBox();
			this.cmdDecode = new Button();
			this.cmdEncode = new Button();
			this.txtResult = new TextBox();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new Point(108, 280);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new Size(392, 16);
			this.lblStatus.TabIndex = 34;
			this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new Point(320, 16);
			this.label2.Name = "label2";
			this.label2.Size = new Size(120, 17);
			this.label2.TabIndex = 32;
			this.label2.Text = "인코딩된 바이트배열";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(70, 17);
			this.label1.TabIndex = 31;
			this.label1.Text = "원본 문자열";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new Font("굴림체", 8F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(129)));
			this.txtOriginal.Location = new Point(16, 32);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new Size(272, 232);
			this.txtOriginal.TabIndex = 30;
			this.txtOriginal.Text = "";
			// 
			// cmdDecode
			// 
			this.cmdDecode.Location = new Point(319, 312);
			this.cmdDecode.Name = "cmdDecode";
			this.cmdDecode.TabIndex = 25;
			this.cmdDecode.Text = "디코딩";
			this.cmdDecode.Click += new EventHandler(this.cmdDecode_Click);
			// 
			// cmdEncode
			// 
			this.cmdEncode.Location = new Point(215, 312);
			this.cmdEncode.Name = "cmdEncode";
			this.cmdEncode.TabIndex = 36;
			this.cmdEncode.Text = "인코딩";
			this.cmdEncode.Click += new EventHandler(this.cmdEncode_Click);
			// 
			// txtResult
			// 
			this.txtResult.Font = new Font("굴림체", 8F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(129)));
			this.txtResult.Location = new Point(320, 32);
			this.txtResult.Multiline = true;
			this.txtResult.Name = "txtResult";
			this.txtResult.Size = new Size(272, 232);
			this.txtResult.TabIndex = 35;
			this.txtResult.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new Size(6, 14);
			this.ClientSize = new Size(608, 347);
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.cmdDecode);
			this.Controls.Add(this.cmdEncode);
			this.Name = "Form1";
			this.Text = "Base64 Encoding (문자열)";
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

		private void cmdEncode_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = Convert.ToBase64String(Encoding.Default.GetBytes(txtOriginal.Text));

				txtResult.Text = sTemp;
				lblStatus.Text = "Base64 Encoded.";
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
				string sTemp = Encoding.Default.GetString(Convert.FromBase64String(txtResult.Text));

				txtOriginal.Text = sTemp;
				lblStatus.Text = "Base64 Decoded.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
