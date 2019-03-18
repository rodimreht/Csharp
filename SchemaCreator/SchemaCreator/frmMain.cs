using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace SchemaCreator
{
	/// <summary>
	/// frmMain에 대한 요약 설명입니다.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOidgen;
		private System.Windows.Forms.Button cmdFindOidgen;
		private System.Windows.Forms.Button cmdExecOidgen;
		private System.Windows.Forms.TextBox lblConsole;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabSSO;
		private System.Windows.Forms.TabPage tabEtc;
		private System.Windows.Forms.CheckBox chkOrganization;
		private System.Windows.Forms.CheckBox chkProvider;
		private System.Windows.Forms.CheckBox ckSite;
		private System.Windows.Forms.CheckBox chkACL;
		private System.Windows.Forms.Button cmdExecSSO;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMain()
		{
			//
			// Windows Form 디자이너 지원에 필요합니다.
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
			//
		}

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form 디자이너에서 생성한 코드
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOidgen = new System.Windows.Forms.TextBox();
			this.cmdFindOidgen = new System.Windows.Forms.Button();
			this.cmdExecOidgen = new System.Windows.Forms.Button();
			this.lblConsole = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabSSO = new System.Windows.Forms.TabPage();
			this.tabEtc = new System.Windows.Forms.TabPage();
			this.chkOrganization = new System.Windows.Forms.CheckBox();
			this.chkProvider = new System.Windows.Forms.CheckBox();
			this.ckSite = new System.Windows.Forms.CheckBox();
			this.chkACL = new System.Windows.Forms.CheckBox();
			this.cmdExecSSO = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabSSO.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "oidgen:";
			// 
			// txtOidgen
			// 
			this.txtOidgen.Location = new System.Drawing.Point(56, 16);
			this.txtOidgen.Name = "txtOidgen";
			this.txtOidgen.Size = new System.Drawing.Size(416, 21);
			this.txtOidgen.TabIndex = 2;
			this.txtOidgen.Text = "";
			this.txtOidgen.Leave += new System.EventHandler(this.txtOidgen_Leave);
			// 
			// cmdFindOidgen
			// 
			this.cmdFindOidgen.Location = new System.Drawing.Point(472, 16);
			this.cmdFindOidgen.Name = "cmdFindOidgen";
			this.cmdFindOidgen.Size = new System.Drawing.Size(80, 24);
			this.cmdFindOidgen.TabIndex = 3;
			this.cmdFindOidgen.Text = "파일찾기...";
			this.cmdFindOidgen.Click += new System.EventHandler(this.cmdFindOidgen_Click);
			// 
			// cmdExecOidgen
			// 
			this.cmdExecOidgen.Location = new System.Drawing.Point(568, 16);
			this.cmdExecOidgen.Name = "cmdExecOidgen";
			this.cmdExecOidgen.Size = new System.Drawing.Size(56, 24);
			this.cmdExecOidgen.TabIndex = 4;
			this.cmdExecOidgen.Text = "실 행";
			this.cmdExecOidgen.Click += new System.EventHandler(this.cmdExecOidgen_Click);
			// 
			// lblConsole
			// 
			this.lblConsole.BackColor = System.Drawing.Color.Black;
			this.lblConsole.Font = new System.Drawing.Font("바탕체", 9F);
			this.lblConsole.ForeColor = System.Drawing.Color.LightGray;
			this.lblConsole.Location = new System.Drawing.Point(8, 48);
			this.lblConsole.Multiline = true;
			this.lblConsole.Name = "lblConsole";
			this.lblConsole.ReadOnly = true;
			this.lblConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.lblConsole.Size = new System.Drawing.Size(616, 88);
			this.lblConsole.TabIndex = 5;
			this.lblConsole.Text = "";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabSSO);
			this.tabControl1.Controls.Add(this.tabEtc);
			this.tabControl1.Location = new System.Drawing.Point(8, 144);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(616, 256);
			this.tabControl1.TabIndex = 6;
			// 
			// tabSSO
			// 
			this.tabSSO.Controls.Add(this.cmdExecSSO);
			this.tabSSO.Controls.Add(this.chkACL);
			this.tabSSO.Controls.Add(this.ckSite);
			this.tabSSO.Controls.Add(this.chkProvider);
			this.tabSSO.Controls.Add(this.chkOrganization);
			this.tabSSO.Location = new System.Drawing.Point(4, 21);
			this.tabSSO.Name = "tabSSO";
			this.tabSSO.Size = new System.Drawing.Size(608, 231);
			this.tabSSO.TabIndex = 0;
			this.tabSSO.Text = "SSO 스키마";
			// 
			// tabEtc
			// 
			this.tabEtc.Location = new System.Drawing.Point(4, 21);
			this.tabEtc.Name = "tabEtc";
			this.tabEtc.Size = new System.Drawing.Size(608, 231);
			this.tabEtc.TabIndex = 1;
			this.tabEtc.Text = "일반 스키마";
			// 
			// chkOrganization
			// 
			this.chkOrganization.Location = new System.Drawing.Point(176, 40);
			this.chkOrganization.Name = "chkOrganization";
			this.chkOrganization.Size = new System.Drawing.Size(256, 24);
			this.chkOrganization.TabIndex = 0;
			this.chkOrganization.Text = "1. 최상위 개체(nsOrganization) 생성";
			// 
			// chkProvider
			// 
			this.chkProvider.Location = new System.Drawing.Point(176, 72);
			this.chkProvider.Name = "chkProvider";
			this.chkProvider.Size = new System.Drawing.Size(256, 24);
			this.chkProvider.TabIndex = 1;
			this.chkProvider.Text = "2. 인증 서버 개체(nsSSOProvider) 생성";
			// 
			// ckSite
			// 
			this.ckSite.Location = new System.Drawing.Point(176, 104);
			this.ckSite.Name = "ckSite";
			this.ckSite.Size = new System.Drawing.Size(256, 24);
			this.ckSite.TabIndex = 2;
			this.ckSite.Text = "3. 사이트 개체(nsSSOSite) 생성";
			// 
			// chkACL
			// 
			this.chkACL.Location = new System.Drawing.Point(176, 136);
			this.chkACL.Name = "chkACL";
			this.chkACL.Size = new System.Drawing.Size(256, 24);
			this.chkACL.TabIndex = 3;
			this.chkACL.Text = "4. ACL 개체(nsACL) 생성";
			// 
			// cmdExecSSO
			// 
			this.cmdExecSSO.Location = new System.Drawing.Point(276, 184);
			this.cmdExecSSO.Name = "cmdExecSSO";
			this.cmdExecSSO.Size = new System.Drawing.Size(56, 24);
			this.cmdExecSSO.TabIndex = 4;
			this.cmdExecSSO.Text = "실 행";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(632, 411);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.lblConsole);
			this.Controls.Add(this.cmdExecOidgen);
			this.Controls.Add(this.cmdFindOidgen);
			this.Controls.Add(this.txtOidgen);
			this.Controls.Add(this.label1);
			this.Name = "frmMain";
			this.Text = "Active Directory Schema Creator 2.0";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabSSO.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		private void frmMain_Load(object sender, System.EventArgs e)
		{
			txtOidgen.Text = Application.StartupPath + @"\" + "oidgen.exe";
			FileInfo fi = new FileInfo(txtOidgen.Text);
			if (fi.Exists)
			{
				cmdExecOidgen.Enabled = true;
				lblConsole.BackColor = Color.Black;
				lblConsole.ScrollBars = ScrollBars.Vertical;
			}
			else
			{
				cmdExecOidgen.Enabled = false;
				lblConsole.BackColor = Color.LightGray;
				lblConsole.ScrollBars = ScrollBars.None;
			}
		}

		private void cmdFindOidgen_Click(object sender, System.EventArgs e)
		{
			openFileDialog1.DefaultExt = "exe";
			openFileDialog1.InitialDirectory = Application.StartupPath;
			openFileDialog1.Filter = "실행 파일 (*.exe)|*.exe";
			openFileDialog1.Multiselect = false;
			//openFileDialog1.FilterIndex = 1;
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.Title = "oidgen.exe 찾기...";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				txtOidgen.Text = openFileDialog1.FileName;
				cmdExecOidgen.Enabled = true;
				lblConsole.BackColor = Color.Black;
				lblConsole.ScrollBars = ScrollBars.Vertical;
			}
		}

		private void cmdExecOidgen_Click(object sender, System.EventArgs e)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.UseShellExecute = false;
			psi.WindowStyle = ProcessWindowStyle.Normal;
			psi.FileName = txtOidgen.Text;
			//psi.Arguments = "";
			psi.CreateNoWindow = true;
			psi.RedirectStandardInput = false;
			psi.RedirectStandardOutput = true;
			Process proc = Process.Start(psi);
			StreamReader reader = proc.StandardOutput;
			
			// 내용이 있으면 줄바꿈 추가
			String newText = lblConsole.Text;
			if (newText.Length > 0)
				newText += "\r\n";
			newText += reader.ReadToEnd();
			reader.Close();
			proc.Close();
			
			// 표시 후 맨 아래로 스크롤
			lblConsole.Text = newText;
			lblConsole.SelectionStart = newText.Length;
			lblConsole.SelectionLength = 0;
			lblConsole.ScrollToCaret();
		}

		private void txtOidgen_Leave(object sender, System.EventArgs e)
		{
			if (txtOidgen.Text.Length > 0)
			{
				FileInfo fi = new FileInfo(txtOidgen.Text);
				if (fi.Exists)
				{
					cmdExecOidgen.Enabled = true;
					lblConsole.BackColor = Color.Black;
					lblConsole.ScrollBars = ScrollBars.Vertical;
				}
				else
				{
					cmdExecOidgen.Enabled = false;
					lblConsole.BackColor = Color.LightGray;
					lblConsole.ScrollBars = ScrollBars.None;
				}
			}
		}
	}
}
