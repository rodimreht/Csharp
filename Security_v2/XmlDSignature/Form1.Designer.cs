namespace XmlDSignature
{
	partial class Form1
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
			this.lblResult = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOriginal = new System.Windows.Forms.TextBox();
			this.cmdVerify = new System.Windows.Forms.Button();
			this.cmdSign = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblResult
			// 
			this.lblResult.Font = new System.Drawing.Font("굴림체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblResult.Location = new System.Drawing.Point(317, 29);
			this.lblResult.Multiline = true;
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(272, 232);
			this.lblResult.TabIndex = 30;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(317, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 12);
			this.label2.TabIndex = 29;
			this.label2.Text = "서명된 XML";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 12);
			this.label1.TabIndex = 28;
			this.label1.Text = "원본 XML";
			// 
			// txtOriginal
			// 
			this.txtOriginal.Font = new System.Drawing.Font("굴림체", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.txtOriginal.Location = new System.Drawing.Point(16, 29);
			this.txtOriginal.Multiline = true;
			this.txtOriginal.Name = "txtOriginal";
			this.txtOriginal.Size = new System.Drawing.Size(272, 232);
			this.txtOriginal.TabIndex = 27;
			// 
			// cmdVerify
			// 
			this.cmdVerify.Location = new System.Drawing.Point(317, 280);
			this.cmdVerify.Name = "cmdVerify";
			this.cmdVerify.Size = new System.Drawing.Size(75, 23);
			this.cmdVerify.TabIndex = 26;
			this.cmdVerify.Text = "검증";
			// 
			// cmdSign
			// 
			this.cmdSign.Location = new System.Drawing.Point(213, 280);
			this.cmdSign.Name = "cmdSign";
			this.cmdSign.Size = new System.Drawing.Size(75, 23);
			this.cmdSign.TabIndex = 25;
			this.cmdSign.Text = "서명";
			this.cmdSign.Click += new System.EventHandler(this.cmdSign_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(611, 320);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOriginal);
			this.Controls.Add(this.cmdVerify);
			this.Controls.Add(this.cmdSign);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox lblResult;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOriginal;
		private System.Windows.Forms.Button cmdVerify;
		private System.Windows.Forms.Button cmdSign;
	}
}

