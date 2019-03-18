namespace NETS_iMan
{
  partial class AboutForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.okButton = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.licenseButton = new System.Windows.Forms.LinkLabel();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.label5 = new System.Windows.Forms.Label();
		this.lblVersion = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.lblDistVersion = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.groupBox1.SuspendLayout();
		this.SuspendLayout();
		// 
		// label1
		// 
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		// 
		// label2
		// 
		resources.ApplyResources(this.label2, "label2");
		this.label2.Name = "label2";
		// 
		// label3
		// 
		resources.ApplyResources(this.label3, "label3");
		this.label3.Name = "label3";
		// 
		// okButton
		// 
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.okButton, "okButton");
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		this.okButton.Click += new System.EventHandler(this.okButton_Click);
		// 
		// label4
		// 
		resources.ApplyResources(this.label4, "label4");
		this.label4.Name = "label4";
		// 
		// licenseButton
		// 
		resources.ApplyResources(this.licenseButton, "licenseButton");
		this.licenseButton.Name = "licenseButton";
		this.licenseButton.TabStop = true;
		this.licenseButton.Click += new System.EventHandler(this.licenseButton_Click);
		// 
		// groupBox1
		// 
		this.groupBox1.Controls.Add(this.label5);
		this.groupBox1.Controls.Add(this.label4);
		this.groupBox1.Controls.Add(this.licenseButton);
		resources.ApplyResources(this.groupBox1, "groupBox1");
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.TabStop = false;
		// 
		// label5
		// 
		resources.ApplyResources(this.label5, "label5");
		this.label5.Name = "label5";
		// 
		// lblVersion
		// 
		resources.ApplyResources(this.lblVersion, "lblVersion");
		this.lblVersion.Name = "lblVersion";
		// 
		// label6
		// 
		resources.ApplyResources(this.label6, "label6");
		this.label6.Name = "label6";
		// 
		// lblDistVersion
		// 
		resources.ApplyResources(this.lblDistVersion, "lblDistVersion");
		this.lblDistVersion.Name = "lblDistVersion";
		// 
		// label8
		// 
		resources.ApplyResources(this.label8, "label8");
		this.label8.Name = "label8";
		// 
		// AboutForm
		// 
		this.AcceptButton = this.okButton;
		resources.ApplyResources(this, "$this");
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.CancelButton = this.okButton;
		this.Controls.Add(this.label8);
		this.Controls.Add(this.lblDistVersion);
		this.Controls.Add(this.label6);
		this.Controls.Add(this.lblVersion);
		this.Controls.Add(this.groupBox1);
		this.Controls.Add(this.okButton);
		this.Controls.Add(this.label3);
		this.Controls.Add(this.label2);
		this.Controls.Add(this.label1);
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "AboutForm";
		this.Load += new System.EventHandler(this.AboutForm_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
	  private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.LinkLabel licenseButton;
	  private System.Windows.Forms.GroupBox groupBox1;
	  private System.Windows.Forms.Label lblVersion;
	  private System.Windows.Forms.Label label5;
	  private System.Windows.Forms.Label label6;
	  private System.Windows.Forms.Label lblDistVersion;
	  private System.Windows.Forms.Label label8;
  }
}