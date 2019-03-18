using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class frmEventAlarm : Form
	{
		private SortedList<DateTime, string> m_alarmList;
		private readonly DateTime m_daily = DateTime.Parse("2000-01-01");

		private bool doClose;
		private ResourceInfo[] _res;
		private string _uniName;
		private int tempID = -1;

		public SortedList<DateTime, string> AlarmList
		{
			get { return m_alarmList; }
			set
			{
				lock (m_alarmList)
				{
					if (m_alarmList.Count > 0) m_alarmList.Clear();
					m_alarmList = null;
					m_alarmList = new SortedList<DateTime, string>(value);
				}
			}
		}

		public bool DoClose
		{
			set { doClose = value; }
		}

		public ResourceInfo[] ResourceInfos
		{
			set { _res = value; }
		}

		internal string UniverseName
		{
			set { _uniName = value; }
		}

		public frmEventAlarm()
		{
			InitializeComponent();

			doClose = false;
			m_alarmList = new SortedList<DateTime, string>();
			_res = null;
		}

		public frmEventAlarm(SortedList<DateTime, string> list)
		{
			InitializeComponent();

			doClose = false;
			m_alarmList = new SortedList<DateTime, string>(list);
			_res = null;
		}

		private void frmEventAlarm_Load(object sender, EventArgs e)
		{
			if (_res == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 이벤트 설정을 할 수 없는 상태입니다.",
								  "oBrowser2: " + _uniName + " 이벤트 설정 장애",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				doClose = true;
				this.Close();
				return;
			}

			cboContent.SelectedIndex = 0;
			cboGalaxy.SelectedIndex = 0;
			cboExpedition.SelectedIndex = 0;

			// 행성 좌표 목록 채우기
			cboPlanet.Items.Clear();
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				if ((_res[i].ColonyName.IndexOf("(달)") > 0) || (_res[i].ColonyName.Trim() == "달"))
					cboPlanet.Items.Add(_res[i].Location + " (달)");
				else
					cboPlanet.Items.Add(_res[i].Location);
			}
			cboPlanet.SelectedIndex = 0;
			cboFleetMove.SelectedIndex = 0;

			// 컨트롤 속성 기본값
			optType1.Checked = true;
			chkDaily.Visible = true;
			txtContent.Visible = true;
			cboContent.Visible = false;
			cboGalaxy.Visible = false;
			cboPlanet.Visible = false;
			cboExpedition.Visible = false;
			cboFleetMove.Visible = false;
			cboDetail.Visible = false;

			if (m_alarmList.Count > 0)
			{
				RefreshList();
				if (alarmList.Items.Count > 0) alarmList.Items[0].Selected = true;
			}
		}

		private void cmdRegister_Click(object sender, EventArgs e)
		{
			bool opt = optType1.Checked;
			if (opt)
			{
				string val = txtContent.Text;
				if (val.Trim().Length == 0)
				{
					MessageBox.Show("내용을 입력하십시오.", "내용 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				string val2 = val.Replace("[R]", "/R/");
				DateTime picker = chkDaily.Checked ? m_daily : dateTimePicker1.Value.Date;
				DateTime key = picker.AddHours((double)hourUpdown.Value).AddMinutes((double)minUpdown.Value);

				lock (m_alarmList)
				{
					if (!m_alarmList.ContainsKey(key))
					{
						m_alarmList.Add(key, val2);
						RefreshList();
					}
					else
						MessageBox.Show("해당 시각에 이미 등록된 이벤트(혹은 예약) 항목이 있습니다.", "이벤트 중복", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			else
			{
				// 함대 기동 추가 설정창 표시
				int itemID = -1;
				if (cboFleetMove.Visible)
				{
					this.TopMost = false;
					frmFleetAttack attackForm = new frmFleetAttack();
					attackForm.ResourceInfos = _res;
					attackForm.UniverseName = _uniName;
					attackForm.ItemID = tempID;
					DialogResult result = attackForm.ShowDialog(this.Parent);
					if (result == DialogResult.OK)
					{
						itemID = attackForm.ItemID;
						tempID = -1;
						this.TopMost = true;
					}
					else if (result == DialogResult.Cancel)
					{
						this.TopMost = true;
						return;
					}
				}

				string val = "[R]" + cboContent.SelectedIndex + "^";
				if (cboGalaxy.Visible)
					val += cboGalaxy.SelectedIndex;
				else if (cboPlanet.Visible)
				{
					if (cboContent.SelectedIndex > 3) // 건설 및 연구
					{
						switch (cboContent.SelectedIndex)
						{
							case 4:
								val += "BB" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 5:
								val += "BC" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 6:
								val += "BD" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 7:
								val += "RB" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 8:
								val += "RC" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
						}

						if (cboDetail.Visible)
						{
							NVItem item = (NVItem)cboDetail.Items[cboDetail.SelectedIndex];
							val += "-" + item.Value;
						}
					}
					else
						val += cboPlanet.Items[cboPlanet.SelectedIndex];
				}
				else if (cboExpedition.Visible)
					val += "E" + cboExpedition.SelectedIndex;
				else if (cboFleetMove.Visible)
					val += "M" + cboFleetMove.SelectedIndex + (itemID != -1 ? ":" + itemID : "");

				DateTime picker = chkDaily.Checked ? m_daily : dateTimePicker1.Value.Date;
				DateTime key = picker.AddHours((double)hourUpdown.Value).AddMinutes((double)minUpdown.Value);

				lock (m_alarmList)
				{
					if (!m_alarmList.ContainsKey(key))
					{
						m_alarmList.Add(key, val);
						RefreshList();
					}
					else
						MessageBox.Show("해당 시각에 이미 등록된 이벤트(혹은 예약) 항목이 있습니다.", "이벤트 중복", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		public void RefreshList()
		{
			NameObjectCollection fleetMoveInfo2 = OB2Util.GetFleetMoveInfo2();

			alarmList.Items.Clear();
			for (int i = 0; i < m_alarmList.Count; i++)
			{
				DateTime theDate = m_alarmList.Keys[i];
				string val = m_alarmList.Values[i];
				
				if (optType1.Checked) // 이벤트 알림 목록
				{
					if (val.StartsWith("[R]")) continue;
				}
				else	// 예약 목록
				{
					if (!val.StartsWith("[R]")) continue;
				}

				string itemText;
				if (theDate.Date == DateTime.Today)
					itemText = theDate.ToString("오늘, HH:mm");
				else if (theDate.Date == DateTime.Today.AddDays(1))
					itemText = theDate.ToString("내일, HH:mm");
				else if (theDate.Date == DateTime.Today.AddDays(-1))
					itemText = theDate.ToString("어제, HH:mm");
				else if (theDate.Date == m_daily)	// 매일 옵션
					itemText = theDate.ToString("매일, HH:mm");
				else
					itemText = theDate.ToString("M월 d일, HH:mm");

				ListViewItem item = new ListViewItem(itemText);
				item.Tag = m_alarmList.Keys[i].ToString("yyyy-MM-dd HH:mm");
				
				if (theDate.Date != m_daily)
				{
					if (theDate < DateTime.Now) item.ForeColor = Color.DarkRed;
					item.ToolTipText = m_alarmList.Keys[i].ToString("yyyy년 M월 d일 H시 m분");
				}
				else
				{
					if (theDate < m_daily.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute)) item.ForeColor = Color.DarkRed;
					item.ToolTipText = m_alarmList.Keys[i].ToString("매일 H시 m분");
				}

				if (val.StartsWith("[R]"))	// 예약
				{
					string[] vals = val.Substring(3).Split(new char[] { '^' });
					if (vals[1].Length > 0)
					{
						if (vals[1].Length == 2)	// = 2: 원정(새버전) 횟수
						{
							string tmp = vals[1];
							if (tmp.StartsWith("E"))	// 원정
							{
								int iTmp = int.Parse(tmp.Replace("E", "")) + 1;
								if (iTmp == 1)
								{
									item.SubItems.Add("예약: " + cboContent.Items[int.Parse(vals[0])]);
									item.ToolTipText = "예약: " + cboContent.Items[int.Parse(vals[0])];
								}
								else
								{
									item.SubItems.Add("예약: [" + iTmp + "회]" + cboContent.Items[int.Parse(vals[0])]);
									item.ToolTipText = "예약: [" + iTmp + "회]" + cboContent.Items[int.Parse(vals[0])];
								}
							}
							else if (tmp.StartsWith("M"))	// 함대 기동
							{
								int iTmp = int.Parse(tmp.Replace("M", "")) + 1;
								if (iTmp == 1)
								{
									item.SubItems.Add("예약: " + cboContent.Items[int.Parse(vals[0])]);
									item.ToolTipText = "예약: " + cboContent.Items[int.Parse(vals[0])];
								}
								else
								{
									item.SubItems.Add("예약: [" + iTmp + "회]" + cboContent.Items[int.Parse(vals[0])]);
									item.ToolTipText = "예약: [" + iTmp + "회]" + cboContent.Items[int.Parse(vals[0])];
								}
							}
						}
						else if (vals[1].Length > 1)	// > 1: 행성 좌표(플릿 세이빙) 또는 함대 기동 또는 건설/연구
						{
							string tmp = vals[1];
							if (tmp.StartsWith("M"))	// 함대 기동
							{
								string text;

								string[] ss = tmp.Split(new char[] {':'});
								if ((fleetMoveInfo2 != null) && fleetMoveInfo2.ContainsKey(ss[1]))
								{
									FleetMoveInfo info = (FleetMoveInfo) fleetMoveInfo2[ss[1]];

									text = getMoveType(info.MoveType) + "/" + info.PlanetCoordinate +
									       "→" + info.TargetCoords + getTargetType(info.TargetType);
								}
								else
								{
									text = cboContent.Items[int.Parse(vals[0])].ToString();
								}

								int iTmp = int.Parse(ss[0].Replace("M", "")) + 1;
								if (iTmp == 1)
								{
									item.SubItems.Add("예약: " + text);
									item.ToolTipText = "예약: " + text;
								}
								else
								{
									item.SubItems.Add("예약: [" + iTmp + "회]" + text);
									item.ToolTipText = "예약: [" + iTmp + "회]" + text;
								}
							}
							else if (tmp.StartsWith("B"))	// 건설
							{
								string[] ss = tmp.Split(new char[] {'-'});
								string text = "[" + ss[0].Substring(2) + "]" + cboContent.Items[int.Parse(vals[0])];
								if (!tmp.StartsWith("BC"))
								{
									string bName = "";
									for (int k = 0; k < OB2Util.BUILDING1_IDS.Length; k++)
									{
										if (ss[1] == OB2Util.BUILDING1_IDS[k])
										{
											bName = OB2Util.BUILDING1_NAMES[k];
											break;
										}
									}
									for (int k = 0; k < OB2Util.BUILDING2_IDS.Length; k++)
									{
										if (ss[1] == OB2Util.BUILDING2_IDS[k])
										{
											bName = OB2Util.BUILDING2_NAMES[k];
											break;
										}
									}
									text += "/" + bName;
								}

								item.SubItems.Add("예약: " + text);
								item.ToolTipText = "예약: " + text;
							}
							else if (tmp.StartsWith("R"))	// 연구
							{
								string[] ss = tmp.Split(new char[] { '-' });
								string text = "[" + ss[0].Substring(2) + "]" + cboContent.Items[int.Parse(vals[0])];
								string rName = "";
								for (int k = 0; k < OB2Util.RESEARCH_IDS.Length; k++)
								{
									if (ss[1] == OB2Util.RESEARCH_IDS[k])
									{
										rName = OB2Util.RESEARCH_NAMES[k];
										break;
									}
								}
								text += "/" + rName;

								item.SubItems.Add("예약: " + text);
								item.ToolTipText = "예약: " + text;
							}
							else // 플릿 세이빙
							{
								item.SubItems.Add("예약: [" + tmp + "]" + cboContent.Items[int.Parse(vals[0])]);
								item.ToolTipText = "예약: [" + tmp + "]" + cboContent.Items[int.Parse(vals[0])];
							}
						}
						else // = 1: 은하 인덱스(자원 모으기)
						{
							item.SubItems.Add("예약: [" + cboGalaxy.Items[int.Parse(vals[1])] + "]" + cboContent.Items[int.Parse(vals[0])]);
							item.ToolTipText = "예약: [" + cboGalaxy.Items[int.Parse(vals[1])] + "]" + cboContent.Items[int.Parse(vals[0])];
						}
					}
					else	// 원정(이전 버전)
					{
						item.SubItems.Add("예약: " + cboContent.Items[int.Parse(vals[0])]);
						item.ToolTipText = "예약: " + cboContent.Items[int.Parse(vals[0])];
					}
				}
				else
				{
					item.SubItems.Add(val);
					item.ToolTipText = val;
				}
				alarmList.Items.Add(item);
			}
			alarmList.Refresh();
		}

		private void alarmList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (alarmList.SelectedIndices.Count > 0)
			{
				DateTime theDate = DateTime.Parse((string)alarmList.SelectedItems[0].Tag);
				int index = -1;
				for (int i = 0; i < m_alarmList.Count; i++)
				{
					DateTime dd = m_alarmList.Keys[i];
					if (dd.Equals(theDate))
					{
						index = i;
						break;
					}
				}
				DateTime sel1 = m_alarmList.Keys[index];
				string sel2 = m_alarmList.Values[index];
				
				if (sel2.StartsWith("[R]"))	// 예약
				{
					if (sel1.Date == m_daily)
					{
						chkDaily.Checked = true;
					}
					else
					{
						chkDaily.Checked = false;
						dateTimePicker1.Value = sel1;
					}

					hourUpdown.Value = sel1.Hour;
					minUpdown.Value = sel1.Minute;

					// [R]0^ or [R]0^E1
					// [R]1^1
					// [R]2^1:200:1
					// [R]3^M0 or [R]3^M3:150
					// [R]4^BB1:200:1-1 or [R]6^BD1:200:1-1
					// [R]5^BC1:200:1
					// [R]7^RB1:200:1-106 or [R]8^RC1:200:1-106

					string sel = sel2.Substring(3);
					string[] vals = sel.Split(new char[] { '^' });

					int iSelected = int.Parse(vals[0]);
					cboContent.SelectedIndex = iSelected;

					if (vals[1].Length > 0)
					{
						if (vals[1].Length == 2) // = 2: 원정(새버전) 횟수 또는 함대 기동 횟수
						{
							string tmp = vals[1];
							if (tmp.StartsWith("E"))	// 원정
							{
								cboExpedition.Visible = true;

								int iTmp = int.Parse(tmp.Replace("E", ""));
								cboExpedition.SelectedIndex = iTmp;
							}
							else if (tmp.StartsWith("M"))	// 함대 기동
							{
								cboFleetMove.Visible = true;

								int iTmp = int.Parse(tmp.Replace("M", ""));
								cboFleetMove.SelectedIndex = iTmp;
							}
						}
						else if (vals[1].Length > 1) // > 1: 행성 좌표(플릿 세이빙) 또는 함대 기동 또는 연구/건설
						{
							string tmp = vals[1];
							if (tmp.StartsWith("M"))	// 함대 기동
							{
								cboFleetMove.Visible = true;

								string[] ss = tmp.Split(new char[] { ':' });
								int iTmp = int.Parse(ss[0].Replace("M", ""));
								cboFleetMove.SelectedIndex = iTmp;
							}
							else if (tmp.StartsWith("B")) // 건설
							{
								string[] ss = tmp.Split(new char[] { '-' });
								string sCrd = ss[0].Substring(2);

								cboPlanet.Visible = true;
								for (int i = 0; i < cboPlanet.Items.Count; i++)
								{
									if ((string)cboPlanet.Items[i] == sCrd)
									{
										cboPlanet.SelectedIndex = i;
										break;
									}
								}

								cboDetail.Visible = true;

								string bID = ss[1];
								setBuildings(bID);
							}
							else if (tmp.StartsWith("R")) // 연구
							{
								string[] ss = tmp.Split(new char[] { '-' });
								string sCrd = ss[0].Substring(2);

								cboPlanet.Visible = true;
								for (int i = 0; i < cboPlanet.Items.Count; i++)
								{
									if ((string)cboPlanet.Items[i] == sCrd)
									{
										cboPlanet.SelectedIndex = i;
										break;
									}
								}

								cboDetail.Visible = true;

								string rID = ss[1];
								setResearches(rID);
							}
							else // 플릿 세이빙
							{
								cboPlanet.Visible = true;
								for (int i = 0; i < cboPlanet.Items.Count; i++)
								{
									if ((string) cboPlanet.Items[i] == tmp)
									{
										cboPlanet.SelectedIndex = i;
										break;
									}
								}
							}
						}
						else // = 1: 은하 인덱스(자원 모으기)
						{
							cboGalaxy.Visible = true;
							cboGalaxy.SelectedIndex = int.Parse(vals[1]);
						}
					}
					else	// 원정(이전 버전)
					{
						cboExpedition.Visible = true;
						cboExpedition.SelectedIndex = 0;
					}
				}
				else
				{
					if (sel1.Date == m_daily)
					{
						chkDaily.Checked = true;
					}
					else
					{
						chkDaily.Checked = false;
						dateTimePicker1.Value = sel1;
					}
					
					hourUpdown.Value = sel1.Hour;
					minUpdown.Value = sel1.Minute;
					txtContent.Text = sel2;
				}
			}
		}

		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			bool opt = optType1.Checked;
			if (opt)
			{
				string val = txtContent.Text;
				if (val.Trim().Length == 0)
				{
					MessageBox.Show("내용을 입력하십시오.", "내용 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				val = val.Replace("[R]", "/R/");
				DateTime picker = chkDaily.Checked ? m_daily : dateTimePicker1.Value.Date;
				DateTime key = picker.AddHours((double)hourUpdown.Value).AddMinutes((double)minUpdown.Value);

				lock (m_alarmList)
				{
					if (m_alarmList.ContainsKey(key))
					{
						for (int i = 0; i < m_alarmList.Count; i++)
						{
							if (m_alarmList.Keys[i].Equals(key))
							{
								m_alarmList.RemoveAt(i);
								m_alarmList.Add(key, val);
								RefreshList();
								break;
							}
						}
					}
					else
						MessageBox.Show("등록된 이벤트가 없습니다.\r\n(등록된 이벤트의 시각을 변경할 때는 삭제-등록 절차로 변경하십시오.)", "이벤트 없음", MessageBoxButtons.OK,
						                MessageBoxIcon.Warning);
				}
			}
			else
			{
				DateTime picker = chkDaily.Checked ? m_daily : dateTimePicker1.Value.Date;
				DateTime key = picker.AddHours((double)hourUpdown.Value).AddMinutes((double)minUpdown.Value);

				int itemID = -1;
				if (m_alarmList.ContainsKey(key))
				{
					for (int i = 0; i < m_alarmList.Count; i++)
					{
						if (m_alarmList.Keys[i].Equals(key))
						{
							string s = m_alarmList.Values[i]; // [R]1^M1:123
							if (s.StartsWith("[R]"))
							{
								string[] vals = s.Substring(3).Split(new char[] {'^'});
								if ((vals.Length > 1) && (vals[1].StartsWith("M")))
								{
									string[] ss = s.Split(new char[] {':'});
									if (ss.Length > 1) itemID = int.Parse(ss[1]);
									break;
								}
							}
						}
					}
				}

				// 함대 기동 추가 설정창 표시
				if (cboFleetMove.Visible)
				{
					this.TopMost = false;
					frmFleetAttack attackForm = new frmFleetAttack();
					attackForm.ResourceInfos = _res;
					attackForm.UniverseName = _uniName;
					attackForm.ItemID = itemID;
					DialogResult result = attackForm.ShowDialog(this.Parent);
					if (result == DialogResult.OK)
					{
						itemID = attackForm.ItemID;
						this.TopMost = true;
					}
					else if (result == DialogResult.Cancel)
					{
						this.TopMost = true;
						return;
					}
				}

				string val = "[R]" + cboContent.SelectedIndex + "^";
				if (cboGalaxy.Visible)
					val += cboGalaxy.SelectedIndex;
				else if (cboPlanet.Visible)
				{
					if (cboContent.SelectedIndex > 3) // 건설 및 연구
					{
						switch (cboContent.SelectedIndex)
						{
							case 4:
								val += "BB" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 5:
								val += "BC" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 6:
								val += "BD" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 7:
								val += "RB" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
							case 8:
								val += "RC" + cboPlanet.Items[cboPlanet.SelectedIndex];
								break;
						}

						if (cboDetail.Visible)
						{
							NVItem item = (NVItem)cboDetail.Items[cboDetail.SelectedIndex];
							val += "-" + item.Value;
						}
					}
					else
						val += cboPlanet.Items[cboPlanet.SelectedIndex];
				}
				else if (cboExpedition.Visible)
					val += "E" + cboExpedition.SelectedIndex;
				else if (cboFleetMove.Visible)
					val += "M" + cboFleetMove.SelectedIndex + (itemID != -1 ? ":" + itemID : "");

				lock (m_alarmList)
				{
					if (m_alarmList.ContainsKey(key))
					{
						for (int i = 0; i < m_alarmList.Count; i++)
						{
							if (m_alarmList.Keys[i].Equals(key))
							{
								m_alarmList.RemoveAt(i);
								m_alarmList.Add(key, val);
								RefreshList();
								break;
							}
						}
					}
					else
						MessageBox.Show("등록된 이벤트가 없습니다.\r\n(등록된 이벤트의 시각을 변경할 때는 삭제-등록 절차로 변경하십시오.)", "이벤트 없음", MessageBoxButtons.OK,
						                MessageBoxIcon.Warning);
				}
			}
		}

		private void cmdDelete_Click(object sender, EventArgs e)
		{
			if (alarmList.CheckedItems.Count > 0)
			{
				for (int i = 0; i < alarmList.CheckedItems.Count; i++)
				{
					DateTime key = DateTime.Parse((string)alarmList.CheckedItems[i].Tag);
					lock (m_alarmList)
					{
						if (m_alarmList.ContainsKey(key))
						{
							for (int k = 0; k < m_alarmList.Count; k++)
							{
								if (m_alarmList.Keys[k].Equals(key))
								{
									// 함대 기동의 경우 임시변수에 기록 --> 이후 등록 시 활용
									string s = m_alarmList.Values[k]; // [R]1^M1:123
									if (s.StartsWith("[R]"))
									{
										string[] vals = s.Substring(3).Split(new char[] { '^' });
										if ((vals.Length > 1) && (vals[1].StartsWith("M")))
										{
											string[] ss = vals[1].Split(new char[] { ':' });
											if (ss.Length > 1) tempID = int.Parse(ss[1]);
										}
									}
									m_alarmList.RemoveAt(k);
									break;
								}
							}
						}
					}
				}
				RefreshList();
			}
			else
			{
				if (alarmList.SelectedItems.Count > 0)
				{
					DateTime key = DateTime.Parse((string)alarmList.SelectedItems[0].Tag);
					lock (m_alarmList)
					{
						if (m_alarmList.ContainsKey(key))
						{
							for (int k = 0; k < m_alarmList.Count; k++)
							{
								if (m_alarmList.Keys[k].Equals(key))
								{
									// 함대 기동의 경우 임시변수에 기록 --> 이후 등록 시 활용
									string s = m_alarmList.Values[k]; // [R]1^M1:123
									if (s.StartsWith("[R]"))
									{
										string[] vals = s.Substring(3).Split(new char[] { '^' });
										if ((vals.Length > 1) && (vals[1].StartsWith("M")))
										{
											string[] ss = vals[1].Split(new char[] { ':' });
											if (ss.Length > 1) tempID = int.Parse(ss[1]);
										}
									}
									m_alarmList.RemoveAt(k);
									break;
								}
							}
						}
					}
					RefreshList();
				}
				else
					MessageBox.Show("삭제할 이벤트(혹은 예약) 항목이 없습니다.", "이벤트 없음", MessageBoxButtons.OK,
									MessageBoxIcon.Warning);
			}
		}

		private void hourUpdown_Enter(object sender, EventArgs e)
		{
			hourUpdown.Select(0, hourUpdown.Text.Length);
		}

		private void minUpdown_Enter(object sender, EventArgs e)
		{
			minUpdown.Select(0, minUpdown.Text.Length);
		}

		private void hourUpdown_ValueChanged(object sender, EventArgs e)
		{
			if (hourUpdown.Value == 24)
			{
				dateTimePicker1.Value = dateTimePicker1.Value.AddDays(1);
				hourUpdown.Value = 0;
			}
			else if (hourUpdown.Value == -1)
			{
				dateTimePicker1.Value = dateTimePicker1.Value.AddDays(-1);
				hourUpdown.Value = 23;
			}
		}

		private void minUpdown_ValueChanged(object sender, EventArgs e)
		{
			if (minUpdown.Value == 60)
			{
				hourUpdown.Value = hourUpdown.Value + 1;
				minUpdown.Value = 0;
			}
			else if (minUpdown.Value == -1)
			{
				hourUpdown.Value = hourUpdown.Value - 1;
				minUpdown.Value = 59;
			}
		}

		private void optType1_CheckedChanged(object sender, EventArgs e)
		{
			optChanged();
		}

		private void optType2_CheckedChanged(object sender, EventArgs e)
		{
			optChanged();
		}

		private void optChanged()
		{
			if (optType1.Checked)
			{
				chkDaily.Checked = false;
				txtContent.Visible = true;
				cboContent.Visible = false;
				cboGalaxy.Visible = false;
				cboPlanet.Visible = false;
				cboExpedition.Visible = false;
				cboFleetMove.Visible = false;
				cboDetail.Visible = false;
			}
			else
			{
				chkDaily.Checked = false;
				txtContent.Visible = false;
				cboContent.Visible = true;
				cboContentChanged();
			}

			RefreshList();
			if (alarmList.Items.Count > 0) alarmList.Items[0].Selected = true;
		}

		private void chkDaily_CheckedChanged(object sender, EventArgs e)
		{
			dateTimePicker1.Enabled = !chkDaily.Checked;
		}

		private void cboContent_SelectedIndexChanged(object sender, EventArgs e)
		{
			cboContentChanged();
		}

		private void cboContentChanged()
		{
			switch (cboContent.SelectedIndex)
			{
				case 0: // 원정
					cboGalaxy.Visible = false;
					cboPlanet.Visible = false;
					cboExpedition.Visible = true;
					cboFleetMove.Visible = false;
					cboDetail.Visible = false;
					break;
				case 1: // 자원모으기
					cboGalaxy.Visible = true;
					cboPlanet.Visible = false;
					cboExpedition.Visible = false;
					cboFleetMove.Visible = false;
					cboDetail.Visible = false;
					break;
				case 2: // 플릿세이빙
					cboGalaxy.Visible = false;
					cboPlanet.Visible = true;
					cboExpedition.Visible = false;
					cboFleetMove.Visible = false;
					cboDetail.Visible = false;
					break;
				case 3: // 함대 기동
					cboGalaxy.Visible = false;
					cboPlanet.Visible = false;
					cboExpedition.Visible = false;
					cboFleetMove.Visible = true;
					cboDetail.Visible = false;
					break;
				case 4: // 건물-건설
				case 5: // 건물-취소
				case 6: // 건물-파괴
					cboGalaxy.Visible = false;
					cboPlanet.Visible = true;
					cboExpedition.Visible = false;
					cboFleetMove.Visible = false;
					cboDetail.Visible = true;
					setBuildings(null);
					break;
				case 7: // 연구-연구
				case 8: // 연구-취소
					cboGalaxy.Visible = false;
					cboPlanet.Visible = true;
					cboExpedition.Visible = false;
					cboFleetMove.Visible = false;
					cboDetail.Visible = true;
					setResearches(null);
					break;
			}
		}

		private void setBuildings(string id)
		{
			// 건물 목록 채우기
			cboDetail.Items.Clear();
			int index = 0;
			for (int i = 0; i < OB2Util.BUILDING1_IDS.Length; i++)
			{
				cboDetail.Items.Add(new NVItem(OB2Util.BUILDING1_NAMES[i], OB2Util.BUILDING1_IDS[i]));
				if (!string.IsNullOrEmpty(id) && (id == OB2Util.BUILDING1_IDS[i])) index = i;
			}
			for (int i = 0; i < OB2Util.BUILDING2_IDS.Length; i++)
			{
				cboDetail.Items.Add(new NVItem(OB2Util.BUILDING2_NAMES[i], OB2Util.BUILDING2_IDS[i]));
				if (!string.IsNullOrEmpty(id) && (id == OB2Util.BUILDING2_IDS[i])) index = i + OB2Util.BUILDING1_IDS.Length;
			}
			cboDetail.SelectedIndex = index;
		}

		private void setResearches(string id)
		{
			// 연구 목록 채우기
			cboDetail.Items.Clear();
			int index = 0;
			for (int i = 0; i < OB2Util.RESEARCH_IDS.Length; i++)
			{
				cboDetail.Items.Add(new NVItem(OB2Util.RESEARCH_NAMES[i], OB2Util.RESEARCH_IDS[i]));
				if (!string.IsNullOrEmpty(id) && (id == OB2Util.RESEARCH_IDS[i])) index = i;
			}
			cboDetail.SelectedIndex = index;
		}

		private void frmEventAlarm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!doClose) e.Cancel = true;
			rearrangeCache();
			this.Hide();
		}

		private void rearrangeCache()
		{
			NameObjectCollection fleetInfo2 = OB2Util.GetFleetMoveInfo2();
			if ((fleetInfo2 == null) || (fleetInfo2.Count == 0)) return;

			NameObjectCollection newCollection = new NameObjectCollection();
			for (int i = 0; i < fleetInfo2.Count; i++)
			{
				string key = fleetInfo2.GetKey(i);

				for (int k = 0; k < m_alarmList.Count; k++)
				{
					// 함대 기동의 경우 이벤트에 없는 캐시 삭제
					string s = m_alarmList.Values[k]; // [R]1^M1:123
					if (s.StartsWith("[R]"))
					{
						string[] vals = s.Substring(3).Split(new char[] {'^'});
						if ((vals.Length > 1) && (vals[1].StartsWith("M")))
						{
							string[] ss = vals[1].Split(new char[] {':'});
							if (ss.Length > 1)
							{
								if (key == ss[1])
								{
									newCollection.Add(key, fleetInfo2[key]);
									break;
								}
							}
						}
					}
				}
			}

			string newString = OB2Util.FleetMoveInfo2ToString(newCollection);

			SettingsHelper settings = SettingsHelper.Current;
			if (newString != settings.FleetMoving2)
			{
				settings.FleetMoving2 = newString;
				settings.Changed = true;
				// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
			}
		}

		private static string getMoveType(int moveType)
		{
			switch (moveType)
			{
				case 1:
					return "공격";
				case 3:
					return "운송";
				case 4:
					return "배치";
				case 6:
					return "정탐";
				case 7:
					return "식민";
				case 8:
					return "수확";
				case 15:
					return "원정";
			}
			return "(잘못됨)";
		}

		private static string getTargetType(int targetType)
		{
			switch (targetType)
			{
				case 1:
					return "";
				case 2:
					return "(파편지대)";
				case 3:
					return "(달)";
			}
			return "(잘못됨)";
		}
	}
}
