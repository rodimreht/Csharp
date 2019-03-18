using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NETS_iMan
{
	public partial class frmChatHistory : Form
	{
		private bool isLoaded;
		private TreeNode preNode;

		public frmChatHistory()
		{
			InitializeComponent();
		}

		private void frmChatHistory_Load(object sender, EventArgs e)
		{
			isLoaded = false;

			// 날짜 선택
			treeView1.SelectedNode = treeView1.Nodes[0];
			DateTime targetDateStart = DateTime.Today;

			switch (targetDateStart.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					treeView1.Nodes.RemoveAt(6);
					treeView1.Nodes.RemoveAt(5);
					treeView1.Nodes.RemoveAt(4);
					treeView1.Nodes.RemoveAt(3);
					treeView1.Nodes.RemoveAt(2);
					treeView1.Nodes.RemoveAt(1);
					break;

				case DayOfWeek.Monday:
					treeView1.Nodes.RemoveAt(6);
					treeView1.Nodes.RemoveAt(5);
					treeView1.Nodes.RemoveAt(4);
					treeView1.Nodes.RemoveAt(3);
					treeView1.Nodes.RemoveAt(2);
					break;

				case DayOfWeek.Tuesday:
					treeView1.Nodes.RemoveAt(5);
					treeView1.Nodes.RemoveAt(4);
					treeView1.Nodes.RemoveAt(3);
					treeView1.Nodes.RemoveAt(2);
					break;

				case DayOfWeek.Wednesday:
					treeView1.Nodes.RemoveAt(4);
					treeView1.Nodes.RemoveAt(3);
					treeView1.Nodes.RemoveAt(2);
					break;

				case DayOfWeek.Thursday:
					treeView1.Nodes.RemoveAt(3);
					treeView1.Nodes.RemoveAt(2);
					break;

				case DayOfWeek.Friday: // 금요일 이후
					treeView1.Nodes.RemoveAt(2);
					break;

				case DayOfWeek.Saturday:
					break;
			}

			showLog(true);

			isLoaded = true;
		}

		private void showLog(bool bClear)
		{
			// 날짜 선택
			DateTime startDate, endDate;
			switch (treeView1.SelectedNode.Name)
			{
				case "TODAY":
					startDate = DateTime.Today;
					endDate = startDate.AddDays(1);
					break;

				case "YESTERDAY":
					startDate = DateTime.Today.AddDays(-1);
					endDate = startDate.AddDays(1);
					break;

				case "-7":
					startDate = DateTime.Today.AddDays(0 - DateTime.Today.DayOfWeek - 7);
					endDate = startDate.AddDays(7);
					break;

				case "-14":
					startDate = DateTime.Today.AddDays(0 - DateTime.Today.DayOfWeek - 14);
					endDate = startDate.AddDays(7);
					break;

				case "-21":
					startDate = DateTime.Today.AddDays(0 - DateTime.Today.DayOfWeek - 21);
					endDate = startDate.AddDays(7);
					break;

				case "-28":
					startDate = DateTime.Today.AddDays(0 - DateTime.Today.DayOfWeek - 28);
					endDate = startDate.AddDays(7);
					break;

				case "OLD":
					startDate = DateTime.MinValue;
					endDate = DateTime.Today.AddDays(0 - DateTime.Today.DayOfWeek - 28);
					break;

				default:
					startDate = DateTime.Today.AddDays(0 - (DateTime.Today.DayOfWeek - int.Parse(treeView1.SelectedNode.Name)));
					endDate = startDate.AddDays(1);
					break;
			}

			if (bClear)
			{
				// 대상 선택
				cboUsers.Items.Clear();
				cboUsers.Items.Add("전체보기");
				cboUsers.SelectedIndex = 0;
			}

			bool showNextLine = false;
			StringBuilder sb = new StringBuilder();
			StreamReader sr = getChatLog(startDate, endDate);
			if (sr != null)
			{
				string line = sr.ReadLine();
				while (line != null)
				{
					if (line.Length > 22)
					{
						string s = line.Substring(1, 19);
						DateTime theDate;
						if (DateTime.TryParse(s, out theDate))
						{
							if ((theDate >= startDate) && (theDate < endDate)) // 정해진 날짜 범위
							{
								int pos1 = line.IndexOf("회의실{");
								int pos2 = line.IndexOf("님");
								if (pos2 > 22)
								{
									try
									{
										if (pos1 == 22)
											pos2 = line.IndexOf("}", pos1) + 1;
										else
										{
											pos1 = line.IndexOf("회의실(");
											if (pos1 == 22)
												pos2 = line.IndexOf(")", pos1) + 1;
											else
											{
												if (line.Substring(pos2 - 1, 1) != ")")
												{
													pos1 = line.IndexOf(" ", pos2 + 1) + 1;
													pos2 = line.IndexOf("님", pos1);
													if (pos2 < 0)
													{
														pos1 = 22;
														pos2 = line.IndexOf("님");
													}
												}
												else
													pos1 = 22;
											}
										}

										if (bClear) // 전체보기(목록생성)
										{
											if (pos2 >= 0)
											{
												string s2 = line.Substring(pos1, pos2 - pos1);
												if (!cboUsers.Items.Contains(s2)) cboUsers.Items.Add(s2);
											}
											sb.Append(line).Append("\r\n");
											showNextLine = true;
										}
										else if (cboUsers.SelectedIndex == 0) // 전체보기
										{
											sb.Append(line).Append("\r\n");
											showNextLine = true;
										}
										else
										{
											if (pos2 >= 0)
											{
												string s2 = line.Substring(pos1, pos2 - pos1);
												if (cboUsers.SelectedItem.Equals(s2)) // 해당 사용자가 맞으면
												{
													sb.Append(line).Append("\r\n");
													showNextLine = true;
												}
												else
												{
													showNextLine = false;
												}
											}
											else
											{
												showNextLine = false;
											}
										}
									}
									catch (Exception ex)
									{
										Logger.Log(Logger.LogLevel.WARNING, "frmChatHistory::showLog(" + bClear + "):[" + line + "] " + ex);
									}
								}
							}
							else
							{
								showNextLine = false;
							}
						}
						else
						{
							if (showNextLine)
								sb.Append(new string(' ', 21)).Append(line).Append("\r\n");
						}
					}
					else
					{
						if (showNextLine)
							sb.Append(new string(' ', 21)).Append(line).Append("\r\n");
					}
					line = sr.ReadLine();
				}
				sr.Close();
				txtContents.Text = (sb.Length > 0) ? sb.ToString() : "(대화 기록이 없습니다.)";
			}
			else
			{
				txtContents.Text = "(대화 기록이 없습니다.)";
			}
		}

		private static StreamReader getChatLog(DateTime startDate, DateTime endDate)
		{
			try
			{
				string logPath = SettingsHelper.Current.LogPath;
				if (string.IsNullOrEmpty(logPath))
				{
					logPath = Application.ExecutablePath;
					logPath = logPath.Substring(0, logPath.LastIndexOf(@"\"));
				}

				bool bFlag = true;
				MemoryStream ms = new MemoryStream();
				DirectoryInfo di = new DirectoryInfo(logPath);
				FileInfo[] fis = di.GetFiles("NETS-iMan_chat.*.Log");
				if (fis.Length > 0)
				{
					foreach (FileInfo info in fis)
					{
						string s = info.Name.Replace("NETS-iMan_chat.", "").Replace(".Log", "");
						DateTime lastDate = DateTime.ParseExact(s, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
						if (lastDate < startDate) continue;
						if (lastDate > endDate) bFlag = false;

						Stream stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						byte[] bytes = new byte[stream.Length];
						stream.Read(bytes, 0, bytes.Length);
						stream.Close();

						ms.Write(bytes, 0, bytes.Length);
						ms.Flush();
					}
				}

				if (bFlag)
				{
					logPath += (logPath.EndsWith(@"\") ? "" : @"\") + "NETS-iMan_chat";
					string path = logPath + ".Log";
					FileInfo fi = new FileInfo(path);
					Stream stream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					byte[] bytes = new byte[stream.Length];
					stream.Read(bytes, 0, bytes.Length);
					stream.Close();

					ms.Write(bytes, 0, bytes.Length);
					ms.Flush();
				}
				ms.Position = 0;
				return new StreamReader(ms, Encoding.Default);
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.WARNING, "getChatLog(): " + ex.Message);
				return null;
			}
		}

		private void cboUsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (isLoaded)
			{
				showLog(false);
				txtContents.Select(0, 0);
				txtContents.Focus();
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			preNode = treeView1.SelectedNode;
			preNode.NodeFont = new Font(treeView1.Font, FontStyle.Bold);

			isLoaded = false;
			showLog(true);
			isLoaded = true;
		}

		private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			if (preNode == null) return;
			preNode.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
		}
	}
}