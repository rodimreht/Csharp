namespace oBrowser2
{
	partial class frmMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loginRetryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.openbrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.alarmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.expeditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flSavSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fleetMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.popupMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.showMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openbrowserToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.treeContainer = new System.Windows.Forms.SplitContainer();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.treeView = new System.Windows.Forms.TreeView();
			this.popupMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.updateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.containerPanel = new System.Windows.Forms.Panel();
			this.popupMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.expeditionToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.resMoveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resMoveToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.fleetSavingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fleetMove2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.popupMenuStrip.SuspendLayout();
			this.treeContainer.Panel1.SuspendLayout();
			this.treeContainer.Panel2.SuspendLayout();
			this.treeContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.popupMenuStrip2.SuspendLayout();
			this.popupMenuStrip3.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
			this.menuStrip.Size = new System.Drawing.Size(262, 24);
			this.menuStrip.TabIndex = 1;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.loginRetryToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.fileToolStripMenuItem.Text = "파일(&F)";
			// 
			// loginToolStripMenuItem
			// 
			this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
			this.loginToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.loginToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.loginToolStripMenuItem.Text = "O-Game 접속(&L)";
			this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
			// 
			// loginRetryToolStripMenuItem
			// 
			this.loginRetryToolStripMenuItem.Enabled = false;
			this.loginRetryToolStripMenuItem.Name = "loginRetryToolStripMenuItem";
			this.loginRetryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.loginRetryToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.loginRetryToolStripMenuItem.Text = "O-Game 재접속(&R)";
			this.loginRetryToolStripMenuItem.Click += new System.EventHandler(this.loginRetryToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.exitToolStripMenuItem.Text = "종료(&X)";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.resRefreshToolStripMenuItem,
            this.toolStripMenuItem4,
            this.openbrowserToolStripMenuItem,
            this.toolStripMenuItem1,
            this.alarmToolStripMenuItem,
            this.expeditionToolStripMenuItem,
            this.resMoveToolStripMenuItem,
            this.flSavSettingToolStripMenuItem,
            this.fleetMoveToolStripMenuItem,
            this.toolStripMenuItem2,
            this.optionsToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.toolsToolStripMenuItem.Text = "도구(&T)";
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.refreshToolStripMenuItem.Text = "함대현황 새로고침(&F)";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// resRefreshToolStripMenuItem
			// 
			this.resRefreshToolStripMenuItem.Name = "resRefreshToolStripMenuItem";
			this.resRefreshToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.resRefreshToolStripMenuItem.Text = "자원현황 새로고침(&R)";
			this.resRefreshToolStripMenuItem.Click += new System.EventHandler(this.resRefreshToolStripMenuItem_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(230, 6);
			// 
			// openbrowserToolStripMenuItem
			// 
			this.openbrowserToolStripMenuItem.Name = "openbrowserToolStripMenuItem";
			this.openbrowserToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.openbrowserToolStripMenuItem.Text = "웹 브라우저로 보기(&B)";
			this.openbrowserToolStripMenuItem.Click += new System.EventHandler(this.openbrowserToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(230, 6);
			// 
			// alarmToolStripMenuItem
			// 
			this.alarmToolStripMenuItem.Name = "alarmToolStripMenuItem";
			this.alarmToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.alarmToolStripMenuItem.Text = "이벤트 알림 및 예약 설정(&A)...";
			this.alarmToolStripMenuItem.Click += new System.EventHandler(this.alarmToolStripMenuItem_Click);
			// 
			// expeditionToolStripMenuItem
			// 
			this.expeditionToolStripMenuItem.Name = "expeditionToolStripMenuItem";
			this.expeditionToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.expeditionToolStripMenuItem.Text = "원정 설정(&E)...";
			this.expeditionToolStripMenuItem.Click += new System.EventHandler(this.expeditionToolStripMenuItem_Click);
			// 
			// resMoveToolStripMenuItem
			// 
			this.resMoveToolStripMenuItem.Name = "resMoveToolStripMenuItem";
			this.resMoveToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.resMoveToolStripMenuItem.Text = "자원 모으기 설정(&C)...";
			this.resMoveToolStripMenuItem.Click += new System.EventHandler(this.resMoveToolStripMenuItem_Click);
			// 
			// flSavSettingToolStripMenuItem
			// 
			this.flSavSettingToolStripMenuItem.Name = "flSavSettingToolStripMenuItem";
			this.flSavSettingToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.flSavSettingToolStripMenuItem.Text = "플릿 세이빙 설정(&S)...";
			this.flSavSettingToolStripMenuItem.Click += new System.EventHandler(this.flSavSettingToolStripMenuItem_Click);
			// 
			// fleetMoveToolStripMenuItem
			// 
			this.fleetMoveToolStripMenuItem.Name = "fleetMoveToolStripMenuItem";
			this.fleetMoveToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.fleetMoveToolStripMenuItem.Text = "함대 기동 설정(&M)...";
			this.fleetMoveToolStripMenuItem.Click += new System.EventHandler(this.fleetMoveToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(230, 6);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.optionsToolStripMenuItem.Text = "옵션(&O)...";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.testToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
			this.helpToolStripMenuItem.Text = "도움말(&H)";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
			this.aboutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.aboutToolStripMenuItem.Text = "이 프로그램은...(&A)";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// popupMenuStrip
			// 
			this.popupMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMenuItem,
            this.openbrowserToolStripMenuItem2,
            this.toolStripMenuItem3,
            this.exitMenuItem});
			this.popupMenuStrip.Name = "popupMenuStrip";
			this.popupMenuStrip.Size = new System.Drawing.Size(191, 76);
			this.popupMenuStrip.Text = "O-Game 메뉴";
			// 
			// showMenuItem
			// 
			this.showMenuItem.Name = "showMenuItem";
			this.showMenuItem.Size = new System.Drawing.Size(190, 22);
			this.showMenuItem.Text = "창 보이기";
			this.showMenuItem.Click += new System.EventHandler(this.showMenuItem_Click);
			// 
			// openbrowserToolStripMenuItem2
			// 
			this.openbrowserToolStripMenuItem2.Name = "openbrowserToolStripMenuItem2";
			this.openbrowserToolStripMenuItem2.Size = new System.Drawing.Size(190, 22);
			this.openbrowserToolStripMenuItem2.Text = "웹 브라우저로 보기(&B)";
			this.openbrowserToolStripMenuItem2.Click += new System.EventHandler(this.openbrowserToolStripMenuItem2_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(187, 6);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(190, 22);
			this.exitMenuItem.Text = "종료(&X)";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// treeContainer
			// 
			this.treeContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.treeContainer.IsSplitterFixed = true;
			this.treeContainer.Location = new System.Drawing.Point(0, 24);
			this.treeContainer.Name = "treeContainer";
			this.treeContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// treeContainer.Panel1
			// 
			this.treeContainer.Panel1.BackColor = System.Drawing.Color.White;
			this.treeContainer.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.treeContainer.Panel1.Controls.Add(this.pictureBox1);
			// 
			// treeContainer.Panel2
			// 
			this.treeContainer.Panel2.Controls.Add(this.treeView);
			this.treeContainer.Size = new System.Drawing.Size(262, 443);
			this.treeContainer.SplitterDistance = 45;
			this.treeContainer.SplitterWidth = 1;
			this.treeContainer.TabIndex = 3;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(262, 45);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Font = new System.Drawing.Font("돋움체", 9F);
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(262, 397);
			this.treeView.TabIndex = 0;
			this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
			// 
			// popupMenuStrip2
			// 
			this.popupMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateMenuItem});
			this.popupMenuStrip2.Name = "popupMenuStrip";
			this.popupMenuStrip2.Size = new System.Drawing.Size(192, 26);
			// 
			// updateMenuItem
			// 
			this.updateMenuItem.Name = "updateMenuItem";
			this.updateMenuItem.Size = new System.Drawing.Size(191, 22);
			this.updateMenuItem.Text = "지금 자원현황 갱신(&U)";
			this.updateMenuItem.Click += new System.EventHandler(this.updateMenuItem_Click);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.notifyIcon1.BalloonTipText = "O-Game 모니터링 도구입니다.";
			this.notifyIcon1.BalloonTipTitle = "O-Game 브라우저 Vol.2";
			this.notifyIcon1.ContextMenuStrip = this.popupMenuStrip;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "O-Game 브라우저 Vol.2";
			this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			// 
			// containerPanel
			// 
			this.containerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerPanel.Location = new System.Drawing.Point(0, 24);
			this.containerPanel.Name = "containerPanel";
			this.containerPanel.Size = new System.Drawing.Size(262, 443);
			this.containerPanel.TabIndex = 4;
			this.containerPanel.Visible = false;
			// 
			// popupMenuStrip3
			// 
			this.popupMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expeditionToolStripMenuItem2,
            this.resMoveAllToolStripMenuItem,
            this.resMoveToolStripMenuItem2,
            this.fleetSavingToolStripMenuItem,
            this.fleetMove2ToolStripMenuItem});
			this.popupMenuStrip3.Name = "popupMenuStrip3";
			this.popupMenuStrip3.Size = new System.Drawing.Size(258, 114);
			this.popupMenuStrip3.Opening += new System.ComponentModel.CancelEventHandler(this.popupMenuStrip3_Opening);
			// 
			// expeditionToolStripMenuItem2
			// 
			this.expeditionToolStripMenuItem2.Name = "expeditionToolStripMenuItem2";
			this.expeditionToolStripMenuItem2.Size = new System.Drawing.Size(257, 22);
			this.expeditionToolStripMenuItem2.Text = "원정함대 출발";
			this.expeditionToolStripMenuItem2.ToolTipText = "원정 옵션에서 설정된 내용대로 즉시 원정함대를 보냅니다.";
			this.expeditionToolStripMenuItem2.Click += new System.EventHandler(this.expeditionToolStripMenuItem2_Click);
			// 
			// resMoveAllToolStripMenuItem
			// 
			this.resMoveAllToolStripMenuItem.Name = "resMoveAllToolStripMenuItem";
			this.resMoveAllToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
			this.resMoveAllToolStripMenuItem.Text = "이 은하의 모든 행성에서 자원 운송";
			this.resMoveAllToolStripMenuItem.ToolTipText = "이 은하에 있는 모든 행성들의 모든 자원을 즉시 지정된 행성으로 보냅니다.";
			this.resMoveAllToolStripMenuItem.Click += new System.EventHandler(this.resMoveAllToolStripMenuItem_Click);
			// 
			// resMoveToolStripMenuItem2
			// 
			this.resMoveToolStripMenuItem2.Name = "resMoveToolStripMenuItem2";
			this.resMoveToolStripMenuItem2.Size = new System.Drawing.Size(257, 22);
			this.resMoveToolStripMenuItem2.Text = "이 행성에서 자원 운송";
			this.resMoveToolStripMenuItem2.ToolTipText = "이 행성의 모든 자원을 즉시 지정된 행성으로 보냅니다.";
			this.resMoveToolStripMenuItem2.Click += new System.EventHandler(this.resMoveToolStripMenuItem2_Click);
			// 
			// fleetSavingToolStripMenuItem
			// 
			this.fleetSavingToolStripMenuItem.Name = "fleetSavingToolStripMenuItem";
			this.fleetSavingToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
			this.fleetSavingToolStripMenuItem.Text = "이 행성에서 플릿 세이빙!!";
			this.fleetSavingToolStripMenuItem.ToolTipText = "이 행성의 모든 함대와 자원을 10% 속도로 목적 행성으로 출발시킵니다.";
			this.fleetSavingToolStripMenuItem.Click += new System.EventHandler(this.fleetSavingToolStripMenuItem_Click);
			// 
			// fleetMove2ToolStripMenuItem
			// 
			this.fleetMove2ToolStripMenuItem.Name = "fleetMove2ToolStripMenuItem";
			this.fleetMove2ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
			this.fleetMove2ToolStripMenuItem.Text = "기동함대 출발";
			this.fleetMove2ToolStripMenuItem.ToolTipText = "함대 기동 옵션에서 설정된 내용대로 즉시 기동함대를 보냅니다.";
			this.fleetMove2ToolStripMenuItem.Click += new System.EventHandler(this.fleetMove2ToolStripMenuItem_Click);
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.testToolStripMenuItem.Text = "Test(&T)";
			this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(262, 467);
			this.Controls.Add(this.containerPanel);
			this.Controls.Add(this.treeContainer);
			this.Controls.Add(this.menuStrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "O-Game 브라우저 Vol.2";
			this.Activated += new System.EventHandler(this.frmMain_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.popupMenuStrip.ResumeLayout(false);
			this.treeContainer.Panel1.ResumeLayout(false);
			this.treeContainer.Panel2.ResumeLayout(false);
			this.treeContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.popupMenuStrip2.ResumeLayout(false);
			this.popupMenuStrip3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip popupMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem showMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.SplitContainer treeContainer;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openbrowserToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip popupMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem updateMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resRefreshToolStripMenuItem;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.Panel containerPanel;
		private System.Windows.Forms.ToolStripMenuItem loginRetryToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem alarmToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem openbrowserToolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem expeditionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resMoveToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip popupMenuStrip3;
		private System.Windows.Forms.ToolStripMenuItem expeditionToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem resMoveToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem fleetSavingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resMoveAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flSavSettingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fleetMoveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fleetMove2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
	}
}

