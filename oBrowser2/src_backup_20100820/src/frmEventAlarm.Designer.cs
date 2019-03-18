namespace oBrowser2
{
	partial class frmEventAlarm
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cboDetail = new System.Windows.Forms.ComboBox();
			this.cboFleetMove = new System.Windows.Forms.ComboBox();
			this.cboExpedition = new System.Windows.Forms.ComboBox();
			this.alarmList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.cmdDelete = new System.Windows.Forms.Button();
			this.optType2 = new System.Windows.Forms.RadioButton();
			this.optType1 = new System.Windows.Forms.RadioButton();
			this.cboPlanet = new System.Windows.Forms.ComboBox();
			this.cboGalaxy = new System.Windows.Forms.ComboBox();
			this.cboContent = new System.Windows.Forms.ComboBox();
			this.chkDaily = new System.Windows.Forms.CheckBox();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.cmdUpdate = new System.Windows.Forms.Button();
			this.cmdRegister = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.minUpdown = new System.Windows.Forms.NumericUpDown();
			this.hourUpdown = new System.Windows.Forms.NumericUpDown();
			this.txtContent = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.minUpdown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hourUpdown)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cboDetail);
			this.groupBox1.Controls.Add(this.cboFleetMove);
			this.groupBox1.Controls.Add(this.cboExpedition);
			this.groupBox1.Controls.Add(this.alarmList);
			this.groupBox1.Controls.Add(this.cmdDelete);
			this.groupBox1.Controls.Add(this.optType2);
			this.groupBox1.Controls.Add(this.optType1);
			this.groupBox1.Controls.Add(this.cboPlanet);
			this.groupBox1.Controls.Add(this.cboGalaxy);
			this.groupBox1.Controls.Add(this.cboContent);
			this.groupBox1.Controls.Add(this.chkDaily);
			this.groupBox1.Controls.Add(this.dateTimePicker1);
			this.groupBox1.Controls.Add(this.cmdUpdate);
			this.groupBox1.Controls.Add(this.cmdRegister);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.minUpdown);
			this.groupBox1.Controls.Add(this.hourUpdown);
			this.groupBox1.Controls.Add(this.txtContent);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(406, 355);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "                                      ";
			// 
			// cboDetail
			// 
			this.cboDetail.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboDetail.FormattingEnabled = true;
			this.cboDetail.Location = new System.Drawing.Point(288, 294);
			this.cboDetail.Name = "cboDetail";
			this.cboDetail.Size = new System.Drawing.Size(108, 20);
			this.cboDetail.TabIndex = 11;
			// 
			// cboFleetMove
			// 
			this.cboFleetMove.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFleetMove.FormattingEnabled = true;
			this.cboFleetMove.Items.AddRange(new object[] {
            "한 번만 보냄",
            "두 번 보냄",
            "세 번 보냄",
            "네 번 보냄",
            "다섯 번 보냄",
            "여섯 번 보냄"});
			this.cboFleetMove.Location = new System.Drawing.Point(204, 294);
			this.cboFleetMove.Name = "cboFleetMove";
			this.cboFleetMove.Size = new System.Drawing.Size(120, 20);
			this.cboFleetMove.TabIndex = 10;
			// 
			// cboExpedition
			// 
			this.cboExpedition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboExpedition.FormattingEnabled = true;
			this.cboExpedition.Items.AddRange(new object[] {
            "한 번만 보냄",
            "두 번 보냄",
            "세 번 보냄",
            "네 번 보냄"});
			this.cboExpedition.Location = new System.Drawing.Point(204, 294);
			this.cboExpedition.Name = "cboExpedition";
			this.cboExpedition.Size = new System.Drawing.Size(120, 20);
			this.cboExpedition.TabIndex = 10;
			// 
			// alarmList
			// 
			this.alarmList.CheckBoxes = true;
			this.alarmList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.alarmList.FullRowSelect = true;
			this.alarmList.GridLines = true;
			this.alarmList.Location = new System.Drawing.Point(11, 27);
			this.alarmList.MultiSelect = false;
			this.alarmList.Name = "alarmList";
			this.alarmList.Size = new System.Drawing.Size(385, 194);
			this.alarmList.TabIndex = 3;
			this.alarmList.UseCompatibleStateImageBehavior = false;
			this.alarmList.View = System.Windows.Forms.View.Details;
			this.alarmList.SelectedIndexChanged += new System.EventHandler(this.alarmList_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "설정시각";
			this.columnHeader1.Width = 140;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "내용";
			this.columnHeader2.Width = 240;
			// 
			// cmdDelete
			// 
			this.cmdDelete.Location = new System.Drawing.Point(340, 219);
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.Size = new System.Drawing.Size(57, 25);
			this.cmdDelete.TabIndex = 4;
			this.cmdDelete.Text = "삭 제";
			this.cmdDelete.UseVisualStyleBackColor = true;
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			// 
			// optType2
			// 
			this.optType2.AutoSize = true;
			this.optType2.Location = new System.Drawing.Point(115, 0);
			this.optType2.Name = "optType2";
			this.optType2.Size = new System.Drawing.Size(47, 16);
			this.optType2.TabIndex = 2;
			this.optType2.Text = "예약";
			this.optType2.UseVisualStyleBackColor = true;
			this.optType2.CheckedChanged += new System.EventHandler(this.optType2_CheckedChanged);
			// 
			// optType1
			// 
			this.optType1.AutoSize = true;
			this.optType1.Checked = true;
			this.optType1.Location = new System.Drawing.Point(16, 0);
			this.optType1.Name = "optType1";
			this.optType1.Size = new System.Drawing.Size(87, 16);
			this.optType1.TabIndex = 1;
			this.optType1.TabStop = true;
			this.optType1.Text = "이벤트 알림";
			this.optType1.UseVisualStyleBackColor = true;
			this.optType1.CheckedChanged += new System.EventHandler(this.optType1_CheckedChanged);
			// 
			// cboPlanet
			// 
			this.cboPlanet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPlanet.FormattingEnabled = true;
			this.cboPlanet.Location = new System.Drawing.Point(204, 294);
			this.cboPlanet.Name = "cboPlanet";
			this.cboPlanet.Size = new System.Drawing.Size(73, 20);
			this.cboPlanet.TabIndex = 10;
			// 
			// cboGalaxy
			// 
			this.cboGalaxy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboGalaxy.FormattingEnabled = true;
			this.cboGalaxy.Items.AddRange(new object[] {
            "모든 은하",
            "1은하",
            "2은하",
            "3은하",
            "4은하",
            "5은하",
            "6은하",
            "7은하",
            "8은하",
            "9은하"});
			this.cboGalaxy.Location = new System.Drawing.Point(204, 294);
			this.cboGalaxy.Name = "cboGalaxy";
			this.cboGalaxy.Size = new System.Drawing.Size(89, 20);
			this.cboGalaxy.TabIndex = 10;
			// 
			// cboContent
			// 
			this.cboContent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboContent.FormattingEnabled = true;
			this.cboContent.Items.AddRange(new object[] {
            "원정함대 출발",
            "자원 모으기",
            "플릿 세이빙",
            "함대 기동",
            "건물-건설",
            "건물-취소",
            "건물-파괴",
            "연구-연구",
            "연구-취소"});
			this.cboContent.Location = new System.Drawing.Point(93, 294);
			this.cboContent.Name = "cboContent";
			this.cboContent.Size = new System.Drawing.Size(105, 20);
			this.cboContent.TabIndex = 9;
			this.cboContent.SelectedIndexChanged += new System.EventHandler(this.cboContent_SelectedIndexChanged);
			// 
			// chkDaily
			// 
			this.chkDaily.AutoSize = true;
			this.chkDaily.Location = new System.Drawing.Point(94, 245);
			this.chkDaily.Name = "chkDaily";
			this.chkDaily.Size = new System.Drawing.Size(104, 16);
			this.chkDaily.TabIndex = 5;
			this.chkDaily.Text = "매일 같은 시각";
			this.chkDaily.UseVisualStyleBackColor = true;
			this.chkDaily.CheckedChanged += new System.EventHandler(this.chkDaily_CheckedChanged);
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.Checked = false;
			this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePicker1.Location = new System.Drawing.Point(94, 267);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(87, 21);
			this.dateTimePicker1.TabIndex = 6;
			// 
			// cmdUpdate
			// 
			this.cmdUpdate.Location = new System.Drawing.Point(339, 321);
			this.cmdUpdate.Name = "cmdUpdate";
			this.cmdUpdate.Size = new System.Drawing.Size(57, 25);
			this.cmdUpdate.TabIndex = 13;
			this.cmdUpdate.Text = "수 정";
			this.cmdUpdate.UseVisualStyleBackColor = true;
			this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
			// 
			// cmdRegister
			// 
			this.cmdRegister.Location = new System.Drawing.Point(276, 321);
			this.cmdRegister.Name = "cmdRegister";
			this.cmdRegister.Size = new System.Drawing.Size(57, 25);
			this.cmdRegister.TabIndex = 12;
			this.cmdRegister.Text = "등 록";
			this.cmdRegister.UseVisualStyleBackColor = true;
			this.cmdRegister.Click += new System.EventHandler(this.cmdRegister_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(292, 269);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "분";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(227, 269);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 12);
			this.label2.TabIndex = 9;
			this.label2.Text = "시";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("굴림체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label4.Location = new System.Drawing.Point(29, 297);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(59, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "내    용:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("굴림체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.Location = new System.Drawing.Point(29, 269);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 12);
			this.label1.TabIndex = 11;
			this.label1.Text = "설정시각:";
			// 
			// minUpdown
			// 
			this.minUpdown.Location = new System.Drawing.Point(252, 267);
			this.minUpdown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.minUpdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.minUpdown.Name = "minUpdown";
			this.minUpdown.Size = new System.Drawing.Size(38, 21);
			this.minUpdown.TabIndex = 8;
			this.minUpdown.Enter += new System.EventHandler(this.minUpdown_Enter);
			this.minUpdown.ValueChanged += new System.EventHandler(this.minUpdown_ValueChanged);
			// 
			// hourUpdown
			// 
			this.hourUpdown.Location = new System.Drawing.Point(187, 267);
			this.hourUpdown.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.hourUpdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.hourUpdown.Name = "hourUpdown";
			this.hourUpdown.Size = new System.Drawing.Size(38, 21);
			this.hourUpdown.TabIndex = 7;
			this.hourUpdown.Enter += new System.EventHandler(this.hourUpdown_Enter);
			this.hourUpdown.ValueChanged += new System.EventHandler(this.hourUpdown_ValueChanged);
			// 
			// txtContent
			// 
			this.txtContent.Location = new System.Drawing.Point(94, 294);
			this.txtContent.Name = "txtContent";
			this.txtContent.Size = new System.Drawing.Size(247, 21);
			this.txtContent.TabIndex = 9;
			// 
			// frmEventAlarm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(430, 377);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmEventAlarm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "이벤트 알림 및 예약 설정";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEventAlarm_FormClosing);
			this.Load += new System.EventHandler(this.frmEventAlarm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.minUpdown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hourUpdown)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button cmdUpdate;
		private System.Windows.Forms.Button cmdRegister;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown minUpdown;
		private System.Windows.Forms.NumericUpDown hourUpdown;
		private System.Windows.Forms.TextBox txtContent;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private System.Windows.Forms.RadioButton optType1;
		private System.Windows.Forms.CheckBox chkDaily;
		private System.Windows.Forms.RadioButton optType2;
		private System.Windows.Forms.ComboBox cboContent;
		private System.Windows.Forms.ComboBox cboGalaxy;
		private System.Windows.Forms.ComboBox cboPlanet;
		private System.Windows.Forms.ListView alarmList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button cmdDelete;
		private System.Windows.Forms.ComboBox cboExpedition;
		private System.Windows.Forms.ComboBox cboFleetMove;
		private System.Windows.Forms.ComboBox cboDetail;
	}
}