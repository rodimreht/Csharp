namespace HeroMiner
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
			this.cboKey11 = new System.Windows.Forms.ComboBox();
			this.cboKey12 = new System.Windows.Forms.ComboBox();
			this.txtTime1 = new System.Windows.Forms.TextBox();
			this.txtTime2 = new System.Windows.Forms.TextBox();
			this.txtTime3 = new System.Windows.Forms.TextBox();
			this.txtTime4 = new System.Windows.Forms.TextBox();
			this.txtTime5 = new System.Windows.Forms.TextBox();
			this.cmdStart = new System.Windows.Forms.Button();
			this.cmdEnd = new System.Windows.Forms.Button();
			this.txtCustKey1 = new System.Windows.Forms.TextBox();
			this.txtCustKey2 = new System.Windows.Forms.TextBox();
			this.txtCustTime = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cboKey21 = new System.Windows.Forms.ComboBox();
			this.cboKey22 = new System.Windows.Forms.ComboBox();
			this.cboKey31 = new System.Windows.Forms.ComboBox();
			this.cboKey32 = new System.Windows.Forms.ComboBox();
			this.cboKey41 = new System.Windows.Forms.ComboBox();
			this.cboKey42 = new System.Windows.Forms.ComboBox();
			this.cboKey51 = new System.Windows.Forms.ComboBox();
			this.cboKey52 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.lblStatus = new System.Windows.Forms.Label();
			this.cboKey61 = new System.Windows.Forms.ComboBox();
			this.cboKey62 = new System.Windows.Forms.ComboBox();
			this.txtTime6 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.txtCustKey3 = new System.Windows.Forms.TextBox();
			this.txtCustKey4 = new System.Windows.Forms.TextBox();
			this.txtCustTime2 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtPID = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// cboKey11
			// 
			this.cboKey11.FormattingEnabled = true;
			this.cboKey11.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "LShift",
            "RShift",
            "LCtrl",
            "RCtrl"});
			this.cboKey11.Location = new System.Drawing.Point(24, 32);
			this.cboKey11.Name = "cboKey11";
			this.cboKey11.Size = new System.Drawing.Size(50, 20);
			this.cboKey11.TabIndex = 2;
			// 
			// cboKey12
			// 
			this.cboKey12.FormattingEnabled = true;
			this.cboKey12.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "-",
            "="});
			this.cboKey12.Location = new System.Drawing.Point(86, 32);
			this.cboKey12.Name = "cboKey12";
			this.cboKey12.Size = new System.Drawing.Size(50, 20);
			this.cboKey12.TabIndex = 3;
			// 
			// txtTime1
			// 
			this.txtTime1.Location = new System.Drawing.Point(148, 31);
			this.txtTime1.Name = "txtTime1";
			this.txtTime1.Size = new System.Drawing.Size(50, 21);
			this.txtTime1.TabIndex = 4;
			// 
			// txtTime2
			// 
			this.txtTime2.Location = new System.Drawing.Point(148, 57);
			this.txtTime2.Name = "txtTime2";
			this.txtTime2.Size = new System.Drawing.Size(50, 21);
			this.txtTime2.TabIndex = 7;
			// 
			// txtTime3
			// 
			this.txtTime3.Location = new System.Drawing.Point(148, 83);
			this.txtTime3.Name = "txtTime3";
			this.txtTime3.Size = new System.Drawing.Size(50, 21);
			this.txtTime3.TabIndex = 10;
			// 
			// txtTime4
			// 
			this.txtTime4.Location = new System.Drawing.Point(361, 31);
			this.txtTime4.Name = "txtTime4";
			this.txtTime4.Size = new System.Drawing.Size(50, 21);
			this.txtTime4.TabIndex = 13;
			// 
			// txtTime5
			// 
			this.txtTime5.Location = new System.Drawing.Point(361, 57);
			this.txtTime5.Name = "txtTime5";
			this.txtTime5.Size = new System.Drawing.Size(50, 21);
			this.txtTime5.TabIndex = 16;
			// 
			// cmdStart
			// 
			this.cmdStart.Location = new System.Drawing.Point(251, 171);
			this.cmdStart.Name = "cmdStart";
			this.cmdStart.Size = new System.Drawing.Size(75, 38);
			this.cmdStart.TabIndex = 0;
			this.cmdStart.Text = "START (INSERT)";
			this.cmdStart.UseVisualStyleBackColor = true;
			this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
			// 
			// cmdEnd
			// 
			this.cmdEnd.Enabled = false;
			this.cmdEnd.Location = new System.Drawing.Point(350, 171);
			this.cmdEnd.Name = "cmdEnd";
			this.cmdEnd.Size = new System.Drawing.Size(75, 38);
			this.cmdEnd.TabIndex = 1;
			this.cmdEnd.Text = "END (DELETE)";
			this.cmdEnd.UseVisualStyleBackColor = true;
			this.cmdEnd.Click += new System.EventHandler(this.cmdEnd_Click);
			// 
			// txtCustKey1
			// 
			this.txtCustKey1.Location = new System.Drawing.Point(24, 139);
			this.txtCustKey1.Name = "txtCustKey1";
			this.txtCustKey1.Size = new System.Drawing.Size(50, 21);
			this.txtCustKey1.TabIndex = 17;
			// 
			// txtCustKey2
			// 
			this.txtCustKey2.Location = new System.Drawing.Point(86, 139);
			this.txtCustKey2.Name = "txtCustKey2";
			this.txtCustKey2.Size = new System.Drawing.Size(50, 21);
			this.txtCustKey2.TabIndex = 18;
			// 
			// txtCustTime
			// 
			this.txtCustTime.Location = new System.Drawing.Point(148, 139);
			this.txtCustTime.Name = "txtCustTime";
			this.txtCustTime.Size = new System.Drawing.Size(50, 21);
			this.txtCustTime.TabIndex = 19;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 124);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "Custom";
			// 
			// cboKey21
			// 
			this.cboKey21.FormattingEnabled = true;
			this.cboKey21.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "LShift",
            "RShift",
            "LCtrl",
            "RCtrl"});
			this.cboKey21.Location = new System.Drawing.Point(24, 57);
			this.cboKey21.Name = "cboKey21";
			this.cboKey21.Size = new System.Drawing.Size(50, 20);
			this.cboKey21.TabIndex = 5;
			// 
			// cboKey22
			// 
			this.cboKey22.FormattingEnabled = true;
			this.cboKey22.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "-",
            "="});
			this.cboKey22.Location = new System.Drawing.Point(86, 57);
			this.cboKey22.Name = "cboKey22";
			this.cboKey22.Size = new System.Drawing.Size(50, 20);
			this.cboKey22.TabIndex = 6;
			// 
			// cboKey31
			// 
			this.cboKey31.FormattingEnabled = true;
			this.cboKey31.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "LShift",
            "RShift",
            "LCtrl",
            "RCtrl"});
			this.cboKey31.Location = new System.Drawing.Point(24, 83);
			this.cboKey31.Name = "cboKey31";
			this.cboKey31.Size = new System.Drawing.Size(50, 20);
			this.cboKey31.TabIndex = 8;
			// 
			// cboKey32
			// 
			this.cboKey32.FormattingEnabled = true;
			this.cboKey32.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "-",
            "="});
			this.cboKey32.Location = new System.Drawing.Point(86, 83);
			this.cboKey32.Name = "cboKey32";
			this.cboKey32.Size = new System.Drawing.Size(50, 20);
			this.cboKey32.TabIndex = 9;
			// 
			// cboKey41
			// 
			this.cboKey41.FormattingEnabled = true;
			this.cboKey41.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "LShift",
            "RShift",
            "LCtrl",
            "RCtrl"});
			this.cboKey41.Location = new System.Drawing.Point(237, 31);
			this.cboKey41.Name = "cboKey41";
			this.cboKey41.Size = new System.Drawing.Size(50, 20);
			this.cboKey41.TabIndex = 11;
			// 
			// cboKey42
			// 
			this.cboKey42.FormattingEnabled = true;
			this.cboKey42.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "-",
            "="});
			this.cboKey42.Location = new System.Drawing.Point(299, 31);
			this.cboKey42.Name = "cboKey42";
			this.cboKey42.Size = new System.Drawing.Size(50, 20);
			this.cboKey42.TabIndex = 12;
			// 
			// cboKey51
			// 
			this.cboKey51.FormattingEnabled = true;
			this.cboKey51.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "LShift",
            "RShift",
            "LCtrl",
            "RCtrl"});
			this.cboKey51.Location = new System.Drawing.Point(237, 57);
			this.cboKey51.Name = "cboKey51";
			this.cboKey51.Size = new System.Drawing.Size(50, 20);
			this.cboKey51.TabIndex = 14;
			// 
			// cboKey52
			// 
			this.cboKey52.FormattingEnabled = true;
			this.cboKey52.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "-",
            "="});
			this.cboKey52.Location = new System.Drawing.Point(299, 57);
			this.cboKey52.Name = "cboKey52";
			this.cboKey52.Size = new System.Drawing.Size(50, 20);
			this.cboKey52.TabIndex = 15;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "Key1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(84, 17);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(33, 12);
			this.label3.TabIndex = 6;
			this.label3.Text = "Key2";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(146, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 12);
			this.label4.TabIndex = 6;
			this.label4.Text = "Time(sec.)";
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.ForeColor = System.Drawing.Color.Teal;
			this.lblStatus.Location = new System.Drawing.Point(12, 212);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(412, 23);
			this.lblStatus.TabIndex = 20;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cboKey61
			// 
			this.cboKey61.FormattingEnabled = true;
			this.cboKey61.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "LShift",
            "RShift",
            "LCtrl",
            "RCtrl"});
			this.cboKey61.Location = new System.Drawing.Point(237, 82);
			this.cboKey61.Name = "cboKey61";
			this.cboKey61.Size = new System.Drawing.Size(50, 20);
			this.cboKey61.TabIndex = 14;
			// 
			// cboKey62
			// 
			this.cboKey62.FormattingEnabled = true;
			this.cboKey62.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "-",
            "="});
			this.cboKey62.Location = new System.Drawing.Point(299, 82);
			this.cboKey62.Name = "cboKey62";
			this.cboKey62.Size = new System.Drawing.Size(50, 20);
			this.cboKey62.TabIndex = 15;
			// 
			// txtTime6
			// 
			this.txtTime6.Location = new System.Drawing.Point(361, 82);
			this.txtTime6.Name = "txtTime6";
			this.txtTime6.Size = new System.Drawing.Size(50, 21);
			this.txtTime6.TabIndex = 16;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(235, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 12);
			this.label5.TabIndex = 5;
			this.label5.Text = "Key1";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(297, 17);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(33, 12);
			this.label6.TabIndex = 6;
			this.label6.Text = "Key2";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(359, 17);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(69, 12);
			this.label7.TabIndex = 6;
			this.label7.Text = "Time(sec.)";
			// 
			// txtCustKey3
			// 
			this.txtCustKey3.Location = new System.Drawing.Point(237, 139);
			this.txtCustKey3.Name = "txtCustKey3";
			this.txtCustKey3.Size = new System.Drawing.Size(50, 21);
			this.txtCustKey3.TabIndex = 17;
			// 
			// txtCustKey4
			// 
			this.txtCustKey4.Location = new System.Drawing.Point(299, 139);
			this.txtCustKey4.Name = "txtCustKey4";
			this.txtCustKey4.Size = new System.Drawing.Size(50, 21);
			this.txtCustKey4.TabIndex = 18;
			// 
			// txtCustTime2
			// 
			this.txtCustTime2.Location = new System.Drawing.Point(361, 139);
			this.txtCustTime2.Name = "txtCustTime2";
			this.txtCustTime2.Size = new System.Drawing.Size(50, 21);
			this.txtCustTime2.TabIndex = 19;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(22, 184);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(30, 12);
			this.label8.TabIndex = 4;
			this.label8.Text = "PID=";
			// 
			// txtPID
			// 
			this.txtPID.Location = new System.Drawing.Point(58, 181);
			this.txtPID.Name = "txtPID";
			this.txtPID.Size = new System.Drawing.Size(50, 21);
			this.txtPID.TabIndex = 19;
			this.txtPID.Text = "0";
			this.txtPID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(436, 246);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmdEnd);
			this.Controls.Add(this.cmdStart);
			this.Controls.Add(this.txtCustTime2);
			this.Controls.Add(this.txtPID);
			this.Controls.Add(this.txtCustTime);
			this.Controls.Add(this.txtCustKey4);
			this.Controls.Add(this.txtCustKey3);
			this.Controls.Add(this.txtCustKey2);
			this.Controls.Add(this.txtCustKey1);
			this.Controls.Add(this.txtTime6);
			this.Controls.Add(this.txtTime5);
			this.Controls.Add(this.txtTime4);
			this.Controls.Add(this.txtTime3);
			this.Controls.Add(this.txtTime2);
			this.Controls.Add(this.cboKey62);
			this.Controls.Add(this.txtTime1);
			this.Controls.Add(this.cboKey61);
			this.Controls.Add(this.cboKey52);
			this.Controls.Add(this.cboKey51);
			this.Controls.Add(this.cboKey42);
			this.Controls.Add(this.cboKey41);
			this.Controls.Add(this.cboKey32);
			this.Controls.Add(this.cboKey31);
			this.Controls.Add(this.cboKey22);
			this.Controls.Add(this.cboKey21);
			this.Controls.Add(this.cboKey12);
			this.Controls.Add(this.cboKey11);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "HeroMiner";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboKey11;
		private System.Windows.Forms.ComboBox cboKey12;
		private System.Windows.Forms.TextBox txtTime1;
		private System.Windows.Forms.TextBox txtTime2;
		private System.Windows.Forms.TextBox txtTime3;
		private System.Windows.Forms.TextBox txtTime4;
		private System.Windows.Forms.TextBox txtTime5;
		private System.Windows.Forms.Button cmdStart;
		private System.Windows.Forms.Button cmdEnd;
		private System.Windows.Forms.TextBox txtCustKey1;
		private System.Windows.Forms.TextBox txtCustKey2;
		private System.Windows.Forms.TextBox txtCustTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboKey21;
		private System.Windows.Forms.ComboBox cboKey22;
		private System.Windows.Forms.ComboBox cboKey31;
		private System.Windows.Forms.ComboBox cboKey32;
		private System.Windows.Forms.ComboBox cboKey41;
		private System.Windows.Forms.ComboBox cboKey42;
		private System.Windows.Forms.ComboBox cboKey51;
		private System.Windows.Forms.ComboBox cboKey52;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.ComboBox cboKey61;
		private System.Windows.Forms.ComboBox cboKey62;
		private System.Windows.Forms.TextBox txtTime6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtCustKey3;
		private System.Windows.Forms.TextBox txtCustKey4;
		private System.Windows.Forms.TextBox txtCustTime2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtPID;
	}
}

