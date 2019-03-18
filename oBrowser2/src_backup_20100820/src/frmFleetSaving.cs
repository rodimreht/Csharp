using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class frmFleetSaving : Form
	{
		private ResourceInfo[] _res;
		private string contextCookies;
		private ResCollectingInfo flSavInfo;
		private string fromLocation;
		private Uri naviURL;
		private int planetType;
		private string _uniName;

		private string r_crystal;
		private string r_deuterium;
		private string r_metal;

		public frmFleetSaving()
		{
			InitializeComponent();

			naviURL = null;
			contextCookies = "";
			fromLocation = "";
			planetType = 1;
			_res = null;

			r_metal = "";
			r_crystal = "";
			r_deuterium = "";
			flSavInfo = null;
		}

		public Uri NaviURL
		{
			set { naviURL = value; }
		}

		public string ContextCookies
		{
			set { contextCookies = value; }
		}

		public string FromLocation
		{
			set { fromLocation = value; }
		}

		public int PlanetType
		{
			set { planetType = value; }
		}

		public string UniverseName
		{
			set { _uniName = value; }
		}

		public ResourceInfo[] ResourceInfos
		{
			set { _res = value; }
		}

		private void frmFleetSaving_Load(object sender, EventArgs e)
		{
			if ((naviURL == null) || (_res == null))
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙 설정을 할 수 없는 상태입니다.",
								  "플릿 세이빙 설정 장애 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				Close();
				return;
			}

			// 환경설정에서 플릿 세이빙값 읽기
			loadFleetSavingSettings();

			// 행성 좌표 목록 채우기
			cboPlanet.Items.Clear();

			bool isExist = false;
			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				cboPlanet.Items.Add(_res[i].Location);

				if ((flSavInfo != null) && (flSavInfo.PlanetCoordinate == _res[i].Location))
				{
					selectedIndex = i;
					isExist = true;
				}
			}
			if (isExist) cboPlanet.SelectedIndex = selectedIndex;
			if (!isExist && (flSavInfo != null) && !string.IsNullOrEmpty(flSavInfo.PlanetCoordinate))
			{
				cboPlanet.Items.Add(flSavInfo.PlanetCoordinate);
				cboPlanet.SelectedIndex = cboPlanet.Items.Count - 1;
			}

			// 행성 형태
			cboPlanetType.Items.Clear();
			cboPlanetType.Items.Add(new NVItem("행성", "1"));
			cboPlanetType.Items.Add(new NVItem("파편지대", "2"));
			cboPlanetType.Items.Add(new NVItem("달", "3"));
			cboPlanetType.SelectedIndex = 0;

			if (flSavInfo != null)
			{
				for (int i = 0; i < cboPlanetType.Items.Count; i++)
				{
					if (((NVItem) cboPlanetType.Items[i]).Value == flSavInfo.PlanetType.ToString())
					{
						cboPlanetType.SelectedIndex = i;
						break;
					}
				}
			}

			// 플릿 세이빙 형태 채우기
			cboMoveType.Items.Clear();
			cboMoveType.Items.Add(new NVItem("운송", "3"));
			cboMoveType.Items.Add(new NVItem("배치", "4"));
			cboMoveType.Items.Add(new NVItem("공격", "1"));
			cboMoveType.Items.Add(new NVItem("정탐", "6"));
			cboMoveType.Items.Add(new NVItem("수확", "8"));
			cboMoveType.SelectedIndex = 0;

			if (flSavInfo != null)
			{
				for (int i = 0; i < cboMoveType.Items.Count; i++)
				{
					if (((NVItem) cboMoveType.Items[i]).Value == flSavInfo.MoveType.ToString())
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
			cboSpeed.SelectedIndex = 9;

			if (flSavInfo != null)
			{
				for (int i = 0; i < cboSpeed.Items.Count; i++)
				{
					if (((NVItem) cboSpeed.Items[i]).Value == flSavInfo.Speed.ToString())
					{
						cboSpeed.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void saveFleetSavingSettings()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(cboPlanet.Text).Append("|");
			sb.Append(((NVItem) cboPlanetType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboMoveType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboSpeed.SelectedItem).Value);

			SettingsHelper settings = SettingsHelper.Current;
			settings.FleetSaving = sb.ToString();
			settings.Changed = true;
			//settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
		}

		private void loadFleetSavingSettings()
		{
			string flSav = SettingsHelper.Current.FleetSaving;
			if (string.IsNullOrEmpty(flSav)) return;

			string[] s1 = flSav.Split(new char[] {'|'});
			if (s1.Length < 4) return;

			if (flSavInfo == null) flSavInfo = new ResCollectingInfo();

			flSavInfo.PlanetCoordinate = s1[0];
			flSavInfo.PlanetType = s1[1].Trim().Length > 0 ? int.Parse(s1[1]) : 1; // 기본값: 1-행성
			flSavInfo.MoveType = s1[2].Trim().Length > 0 ? int.Parse(s1[2]) : 3; // 기본값: 3-운송
			flSavInfo.Speed = s1[3].Trim().Length > 0 ? int.Parse(s1[3]) : 1; // 기본값: 10%-1
		}

		private void cmdSave_Click(object sender, EventArgs e)
		{
			saveFleetSavingSettings();
			Close();
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
					if ((sTemp.ToLower().IndexOf("#ff0000") > 0) || (sTemp.ToLower().IndexOf("<font >") >= 0)) // 새로 변경된 페이지 자원 표시
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

		private int getTotalResources()
		{
			int total = 0;
			int r;
			if (!int.TryParse(r_metal, out r)) r = 0;
			total += r;
			if (!int.TryParse(r_crystal, out r)) r = 0;
			total += r;
			if (!int.TryParse(r_deuterium, out r)) r = 0;
			total += r;
			return total;
		}

		// 플릿 세이빙
		private bool startFleetSaving(string sessID, NameValueCollection allFleets, string fleets,
		                              int remainResources, int consumption, out string errMsg)
		{
			errMsg = "";
			string referer = "http://" + naviURL.Host + "/game/index.php?page=flotten1&session=" + sessID + "&mode=Flotte";
			string postURL1 = "http://" + naviURL.Host + "/game/index.php?page=flotten2&session=" + sessID;
			string postURL2 = "http://" + naviURL.Host + "/game/index.php?page=flotten3&session=" + sessID;
			string postURL3 = "http://" + naviURL.Host + "/game/index.php?page=flottenversand&session=" + sessID;

			string[] fromCoords = fromLocation.Split(new char[] {':'});
			string[] toCoords = flSavInfo.PlanetCoordinate.Split(new char[] {':'});

			string postData1 = OB2Util.GetFleetsAllOnPlanetText(allFleets);

			string postData2 = string.Format("thisgalaxy={0}&thissystem={1}&thisplanet={2}" +
			                                 "&thisplanettype={3}&speedfactor=1&thisresource1={4}&thisresource2={5}" +
			                                 "&thisresource3={6}&{7}&galaxy={8}&system={9}&planet={10}&planettype={11}" +
			                                 "&speed={12}",
			                                 fromCoords[0],
			                                 fromCoords[1],
			                                 fromCoords[2],
			                                 planetType,
			                                 r_metal,
			                                 r_crystal,
			                                 r_deuterium,
			                                 fleets,
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 flSavInfo.PlanetType,
			                                 flSavInfo.Speed);

			int met;
			int cryst;
			int deut;
			if (!int.TryParse(r_metal, out met)) met = 0;
			if (!int.TryParse(r_crystal, out cryst)) cryst = 0;
			if (!int.TryParse(r_deuterium, out deut)) deut = 0;

			deut -= consumption;
			if (deut < 0) deut = 0;

			// 함대 저장공간이 부족해 자원을 다 싣지 못하는 경우
			if (remainResources > 0)
			{
				// 메탈 총량보다 더 많이 남은 경우
				if (met < remainResources)
				{
					remainResources -= met;
					met = 0;

					// 크리스탈 총량보다 더 많이 남은 경우
					if (cryst < remainResources)
					{
						remainResources -= cryst;
						cryst = 0;

						// 듀테륨 총량보다 더 많이 남은 경우
						if (deut < remainResources)
						{
							remainResources -= deut;
							deut = 0;
						}
						else
						{
							deut -= remainResources;
						}
					}
					else
					{
						cryst -= remainResources;
					}
				}
				else
				{
					met -= remainResources;
				}
			}

			string postData3 = string.Format("thisgalaxy={0}&thissystem={1}&thisplanet={2}" +
			                                 "&thisplanettype={3}&speedfactor=1&thisresource1={4}&thisresource2={5}" +
			                                 "&thisresource3={6}&galaxy={7}&system={8}&planet={9}&planettype={10}&{11}" +
			                                 "&speed={12}&order={13}&resource1={14}&resource2={15}&resource3={16}",
			                                 fromCoords[0],
			                                 fromCoords[1],
			                                 fromCoords[2],
			                                 planetType,
			                                 r_metal,
			                                 r_crystal,
			                                 r_deuterium,
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 flSavInfo.PlanetType,
			                                 fleets,
			                                 flSavInfo.Speed,
			                                 flSavInfo.MoveType,
			                                 met,
			                                 cryst,
			                                 deut);

			Uri newUri = new Uri(postURL1);
			string ret = WebCall.Post(newUri, referer, contextCookies, postData1);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			// 최대 0.5초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 10);
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

			// 최대 0.5초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			r = new Random();
			num = r.Next(0, 10);
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

		public bool FleetSaving(ref bool retry)
		{
			if ((naviURL == null) || (_res == null) || string.IsNullOrEmpty(fromLocation))
			{
				Logger.Log("WARNING: 플릿 세이빙 장애1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙을 할 수 없는 상태입니다.",
								  "플릿 세이빙 장애1 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			loadFleetSavingSettings();

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if ((_res[i] == null) || (flSavInfo == null)) break;

				if (fromLocation == _res[i].Location)
				{
					planetID = _res[i].ColonyID;
					break;
				}
			}

			if (planetID == "")
			{
				Logger.Log("WARNING: 플릿 세이빙 장애2");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙을 할 수 없는 상태입니다.",
								  "플릿 세이빙 장애2 - oBrowser2: " + _uniName,
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
				Logger.Log("WARNING: 플릿 세이빙 장애3");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙을 할 수 없는 상태입니다.",
								  "플릿 세이빙 장애3 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			if (!getResourcesOnPlanet(sHtml))
			{
				Logger.Log("WARNING: 플릿 세이빙 장애4");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙을 할 수 없는 상태입니다.",
								  "플릿 세이빙 장애4 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			NameValueCollection allFleets = OB2Util.GetAllFleetsOnPlanet(sHtml);
			string[] fls = OB2Util.GetAllFleets(allFleets);

			int maxSpeed = OB2Util.GetMaxSpeed(allFleets, fls);
			int distance = OB2Util.GetDistance(fromLocation, flSavInfo.PlanetCoordinate);
			int duration = OB2Util.GetDuration(maxSpeed, flSavInfo.Speed, distance);
			Logger.Log("[플릿 세이빙] 최대속도: " + maxSpeed + ", 거리:" + distance + ", 시간: " + duration);

			int resQuantity = getTotalResources();
			double consumption = 0;
			Logger.Log("[플릿 세이빙] 행성 자원 총량: " + resQuantity);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < fls.Length; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				int fleetNum;
				string maxShip = allFleets["maxship" + fls[i]];
				if (!int.TryParse(maxShip, out fleetNum)) fleetNum = 0;

				// 선택된 함대의 듀테륨 소모량 계산
				consumption += OB2Util.GetConsumption(allFleets, fls[i], fleetNum, distance, duration);

				int useFleet;
				string fleets = OB2Util.GetFleetsText(allFleets, fls[i], fleetNum, out useFleet);
				sb.Append(fleets);

				string fleetName = fls[i];
				for (int kk = 0; kk < OB2Util.FLEET_IDS.Length; kk++)
				{
					if (OB2Util.FLEET_IDS[kk].Equals(fls[i]))
					{
						fleetName = OB2Util.FLEET_NAMES[kk];
						break;
					}
				}
				Logger.Log("[플릿 세이빙] 함대: " + fleetName + "(" + fls[i] + ") - " + useFleet + " 대");

				// 남은 자원 계산
				resQuantity -= OB2Util.GetCapacity(fls[i], useFleet);
			}
			consumption = Math.Round(consumption) + 1;
			if (resQuantity < 0) resQuantity = 0;
			Logger.Log("[플릿 세이빙] 듀테륨 소모량: " + consumption + ", 남는 자원 총량: " + resQuantity);

			if (sb.Length == 0)
			{
				Logger.Log("WARNING: 플릿 세이빙 - 출발할 함대가 하나도 없습니다.");
				MessageBoxEx.Show("출발할 함대가 하나도 없습니다.",
								  "플릿 세이빙 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			string errMsg;
			if (!startFleetSaving(sessID, allFleets, sb.ToString(), resQuantity, (int) consumption, out errMsg))
			{
				Logger.Log("WARNING: 플릿 세이빙 장애5");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 플릿 세이빙을 할 수 없는 상태입니다.\r\n\r\n상세정보:[" + errMsg + "]",
								  "플릿 세이빙 장애5 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				retry = true;
				return false;
			}
			return true;
		}
		*/
	}
}