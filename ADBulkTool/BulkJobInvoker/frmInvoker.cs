using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace BulkJobInvoker
{
	/// <summary>
	/// frmInvoker에 대한 요약 설명입니다.
	/// </summary>
	public class frmInvoker : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button cmdInvoke;
		private System.Windows.Forms.TextBox txtUsers;
		private System.Windows.Forms.TextBox txtProcesses;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button cmdOpen;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button cmdFileInvoke;
		private System.Windows.Forms.TextBox txtStartNumber;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton optInsert;
		private System.Windows.Forms.RadioButton optUpdate;
		private System.Windows.Forms.RadioButton optDelete;
		private System.Windows.Forms.RadioButton optSearch;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmInvoker()
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.cmdFileInvoke = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.cmdOpen = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtStartNumber = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cmdInvoke = new System.Windows.Forms.Button();
			this.txtUsers = new System.Windows.Forms.TextBox();
			this.txtProcesses = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.optInsert = new System.Windows.Forms.RadioButton();
			this.optUpdate = new System.Windows.Forms.RadioButton();
			this.optDelete = new System.Windows.Forms.RadioButton();
			this.optSearch = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtFile);
			this.groupBox1.Controls.Add(this.cmdFileInvoke);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.cmdOpen);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(480, 112);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File-based Bulk Job";
			// 
			// txtFile
			// 
			this.txtFile.BackColor = System.Drawing.Color.White;
			this.txtFile.Location = new System.Drawing.Point(131, 32);
			this.txtFile.Name = "txtFile";
			this.txtFile.ReadOnly = true;
			this.txtFile.Size = new System.Drawing.Size(280, 21);
			this.txtFile.TabIndex = 15;
			this.txtFile.Text = "";
			// 
			// cmdFileInvoke
			// 
			this.cmdFileInvoke.Location = new System.Drawing.Point(207, 72);
			this.cmdFileInvoke.Name = "cmdFileInvoke";
			this.cmdFileInvoke.TabIndex = 14;
			this.cmdFileInvoke.Text = "Invoke";
			this.cmdFileInvoke.Click += new System.EventHandler(this.cmdFileInvoke_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(24, 36);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(105, 17);
			this.label4.TabIndex = 12;
			this.label4.Text = "Start file to load :";
			// 
			// cmdOpen
			// 
			this.cmdOpen.Location = new System.Drawing.Point(410, 31);
			this.cmdOpen.Name = "cmdOpen";
			this.cmdOpen.Size = new System.Drawing.Size(56, 23);
			this.cmdOpen.TabIndex = 13;
			this.cmdOpen.Text = "File";
			this.cmdOpen.Click += new System.EventHandler(this.cmdOpen_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.optSearch);
			this.groupBox2.Controls.Add(this.optDelete);
			this.groupBox2.Controls.Add(this.optUpdate);
			this.groupBox2.Controls.Add(this.optInsert);
			this.groupBox2.Controls.Add(this.txtStartNumber);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.cmdInvoke);
			this.groupBox2.Controls.Add(this.txtUsers);
			this.groupBox2.Controls.Add(this.txtProcesses);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(8, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(480, 184);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Test Bulk Job";
			// 
			// txtStartNumber
			// 
			this.txtStartNumber.Location = new System.Drawing.Point(176, 56);
			this.txtStartNumber.Name = "txtStartNumber";
			this.txtStartNumber.TabIndex = 12;
			this.txtStartNumber.Text = "1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(19, 59);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(156, 17);
			this.label3.TabIndex = 11;
			this.label3.Text = "Starting number of users :";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmdInvoke
			// 
			this.cmdInvoke.Location = new System.Drawing.Point(200, 144);
			this.cmdInvoke.Name = "cmdInvoke";
			this.cmdInvoke.TabIndex = 10;
			this.cmdInvoke.Text = "Invoke";
			this.cmdInvoke.Click += new System.EventHandler(this.cmdInvoke_Click);
			// 
			// txtUsers
			// 
			this.txtUsers.Location = new System.Drawing.Point(176, 85);
			this.txtUsers.Name = "txtUsers";
			this.txtUsers.TabIndex = 9;
			this.txtUsers.Text = "100000";
			// 
			// txtProcesses
			// 
			this.txtProcesses.Location = new System.Drawing.Point(176, 27);
			this.txtProcesses.Name = "txtProcesses";
			this.txtProcesses.TabIndex = 8;
			this.txtProcesses.Text = "20";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(151, 17);
			this.label2.TabIndex = 7;
			this.label2.Text = "Users per each process :";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(101, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 17);
			this.label1.TabIndex = 6;
			this.label1.Text = "Processes :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// optInsert
			// 
			this.optInsert.Checked = true;
			this.optInsert.Location = new System.Drawing.Point(320, 24);
			this.optInsert.Name = "optInsert";
			this.optInsert.Size = new System.Drawing.Size(64, 24);
			this.optInsert.TabIndex = 13;
			this.optInsert.TabStop = true;
			this.optInsert.Text = "Insert";
			// 
			// optUpdate
			// 
			this.optUpdate.Location = new System.Drawing.Point(320, 48);
			this.optUpdate.Name = "optUpdate";
			this.optUpdate.Size = new System.Drawing.Size(64, 24);
			this.optUpdate.TabIndex = 14;
			this.optUpdate.Text = "Update";
			// 
			// optDelete
			// 
			this.optDelete.Location = new System.Drawing.Point(320, 72);
			this.optDelete.Name = "optDelete";
			this.optDelete.Size = new System.Drawing.Size(64, 24);
			this.optDelete.TabIndex = 15;
			this.optDelete.Text = "Delete";
			// 
			// optSearch
			// 
			this.optSearch.Location = new System.Drawing.Point(320, 96);
			this.optSearch.Name = "optSearch";
			this.optSearch.Size = new System.Drawing.Size(64, 24);
			this.optSearch.TabIndex = 16;
			this.optSearch.Text = "Search";
			// 
			// frmInvoker
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(496, 317);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmInvoker";
			this.Text = "Invoker";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmInvoker());
		}

		private void cmdInvoke_Click(object sender, System.EventArgs e)
		{
			if (cmdInvoke.Text == "Invoke")
			{
				cmdInvoke.Text = "Stop";
				cmdFileInvoke.Enabled = false;

				string sOption;
				if (optInsert.Checked)
					sOption = "C";
				else if (optUpdate.Checked)
					sOption = "U";
				else if (optDelete.Checked)
					sOption = "D";
				else
					sOption = "S";

				for (int i = 0; i < int.Parse(txtProcesses.Text); i++)
				{
					int nUsers = int.Parse(txtUsers.Text);
					String sUsers = sOption + "|" + 
						((int) ((nUsers * i) + int.Parse(txtStartNumber.Text))).ToString() + "|" +
						((int) (nUsers * (i + 1) + int.Parse(txtStartNumber.Text) - 1)).ToString();
					System.Diagnostics.Process.Start("ADBulkTool.exe", sUsers);
				}
				cmdInvoke.Text = "Invoke";
				cmdFileInvoke.Enabled = true;
			}
		}

		private void cmdOpen_Click(object sender, System.EventArgs e)
		{
			DialogResult dr = openFileDialog1.ShowDialog();
			if (dr == DialogResult.OK)
			{
				txtFile.Text = openFileDialog1.FileName;
			}
		}

		private void cmdFileInvoke_Click(object sender, System.EventArgs e)
		{
			if (txtFile.Text.IndexOf("_L") <= 0)
			{
				MessageBox.Show("Select the first splitted file for bulk job!\r\n"
					+ "(e.g. 'C:\\Temp\\abc_L01.txt')");
				return;
			}

			if (cmdFileInvoke.Text == "Invoke")
			{
				cmdInvoke.Enabled = false;
				cmdFileInvoke.Text = "Stop";

				String sFilename = txtFile.Text.Substring(0, txtFile.Text.IndexOf("_L") + 2);
				int nIndex = int.Parse(txtFile.Text.Substring(txtFile.Text.IndexOf("_L") + 2, 2));
				String sFileExt = txtFile.Text.Substring(txtFile.Text.IndexOf("."));

				while (true)
				{
					String sFile = sFilename + nIndex.ToString("00") + sFileExt;
					if (File.Exists(sFile))
					{
						System.Diagnostics.Process.Start("C:\\Temp\\ADBulkTool.exe", "FILES|" + sFile);
					}
					else
					{
						break;
					}
					nIndex++;
				}
				cmdInvoke.Enabled = true;
				cmdFileInvoke.Text = "Invoke";
			}
		}
	}
}
