namespace NETS_iMan
{
	partial class frmChatHistory
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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("날짜: 오늘 ");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("날짜: 어제 ");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("날짜: 목요일 ");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("날짜: 수요일 ");
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("날짜: 화요일 ");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("날짜: 월요일 ");
			System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("날짜: 일요일 ");
			System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("날짜: 지난 주 ");
			System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("날짜: 2주 전 ");
			System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("날짜: 3주 전 ");
			System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("날짜: 4주 전 ");
			System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("날짜: 오래된 항목  ");
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.cboUsers = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtContents = new System.Windows.Forms.TextBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(755, 425);
			this.splitContainer1.SplitterDistance = 150;
			this.splitContainer1.SplitterWidth = 1;
			this.splitContainer1.TabIndex = 0;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.FullRowSelect = true;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.MinimumSize = new System.Drawing.Size(150, 280);
			this.treeView1.Name = "treeView1";
			treeNode1.Name = "TODAY";
			treeNode1.Text = "날짜: 오늘 ";
			treeNode2.Name = "YESTERDAY";
			treeNode2.Text = "날짜: 어제 ";
			treeNode3.Name = "4";
			treeNode3.Text = "날짜: 목요일 ";
			treeNode4.Name = "3";
			treeNode4.Text = "날짜: 수요일 ";
			treeNode5.Name = "2";
			treeNode5.Text = "날짜: 화요일 ";
			treeNode6.Name = "1";
			treeNode6.Text = "날짜: 월요일 ";
			treeNode7.Name = "0";
			treeNode7.Text = "날짜: 일요일 ";
			treeNode8.Name = "-7";
			treeNode8.Text = "날짜: 지난 주 ";
			treeNode9.Name = "-14";
			treeNode9.Text = "날짜: 2주 전 ";
			treeNode10.Name = "-21";
			treeNode10.Text = "날짜: 3주 전 ";
			treeNode11.Name = "-28";
			treeNode11.Text = "날짜: 4주 전 ";
			treeNode12.Name = "OLD";
			treeNode12.Text = "날짜: 오래된 항목  ";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12});
			this.treeView1.ShowLines = false;
			this.treeView1.ShowPlusMinus = false;
			this.treeView1.ShowRootLines = false;
			this.treeView1.Size = new System.Drawing.Size(150, 425);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.cboUsers);
			this.splitContainer2.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.txtContents);
			this.splitContainer2.Size = new System.Drawing.Size(604, 425);
			this.splitContainer2.SplitterDistance = 25;
			this.splitContainer2.SplitterWidth = 1;
			this.splitContainer2.TabIndex = 0;
			// 
			// cboUsers
			// 
			this.cboUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboUsers.FormattingEnabled = true;
			this.cboUsers.Location = new System.Drawing.Point(76, 3);
			this.cboUsers.Name = "cboUsers";
			this.cboUsers.Size = new System.Drawing.Size(368, 20);
			this.cboUsers.TabIndex = 1;
			this.cboUsers.SelectedIndexChanged += new System.EventHandler(this.cboUsers_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "대화상대:";
			// 
			// txtContents
			// 
			this.txtContents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtContents.Location = new System.Drawing.Point(0, 0);
			this.txtContents.Multiline = true;
			this.txtContents.Name = "txtContents";
			this.txtContents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtContents.Size = new System.Drawing.Size(604, 399);
			this.txtContents.TabIndex = 0;
			// 
			// frmChatHistory
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(755, 425);
			this.Controls.Add(this.splitContainer1);
			this.Name = "frmChatHistory";
			this.Text = "지난 대화 보기";
			this.Load += new System.EventHandler(this.frmChatHistory_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.ComboBox cboUsers;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtContents;
	}
}