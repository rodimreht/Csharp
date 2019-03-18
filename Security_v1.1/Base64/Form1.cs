using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Base64
{
	/// <summary>
	/// Form1�� ���� ��� �����Դϴ�.
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
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private Container components = null;

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

		#region Windows Form �����̳ʿ��� ������ �ڵ�
		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
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
			this.label2.Text = "���ڵ��� ����Ʈ�迭";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(70, 17);
			this.label1.TabIndex = 31;
			this.label1.Text = "���� ���ڿ�";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new Font("����ü", 8F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(129)));
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
			this.cmdDecode.Text = "���ڵ�";
			this.cmdDecode.Click += new EventHandler(this.cmdDecode_Click);
			// 
			// cmdEncode
			// 
			this.cmdEncode.Location = new Point(215, 312);
			this.cmdEncode.Name = "cmdEncode";
			this.cmdEncode.TabIndex = 36;
			this.cmdEncode.Text = "���ڵ�";
			this.cmdEncode.Click += new EventHandler(this.cmdEncode_Click);
			// 
			// txtResult
			// 
			this.txtResult.Font = new Font("����ü", 8F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(129)));
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
			this.Text = "Base64 Encoding (���ڿ�)";
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
