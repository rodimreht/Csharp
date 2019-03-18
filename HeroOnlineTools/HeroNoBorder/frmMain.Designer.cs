namespace HeroNoBorder
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
			this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button3 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPID = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtX = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtY = new System.Windows.Forms.TextBox();
			this.button4 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(43, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(118, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "가장자리 없애기!";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(167, 12);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "원래대로...";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// shapeContainer1
			// 
			this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
			this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.shapeContainer1.Name = "shapeContainer1";
			this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
			this.shapeContainer1.Size = new System.Drawing.Size(292, 151);
			this.shapeContainer1.TabIndex = 2;
			this.shapeContainer1.TabStop = false;
			// 
			// lineShape1
			// 
			this.lineShape1.BorderColor = System.Drawing.SystemColors.ButtonShadow;
			this.lineShape1.Name = "lineShape1";
			this.lineShape1.X1 = 15;
			this.lineShape1.X2 = 280;
			this.lineShape1.Y1 = 51;
			this.lineShape1.Y2 = 51;
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "640x480",
            "800x600",
            "1024x768",
            "1152x864",
            "1280x960",
            "1280x1024",
            "1360x768",
            "1366x768",
            "1440x1050",
            "1680x1080",
            "1600x1200",
            "1920x1080"});
			this.comboBox1.Location = new System.Drawing.Point(88, 90);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 20);
			this.comboBox1.TabIndex = 3;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(215, 88);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(47, 23);
			this.button3.TabIndex = 1;
			this.button3.Text = "변경";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(41, 93);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "창크기:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(58, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(28, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "PID:";
			// 
			// txtPID
			// 
			this.txtPID.Location = new System.Drawing.Point(88, 68);
			this.txtPID.Name = "txtPID";
			this.txtPID.Size = new System.Drawing.Size(121, 21);
			this.txtPID.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(53, 115);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 12);
			this.label3.TabIndex = 4;
			this.label3.Text = "위치: x=";
			// 
			// txtX
			// 
			this.txtX.Location = new System.Drawing.Point(103, 112);
			this.txtX.Name = "txtX";
			this.txtX.Size = new System.Drawing.Size(42, 21);
			this.txtX.TabIndex = 5;
			this.txtX.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(149, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(18, 12);
			this.label4.TabIndex = 4;
			this.label4.Text = "y=";
			// 
			// txtY
			// 
			this.txtY.Location = new System.Drawing.Point(167, 112);
			this.txtY.Name = "txtY";
			this.txtY.Size = new System.Drawing.Size(42, 21);
			this.txtY.TabIndex = 5;
			this.txtY.Text = "0";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(215, 110);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(47, 23);
			this.button4.TabIndex = 1;
			this.button4.Text = "변경";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 151);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.txtY);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtX);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtPID);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.shapeContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "HeroNoBorder";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
		private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtPID;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtX;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtY;
		private System.Windows.Forms.Button button4;
	}
}

