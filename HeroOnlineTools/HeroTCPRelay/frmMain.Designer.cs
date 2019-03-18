namespace HeroTCPRelay
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
			this.svrCounter = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.cmdGo = new System.Windows.Forms.Button();
			this.lblPacket = new System.Windows.Forms.Label();
			this.lblPacketR = new System.Windows.Forms.Label();
			this.attackCounter = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.svrCounter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.attackCounter)).BeginInit();
			this.SuspendLayout();
			// 
			// svrCounter
			// 
			this.svrCounter.Location = new System.Drawing.Point(68, 12);
			this.svrCounter.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.svrCounter.Name = "svrCounter";
			this.svrCounter.Size = new System.Drawing.Size(44, 21);
			this.svrCounter.TabIndex = 1;
			this.svrCounter.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "서버수:";
			// 
			// cmdGo
			// 
			this.cmdGo.Location = new System.Drawing.Point(28, 47);
			this.cmdGo.Name = "cmdGo";
			this.cmdGo.Size = new System.Drawing.Size(75, 23);
			this.cmdGo.TabIndex = 0;
			this.cmdGo.Text = "시작";
			this.cmdGo.UseVisualStyleBackColor = true;
			this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
			// 
			// lblPacket
			// 
			this.lblPacket.BackColor = System.Drawing.SystemColors.ControlText;
			this.lblPacket.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPacket.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblPacket.ForeColor = System.Drawing.Color.White;
			this.lblPacket.Location = new System.Drawing.Point(124, 11);
			this.lblPacket.Name = "lblPacket";
			this.lblPacket.Size = new System.Drawing.Size(412, 62);
			this.lblPacket.TabIndex = 5;
			this.lblPacket.Click += new System.EventHandler(this.lblPacket_Click);
			// 
			// lblPacketR
			// 
			this.lblPacketR.BackColor = System.Drawing.SystemColors.ControlText;
			this.lblPacketR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPacketR.Font = new System.Drawing.Font("돋움체", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblPacketR.ForeColor = System.Drawing.Color.White;
			this.lblPacketR.Location = new System.Drawing.Point(124, 79);
			this.lblPacketR.Name = "lblPacketR";
			this.lblPacketR.Size = new System.Drawing.Size(412, 62);
			this.lblPacketR.TabIndex = 5;
			// 
			// attackCounter
			// 
			this.attackCounter.Location = new System.Drawing.Point(70, 115);
			this.attackCounter.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.attackCounter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.attackCounter.Name = "attackCounter";
			this.attackCounter.Size = new System.Drawing.Size(44, 21);
			this.attackCounter.TabIndex = 2;
			this.attackCounter.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 101);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "공격반복횟수:";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(550, 152);
			this.Controls.Add(this.lblPacketR);
			this.Controls.Add(this.lblPacket);
			this.Controls.Add(this.cmdGo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.attackCounter);
			this.Controls.Add(this.svrCounter);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "영웅 TCP 릴레이";
			this.Load += new System.EventHandler(this.frmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.svrCounter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.attackCounter)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown svrCounter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cmdGo;
		private System.Windows.Forms.Label lblPacket;
		private System.Windows.Forms.Label lblPacketR;
		private System.Windows.Forms.NumericUpDown attackCounter;
		private System.Windows.Forms.Label label2;
	}
}

