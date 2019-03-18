using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class frmMission : Form
	{
		private ResourceInfo[] _res;
		private string contextCookies;
		private ExpeditionInfo expInfo;
		private string fromLocation;
		private Uri naviURL;
		private int planetType;
		private string _uniName;

		private bool formLoaded;

		public frmMission()
		{
			InitializeComponent();

			naviURL = null;
			contextCookies = "";
			_res = null;
			fromLocation = "";
			planetType = 1;
			expInfo = null;

			formLoaded = false;
		}

		public Uri NaviURL
		{
			set { naviURL = value; }
		}

		public string ContextCookies
		{
			get { return contextCookies; }
			set { contextCookies = string.Copy(value); }
		}

		public ResourceInfo[] ResourceInfos
		{
			set { _res = value; }
		}

		public string FromLocation
		{
			set { fromLocation = string.Copy(value); }
		}

		public int PlanetType
		{
			set { planetType = value; }
		}

		public string UniverseName
		{
			set { _uniName = value; }
		}

		private void frmMission_Load(object sender, EventArgs e)
		{
			if ((naviURL == null) || (_res == null))
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정 설정을 할 수 없는 상태입니다.",
								  "원정 설정 장애 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				Close();
				return;
			}

			// 환경설정에서 원정값 읽기
			loadExpeditionSettings();

			// 행성 좌표 목록 채우기
			cboPlanet.Items.Clear();
			cboPlanet.Items.Add(new NVItem("", ""));

			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				if ((_res[i].ColonyName.IndexOf("(달)") > 0) || (_res[i].ColonyName.Trim() == "달"))
				{
					cboPlanet.Items.Add(new NVItem(_res[i].Location + " (달)", _res[i].ColonyID));

					if ((expInfo != null) && (expInfo.PlanetCoordinate == _res[i].Location + " (달)"))
						selectedIndex = i + 1;
				}
				else
				{
					cboPlanet.Items.Add(new NVItem(_res[i].Location, _res[i].ColonyID));

					if ((expInfo != null) && (expInfo.PlanetCoordinate == _res[i].Location))
						selectedIndex = i + 1;
				}
			}
			cboPlanet.SelectedIndex = selectedIndex;

			// 탐사 시간 채우기
			if ((expInfo != null) && (expInfo.ExpeditionTime > 0))
				txtExpTime.Value = expInfo.ExpeditionTime;

			// 함대 구성 채우기
			if ((expInfo != null) && (expInfo.Fleets.Count > 0))
				initFleetDropdown(cboFleet1.Parent, expInfo.Fleets.Count);
			else
				initFleetDropdown(cboFleet1.Parent, 0);

			// 이벤트 체크 채우기
			chkExpEvent.Checked = expInfo.AddEvent;

			formLoaded = true;
		}

		private void saveExpeditionSettings()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(((NVItem) cboPlanet.SelectedItem).Text).Append("|");
			sb.Append(txtExpTime.Value.ToString()).Append("|");

			int num = (int) txtFleetNum1.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet1.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFleetNum2.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet2.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFleetNum3.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet3.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFleetNum4.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet4.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFleetNum5.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet5.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFleetNum6.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet6.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFleetNum7.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFleet7.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}

			sb.Append("|").Append(chkExpEvent.Checked ? "1" : "0");

			SettingsHelper settings = SettingsHelper.Current;
			settings.Expedition = sb.ToString();
			settings.Changed = true;
			// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
		}

		private void loadExpeditionSettings()
		{
			string expedition = SettingsHelper.Current.Expedition;
			if (string.IsNullOrEmpty(expedition)) return;

			string[] s1 = expedition.Split(new char[] {'|'});
			if (s1.Length < 3) return;

			if (expInfo == null) expInfo = new ExpeditionInfo();

			expInfo.PlanetCoordinate = s1[0];
			expInfo.ExpeditionTime = s1[1].Trim().Length > 0 ? int.Parse(s1[1]) : 1;
			expInfo.Fleets.Clear();

			string[] s2 = s1[2].Split(new char[] {'^'});
			for (int i = 0; i < s2.Length; i++)
			{
				string[] s3 = s2[i].Split(new char[] {'&'});
				if (s3.Length == 2) expInfo.Fleets.Add(s3[0], s3[1]);
			}

			// 원정 귀환 이벤트 추가 여부
			if (s1.Length == 4) expInfo.AddEvent = (s1[3] == "1");
		}

		private void cmdSave_Click(object sender, EventArgs e)
		{
			saveExpeditionSettings();
			Close();
		}

		private void cmdGo_Click(object sender, EventArgs e)
		{
			saveExpeditionSettings();

			frmOption missionForm = new frmOption();
			missionForm.NaviURL = naviURL;
			missionForm.ContextCookies = contextCookies;
			missionForm.ResourceInfos = _res;
			missionForm.UniverseName = _uniName;
			missionForm.FromLocation = fromLocation;
			missionForm.PlanetType = planetType;

			bool retry = false;
			if (missionForm.GoExpedition(ref retry))
			{
				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("원정함대가 출발했습니다.",
								  "원정 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Information,
				                  3*1000);
			}
			if (missionForm.ContextCookies != contextCookies) contextCookies = missionForm.ContextCookies;
		}

		private void initFleetDropdown(Control parent, int count)
		{
			for (int i = 1; i <= 7; i++)
			{
				ComboBox cboFleet = null;
				NumericUpDown txtFleetNum = null;

				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("cboFleet" + i))
					{
						cboFleet = (ComboBox) c;
						break;
					}
				}
				
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("txtFleetNum" + i))
					{
						txtFleetNum = (NumericUpDown)c;
						break;
					}
				}

				if ((cboFleet != null) && (txtFleetNum != null))
				{
					if (count == 0) // 하나도 없을 때
					{
						cboFleet.Items.Clear();
						for (int k = 0; k < OB2Util.FLEET_NAMES.Length; k++)
						{
							cboFleet.Items.Add(new NVItem(OB2Util.FLEET_NAMES[k], OB2Util.FLEET_IDS[k]));
						}
						cboFleet.SelectedIndex = 0;
						Application.DoEvents();
					}
					else if (i > count + 1)
					{
						cboFleet.Enabled = false;
						txtFleetNum.Enabled = false;
					}
					else if (i <= count)
					{
						cboFleet.Items.Clear();
						int selectedIndex = 0;
						for (int k = 0; k < OB2Util.FLEET_NAMES.Length; k++)
						{
							cboFleet.Items.Add(new NVItem(OB2Util.FLEET_NAMES[k], OB2Util.FLEET_IDS[k]));

							if ((expInfo != null) && (expInfo.Fleets.GetKey(i - 1) == OB2Util.FLEET_IDS[k]))
								selectedIndex = k;
						}
						cboFleet.SelectedIndex = selectedIndex;
						txtFleetNum.Value = decimal.Parse(expInfo.Fleets[OB2Util.FLEET_IDS[selectedIndex]]);
						
						// 앞 함대구성에서 이미 선택된 함대 빼기
						for (int j = 0; j < expInfo.Fleets.Count; j++)
						{
							for (int k = 0; k < cboFleet.Items.Count; k++)
							{
								if (j >= i - 1) break;
								if ((expInfo != null) && (expInfo.Fleets.GetKey(j) == ((NVItem)cboFleet.Items[k]).Value))
								{
									cboFleet.Items.RemoveAt(k);
									k--;
								}
							}
							if (cboFleet.SelectedIndex < 0) cboFleet.SelectedIndex = 0;
						}

						Application.DoEvents();
					}
				}
			}
		}

		private static void fillNextFleetDropdown(Control parent, string name, int count)
		{
			int last = int.Parse(name.Substring(name.Length - 1));
			if (last >= 7) return;

			string[] reged = new string[last];
			for (int i = 1; i <= last; i++)
			{
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("cboFleet" + i))
					{
						ComboBox cbo = (ComboBox) c;
						reged[i - 1] = cbo.SelectedIndex < 0 ? "" : ((NVItem) cbo.Items[cbo.SelectedIndex]).Value;
						break;
					}
				}
			}

			while (last < 7)
			{
				last++;

				ComboBox cboFleet = null;
				NumericUpDown txtFleetNum = null;

				string selectedItem = "";
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("cboFleet" + last))
					{
						cboFleet = (ComboBox) c;
						if (cboFleet.SelectedIndex > 0) selectedItem = ((NVItem) cboFleet.Items[cboFleet.SelectedIndex]).Value;
						break;
					}
				}

				int num = 0;
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("txtFleetNum" + last))
					{
						txtFleetNum = (NumericUpDown) c;
						num = (int) txtFleetNum.Value;
						break;
					}
				}

				if ((cboFleet != null) && (txtFleetNum != null))
				{
					if (count == 0)
					{
						if ((selectedItem != "") && (num > 0)) continue;

						cboFleet.Enabled = false;
						txtFleetNum.Enabled = false;
					}
					else
					{
						cboFleet.Enabled = true;
						txtFleetNum.Enabled = true;

						int selectedIndex = 0;
						int cnt = 0;
						cboFleet.Items.Clear();
						for (int k = 0; k < OB2Util.FLEET_IDS.Length; k++)
						{
							bool isSkip = false;
							for (int j = 0; j < reged.Length; j++)
							{
								if ((reged[j] != "") && (reged[j] == OB2Util.FLEET_IDS[k]))
								{
									isSkip = true;
									break;
								}
							}

							if (!isSkip)
							{
								cboFleet.Items.Add(new NVItem(OB2Util.FLEET_NAMES[k], OB2Util.FLEET_IDS[k]));
								if (selectedItem == OB2Util.FLEET_IDS[k]) selectedIndex = cnt;

								cnt++;
							}
						}
						cboFleet.SelectedIndex = selectedIndex;
						break;
					}
				}
			}
		}

		private void cboFleets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!formLoaded) return;

			ComboBox cbo = (ComboBox) sender;
			string name = cbo.Name;
			int last = int.Parse(name.Substring(name.Length - 1));

			for (int i = 7; i >= last; i--)
			{
				foreach (Control c in cbo.Parent.Controls)
				{
					if (c.Name.Equals("txtFleetNum" + i))
					{
						NumericUpDown txtFleetNum = (NumericUpDown) c;
						fillNextFleetDropdown(txtFleetNum.Parent, txtFleetNum.Name, (int) txtFleetNum.Value);
						break;
					}
				}
			}
		}

		private void txtFleetNums_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown num = (NumericUpDown) sender;
			fillNextFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void txtFleetNums_KeyUp(object sender, KeyEventArgs e)
		{
			NumericUpDown num = (NumericUpDown)sender;
			fillNextFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void frmMission_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
		}
	}
}
