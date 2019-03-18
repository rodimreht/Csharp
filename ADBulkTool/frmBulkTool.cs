using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.IO;
using System.Collections.Specialized;

namespace BulkJob
{
	/// <summary>
	/// frmBulkTool에 대한 요약 설명입니다.
	/// </summary>
	public class frmBulkTool : System.Windows.Forms.Form
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

		private string SCHEMA_CLASS_NAME = "netmarbleuser";

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cmdOpen;
		private System.Windows.Forms.Button cmdStart;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button cmdMore;
		private System.Windows.Forms.Button cmdADTest;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtNumber;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtEndNumber;
		private System.Windows.Forms.TextBox txtEndTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtStartTime;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtDirectorySuffix;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Button cmdADUpdate;
		private System.Windows.Forms.Button cmdADDelete;
		private System.Windows.Forms.Button cmdADRetrieve;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmBulkTool()
		{
			//
			// Windows Form 디자이너 지원에 필요합니다.
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
			//
			this.Size = new Size(472, 328);
			cmdMore.Text = "More <<";
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
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cmdOpen = new System.Windows.Forms.Button();
			this.cmdStart = new System.Windows.Forms.Button();
			this.lblStatus = new System.Windows.Forms.Label();
			this.cmdMore = new System.Windows.Forms.Button();
			this.cmdADTest = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtNumber = new System.Windows.Forms.TextBox();
			this.txtEndNumber = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtEndTime = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtStartTime = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtDirectorySuffix = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.cmdADUpdate = new System.Windows.Forms.Button();
			this.cmdADDelete = new System.Windows.Forms.Button();
			this.cmdADRetrieve = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtFile
			// 
			this.txtFile.BackColor = System.Drawing.Color.White;
			this.txtFile.Location = new System.Drawing.Point(88, 48);
			this.txtFile.Name = "txtFile";
			this.txtFile.ReadOnly = true;
			this.txtFile.Size = new System.Drawing.Size(312, 21);
			this.txtFile.TabIndex = 3;
			this.txtFile.Text = "";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "File to load :";
			// 
			// cmdOpen
			// 
			this.cmdOpen.Location = new System.Drawing.Point(400, 48);
			this.cmdOpen.Name = "cmdOpen";
			this.cmdOpen.Size = new System.Drawing.Size(56, 23);
			this.cmdOpen.TabIndex = 5;
			this.cmdOpen.Text = "File";
			this.cmdOpen.Click += new System.EventHandler(this.cmdOpen_Click);
			// 
			// cmdStart
			// 
			this.cmdStart.Location = new System.Drawing.Point(200, 192);
			this.cmdStart.Name = "cmdStart";
			this.cmdStart.TabIndex = 6;
			this.cmdStart.Text = "Start";
			this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(8, 80);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(448, 64);
			this.lblStatus.TabIndex = 7;
			// 
			// cmdMore
			// 
			this.cmdMore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdMore.Location = new System.Drawing.Point(384, 192);
			this.cmdMore.Name = "cmdMore";
			this.cmdMore.TabIndex = 8;
			this.cmdMore.Text = "More >>";
			this.cmdMore.Click += new System.EventHandler(this.cmdMore_Click);
			// 
			// cmdADTest
			// 
			this.cmdADTest.Location = new System.Drawing.Point(16, 264);
			this.cmdADTest.Name = "cmdADTest";
			this.cmdADTest.Size = new System.Drawing.Size(96, 23);
			this.cmdADTest.TabIndex = 9;
			this.cmdADTest.Text = "Test Input";
			this.cmdADTest.Click += new System.EventHandler(this.cmdADTest_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(37, 240);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 17);
			this.label2.TabIndex = 12;
			this.label2.Text = "Start number:";
			// 
			// txtNumber
			// 
			this.txtNumber.BackColor = System.Drawing.Color.White;
			this.txtNumber.Location = new System.Drawing.Point(120, 232);
			this.txtNumber.Name = "txtNumber";
			this.txtNumber.Size = new System.Drawing.Size(72, 21);
			this.txtNumber.TabIndex = 13;
			this.txtNumber.Text = "1";
			this.txtNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtEndNumber
			// 
			this.txtEndNumber.BackColor = System.Drawing.Color.White;
			this.txtEndNumber.Location = new System.Drawing.Point(304, 232);
			this.txtEndNumber.Name = "txtEndNumber";
			this.txtEndNumber.Size = new System.Drawing.Size(72, 21);
			this.txtEndNumber.TabIndex = 15;
			this.txtEndNumber.Text = "20000000";
			this.txtEndNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(224, 240);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 17);
			this.label3.TabIndex = 14;
			this.label3.Text = "End number:";
			// 
			// txtEndTime
			// 
			this.txtEndTime.BackColor = System.Drawing.Color.White;
			this.txtEndTime.Location = new System.Drawing.Point(312, 152);
			this.txtEndTime.Name = "txtEndTime";
			this.txtEndTime.ReadOnly = true;
			this.txtEndTime.Size = new System.Drawing.Size(144, 21);
			this.txtEndTime.TabIndex = 19;
			this.txtEndTime.Text = "";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(248, 160);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 17);
			this.label4.TabIndex = 18;
			this.label4.Text = "End time:";
			// 
			// txtStartTime
			// 
			this.txtStartTime.BackColor = System.Drawing.Color.White;
			this.txtStartTime.Location = new System.Drawing.Point(72, 152);
			this.txtStartTime.Name = "txtStartTime";
			this.txtStartTime.ReadOnly = true;
			this.txtStartTime.Size = new System.Drawing.Size(144, 21);
			this.txtStartTime.TabIndex = 17;
			this.txtStartTime.Text = "";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 17);
			this.label5.TabIndex = 16;
			this.label5.Text = "Start time:";
			// 
			// txtDirectorySuffix
			// 
			this.txtDirectorySuffix.BackColor = System.Drawing.Color.White;
			this.txtDirectorySuffix.Location = new System.Drawing.Point(288, 8);
			this.txtDirectorySuffix.Name = "txtDirectorySuffix";
			this.txtDirectorySuffix.Size = new System.Drawing.Size(168, 21);
			this.txtDirectorySuffix.TabIndex = 23;
			this.txtDirectorySuffix.Text = "dc=bmt,dc=com";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(192, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(97, 17);
			this.label6.TabIndex = 22;
			this.label6.Text = "Directory Suffix:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(24, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 17);
			this.label7.TabIndex = 20;
			this.label7.Text = "Server:";
			// 
			// txtServer
			// 
			this.txtServer.BackColor = System.Drawing.Color.White;
			this.txtServer.Location = new System.Drawing.Point(72, 8);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(96, 21);
			this.txtServer.TabIndex = 21;
			this.txtServer.Text = "real";
			// 
			// cmdADUpdate
			// 
			this.cmdADUpdate.Location = new System.Drawing.Point(128, 264);
			this.cmdADUpdate.Name = "cmdADUpdate";
			this.cmdADUpdate.Size = new System.Drawing.Size(96, 23);
			this.cmdADUpdate.TabIndex = 24;
			this.cmdADUpdate.Text = "Test Update";
			this.cmdADUpdate.Click += new System.EventHandler(this.cmdADUpdate_Click);
			// 
			// cmdADDelete
			// 
			this.cmdADDelete.Location = new System.Drawing.Point(240, 264);
			this.cmdADDelete.Name = "cmdADDelete";
			this.cmdADDelete.Size = new System.Drawing.Size(96, 23);
			this.cmdADDelete.TabIndex = 25;
			this.cmdADDelete.Text = "Test Delete";
			this.cmdADDelete.Click += new System.EventHandler(this.cmdADDelete_Click);
			// 
			// cmdADRetrieve
			// 
			this.cmdADRetrieve.Location = new System.Drawing.Point(352, 264);
			this.cmdADRetrieve.Name = "cmdADRetrieve";
			this.cmdADRetrieve.Size = new System.Drawing.Size(96, 23);
			this.cmdADRetrieve.TabIndex = 26;
			this.cmdADRetrieve.Text = "Test Retrieve";
			this.cmdADRetrieve.Click += new System.EventHandler(this.cmdADRetrieve_Click);
			// 
			// frmBulkTool
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(464, 303);
			this.Controls.Add(this.cmdADRetrieve);
			this.Controls.Add(this.cmdADDelete);
			this.Controls.Add(this.cmdADUpdate);
			this.Controls.Add(this.txtDirectorySuffix);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.txtEndTime);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtStartTime);
			this.Controls.Add(this.txtEndNumber);
			this.Controls.Add(this.txtNumber);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.cmdADTest);
			this.Controls.Add(this.cmdMore);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.cmdStart);
			this.Controls.Add(this.cmdOpen);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmBulkTool";
			this.Text = "AD Bulk Tool";
			this.Load += new System.EventHandler(this.frmBulkTool_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmComputerBulk());
		}

		private void cmdOpen_Click(object sender, System.EventArgs e)
		{
			DialogResult dr = openFileDialog1.ShowDialog();
			if (dr == DialogResult.OK)
			{
				txtFile.Text = openFileDialog1.FileName;
			}
		}

		private void cmdStart_Click(object sender, System.EventArgs e)
		{
			if (cmdStart.Text == "Start")
			{
				cmdStart.Text = "Stop";

				GoAD();
				cmdStart.Text = "Exit";
			}
			else if (cmdStart.Text == "Exit")
			{
				Application.Exit();
			}
			else
			{
				cmdStart.Text = "Start";
			}
		}

		private void GoAD()
		{
			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			string SERVER_NAME = txtServer.Text;
			string DIRECTORY_SUFFIX = txtDirectorySuffix.Text;

			using (DirectoryEntry de = new DirectoryEntry())
			{
				lblStatus.Text = "Connecting to Active Directory...";
				lblStatus.Refresh();

				de.Path = "LDAP://" + SERVER_NAME + "/ou=Members," + DIRECTORY_SUFFIX;
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users," + DIRECTORY_SUFFIX;
				de.Password = "netsserver1@";

				lblStatus.Text = "Reading the file...";
				lblStatus.Refresh();

				FileStream fs = new FileStream(txtFile.Text, 
					FileMode.Open, FileAccess.Read, FileShare.Read);
				StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
			
				lblStatus.Text = "Getting attribute names...";
				lblStatus.Refresh();

				// get Attribute Names
				String sLine = "uno|cn|upwd|displayName|resRegNum" 
					+ "|mail|ugrade|uprofile|uzipcode|uaddr"
					+ "|uregdate";
				String[] arrAttrib = getAttributes(sLine);

				lblStatus.Text = "Ready for insert...";
				lblStatus.Refresh();

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				// insert all attributes from file to Data Source
				sLine = sr.ReadLine();
				int niCount = 0;
				while ((sLine != null) && (cmdStart.Text == "Stop"))
				{
					String[] arrTemp = getAttributes(sLine);
					if (arrTemp.Length != arrAttrib.Length)
					{
						sLine = sr.ReadLine();
						String[] arrTemp2 = getAttributes(sLine);
						arrTemp[arrTemp.Length - 1] += arrTemp2[0];
						String[] arrTemp3 = new String[arrAttrib.Length];

						Array.Copy(arrTemp, 0, arrTemp3, 0, arrTemp.Length);
						Array.Copy(arrTemp2, 1, arrTemp3, arrTemp.Length, arrTemp2.Length - 1);
						arrTemp = arrTemp3;
						arrTemp2 = null;
						arrTemp3 = null;

						if (arrTemp.Length != arrAttrib.Length)
						{
							lblStatus.Text += "Invalid format!";
							break;
						}
					}

					try
					{
						using (DirectoryEntry user = de.Children.Add("cn=" + arrTemp[1], SCHEMA_CLASS_NAME))
						{
							for (int i = 0; i < arrTemp.Length; i++)
							{
								if (arrTemp[i].Trim().Length > 0)
									user.Properties[arrAttrib[i]].Value = arrTemp[i].Trim();
							}
							user.CommitChanges();
							user.Close();
						}

						arrTemp = null;
						sLine = sr.ReadLine();
						niCount++;

						if ( (niCount != 0) && (niCount % 10) == 0 )
						{
							lblStatus.Text = niCount.ToString() + " record(s) is inserted...";
							lblStatus.Refresh();
							Application.DoEvents();
						}
					}
					catch (System.Runtime.InteropServices.COMException cEx)
					{
						if (cEx.ErrorCode == -2147019886) // Already exists
						{
							arrTemp = null;
							sLine = sr.ReadLine();
							niCount++;
						}
						else if (cEx.ErrorCode == -2147016646)	// Server down
						{
							de.Close();

							de.Path = "LDAP://" + SERVER_NAME + "/ou=Members," + DIRECTORY_SUFFIX;
							Random rr = new Random();
							de.Username = "cn=admin" + rr.Next(1, 20).ToString() + ",cn=Users," + DIRECTORY_SUFFIX;
							de.Password = "netsserver1@";
						}
					}
					catch (Exception ex)
					{
						FileInfo fiLog = new FileInfo(Application.StartupPath 
							+ "\\" + txtFile.Text.Substring(txtFile.Text.IndexOf("_L"), 4) + ".err");
						FileStream fsLog = fiLog.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
						StreamWriter swLog = new StreamWriter(fsLog);
						swLog.WriteLine(arrTemp[1] + " : " 
							+ ex.ToString().Replace("\n", "\\n").Replace("\r", "\\r") 
							+ " (" + ex.Message.Replace("\r", "\\r").Replace("\n", "\\n") + ")");
						swLog.Close();
						fsLog.Close();
					}
				}
				lblStatus.Text = niCount.ToString() + " record(s) is inserted.";
				lblStatus.Refresh();
				sr.Close();
				fs.Close();

				de.Close();

				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtEndTime.Refresh();
			}

			lblStatus.Text += "\r\nCompleted.";
			lblStatus.Refresh();
		}

		private String[] getAttributes(String sLine)
		{
			String[] arrTemp = sLine.Split(new char[] {'|'});
			return arrTemp;
		}

		private void cmdMore_Click(object sender, System.EventArgs e)
		{
			if (cmdMore.Text == "More >>")
			{
				cmdMore.Text = "More <<";

				this.Size = new Size(472, 328);
			}
			else
			{
				cmdMore.Text = "More >>";

				this.Size = new Size(472, 248);
			}
		}

		private void cmdADTest_Click(object sender, System.EventArgs e)
		{
			ResetTime();
			GoAD2Input();
			CalculateTime();
		}

		private void GoAD2Input()
		{
			string SERVER_NAME = txtServer.Text;
			string DIRECTORY_SUFFIX = txtDirectorySuffix.Text;

			if (cmdADTest.Text == "Stop")
			{
				cmdADTest.Text = "Test Input";
				txtNumber.ReadOnly = false;
				txtEndNumber.ReadOnly = false;
				return;
			}

			cmdADTest.Text = "Stop";
			txtNumber.ReadOnly = true;
			txtEndNumber.ReadOnly = true;

			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			using (DirectoryEntry de = new DirectoryEntry())
			{
				lblStatus.Text = "Connecting to Active Directory...\r\n\r\n";
				lblStatus.Refresh();

				de.Path = "LDAP://" + SERVER_NAME + "/ou=Members," + DIRECTORY_SUFFIX;
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users," + DIRECTORY_SUFFIX;
				de.Password = "netsserver1@";

				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				// get Attribute Names
				String sLine = "uno|cn|upwd|displayName|resRegNum" 
					+ "|mail|ugrade|uprofile|uzipcode|uaddr"
					+ "|uregdate";
				String[] arrAttrib = getAttributes(sLine);

				lblStatus.Text = "Ready for insert...";
				lblStatus.Refresh();

				// insert all attributes from file to Data Source
				sLine = "|nu|pw|테스터|83051" 
					+ "|@bmt.com|01|테스터|135712|서울특별시 강남구 대치4동 샹제리제센터 B동 204호" 
					+ "|";
				//sLine = "test||||" 
				//	+ "||테스터|test@magicn.com||" 
				//	+ "||||||" 
				//	+ "|||0161|test";

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
				int nEnd = int.Parse(txtEndNumber.Text);
				while ((sLine != null) && (cmdADTest.Text == "Stop") && (nCnt < nEnd))
				{
					String[] arrTemp = getAttributes(sLine);

					arrTemp[0] = String.Format("{0:00000000}", ++nCnt);	// uno
					arrTemp[1] += String.Format("{0:00000000}", nCnt);	// cn
					arrTemp[2] += String.Format("{0:00000000}", nCnt);	// upwd
					arrTemp[4] += String.Format("{0:00000000}", nCnt + 1000000);	// resRegNum
					arrTemp[5] = arrTemp[1] + arrTemp[5];	// mail
					arrTemp[7] += String.Format("{0:00000000}", nCnt) + "의 프로필입니다.";	// uprofile
					arrTemp[10] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");	// uregdate
					
					try
					{
						using (DirectoryEntry user = de.Children.Add("cn=" + arrTemp[1], SCHEMA_CLASS_NAME))
						{
							for (int i = 0; i < arrTemp.Length; i++)
							{
								if (arrTemp[i].Trim().Length > 0)
									user.Properties[arrAttrib[i]].Value = arrTemp[i].Trim();
							}
							user.CommitChanges();
							user.Close();
						}
					}
					catch (System.Runtime.InteropServices.COMException ex)
					{
						lblStatus.Text = "error[0x" + ex.ErrorCode.ToString("X") + "]: " + ex.Message;
						lblStatus.Text += "\r\n[cn=" + arrTemp[1] + "]";
						lblStatus.Refresh();
						de.Close();
						return;
					}

					arrTemp = null;
					niCount++;

					if ( (niCount != 0) && (niCount % 10) == 0 )
					{
						lblStatus.Text = niCount.ToString() + " record(s) inserted...";
						lblStatus.Refresh();
						Application.DoEvents();
					}
				}// end of while
				lblStatus.Text = niCount.ToString() + " record(s) inserted.";
				lblStatus.Refresh();

				de.Close();

				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtEndTime.Refresh();
			}// end of using de

			lblStatus.Text += "\r\nCompleted.";
			lblStatus.Refresh();

			txtNumber.ReadOnly = false;
			txtEndNumber.ReadOnly = false;
			cmdADTest.Text = "Test Input";
		}

		private void frmBulkTool_Load(object sender, System.EventArgs e)
		{
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				String[] sUsers = Environment.GetCommandLineArgs()[1].Split(new char[] {'|'});
				if ((sUsers.Length == 2) && (sUsers[0].ToUpper().Trim() == "FILES"))
				{
					txtFile.Text = sUsers[1];

					cmdMore.Text = "More >>";
					this.Size = new Size(472, 248);

					this.Show();

					cmdStart.Text = "Stop";
					cmdStart.Focus();

					this.WindowState = FormWindowState.Minimized;

					GoAD();
					cmdStart.Text = "Exit";
				}
				else if (sUsers.Length == 3)
				{
					txtNumber.Text = sUsers[1];
					txtEndNumber.Text = sUsers[2];

					cmdMore.Text = "More <<";
					this.Size = new Size(472, 328);

					this.Show();
					cmdADTest.Focus();

					this.WindowState = FormWindowState.Normal;

					string sOption = sUsers[0];
					switch (sOption)
					{
						case "C":
							ResetTime();
							GoAD2Input();
							CalculateTime();
							break;
						case "U":
							ResetTime();
							GoAD2Update();
							CalculateTime();
							break;
						case "D":
							ResetTime();
							GoAD2Delete();
							CalculateTime();
							break;
						case "S":
							ResetTime();
							GoAD2Retrieve();
							CalculateTime();
							break;
					}
				}
			}
		}

		private void GoAD2Update()
		{
			string SERVER_NAME = txtServer.Text;
			string DIRECTORY_SUFFIX = txtDirectorySuffix.Text;

			if (cmdADUpdate.Text == "Stop")
			{
				cmdADUpdate.Text = "Test Update";
				txtNumber.ReadOnly = false;
				txtEndNumber.ReadOnly = false;
				return;
			}

			cmdADUpdate.Text = "Stop";
			txtNumber.ReadOnly = true;
			txtEndNumber.ReadOnly = true;

			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			using (DirectoryEntry de = new DirectoryEntry())
			{
				lblStatus.Text = "Connecting to Active Directory...\r\n\r\n";
				lblStatus.Refresh();

				de.Path = "LDAP://" + SERVER_NAME + "/ou=Members," + DIRECTORY_SUFFIX;
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users," + DIRECTORY_SUFFIX;
				de.Password = "netsserver1@";

				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				// get Attribute Names
				String sLine = "cn|uprofile";
				String[] arrAttrib = getAttributes(sLine);

				lblStatus.Text = "Ready for update...";
				lblStatus.Refresh();

				// update uprofile
				sLine = "nu|테스터";

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
				int nEnd = int.Parse(txtEndNumber.Text);
				while ((sLine != null) && (cmdADUpdate.Text == "Stop") && (nCnt < nEnd))
				{
					String[] arrTemp = getAttributes(sLine);

					arrTemp[0] += String.Format("{0:00000000}", ++nCnt);	// cn
					arrTemp[1] += String.Format("{0:00000000}", nCnt) + "의 프로필입니다.";	// uprofile
					
					try
					{
						using (DirectoryEntry user = de.Children.Find("cn=" + arrTemp[0], SCHEMA_CLASS_NAME))
						{
							for (int i = 1; i < arrTemp.Length; i++)
							{
								if (arrTemp[i].Trim().Length > 0)
									user.Properties[arrAttrib[i]].Value = arrTemp[i].Trim();
							}
							user.CommitChanges();
							user.Close();
						}
					}
					catch (System.Runtime.InteropServices.COMException ex)
					{
						lblStatus.Text = "error[0x" + ex.ErrorCode.ToString("X") + "]: " + ex.Message;
						lblStatus.Text += "\r\n[cn=" + arrTemp[0] + "]";
						lblStatus.Refresh();
						de.Close();
						return;
					}

					arrTemp = null;
					niCount++;

					if ( (niCount != 0) && (niCount % 10) == 0 )
					{
						lblStatus.Text = niCount.ToString() + " record(s) updated...";
						lblStatus.Refresh();
						Application.DoEvents();
					}
				}// end of while
				lblStatus.Text = niCount.ToString() + " record(s) updated.";
				lblStatus.Refresh();

				de.Close();

				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtEndTime.Refresh();
			}// end of using de

			lblStatus.Text += "\r\nCompleted.";
			lblStatus.Refresh();

			txtNumber.ReadOnly = false;
			txtEndNumber.ReadOnly = false;
			cmdADUpdate.Text = "Test Update";
		}

		private void cmdADUpdate_Click(object sender, System.EventArgs e)
		{
			ResetTime();
			GoAD2Update();
			CalculateTime();
		}

		private void GoAD2Delete()
		{
			string SERVER_NAME = txtServer.Text;
			string DIRECTORY_SUFFIX = txtDirectorySuffix.Text;

			if (cmdADDelete.Text == "Stop")
			{
				cmdADDelete.Text = "Test Delete";
				txtNumber.ReadOnly = false;
				txtEndNumber.ReadOnly = false;
				return;
			}

			cmdADDelete.Text = "Stop";
			txtNumber.ReadOnly = true;
			txtEndNumber.ReadOnly = true;

			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			using (DirectoryEntry de = new DirectoryEntry())
			{
				lblStatus.Text = "Connecting to Active Directory...\r\n\r\n";
				lblStatus.Refresh();

				de.Path = "LDAP://" + SERVER_NAME + "/ou=Members," + DIRECTORY_SUFFIX;
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users," + DIRECTORY_SUFFIX;
				de.Password = "netsserver1@";

				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				// get Attribute Names
				String sLine = "cn";
				String[] arrAttrib = getAttributes(sLine);

				lblStatus.Text = "Ready for delete...";
				lblStatus.Refresh();

				// delete cn
				sLine = "nu";

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
				int nEnd = int.Parse(txtEndNumber.Text);
				while ((sLine != null) && (cmdADDelete.Text == "Stop") && (nCnt < nEnd))
				{
					String[] arrTemp = getAttributes(sLine);

					arrTemp[0] += String.Format("{0:00000000}", ++nCnt);	// cn
					
					try
					{
						using (DirectoryEntry user = de.Children.Find("cn=" + arrTemp[0], SCHEMA_CLASS_NAME))
						{
							de.Children.Remove(user);
						}
					}
					catch (System.Runtime.InteropServices.COMException ex)
					{
						lblStatus.Text = "error[0x" + ex.ErrorCode.ToString("X") + "]: " + ex.Message;
						lblStatus.Text += "\r\n[cn=" + arrTemp[0] + "]";
						lblStatus.Refresh();
						de.Close();
						return;
					}

					arrTemp = null;
					niCount++;

					if ( (niCount != 0) && (niCount % 10) == 0 )
					{
						lblStatus.Text = niCount.ToString() + " record(s) deleted...";
						lblStatus.Refresh();
						Application.DoEvents();
					}
				}// end of while
				lblStatus.Text = niCount.ToString() + " record(s) deleted.";
				lblStatus.Refresh();

				de.Close();

				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtEndTime.Refresh();
			}// end of using de

			lblStatus.Text += "\r\nCompleted.";
			lblStatus.Refresh();

			txtNumber.ReadOnly = false;
			txtEndNumber.ReadOnly = false;
			cmdADDelete.Text = "Test Delete";
		}

		private void cmdADDelete_Click(object sender, System.EventArgs e)
		{
			ResetTime();
			GoAD2Delete();
			CalculateTime();
		}

		private void GoAD2Retrieve()
		{
			string SERVER_NAME = txtServer.Text;
			string DIRECTORY_SUFFIX = txtDirectorySuffix.Text;

			if (cmdADRetrieve.Text == "Stop")
			{
				cmdADRetrieve.Text = "Test Retrieve";
				txtNumber.ReadOnly = false;
				txtEndNumber.ReadOnly = false;
				return;
			}

			cmdADRetrieve.Text = "Stop";
			txtNumber.ReadOnly = true;
			txtEndNumber.ReadOnly = true;

			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			using (DirectoryEntry de = new DirectoryEntry())
			{
				lblStatus.Text = "Connecting to Active Directory...\r\n\r\n";
				lblStatus.Refresh();

				de.Path = "LDAP://" + SERVER_NAME + "/ou=Members," + DIRECTORY_SUFFIX;
				Random r = new Random();
				de.Username = "cn=admin" + r.Next(1, 20).ToString() + ",cn=Users," + DIRECTORY_SUFFIX;
				de.Password = "netsserver1@";

				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				// get Attribute Names
				String sLine = "cn|upwd";
				String[] arrAttrib = getAttributes(sLine);

				lblStatus.Text = "Ready for retrieve...";
				lblStatus.Refresh();

				// retrieve upwd
				sLine = "nu|";

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
				int nEnd = int.Parse(txtEndNumber.Text);
				while ((sLine != null) && (cmdADRetrieve.Text == "Stop") && (nCnt < nEnd))
				{
					String[] arrTemp = getAttributes(sLine);

					arrTemp[0] += String.Format("{0:00000000}", ++nCnt);	// cn
					arrTemp[1] = "";
					
					try
					{
						using (DirectoryEntry user = de.Children.Find("cn=" + arrTemp[0], SCHEMA_CLASS_NAME))
						{
							for (int i = 1; i < arrTemp.Length; i++)
							{
								arrTemp[i] = (string) user.Properties[arrAttrib[i]].Value;
							}
							user.Close();
						}
					}
					catch (System.Runtime.InteropServices.COMException ex)
					{
						lblStatus.Text = "error[0x" + ex.ErrorCode.ToString("X") + "]: " + ex.Message;
						lblStatus.Text += "\r\n[cn=" + arrTemp[0] + "]";
						lblStatus.Refresh();
						de.Close();
						return;
					}

					arrTemp = null;
					niCount++;

					if ( (niCount != 0) && (niCount % 10) == 0 )
					{
						lblStatus.Text = niCount.ToString() + " record(s) retrieved...";
						lblStatus.Refresh();
						Application.DoEvents();
					}
				}// end of while
				lblStatus.Text = niCount.ToString() + " record(s) retrieved.";
				lblStatus.Refresh();

				de.Close();

				txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtEndTime.Refresh();
			}// end of using de

			lblStatus.Text += "\r\nCompleted.";
			lblStatus.Refresh();

			txtNumber.ReadOnly = false;
			txtEndNumber.ReadOnly = false;
			cmdADRetrieve.Text = "Test Retrieve";
		}

		private void cmdADRetrieve_Click(object sender, System.EventArgs e)
		{
			ResetTime();
			GoAD2Retrieve();
			CalculateTime();
		}

		private void ResetTime()
		{
			txtStartTime.Text = "";
			txtEndTime.Text = "";
			lblStatus.Text = "";
		}

		private void CalculateTime()
		{
			if ((txtStartTime.Text.Trim().Length > 0) && (txtEndTime.Text.Trim().Length > 0))
			{
				DateTime dtStart = DateTime.Parse(txtStartTime.Text);
				DateTime dtEnd = DateTime.Parse(txtEndTime.Text);
				TimeSpan elapsed = dtEnd.Subtract(dtStart);
			
				lblStatus.Text += "\r\nElapsed: " + elapsed.TotalSeconds.ToString() + " sec. (" +
					elapsed.Hours.ToString("00") + ":" + elapsed.Minutes.ToString("00") + ":" + 
					elapsed.Seconds.ToString("00") + ")";
			}
		}
	}
}
