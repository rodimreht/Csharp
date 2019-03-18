﻿namespace oBrowser2
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
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.cboUniverse = new System.Windows.Forms.ComboBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtLoginID = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.selOption1 = new System.Windows.Forms.RadioButton();
			this.txtSessionID = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.selOption2 = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cancelButton.Location = new System.Drawing.Point(348, 240);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 21);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.okButton.Location = new System.Drawing.Point(254, 240);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(87, 21);
			this.okButton.TabIndex = 7;
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
			// label6
			// 
			this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label6.Location = new System.Drawing.Point(74, 18);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(320, 19);
			this.label6.TabIndex = 16;
			this.label6.Text = "접속할 O-Game 서버를 선택하세요.";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label7.Location = new System.Drawing.Point(83, 37);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 12);
			this.label7.TabIndex = 17;
			this.label7.Text = "은하계:";
			// 
			// cboUniverse
			// 
			this.cboUniverse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboUniverse.FormattingEnabled = true;
			this.cboUniverse.Location = new System.Drawing.Point(134, 34);
			this.cboUniverse.Name = "cboUniverse";
			this.cboUniverse.Size = new System.Drawing.Size(121, 20);
			this.cboUniverse.TabIndex = 1;
			// 
			// txtPassword
			// 
			this.txtPassword.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.txtPassword.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.txtPassword.Enabled = false;
			this.txtPassword.Location = new System.Drawing.Point(334, 184);
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
			this.label5.Location = new System.Drawing.Point(268, 187);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 12);
			this.label5.TabIndex = 19;
			this.label5.Text = "패스워드:";
			// 
			// txtLoginID
			// 
			this.txtLoginID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.txtLoginID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.txtLoginID.Enabled = false;
			this.txtLoginID.Location = new System.Drawing.Point(167, 184);
			this.txtLoginID.Name = "txtLoginID";
			this.txtLoginID.Size = new System.Drawing.Size(87, 21);
			this.txtLoginID.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label4.Location = new System.Drawing.Point(101, 187);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 12);
			this.label4.TabIndex = 20;
			this.label4.Text = "로그인 ID:";
			// 
			// selOption1
			// 
			this.selOption1.Checked = true;
			this.selOption1.Location = new System.Drawing.Point(76, 74);
			this.selOption1.Name = "selOption1";
			this.selOption1.Size = new System.Drawing.Size(359, 31);
			this.selOption1.TabIndex = 2;
			this.selOption1.TabStop = true;
			this.selOption1.Text = "다른 브라우저에서 이미 로그인했으면 해당 로그인 세션ID를 찾아서 입력하세요.";
			this.selOption1.UseVisualStyleBackColor = true;
			this.selOption1.CheckedChanged += new System.EventHandler(this.selOption1_CheckedChanged);
			// 
			// txtSessionID
			// 
			this.txtSessionID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.txtSessionID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.txtSessionID.Location = new System.Drawing.Point(156, 105);
			this.txtSessionID.Name = "txtSessionID";
			this.txtSessionID.Size = new System.Drawing.Size(102, 21);
			this.txtSessionID.TabIndex = 4;
			this.txtSessionID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSessionID_KeyUp);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label2.Location = new System.Drawing.Point(101, 108);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 12);
			this.label2.TabIndex = 24;
			this.label2.Text = "세션 ID:";
			// 
			// selOption2
			// 
			this.selOption2.Location = new System.Drawing.Point(76, 147);
			this.selOption2.Name = "selOption2";
			this.selOption2.Size = new System.Drawing.Size(345, 31);
			this.selOption2.TabIndex = 3;
			this.selOption2.Text = "여기서 바로 로그인하시려면 로그인 ID와 패스워드를 입력하세요.";
			this.selOption2.UseVisualStyleBackColor = true;
			this.selOption2.CheckedChanged += new System.EventHandler(this.selOption2_CheckedChanged);
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(447, 269);
			this.Controls.Add(this.selOption2);
			this.Controls.Add(this.selOption1);
			this.Controls.Add(this.txtSessionID);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtLoginID);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cboUniverse);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
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
			this.Text = "O-Game 로그인";
			this.Load += new System.EventHandler(this.LoginForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cboUniverse;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtLoginID;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton selOption1;
		private System.Windows.Forms.TextBox txtSessionID;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton selOption2;
	}
}