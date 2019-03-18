using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BulkJob
{
	/// <summary>
	/// frmComputerBulk에 대한 요약 설명입니다.
	/// </summary>
	public class frmComputerBulk : Form
	{
		private string SCHEMA_CLASS_NAME = "computer";

		private Label label1;
		private Label lblStatus;
		private Button cmdMore;
		private Button cmdADTest;
		private Label label2;
		private TextBox txtNumber;
		private Label label3;
		private TextBox txtEndNumber;
		private TextBox txtEndTime;
		private Label label4;
		private TextBox txtStartTime;
		private Label label5;
		private TextBox txtDirectorySuffix;
		private Label label6;
		private Label label7;
		private TextBox txtServer;
		private Button cmdADDelete;
		private Button cmdADRetrieve;
		private TextBox txtOU;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private Container components = null;

		public frmComputerBulk()
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
			this.txtOU = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
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
			this.cmdADDelete = new System.Windows.Forms.Button();
			this.cmdADRetrieve = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtOU
			// 
			this.txtOU.BackColor = System.Drawing.Color.White;
			this.txtOU.Location = new System.Drawing.Point(128, 48);
			this.txtOU.Name = "txtOU";
			this.txtOU.Size = new System.Drawing.Size(120, 21);
			this.txtOU.TabIndex = 3;
			this.txtOU.Text = "ou=cpTestOU";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "OU to process :";
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
			this.txtDirectorySuffix.Text = "dc=gusslab,dc=net";
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
			this.txtServer.Text = "guss";
			// 
			// cmdADDelete
			// 
			this.cmdADDelete.Location = new System.Drawing.Point(224, 264);
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
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(120, 264);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(96, 23);
			this.button1.TabIndex = 25;
			this.button1.Text = "Test Update";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// frmComputerBulk
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(464, 303);
			this.Controls.Add(this.cmdADRetrieve);
			this.Controls.Add(this.cmdADDelete);
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
			this.Controls.Add(this.txtOU);
			this.Controls.Add(this.cmdADTest);
			this.Controls.Add(this.cmdMore);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmComputerBulk";
			this.Text = "AD Bulk Tool";
			this.Load += new System.EventHandler(this.frmComputerBulk_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdMore_Click(object sender, EventArgs e)
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

		private void cmdADTest_Click(object sender, EventArgs e)
		{
			ResetTime();
			GoAD2Input();
			CalculateTime();
		}

		private DirectoryEntry getBindInfo()
		{
			string SERVER_NAME = txtServer.Text;
			string DIRECTORY_SUFFIX = txtDirectorySuffix.Text;
			string PROC_OU = (txtOU.Text.Length) > 0 ? txtOU.Text + "," : "";

			lblStatus.Text = "Connecting to Active Directory...\r\n\r\n";
			lblStatus.Refresh();

			DirectoryEntry de = new DirectoryEntry();
			
			lblStatus.Text = "Connecting to Active Directory...\r\n\r\n";
			lblStatus.Refresh();

			de.Path = "LDAP://" + SERVER_NAME + "/" + PROC_OU + DIRECTORY_SUFFIX;
			de.Username = "cn=administrator,cn=Users," + DIRECTORY_SUFFIX;
			de.Password = "dusrnthekffu!";
			de.AuthenticationType =AuthenticationTypes.ServerBind;

			return de;
		}

		private void GoAD2Input()
		{
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

			using (DirectoryEntry de = getBindInfo())
			{
				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				lblStatus.Text = "Ready for insert...";
				lblStatus.Refresh();

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
				int nEnd = int.Parse(txtEndNumber.Text);
				while ((cmdADTest.Text == "Stop") && (nCnt < nEnd))
				{
					string[] arrTemp = new string[3];

					arrTemp[0] = String.Format("computer{0}", ++nCnt);	// cn
					arrTemp[1] = String.Format("컴퓨터{0}", nCnt);	// displayName
					arrTemp[2] = String.Format("computer{0}$", nCnt);	// sAMAccountName
					
					try
					{
						using (DirectoryEntry user = de.Children.Add("cn=" + arrTemp[0], SCHEMA_CLASS_NAME))
						{
							user.Properties["displayName"].Value = arrTemp[1].Trim();
							user.Properties["sAMAccountName"].Value = arrTemp[2].Trim();
							user.CommitChanges();
							user.Close();
						}
					}
					catch (COMException ex)
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

		private void frmComputerBulk_Load(object sender, EventArgs e)
		{
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				String[] sUsers = Environment.GetCommandLineArgs()[1].Split(new char[] {'|'});
				if (sUsers.Length == 3)
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

		private void GoAD2Delete()
		{
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

			using (DirectoryEntry de = getBindInfo())
			{
				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				lblStatus.Text = "Ready for delete...";
				lblStatus.Refresh();

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
				int nEnd = int.Parse(txtEndNumber.Text);
				while ((cmdADDelete.Text == "Stop") && (nCnt < nEnd))
				{
					string[] arrTemp = new string[1];

					arrTemp[0] = String.Format("computer{0}", ++nCnt);	// cn
					
					try
					{
						DirectoryEntry user = de.Children.Find("cn=" + arrTemp[0], SCHEMA_CLASS_NAME);
						de.Children.Remove(user);
					}
					catch (COMException ex)
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

		private void cmdADDelete_Click(object sender, EventArgs e)
		{
			ResetTime();
			GoAD2Delete();
			CalculateTime();
		}

		private void GoAD2Retrieve()
		{
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

			using (DirectoryEntry de = getBindInfo())
			{
				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				DirectorySearcher searcher = new DirectorySearcher(de);
				searcher.Filter = "(&(objectclass=" + SCHEMA_CLASS_NAME + ")(modifyTimeStamp>=" + toUTCTimeString(DateTime.Parse("2005-12-13 12:00:00")) + "))";
				searcher.PropertiesToLoad.Add("sAMAccountName");
				searcher.SearchScope = SearchScope.Subtree;
				searcher.PageSize = 10000;
				searcher.CacheResults = false;
				searcher.ReferralChasing = ReferralChasingOption.None;

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0;

				SearchResultCollection Result = searcher.FindAll();
				foreach (SearchResult result in Result)
				{
					string s = getADProperty(result, "sAMAccountName").Replace("$","");

					niCount++;

					if ( (niCount != 0) && (niCount % 500) == 0 )	// 500개마다 표시
					{
						string path = result.GetDirectoryEntry().Path;
						lblStatus.Text = niCount.ToString() + " record(s) retrieved...";
						lblStatus.Text += "\r\nPeek:[" + path.Substring(path.ToLower().IndexOf("cn")) + "]";
						lblStatus.Refresh();
						Application.DoEvents();
					}
				}
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

		private string toUTCTimeString(DateTime date)
		{
			return date.ToUniversalTime().ToString("yyyyMMddHHmmss.0Z");
		}

		private void GoAD2Update()
		{
			if (button1.Text == "Stop")
			{
				button1.Text = "Test Update";
				txtNumber.ReadOnly = false;
				txtEndNumber.ReadOnly = false;
				return;
			}

			button1.Text = "Stop";
			txtNumber.ReadOnly = true;
			txtEndNumber.ReadOnly = true;

			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			using (DirectoryEntry de = getBindInfo())
			{
				lblStatus.Text = "Getting original data...";
				lblStatus.Refresh();

				DirectorySearcher searcher = new DirectorySearcher(de);
				searcher.Filter = "objectclass=" + SCHEMA_CLASS_NAME;
				searcher.PropertiesToLoad.Add("sAMAccountName");
				searcher.SearchScope = SearchScope.Subtree;
				searcher.PageSize = 10000;
				searcher.CacheResults = false;
				searcher.ReferralChasing = ReferralChasingOption.None;

				txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				txtStartTime.Refresh();

				int niCount = 0;

				//System.Diagnostics.Debug.WriteLine(searcher.PageSize);
				//System.Diagnostics.Debug.WriteLine(searcher.SizeLimit);

				SearchResultCollection Result = searcher.FindAll();

				//System.Diagnostics.Debug.WriteLine(Result.Count);

				foreach (SearchResult result in Result)
				{
					DirectoryEntry computer = result.GetDirectoryEntry();
					computer.Properties["userAccountControl"].Value = 0x1020;
					computer.CommitChanges();

					niCount++;

					if ( (niCount != 0) && (niCount % 10) == 0 )	// 10개마다 표시
					{
						string path = result.GetDirectoryEntry().Path;
						lblStatus.Text = niCount.ToString() + " record(s) updated...";
						lblStatus.Text += "\r\nPeek:[" + path.Substring(path.ToLower().IndexOf("cn")) + "]";
						lblStatus.Refresh();
						Application.DoEvents();
					}
				}
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
			button1.Text = "Test Update";
		}

		private string getADProperty(SearchResult result, string prop)
		{
			ResultPropertyValueCollection attr = result.Properties[prop];
			try
			{
				if ((attr != null) && (attr[0] != null))
					return attr[0].ToString();
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return "";
		}

		private void cmdADRetrieve_Click(object sender, EventArgs e)
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

		private void button1_Click(object sender, System.EventArgs e)
		{
			ResetTime();
			GoAD2Update();
			CalculateTime();
		}
	}
}
