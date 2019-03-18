namespace oBrowser2
{
	partial class frmFleetSaving
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
			this.cboMoveType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cboSpeed = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cboPlanetType = new System.Windows.Forms.ComboBox();
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
			// cmdSave
			// 
			this.cmdSave.Location = new System.Drawing.Point(231, 94);
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
			// frmFleetSaving
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(330, 129);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.cboPlanetType);
			this.Controls.Add(this.cboSpeed);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cboMoveType);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cboPlanet);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmFleetSaving";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "플릿 세이빙 설정";
			this.Load += new System.EventHandler(this.frmFleetSaving_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboPlanet;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboMoveType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboSpeed;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.ComboBox cboPlanetType;

	}
}