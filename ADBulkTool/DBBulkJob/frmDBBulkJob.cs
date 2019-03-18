using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace DBBulkJob
{
	/// <summary>
	/// frmDBBulkJob에 대한 요약 설명입니다.
	/// </summary>
	public class frmDBBulkJob : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtEndNumber;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button cmdStart;
		private System.Windows.Forms.TextBox txtConn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtUID;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtPWD;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtTable;
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmDBBulkJob()
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
			this.txtEndNumber = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtNumber = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.cmdStart = new System.Windows.Forms.Button();
			this.txtConn = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtUID = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtPWD = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtTable = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtEndNumber
			// 
			this.txtEndNumber.BackColor = System.Drawing.Color.White;
			this.txtEndNumber.Location = new System.Drawing.Point(304, 192);
			this.txtEndNumber.Name = "txtEndNumber";
			this.txtEndNumber.Size = new System.Drawing.Size(72, 21);
			this.txtEndNumber.TabIndex = 21;
			this.txtEndNumber.Text = "20000000";
			this.txtEndNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(224, 197);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 14);
			this.label3.TabIndex = 20;
			this.label3.Text = "End number:";
			// 
			// txtNumber
			// 
			this.txtNumber.BackColor = System.Drawing.Color.White;
			this.txtNumber.Location = new System.Drawing.Point(120, 192);
			this.txtNumber.Name = "txtNumber";
			this.txtNumber.Size = new System.Drawing.Size(72, 21);
			this.txtNumber.TabIndex = 19;
			this.txtNumber.Text = "1";
			this.txtNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(40, 197);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 14);
			this.label2.TabIndex = 18;
			this.label2.Text = "Start number:";
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(8, 104);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(448, 64);
			this.lblStatus.TabIndex = 17;
			// 
			// cmdStart
			// 
			this.cmdStart.Location = new System.Drawing.Point(200, 232);
			this.cmdStart.Name = "cmdStart";
			this.cmdStart.TabIndex = 16;
			this.cmdStart.Text = "Start";
			this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
			// 
			// txtConn
			// 
			this.txtConn.BackColor = System.Drawing.Color.White;
			this.txtConn.Location = new System.Drawing.Point(96, 11);
			this.txtConn.Name = "txtConn";
			this.txtConn.Size = new System.Drawing.Size(359, 21);
			this.txtConn.TabIndex = 22;
			this.txtConn.Text = "Data Source=netsmms;Initial Catalog=NetmarbleUser";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 14);
			this.label1.TabIndex = 23;
			this.label1.Text = "Connection:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(49, 45);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 14);
			this.label4.TabIndex = 25;
			this.label4.Text = "UserID:";
			// 
			// txtUID
			// 
			this.txtUID.BackColor = System.Drawing.Color.White;
			this.txtUID.Location = new System.Drawing.Point(96, 40);
			this.txtUID.Name = "txtUID";
			this.txtUID.Size = new System.Drawing.Size(98, 21);
			this.txtUID.TabIndex = 24;
			this.txtUID.Text = "sa";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(208, 45);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 14);
			this.label5.TabIndex = 27;
			this.label5.Text = "Passwd:";
			// 
			// txtPWD
			// 
			this.txtPWD.BackColor = System.Drawing.Color.White;
			this.txtPWD.Location = new System.Drawing.Point(264, 40);
			this.txtPWD.Name = "txtPWD";
			this.txtPWD.PasswordChar = '*';
			this.txtPWD.Size = new System.Drawing.Size(98, 21);
			this.txtPWD.TabIndex = 26;
			this.txtPWD.Text = "sptcmdhk";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(5, 73);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(91, 14);
			this.label6.TabIndex = 29;
			this.label6.Text = "Table to insert:";
			// 
			// txtTable
			// 
			this.txtTable.BackColor = System.Drawing.Color.White;
			this.txtTable.Location = new System.Drawing.Point(96, 69);
			this.txtTable.Name = "txtTable";
			this.txtTable.Size = new System.Drawing.Size(98, 21);
			this.txtTable.TabIndex = 28;
			this.txtTable.Text = "members";
			// 
			// frmDBBulkJob
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(464, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label6,
																		  this.label5,
																		  this.label4,
																		  this.label1,
																		  this.label3,
																		  this.label2,
																		  this.txtTable,
																		  this.txtPWD,
																		  this.txtUID,
																		  this.txtConn,
																		  this.txtEndNumber,
																		  this.txtNumber,
																		  this.lblStatus,
																		  this.cmdStart});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmDBBulkJob";
			this.Text = "DBBulkJob";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmDBBulkJob());
		}

		private void cmdStart_Click(object sender, System.EventArgs e)
		{
			if (cmdStart.Text == "Stop")
			{
				cmdStart.Text = "Start";
				txtConn.ReadOnly = false;
				txtUID.ReadOnly = false;
				txtPWD.ReadOnly = false;
				txtTable.ReadOnly = false;
				txtNumber.ReadOnly = false;
				txtEndNumber.ReadOnly = false;
				return;
			}
			else if (cmdStart.Text == "End")
			{
				Application.Exit();
				return;
			}
			cmdStart.Text = "Stop";
			txtConn.ReadOnly = true;
			txtUID.ReadOnly = true;
			txtPWD.ReadOnly = true;
			txtTable.ReadOnly = true;
			txtNumber.ReadOnly = true;
			txtEndNumber.ReadOnly = true;

			lblStatus.Text = "Initializing...";
			lblStatus.Refresh();

			// @, *, |, \ : can not in AD
			// % : can not in DB
			String[] sSpecial = {"!", "#", "~", "^", "&", "."};

			using (SqlConnection conn = new SqlConnection())
			{
				lblStatus.Text = "Connecting to UserDB...";
				lblStatus.Refresh();

				conn.ConnectionString = txtConn.Text 
					+ ";User ID=" + txtUID.Text + ";Password=" + txtPWD.Text;
				conn.Open();

				using (SqlCommand comm = new SqlCommand())
				{
					comm.Connection = conn;
					comm.CommandType = CommandType.Text;

					lblStatus.Text = "Getting original data...";
					lblStatus.Refresh();

					// get Attribute Names
					String sLine = "uno|userid|upwd|uname|uregnum" 
						+ "|umail|ugrade|uprofile|uzipcode|uaddr"
						+ "|uregdate";

					String[] arrAttrib = getAttributes(sLine);
					String sNames = "";
					for (int i = 0; i < arrAttrib.Length; i++)
					{
						if (sNames.Length > 0)
							sNames += ", " + arrAttrib[i];
						else
							sNames = arrAttrib[i];
					}

					lblStatus.Text = "Ready for insert...";
					lblStatus.Refresh();

					// insert all attributes from file to Data Source
					sLine = "|nu|pw|테스터|83051" 
						+ "|@netmarble.net|01|테스터|135712|서울특별시 강남구 대치4동 샹제리제센터 B동 204호" 
						+ "|";

					int nCount = 0, nCnt = int.Parse(txtNumber.Text) - 1;
					int nEnd = int.Parse(txtEndNumber.Text);
					while ((sLine != null) && (cmdStart.Text == "Stop") && (nCnt < nEnd))
					{
						String[] arrTemp = getAttributes(sLine);

						arrTemp[0] = String.Format("{0:00000000}", ++nCnt);	// uno
						Random r = new Random();
						arrTemp[1] += sSpecial[r.Next(0, 5)]
							+ String.Format("{0:00000000}", nCnt) + sSpecial[r.Next(0, 4)];	// userid
						arrTemp[2] += String.Format("{0:00000000}", nCnt);	// upwd
						arrTemp[4] += String.Format("{0:00000000}", nCnt + 1000000);	// uregnum
						arrTemp[5] = arrTemp[1] + arrTemp[5];	// umail
						arrTemp[7] += String.Format("{0:00000000}", nCnt) + "의 프로필입니다.";	// uprofile
						arrTemp[10] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");	// uregdate

						String sValues = "";
						for (int i = 0; i < arrTemp.Length; i++)
						{
							if (sValues.Length > 0)
								sValues += ", '" + arrTemp[i].Trim() + "'";
							else
								sValues = "'" + arrTemp[i].Trim() + "'";
						}

						comm.CommandText = String.Format("INSERT INTO {0}({1})\n"
							+ "VALUES({2})", txtTable.Text, sNames, sValues);
						comm.ExecuteNonQuery();

						arrTemp = null;
						nCount++;

						if ( (nCount != 0) && (nCount % 100) == 0 )
						{
							lblStatus.Text = nCount.ToString() + " record(s) is inserted...";
							lblStatus.Refresh();
							Application.DoEvents();
						}
					}// end of while
					lblStatus.Text = nCount.ToString() + " record(s) is inserted.";
					lblStatus.Refresh();
				}// end of using SqlCommand
				conn.Close();
			}// end of using SqlConnection

			lblStatus.Text += "\r\nInsertion is completed.";
			lblStatus.Refresh();

			txtConn.ReadOnly = false;
			txtUID.ReadOnly = false;
			txtPWD.ReadOnly = false;
			txtTable.ReadOnly = false;
			txtNumber.ReadOnly = false;
			txtEndNumber.ReadOnly = false;
			cmdStart.Text = "End";
		}

		private String[] getAttributes(String sLine)
		{
			String[] arrTemp = sLine.Split(new char[] {'|'});
			return arrTemp;
		}
	}
}
