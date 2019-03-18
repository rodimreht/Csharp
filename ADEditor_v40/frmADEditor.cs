using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ActiveDs;

namespace ADEditor
{
	/// <summary>
	/// Form1에 대한 요약 설명입니다.
	/// </summary>
	public partial class frmADEditor : Form
	{
		private const string	FILE_NAME = "ADManager.ini";
		
		private static int		currentCount = 0;
		private static bool		conditioned = false;
		private static string	condString = "";
		private static string	searchedString = "";

		private void Form1_Load(object sender, EventArgs e)
		{
			int retCode = ADInit(txtDN, txtADUser, txtADPwd, txtOU);
			if (retCode < 0)
				MessageBox.Show("구성정보를 읽는데 오류가 발생했습니다.");

			if (txtOU.Text.ToUpper().StartsWith("OU="))
				txtSchemaClass.Text = "organizationalUnit";
			else if (txtOU.Text.ToUpper().StartsWith("CN="))
				txtSchemaClass.Text = "container";

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
			lblStatus.Text = "조회 중입니다. 잠시만 기다려주세요...";
			lblStatus.Refresh();

			// ADAM에서는 반드시 인증형태를 None 혹은 ServerBind 형태로 명시해주어야 한다.
			// ADAM의 버그일런지도... -_-
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

				if (txtName01.Text.Trim().Length == 0)
				{
					target = child;
				}
				else
				{
					if (searchedString.Trim().Length == 0)
					{
						lblStatus.Text = "조회할 객체의 첫번째 속성(검색 키)을 채워주세요.";
						lblStatus.Refresh();
						return;
					}

					if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					    (txtName01.Text.Trim().ToLower() != "o") && (!conditioned))
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
								throw new COMException("조회 결과가 없습니다.", (int)0x80072030);
						}

						target = resultCollection[currentCount - 1].GetDirectoryEntry();
					}
					else if ((searchedString.IndexOf(">") >= 0) || (searchedString.IndexOf("<") >= 0) ||
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
								throw new COMException("조회 결과가 없습니다.", (int)0x80072030);
						}

						target = resultCollection[currentCount - 1].GetDirectoryEntry();
					}
					else
					{
						target = child.Children.Find(txtName01.Text + "=" + searchedString, txtobjectClass.Text);
					}
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
											{
												string tagString;
												targetTextBox.Text = LdapServer.GetStringProperty(target, tb.Text, out tagString);
												tb.Tag = tagString;
											}
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
				lblStatus.Text = "성공적으로 조회되었습니다.";
				lblStatus.Refresh();
			}
			catch (COMException exx)
			{
				MessageBox.Show("조회실패 / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
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
			lblStatus.Text = "수정 중입니다. 잠시만 기다려주세요...";
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

				if (txtName01.Text.Trim().Length == 0)
				{
					target = child;
				}
				else
				{
					if (txtValue01.Text.Trim().Length == 0)
					{
						lblStatus.Text = "수정할 객체의 첫번째 속성을 채워주세요.";
						lblStatus.Refresh();
						return;
					}

					if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
						(txtName01.Text.Trim().ToLower() != "o"))
					{
						lblStatus.Text = "수정할 객체의 첫번째 속성 이름은 cn, ou, o만 허용됩니다.";
						lblStatus.Refresh();
						return;
					}
					target = child.Children.Find(txtName01.Text + "=" + txtValue01.Text, txtobjectClass.Text);
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
									if ((targetTextBox.Name.ToLower() == "txtvalue" + number) && (number != "01"))
									{
										if (targetTextBox.Text.Trim().Length > 0)
										{
											/*
											if ((tb.Tag != null) && ((string)tb.Tag == "System.__ComObject"))
												SetInt64Property(target, tb.Text, DateTime.Parse(targetTextBox.Text));
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

				// Password 동기화 옵션이 켜져 있는 경우
				if ((ckSyncPwd.Checked) && (ckSyncPwd.Enabled) && (txtValue02.Text.Trim().Length > 0))
				{
					if (ckADAM.Checked)
					{
						// (로컬에서만) 부분적으로만 신뢰할 수 있는 웹 어플리케이션 등의 경우에
						// 바인딩할 때 사용자와 암호를 지정하지 않고 Secure, Sealing, Signing 옵션을 쓰려면
						// 강력한 이름으로 서명하여 GAC에 등록해야 이와 같은 형식으로 사용 가능함.
						//
						// --> dsHeuristic 설정을 하면 부분 신뢰에서도 사용 가능함
						using (DirectoryEntry pwdDN = new DirectoryEntry(target.Path,
								   null, null, 
								   AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing))
						{
							pwdDN.Invoke("SetOption", new object[] { LdapServer.ADS_OPTION_PASSWORD_PORTNUMBER, LdapServer.ADS_LDAP_PORT });
							pwdDN.Invoke("SetOption", new object[] { LdapServer.ADS_OPTION_PASSWORD_METHOD, LdapServer.ADS_PASSWORD_ENCODE_CLEAR });
							pwdDN.Invoke("SetPassword", txtValue02.Text);
							pwdDN.CommitChanges();
							pwdDN.Close();
						}
					}
					else
					{
						// SetPassword는 Kerberos인증이 요구되므로 Secure 옵션이 필요하다.
						target.Invoke("SetPassword", txtValue02.Text);
						//target.Invoke("ChangePassword", new object[] { txtValue08.Text, txtValue02.Text });
						target.Close();
					}
				}

				lblStatus.Text = "성공적으로 수정되었습니다.";
				lblStatus.Refresh();
			}
			catch (TargetInvocationException tiEx)
			{
				COMException exx = (COMException)tiEx.InnerException;

				if (exx.ErrorCode == -2147024810) // 0x80070056: 이전 패스워드 틀림
					MessageBox.Show("이전 패스워드가 틀렸습니다.");
				else if (exx.ErrorCode == -2147022651) // 0x800708C5: 패스워드 정책 위반
					MessageBox.Show("패스워드 정책에 맞지 않는 패스워드를 입력하셨습니다.");
				else
					MessageBox.Show("수정실패 / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
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
			lblStatus.Text = "생성 중입니다. 잠시만 기다려주세요...";
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
					lblStatus.Text = "생성할 객체의 첫번째 속성을 채워주세요.";
					lblStatus.Refresh();
					return;
				}

				if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					(txtName01.Text.Trim().ToLower() != "o"))
				{
					lblStatus.Text = "생성할 객체의 첫번째 속성 이름은 cn, ou, o만 허용됩니다.";
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

				// AD user 계정인 경우에는 sAMAccountName을 설정해준다.
				if ((ckSyncPwd.Enabled) && (!ckADAM.Checked))
				{
					target.Properties["sAMAccountName"].Value = txtValue01.Text;
				}
				target.CommitChanges();

				// Password 동기화가 설정된 경우
				if ((txtValue02.Text.Trim().Length > 0) && (ckSyncPwd.Checked))
				{
					// ADAM인 경우와 AD인 경우를 구분하여 처리한다.
					if (ckADAM.Checked)
					{
						target.Properties["ms-DS-UserAccountAutoLocked"].Value = false;
						target.Properties["msDS-User-Account-Control-Computed"].Value = (int) ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT;
						target.Properties["msDS-UserAccountDisabled"].Clear();
						if (ckDontExpirePwd.Checked)
							target.Properties["msDS-UserPasswordExpired"].Value = false;

						// (로컬에서만) 부분적으로만 신뢰할 수 있는 웹 어플리케이션 등의 경우에
						// 바인딩할 때 사용자와 암호를 지정하지 않고 Secure, Sealing, Signing 옵션을 쓰려면
						// 강력한 이름으로 서명하여 GAC에 등록해야 이와 같은 형식으로 사용 가능함.
						//
						// --> dsHeuristic 설정을 하면 부분 신뢰에서도 사용 가능함
						using (DirectoryEntry pwdDN = new DirectoryEntry(target.Path,
								   null, null, 
								   AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing))
						{
							pwdDN.Invoke("SetOption", new object[] { LdapServer.ADS_OPTION_PASSWORD_PORTNUMBER, LdapServer.ADS_LDAP_PORT });
							pwdDN.Invoke("SetOption", new object[] { LdapServer.ADS_OPTION_PASSWORD_METHOD, LdapServer.ADS_PASSWORD_ENCODE_CLEAR });
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

				lblStatus.Text = "성공적으로 생성되었습니다.";
				lblStatus.Refresh();
			}
			catch (TargetInvocationException tiEx)
			{
				COMException exx = (COMException)tiEx.InnerException;

				if (exx.ErrorCode == -2147022651) // 0x800708C5: 패스워드 정책 위반
					MessageBox.Show("패스워드 정책에 맞지 않는 패스워드를 입력하셨습니다.");
				else
					MessageBox.Show("생성실패 / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
			}
			catch (COMException exx)
			{
				MessageBox.Show("생성실패 / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
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
			lblStatus.Text = "삭제 중입니다. 잠시만 기다려주세요...";
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
					lblStatus.Text = "삭제할 객체의 첫번째 속성을 채워주세요.";
					lblStatus.Refresh();
					return;
				}

				if ((txtName01.Text.Trim().ToLower() != "cn") && (txtName01.Text.Trim().ToLower() != "ou") &&
					(txtName01.Text.Trim().ToLower() != "o"))
				{
					lblStatus.Text = "삭제할 객체의 첫번째 속성 이름은 cn, ou, o만 허용됩니다.";
					lblStatus.Refresh();
					return;
				}

				target = child.Children.Find(txtName01.Text + "=" + txtValue01.Text, txtobjectClass.Text);
				child.Children.Remove(target);

				lblStatus.Text = "성공적으로 삭제되었습니다.";
				lblStatus.Refresh();
			}
			catch (COMException exx)
			{
				MessageBox.Show("삭제실패 / Err(0x" + exx.ErrorCode.ToString("X") + ") : " + exx.Message);
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
			lblStatus.Text = "인증 처리 중입니다. 잠시만 기다려주세요...";
			lblStatus.Refresh();

			try
			{
				// ADAM에서는 반드시 인증형태를 None 혹은 ServerBind 형태로 명시해주어야 한다.
				// ADAM의 버그일런지도... -_-
				using (DirectoryEntry dn = new DirectoryEntry(txtDN.Text, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind))
				{
					using (DirectoryEntry user = LdapServer.SearchAD(dn, txtValue01.Text))
					{
						if (user == null) throw new Exception("아이디가 없습니다.");

						// pwdLastSet(Int64)속성이 0이면 다음 로그온때 패스워드 변경 필요
						// userAccountControl이 512인 경우에만.
						int userAccountControl = LdapServer.GetIntProperty(user, "userAccountControl");
						
						// 패스워드가 만료될 수 있는 경우
						if ((userAccountControl & (int)ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD) == 0)
						{
							long pwdLastSet = LdapServer.GetInt64Property(user, "pwdLastSet");
							if (pwdLastSet == 0)
							{
								user.Close();
								throw new Exception("최초 접속: 패스워드 변경이 필요합니다.");
							}

							string newUserName = user.Path.Substring(user.Path.IndexOf(user.Name)).Replace(user.Name, "CN=" + txtValue01.Text);
							user.Username = newUserName;
							user.Password = txtValue02.Text;
							user.AuthenticationType = AuthenticationTypes.ServerBind;
							user.RefreshCache();

							// 패스워드 만료 알림 기능이 설정된 경우
							string rootDN = LdapServer.GetRootDN(txtDN.Text);
							long maxPwdAge = 0;
							using (DirectoryEntry domain = new DirectoryEntry(rootDN, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind))
							{
								maxPwdAge = LdapServer.GetInt64Property(domain, "maxPwdAge");
								domain.Close();
							}

							if (maxPwdAge != 0)	// 도메인에 패스워드 만료기간이 설정되어 있는 경우
							{
								int maxDay = (int)Math.Abs(maxPwdAge / 10000000 / 86400);
								DateTime expireDate = DateTime.FromFileTime(pwdLastSet).AddDays(maxDay);
								int remains = expireDate.Subtract(DateTime.Now).Days;
								if (remains < 0)
									throw new Exception("패스워드가 만료되었습니다.");
								else if (remains < 7)
									throw new Exception("패스워드 만료일까지 " + remains + "일 남았습니다.");
							}
						}
						else
						{
							// 인증 확인 및 패스워드 관련 다시 체크
							string newUserName = user.Path.Substring(user.Path.IndexOf(user.Name)).Replace(user.Name, "CN=" + txtValue01.Text);
							user.Username = newUserName;
							user.Password = txtValue02.Text;
							user.AuthenticationType = AuthenticationTypes.ServerBind;
							user.RefreshCache();
						}
					}
					dn.Close();
				}
				lblStatus.Text = "성공적으로 인증되었습니다.";
				lblStatus.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void txtName02_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(txtName02, "시스템 속성 외 패스워드 속성을 별도로 관리하는 경우에는\r\n이곳에 패스워드 속성명(예:userPassword)을 입력하세요.");
		}

		private void ckSyncPwd_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(ckSyncPwd, "위 입력한 패스워드 값으로 시스템 패스워드를 변경할 경우\r\n이 항목에 체크하세요.");
		}

		private void ckADAM_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(ckADAM, "생성하려는 개체가 ADAM user 개체인 경우\r\n이 항목에 체크하세요.");
		}

		private void btnLdapTest_Click(object sender, EventArgs e)
		{
			string dn = txtDN.Text.ToUpper();
			string ldapPath = dn.Substring(dn.LastIndexOf("/") + 1);
			string server = dn.Replace(ldapPath, "").Replace("LDAP://", "").Replace("/", "");
			string reptype = ckADAM.Checked ? "ADAM" : "AD";

			ADBrowser adBrowser = new ADBrowser();
			adBrowser.LdapPath = ldapPath;
			adBrowser.SetDialog(reptype, server, txtADUser.Text, txtADPwd.Text);
			adBrowser.ShowDialog();

			if (adBrowser.IsOk)
			{
				string tmp = adBrowser.LdapPath.ToUpper();
				tmp = tmp.Replace("," + ldapPath, "").Replace(ldapPath, "");
				if (tmp.IndexOf(",") > 0)
				{
					txtDN.Text = "LDAP://" + server + "/" + tmp.Substring(tmp.IndexOf(",") + 1) + "," + ldapPath;
					txtOU.Text = tmp.Substring(0, tmp.IndexOf(","));
				}
				else
					txtOU.Text = tmp.Replace("," + ldapPath, "").Replace(ldapPath, "");

				if (txtOU.Text.ToUpper().StartsWith("OU="))
					txtSchemaClass.Text = "organizationalUnit";
				else if (txtOU.Text.ToUpper().StartsWith("CN="))
					txtSchemaClass.Text = "container";
			}
		}
	}
}
