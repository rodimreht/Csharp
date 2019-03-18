namespace NETS_iMan
{
	partial class LoginForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtLoginID = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.chkRemPwd = new System.Windows.Forms.CheckBox();
			this.chkAutoStart = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cancelButton.Location = new System.Drawing.Point(348, 133);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 21);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.okButton.Location = new System.Drawing.Point(254, 133);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(87, 21);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox1.Location = new System.Drawing.Point(16, 18);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(37, 30);
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// txtPassword
			// 
			this.txtPassword.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.txtPassword.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.txtPassword.Location = new System.Drawing.Point(330, 44);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(87, 21);
			this.txtPassword.TabIndex = 6;
			this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label5.Location = new System.Drawing.Point(264, 47);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 12);
			this.label5.TabIndex = 19;
			this.label5.Text = "패스워드:";
			// 
			// txtLoginID
			// 
			this.txtLoginID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.txtLoginID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.txtLoginID.Location = new System.Drawing.Point(163, 44);
			this.txtLoginID.Name = "txtLoginID";
			this.txtLoginID.Size = new System.Drawing.Size(87, 21);
			this.txtLoginID.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label4.Location = new System.Drawing.Point(97, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 12);
			this.label4.TabIndex = 20;
			this.label4.Text = "로그인 ID:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(66, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(369, 12);
			this.label1.TabIndex = 21;
			this.label1.Text = "NETS-ⓘMan에 로그인하시려면 아이디와 패스워드를 입력하세요.";
			// 
			// chkRemPwd
			// 
			this.chkRemPwd.AutoSize = true;
			this.chkRemPwd.Checked = true;
			this.chkRemPwd.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkRemPwd.Location = new System.Drawing.Point(163, 77);
			this.chkRemPwd.Name = "chkRemPwd";
			this.chkRemPwd.Size = new System.Drawing.Size(157, 16);
			this.chkRemPwd.TabIndex = 7;
			this.chkRemPwd.Text = "로그인 ID/패스워드 기억";
			this.chkRemPwd.UseVisualStyleBackColor = true;
			// 
			// chkAutoStart
			// 
			this.chkAutoStart.AutoSize = true;
			this.chkAutoStart.Checked = true;
			this.chkAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoStart.Location = new System.Drawing.Point(163, 99);
			this.chkAutoStart.Name = "chkAutoStart";
			this.chkAutoStart.Size = new System.Drawing.Size(160, 16);
			this.chkAutoStart.TabIndex = 7;
			this.chkAutoStart.Text = "윈도우 시작 시 자동 실행";
			this.chkAutoStart.UseVisualStyleBackColor = true;
			this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(447, 163);
			this.Controls.Add(this.chkAutoStart);
			this.Controls.Add(this.chkRemPwd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtLoginID);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NETS-ⓘMan 로그인";
			this.Activated += new System.EventHandler(this.LoginForm_Activated);
			this.Load += new System.EventHandler(this.LoginForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtLoginID;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chkRemPwd;
		private System.Windows.Forms.CheckBox chkAutoStart;
	}
}