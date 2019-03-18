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

		private string r_crystal;
		private string r_deuterium;
		private string r_metal;

		private bool formLoaded;

		public frmMission()
		{
			InitializeComponent();

			naviURL = null;
			contextCookies = "";
			_res = null;
			fromLocation = "";
			planetType = 1;

			r_metal = "";
			r_crystal = "";
			r_deuterium = "";
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

		/*
		private bool getResourcesOnPlanet(string sHtml)
		{
			// 함대1 페이지가 맞는지 확인
			int pos = sHtml.ToLower().IndexOf("함대를 선택하십시오:");
			if (pos >= 0)
			{
				pos = sHtml.IndexOf("에너지");

				for (int i = 0; i < 3; i++)
				{
					pos = sHtml.IndexOf("=\"90\">", pos + 1);
					int pos2 = sHtml.ToLower().IndexOf("</td", pos + 1);
					string sTemp = sHtml.Substring(pos + 6, pos2 - pos - 6);
					if (sTemp.ToLower().IndexOf("#ff0000") > 0)
					{
						int p1 = sTemp.IndexOf(">");
						int p2 = sTemp.IndexOf("</", p1 + 1);
						sTemp = sTemp.Substring(p1 + 1, p2 - p1 - 1) + "(*)";
					}
					else if (sTemp.ToLower().IndexOf("<font >") >= 0) // 새로 변경된 페이지 자원 표시
					{
						int p1 = sTemp.IndexOf(">");
						int p2 = sTemp.IndexOf("</", p1 + 1);
						sTemp = sTemp.Substring(p1 + 1, p2 - p1 - 1);
					}

					switch (i)
					{
						case 0:
							r_metal = sTemp.Replace(".", "");
							break;
						case 1:
							r_crystal = sTemp.Replace(".", "");
							break;
						case 2:
							r_deuterium = sTemp.Replace(".", "");
							break;
					}
				}

				return true;
			}
			else
			{
				Logger.Log("함대1 페이지 처리 실패: " + sHtml);
				return false;
			}
		}

		// 원정 보내기
		private bool sendFleets(string sessID, string from, NameValueCollection allFleets, string fleets, int expTime,
								out string errMsg)
		{
			errMsg = "";
			string referer = "http://" + naviURL.Host + "/game/index.php?page=flotten1&session=" + sessID + "&mode=Flotte";
			string postURL1 = "http://" + naviURL.Host + "/game/index.php?page=flotten2&session=" + sessID;
			string postURL2 = "http://" + naviURL.Host + "/game/index.php?page=flotten3&session=" + sessID;
			string postURL3 = "http://" + naviURL.Host + "/game/index.php?page=flottenversand&session=" + sessID;

			string[] coords = from.Split(new char[] {':'});

			string postData1 = OB2Util.GetFleetsAllOnPlanetText(allFleets);

			string postData2 = string.Format("thisgalaxy={0}&thissystem={1}&thisplanet={2}" +
											 "&thisplanettype={7}&speedfactor=1&thisresource1={3}&thisresource2={4}" +
											 "&thisresource3={5}&{6}&galaxy={0}&system={1}&planet=16&planettype=1" +
											 "&speed=10",
											 coords[0],
											 coords[1],
											 coords[2],
											 r_metal,
											 r_crystal,
											 r_deuterium,
											 fleets,
											 planetType);

			string postData3 = string.Format("thisgalaxy={0}&thissystem={1}&thisplanet={2}" +
											 "&thisplanettype={8}&speedfactor=1&thisresource1={3}&thisresource2={4}" +
											 "&thisresource3={5}&galaxy={0}&system={1}&planet=16&planettype=1&{6}" +
											 "&speed=10&order=15&resource1=&resource2=&resource3=&expeditiontime={7}",
											 coords[0],
											 coords[1],
											 coords[2],
											 r_metal,
											 r_crystal,
											 r_deuterium,
											 fleets,
											 expTime,
											 planetType);

			Uri newUri = new Uri(postURL1);
			string ret = WebCall.Post(newUri, referer, contextCookies, postData1);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL2);
			ret = WebCall.Post(newUri, postURL1, contextCookies, postData2);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			r = new Random();
			num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL3);
			ret = WebCall.Post(newUri, postURL2, contextCookies, postData3);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			int pos = ret.IndexOf("함대를 보내실 수 없습니다!");
			if (pos < 0)
				pos = ret.IndexOf("<span class=\"error\">");

			if (pos > 0)
			{
				errMsg = "..." + ret.Substring(pos, 512) + "...";
				return false;
			}

			return true;
		}

		public bool GoMission(ref bool retry)
		{
			if ((naviURL == null) || (_res == null))
			{
				Logger.Log("WARNING: 원정 장애1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정을 출발할 수 없는 상태입니다.",
								  "원정 장애1 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10*1000);
				return false;
			}

			loadExpeditionSettings();

			if (string.IsNullOrEmpty(fromLocation) && (expInfo != null))
			{
				fromLocation = expInfo.PlanetCoordinate;
				if (fromLocation.IndexOf("(달)") > 0)
				{
					fromLocation = fromLocation.Replace(" (달)", "");
					planetType = 3;
				}
			}
			Logger.Log("[원정] 출발지: " + fromLocation + (planetType == 3 ? " (달)" : ""));

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) break;

				if (fromLocation == _res[i].Location)
				{
					planetID = _res[i].ColonyID;
					break;
				}
			}

			if (planetID == "")
			{
				Logger.Log("WARNING: 원정 장애2");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정을 출발할 수 없는 상태입니다.",
								  "원정 장애2 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10*1000);
				return false;
			}

			string sessID = OB2Util.GetSessID(naviURL);
			string url = "http://" + naviURL.Host + "/game/index.php?page=flotten1&session=" + sessID + "&cp=" + planetID +
						 "&mode=Flotte&gid=&messageziel=&re=0";
			Uri newUri = new Uri(url);
			string sHtml = WebCall.GetHtml(newUri, contextCookies);

			if (fromLocation != OB2Util.GetCoordinate(sHtml))
			{
				Logger.Log("WARNING: 원정 장애3");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정을 출발할 수 없는 상태입니다.",
								  "원정 장애3 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10*1000);
				return false;
			}

			if (!getResourcesOnPlanet(sHtml))
			{
				Logger.Log("WARNING: 원정 장애4");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정을 출발할 수 없는 상태입니다.",
								  "원정 장애4 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10*1000);
				return false;
			}

			NameValueCollection allFleets = OB2Util.GetAllFleetsOnPlanet(sHtml);

			int maxSpeed = OB2Util.GetMaxSpeed(allFleets, expInfo.Fleets);
			int pos = fromLocation.LastIndexOf(":");
			string toLocation = fromLocation.Substring(0, pos + 1) + "16";
			int distance = OB2Util.GetDistance(fromLocation, toLocation);
			int duration = OB2Util.GetDuration(maxSpeed, 10, distance);

			int[] useFleet = new int[expInfo.Fleets.Count];
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < expInfo.Fleets.Count; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string key = expInfo.Fleets.GetKey(i);
				int num;
				if (!int.TryParse(expInfo.Fleets[key], out num)) continue;

				string fleets = OB2Util.GetFleetsText(allFleets, key, num, out useFleet[i]);
				sb.Append(fleets);

				string fleetName = key;
				for (int kk = 0; kk < OB2Util.FLEET_IDS.Length; kk++)
				{
					if (OB2Util.FLEET_IDS[kk].Equals(key))
					{
						fleetName = OB2Util.FLEET_NAMES[kk];
						break;
					}
				}
				Logger.Log("[원정] 함대: " + fleetName + "(" + key + ") - " + useFleet[i] + " 대");
			}
			Logger.Log("[원정] 최대속도: " + maxSpeed + ", 거리:" + distance + ", 시간: " +
					   duration + " (" + OB2Util.GetTime(duration) + ")");

			// 선택된 함대의 듀테륨 소모량 계산
			double consumption = 0;
			for (int i = 0; i < expInfo.Fleets.Count; i++)
			{
				consumption += OB2Util.GetConsumption(allFleets, expInfo.Fleets.GetKey(i), useFleet[i], distance, duration);
			}
			consumption = Math.Round(consumption) + 1;
			Logger.Log("[원정-이동] 듀테륨 소모량: " + consumption);
	
			// 선택된 함대의 탐사시간당 듀테륨 소모량 계산
			double consumption2 = 0;
			for (int i = 0; i < expInfo.Fleets.Count; i++)
			{
				consumption2 += OB2Util.GetExpConsumption(allFleets, expInfo.Fleets.GetKey(i), useFleet[i], expInfo.ExpeditionTime);
			}
			consumption2 = Math.Round(consumption2) + 1;
			Logger.Log("[원정-" + expInfo.ExpeditionTime + "시간 탐사] 듀테륨 소모량: " + consumption2);
			Logger.Log("[원정-전체] 듀테륨 소모량: " + (consumption + consumption2));
			Logger.Log("------------------------------------------------");

			if (sb.Length == 0)
			{
				Logger.Log("WARNING: 원정 - 출발할 원정함대가 없습니다.");
				MessageBoxEx.Show("출발할 원정함대가 없습니다.",
								  "원정 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10*1000);
				return false;
			}

			string errMsg;
			if (!sendFleets(sessID, fromLocation, allFleets, sb.ToString(), expInfo.ExpeditionTime, out errMsg))
			{
				Logger.Log("WARNING: 원정 장애5");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 원정을 출발할 수 없는 상태입니다.\r\n\r\n상세정보:[" + errMsg + "]",
								  "원정 장애5 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10*1000);
				if (errMsg.IndexOf("너무나 많은 원정") < 0) retry = true;
				return false;
			}

			// 원정 귀환 이벤트 추가 여부
			if (expInfo.AddEvent)
			{
				SortedList<DateTime, string> list = OB2Util.LoadAlarmSettings();
				DateTime picker = DateTime.Now.AddSeconds(duration*2 + expInfo.ExpeditionTime*60*60);
				DateTime key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute);

				string evt = "원정함대 귀환";
				if (list == null) list = new SortedList<DateTime, string>();
				if (!list.ContainsKey(key))
				{
					bool isSkip = false;
					for (int i = 0; i < list.Count; i++)
					{
						if (list.Values[i].Equals(evt))
						{
							// 시간 비교: 더 나중 시간이 들어있으면 건너뛴다.
							if (key < list.Keys[i])
							{
								isSkip = true;
								break;
							}
							list.RemoveAt(i);
							break;
						}
					}
					if (!isSkip) list.Add(key, evt);
				}

				OB2Util.SaveAlarmSettings(list);
			}
			return true;
		}
		*/

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
