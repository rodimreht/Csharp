namespace NETS_iMan
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
			this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.msgLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.devSiteBrowseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mListBrowseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dListBrowseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.topMostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showOnlyOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.offLoginAlarmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.popupMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.showMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatusToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatus11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatus33ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatus22ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.treeContainer = new System.Windows.Forms.SplitContainer();
			this.picSplitContainer = new System.Windows.Forms.SplitContainer();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.treeView = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.containerPanel = new System.Windows.Forms.Panel();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.chatTimer = new System.Windows.Forms.Timer(this.components);
			this.statusTimer = new System.Windows.Forms.Timer(this.components);
			this.opacityTimer = new System.Windows.Forms.Timer(this.components);
			this.popupMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.viewProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openPrvHomePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.netsQAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatus1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatus3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeStatus2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.privateMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.offlineMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.popupMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.offlineMessageToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
			this.menuStrip.SuspendLayout();
			this.popupMenuStrip.SuspendLayout();
			this.treeContainer.Panel1.SuspendLayout();
			this.treeContainer.Panel2.SuspendLayout();
			this.treeContainer.SuspendLayout();
			this.picSplitContainer.Panel1.SuspendLayout();
			this.picSplitContainer.Panel2.SuspendLayout();
			this.picSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            this.logoutToolStripMenuItem,
            this.toolStripMenuItem4,
            this.msgLogToolStripMenuItem,
            this.toolStripMenuItem5,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.fileToolStripMenuItem.Text = "파일(&F)";
			// 
			// loginToolStripMenuItem
			// 
			this.loginToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loginToolStripMenuItem.Image")));
			this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
			this.loginToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.loginToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.loginToolStripMenuItem.Text = "로그인(&L)";
			this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
			// 
			// logoutToolStripMenuItem
			// 
			this.logoutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("logoutToolStripMenuItem.Image")));
			this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
			this.logoutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.logoutToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.logoutToolStripMenuItem.Text = "로그아웃(&O)";
			this.logoutToolStripMenuItem.Visible = false;
			this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(201, 6);
			// 
			// msgLogToolStripMenuItem
			// 
			this.msgLogToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("msgLogToolStripMenuItem.Image")));
			this.msgLogToolStripMenuItem.Name = "msgLogToolStripMenuItem";
			this.msgLogToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.msgLogToolStripMenuItem.Text = "지난 대화 기록(&L) 보기...";
			this.msgLogToolStripMenuItem.Click += new System.EventHandler(this.msgLogToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(201, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.exitToolStripMenuItem.Text = "종료(&X)";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.devSiteBrowseToolStripMenuItem,
            this.mListBrowseToolStripMenuItem,
            this.dListBrowseToolStripMenuItem,
            this.toolStripMenuItem3,
            this.topMostToolStripMenuItem,
            this.showOnlyOnlineToolStripMenuItem,
            this.offLoginAlarmToolStripMenuItem,
            this.toolStripMenuItem1,
            this.optionsToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.toolsToolStripMenuItem.Text = "도구(&T)";
			// 
			// devSiteBrowseToolStripMenuItem
			// 
			this.devSiteBrowseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("devSiteBrowseToolStripMenuItem.Image")));
			this.devSiteBrowseToolStripMenuItem.Name = "devSiteBrowseToolStripMenuItem";
			this.devSiteBrowseToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
			this.devSiteBrowseToolStripMenuItem.Text = "넷츠 커뮤니티 사이트 접속(&D)";
			this.devSiteBrowseToolStripMenuItem.Click += new System.EventHandler(this.devSiteBrowseToolStripMenuItem_Click);
			// 
			// mListBrowseToolStripMenuItem
			// 
			this.mListBrowseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mListBrowseToolStripMenuItem.Image")));
			this.mListBrowseToolStripMenuItem.Name = "mListBrowseToolStripMenuItem";
			this.mListBrowseToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
			this.mListBrowseToolStripMenuItem.Text = "넷츠 직원 목록(&M)";
			this.mListBrowseToolStripMenuItem.Click += new System.EventHandler(this.mListBrowseToolStripMenuItem_Click);
			// 
			// dListBrowseToolStripMenuItem
			// 
			this.dListBrowseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("dListBrowseToolStripMenuItem.Image")));
			this.dListBrowseToolStripMenuItem.Name = "dListBrowseToolStripMenuItem";
			this.dListBrowseToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
			this.dListBrowseToolStripMenuItem.Text = "넷츠 인프라 자료 목록(&L)";
			this.dListBrowseToolStripMenuItem.Click += new System.EventHandler(this.dListBrowseToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(227, 6);
			// 
			// topMostToolStripMenuItem
			// 
			this.topMostToolStripMenuItem.CheckOnClick = true;
			this.topMostToolStripMenuItem.Name = "topMostToolStripMenuItem";
			this.topMostToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
			this.topMostToolStripMenuItem.Text = "항상 위에(&T)";
			this.topMostToolStripMenuItem.Click += new System.EventHandler(this.topMostToolStripMenuItem_Click);
			// 
			// showOnlyOnlineToolStripMenuItem
			// 
			this.showOnlyOnlineToolStripMenuItem.CheckOnClick = true;
			this.showOnlyOnlineToolStripMenuItem.Name = "showOnlyOnlineToolStripMenuItem";
			this.showOnlyOnlineToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
			this.showOnlyOnlineToolStripMenuItem.Text = "온라인 사용자만 표시(&S)";
			this.showOnlyOnlineToolStripMenuItem.Click += new System.EventHandler(this.showOnlyOnlineToolStripMenuItem_Click);
			// 
			// offLoginAlarmToolStripMenuItem
			// 
			this.offLoginAlarmToolStripMenuItem.CheckOnClick = true;
			this.offLoginAlarmToolStripMenuItem.Name = "offLoginAlarmToolStripMenuItem";
			this.offLoginAlarmToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
			this.offLoginAlarmToolStripMenuItem.Text = "로그인 알림 기능 끄기(&F)";
			this.offLoginAlarmToolStripMenuItem.Click += new System.EventHandler(this.offLoginAlarmToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(227, 6);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripMenuItem.Image")));
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
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
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.testToolStripMenuItem.Text = "Test(&T)";
			this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
			// 
			// popupMenuStrip
			// 
			this.popupMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMenuItem,
            this.changeStatusToolStripMenuItem2,
            this.exitMenuItem});
			this.popupMenuStrip.Name = "popupMenuStrip";
			this.popupMenuStrip.Size = new System.Drawing.Size(153, 92);
			this.popupMenuStrip.Text = "트레이 메뉴";
			// 
			// showMenuItem
			// 
			this.showMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showMenuItem.Image")));
			this.showMenuItem.Name = "showMenuItem";
			this.showMenuItem.Size = new System.Drawing.Size(152, 22);
			this.showMenuItem.Text = "창 보이기(&S)";
			this.showMenuItem.Click += new System.EventHandler(this.showMenuItem_Click);
			// 
			// changeStatusToolStripMenuItem2
			// 
			this.changeStatusToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeStatus11ToolStripMenuItem,
            this.changeStatus33ToolStripMenuItem,
            this.changeStatus22ToolStripMenuItem});
			this.changeStatusToolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("changeStatusToolStripMenuItem2.Image")));
			this.changeStatusToolStripMenuItem2.Name = "changeStatusToolStripMenuItem2";
			this.changeStatusToolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
			this.changeStatusToolStripMenuItem2.Text = "상태 변경(&T)...";
			// 
			// changeStatus11ToolStripMenuItem
			// 
			this.changeStatus11ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatus11ToolStripMenuItem.Image")));
			this.changeStatus11ToolStripMenuItem.Name = "changeStatus11ToolStripMenuItem";
			this.changeStatus11ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.changeStatus11ToolStripMenuItem.Text = "온라인(&O)";
			this.changeStatus11ToolStripMenuItem.Click += new System.EventHandler(this.changeStatus1ToolStripMenuItem_Click);
			// 
			// changeStatus33ToolStripMenuItem
			// 
			this.changeStatus33ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatus33ToolStripMenuItem.Image")));
			this.changeStatus33ToolStripMenuItem.Name = "changeStatus33ToolStripMenuItem";
			this.changeStatus33ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.changeStatus33ToolStripMenuItem.Text = "자리 비움(&A)";
			this.changeStatus33ToolStripMenuItem.Click += new System.EventHandler(this.changeStatus3ToolStripMenuItem_Click);
			// 
			// changeStatus22ToolStripMenuItem
			// 
			this.changeStatus22ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatus22ToolStripMenuItem.Image")));
			this.changeStatus22ToolStripMenuItem.Name = "changeStatus22ToolStripMenuItem";
			this.changeStatus22ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.changeStatus22ToolStripMenuItem.Text = "다른 용무 중(&B)";
			this.changeStatus22ToolStripMenuItem.Click += new System.EventHandler(this.changeStatus2ToolStripMenuItem_Click);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitMenuItem.Image")));
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(152, 22);
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
			this.treeContainer.Panel1.Controls.Add(this.picSplitContainer);
			// 
			// treeContainer.Panel2
			// 
			this.treeContainer.Panel2.Controls.Add(this.treeView);
			this.treeContainer.Panel2.Controls.Add(this.containerPanel);
			this.treeContainer.Size = new System.Drawing.Size(262, 443);
			this.treeContainer.SplitterDistance = 45;
			this.treeContainer.SplitterWidth = 1;
			this.treeContainer.TabIndex = 3;
			this.treeContainer.TabStop = false;
			// 
			// picSplitContainer
			// 
			this.picSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.picSplitContainer.IsSplitterFixed = true;
			this.picSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.picSplitContainer.Name = "picSplitContainer";
			// 
			// picSplitContainer.Panel1
			// 
			this.picSplitContainer.Panel1.Controls.Add(this.pictureBox2);
			// 
			// picSplitContainer.Panel2
			// 
			this.picSplitContainer.Panel2.Controls.Add(this.pictureBox1);
			this.picSplitContainer.Size = new System.Drawing.Size(262, 45);
			this.picSplitContainer.SplitterDistance = 42;
			this.picSplitContainer.SplitterWidth = 1;
			this.picSplitContainer.TabIndex = 1;
			this.picSplitContainer.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox2.Location = new System.Drawing.Point(0, 0);
			this.pictureBox2.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(42, 45);
			this.pictureBox2.TabIndex = 0;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(219, 45);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Font = new System.Drawing.Font("돋움체", 9F);
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageList1;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = 0;
			this.treeView.Size = new System.Drawing.Size(262, 397);
			this.treeView.TabIndex = 0;
			this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
			this.treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseUp);
			this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "0.bmp");
			this.imageList1.Images.SetKeyName(1, "1.bmp");
			this.imageList1.Images.SetKeyName(2, "2.bmp");
			this.imageList1.Images.SetKeyName(3, "3.bmp");
			this.imageList1.Images.SetKeyName(4, "box1.gif");
			this.imageList1.Images.SetKeyName(5, "box2.gif");
			this.imageList1.Images.SetKeyName(6, "box3.gif");
			this.imageList1.Images.SetKeyName(7, "box4.gif");
			this.imageList1.Images.SetKeyName(8, "box5.gif");
			this.imageList1.Images.SetKeyName(9, "4.bmp");
			// 
			// containerPanel
			// 
			this.containerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerPanel.Location = new System.Drawing.Point(0, 0);
			this.containerPanel.Name = "containerPanel";
			this.containerPanel.Size = new System.Drawing.Size(262, 397);
			this.containerPanel.TabIndex = 4;
			this.containerPanel.Visible = false;
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.notifyIcon1.BalloonTipText = "NETS-ⓘMan";
			this.notifyIcon1.BalloonTipTitle = "NETS-ⓘMan";
			this.notifyIcon1.ContextMenuStrip = this.popupMenuStrip;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "NETS-ⓘMan";
			this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			// 
			// chatTimer
			// 
			this.chatTimer.Interval = 1500;
			this.chatTimer.Tick += new System.EventHandler(this.chatTimer_Tick);
			// 
			// statusTimer
			// 
			this.statusTimer.Interval = 990;
			this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
			// 
			// opacityTimer
			// 
			this.opacityTimer.Interval = 250;
			this.opacityTimer.Tick += new System.EventHandler(this.opacityTimer_Tick);
			// 
			// popupMenuStrip2
			// 
			this.popupMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewProfileToolStripMenuItem,
            this.openPrvHomePageToolStripMenuItem,
            this.netsQAToolStripMenuItem,
            this.changeStatusToolStripMenuItem,
            this.toolStripMenuItem2,
            this.privateMessageToolStripMenuItem,
            this.offlineMessageToolStripMenuItem,
            this.newGroupToolStripMenuItem});
			this.popupMenuStrip2.Name = "popupMenuStrip2";
			this.popupMenuStrip2.Size = new System.Drawing.Size(180, 164);
			this.popupMenuStrip2.Text = "사용자 메뉴";
			// 
			// viewProfileToolStripMenuItem
			// 
			this.viewProfileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewProfileToolStripMenuItem.Image")));
			this.viewProfileToolStripMenuItem.Name = "viewProfileToolStripMenuItem";
			this.viewProfileToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.viewProfileToolStripMenuItem.Text = "프로필 보기(&V)";
			this.viewProfileToolStripMenuItem.ToolTipText = "선택한 직원의 개인 프로필을 조회합니다.";
			this.viewProfileToolStripMenuItem.Click += new System.EventHandler(this.viewProfileToolStripMenuItem_Click);
			// 
			// openPrvHomePageToolStripMenuItem
			// 
			this.openPrvHomePageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openPrvHomePageToolStripMenuItem.Image")));
			this.openPrvHomePageToolStripMenuItem.Name = "openPrvHomePageToolStripMenuItem";
			this.openPrvHomePageToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.openPrvHomePageToolStripMenuItem.Text = "홈페이지 열기(&H)";
			this.openPrvHomePageToolStripMenuItem.ToolTipText = "해당 직원의 개인 홈페이지에 방문합니다.";
			this.openPrvHomePageToolStripMenuItem.Click += new System.EventHandler(this.openPrvHomePageToolStripMenuItem_Click);
			// 
			// netsQAToolStripMenuItem
			// 
			this.netsQAToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("netsQAToolStripMenuItem.Image")));
			this.netsQAToolStripMenuItem.Name = "netsQAToolStripMenuItem";
			this.netsQAToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.netsQAToolStripMenuItem.Text = "NETS*QA 열기(&Q)";
			this.netsQAToolStripMenuItem.ToolTipText = "타임시트 작성을 위해 NETS*QA 사이트를 엽니다.";
			this.netsQAToolStripMenuItem.Click += new System.EventHandler(this.netsQAToolStripMenuItem_Click);
			// 
			// changeStatusToolStripMenuItem
			// 
			this.changeStatusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeStatus1ToolStripMenuItem,
            this.changeStatus3ToolStripMenuItem,
            this.changeStatus2ToolStripMenuItem});
			this.changeStatusToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatusToolStripMenuItem.Image")));
			this.changeStatusToolStripMenuItem.Name = "changeStatusToolStripMenuItem";
			this.changeStatusToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.changeStatusToolStripMenuItem.Text = "상태 변경(&T)...";
			// 
			// changeStatus1ToolStripMenuItem
			// 
			this.changeStatus1ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatus1ToolStripMenuItem.Image")));
			this.changeStatus1ToolStripMenuItem.Name = "changeStatus1ToolStripMenuItem";
			this.changeStatus1ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.changeStatus1ToolStripMenuItem.Text = "온라인(&O)";
			this.changeStatus1ToolStripMenuItem.Click += new System.EventHandler(this.changeStatus1ToolStripMenuItem_Click);
			// 
			// changeStatus3ToolStripMenuItem
			// 
			this.changeStatus3ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatus3ToolStripMenuItem.Image")));
			this.changeStatus3ToolStripMenuItem.Name = "changeStatus3ToolStripMenuItem";
			this.changeStatus3ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.changeStatus3ToolStripMenuItem.Text = "자리 비움(&A)";
			this.changeStatus3ToolStripMenuItem.Click += new System.EventHandler(this.changeStatus3ToolStripMenuItem_Click);
			// 
			// changeStatus2ToolStripMenuItem
			// 
			this.changeStatus2ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeStatus2ToolStripMenuItem.Image")));
			this.changeStatus2ToolStripMenuItem.Name = "changeStatus2ToolStripMenuItem";
			this.changeStatus2ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.changeStatus2ToolStripMenuItem.Text = "다른 용무 중(&B)";
			this.changeStatus2ToolStripMenuItem.Click += new System.EventHandler(this.changeStatus2ToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(176, 6);
			// 
			// privateMessageToolStripMenuItem
			// 
			this.privateMessageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("privateMessageToolStripMenuItem.Image")));
			this.privateMessageToolStripMenuItem.Name = "privateMessageToolStripMenuItem";
			this.privateMessageToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.privateMessageToolStripMenuItem.Text = "대화하기(&S)";
			this.privateMessageToolStripMenuItem.ToolTipText = "선택한 직원과 1:1 대화를 시작합니다.";
			this.privateMessageToolStripMenuItem.Click += new System.EventHandler(this.privateMessageToolStripMenuItem_Click);
			// 
			// offlineMessageToolStripMenuItem
			// 
			this.offlineMessageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("offlineMessageToolStripMenuItem.Image")));
			this.offlineMessageToolStripMenuItem.Name = "offlineMessageToolStripMenuItem";
			this.offlineMessageToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.offlineMessageToolStripMenuItem.Text = "쪽지 보내기(&S)";
			this.offlineMessageToolStripMenuItem.ToolTipText = "선택한 직원에게 쪽지를 보냅니다.";
			this.offlineMessageToolStripMenuItem.Visible = false;
			this.offlineMessageToolStripMenuItem.Click += new System.EventHandler(this.offlineMessageToolStripMenuItem_Click);
			// 
			// newGroupToolStripMenuItem
			// 
			this.newGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newGroupToolStripMenuItem.Image")));
			this.newGroupToolStripMenuItem.Name = "newGroupToolStripMenuItem";
			this.newGroupToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.newGroupToolStripMenuItem.Text = "새 회의실에 초대(&C)";
			this.newGroupToolStripMenuItem.ToolTipText = "새 회의실을 생성하고 선택한 직원을 초대(포함)합니다.";
			this.newGroupToolStripMenuItem.Visible = false;
			this.newGroupToolStripMenuItem.Click += new System.EventHandler(this.newGroupToolStripMenuItem_Click);
			// 
			// popupMenuStrip3
			// 
			this.popupMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.offlineMessageToolStripMenuItem2});
			this.popupMenuStrip3.Name = "popupMenuStrip3";
			this.popupMenuStrip3.Size = new System.Drawing.Size(179, 48);
			this.popupMenuStrip3.Text = "조직 메뉴";
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.refreshToolStripMenuItem.Text = "새로 고침(&N)";
			this.refreshToolStripMenuItem.ToolTipText = "조직의 구성원 목록을 새로 고칩니다.";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// offlineMessageToolStripMenuItem2
			// 
			this.offlineMessageToolStripMenuItem2.Name = "offlineMessageToolStripMenuItem2";
			this.offlineMessageToolStripMenuItem2.Size = new System.Drawing.Size(178, 22);
			this.offlineMessageToolStripMenuItem2.Text = "그룹 쪽지 보내기(&S)";
			this.offlineMessageToolStripMenuItem2.ToolTipText = "이 조직에 소속된 직원들 모두에게 쪽지를 보낸다.";
			this.offlineMessageToolStripMenuItem2.Click += new System.EventHandler(this.offlineMessageToolStripMenuItem2_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(262, 467);
			this.Controls.Add(this.treeContainer);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(200, 400);
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "NETS-ⓘMan";
			this.ResizeBegin += new System.EventHandler(this.frmMain_ResizeBegin);
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Activated += new System.EventHandler(this.frmMain_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.LocationChanged += new System.EventHandler(this.frmMain_LocationChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
			this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.popupMenuStrip.ResumeLayout(false);
			this.treeContainer.Panel1.ResumeLayout(false);
			this.treeContainer.Panel2.ResumeLayout(false);
			this.treeContainer.ResumeLayout(false);
			this.picSplitContainer.Panel1.ResumeLayout(false);
			this.picSplitContainer.Panel2.ResumeLayout(false);
			this.picSplitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.Panel containerPanel;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem devSiteBrowseToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Timer chatTimer;
		private System.Windows.Forms.Timer statusTimer;
		private System.Windows.Forms.Timer opacityTimer;
		private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip popupMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem viewProfileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem privateMessageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem offlineMessageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mListBrowseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dListBrowseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openPrvHomePageToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem showOnlyOnlineToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem msgLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem newGroupToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip popupMenuStrip3;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem topMostToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem offlineMessageToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem offLoginAlarmToolStripMenuItem;
		private System.Windows.Forms.SplitContainer picSplitContainer;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.ToolTip toolTip2;
		private System.Windows.Forms.ToolStripMenuItem netsQAToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatusToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatus1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatus2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatus3ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatusToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem changeStatus11ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatus33ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeStatus22ToolStripMenuItem;
	}
}

