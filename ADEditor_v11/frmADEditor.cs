using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ActiveDs;

namespace ADEditor
{
	/// <summary>
	/// Form1�� ���� ��� �����Դϴ�.
	/// </summary>
	public class Form1 : Form
	{
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

		private const string	FILE_NAME = "ADManager.ini";
		private static int		currentCount = 0;
		private static bool		conditioned = false;
		private static string	condString = "";
		private static string	searchedString = "";
		private IContainer components;

		// Active Directory ������ ���Ǵ� ��� ���
		enum ADS_USER_FLAG_ENUM
		{
			ADS_UF_SCRIPT = 0x0001, 
			ADS_UF_ACCOUNTDISABLE = 0x0002, 
			ADS_UF_HOMEDIR_REQUIRED = 0x0008, 
			ADS_UF_LOCKOUT = 0x0010, 
			ADS_UF_PASSWD_NOTREQD = 0x0020, 
			ADS_UF_PASSWD_CANT_CHANGE = 0x0040, 
			ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x0080, 
			ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x0100, 
			ADS_UF_NORMAL_ACCOUNT = 0x0200, 
			ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800, 
			ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000, 
			ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000, 
			ADS_UF_DONT_EXPIRE_PASSWD = 0x10000, 
			ADS_UF_MNS_LOGON_ACCOUNT = 0x20000, 
			ADS_UF_SMARTCARD_REQUIRED = 0x40000, 
			ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000, 
			ADS_UF_NOT_DELEGATED = 0x100000, 
			ADS_UF_USE_DES_KEY_ONLY = 0x200000, 
			ADS_UF_DONT_REQUIRE_PREAUTH = 0x400000, 
			ADS_UF_PASSWORD_EXPIRED = 0x800000, 
			ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000
		};

		// ADAM Password �ɼ�
		private const int ADS_OPTION_PASSWORD_PORTNUMBER = 6;
		private const int ADS_OPTION_PASSWORD_METHOD = 7;

		private const int ADS_LDAP_PORT = 389;
		private const int ADS_PASSWORD_ENCODE_REQUIRE_SSL = 0;
		private Button btnNext;
		private Label lblNext;
		private Button cmdAuthenticate;
		private CheckBox ckDontExpirePwd;
		private ToolTip toolTip;
		private const int ADS_PASSWORD_ENCODE_CLEAR = 1;


		public Form1()
		{
			//
			// Windows Form �����̳� ������ �ʿ��մϴ�.
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent�� ȣ���� ���� ������ �ڵ带 �߰��մϴ�.
			//
		}

		/// <summary>
		/// ��� ���� ��� ���ҽ��� �����մϴ�.
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

		#region Windows Form �����̳ʿ��� ������ �ڵ�
		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
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
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Bind DN :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Auth. User :";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "Password :";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 23);
			this.label4.TabIndex = 3;
			this.label4.Text = "��ȸ�� OU �Ǵ� CN :";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(296, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 23);
			this.label5.TabIndex = 4;
			this.label5.Text = "Ŭ���� :";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtDN
			// 
			this.txtDN.Location = new System.Drawing.Point(104, 8);
			this.txtDN.Name = "txtDN";
			this.txtDN.Size = new System.Drawing.Size(368, 21);
			this.txtDN.TabIndex = 5;
			this.txtDN.Text = "";
			// 
			// txtADUser
			// 
			this.txtADUser.Location = new System.Drawing.Point(104, 32);
			this.txtADUser.Name = "txtADUser";
			this.txtADUser.Size = new System.Drawing.Size(368, 21);
			this.txtADUser.TabIndex = 6;
			this.txtADUser.Text = "";
			// 
			// txtADPwd
			// 
			this.txtADPwd.Location = new System.Drawing.Point(104, 56);
			this.txtADPwd.Name = "txtADPwd";
			this.txtADPwd.PasswordChar = '*';
			this.txtADPwd.Size = new System.Drawing.Size(184, 21);
			this.txtADPwd.TabIndex = 7;
			this.txtADPwd.Text = "";
			// 
			// txtOU
			// 
			this.txtOU.Location = new System.Drawing.Point(176, 96);
			this.txtOU.Name = "txtOU";
			this.txtOU.Size = new System.Drawing.Size(112, 21);
			this.txtOU.TabIndex = 8;
			this.txtOU.Text = "";
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
			this.txtValue01.Text = "";
			this.txtValue01.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtValue02
			// 
			this.txtValue02.Location = new System.Drawing.Point(184, 184);
			this.txtValue02.Name = "txtValue02";
			this.txtValue02.Size = new System.Drawing.Size(288, 21);
			this.txtValue02.TabIndex = 13;
			this.txtValue02.Text = "";
			this.txtValue02.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName02
			// 
			this.txtName02.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(192)), ((System.Byte)(255)));
			this.txtName02.Location = new System.Drawing.Point(40, 184);
			this.txtName02.Name = "txtName02";
			this.txtName02.Size = new System.Drawing.Size(144, 21);
			this.txtName02.TabIndex = 12;
			this.txtName02.Text = "";
			this.txtName02.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtName02_MouseMove);
			// 
			// txtValue03
			// 
			this.txtValue03.Location = new System.Drawing.Point(184, 256);
			this.txtValue03.Name = "txtValue03";
			this.txtValue03.Size = new System.Drawing.Size(288, 21);
			this.txtValue03.TabIndex = 15;
			this.txtValue03.Text = "";
			this.txtValue03.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName03
			// 
			this.txtName03.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
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
			this.txtValue04.Text = "";
			this.txtValue04.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName04
			// 
			this.txtName04.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
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
			this.txtValue05.Text = "";
			this.txtValue05.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName05
			// 
			this.txtName05.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.txtName05.Location = new System.Drawing.Point(40, 304);
			this.txtName05.Name = "txtName05";
			this.txtName05.Size = new System.Drawing.Size(144, 21);
			this.txtName05.TabIndex = 18;
			this.txtName05.Text = "";
			// 
			// txtValue08
			// 
			this.txtValue08.Location = new System.Drawing.Point(184, 376);
			this.txtValue08.Name = "txtValue08";
			this.txtValue08.Size = new System.Drawing.Size(288, 21);
			this.txtValue08.TabIndex = 25;
			this.txtValue08.Text = "";
			this.txtValue08.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName08
			// 
			this.txtName08.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.txtName08.Location = new System.Drawing.Point(40, 376);
			this.txtName08.Name = "txtName08";
			this.txtName08.Size = new System.Drawing.Size(144, 21);
			this.txtName08.TabIndex = 24;
			this.txtName08.Text = "";
			// 
			// txtValue07
			// 
			this.txtValue07.Location = new System.Drawing.Point(184, 352);
			this.txtValue07.Name = "txtValue07";
			this.txtValue07.Size = new System.Drawing.Size(288, 21);
			this.txtValue07.TabIndex = 23;
			this.txtValue07.Text = "";
			this.txtValue07.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName07
			// 
			this.txtName07.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.txtName07.Location = new System.Drawing.Point(40, 352);
			this.txtName07.Name = "txtName07";
			this.txtName07.Size = new System.Drawing.Size(144, 21);
			this.txtName07.TabIndex = 22;
			this.txtName07.Text = "";
			// 
			// txtValue06
			// 
			this.txtValue06.Location = new System.Drawing.Point(184, 328);
			this.txtValue06.Name = "txtValue06";
			this.txtValue06.Size = new System.Drawing.Size(288, 21);
			this.txtValue06.TabIndex = 21;
			this.txtValue06.Text = "";
			this.txtValue06.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
			// 
			// txtName06
			// 
			this.txtName06.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.txtName06.Location = new System.Drawing.Point(40, 328);
			this.txtName06.Name = "txtName06";
			this.txtName06.Size = new System.Drawing.Size(144, 21);
			this.txtName06.TabIndex = 20;
			this.txtName06.Text = "";
			// 
			// cmdCreate
			// 
			this.cmdCreate.Location = new System.Drawing.Point(28, 456);
			this.cmdCreate.Name = "cmdCreate";
			this.cmdCreate.Size = new System.Drawing.Size(88, 32);
			this.cmdCreate.TabIndex = 26;
			this.cmdCreate.Text = "����";
			this.cmdCreate.Click += new System.EventHandler(this.cmdCreate_Click);
			// 
			// cmdRetrieve
			// 
			this.cmdRetrieve.Location = new System.Drawing.Point(116, 456);
			this.cmdRetrieve.Name = "cmdRetrieve";
			this.cmdRetrieve.Size = new System.Drawing.Size(88, 32);
			this.cmdRetrieve.TabIndex = 27;
			this.cmdRetrieve.Text = "��ȸ";
			this.cmdRetrieve.Click += new System.EventHandler(this.cmdRetrieve_Click);
			// 
			// cmdUpdate
			// 
			this.cmdUpdate.Location = new System.Drawing.Point(204, 456);
			this.cmdUpdate.Name = "cmdUpdate";
			this.cmdUpdate.Size = new System.Drawing.Size(88, 32);
			this.cmdUpdate.TabIndex = 28;
			this.cmdUpdate.Text = "����";
			this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
			// 
			// cmdDelete
			// 
			this.cmdDelete.Location = new System.Drawing.Point(292, 456);
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.Size = new System.Drawing.Size(88, 32);
			this.cmdDelete.TabIndex = 29;
			this.cmdDelete.Text = "����";
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Location = new System.Drawing.Point(40, 136);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(144, 23);
			this.label6.TabIndex = 30;
			this.label6.Text = "��ȸ�� ��ü�� Ŭ����";
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
			this.ckSyncPwd.Text = "System Password�� ����ȭ";
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
			this.btnNext.Text = "����";
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
			this.cmdAuthenticate.Text = "����";
			this.cmdAuthenticate.Click += new System.EventHandler(this.cmdAuthenticate_Click);
			// 
			// ckDontExpirePwd
			// 
			this.ckDontExpirePwd.Location = new System.Drawing.Point(48, 232);
			this.ckDontExpirePwd.Name = "ckDontExpirePwd";
			this.ckDontExpirePwd.Size = new System.Drawing.Size(200, 24);
			this.ckDontExpirePwd.TabIndex = 38;
			this.ckDontExpirePwd.Text = "Password ���Ⱓ ���� ����";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(512, 507);
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
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "����� ����(AD) ������";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			int retCode = ADInit(txtDN, txtADUser, txtADPwd, txtOU);
			if (retCode < 0)
				MessageBox.Show("���������� �дµ� ������ �߻��߽��ϴ�.");

			if (txtobjectClass.Text.Trim().ToLower() == "user")
			{
				ckSyncPwd.Enabled = true;
				ckADAM.Enabled = true;
			}
			else if (txtobjectClass.Text.Trim().ToLower() == "inetorgperson")
			{
				ckSyncPwd.Enabled = true;
				ckADAM.Enabled = true;
			}
			else
			{
				ckSyncPwd.Enabled = false;
				ckADAM.Enabled = false;
			}
		}

		private int ADInit(TextBox dn, TextBox admin, TextBox adminPwd, TextBox ou)
		{
			FileStream fs = null;
			StreamReader sr = null;
			string sLine = null;

			try
			{
				fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read);
				sr = new StreamReader(fs, Encoding.Default);
				while ((sLine = sr.ReadLine()) != null)
				{
					switch (sLine.ToLower().Trim())
					{
						case "[ldap]":
							sLine = sr.ReadLine();
							dn.Text = sLine.Trim();
							break;

						case "[administrator]":
							sLine = sr.ReadLine();
							admin.Text = sLine.Trim();
							break;

						case "[adminpassword]":
							sLine = sr.ReadLine();
							adminPwd.Text = sLine.Trim();
							break;

						case "[source]":
							sLine = sr.ReadLine();
							ou.Text = sLine.Trim();
							break;

						default :
							sLine = "";
							break;
					}
				}
				sr.Close();
				fs.Close();

				return 0;
			}
			catch(Exception)
			{
				return -1;
			}
			finally
			{
				if (sr != null)
					sr.Close();

				if (fs != null)
					fs.Close();
			}
		}

		private void txtValue_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				cmdRetrieve_Click(sender, null);
		}

		private void cmdRetrieve_Click(object sender, EventArgs e)
		{
			currentCount = 0;
			conditioned = false;

			searchedString = txtValue01.Text;
			Retrieve();
		}

		private void Retrieve()
		{
			lblStatus.Text = "��ȸ ���Դϴ�. ��ø� ��ٷ��ּ���...";
			lblStatus.Refresh();

			// ADAM������ �ݵ�� �������¸� None Ȥ�� ServerBind ���·� ������־�� �Ѵ�.
			// ADAM�� �����Ϸ�����... -_-
			DirectoryEntry dn = new DirectoryEntry(txtDN.Text, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind);
			DirectoryEntry child = null;
			DirectoryEntry target = null;

			try
			{
				if (txtOU.Text.Trim().Length == 0)
				{
					child = dn;
				}
				else
				{
					child = dn.Children.Find(txtOU.Text, txtSchemaClass.Text);
				}

				if ((txtName01.Text.Trim().Length == 0) || (searchedString.Trim().Length == 0))
				{
					lblStatus.Text = "��ȸ�� ��ü�� ù��° �Ӽ�(�˻� Ű)�� ä���ּ���.";
					lblStatus.Refresh();
					return;
				}

				if ( (txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					(txtName01.Text.Trim().ToLower() != "o") && (!conditioned) )
				{
					DirectorySearcher searcher = new DirectorySearcher(child);
					searcher.Filter = "(&(" + txtName01.Text.Trim() + "=" + getReplacedString(searchedString) + ")" +
						"(objectClass=" + txtobjectClass.Text + "))";
					searcher.SearchScope = SearchScope.OneLevel;

					SearchResultCollection resultCollection = searcher.FindAll();
					int totalCount = resultCollection.Count;
					currentCount++;
					if ((totalCount - currentCount) > 0)
					{
						lblNext.Text = "(+" + (totalCount - currentCount).ToString() + ")";
						btnNext.Enabled = true;
					}
					else
					{
						lblNext.Text = "";
						btnNext.Enabled = false;
						cmdRetrieve.Focus();
					}
					unchecked
					{
						if (resultCollection.Count == 0)
							throw new COMException("��ȸ ����� �����ϴ�.", (int) 0x80072030);
					}

					target = resultCollection[currentCount - 1].GetDirectoryEntry();
				}
				else if ( (searchedString.IndexOf(">") >= 0) || (searchedString.IndexOf("<") >= 0) || 
					(searchedString.IndexOf("!") >= 0) || conditioned)
				{
					if (!conditioned)
					{
						conditioned = true;
						condString = "(&(" + txtName01.Text.Trim() + getReplacedString(searchedString) + ")" +
							"(objectClass=" + txtobjectClass.Text + "))";
					}

					DirectorySearcher searcher = new DirectorySearcher(child);
					searcher.Filter = condString;
					searcher.SearchScope = SearchScope.OneLevel;
					
					searcher.PropertiesToLoad.Clear();
					searcher.PropertiesToLoad.Add("cn");

					SearchResultCollection resultCollection = searcher.FindAll();
					int totalCount = resultCollection.Count;
					currentCount++;
					if ((totalCount - currentCount) > 0)
					{
						lblNext.Text = "(+" + (totalCount - currentCount).ToString() + ")";
						btnNext.Enabled = true;
					}
					else
					{
						lblNext.Text = "";
						btnNext.Enabled = false;
						cmdRetrieve.Focus();
					}
					unchecked
					{
						if (resultCollection.Count == 0)
							throw new COMException("��ȸ ����� �����ϴ�.", (int) 0x80072030);
					}

					target = resultCollection[currentCount - 1].GetDirectoryEntry();
				}
				else
				{
					target = child.Children.Find(txtName01.Text + "=" + searchedString, txtobjectClass.Text);
				}

				foreach(Control ctrl in Controls)
				{
					if (ctrl.GetType() == typeof(TextBox))
					{
						TextBox tb = (TextBox) ctrl;
						if ((tb.Name.IndexOf("txtName") >= 0) && (tb.Text.Trim().Length > 0))
						{
							string number = tb.Name.Substring(7, 2);
							foreach(Control ctrl2 in Controls)
							{
								if (ctrl2.GetType() == typeof(TextBox))
								{
									TextBox targetTextBox = (TextBox) ctrl2;
									if (targetTextBox.Name.ToLower() == "txtvalue" + number)
									{
										try
										{
											if (target.Properties[tb.Text].Count > 0)
												targetTextBox.Text = getStringProperty(target, tb);
											else
												targetTextBox.Text = "";
										}
										catch (COMException ex)
										{
											unchecked
											{
												if (ex.ErrorCode == (int) 0x8000500D)
													targetTextBox.Text = "";
												else if (ex.ErrorCode != 0)
													throw ex;
											}
										}
									}
								}
							}
						}
					}
				}
				lblStatus.Text = "���������� ��ȸ�Ǿ����ϴ�.";
				lblStatus.Refresh();
			}
			catch (COMException exx)
			{
				MessageBox.Show("��ȸ���� / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
			}
			finally
			{
				if (target != null)
					target.Close();

				if (child != null)
					child.Close();

				if (dn != null)
					dn.Close();
			}
		}

		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			lblStatus.Text = "���� ���Դϴ�. ��ø� ��ٷ��ּ���...";
			lblStatus.Refresh();

			DirectoryEntry dn = new DirectoryEntry(txtDN.Text, txtADUser.Text, txtADPwd.Text,
				AuthenticationTypes.ServerBind | AuthenticationTypes.Secure);
			DirectoryEntry child = null;
			DirectoryEntry target = null;

			try
			{
				if (txtOU.Text.Trim().Length == 0)
				{
					child = dn;
				}
				else
				{
					child = dn.Children.Find(txtOU.Text, txtSchemaClass.Text);
				}

				if ((txtName01.Text.Trim().Length == 0) || (txtValue01.Text.Trim().Length == 0))
				{
					lblStatus.Text = "������ ��ü�� ù��° �Ӽ��� ä���ּ���.";
					lblStatus.Refresh();
					return;
				}

				if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					(txtName01.Text.Trim().ToLower() != "o"))
				{
					lblStatus.Text = "������ ��ü�� ù��° �Ӽ� �̸��� cn, ou, o�� ���˴ϴ�.";
					lblStatus.Refresh();
					return;
				}

				target = child.Children.Find(txtName01.Text + "=" + txtValue01.Text, txtobjectClass.Text);
				foreach(Control ctrl in Controls)
				{
					if (ctrl.GetType() == typeof(TextBox))
					{
						TextBox tb = (TextBox) ctrl;
						if ((tb.Name.IndexOf("txtName") >= 0) && (tb.Text.Trim().Length > 0))
						{
							string number = tb.Name.Substring(7, 2);
							foreach(Control ctrl2 in Controls)
							{
								if (ctrl2.GetType() == typeof(TextBox))
								{
									TextBox targetTextBox = (TextBox) ctrl2;
									if ((targetTextBox.Name.ToLower() == "txtvalue" + number) && (number != "01"))
									{
										if (targetTextBox.Text.Trim().Length > 0)
										{
											/*
											if ((tb.Tag != null) && ((string)tb.Tag == "System.__ComObject"))
												setInt64Property(target, tb.Text, DateTime.Parse(targetTextBox.Text));
											else
											*/
											target.Properties[tb.Text].Value = targetTextBox.Text;
										}
										else
										{
											try
											{
												if (target.Properties[tb.Text].Value != null)
													target.Properties[tb.Text].Clear();
											}
											catch (COMException ex)
											{
												unchecked
												{
													if (ex.ErrorCode != (int) 0x8000500D)
														throw ex;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				target.CommitChanges();

				// Password ����ȭ �ɼ��� ���� �ִ� ���
				if ((ckSyncPwd.Checked) && (ckSyncPwd.Enabled) && (txtValue02.Text.Trim().Length > 0))
				{
					if (ckADAM.Checked)
					{
						// (���ÿ�����) �κ������θ� �ŷ��� �� �ִ� �� ���ø����̼� ���� ��쿡
						// ���ε��� �� ����ڿ� ��ȣ�� �������� �ʰ� Secure, Sealing, Signing �ɼ��� ������
						// ������ �̸����� �����Ͽ� GAC�� ����ؾ� �̿� ���� �������� ��� ������.
						using (DirectoryEntry pwdDN = new DirectoryEntry(target.Path,
								   null, null, 
								   AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing))
						{
							pwdDN.Invoke("SetOption", new object[] {ADS_OPTION_PASSWORD_PORTNUMBER, ADS_LDAP_PORT});
							pwdDN.Invoke("SetOption", new object[] {ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR});
							pwdDN.Invoke("SetPassword", txtValue02.Text);
							pwdDN.CommitChanges();
							pwdDN.Close();
						}
					}
					else
					{
						// SetPassword�� Kerberos������ �䱸�ǹǷ� Secure �ɼ��� �ʿ��ϴ�.
						target.Invoke("SetPassword", txtValue02.Text);
						//target.Invoke("ChangePassword", new object[] { txtValue08.Text, txtValue02.Text });
						target.Close();
					}
				}

				lblStatus.Text = "���������� �����Ǿ����ϴ�.";
				lblStatus.Refresh();
			}
			catch (TargetInvocationException tiEx)
			{
				COMException exx = (COMException)tiEx.InnerException;

				if (exx.ErrorCode == -2147024810) // 0x80070056: ���� �н����� Ʋ��
					MessageBox.Show("���� �н����尡 Ʋ�Ƚ��ϴ�.");
				else if (exx.ErrorCode == -2147022651) // 0x800708C5: �н����� ��å ����
					MessageBox.Show("�н����� ��å�� ���� �ʴ� �н����带 �Է��ϼ̽��ϴ�.");
				else
					MessageBox.Show("�������� / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
			}
			finally
			{
				if (target != null)
					target.Close();

				if (child != null)
					child.Close();

				if (dn != null)
					dn.Close();
			}
		}

		private void cmdCreate_Click(object sender, EventArgs e)
		{
			lblStatus.Text = "���� ���Դϴ�. ��ø� ��ٷ��ּ���...";
			lblStatus.Refresh();

			DirectoryEntry dn = new DirectoryEntry(txtDN.Text, txtADUser.Text, txtADPwd.Text,
				AuthenticationTypes.ServerBind | AuthenticationTypes.Secure);
			DirectoryEntry child = null;
			DirectoryEntry target = null;

			try
			{
				if (txtOU.Text.Trim().Length == 0)
				{
					child = dn;
				}
				else
				{
					child = dn.Children.Find(txtOU.Text, txtSchemaClass.Text);
				}

				if ((txtName01.Text.Trim().Length == 0) || (txtValue01.Text.Trim().Length == 0))
				{
					lblStatus.Text = "������ ��ü�� ù��° �Ӽ��� ä���ּ���.";
					lblStatus.Refresh();
					return;
				}

				if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					(txtName01.Text.Trim().ToLower() != "o"))
				{
					lblStatus.Text = "������ ��ü�� ù��° �Ӽ� �̸��� cn, ou, o�� ���˴ϴ�.";
					lblStatus.Refresh();
					return;
				}

				target = child.Children.Add(txtName01.Text + "=" + txtValue01.Text, txtobjectClass.Text);
				foreach(Control ctrl in Controls)
				{
					if (ctrl.GetType() == typeof(TextBox))
					{
						TextBox tb = (TextBox) ctrl;
						if ((tb.Name.IndexOf("txtName") >= 0) && (tb.Text.Trim().Length > 0))
						{
							string number = tb.Name.Substring(7, 2);
							foreach(Control ctrl2 in Controls)
							{
								if (ctrl2.GetType() == typeof(TextBox))
								{
									TextBox targetTextBox = (TextBox) ctrl2;
									if (targetTextBox.Name.ToLower() == "txtvalue" + number)
									{
										if (targetTextBox.Text.Trim().Length > 0)
											target.Properties[tb.Text].Value = targetTextBox.Text;
									}
								}
							}
						}
					}
				}

				// AD user ������ ��쿡�� sAMAccountName�� �������ش�.
				if ((ckSyncPwd.Enabled) && (!ckADAM.Checked))
				{
					target.Properties["sAMAccountName"].Value = txtName01.Text;
				}
				target.CommitChanges();

				// Password ����ȭ�� ������ ���
				if ((txtValue02.Text.Trim().Length > 0) && (ckSyncPwd.Checked))
				{
					// ADAM�� ���� AD�� ��츦 �����Ͽ� ó���Ѵ�.
					if (ckADAM.Checked)
					{
						target.Properties["ms-DS-UserAccountAutoLocked"].Value = false;
						target.Properties["msDS-User-Account-Control-Computed"].Value = (int) ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT;
						target.Properties["msDS-UserAccountDisabled"].Clear();
						if (ckDontExpirePwd.Checked)
							target.Properties["msDS-UserPasswordExpired"].Value = false;

						// (���ÿ�����) �κ������θ� �ŷ��� �� �ִ� �� ���ø����̼� ���� ��쿡
						// ���ε��� �� ����ڿ� ��ȣ�� �������� �ʰ� Secure, Sealing, Signing �ɼ��� ������
						// ������ �̸����� �����Ͽ� GAC�� ����ؾ� �̿� ���� �������� ��� ������.
						using (DirectoryEntry pwdDN = new DirectoryEntry(target.Path,
								   null, null, 
								   AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing))
						{
							pwdDN.Invoke("SetOption", new object[] {ADS_OPTION_PASSWORD_PORTNUMBER, ADS_LDAP_PORT});
							pwdDN.Invoke("SetOption", new object[] {ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR});
							pwdDN.Invoke("SetPassword", txtValue02.Text);
							pwdDN.CommitChanges();
							pwdDN.Close();
						}
					}
					else
					{
						target.Invoke("SetPassword", txtValue02.Text);

						if (ckDontExpirePwd.Checked)
							target.Properties["userAccountControl"].Value = (int) ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT |
								(int) ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD;
						else
							target.Properties["userAccountControl"].Value = (int) ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT;
					}
					target.CommitChanges();
				}

				lblStatus.Text = "���������� �����Ǿ����ϴ�.";
				lblStatus.Refresh();
			}
			catch (TargetInvocationException tiEx)
			{
				COMException exx = (COMException)tiEx.InnerException;

				if (exx.ErrorCode == -2147022651) // 0x800708C5: �н����� ��å ����
					MessageBox.Show("�н����� ��å�� ���� �ʴ� �н����带 �Է��ϼ̽��ϴ�.");
				else
					MessageBox.Show("�������� / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
			}
			catch (COMException exx)
			{
				MessageBox.Show("�������� / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
			}
			finally
			{
				if (target != null)
					target.Close();

				if (child != null)
					child.Close();

				if (dn != null)
					dn.Close();
			}
		}

		private void txtobjectClass_Leave(object sender, EventArgs e)
		{
			if (txtobjectClass.Text.Trim().ToLower() == "user")
			{
				ckSyncPwd.Enabled = true;
				ckADAM.Enabled = true;
			}
			else if (txtobjectClass.Text.Trim().ToLower() == "inetorgperson")
			{
				ckSyncPwd.Enabled = true;
				ckADAM.Enabled = true;
			}
			else
			{
				ckSyncPwd.Enabled = false;
				ckADAM.Enabled = false;
			}
		}

		private void cmdDelete_Click(object sender, EventArgs e)
		{
			lblStatus.Text = "���� ���Դϴ�. ��ø� ��ٷ��ּ���...";
			lblStatus.Refresh();

			DirectoryEntry dn = new DirectoryEntry(txtDN.Text, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind);
			DirectoryEntry child = null;
			DirectoryEntry target = null;

			try
			{
				if (txtOU.Text.Trim().Length == 0)
				{
					child = dn;
				}
				else
				{
					child = dn.Children.Find(txtOU.Text, txtSchemaClass.Text);
				}

				if ((txtName01.Text.Trim().Length == 0) || (txtValue01.Text.Trim().Length == 0))
				{
					lblStatus.Text = "������ ��ü�� ù��° �Ӽ��� ä���ּ���.";
					lblStatus.Refresh();
					return;
				}

				if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					(txtName01.Text.Trim().ToLower() != "o"))
				{
					lblStatus.Text = "������ ��ü�� ù��° �Ӽ� �̸��� cn, ou, o�� ���˴ϴ�.";
					lblStatus.Refresh();
					return;
				}

				target = child.Children.Find(txtName01.Text + "=" + txtValue01.Text, txtobjectClass.Text);
				child.Children.Remove(target);

				lblStatus.Text = "���������� �����Ǿ����ϴ�.";
				lblStatus.Refresh();
			}
			catch (COMException exx)
			{
				MessageBox.Show("�������� / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
			}
			finally
			{
				if (target != null)
					target.Close();

				if (child != null)
					child.Close();

				if (dn != null)
					dn.Close();
			}
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			Retrieve();
		}

		private string getReplacedString(string sOrg)
		{
			string sReplaced = sOrg.Replace("\\", "\\5c");

			if (!sReplaced.EndsWith("*") && (sReplaced.IndexOf("*)") < 0))
			{
				sReplaced = sReplaced.Replace("*", "\\2a");
				sReplaced = sReplaced.Replace("(", "\\28");
				sReplaced = sReplaced.Replace(")", "\\29");
			}
			sReplaced = sReplaced.Replace("\0", "\\00");
			sReplaced = sReplaced.Replace(" ", "\\20");

			return sReplaced;
		}

		private void cmdAuthenticate_Click(object sender, EventArgs e)
		{
			lblStatus.Text = "���� ó�� ���Դϴ�. ��ø� ��ٷ��ּ���...";
			lblStatus.Refresh();

			try
			{
				// ADAM������ �ݵ�� �������¸� None Ȥ�� ServerBind ���·� ������־�� �Ѵ�.
				// ADAM�� �����Ϸ�����... -_-
				using (DirectoryEntry dn = new DirectoryEntry(txtDN.Text, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind))
				{
					using (DirectoryEntry user = searchAD(dn, txtValue01.Text))
					{
						if (user == null) throw new Exception("���̵� �����ϴ�.");

						// pwdLastSet(Int64)�Ӽ��� 0�̸� ���� �α׿¶� �н����� ���� �ʿ�
						// userAccountControl�� 512�� ��쿡��.
						int userAccountControl = getIntProperty(user, "userAccountControl");
						if ((userAccountControl & (int)ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD) == 0)
						{
							long pwdLastSet = getInt64Property(user, "pwdLastSet");
							if (pwdLastSet == 0)
							{
								user.Close();
								throw new Exception("���� ����: �н����� ������ �ʿ��մϴ�.");
							}

							string newUserName = user.Path.Substring(user.Path.IndexOf(user.Name)).Replace(user.Name, "CN=" + txtValue01.Text);
							user.Username = newUserName;
							user.Password = txtValue02.Text;
							user.AuthenticationType = AuthenticationTypes.ServerBind;
							user.RefreshCache();

							// �н����� ���� �˸� ����� ������ ���
							string rootDN = getRootDN(txtDN.Text);
							long maxPwdAge = 0;
							using (DirectoryEntry domain = new DirectoryEntry(rootDN, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind))
							{
								maxPwdAge = getInt64Property(domain, "maxPwdAge");
								domain.Close();
							}

							if (maxPwdAge != 0)	// �����ο� �н����� ����Ⱓ�� �����Ǿ� �ִ� ���
							{
								int maxDay = (int)Math.Abs(maxPwdAge / 10000000 / 86400);
								DateTime expireDate = DateTime.FromFileTime(pwdLastSet).AddDays(maxDay);
								int remains = expireDate.Subtract(DateTime.Now).Days;
								if (remains < 0)
									throw new Exception("�н����尡 ����Ǿ����ϴ�.");
								else if (remains < 7)
									throw new Exception("�н����� �����ϱ��� " + remains + "�� ���ҽ��ϴ�.");
							}
						}
					}
					dn.Close();
				}
				lblStatus.Text = "���������� �����Ǿ����ϴ�.";
				lblStatus.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private DirectoryEntry searchAD(DirectoryEntry AD, string uid)
		{
			try
			{
				using (DirectorySearcher searcher = new DirectorySearcher(AD))
				{
					// contact�� ����, user�� ���
					searcher.Filter = "(&(objectClass=user)(cn=" + uid + "))";
					searcher.SearchScope = SearchScope.Subtree;
					searcher.CacheResults = false;
					SearchResult result = searcher.FindOne();

					return result.GetDirectoryEntry();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private string getRootDN(string dn)
		{
			int pos1 = dn.LastIndexOf("/");
			int pos2 = dn.ToLower().IndexOf("dc=");
			return dn.Substring(0, pos1 + 1) + dn.Substring(pos2);
		}

		/// <summary>
		/// AD���� ���� �о� ���ڿ��� �����Ѵ�.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="tb"></param>
		/// <returns></returns>
		private string getStringProperty(DirectoryEntry entry, TextBox tb)
		{
			try
			{
				string text = tb.Text;
				if (entry.Properties[text].Value != null)
				{
					Type type = entry.Properties[text].Value.GetType();
					if (type.FullName == "System.__ComObject")
					{
						tb.Tag = type.FullName;
						return DateTime.FromFileTime(getInt64Property(entry, text)).ToString("yyyy-MM-dd HH:mm:ss");
					}
					else
					{
						tb.Tag = null;
						return entry.Properties[text].Value.ToString();
					}
				}
			}
			catch (COMException)
			{
			}
			return null;
		}

		/// <summary>
		/// AD���� ���� �о� ������ �����Ѵ�.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		private int getIntProperty(DirectoryEntry entry, string property)
		{
			try
			{
				if (entry.Properties[property].Value != null)
					return (int)entry.Properties[property].Value;
			}
			catch (COMException)
			{
			}
			return 0;
		}

		/// <summary>
		/// AD���� ���� �о� 64��Ʈ ������ �����Ѵ�.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		private long getInt64Property(DirectoryEntry entry, string property)
		{
			try
			{
				if (entry.Properties[property].Value != null)
				{
					LargeInteger li = (LargeInteger)entry.Properties[property].Value;
					int highPart = li.HighPart;
					int lowPart = li.LowPart;
					if (lowPart == 0)	// ���� 32��Ʈ ������ 0�̸� �� ���� �ȵ� ����
						return 0;
					else
						return highPart * (long)Math.Pow(2, 32) + lowPart;
				}
			}
			catch (COMException)
			{
			}
			return 0;
		}

		/// <summary>
		/// AD���� DateTime ���� 64��Ʈ ���� ���·� ���� �����Ѵ�.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="property"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		private void setInt64Property(DirectoryEntry entry, string property, DateTime val)
		{
			try
			{
				long lngValue = val.ToFileTimeUtc();
				/*
				entry.Properties[property].Value = lngValue;
				*/
				LargeInteger li = new LargeIntegerClass();
				li.HighPart = (int)(lngValue / (long)Math.Pow(2, 32));
				li.LowPart = (int)(lngValue - li.HighPart * (long)Math.Pow(2, 32));
				entry.Properties[property].Value = li;
			}
			catch (COMException)
			{
			}
			return;
		}

		private void txtName02_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(txtName02, "�ý��� �Ӽ� �� �н����� �Ӽ��� ������ �����ϴ� ��쿡��\r\n�̰��� �н����� �Ӽ���(��:userPassword)�� �Է��ϼ���.");
		}

		private void ckSyncPwd_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			toolTip.SetToolTip(ckSyncPwd, "�� �Է��� �н����� ������ �ý��� �н����带 ������ ���\r\n�� �׸� üũ�ϼ���.");
		}

		private void ckADAM_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			toolTip.SetToolTip(ckADAM, "�����Ϸ��� ��ü�� ADAM user ��ü�� ���\r\n�� �׸� üũ�ϼ���.");
		}
	}
}
