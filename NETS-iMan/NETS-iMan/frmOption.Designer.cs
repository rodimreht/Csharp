namespace NETS_iMan
{
	partial class frmOption
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
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtOpacity = new System.Windows.Forms.NumericUpDown();
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.lblQA = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.chkShowOnline = new System.Windows.Forms.CheckBox();
			this.chkRemPwd = new System.Windows.Forms.CheckBox();
			this.txtQAPwd = new System.Windows.Forms.TextBox();
			this.txtLogPath = new System.Windows.Forms.TextBox();
			this.chkLoginAlarm = new System.Windows.Forms.CheckBox();
			this.chkAutoStart = new System.Windows.Forms.CheckBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtOpacity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtOpacity);
			this.groupBox1.Controls.Add(this.trackBar);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.lblQA);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.chkShowOnline);
			this.groupBox1.Controls.Add(this.chkRemPwd);
			this.groupBox1.Controls.Add(this.txtQAPwd);
			this.groupBox1.Controls.Add(this.txtLogPath);
			this.groupBox1.Controls.Add(this.chkLoginAlarm);
			this.groupBox1.Controls.Add(this.chkAutoStart);
			this.groupBox1.Controls.Add(this.btnBrowse);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(497, 237);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// txtOpacity
			// 
			this.txtOpacity.Location = new System.Drawing.Point(301, 191);
			this.txtOpacity.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.txtOpacity.Name = "txtOpacity";
			this.txtOpacity.Size = new System.Drawing.Size(49, 21);
			this.txtOpacity.TabIndex = 8;
			this.txtOpacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.txtOpacity.ValueChanged += new System.EventHandler(this.txtOpacity_ValueChanged);
			// 
			// trackBar
			// 
			this.trackBar.AutoSize = false;
			this.trackBar.Location = new System.Drawing.Point(177, 187);
			this.trackBar.Maximum = 100;
			this.trackBar.Minimum = 10;
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(118, 34);
			this.trackBar.TabIndex = 7;
			this.trackBar.TickFrequency = 10;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trackBar.Value = 100;
			this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 193);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(161, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "*메인창/대화창 투명도 조정:";
			// 
			// lblQA
			// 
			this.lblQA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblQA.AutoSize = true;
			this.lblQA.Location = new System.Drawing.Point(14, 163);
			this.lblQA.Name = "lblQA";
			this.lblQA.Size = new System.Drawing.Size(117, 12);
			this.lblQA.TabIndex = 0;
			this.lblQA.Text = "NETS*QA 패스워드:";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "로그파일 경로:";
			// 
			// chkShowOnline
			// 
			this.chkShowOnline.AutoSize = true;
			this.chkShowOnline.Location = new System.Drawing.Point(17, 107);
			this.chkShowOnline.Name = "chkShowOnline";
			this.chkShowOnline.Size = new System.Drawing.Size(140, 16);
			this.chkShowOnline.TabIndex = 4;
			this.chkShowOnline.Text = "온라인 사용자만 표시";
			this.chkShowOnline.UseVisualStyleBackColor = true;
			// 
			// chkRemPwd
			// 
			this.chkRemPwd.AutoSize = true;
			this.chkRemPwd.Location = new System.Drawing.Point(17, 82);
			this.chkRemPwd.Name = "chkRemPwd";
			this.chkRemPwd.Size = new System.Drawing.Size(171, 16);
			this.chkRemPwd.TabIndex = 3;
			this.chkRemPwd.Text = "로그인 ID 및 패스워드 기억";
			this.chkRemPwd.UseVisualStyleBackColor = true;
			// 
			// txtQAPwd
			// 
			this.txtQAPwd.Location = new System.Drawing.Point(135, 160);
			this.txtQAPwd.Name = "txtQAPwd";
			this.txtQAPwd.PasswordChar = '*';
			this.txtQAPwd.Size = new System.Drawing.Size(117, 21);
			this.txtQAPwd.TabIndex = 6;
			// 
			// txtLogPath
			// 
			this.txtLogPath.Location = new System.Drawing.Point(103, 20);
			this.txtLogPath.Name = "txtLogPath";
			this.txtLogPath.Size = new System.Drawing.Size(300, 21);
			this.txtLogPath.TabIndex = 1;
			// 
			// chkLoginAlarm
			// 
			this.chkLoginAlarm.AutoSize = true;
			this.chkLoginAlarm.Location = new System.Drawing.Point(17, 132);
			this.chkLoginAlarm.Name = "chkLoginAlarm";
			this.chkLoginAlarm.Size = new System.Drawing.Size(200, 16);
			this.chkLoginAlarm.TabIndex = 5;
			this.chkLoginAlarm.Text = "다른 직원 로그인 알림 팝업 끄기";
			this.chkLoginAlarm.UseVisualStyleBackColor = true;
			// 
			// chkAutoStart
			// 
			this.chkAutoStart.AutoSize = true;
			this.chkAutoStart.Location = new System.Drawing.Point(17, 57);
			this.chkAutoStart.Name = "chkAutoStart";
			this.chkAutoStart.Size = new System.Drawing.Size(156, 16);
			this.chkAutoStart.TabIndex = 2;
			this.chkAutoStart.Text = "윈도우 시작 시 자동실행";
			this.chkAutoStart.UseVisualStyleBackColor = true;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(399, 19);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(85, 23);
			this.btnBrowse.TabIndex = 4;
			this.btnBrowse.Text = "찾아보기...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Location = new System.Drawing.Point(343, 256);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 8;
			this.btnSave.Text = "저 장";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(425, 256);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "취 소";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmOption
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(513, 290);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOption";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "옵션";
			this.Load += new System.EventHandler(this.frmOption_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtOpacity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtLogPath;
		private System.Windows.Forms.CheckBox chkAutoStart;
		private System.Windows.Forms.CheckBox chkRemPwd;
		private System.Windows.Forms.CheckBox chkLoginAlarm;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox chkShowOnline;
		private System.Windows.Forms.Label lblQA;
		private System.Windows.Forms.TextBox txtQAPwd;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar trackBar;
		private System.Windows.Forms.NumericUpDown txtOpacity;
	}
}