namespace NETS_iMan
{
    partial class PrivateMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrivateMessage));
			this.btnSend = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.rtbWriteMsg = new System.Windows.Forms.RichTextBox();
			this.btnEmotion = new System.Windows.Forms.Button();
			this.btnColor = new System.Windows.Forms.Button();
			this.btnFont = new System.Windows.Forms.Button();
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.lblSelfStatus = new System.Windows.Forms.Label();
			this.rtbMessage = new NETS_iMan.RichTextBoxEx();
			this.lblMobile = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.picUser = new System.Windows.Forms.PictureBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnClose = new System.Windows.Forms.Button();
			this.btn22 = new System.Windows.Forms.Button();
			this.btn21 = new System.Windows.Forms.Button();
			this.btn20 = new System.Windows.Forms.Button();
			this.btn19 = new System.Windows.Forms.Button();
			this.btn18 = new System.Windows.Forms.Button();
			this.btn17 = new System.Windows.Forms.Button();
			this.btn16 = new System.Windows.Forms.Button();
			this.btn15 = new System.Windows.Forms.Button();
			this.btn14 = new System.Windows.Forms.Button();
			this.btn13 = new System.Windows.Forms.Button();
			this.btn12 = new System.Windows.Forms.Button();
			this.btn11 = new System.Windows.Forms.Button();
			this.btn10 = new System.Windows.Forms.Button();
			this.btn9 = new System.Windows.Forms.Button();
			this.btn8 = new System.Windows.Forms.Button();
			this.btn7 = new System.Windows.Forms.Button();
			this.btn6 = new System.Windows.Forms.Button();
			this.btn5 = new System.Windows.Forms.Button();
			this.btn4 = new System.Windows.Forms.Button();
			this.btn3 = new System.Windows.Forms.Button();
			this.btnSad = new System.Windows.Forms.Button();
			this.btnSmile = new System.Windows.Forms.Button();
			this.chkOffSound = new System.Windows.Forms.CheckBox();
			this.statusStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSend
			// 
			this.btnSend.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSend.Location = new System.Drawing.Point(0, 48);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(110, 34);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "보내기";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 1500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// rtbWriteMsg
			// 
			this.rtbWriteMsg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbWriteMsg.EnableAutoDragDrop = true;
			this.rtbWriteMsg.Location = new System.Drawing.Point(0, 0);
			this.rtbWriteMsg.Name = "rtbWriteMsg";
			this.rtbWriteMsg.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbWriteMsg.Size = new System.Drawing.Size(416, 57);
			this.rtbWriteMsg.TabIndex = 1;
			this.rtbWriteMsg.Text = "";
			this.rtbWriteMsg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbWriteMsg_KeyDown);
			this.rtbWriteMsg.TextChanged += new System.EventHandler(this.rtbWriteMsg_TextChanged);
			// 
			// btnEmotion
			// 
			this.btnEmotion.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEmotion.BackgroundImage")));
			this.btnEmotion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnEmotion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnEmotion.Location = new System.Drawing.Point(3, 2);
			this.btnEmotion.Name = "btnEmotion";
			this.btnEmotion.Size = new System.Drawing.Size(24, 19);
			this.btnEmotion.TabIndex = 6;
			this.btnEmotion.UseVisualStyleBackColor = true;
			this.btnEmotion.Click += new System.EventHandler(this.btnEmotion_Click);
			// 
			// btnColor
			// 
			this.btnColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnColor.BackgroundImage")));
			this.btnColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnColor.Location = new System.Drawing.Point(32, 2);
			this.btnColor.Name = "btnColor";
			this.btnColor.Size = new System.Drawing.Size(24, 19);
			this.btnColor.TabIndex = 7;
			this.btnColor.UseVisualStyleBackColor = true;
			this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
			// 
			// btnFont
			// 
			this.btnFont.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFont.BackgroundImage")));
			this.btnFont.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnFont.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnFont.Location = new System.Drawing.Point(62, 2);
			this.btnFont.Name = "btnFont";
			this.btnFont.Size = new System.Drawing.Size(24, 19);
			this.btnFont.TabIndex = 8;
			this.btnFont.Text = "button3";
			this.btnFont.UseVisualStyleBackColor = true;
			this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
			// 
			// folderBrowserDialog1
			// 
			this.folderBrowserDialog1.Description = "파일을 저장할 폴더 선택";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 276);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(527, 22);
			this.statusStrip1.TabIndex = 10;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.AutoSize = false;
			this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(468, 17);
			this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer4);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel2MinSize = 82;
			this.splitContainer1.Size = new System.Drawing.Size(527, 276);
			this.splitContainer1.SplitterDistance = 193;
			this.splitContainer1.SplitterWidth = 1;
			this.splitContainer1.TabIndex = 11;
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer4.IsSplitterFixed = true;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.lblSelfStatus);
			this.splitContainer4.Panel1.Controls.Add(this.rtbMessage);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer4.Panel2.Controls.Add(this.lblMobile);
			this.splitContainer4.Panel2.Controls.Add(this.lblName);
			this.splitContainer4.Panel2.Controls.Add(this.picUser);
			this.splitContainer4.Panel2.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this.splitContainer4.Panel2MinSize = 110;
			this.splitContainer4.Size = new System.Drawing.Size(527, 193);
			this.splitContainer4.SplitterDistance = 416;
			this.splitContainer4.SplitterWidth = 1;
			this.splitContainer4.TabIndex = 5;
			// 
			// lblSelfStatus
			// 
			this.lblSelfStatus.BackColor = System.Drawing.Color.NavajoWhite;
			this.lblSelfStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblSelfStatus.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblSelfStatus.Location = new System.Drawing.Point(0, 0);
			this.lblSelfStatus.Name = "lblSelfStatus";
			this.lblSelfStatus.Padding = new System.Windows.Forms.Padding(2);
			this.lblSelfStatus.Size = new System.Drawing.Size(416, 18);
			this.lblSelfStatus.TabIndex = 5;
			this.lblSelfStatus.Visible = false;
			// 
			// rtbMessage
			// 
			this.rtbMessage.BackColor = System.Drawing.SystemColors.Window;
			this.rtbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbMessage.Location = new System.Drawing.Point(0, 0);
			this.rtbMessage.Name = "rtbMessage";
			this.rtbMessage.ReadOnly = true;
			this.rtbMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbMessage.Size = new System.Drawing.Size(416, 193);
			this.rtbMessage.TabIndex = 4;
			this.rtbMessage.Text = "";
			this.rtbMessage.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbMessage_LinkClicked);
			this.rtbMessage.SelectionChanged += new System.EventHandler(this.rtbMessage_SelectionChanged);
			this.rtbMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbMessage_KeyDown);
			// 
			// lblMobile
			// 
			this.lblMobile.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblMobile.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblMobile.ForeColor = System.Drawing.Color.Blue;
			this.lblMobile.Location = new System.Drawing.Point(0, 157);
			this.lblMobile.Margin = new System.Windows.Forms.Padding(0);
			this.lblMobile.Name = "lblMobile";
			this.lblMobile.Size = new System.Drawing.Size(108, 19);
			this.lblMobile.TabIndex = 7;
			this.lblMobile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblName
			// 
			this.lblName.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblName.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblName.ForeColor = System.Drawing.Color.Blue;
			this.lblName.Location = new System.Drawing.Point(0, 138);
			this.lblName.Margin = new System.Windows.Forms.Padding(0);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(108, 19);
			this.lblName.TabIndex = 6;
			this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// picUser
			// 
			this.picUser.BackColor = System.Drawing.Color.Transparent;
			this.picUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.picUser.Cursor = System.Windows.Forms.Cursors.Hand;
			this.picUser.Dock = System.Windows.Forms.DockStyle.Top;
			this.picUser.Location = new System.Drawing.Point(0, 2);
			this.picUser.Name = "picUser";
			this.picUser.Size = new System.Drawing.Size(108, 136);
			this.picUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picUser.TabIndex = 5;
			this.picUser.TabStop = false;
			this.picUser.Visible = false;
			this.picUser.MouseLeave += new System.EventHandler(this.picUser_MouseLeave);
			this.picUser.Click += new System.EventHandler(this.picUser_Click);
			this.picUser.MouseEnter += new System.EventHandler(this.picUser_MouseEnter);
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
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.btnSend);
			this.splitContainer2.Panel2MinSize = 110;
			this.splitContainer2.Size = new System.Drawing.Size(527, 82);
			this.splitContainer2.SplitterDistance = 416;
			this.splitContainer2.SplitterWidth = 1;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer3.IsSplitterFixed = true;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.chkOffSound);
			this.splitContainer3.Panel1.Controls.Add(this.trackBar);
			this.splitContainer3.Panel1.Controls.Add(this.btnFont);
			this.splitContainer3.Panel1.Controls.Add(this.btnColor);
			this.splitContainer3.Panel1.Controls.Add(this.btnEmotion);
			this.splitContainer3.Panel1MinSize = 24;
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.rtbWriteMsg);
			this.splitContainer3.Size = new System.Drawing.Size(416, 82);
			this.splitContainer3.SplitterDistance = 24;
			this.splitContainer3.SplitterWidth = 1;
			this.splitContainer3.TabIndex = 9;
			// 
			// trackBar
			// 
			this.trackBar.AutoSize = false;
			this.trackBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.trackBar.Location = new System.Drawing.Point(312, 0);
			this.trackBar.Maximum = 100;
			this.trackBar.Minimum = 10;
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(104, 24);
			this.trackBar.TabIndex = 9;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar.Value = 100;
			this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.btnClose);
			this.panel1.Controls.Add(this.btn22);
			this.panel1.Controls.Add(this.btn21);
			this.panel1.Controls.Add(this.btn20);
			this.panel1.Controls.Add(this.btn19);
			this.panel1.Controls.Add(this.btn18);
			this.panel1.Controls.Add(this.btn17);
			this.panel1.Controls.Add(this.btn16);
			this.panel1.Controls.Add(this.btn15);
			this.panel1.Controls.Add(this.btn14);
			this.panel1.Controls.Add(this.btn13);
			this.panel1.Controls.Add(this.btn12);
			this.panel1.Controls.Add(this.btn11);
			this.panel1.Controls.Add(this.btn10);
			this.panel1.Controls.Add(this.btn9);
			this.panel1.Controls.Add(this.btn8);
			this.panel1.Controls.Add(this.btn7);
			this.panel1.Controls.Add(this.btn6);
			this.panel1.Controls.Add(this.btn5);
			this.panel1.Controls.Add(this.btn4);
			this.panel1.Controls.Add(this.btn3);
			this.panel1.Controls.Add(this.btnSad);
			this.panel1.Controls.Add(this.btnSmile);
			this.panel1.Location = new System.Drawing.Point(14, 119);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(157, 84);
			this.panel1.TabIndex = 12;
			// 
			// btnClose
			// 
			this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
			this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Location = new System.Drawing.Point(129, 62);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(24, 19);
			this.btnClose.TabIndex = 32;
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btn22
			// 
			this.btn22.BackColor = System.Drawing.Color.White;
			this.btn22.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn22.BackgroundImage")));
			this.btn22.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn22.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn22.Location = new System.Drawing.Point(91, 62);
			this.btn22.Name = "btn22";
			this.btn22.Size = new System.Drawing.Size(24, 19);
			this.btn22.TabIndex = 31;
			this.btn22.UseVisualStyleBackColor = false;
			this.btn22.Click += new System.EventHandler(this.btn22_Click);
			// 
			// btn21
			// 
			this.btn21.BackColor = System.Drawing.Color.White;
			this.btn21.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn21.BackgroundImage")));
			this.btn21.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn21.Location = new System.Drawing.Point(65, 62);
			this.btn21.Name = "btn21";
			this.btn21.Size = new System.Drawing.Size(24, 19);
			this.btn21.TabIndex = 31;
			this.btn21.UseVisualStyleBackColor = false;
			this.btn21.Click += new System.EventHandler(this.btn21_Click);
			// 
			// btn20
			// 
			this.btn20.BackColor = System.Drawing.Color.White;
			this.btn20.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn20.BackgroundImage")));
			this.btn20.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn20.Location = new System.Drawing.Point(39, 62);
			this.btn20.Name = "btn20";
			this.btn20.Size = new System.Drawing.Size(24, 19);
			this.btn20.TabIndex = 30;
			this.btn20.UseVisualStyleBackColor = false;
			this.btn20.Click += new System.EventHandler(this.btn20_Click);
			// 
			// btn19
			// 
			this.btn19.BackColor = System.Drawing.Color.White;
			this.btn19.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn19.BackgroundImage")));
			this.btn19.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn19.Location = new System.Drawing.Point(1, 62);
			this.btn19.Name = "btn19";
			this.btn19.Size = new System.Drawing.Size(36, 19);
			this.btn19.TabIndex = 29;
			this.btn19.UseVisualStyleBackColor = false;
			this.btn19.Click += new System.EventHandler(this.btn19_Click);
			// 
			// btn18
			// 
			this.btn18.BackColor = System.Drawing.Color.White;
			this.btn18.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn18.BackgroundImage")));
			this.btn18.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn18.Location = new System.Drawing.Point(129, 42);
			this.btn18.Name = "btn18";
			this.btn18.Size = new System.Drawing.Size(24, 19);
			this.btn18.TabIndex = 28;
			this.btn18.UseVisualStyleBackColor = false;
			this.btn18.Click += new System.EventHandler(this.btn18_Click);
			// 
			// btn17
			// 
			this.btn17.BackColor = System.Drawing.Color.White;
			this.btn17.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn17.BackgroundImage")));
			this.btn17.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn17.Location = new System.Drawing.Point(104, 42);
			this.btn17.Name = "btn17";
			this.btn17.Size = new System.Drawing.Size(24, 19);
			this.btn17.TabIndex = 27;
			this.btn17.UseVisualStyleBackColor = false;
			this.btn17.Click += new System.EventHandler(this.btn17_Click);
			// 
			// btn16
			// 
			this.btn16.BackColor = System.Drawing.Color.White;
			this.btn16.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn16.BackgroundImage")));
			this.btn16.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn16.Location = new System.Drawing.Point(78, 42);
			this.btn16.Name = "btn16";
			this.btn16.Size = new System.Drawing.Size(24, 19);
			this.btn16.TabIndex = 26;
			this.btn16.UseVisualStyleBackColor = false;
			this.btn16.Click += new System.EventHandler(this.btn16_Click);
			// 
			// btn15
			// 
			this.btn15.BackColor = System.Drawing.Color.White;
			this.btn15.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn15.BackgroundImage")));
			this.btn15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn15.Location = new System.Drawing.Point(52, 42);
			this.btn15.Name = "btn15";
			this.btn15.Size = new System.Drawing.Size(24, 19);
			this.btn15.TabIndex = 25;
			this.btn15.UseVisualStyleBackColor = false;
			this.btn15.Click += new System.EventHandler(this.btn15_Click);
			// 
			// btn14
			// 
			this.btn14.BackColor = System.Drawing.Color.White;
			this.btn14.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn14.BackgroundImage")));
			this.btn14.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn14.Location = new System.Drawing.Point(27, 42);
			this.btn14.Name = "btn14";
			this.btn14.Size = new System.Drawing.Size(24, 19);
			this.btn14.TabIndex = 24;
			this.btn14.UseVisualStyleBackColor = false;
			this.btn14.Click += new System.EventHandler(this.btn14_Click);
			// 
			// btn13
			// 
			this.btn13.BackColor = System.Drawing.Color.White;
			this.btn13.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn13.BackgroundImage")));
			this.btn13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn13.Location = new System.Drawing.Point(1, 42);
			this.btn13.Name = "btn13";
			this.btn13.Size = new System.Drawing.Size(24, 19);
			this.btn13.TabIndex = 23;
			this.btn13.UseVisualStyleBackColor = false;
			this.btn13.Click += new System.EventHandler(this.btn13_Click);
			// 
			// btn12
			// 
			this.btn12.BackColor = System.Drawing.Color.White;
			this.btn12.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn12.BackgroundImage")));
			this.btn12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn12.Location = new System.Drawing.Point(129, 21);
			this.btn12.Name = "btn12";
			this.btn12.Size = new System.Drawing.Size(24, 19);
			this.btn12.TabIndex = 22;
			this.btn12.UseVisualStyleBackColor = false;
			this.btn12.Click += new System.EventHandler(this.btn12_Click);
			// 
			// btn11
			// 
			this.btn11.BackColor = System.Drawing.Color.White;
			this.btn11.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn11.BackgroundImage")));
			this.btn11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn11.Location = new System.Drawing.Point(104, 21);
			this.btn11.Name = "btn11";
			this.btn11.Size = new System.Drawing.Size(24, 19);
			this.btn11.TabIndex = 21;
			this.btn11.UseVisualStyleBackColor = false;
			this.btn11.Click += new System.EventHandler(this.btn11_Click);
			// 
			// btn10
			// 
			this.btn10.BackColor = System.Drawing.Color.White;
			this.btn10.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn10.BackgroundImage")));
			this.btn10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn10.Location = new System.Drawing.Point(78, 21);
			this.btn10.Name = "btn10";
			this.btn10.Size = new System.Drawing.Size(24, 19);
			this.btn10.TabIndex = 20;
			this.btn10.UseVisualStyleBackColor = false;
			this.btn10.Click += new System.EventHandler(this.btn10_Click);
			// 
			// btn9
			// 
			this.btn9.BackColor = System.Drawing.Color.White;
			this.btn9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn9.BackgroundImage")));
			this.btn9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn9.Location = new System.Drawing.Point(52, 21);
			this.btn9.Name = "btn9";
			this.btn9.Size = new System.Drawing.Size(24, 19);
			this.btn9.TabIndex = 19;
			this.btn9.UseVisualStyleBackColor = false;
			this.btn9.Click += new System.EventHandler(this.btn9_Click);
			// 
			// btn8
			// 
			this.btn8.BackColor = System.Drawing.Color.White;
			this.btn8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn8.BackgroundImage")));
			this.btn8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn8.Location = new System.Drawing.Point(27, 21);
			this.btn8.Name = "btn8";
			this.btn8.Size = new System.Drawing.Size(24, 19);
			this.btn8.TabIndex = 18;
			this.btn8.UseVisualStyleBackColor = false;
			this.btn8.Click += new System.EventHandler(this.btn8_Click);
			// 
			// btn7
			// 
			this.btn7.BackColor = System.Drawing.Color.White;
			this.btn7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn7.BackgroundImage")));
			this.btn7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn7.Location = new System.Drawing.Point(1, 21);
			this.btn7.Name = "btn7";
			this.btn7.Size = new System.Drawing.Size(24, 19);
			this.btn7.TabIndex = 17;
			this.btn7.UseVisualStyleBackColor = false;
			this.btn7.Click += new System.EventHandler(this.btn7_Click);
			// 
			// btn6
			// 
			this.btn6.BackColor = System.Drawing.Color.White;
			this.btn6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn6.BackgroundImage")));
			this.btn6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn6.Location = new System.Drawing.Point(129, 1);
			this.btn6.Name = "btn6";
			this.btn6.Size = new System.Drawing.Size(24, 19);
			this.btn6.TabIndex = 16;
			this.btn6.UseVisualStyleBackColor = false;
			this.btn6.Click += new System.EventHandler(this.btn6_Click);
			// 
			// btn5
			// 
			this.btn5.BackColor = System.Drawing.Color.White;
			this.btn5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn5.BackgroundImage")));
			this.btn5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn5.Location = new System.Drawing.Point(104, 1);
			this.btn5.Name = "btn5";
			this.btn5.Size = new System.Drawing.Size(24, 19);
			this.btn5.TabIndex = 15;
			this.btn5.UseVisualStyleBackColor = false;
			this.btn5.Click += new System.EventHandler(this.btn5_Click);
			// 
			// btn4
			// 
			this.btn4.BackColor = System.Drawing.Color.White;
			this.btn4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn4.BackgroundImage")));
			this.btn4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn4.Location = new System.Drawing.Point(78, 1);
			this.btn4.Name = "btn4";
			this.btn4.Size = new System.Drawing.Size(24, 19);
			this.btn4.TabIndex = 14;
			this.btn4.UseVisualStyleBackColor = false;
			this.btn4.Click += new System.EventHandler(this.btn4_Click);
			// 
			// btn3
			// 
			this.btn3.BackColor = System.Drawing.Color.White;
			this.btn3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn3.BackgroundImage")));
			this.btn3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btn3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn3.Location = new System.Drawing.Point(52, 1);
			this.btn3.Name = "btn3";
			this.btn3.Size = new System.Drawing.Size(24, 19);
			this.btn3.TabIndex = 13;
			this.btn3.UseVisualStyleBackColor = false;
			this.btn3.Click += new System.EventHandler(this.btn3_Click);
			// 
			// btnSad
			// 
			this.btnSad.BackColor = System.Drawing.Color.White;
			this.btnSad.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSad.BackgroundImage")));
			this.btnSad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnSad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSad.Location = new System.Drawing.Point(27, 1);
			this.btnSad.Name = "btnSad";
			this.btnSad.Size = new System.Drawing.Size(24, 19);
			this.btnSad.TabIndex = 12;
			this.btnSad.UseVisualStyleBackColor = false;
			this.btnSad.Click += new System.EventHandler(this.btnSad_Click);
			// 
			// btnSmile
			// 
			this.btnSmile.BackColor = System.Drawing.Color.White;
			this.btnSmile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSmile.BackgroundImage")));
			this.btnSmile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnSmile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSmile.Location = new System.Drawing.Point(1, 1);
			this.btnSmile.Name = "btnSmile";
			this.btnSmile.Size = new System.Drawing.Size(24, 19);
			this.btnSmile.TabIndex = 11;
			this.btnSmile.UseVisualStyleBackColor = false;
			this.btnSmile.Click += new System.EventHandler(this.btnSmile_Click);
			// 
			// chkOffSound
			// 
			this.chkOffSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkOffSound.AutoSize = true;
			this.chkOffSound.Location = new System.Drawing.Point(230, 5);
			this.chkOffSound.Name = "chkOffSound";
			this.chkOffSound.Size = new System.Drawing.Size(64, 16);
			this.chkOffSound.TabIndex = 10;
			this.chkOffSound.Text = "소리 끔";
			this.chkOffSound.UseVisualStyleBackColor = true;
			// 
			// PrivateMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 298);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(196, 196);
			this.Name = "PrivateMessage";
			this.Text = "개인 대화";
			this.Resize += new System.EventHandler(this.PrivateMessage_Resize);
			this.Activated += new System.EventHandler(this.PrivateMessage_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrivateMessage_FormClosing);
			this.Load += new System.EventHandler(this.PrivateMessage_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel1.PerformLayout();
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Timer timer1;
        private RichTextBoxEx rtbMessage;
        private System.Windows.Forms.RichTextBox rtbWriteMsg;
        private System.Windows.Forms.Button btnEmotion;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btn22;
		private System.Windows.Forms.Button btn21;
		private System.Windows.Forms.Button btn20;
		private System.Windows.Forms.Button btn19;
		private System.Windows.Forms.Button btn18;
		private System.Windows.Forms.Button btn17;
		private System.Windows.Forms.Button btn16;
		private System.Windows.Forms.Button btn15;
		private System.Windows.Forms.Button btn14;
		private System.Windows.Forms.Button btn13;
		private System.Windows.Forms.Button btn12;
		private System.Windows.Forms.Button btn11;
		private System.Windows.Forms.Button btn10;
		private System.Windows.Forms.Button btn9;
		private System.Windows.Forms.Button btn8;
		private System.Windows.Forms.Button btn7;
		private System.Windows.Forms.Button btn6;
		private System.Windows.Forms.Button btn5;
		private System.Windows.Forms.Button btn4;
		private System.Windows.Forms.Button btn3;
		private System.Windows.Forms.Button btnSad;
		private System.Windows.Forms.Button btnSmile;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.PictureBox picUser;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.Label lblMobile;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label lblSelfStatus;
		private System.Windows.Forms.TrackBar trackBar;
		private System.Windows.Forms.CheckBox chkOffSound;
    }
}