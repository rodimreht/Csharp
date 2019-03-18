namespace oBrowser2
{
  partial class OptionsForm
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
		this.okButton = new System.Windows.Forms.Button();
		this.cancelButton = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.applySummerTime = new System.Windows.Forms.CheckBox();
		this.txtSMSphoneNum = new System.Windows.Forms.TextBox();
		this.txtRateMax = new System.Windows.Forms.TextBox();
		this.txtRate = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.chkShowInLeftBottom = new System.Windows.Forms.CheckBox();
		this.cmdOpenFireFox = new System.Windows.Forms.Button();
		this.txtFireFoxDir = new System.Windows.Forms.TextBox();
		this.useFireFox = new System.Windows.Forms.CheckBox();
		this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.txtMailPassword = new System.Windows.Forms.TextBox();
		this.txtMailUserID = new System.Windows.Forms.TextBox();
		this.txtMailServer = new System.Windows.Forms.TextBox();
		this.txtMailAddress = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.SuspendLayout();
		// 
		// okButton
		// 
		resources.ApplyResources(this.okButton, "okButton");
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		this.okButton.Click += new System.EventHandler(this.okButton_Click);
		// 
		// cancelButton
		// 
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.UseVisualStyleBackColor = true;
		// 
		// groupBox1
		// 
		this.groupBox1.Controls.Add(this.applySummerTime);
		this.groupBox1.Controls.Add(this.txtSMSphoneNum);
		this.groupBox1.Controls.Add(this.txtRateMax);
		this.groupBox1.Controls.Add(this.txtRate);
		this.groupBox1.Controls.Add(this.label4);
		this.groupBox1.Controls.Add(this.label5);
		this.groupBox1.Controls.Add(this.label3);
		resources.ApplyResources(this.groupBox1, "groupBox1");
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.TabStop = false;
		// 
		// applySummerTime
		// 
		resources.ApplyResources(this.applySummerTime, "applySummerTime");
		this.applySummerTime.Name = "applySummerTime";
		this.applySummerTime.UseVisualStyleBackColor = true;
		// 
		// txtSMSphoneNum
		// 
		resources.ApplyResources(this.txtSMSphoneNum, "txtSMSphoneNum");
		this.txtSMSphoneNum.Name = "txtSMSphoneNum";
		// 
		// txtRateMax
		// 
		resources.ApplyResources(this.txtRateMax, "txtRateMax");
		this.txtRateMax.Name = "txtRateMax";
		// 
		// txtRate
		// 
		resources.ApplyResources(this.txtRate, "txtRate");
		this.txtRate.Name = "txtRate";
		// 
		// label4
		// 
		resources.ApplyResources(this.label4, "label4");
		this.label4.Name = "label4";
		// 
		// label5
		// 
		resources.ApplyResources(this.label5, "label5");
		this.label5.Name = "label5";
		// 
		// label3
		// 
		resources.ApplyResources(this.label3, "label3");
		this.label3.Name = "label3";
		// 
		// groupBox2
		// 
		this.groupBox2.Controls.Add(this.chkShowInLeftBottom);
		this.groupBox2.Controls.Add(this.cmdOpenFireFox);
		this.groupBox2.Controls.Add(this.txtFireFoxDir);
		this.groupBox2.Controls.Add(this.useFireFox);
		resources.ApplyResources(this.groupBox2, "groupBox2");
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.TabStop = false;
		// 
		// chkShowInLeftBottom
		// 
		resources.ApplyResources(this.chkShowInLeftBottom, "chkShowInLeftBottom");
		this.chkShowInLeftBottom.Name = "chkShowInLeftBottom";
		this.chkShowInLeftBottom.UseVisualStyleBackColor = true;
		// 
		// cmdOpenFireFox
		// 
		resources.ApplyResources(this.cmdOpenFireFox, "cmdOpenFireFox");
		this.cmdOpenFireFox.Name = "cmdOpenFireFox";
		this.cmdOpenFireFox.UseVisualStyleBackColor = true;
		this.cmdOpenFireFox.Click += new System.EventHandler(this.cmdOpenFireFox_Click);
		// 
		// txtFireFoxDir
		// 
		resources.ApplyResources(this.txtFireFoxDir, "txtFireFoxDir");
		this.txtFireFoxDir.Name = "txtFireFoxDir";
		// 
		// useFireFox
		// 
		resources.ApplyResources(this.useFireFox, "useFireFox");
		this.useFireFox.Name = "useFireFox";
		this.useFireFox.UseVisualStyleBackColor = true;
		this.useFireFox.CheckedChanged += new System.EventHandler(this.useFireFox_CheckedChanged);
		// 
		// openFileDialog1
		// 
		this.openFileDialog1.FileName = "FireFox.exe";
		// 
		// groupBox3
		// 
		this.groupBox3.Controls.Add(this.txtMailPassword);
		this.groupBox3.Controls.Add(this.txtMailUserID);
		this.groupBox3.Controls.Add(this.txtMailServer);
		this.groupBox3.Controls.Add(this.txtMailAddress);
		this.groupBox3.Controls.Add(this.label7);
		this.groupBox3.Controls.Add(this.label6);
		this.groupBox3.Controls.Add(this.label2);
		this.groupBox3.Controls.Add(this.label1);
		resources.ApplyResources(this.groupBox3, "groupBox3");
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.TabStop = false;
		// 
		// txtMailPassword
		// 
		resources.ApplyResources(this.txtMailPassword, "txtMailPassword");
		this.txtMailPassword.Name = "txtMailPassword";
		// 
		// txtMailUserID
		// 
		resources.ApplyResources(this.txtMailUserID, "txtMailUserID");
		this.txtMailUserID.Name = "txtMailUserID";
		// 
		// txtMailServer
		// 
		resources.ApplyResources(this.txtMailServer, "txtMailServer");
		this.txtMailServer.Name = "txtMailServer";
		// 
		// txtMailAddress
		// 
		resources.ApplyResources(this.txtMailAddress, "txtMailAddress");
		this.txtMailAddress.Name = "txtMailAddress";
		// 
		// label7
		// 
		resources.ApplyResources(this.label7, "label7");
		this.label7.Name = "label7";
		// 
		// label6
		// 
		resources.ApplyResources(this.label6, "label6");
		this.label6.Name = "label6";
		// 
		// label2
		// 
		resources.ApplyResources(this.label2, "label2");
		this.label2.Name = "label2";
		// 
		// label1
		// 
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		// 
		// OptionsForm
		// 
		this.AcceptButton = this.okButton;
		resources.ApplyResources(this, "$this");
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.CancelButton = this.cancelButton;
		this.Controls.Add(this.groupBox3);
		this.Controls.Add(this.groupBox2);
		this.Controls.Add(this.groupBox1);
		this.Controls.Add(this.cancelButton);
		this.Controls.Add(this.okButton);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "OptionsForm";
		this.Load += new System.EventHandler(this.OptionsForm_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		this.groupBox3.ResumeLayout(false);
		this.groupBox3.PerformLayout();
		this.ResumeLayout(false);

    }

    #endregion

	private System.Windows.Forms.Button okButton;
	  private System.Windows.Forms.Button cancelButton;
	  private System.Windows.Forms.GroupBox groupBox1;
	  private System.Windows.Forms.TextBox txtRate;
	  private System.Windows.Forms.Label label3;
	  private System.Windows.Forms.TextBox txtSMSphoneNum;
	  private System.Windows.Forms.Label label4;
	  private System.Windows.Forms.CheckBox applySummerTime;
	  private System.Windows.Forms.TextBox txtRateMax;
	  private System.Windows.Forms.Label label5;
	  private System.Windows.Forms.GroupBox groupBox2;
	  private System.Windows.Forms.Button cmdOpenFireFox;
	  private System.Windows.Forms.TextBox txtFireFoxDir;
	  private System.Windows.Forms.CheckBox useFireFox;
	  private System.Windows.Forms.OpenFileDialog openFileDialog1;
	  private System.Windows.Forms.CheckBox chkShowInLeftBottom;
	  private System.Windows.Forms.GroupBox groupBox3;
	  private System.Windows.Forms.TextBox txtMailServer;
	  private System.Windows.Forms.TextBox txtMailAddress;
	  private System.Windows.Forms.Label label7;
	  private System.Windows.Forms.Label label6;
	  private System.Windows.Forms.Label label2;
	  private System.Windows.Forms.Label label1;
	  private System.Windows.Forms.TextBox txtMailPassword;
	  private System.Windows.Forms.TextBox txtMailUserID;
  }
}