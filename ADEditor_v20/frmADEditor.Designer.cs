using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ADEditor
{
	partial class frmADEditor : Form
	{
		private Button btnNext;
		private Label lblNext;
		private Button cmdAuthenticate;
		private CheckBox ckDontExpirePwd;
		private ToolTip toolTip;
		private Button btnLdapTest;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private TextBox txtDN;
		private TextBox txtADUser;
		private TextBox txtADPwd;
		private TextBox txtOU;
		private TextBox txtSchemaClass;
		private TextBox txtName01;
		private TextBox txtValue01;
		private TextBox txtValue02;
		private TextBox txtName02;
		private TextBox txtValue03;
		private TextBox txtName03;
		private TextBox txtValue04;
		private TextBox txtName04;
		private TextBox txtValue05;
		private TextBox txtName05;
		private TextBox txtValue08;
		private TextBox txtName08;
		private TextBox txtValue07;
		private TextBox txtName07;
		private TextBox txtValue06;
		private TextBox txtName06;
		private Button cmdCreate;
		private Button cmdRetrieve;
		private Button cmdUpdate;
		private Button cmdDelete;
		private Label label6;
		private TextBox txtobjectClass;
		private Label lblStatus;
		private CheckBox ckSyncPwd;
		private CheckBox ckADAM;

		private IContainer components;

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.Run(new frmADEditor());
		}

		public frmADEditor()
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
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmADEditor));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtDN = new System.Windows.Forms.TextBox();
			this.txtADUser = new System.Windows.Forms.TextBox();
			this.txtADPwd = new System.Windows.Forms.TextBox();
			this.txtOU = new System.Windows.Forms.TextBox();
			this.txtSchemaClass = new System.Windows.Forms.TextBox();
			this.txtName01 = new System.Windows.Forms.TextBox();
			this.txtValue01 = new System.Windows.Forms.TextBox();
			this.txtValue02 = new System.Windows.Forms.TextBox();
			this.txtName02 = new System.Windows.Forms.TextBox();
			this.txtValue03 = new System.Windows.Forms.TextBox();
			this.txtName03 = new System.Windows.Forms.TextBox();
			this.txtValue04 = new System.Windows.Forms.TextBox();
			this.txtName04 = new System.Windows.Forms.TextBox();
			this.txtValue05 = new System.Windows.Forms.TextBox();
			this.txtName05 = new System.Windows.Forms.TextBox();
			this.txtValue08 = new System.Windows.Forms.TextBox();
			this.txtName08 = new System.Windows.Forms.TextBox();
			this.txtValue07 = new System.Windows.Forms.TextBox();
			this.txtName07 = new System.Windows.Forms.TextBox();
			this.txtValue06 = new System.Windows.Forms.TextBox();
			this.txtName06 = new System.Windows.Forms.TextBox();
			this.cmdCreate = new System.Windows.Forms.Button();
			this.cmdRetrieve = new System.Windows.Forms.Button();
			this.cmdUpdate = new System.Windows.Forms.Button();
			this.cmdDelete = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.txtobjectClass = new System.Windows.Forms.TextBox();
			this.lblStatus = new System.Windows.Forms.Label();
			this.ckSyncPwd = new System.Windows.Forms.CheckBox();
			this.ckADAM = new System.Windows.Forms.CheckBox();
			this.btnNext = new System.Windows.Forms.Button();
			this.lblNext = new System.Windows.Forms.Label();
			this.cmdAuthenticate = new System.Windows.Forms.Button();
			this.ckDontExpirePwd = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.btnLdapTest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Bind DN:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Auth. User:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "Password:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(44, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(126, 12);
			this.label4.TabIndex = 3;
			this.label4.Text = "조회할 root container:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(309, 99);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(45, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "클래스:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtDN
			// 
			this.txtDN.Location = new System.Drawing.Point(104, 8);
			this.txtDN.Name = "txtDN";
			this.txtDN.Size = new System.Drawing.Size(368, 21);
			this.txtDN.TabIndex = 5;
			// 
			// txtADUser
			// 
			this.txtADUser.Location = new System.Drawing.Point(104, 32);
			this.txtADUser.Name = "txtADUser";
			this.txtADUser.Size = new System.Drawing.Size(368, 21);
			this.txtADUser.TabIndex = 6;
			// 
			// txtADPwd
			// 
			this.txtADPwd.Location = new System.Drawing.Point(104, 56);
			this.txtADPwd.Name = "txtADPwd";
			this.txtADPwd.PasswordChar = '*';
			this.txtADPwd.Size = new System.Drawing.Size(248, 21);
			this.txtADPwd.TabIndex = 7;
			// 
			// txtOU
			// 
			this.txtOU.Location = new System.Drawing.Point(176, 96);
			this.txtOU.Name = "txtOU";
			this.txtOU.Size = new System.Drawing.Size(112, 21);
			this.txtOU.TabIndex = 8;
			// 
			// txtSchemaClass
			// 
			this.txtSchemaClass.Location = new System.Drawing.Point(360, 96);
			this.txtSchemaClass.Name = "txtSchemaClass";
			this.txtSchemaClass.Size = new System.Drawing.Size(112, 21);
			this.txtSchemaClass.TabIndex = 9;
			this.txtSchemaClass.Text = "organizationalUnit";
			// 
			// txtName01
			// 
			this.txtName01.BackColor = System.Drawing.Color.Yellow;
			this.txtName01.Location = new System.Drawing.Point(40, 160);
			this.txtName01.Name = "txtName01";
			this.txtName01.Size = new System.Drawing.Size(144, 21);
			this.txtName01.TabIndex = 10;
			this.txtName01.Text = "cn";
			// 
			// txtValue01
			// 
			this.txtValue01.Location = new System.Drawing.Point(184, 160);
			this.txtValue01.Name = "txtValue01";
			this.txtValue01.Size = new System.Drawing.Size(136, 21);
			this.txtValue01.TabIndex = 11;
			this.txtValue01.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtValue02
			// 
			this.txtValue02.Location = new System.Drawing.Point(184, 184);
			this.txtValue02.Name = "txtValue02";
			this.txtValue02.Size = new System.Drawing.Size(288, 21);
			this.txtValue02.TabIndex = 13;
			this.txtValue02.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName02
			// 
			this.txtName02.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
			this.txtName02.Location = new System.Drawing.Point(40, 184);
			this.txtName02.Name = "txtName02";
			this.txtName02.Size = new System.Drawing.Size(144, 21);
			this.txtName02.TabIndex = 12;
			this.txtName02.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtName02_MouseMove);
			// 
			// txtValue03
			// 
			this.txtValue03.Location = new System.Drawing.Point(184, 256);
			this.txtValue03.Name = "txtValue03";
			this.txtValue03.Size = new System.Drawing.Size(288, 21);
			this.txtValue03.TabIndex = 15;
			this.txtValue03.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName03
			// 
			this.txtName03.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtName03.Location = new System.Drawing.Point(40, 256);
			this.txtName03.Name = "txtName03";
			this.txtName03.Size = new System.Drawing.Size(144, 21);
			this.txtName03.TabIndex = 14;
			this.txtName03.Text = "displayName";
			// 
			// txtValue04
			// 
			this.txtValue04.Location = new System.Drawing.Point(184, 280);
			this.txtValue04.Name = "txtValue04";
			this.txtValue04.Size = new System.Drawing.Size(288, 21);
			this.txtValue04.TabIndex = 17;
			this.txtValue04.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName04
			// 
			this.txtName04.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtName04.Location = new System.Drawing.Point(40, 280);
			this.txtName04.Name = "txtName04";
			this.txtName04.Size = new System.Drawing.Size(144, 21);
			this.txtName04.TabIndex = 16;
			this.txtName04.Text = "mail";
			// 
			// txtValue05
			// 
			this.txtValue05.Location = new System.Drawing.Point(184, 304);
			this.txtValue05.Name = "txtValue05";
			this.txtValue05.Size = new System.Drawing.Size(288, 21);
			this.txtValue05.TabIndex = 19;
			this.txtValue05.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName05
			// 
			this.txtName05.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtName05.Location = new System.Drawing.Point(40, 304);
			this.txtName05.Name = "txtName05";
			this.txtName05.Size = new System.Drawing.Size(144, 21);
			this.txtName05.TabIndex = 18;
			// 
			// txtValue08
			// 
			this.txtValue08.Location = new System.Drawing.Point(184, 376);
			this.txtValue08.Name = "txtValue08";
			this.txtValue08.Size = new System.Drawing.Size(288, 21);
			this.txtValue08.TabIndex = 25;
			this.txtValue08.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName08
			// 
			this.txtName08.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtName08.Location = new System.Drawing.Point(40, 376);
			this.txtName08.Name = "txtName08";
			this.txtName08.Size = new System.Drawing.Size(144, 21);
			this.txtName08.TabIndex = 24;
			// 
			// txtValue07
			// 
			this.txtValue07.Location = new System.Drawing.Point(184, 352);
			this.txtValue07.Name = "txtValue07";
			this.txtValue07.Size = new System.Drawing.Size(288, 21);
			this.txtValue07.TabIndex = 23;
			this.txtValue07.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName07
			// 
			this.txtName07.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtName07.Location = new System.Drawing.Point(40, 352);
			this.txtName07.Name = "txtName07";
			this.txtName07.Size = new System.Drawing.Size(144, 21);
			this.txtName07.TabIndex = 22;
			// 
			// txtValue06
			// 
			this.txtValue06.Location = new System.Drawing.Point(184, 328);
			this.txtValue06.Name = "txtValue06";
			this.txtValue06.Size = new System.Drawing.Size(288, 21);
			this.txtValue06.TabIndex = 21;
			this.txtValue06.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName06
			// 
			this.txtName06.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtName06.Location = new System.Drawing.Point(40, 328);
			this.txtName06.Name = "txtName06";
			this.txtName06.Size = new System.Drawing.Size(144, 21);
			this.txtName06.TabIndex = 20;
			// 
			// cmdCreate
			// 
			this.cmdCreate.Location = new System.Drawing.Point(28, 456);
			this.cmdCreate.Name = "cmdCreate";
			this.cmdCreate.Size = new System.Drawing.Size(88, 32);
			this.cmdCreate.TabIndex = 26;
			this.cmdCreate.Text = "생성";
			this.cmdCreate.Click += new System.EventHandler(this.cmdCreate_Click);
			// 
			// cmdRetrieve
			// 
			this.cmdRetrieve.Location = new System.Drawing.Point(116, 456);
			this.cmdRetrieve.Name = "cmdRetrieve";
			this.cmdRetrieve.Size = new System.Drawing.Size(88, 32);
			this.cmdRetrieve.TabIndex = 27;
			this.cmdRetrieve.Text = "조회";
			this.cmdRetrieve.Click += new System.EventHandler(this.cmdRetrieve_Click);
			// 
			// cmdUpdate
			// 
			this.cmdUpdate.Location = new System.Drawing.Point(204, 456);
			this.cmdUpdate.Name = "cmdUpdate";
			this.cmdUpdate.Size = new System.Drawing.Size(88, 32);
			this.cmdUpdate.TabIndex = 28;
			this.cmdUpdate.Text = "수정";
			this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
			// 
			// cmdDelete
			// 
			this.cmdDelete.Location = new System.Drawing.Point(292, 456);
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.Size = new System.Drawing.Size(88, 32);
			this.cmdDelete.TabIndex = 29;
			this.cmdDelete.Text = "삭제";
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Location = new System.Drawing.Point(40, 136);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(144, 23);
			this.label6.TabIndex = 30;
			this.label6.Text = "조회할 개체의 클래스";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtobjectClass
			// 
			this.txtobjectClass.Location = new System.Drawing.Point(184, 136);
			this.txtobjectClass.Name = "txtobjectClass";
			this.txtobjectClass.Size = new System.Drawing.Size(104, 21);
			this.txtobjectClass.TabIndex = 31;
			this.txtobjectClass.Text = "user";
			this.txtobjectClass.Leave += new System.EventHandler(this.txtobjectClass_Leave);
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(40, 408);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(432, 23);
			this.lblStatus.TabIndex = 32;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ckSyncPwd
			// 
			this.ckSyncPwd.Location = new System.Drawing.Point(48, 208);
			this.ckSyncPwd.Name = "ckSyncPwd";
			this.ckSyncPwd.Size = new System.Drawing.Size(184, 24);
			this.ckSyncPwd.TabIndex = 33;
			this.ckSyncPwd.Text = "System Password와 동기화";
			this.ckSyncPwd.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ckSyncPwd_MouseMove);
			// 
			// ckADAM
			// 
			this.ckADAM.Location = new System.Drawing.Point(376, 208);
			this.ckADAM.Name = "ckADAM";
			this.ckADAM.Size = new System.Drawing.Size(96, 24);
			this.ckADAM.TabIndex = 34;
			this.ckADAM.Text = "ADAM User";
			this.ckADAM.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ckADAM_MouseMove);
			// 
			// btnNext
			// 
			this.btnNext.Enabled = false;
			this.btnNext.Location = new System.Drawing.Point(408, 159);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(64, 22);
			this.btnNext.TabIndex = 35;
			this.btnNext.Text = "다음";
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// lblNext
			// 
			this.lblNext.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblNext.Location = new System.Drawing.Point(320, 160);
			this.lblNext.Name = "lblNext";
			this.lblNext.Size = new System.Drawing.Size(86, 21);
			this.lblNext.TabIndex = 36;
			this.lblNext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cmdAuthenticate
			// 
			this.cmdAuthenticate.Location = new System.Drawing.Point(396, 456);
			this.cmdAuthenticate.Name = "cmdAuthenticate";
			this.cmdAuthenticate.Size = new System.Drawing.Size(88, 32);
			this.cmdAuthenticate.TabIndex = 37;
			this.cmdAuthenticate.Text = "인증";
			this.cmdAuthenticate.Click += new System.EventHandler(this.cmdAuthenticate_Click);
			// 
			// ckDontExpirePwd
			// 
			this.ckDontExpirePwd.Location = new System.Drawing.Point(48, 232);
			this.ckDontExpirePwd.Name = "ckDontExpirePwd";
			this.ckDontExpirePwd.Size = new System.Drawing.Size(200, 24);
			this.ckDontExpirePwd.TabIndex = 38;
			this.ckDontExpirePwd.Text = "Password 사용기간 제한 없음";
			// 
			// btnLdapTest
			// 
			this.btnLdapTest.Location = new System.Drawing.Point(358, 56);
			this.btnLdapTest.Name = "btnLdapTest";
			this.btnLdapTest.Size = new System.Drawing.Size(113, 23);
			this.btnLdapTest.TabIndex = 39;
			this.btnLdapTest.Text = "찾아보기...";
			this.btnLdapTest.UseVisualStyleBackColor = true;
			this.btnLdapTest.Click += new System.EventHandler(this.btnLdapTest_Click);
			// 
			// frmADEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(512, 507);
			this.Controls.Add(this.btnLdapTest);
			this.Controls.Add(this.ckDontExpirePwd);
			this.Controls.Add(this.cmdAuthenticate);
			this.Controls.Add(this.lblNext);
			this.Controls.Add(this.btnNext);
			this.Controls.Add(this.ckADAM);
			this.Controls.Add(this.ckSyncPwd);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.txtobjectClass);
			this.Controls.Add(this.txtValue08);
			this.Controls.Add(this.txtName08);
			this.Controls.Add(this.txtValue07);
			this.Controls.Add(this.txtName07);
			this.Controls.Add(this.txtValue06);
			this.Controls.Add(this.txtName06);
			this.Controls.Add(this.txtValue05);
			this.Controls.Add(this.txtName05);
			this.Controls.Add(this.txtValue04);
			this.Controls.Add(this.txtName04);
			this.Controls.Add(this.txtValue03);
			this.Controls.Add(this.txtName03);
			this.Controls.Add(this.txtValue02);
			this.Controls.Add(this.txtName02);
			this.Controls.Add(this.txtValue01);
			this.Controls.Add(this.txtName01);
			this.Controls.Add(this.txtSchemaClass);
			this.Controls.Add(this.txtOU);
			this.Controls.Add(this.txtADPwd);
			this.Controls.Add(this.txtADUser);
			this.Controls.Add(this.txtDN);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.cmdDelete);
			this.Controls.Add(this.cmdUpdate);
			this.Controls.Add(this.cmdRetrieve);
			this.Controls.Add(this.cmdCreate);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmADEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Active Directory 편집기";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}
