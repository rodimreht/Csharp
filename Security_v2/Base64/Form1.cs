using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using CRC32App;

namespace Base64
{
	/// <summary>
	/// Form1�� ���� ��� �����Դϴ�.
	/// </summary>
	public class Form1 : Form
	{
		private Button cmdBinEncode;
		private Button cmdDecode;
		private Button cmdEncode;

		/// <summary>
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private Container components;

		private Label label1;
		private Label label2;
		private Label lblStatus;
		private TextBox txtOriginal;
		private Button cmdCRC;
		private Button cmdBinaryCRC;
		private OpenFileDialog openFileDialog1;
		private Button cmdFile;
		private TextBox txtResult;

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
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		private static void Main()
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
				byte[] bytes = Convert.FromBase64String(txtResult.Text);
				string sTemp = Encoding.Default.GetString(bytes);

				bool bExist = false;
				foreach (char c in sTemp)
				{
					if (c < 32 && c != 13 && c != 10)
					{
						bExist = true;
						break;
					}
				}
				if (bExist) sTemp = CryptUtil.GetHexFromByte(bytes);

				txtOriginal.Text = sTemp;
				lblStatus.Text = "Base64 Decoded." + (bExist ? "(binary)" : "");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdBinEncode_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = Convert.ToBase64String(CryptUtil.GetHexArray(txtOriginal.Text));

				txtResult.Text = sTemp;
				lblStatus.Text = "Base64 Encoded.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		#region Windows Form �����̳ʿ��� ������ �ڵ�

		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblStatus = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOriginal = new System.Windows.Forms.TextBox();
			this.cmdDecode = new System.Windows.Forms.Button();
			this.cmdEncode = new System.Windows.Forms.Button();
			this.txtResult = new System.Windows.Forms.TextBox();
			this.cmdBinEncode = new System.Windows.Forms.Button();
			this.cmdCRC = new System.Windows.Forms.Button();
			this.cmdBinaryCRC = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.cmdFile = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(108, 280);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(392, 16);
			this.lblStatus.TabIndex = 34;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(320, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 12);
			this.label2.TabIndex = 32;
			this.label2.Text = "���ڵ��� ���ڿ�";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 12);
			this.label1.TabIndex = 31;
			this.label1.Text = "���� ���ڿ�";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(16, 32);
			this.txtOriginal.MaxLength = 262144;
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOriginal.Size = new System.Drawing.Size(272, 232);
			this.txtOriginal.TabIndex = 30;
			// 
			// cmdDecode
			// 
			this.cmdDecode.Location = new System.Drawing.Point(425, 312);
			this.cmdDecode.Name = "cmdDecode";
			this.cmdDecode.Size = new System.Drawing.Size(75, 23);
			this.cmdDecode.TabIndex = 25;
			this.cmdDecode.Text = "���ڵ�";
			this.cmdDecode.Click += new System.EventHandler(this.cmdDecode_Click);
			// 
			// cmdEncode
			// 
			this.cmdEncode.Location = new System.Drawing.Point(179, 312);
			this.cmdEncode.Name = "cmdEncode";
			this.cmdEncode.Size = new System.Drawing.Size(75, 23);
			this.cmdEncode.TabIndex = 36;
			this.cmdEncode.Text = "���ڵ�";
			this.cmdEncode.Click += new System.EventHandler(this.cmdEncode_Click);
			// 
			// txtResult
			// 
			this.txtResult.Font = new System.Drawing.Font("����ü", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.txtResult.Location = new System.Drawing.Point(320, 32);
			this.txtResult.MaxLength = 262144;
			this.txtResult.Multiline = true;
			this.txtResult.Name = "txtResult";
			this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtResult.Size = new System.Drawing.Size(272, 232);
			this.txtResult.TabIndex = 35;
			// 
			// cmdBinEncode
			// 
			this.cmdBinEncode.Location = new System.Drawing.Point(59, 312);
			this.cmdBinEncode.Name = "cmdBinEncode";
			this.cmdBinEncode.Size = new System.Drawing.Size(90, 23);
			this.cmdBinEncode.TabIndex = 37;
			this.cmdBinEncode.Text = "binary���ڵ�";
			this.cmdBinEncode.Click += new System.EventHandler(this.cmdBinEncode_Click);
			// 
			// cmdCRC
			// 
			this.cmdCRC.Location = new System.Drawing.Point(179, 341);
			this.cmdCRC.Name = "cmdCRC";
			this.cmdCRC.Size = new System.Drawing.Size(75, 23);
			this.cmdCRC.TabIndex = 36;
			this.cmdCRC.Text = "CRC";
			this.cmdCRC.Click += new System.EventHandler(this.cmdCRC_Click);
			// 
			// cmdBinaryCRC
			// 
			this.cmdBinaryCRC.Location = new System.Drawing.Point(59, 341);
			this.cmdBinaryCRC.Name = "cmdBinaryCRC";
			this.cmdBinaryCRC.Size = new System.Drawing.Size(90, 23);
			this.cmdBinaryCRC.TabIndex = 37;
			this.cmdBinaryCRC.Text = "binaryCRC";
			this.cmdBinaryCRC.Click += new System.EventHandler(this.cmdBinaryCRC_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// cmdFile
			// 
			this.cmdFile.Location = new System.Drawing.Point(16, 262);
			this.cmdFile.Name = "cmdFile";
			this.cmdFile.Size = new System.Drawing.Size(75, 23);
			this.cmdFile.TabIndex = 38;
			this.cmdFile.Text = "���� ����";
			this.cmdFile.Click += new System.EventHandler(this.cmdFile_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(608, 379);
			this.Controls.Add(this.cmdFile);
			this.Controls.Add(this.cmdBinaryCRC);
			this.Controls.Add(this.cmdBinEncode);
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.cmdDecode);
			this.Controls.Add(this.cmdCRC);
			this.Controls.Add(this.cmdEncode);
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Base64 Encoding (���ڿ�)";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private void cmdBinaryCRC_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = CRC32.CreateCRC32(CryptUtil.GetHexArray(txtOriginal.Text));
				txtResult.Text = sTemp;
				lblStatus.Text = "CRC32 Encoded.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdCRC_Click(object sender, EventArgs e)
		{
			try
			{
				string sTemp = CRC32.CreateCRC32(txtOriginal.Text);
				txtResult.Text = sTemp;
				lblStatus.Text = "CRC32 Encoded.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdFile_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string filePath = openFileDialog1.FileName;
				FileInfo fi = new FileInfo(filePath);
				using (FileStream fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					byte[] buffer = new byte[fi.Length];
					fs.Read(buffer, 0, buffer.Length);

					try
					{
						string sTemp = Convert.ToBase64String(buffer);

						txtResult.Text = sTemp;
						lblStatus.Text = "Base64 Encoded.";
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}

					fs.Close();
				}
			}
		}
	}
}
