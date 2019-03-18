namespace oBrowser2
{
	partial class frmResCollect
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
			this.cboPlanet = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cboFleet1 = new System.Windows.Forms.ComboBox();
			this.cboFleet2 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cboMoveType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cboSpeed = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtRDeuterium = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cboPlanetType = new System.Windows.Forms.ComboBox();
			this.chkResEvent = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtRDeuterium)).BeginInit();
			this.SuspendLayout();
			// 
			// cboPlanet
			// 
			this.cboPlanet.FormattingEnabled = true;
			this.cboPlanet.Location = new System.Drawing.Point(112, 18);
			this.cboPlanet.Name = "cboPlanet";
			this.cboPlanet.Size = new System.Drawing.Size(107, 20);
			this.cboPlanet.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "도착 행성 좌표:";
			// 
			// cboFleet1
			// 
			this.cboFleet1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFleet1.FormattingEnabled = true;
			this.cboFleet1.Location = new System.Drawing.Point(110, 21);
			this.cboFleet1.Name = "cboFleet1";
			this.cboFleet1.Size = new System.Drawing.Size(107, 20);
			this.cboFleet1.TabIndex = 5;
			// 
			// cboFleet2
			// 
			this.cboFleet2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFleet2.FormattingEnabled = true;
			this.cboFleet2.Location = new System.Drawing.Point(110, 47);
			this.cboFleet2.Name = "cboFleet2";
			this.cboFleet2.Size = new System.Drawing.Size(107, 20);
			this.cboFleet2.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(40, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "1순위 함대:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(40, 50);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "2순위 함대:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cboFleet2);
			this.groupBox1.Controls.Add(this.cboFleet1);
			this.groupBox1.Location = new System.Drawing.Point(31, 100);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(265, 82);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "함대 구성 (최대 2종류)";
			// 
			// cboMoveType
			// 
			this.cboMoveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMoveType.FormattingEnabled = true;
			this.cboMoveType.Location = new System.Drawing.Point(112, 44);
			this.cboMoveType.Name = "cboMoveType";
			this.cboMoveType.Size = new System.Drawing.Size(107, 20);
			this.cboMoveType.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 12);
			this.label4.TabIndex = 5;
			this.label4.Text = "자원 운송 형태:";
			// 
			// cboSpeed
			// 
			this.cboSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSpeed.FormattingEnabled = true;
			this.cboSpeed.Location = new System.Drawing.Point(112, 70);
			this.cboSpeed.Name = "cboSpeed";
			this.cboSpeed.Size = new System.Drawing.Size(65, 20);
			this.cboSpeed.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(45, 73);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(61, 12);
			this.label5.TabIndex = 7;
			this.label5.Text = "비행 속도:";
			// 
			// txtRDeuterium
			// 
			this.txtRDeuterium.Location = new System.Drawing.Point(198, 188);
			this.txtRDeuterium.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
			this.txtRDeuterium.Name = "txtRDeuterium";
			this.txtRDeuterium.Size = new System.Drawing.Size(91, 21);
			this.txtRDeuterium.TabIndex = 7;
			this.txtRDeuterium.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(40, 193);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(157, 12);
			this.label9.TabIndex = 9;
			this.label9.Text = "이 행성에 남겨둘 듀테륨 량:";
			// 
			// cmdSave
			// 
			this.cmdSave.Location = new System.Drawing.Point(231, 247);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.Size = new System.Drawing.Size(87, 23);
			this.cmdSave.TabIndex = 9;
			this.cmdSave.Text = "저장 후 닫기";
			this.cmdSave.UseVisualStyleBackColor = true;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cboPlanetType
			// 
			this.cboPlanetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPlanetType.FormattingEnabled = true;
			this.cboPlanetType.Location = new System.Drawing.Point(225, 18);
			this.cboPlanetType.Name = "cboPlanetType";
			this.cboPlanetType.Size = new System.Drawing.Size(84, 20);
			this.cboPlanetType.TabIndex = 2;
			// 
			// chkResEvent
			// 
			this.chkResEvent.AutoSize = true;
			this.chkResEvent.Location = new System.Drawing.Point(46, 220);
			this.chkResEvent.Name = "chkResEvent";
			this.chkResEvent.Size = new System.Drawing.Size(236, 16);
			this.chkResEvent.TabIndex = 8;
			this.chkResEvent.Text = "\"자원 운송 완료\" 이벤트 알림 자동 등록";
			this.chkResEvent.UseVisualStyleBackColor = true;
			// 
			// frmResCollect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(330, 282);
			this.Controls.Add(this.chkResEvent);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.txtRDeuterium);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.cboPlanetType);
			this.Controls.Add(this.cboSpeed);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cboMoveType);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cboPlanet);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmResCollect";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "자원 모으기 설정";
			this.Load += new System.EventHandler(this.frmResCollect_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtRDeuterium)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboPlanet;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboFleet1;
		private System.Windows.Forms.ComboBox cboFleet2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cboMoveType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboSpeed;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown txtRDeuterium;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.ComboBox cboPlanetType;
		private System.Windows.Forms.CheckBox chkResEvent;
	}
}