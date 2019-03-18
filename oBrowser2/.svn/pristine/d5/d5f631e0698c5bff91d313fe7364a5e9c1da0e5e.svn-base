using System;
using System.Text;
using System.Windows.Forms;

namespace oBrowser2
{
	internal partial class frmFleetAttack : Form
	{
		private ResourceInfo[] _res;
		private string _uniName;
		private FleetMoveInfo fleetInfo;
		private NameObjectCollection fleetInfo2;

		private bool formLoaded;
		private int itemID;
		private bool toBeSaved;

		internal frmFleetAttack()
		{
			InitializeComponent();

			_res = null;
			itemID = -1;

			fleetInfo = null;
			fleetInfo2 = null;

			formLoaded = false;
			toBeSaved = false;
		}

		internal ResourceInfo[] ResourceInfos
		{
			set { _res = value; }
		}

		internal int ItemID
		{
			get { return itemID; }
			set { itemID = value; }
		}

		internal string UniverseName
		{
			set { _uniName = value; }
		}

		private void frmFleetAttack_Load(object sender, EventArgs e)
		{
			if (_res == null)
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 예약:함대 기동 설정을 할 수 없는 상태입니다.",
								  "예약:함대 기동 설정 장애 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				Close();
				return;
			}

			// 환경설정에서 함대 기동값 읽기
			loadFleetMoveSettings();

			// 환경설정에서 함대 기동값2 읽기
			fleetInfo2 = OB2Util.GetFleetMoveInfo2();

			// 아이디 설정
			if (itemID != -1)
			{
				string sID = itemID.ToString();
				if ((fleetInfo2 != null) && fleetInfo2.ContainsKey(sID))
					fleetInfo = (FleetMoveInfo) fleetInfo2[sID];

				optUserDefine.Checked = true;
			}
			else
			{
				optDefault.Checked = true;
			}

			cboPlanet.Enabled = optUserDefine.Checked;
			cboTarget.Enabled = optUserDefine.Checked;
			cboTargetType.Enabled = optUserDefine.Checked;
			cboMoveType.Enabled = optUserDefine.Checked;
			cboSpeed.Enabled = optUserDefine.Checked;
			chkMoveEvent.Enabled = optUserDefine.Checked;
			chkMoveRes.Enabled = optUserDefine.Checked;
			txtRDeuterium.Enabled = optUserDefine.Checked;

			// 출발 행성 좌표 목록 채우기
			cboPlanet.Items.Clear();
			cboPlanet.Items.Add(new NVItem("", ""));

			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				if ((_res[i].ColonyName.IndexOf("(달)") > 0) || (_res[i].ColonyName.Trim() == "달"))
				{
					cboPlanet.Items.Add(new NVItem(_res[i].Location + " (달)", _res[i].ColonyID));

					if ((fleetInfo != null) && (fleetInfo.PlanetCoordinate == _res[i].Location + " (달)"))
						selectedIndex = i + 1;
				}
				else
				{
					cboPlanet.Items.Add(new NVItem(_res[i].Location, _res[i].ColonyID));

					if ((fleetInfo != null) && (fleetInfo.PlanetCoordinate == _res[i].Location))
						selectedIndex = i + 1;
				}
			}
			cboPlanet.SelectedIndex = selectedIndex;

			// 도착 행성 좌표 목록 채우기
			cboTarget.Items.Clear();

			bool isExist = false;
			selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				// 좌표가 없는 경우에만 채워줌(달 고려)
				if (!cboTarget.Items.Contains(_res[i].Location))
				{
					cboTarget.Items.Add(_res[i].Location);
					if ((fleetInfo != null) && (fleetInfo.TargetCoords == _res[i].Location))
					{
						selectedIndex = cboTarget.Items.Count - 1;
						isExist = true;
					}
				}
			}
			if (isExist) cboTarget.SelectedIndex = selectedIndex;
			if (!isExist && (fleetInfo != null) && !string.IsNullOrEmpty(fleetInfo.TargetCoords))
			{
				cboTarget.Items.Add(fleetInfo.TargetCoords);
				cboTarget.SelectedIndex = cboTarget.Items.Count - 1;
			}

			// 도착 행성 형태
			cboTargetType.Items.Clear();
			cboTargetType.Items.Add(new NVItem("행성", "1"));
			cboTargetType.Items.Add(new NVItem("파편지대", "2"));
			cboTargetType.Items.Add(new NVItem("달", "3"));
			cboTargetType.SelectedIndex = 0;

			if (fleetInfo != null)
			{
				for (int i = 0; i < cboTargetType.Items.Count; i++)
				{
					if (((NVItem) cboTargetType.Items[i]).Value == fleetInfo.TargetType.ToString())
					{
						cboTargetType.SelectedIndex = i;
						break;
					}
				}
			}

			// 비행 형태 채우기
			cboMoveType.Items.Clear();
			cboMoveType.Items.Add(new NVItem("운송", "3"));
			cboMoveType.Items.Add(new NVItem("배치", "4"));
			cboMoveType.Items.Add(new NVItem("공격", "1"));
			cboMoveType.Items.Add(new NVItem("정탐", "6"));
			cboMoveType.Items.Add(new NVItem("식민", "7"));
			cboMoveType.Items.Add(new NVItem("수확", "8"));
			cboMoveType.Items.Add(new NVItem("원정", "15"));
			cboMoveType.SelectedIndex = 0;

			if (fleetInfo != null)
			{
				for (int i = 0; i < cboMoveType.Items.Count; i++)
				{
					if (((NVItem) cboMoveType.Items[i]).Value == fleetInfo.MoveType.ToString())
					{
						cboMoveType.SelectedIndex = i;
						break;
					}
				}
			}

			// 속도 항목 채우기
			cboSpeed.Items.Clear();
			cboSpeed.Items.Add(new NVItem("100%", "10"));
			cboSpeed.Items.Add(new NVItem("90%", "9"));
			cboSpeed.Items.Add(new NVItem("80%", "8"));
			cboSpeed.Items.Add(new NVItem("70%", "7"));
			cboSpeed.Items.Add(new NVItem("60%", "6"));
			cboSpeed.Items.Add(new NVItem("50%", "5"));
			cboSpeed.Items.Add(new NVItem("40%", "4"));
			cboSpeed.Items.Add(new NVItem("30%", "3"));
			cboSpeed.Items.Add(new NVItem("20%", "2"));
			cboSpeed.Items.Add(new NVItem("10%", "1"));
			cboSpeed.SelectedIndex = 0;

			if (fleetInfo != null)
			{
				for (int i = 0; i < cboSpeed.Items.Count; i++)
				{
					if (((NVItem) cboSpeed.Items[i]).Value == fleetInfo.Speed.ToString())
					{
						cboSpeed.SelectedIndex = i;
						break;
					}
				}
			}

			// 함대 구성 채우기
			if ((fleetInfo != null) && (fleetInfo.Fleets.Count > 0))
				initFleetDropdown(cboFleet1.Parent, fleetInfo.Fleets.Count);
			else
				initFleetDropdown(cboFleet1.Parent, 0);

			// 함대 구성 활성화 상태 변경
			if (optDefault.Checked)
			{
				cboFleet1.Enabled = false;
				cboFleet2.Enabled = false;
				cboFleet3.Enabled = false;
				cboFleet4.Enabled = false;
				cboFleet5.Enabled = false;
				cboFleet6.Enabled = false;
				cboFleet7.Enabled = false;
				txtFleetNum1.Enabled = false;
				txtFleetNum2.Enabled = false;
				txtFleetNum3.Enabled = false;
				txtFleetNum4.Enabled = false;
				txtFleetNum5.Enabled = false;
				txtFleetNum6.Enabled = false;
				txtFleetNum7.Enabled = false;
			}

			if (fleetInfo != null)
			{
				// 자원 운송 여부 채우기
				chkMoveRes.Checked = fleetInfo.MoveResource;

				// 남길 듀테륨 채우기
				txtRDeuterium.Value = fleetInfo.RemainDeuterium;

				// 이벤트 체크 채우기
				chkMoveEvent.Checked = fleetInfo.AddEvent;
			}

			formLoaded = true;
		}

		private void loadFleetMoveSettings()
		{
			string fleetMoving = SettingsHelper.Current.FleetMoving;
			if (string.IsNullOrEmpty(fleetMoving)) return;

			string[] s1 = fleetMoving.Split(new char[] {'|'});
			if (s1.Length < 9) return;

			if (fleetInfo == null) fleetInfo = new FleetMoveInfo();

			fleetInfo.PlanetCoordinate = s1[0]; // 좌표 {(달)}
			fleetInfo.TargetCoords = s1[1];
			fleetInfo.TargetType = s1[2].Trim().Length > 0 ? int.Parse(s1[2]) : 1; // 기본값: 1-행성
			fleetInfo.MoveType = s1[3].Trim().Length > 0 ? int.Parse(s1[3]) : 3; // 기본값: 3-운송
			fleetInfo.Speed = s1[4].Trim().Length > 0 ? int.Parse(s1[4]) : 10; // 기본값: 100%-10
			fleetInfo.MoveResource = (s1[5] == "1"); // 자원 운송 여부
			fleetInfo.RemainDeuterium = s1[6].Trim().Length > 0 ? int.Parse(s1[6]) : 0; // 기본값: 0
			fleetInfo.AddEvent = (s1[7] == "1"); // 함대 기동 이벤트

			fleetInfo.Fleets.Clear();

			string[] s2 = s1[8].Split(new char[] {'^'});
			for (int i = 0; i < s2.Length; i++)
			{
				string[] s3 = s2[i].Split(new char[] {'&'});
				if (s3.Length == 2) fleetInfo.Fleets.Add(s3[0], s3[1]);
			}
		}

		private static FleetMoveInfo getFleetMoveInfo(string fleetString)
		{
			string[] s1 = fleetString.Split(new char[] {'|'});
			if (s1.Length < 10) return null;

			FleetMoveInfo info = new FleetMoveInfo();
			info.PlanetCoordinate = s1[1]; // 좌표 {(달)}
			info.TargetCoords = s1[2];
			info.TargetType = s1[3].Trim().Length > 0 ? int.Parse(s1[3]) : 1; // 기본값: 1-행성
			info.MoveType = s1[4].Trim().Length > 0 ? int.Parse(s1[4]) : 3; // 기본값: 3-운송
			info.Speed = s1[5].Trim().Length > 0 ? int.Parse(s1[5]) : 10; // 기본값: 100%-10
			info.MoveResource = (s1[6] == "1"); // 자원 운송 여부
			info.RemainDeuterium = s1[7].Trim().Length > 0 ? int.Parse(s1[7]) : 0; // 기본값: 0
			info.AddEvent = (s1[8] == "1"); // 함대 기동 이벤트

			info.Fleets.Clear();

			string[] s2 = s1[9].Split(new char[] {'^'});
			for (int i = 0; i < s2.Length; i++)
			{
				string[] s3 = s2[i].Split(new char[] {'&'});
				if (s3.Length == 2) info.Fleets.Add(s3[0], s3[1]);
			}

			return info;
		}

		private void saveFleetMoveSettings2()
		{
			if (optDefault.Checked) // 저장된 설정이 있으면 삭제한다.
			{
				if (itemID != -1)
				{
					string key = itemID.ToString();
					if ((fleetInfo2 != null) && fleetInfo2.ContainsKey(key)) fleetInfo2.Remove(key);
					itemID = -1;
				}
			}
			else
			{
				if (itemID == -1)
				{
					Random r = new Random();

					while (true)
					{
						itemID = r.Next(0, 200);
						if ((fleetInfo2 == null) || !fleetInfo2.ContainsKey(itemID.ToString())) break;
					}
				}

				string id = itemID.ToString();

				StringBuilder sb = new StringBuilder();
				sb.Append(id).Append("|");
				sb.Append(((NVItem) cboPlanet.SelectedItem).Text).Append("|");
				sb.Append(cboTarget.Text).Append("|");
				sb.Append(((NVItem) cboTargetType.SelectedItem).Value).Append("|");
				sb.Append(((NVItem) cboMoveType.SelectedItem).Value).Append("|");
				sb.Append(((NVItem) cboSpeed.SelectedItem).Value).Append("|");
				sb.Append(chkMoveRes.Checked ? "1" : "0").Append("|");
				sb.Append(txtRDeuterium.Value.ToString()).Append("|");
				sb.Append(chkMoveEvent.Checked ? "1" : "0").Append("|");

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

				if (fleetInfo2 == null) fleetInfo2 = new NameObjectCollection();
				if (fleetInfo2.ContainsKey(id))
					fleetInfo2[id] = getFleetMoveInfo(sb.ToString());
				else
					fleetInfo2.Add(id, getFleetMoveInfo(sb.ToString()));
			}

			SettingsHelper settings = SettingsHelper.Current;
			settings.FleetMoving2 = OB2Util.FleetMoveInfo2ToString(fleetInfo2);
			settings.Changed = true;
			// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로

			if (((NVItem) cboMoveType.SelectedItem).Value == "15") // 원정
			{
				MessageBoxEx.Show("함대 기동을 '원정'으로 설정하면 탐사시간은 원정 옵션의 시간이 적용됩니다.",
								  "예약:함대 기동 설정 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Information,
				                  10*1000);
			}
		}

		private void cmdSave_Click(object sender, EventArgs e)
		{
			toBeSaved = true;
			saveFleetMoveSettings2();
			DialogResult = DialogResult.OK;
			Close();
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
						txtFleetNum = (NumericUpDown) c;
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

							if ((fleetInfo != null) && (fleetInfo.Fleets.GetKey(i - 1) == OB2Util.FLEET_IDS[k]))
								selectedIndex = k;
						}
						cboFleet.SelectedIndex = selectedIndex;
						txtFleetNum.Value = decimal.Parse(fleetInfo.Fleets[OB2Util.FLEET_IDS[selectedIndex]]);

						// 앞 함대구성에서 이미 선택된 함대 빼기
						for (int j = 0; j < fleetInfo.Fleets.Count; j++)
						{
							for (int k = 0; k < cboFleet.Items.Count; k++)
							{
								if (j >= i - 1) break;
								if ((fleetInfo != null) && (fleetInfo.Fleets.GetKey(j) == ((NVItem) cboFleet.Items[k]).Value))
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
			if (num.Text == "") num.Value = 0;
			fillNextFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void txtFleetNums_KeyUp(object sender, KeyEventArgs e)
		{
			NumericUpDown num = (NumericUpDown) sender;
			if (num.Text == "") num.Value = 0;
			fillNextFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void chkMoveRes_CheckedChanged(object sender, EventArgs e)
		{
			txtRDeuterium.Enabled = chkMoveRes.Checked;
		}

		private void frmFleetAttack_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!toBeSaved) DialogResult = DialogResult.Cancel;
			Hide();
		}

		private void option_CheckedChanged(object sender, EventArgs e)
		{
			if (!formLoaded) return;

			cboPlanet.Enabled = optUserDefine.Checked;
			cboTarget.Enabled = optUserDefine.Checked;
			cboTargetType.Enabled = optUserDefine.Checked;
			cboMoveType.Enabled = optUserDefine.Checked;
			cboSpeed.Enabled = optUserDefine.Checked;
			chkMoveEvent.Enabled = optUserDefine.Checked;
			chkMoveRes.Enabled = optUserDefine.Checked;
			txtRDeuterium.Enabled = optUserDefine.Checked;

			if (optUserDefine.Checked)
			{
				cboFleet1.Enabled = true;
				txtFleetNum1.Enabled = true;

				initFleetDropdown(cboFleet1.Parent, fleetInfo.Fleets.Count);
			}
			else
			{
				cboFleet1.Enabled = false;
				cboFleet2.Enabled = false;
				cboFleet3.Enabled = false;
				cboFleet4.Enabled = false;
				cboFleet5.Enabled = false;
				cboFleet6.Enabled = false;
				cboFleet7.Enabled = false;
				txtFleetNum1.Enabled = false;
				txtFleetNum2.Enabled = false;
				txtFleetNum3.Enabled = false;
				txtFleetNum4.Enabled = false;
				txtFleetNum5.Enabled = false;
				txtFleetNum6.Enabled = false;
				txtFleetNum7.Enabled = false;
			}
		}
	}
}
