using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.DirectoryServices;

namespace UserManage
{
	/// <summary>
	/// frmMain에 대한 요약 설명입니다.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		enum ADS_USER_FLAG_ENUM
		{
			ADS_UF_SCRIPT = 0X0001, 
			ADS_UF_ACCOUNTDISABLE = 0X0002, 
			ADS_UF_HOMEDIR_REQUIRED = 0X0008, 
			ADS_UF_LOCKOUT = 0X0010, 
			ADS_UF_PASSWD_NOTREQD = 0X0020, 
			ADS_UF_PASSWD_CANT_CHANGE = 0X0040, 
			ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080, 
			ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0X0100, 
			ADS_UF_NORMAL_ACCOUNT = 0X0200, 
			ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0X0800, 
			ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0X1000, 
			ADS_UF_SERVER_TRUST_ACCOUNT = 0X2000, 
			ADS_UF_DONT_EXPIRE_PASSWD = 0X10000, 
			ADS_UF_MNS_LOGON_ACCOUNT = 0X20000, 
			ADS_UF_SMARTCARD_REQUIRED = 0X40000, 
			ADS_UF_TRUSTED_FOR_DELEGATION = 0X80000, 
			ADS_UF_NOT_DELEGATED = 0X100000, 
			ADS_UF_USE_DES_KEY_ONLY = 0x200000, 
			ADS_UF_DONT_REQUIRE_PREAUTH = 0x400000, 
			ADS_UF_PASSWORD_EXPIRED = 0x800000, 
			ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000
		};

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtUNO;
		private System.Windows.Forms.TextBox txtUserID;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button cmdCreate;
		private System.Windows.Forms.Button cmdDelete;
		private System.Windows.Forms.Button cmdRetrieve;
		private System.Windows.Forms.Button cmdModify;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label4;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.txtUNO = new System.Windows.Forms.TextBox();
			this.txtUserID = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.cmdCreate = new System.Windows.Forms.Button();
			this.cmdDelete = new System.Windows.Forms.Button();
			this.cmdRetrieve = new System.Windows.Forms.Button();
			this.cmdModify = new System.Windows.Forms.Button();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Unique No :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtUNO
			// 
			this.txtUNO.Location = new System.Drawing.Point(95, 19);
			this.txtUNO.Name = "txtUNO";
			this.txtUNO.TabIndex = 1;
			this.txtUNO.Text = "";
			// 
			// txtUserID
			// 
			this.txtUserID.Location = new System.Drawing.Point(95, 49);
			this.txtUserID.Name = "txtUserID";
			this.txtUserID.TabIndex = 3;
			this.txtUserID.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(39, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "User ID :";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(95, 79);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.TabIndex = 5;
			this.txtPassword.Text = "";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(22, 84);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Password :";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Font = new System.Drawing.Font("굴림체", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
			this.lblStatus.Location = new System.Drawing.Point(216, 16);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(232, 152);
			this.lblStatus.TabIndex = 8;
			// 
			// cmdCreate
			// 
			this.cmdCreate.Location = new System.Drawing.Point(48, 192);
			this.cmdCreate.Name = "cmdCreate";
			this.cmdCreate.TabIndex = 9;
			this.cmdCreate.Text = "Create";
			this.cmdCreate.Click += new System.EventHandler(this.cmdCreate_Click);
			// 
			// cmdDelete
			// 
			this.cmdDelete.Location = new System.Drawing.Point(146, 192);
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.TabIndex = 10;
			this.cmdDelete.Text = "Delete";
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			// 
			// cmdRetrieve
			// 
			this.cmdRetrieve.Location = new System.Drawing.Point(244, 192);
			this.cmdRetrieve.Name = "cmdRetrieve";
			this.cmdRetrieve.TabIndex = 11;
			this.cmdRetrieve.Text = "Retrieve";
			this.cmdRetrieve.Click += new System.EventHandler(this.cmdRetrieve_Click);
			// 
			// cmdModify
			// 
			this.cmdModify.Location = new System.Drawing.Point(342, 192);
			this.cmdModify.Name = "cmdModify";
			this.cmdModify.TabIndex = 12;
			this.cmdModify.Text = "Modify";
			this.cmdModify.Click += new System.EventHandler(this.cmdModify_Click);
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(96, 108);
			this.txtName.Name = "txtName";
			this.txtName.TabIndex = 7;
			this.txtName.Text = "";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(45, 113);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 14);
			this.label4.TabIndex = 6;
			this.label4.Text = "Name :";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(464, 237);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtName,
																		  this.label4,
																		  this.cmdModify,
																		  this.cmdRetrieve,
																		  this.cmdDelete,
																		  this.cmdCreate,
																		  this.lblStatus,
																		  this.txtPassword,
																		  this.label3,
																		  this.txtUserID,
																		  this.label2,
																		  this.txtUNO,
																		  this.label1});
			this.Name = "frmMain";
			this.Text = "User Management";
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

		private void cmdCreate_Click(object sender, System.EventArgs e)
		{
			using (DirectoryEntry de = new DirectoryEntry())
			{
				de.Path = "LDAP://bmtad1/ou=SchemaTest,dc=bmt,dc=com";
				//de.Path = "LDAP://bmtad1/ou=Members,dc=bmt,dc=com";
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users,dc=bmt,dc=com";
				de.Password = "netsserver1@";

				lblStatus.Text = "Connecting to AD (" + de.Username + ")...\r\n";
				lblStatus.Refresh();

				try
				{
					using (DirectoryEntry user = de.Children.Add("cn=" + txtUserID.Text, "netmarbleuser"))
					{
						user.Properties["uno"].Value = txtUNO.Text;
						user.Properties["cn"].Value = txtUserID.Text;
						user.Properties["upwd"].Value = txtPassword.Text;
						user.Properties["displayName"].Value = txtName.Text;
						user.Properties["resRegNum"].Value = "7311111111111";
						user.Properties["mail"].Value = txtUserID.Text + "@netmarble.net";
						user.Properties["ugrade"].Value = "1";
						user.Properties["uprofile"].Value = txtUserID.Text + "의 프로필입니다.";
						user.Properties["uzipcode"].Value = "135712";
						user.Properties["uaddr"].Value = "서울 강남 대치4 상제리제빌딩 B동 204호";
						user.Properties["uregdate"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

						user.CommitChanges();
						user.Close();
					}
				}
				catch (Exception ex)
				{
					lblStatus.Text += ex.ToString() + " (" + ex.Message + ")";
					lblStatus.Refresh();
				}
				de.Close();

				lblStatus.Text += "Creation complete!\r\n";
				lblStatus.Refresh();
			}
		}

		private void cmdDelete_Click(object sender, System.EventArgs e)
		{
			using (DirectoryEntry de = new DirectoryEntry())
			{
				de.Path = "LDAP://bmtad1/ou=SchemaTest,dc=bmt,dc=com";
				//de.Path = "LDAP://bmtad1/ou=Members,dc=bmt,dc=com";
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users,dc=bmt,dc=com";
				de.Password = "netsserver1@";

				lblStatus.Text = "Connecting to AD (" + de.Username + ")...\r\n";
				lblStatus.Refresh();

				try
				{
					DirectoryEntry user = de.Children.Find("cn=" + txtUserID.Text, "netmarbleuser");
					if (user != null)
						user.DeleteTree();
				}
				catch (Exception ex)
				{
					lblStatus.Text += ex.ToString() + " (" + ex.Message + ")";
					lblStatus.Refresh();
				}
				de.Close();

				lblStatus.Text += "Deletion complete!\r\n";
				lblStatus.Refresh();
			}
		}

		private void cmdRetrieve_Click(object sender, System.EventArgs e)
		{
			using (DirectoryEntry de = new DirectoryEntry())
			{
				de.Path = "LDAP://bmtad1/ou=SchemaTest,dc=bmt,dc=com";
				//de.Path = "LDAP://bmtad1/ou=Members,dc=bmt,dc=com";
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users,dc=bmt,dc=com";
				de.Password = "netsserver1@";

				lblStatus.Text = "Connecting to AD (" + de.Username + ")...\r\n";
				lblStatus.Refresh();

				try
				{
					using (DirectoryEntry user = de.Children.Find("cn=" + txtUserID.Text, "netmarbleuser"))
					{
						if (user != null)
						{
							txtUNO.Text = (String) user.Properties["uno"].Value;
							txtPassword.Text = (String) user.Properties["upwd"].Value;
							txtName.Text = (String) user.Properties["displayName"].Value;
							lblStatus.Text += "resRegNum: " + user.Properties["resRegNum"].Value + "\r\n";
							lblStatus.Text += "mail: " + user.Properties["mail"].Value + "\r\n";
							lblStatus.Text += "ugrade: " + user.Properties["ugrade"].Value + "\r\n";
							lblStatus.Text += "uprofile: " + user.Properties["uprofile"].Value + "\r\n";
							lblStatus.Text += "uzipcode: " + user.Properties["uzipcode"].Value + "\r\n";
							lblStatus.Text += "uaddr: " + user.Properties["uaddr"].Value + "\r\n";
							lblStatus.Text += "uregdate: " + user.Properties["uregdate"].Value + "\r\n";
							user.Close();
						}
					}
				}
				catch (Exception ex)
				{
					lblStatus.Text += ex.ToString() + " (" + ex.Message + ")";
					lblStatus.Refresh();
				}
				de.Close();

				lblStatus.Text += "Retrieval complete!\r\n";
				lblStatus.Refresh();
			}
		}

		private void cmdModify_Click(object sender, System.EventArgs e)
		{
			using (DirectoryEntry de = new DirectoryEntry())
			{
				de.Path = "LDAP://bmtad1/ou=SchemaTest,dc=bmt,dc=com";
				//de.Path = "LDAP://bmtad1/ou=Members,dc=bmt,dc=com";
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users,dc=bmt,dc=com";
				de.Password = "netsserver1@";

				lblStatus.Text = "Connecting to AD (" + de.Username + ")...\r\n";
				lblStatus.Refresh();

				try
				{
					using (DirectoryEntry user = de.Children.Find("cn=" + txtUserID.Text, "netmarbleuser"))
					{
						user.Properties["uno"].Value = txtUNO.Text;
						user.Properties["upwd"].Value = txtPassword.Text;
						user.Properties["displayName"].Value = txtName.Text;

						user.CommitChanges();
						user.Close();
					}
				}
				catch (Exception ex)
				{
					lblStatus.Text += ex.ToString() + " (" + ex.Message + ")";
					lblStatus.Refresh();
				}
				de.Close();

				lblStatus.Text += "Modification complete!\r\n";
				lblStatus.Refresh();
			}
		}
	}
}
