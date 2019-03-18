using System.Windows.Forms;

namespace BigTextViewer
{
	partial class Form1
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.txtFilePath = new System.Windows.Forms.TextBox();
			this.cboEncoding = new System.Windows.Forms.ComboBox();
			this.cmdOpen = new System.Windows.Forms.Button();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.txtPreview = new System.Windows.Forms.TextBox();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.chkReverse = new System.Windows.Forms.CheckBox();
			this.txtOrgString = new System.Windows.Forms.TextBox();
			this.cmdSearch = new System.Windows.Forms.Button();
			this.cmdStop = new System.Windows.Forms.Button();
			this.cmdNext = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtStatus = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
			this.menuItem1.Text = "메뉴";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "종료(&X)";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "모든 파일";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel1MinSize = 20;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer1.Size = new System.Drawing.Size(584, 382);
			this.splitContainer1.SplitterDistance = 25;
			this.splitContainer1.SplitterWidth = 1;
			this.splitContainer1.TabIndex = 5;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.txtFilePath);
			this.splitContainer2.Panel1.Controls.Add(this.cboEncoding);
			this.splitContainer2.Panel1.Padding = new System.Windows.Forms.Padding(3, 2, 0, 0);
			this.splitContainer2.Panel1MinSize = 21;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.cmdOpen);
			this.splitContainer2.Panel2MinSize = 21;
			this.splitContainer2.Size = new System.Drawing.Size(584, 25);
			this.splitContainer2.SplitterDistance = 529;
			this.splitContainer2.TabIndex = 7;
			// 
			// txtFilePath
			// 
			this.txtFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtFilePath.Location = new System.Drawing.Point(3, 2);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.ReadOnly = true;
			this.txtFilePath.Size = new System.Drawing.Size(437, 21);
			this.txtFilePath.TabIndex = 6;
			// 
			// cboEncoding
			// 
			this.cboEncoding.Dock = System.Windows.Forms.DockStyle.Right;
			this.cboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboEncoding.FormattingEnabled = true;
			this.cboEncoding.Items.AddRange(new object[] {
            "Default",
            "UTF-8"});
			this.cboEncoding.Location = new System.Drawing.Point(440, 2);
			this.cboEncoding.Name = "cboEncoding";
			this.cboEncoding.Size = new System.Drawing.Size(89, 20);
			this.cboEncoding.TabIndex = 5;
			// 
			// cmdOpen
			// 
			this.cmdOpen.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cmdOpen.Location = new System.Drawing.Point(0, 0);
			this.cmdOpen.Name = "cmdOpen";
			this.cmdOpen.Size = new System.Drawing.Size(51, 25);
			this.cmdOpen.TabIndex = 3;
			this.cmdOpen.Text = "열기";
			this.cmdOpen.Click += new System.EventHandler(this.cmdOpen_Click);
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer3.IsSplitterFixed = true;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.txtPreview);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
			this.splitContainer3.Size = new System.Drawing.Size(584, 356);
			this.splitContainer3.SplitterDistance = 276;
			this.splitContainer3.SplitterWidth = 1;
			this.splitContainer3.TabIndex = 8;
			// 
			// txtPreview
			// 
			this.txtPreview.BackColor = System.Drawing.Color.Black;
			this.txtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtPreview.ForeColor = System.Drawing.Color.LightGray;
			this.txtPreview.HideSelection = false;
			this.txtPreview.Location = new System.Drawing.Point(0, 0);
			this.txtPreview.Multiline = true;
			this.txtPreview.Name = "txtPreview";
			this.txtPreview.ReadOnly = true;
			this.txtPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtPreview.Size = new System.Drawing.Size(584, 276);
			this.txtPreview.TabIndex = 2;
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer4.IsSplitterFixed = true;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.chkReverse);
			this.splitContainer4.Panel1.Controls.Add(this.txtOrgString);
			this.splitContainer4.Panel1.Controls.Add(this.cmdSearch);
			this.splitContainer4.Panel1.Controls.Add(this.cmdStop);
			this.splitContainer4.Panel1.Controls.Add(this.cmdNext);
			this.splitContainer4.Panel1.Controls.Add(this.label1);
			this.splitContainer4.Panel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.splitContainer4.Panel1MinSize = 40;
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.txtStatus);
			this.splitContainer4.Size = new System.Drawing.Size(584, 79);
			this.splitContainer4.SplitterDistance = 40;
			this.splitContainer4.SplitterWidth = 1;
			this.splitContainer4.TabIndex = 8;
			// 
			// chkReverse
			// 
			this.chkReverse.AutoSize = true;
			this.chkReverse.Enabled = false;
			this.chkReverse.Location = new System.Drawing.Point(84, 23);
			this.chkReverse.Name = "chkReverse";
			this.chkReverse.Size = new System.Drawing.Size(100, 16);
			this.chkReverse.TabIndex = 16;
			this.chkReverse.Tag = "";
			this.chkReverse.Text = "역순으로 검색";
			this.chkReverse.UseVisualStyleBackColor = true;
			this.chkReverse.CheckedChanged += new System.EventHandler(this.chkReverse_CheckedChanged);
			// 
			// txtOrgString
			// 
			this.txtOrgString.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOrgString.Location = new System.Drawing.Point(84, 2);
			this.txtOrgString.Margin = new System.Windows.Forms.Padding(60, 3, 60, 3);
			this.txtOrgString.Name = "txtOrgString";
			this.txtOrgString.Size = new System.Drawing.Size(321, 21);
			this.txtOrgString.TabIndex = 15;
			this.txtOrgString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOrgString_KeyDown);
			// 
			// cmdSearch
			// 
			this.cmdSearch.BackColor = System.Drawing.Color.YellowGreen;
			this.cmdSearch.Dock = System.Windows.Forms.DockStyle.Right;
			this.cmdSearch.Location = new System.Drawing.Point(405, 2);
			this.cmdSearch.Name = "cmdSearch";
			this.cmdSearch.Size = new System.Drawing.Size(53, 38);
			this.cmdSearch.TabIndex = 12;
			this.cmdSearch.Text = "찾기";
			this.cmdSearch.UseVisualStyleBackColor = false;
			this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
			// 
			// cmdStop
			// 
			this.cmdStop.BackColor = System.Drawing.Color.Red;
			this.cmdStop.Dock = System.Windows.Forms.DockStyle.Right;
			this.cmdStop.Enabled = false;
			this.cmdStop.Location = new System.Drawing.Point(458, 2);
			this.cmdStop.Name = "cmdStop";
			this.cmdStop.Size = new System.Drawing.Size(53, 38);
			this.cmdStop.TabIndex = 11;
			this.cmdStop.Text = "중지";
			this.cmdStop.UseVisualStyleBackColor = false;
			this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
			// 
			// cmdNext
			// 
			this.cmdNext.Dock = System.Windows.Forms.DockStyle.Right;
			this.cmdNext.Location = new System.Drawing.Point(511, 2);
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Size = new System.Drawing.Size(73, 38);
			this.cmdNext.TabIndex = 8;
			this.cmdNext.Text = "다음읽기";
			this.cmdNext.Visible = false;
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// label1
			// 
			this.label1.Cursor = System.Windows.Forms.Cursors.Default;
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point(0, 2);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 19);
			this.label1.Size = new System.Drawing.Size(84, 38);
			this.label1.TabIndex = 3;
			this.label1.Text = "찾을 문자열:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtStatus
			// 
			this.txtStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
			this.txtStatus.Cursor = System.Windows.Forms.Cursors.Default;
			this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtStatus.Location = new System.Drawing.Point(0, 0);
			this.txtStatus.Name = "txtStatus";
			this.txtStatus.Size = new System.Drawing.Size(584, 38);
			this.txtStatus.TabIndex = 5;
			this.txtStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 382);
			this.Controls.Add(this.splitContainer1);
			this.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Menu = this.mainMenu1;
			this.MinimumSize = new System.Drawing.Size(600, 420);
			this.Name = "Form1";
			this.Text = "BigTextViewer";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel1.PerformLayout();
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenFileDialog openFileDialog1;
		private MainMenu mainMenu1;
		private MenuItem menuItem1;
		private MenuItem menuItem2;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private Button cmdOpen;
		private SplitContainer splitContainer3;
		private TextBox txtPreview;
		private SplitContainer splitContainer4;
		private Label label1;
		private Label txtStatus;
		private TextBox txtFilePath;
		private ComboBox cboEncoding;
		private Button cmdNext;
		private CheckBox chkReverse;
		private TextBox txtOrgString;
		private Button cmdSearch;
		private Button cmdStop;
	}
}

