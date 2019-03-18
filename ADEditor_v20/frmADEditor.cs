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
	/// Form1�� ���� ��� �����Դϴ�.
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
				MessageBox.Show("���������� �дµ� ������ �߻��߽��ϴ�.");

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

				if (txtName01.Text.Trim().Length == 0)
				{
					target = child;
				}
				else
				{
					if (searchedString.Trim().Length == 0)
					{
						lblStatus.Text = "��ȸ�� ��ü�� ù��° �Ӽ�(�˻� Ű)�� ä���ּ���.";
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
								throw new COMException("��ȸ ����� �����ϴ�.", (int)0x80072030);
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
								throw new COMException("��ȸ ����� �����ϴ�.", (int)0x80072030);
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

				if (txtName01.Text.Trim().Length == 0)
				{
					target = child;
				}
				else
				{
					if (txtValue01.Text.Trim().Length == 0)
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

				// Password ����ȭ �ɼ��� ���� �ִ� ���
				if ((ckSyncPwd.Checked) && (ckSyncPwd.Enabled) && (txtValue02.Text.Trim().Length > 0))
				{
					if (ckADAM.Checked)
					{
						// (���ÿ�����) �κ������θ� �ŷ��� �� �ִ� �� ���ø����̼� ���� ��쿡
						// ���ε��� �� ����ڿ� ��ȣ�� �������� �ʰ� Secure, Sealing, Signing �ɼ��� ������
						// ������ �̸����� �����Ͽ� GAC�� ����ؾ� �̿� ���� �������� ��� ������.
						//
						// --> dsHeuristic ������ �ϸ� �κ� �ŷڿ����� ��� ������
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
					target.Properties["sAMAccountName"].Value = txtValue01.Text;
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
						//
						// --> dsHeuristic ������ �ϸ� �κ� �ŷڿ����� ��� ������
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
					using (DirectoryEntry user = LdapServer.SearchAD(dn, txtValue01.Text))
					{
						if (user == null) throw new Exception("���̵� �����ϴ�.");

						// pwdLastSet(Int64)�Ӽ��� 0�̸� ���� �α׿¶� �н����� ���� �ʿ�
						// userAccountControl�� 512�� ��쿡��.
						int userAccountControl = LdapServer.GetIntProperty(user, "userAccountControl");
						
						// �н����尡 ����� �� �ִ� ���
						if ((userAccountControl & (int)ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD) == 0)
						{
							long pwdLastSet = LdapServer.GetInt64Property(user, "pwdLastSet");
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
							string rootDN = LdapServer.GetRootDN(txtDN.Text);
							long maxPwdAge = 0;
							using (DirectoryEntry domain = new DirectoryEntry(rootDN, txtADUser.Text, txtADPwd.Text, AuthenticationTypes.ServerBind))
							{
								maxPwdAge = LdapServer.GetInt64Property(domain, "maxPwdAge");
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
						else
						{
							// ���� Ȯ�� �� �н����� ���� �ٽ� üũ
							string newUserName = user.Path.Substring(user.Path.IndexOf(user.Name)).Replace(user.Name, "CN=" + txtValue01.Text);
							user.Username = newUserName;
							user.Password = txtValue02.Text;
							user.AuthenticationType = AuthenticationTypes.ServerBind;
							user.RefreshCache();
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

		private void txtName02_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(txtName02, "�ý��� �Ӽ� �� �н����� �Ӽ��� ������ �����ϴ� ��쿡��\r\n�̰��� �н����� �Ӽ���(��:userPassword)�� �Է��ϼ���.");
		}

		private void ckSyncPwd_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(ckSyncPwd, "�� �Է��� �н����� ������ �ý��� �н����带 ������ ���\r\n�� �׸� üũ�ϼ���.");
		}

		private void ckADAM_MouseMove(object sender, MouseEventArgs e)
		{
			toolTip.SetToolTip(ckADAM, "�����Ϸ��� ��ü�� ADAM user ��ü�� ���\r\n�� �׸� üũ�ϼ���.");
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
