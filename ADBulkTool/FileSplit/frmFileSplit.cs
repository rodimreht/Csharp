using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace FileSplit
{
	/// <summary>
	/// frmFileSplit에 대한 요약 설명입니다.
	/// </summary>
	public class frmFileSplit : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFilename;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtLines;
		private System.Windows.Forms.TextBox txtProcessed;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button cmdProcess;
		private System.Windows.Forms.Button cmdOpenFile;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private static bool bProcessing = false;

		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmFileSplit()
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

		#region Windows Form Designer generated code
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.txtFilename = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtLines = new System.Windows.Forms.TextBox();
			this.txtProcessed = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.cmdProcess = new System.Windows.Forms.Button();
			this.cmdOpenFile = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Filename :";
			// 
			// txtFilename
			// 
			this.txtFilename.BackColor = System.Drawing.Color.White;
			this.txtFilename.Location = new System.Drawing.Point(74, 12);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.ReadOnly = true;
			this.txtFilename.Size = new System.Drawing.Size(342, 21);
			this.txtFilename.TabIndex = 1;
			this.txtFilename.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(177, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Number of lines for each file :";
			// 
			// txtLines
			// 
			this.txtLines.BackColor = System.Drawing.Color.White;
			this.txtLines.Location = new System.Drawing.Point(184, 44);
			this.txtLines.Name = "txtLines";
			this.txtLines.Size = new System.Drawing.Size(64, 21);
			this.txtLines.TabIndex = 3;
			this.txtLines.Text = "10000";
			// 
			// txtProcessed
			// 
			this.txtProcessed.BackColor = System.Drawing.Color.White;
			this.txtProcessed.Location = new System.Drawing.Point(416, 44);
			this.txtProcessed.Name = "txtProcessed";
			this.txtProcessed.ReadOnly = true;
			this.txtProcessed.Size = new System.Drawing.Size(64, 21);
			this.txtProcessed.TabIndex = 5;
			this.txtProcessed.Text = "";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(264, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(150, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Current lines processed :";
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(16, 72);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(464, 56);
			this.lblStatus.TabIndex = 6;
			this.lblStatus.Text = "Waiting...";
			// 
			// cmdProcess
			// 
			this.cmdProcess.Location = new System.Drawing.Point(211, 144);
			this.cmdProcess.Name = "cmdProcess";
			this.cmdProcess.TabIndex = 7;
			this.cmdProcess.Text = "Start";
			this.cmdProcess.Click += new System.EventHandler(this.cmdProcess_Click);
			// 
			// cmdOpenFile
			// 
			this.cmdOpenFile.Location = new System.Drawing.Point(416, 11);
			this.cmdOpenFile.Name = "cmdOpenFile";
			this.cmdOpenFile.Size = new System.Drawing.Size(64, 23);
			this.cmdOpenFile.TabIndex = 8;
			this.cmdOpenFile.Text = "File";
			this.cmdOpenFile.Click += new System.EventHandler(this.cmdOpenFile_Click);
			// 
			// frmFileSplit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(496, 189);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cmdOpenFile,
																		  this.cmdProcess,
																		  this.lblStatus,
																		  this.txtProcessed,
																		  this.label3,
																		  this.label2,
																		  this.label1,
																		  this.txtLines,
																		  this.txtFilename});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmFileSplit";
			this.Text = "FileSplit";
			this.Load += new System.EventHandler(this.frmFileSplit_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmFileSplit());
		}

		private void cmdProcess_Click(object sender, System.EventArgs e)
		{
			if (cmdProcess.Text == "Stop")
			{
				bProcessing = false;
				return;
			}
			else if (cmdProcess.Text == "End")
			{
				Application.Exit();
				return;
			}

			if (txtFilename.Text.Trim().Length == 0)
			{
				lblStatus.Text = "Select file to split!";
				return;
			}

			if (!System.IO.File.Exists(txtFilename.Text))
			{
				lblStatus.Text = "File does not exist!";
				return;
			}

			txtLines.ReadOnly = true;
			cmdProcess.Text = "Stop";
			txtProcessed.Text = "0";
			bProcessing = true;
			int nCnt = 1, nLines = 0;

			FileInfo fi = new FileInfo(txtFilename.Text);
			FileStream fsr = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(fsr, System.Text.Encoding.Default);

			String sFilename = txtFilename.Text.Substring(0, txtFilename.Text.IndexOf(".")) + "_L";
			String sFileExt = txtFilename.Text.Substring(txtFilename.Text.IndexOf("."));
			FileInfo fiw = new FileInfo(sFilename + nCnt.ToString("00") + sFileExt);
			if (fiw.Exists)
			{
				lblStatus.Text = "File: " + fiw.Name + " already exists!";
				txtLines.ReadOnly = false;
				cmdProcess.Text = "Start";
				txtProcessed.Text = "";
				bProcessing = false;
				return;
			}
			FileStream fsw = fiw.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
			StreamWriter sw = new StreamWriter(fsw, System.Text.Encoding.Default);
			lblStatus.Text = "Writing to File:[" + fiw.Name + "]...\r\n";
			lblStatus.Refresh();

			String sLine = sr.ReadLine();
			nLines = int.Parse(txtProcessed.Text) + 1;
			while (bProcessing && (sLine != null))
			{
				sw.WriteLine(sLine);

				if (nLines >= int.Parse(txtLines.Text))
				{
					nCnt++;
					txtProcessed.Text = "0";
					nLines = 0;

					sw.Close();
					fsw.Close();
					fiw = new FileInfo(sFilename + nCnt.ToString("00") + sFileExt);
					if (fiw.Exists)
					{
						lblStatus.Text = "File: " + fiw.Name + " already exists!";
						txtLines.ReadOnly = false;
						cmdProcess.Text = "Start";
						txtProcessed.Text = "";
						bProcessing = false;
						return;
					}
					fsw = fiw.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
					sw = new StreamWriter(fsw, System.Text.Encoding.Default);
					lblStatus.Text = "Writing to File:[" + fiw.Name + "]...\r\n";
					lblStatus.Refresh();
				}

				if ((nLines > 0) && (nLines % 100 == 0))
				{
					txtProcessed.Text = nLines.ToString();
					Application.DoEvents();
				}
				sLine = sr.ReadLine();
				nLines++;
			}
			sw.Close();
			fsw.Close();

			if ((sLine == null) && (nLines == 1))
			{
				lblStatus.Text += "Deleting file:[" + fiw.Name + "]...\r\n";
				lblStatus.Refresh();
				fiw.Delete();
			}
			
			sr.Close();
			fsr.Close();

			txtLines.ReadOnly = false;
			cmdProcess.Text = "End";

			lblStatus.Text += "Completed!";
		}

		private void cmdOpenFile_Click(object sender, System.EventArgs e)
		{
			DialogResult dr = openFileDialog1.ShowDialog();
			if (dr == DialogResult.OK)
			{
				txtFilename.Text = openFileDialog1.FileName;
			}
		}

		private void frmFileSplit_Load(object sender, System.EventArgs e)
		{
			cmdOpenFile.Focus();
		}
	}
}
