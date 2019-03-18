using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class frmEventAlarm : Form
	{
		private SortedList<DateTime, string> m_alarmList;
		private readonly DateTime m_daily = DateTime.Parse("2000-01-01");

		private ResourceInfo[] _res;

		public SortedList<DateTime, string> AlarmList
		{
			get { return m_alarmList; }
			set
			{
				lock (m_alarmList)
				{
					if (m_alarmList.Count > 0) m_alarmList.Clear();
					m_alarmList = null;
					m_alarmList = value;
				}
			}
		}

		public ResourceInfo[] ResourceInfos
		{
			set { _res = value; }
		}

		public frmEventAlarm()
		{
			InitializeComponent();

			m_alarmList = new SortedList<DateTime, string>();
			_res = null;
		}

		public frmEventAlarm(SortedList<DateTime, string> list)
		{
			InitializeComponent();

			m_alarmList = list;
			_res = null;
		}

		private void frmEventAlarm_Load(object sender, EventArgs e)
		{
			if (_res == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 이벤트 설정을 할 수 없는 상태입니다.",
								  "oBrowser2: 이벤트 설정 장애",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
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

				if (_res[i].ColonyName.IndexOf("(달)") > 0)
					cboPlanet.Items.Add(_res[i].Location + " (달)");
				else
					cboPlanet.Items.Add(_res[i].Location);
			}
			cboPlanet.SelectedIndex = 0;

			// 컨트롤 속성 기본값
			optType1.Checked = true;
			chkDaily.Visible = true;
			txtContent.Visible = true;
			cboContent.Visible = false;
			cboGalaxy.Visible = false;
			cboPlanet.Visible = false;
			cboExpedition.Visible = false;

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

				val = val.Replace("[R]", "/R/");
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
			else
			{
				string val = "[R]" + cboContent.SelectedIndex + "^";
				if (cboGalaxy.Visible)
					val += cboGalaxy.SelectedIndex;
				else if (cboPlanet.Visible)
					val += cboPlanet.Items[cboPlanet.SelectedIndex];
				else if (cboExpedition.Visible)
					val += "E" + cboExpedition.SelectedIndex;

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
					item.ToolTipText = m_alarmList.Keys[i].ToString("yyyy년 M월 d일 H시 m분");
				else
					item.ToolTipText = m_alarmList.Keys[i].ToString("매일 H시 m분");

				if (val.StartsWith("[R]"))	// 예약
				{
					string[] vals = val.Substring(3).Split(new char[] { '^' });
					if (vals[1].Length > 0)
					{
						if (vals[1].Length == 2)	// = 2: 원정(새버전) 횟수
						{
							string tmp = vals[1];
							int iTmp = int.Parse(tmp.Replace("E", "")) + 1;
							item.SubItems.Add("예약: [" + iTmp + "회]" + cboContent.Items[int.Parse(vals[0])]);
						}
						if (vals[1].Length > 1)	// > 1: 행성 좌표(플릿 세이빙)
						{
							item.SubItems.Add("예약: [" + vals[1] + "]" + cboContent.Items[int.Parse(vals[0])]);
						}
						else // = 1: 은하 인덱스(자원 모으기)
						{
							item.SubItems.Add("예약: [" + cboGalaxy.Items[int.Parse(vals[1])] + "]" + cboContent.Items[int.Parse(vals[0])]);
						}
					}
					else	// 원정(이전 버전)
						item.SubItems.Add("예약: " + cboContent.Items[int.Parse(vals[0])]);
				}
				else
				{
					item.SubItems.Add(val);
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

					sel2 = sel2.Substring(3);
					string[] vals = sel2.Split(new char[] { '^' });
					if (vals[1].Length > 0)
					{
						if (vals[1].Length == 2) // = 2: 원정(새버전) 횟수
						{
							cboContent.SelectedIndex = int.Parse(vals[0]);

							cboExpedition.Visible = true;

							string tmp = vals[1];
							int iTmp = int.Parse(tmp.Replace("E", ""));
							cboExpedition.SelectedIndex = iTmp;
						}
						else if (vals[1].Length > 1) // > 1: 행성 좌표(플릿 세이빙)
						{
							cboContent.SelectedIndex = int.Parse(vals[0]);

							cboPlanet.Visible = true;
							for (int i = 0; i < cboPlanet.Items.Count; i++)
							{
								if ((string)cboPlanet.Items[i] == vals[1])
								{
									cboPlanet.SelectedIndex = i;
									break;
								}
							}
						}
						else // = 1: 은하 인덱스(자원 모으기)
						{
							cboContent.SelectedIndex = int.Parse(vals[0]);

							cboGalaxy.Visible = true;
							cboGalaxy.SelectedIndex = int.Parse(vals[1]);
						}
					}
					else	// 원정(이전 버전)
					{
						cboContent.SelectedIndex = int.Parse(vals[0]);
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
				string val = "[R]" + cboContent.SelectedIndex + "^";
				if (cboGalaxy.Visible)
					val += cboGalaxy.SelectedIndex;
				else if (cboPlanet.Visible)
					val += cboPlanet.Items[cboPlanet.SelectedIndex];
				else if (cboExpedition.Visible)
					val += "E" + cboExpedition.SelectedIndex;

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
				case 0:
					cboGalaxy.Visible = false;
					cboPlanet.Visible = false;
					cboExpedition.Visible = true;
					break;
				case 1:
					cboGalaxy.Visible = true;
					cboPlanet.Visible = false;
					cboExpedition.Visible = false;
					break;
				case 2:
					cboGalaxy.Visible = false;
					cboPlanet.Visible = true;
					cboExpedition.Visible = false;
					break;
			}
		}
	}
}