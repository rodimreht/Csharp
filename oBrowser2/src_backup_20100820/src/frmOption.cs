﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class frmOption : Form
	{
		private ResourceInfo[] _res;
		private string _uniName;
		private string contextCookies;
		private ExpeditionInfo expInfo;
		private FleetMoveInfo fleetInfo;
		private ResCollectingInfo flSavInfo;
		private ResCollectingInfo resInfo;

		private bool doClose;
		private string fromLocation;
		private bool galaxyMethod;
		private Uri naviURL;
		private int m_planetType;

		private string r_crystal;
		private string r_deuterium;
		private string r_metal;

		private bool resFormLoaded;
		private bool expFormLoaded;
		private bool fsFormLoaded;
		private bool fmFormLoaded;

		public frmOption()
		{
			InitializeComponent();

			doClose = false;
			naviURL = null;
			contextCookies = "";
			fromLocation = "";
			m_planetType = 1;
			_res = null;
			galaxyMethod = false;

			r_metal = "";
			r_crystal = "";
			r_deuterium = "";
			resInfo = null;
			expInfo = null;
			flSavInfo = null;
			fleetInfo = null;

			resFormLoaded = false;
			expFormLoaded = false;
			fsFormLoaded = false;
			fmFormLoaded = false;
		}

		public bool DoClose
		{
			set { doClose = value; }
		}

		public Uri NaviURL
		{
			set { naviURL = value; }
		}

		public string ContextCookies
		{
			get { return contextCookies;  }
			set { contextCookies = string.Copy(value); }
		}

		public string FromLocation
		{
			set { fromLocation = string.Copy(value); }
		}

		public int PlanetType
		{
			set { m_planetType = value; }
		}

		public string UniverseName
		{
			set { _uniName = value; }
		}

		public ResourceInfo[] ResourceInfos
		{
			set { _res = value; }
		}

		public bool GalaxyMethod
		{
			set { galaxyMethod = value; }
		}

		private void frmOption_Load(object sender, EventArgs e)
		{
			if ((naviURL == null) || (_res == null))
			{
				tabPage2.Enabled = false;
				tabPage3.Enabled = false;
				tabPage4.Enabled = false;
				tabPage5.Enabled = false;
			}
			else
			{
				tabPage2.Enabled = true;
				tabPage3.Enabled = true;
				tabPage4.Enabled = true;
				tabPage5.Enabled = true;
			}

			setOptionForm();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			saveOptions();
			if (resFormLoaded) saveResCollectingSettings();
			if (expFormLoaded) saveExpeditionSettings();
			if (fsFormLoaded) saveFleetSavingSettings();
			if (fmFormLoaded) saveFleetMoveSettings();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void setOptionForm()
		{
			SettingsHelper helper = SettingsHelper.Current;
			txtRate.Text = helper.RefreshRate.ToString();
			txtRateMax.Text = helper.RefreshRateMax.ToString();
			if (string.IsNullOrEmpty(helper.OGameDomain))
			{
				txtOGDomain.Text = "kr.ogame.de";
				txtOGDomain.Left = 136;
				label4.Font = new Font(label4.Font, FontStyle.Bold);
			}
			else
			{
				txtOGDomain.Text = helper.OGameDomain;
				txtOGDomain.Left = 122;
				label4.Font = new Font(label4.Font, FontStyle.Regular);
			}
			applySummerTime.Checked = helper.ApplySummerTime;
			useFireFox.Checked = helper.UseFireFox;
			txtFireFoxDir.Text = helper.FireFoxDirectory;
			chkShowInLeftBottom.Checked = !helper.ShowInLeftBottom;

			SmtpMailInfo info = SmtpMailInfo.ParseInfo(helper.SmtpMail);
			if (info != null)
			{
				txtMailAddress.Text = info.MailAddress;
				txtMailServer.Text = info.MailServer;
				txtMailUserID.Text = info.UserID;
				txtMailPassword.Text = info.Pwd;
			}
		}

		private void setResourceForm()
		{
			if ((naviURL == null) || (_res == null))
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 모으기 설정을 할 수 없는 상태입니다.",
								  "자원 모으기 설정 장애 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				Close();
				return;
			}

			// 환경설정에서 자원 운송값 읽기
			loadResCollectingSettings();

			// 행성 좌표 목록 채우기
			cboResPlanet.Items.Clear();

			bool isExist = false;
			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				cboResPlanet.Items.Add(_res[i].Location);

				if ((resInfo != null) && (resInfo.PlanetCoordinate == _res[i].Location))
				{
					selectedIndex = i;
					isExist = true;
				}
			}
			if (isExist) cboResPlanet.SelectedIndex = selectedIndex;
			if (!isExist && (resInfo != null) && !string.IsNullOrEmpty(resInfo.PlanetCoordinate))
			{
				cboResPlanet.Items.Add(resInfo.PlanetCoordinate);
				cboResPlanet.SelectedIndex = cboResPlanet.Items.Count - 1;
			}

			// 행성 형태
			cboResPlanetType.Items.Clear();
			cboResPlanetType.Items.Add(new NVItem("행성", "1"));
			cboResPlanetType.Items.Add(new NVItem("파편지대", "2"));
			cboResPlanetType.Items.Add(new NVItem("달", "3"));
			cboResPlanetType.SelectedIndex = 0;

			if (resInfo != null)
			{
				for (int i = 0; i < cboResPlanetType.Items.Count; i++)
				{
					if (((NVItem) cboResPlanetType.Items[i]).Value == resInfo.PlanetType.ToString())
					{
						cboResPlanetType.SelectedIndex = i;
						break;
					}
				}
			}

			// 자원 운송 형태 채우기
			cboResMoveType.Items.Clear();
			cboResMoveType.Items.Add(new NVItem("운송", "3"));
			cboResMoveType.Items.Add(new NVItem("배치", "4"));
			cboResMoveType.Items.Add(new NVItem("공격", "1"));
			cboResMoveType.Items.Add(new NVItem("정탐", "6"));
			cboResMoveType.Items.Add(new NVItem("수확", "8"));
			cboResMoveType.SelectedIndex = 0;

			if (resInfo != null)
			{
				for (int i = 0; i < cboResMoveType.Items.Count; i++)
				{
					if (((NVItem) cboResMoveType.Items[i]).Value == resInfo.MoveType.ToString())
					{
						cboResMoveType.SelectedIndex = i;
						break;
					}
				}
			}

			// 속도 항목 채우기
			cboResSpeed.Items.Clear();
			cboResSpeed.Items.Add(new NVItem("100%", "10"));
			cboResSpeed.Items.Add(new NVItem("90%", "9"));
			cboResSpeed.Items.Add(new NVItem("80%", "8"));
			cboResSpeed.Items.Add(new NVItem("70%", "7"));
			cboResSpeed.Items.Add(new NVItem("60%", "6"));
			cboResSpeed.Items.Add(new NVItem("50%", "5"));
			cboResSpeed.Items.Add(new NVItem("40%", "4"));
			cboResSpeed.Items.Add(new NVItem("30%", "3"));
			cboResSpeed.Items.Add(new NVItem("20%", "2"));
			cboResSpeed.Items.Add(new NVItem("10%", "1"));
			cboResSpeed.SelectedIndex = 0;

			if (resInfo != null)
			{
				for (int i = 0; i < cboResSpeed.Items.Count; i++)
				{
					if (((NVItem) cboResSpeed.Items[i]).Value == resInfo.Speed.ToString())
					{
						cboResSpeed.SelectedIndex = i;
						break;
					}
				}
			}

			// 함대 구성 채우기
			selectedIndex = 0;
			cboResFleet1.Items.Clear();
			for (int i = 0; i < OB2Util.FLEET_NAMES.Length; i++)
			{
				cboResFleet1.Items.Add(new NVItem(OB2Util.FLEET_NAMES[i], OB2Util.FLEET_IDS[i]));

				if ((resInfo != null) && (resInfo.Fleets.Length > 0) && (resInfo.Fleets[0] == OB2Util.FLEET_IDS[i]))
					selectedIndex = i;
			}
			cboResFleet1.SelectedIndex = selectedIndex;

			selectedIndex = 0;
			cboResFleet2.Items.Clear();
			for (int i = 0; i < OB2Util.FLEET_NAMES.Length; i++)
			{
				cboResFleet2.Items.Add(new NVItem(OB2Util.FLEET_NAMES[i], OB2Util.FLEET_IDS[i]));

				if ((resInfo != null) && (resInfo.Fleets.Length > 1) && (resInfo.Fleets[1] == OB2Util.FLEET_IDS[i]))
					selectedIndex = i;
			}
			cboResFleet2.SelectedIndex = selectedIndex;

			if (resInfo != null)
			{
				txtResRDeuterium.Value = resInfo.RemainDeuterium;
			}

			// 이벤트 체크 채우기
			chkResEvent.Checked = resInfo.AddEvent;
			resFormLoaded = true;
		}

		private void setExpeditionForm()
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
			cboExpPlanet.Items.Clear();
			cboExpPlanet.Items.Add(new NVItem("", ""));

			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				if ((_res[i].ColonyName.IndexOf("(달)") > 0) || (_res[i].ColonyName.Trim() == "달"))
				{
					cboExpPlanet.Items.Add(new NVItem(_res[i].Location + " (달)", _res[i].ColonyID));

					if ((expInfo != null) && (expInfo.PlanetCoordinate == _res[i].Location + " (달)"))
						selectedIndex = i + 1;
				}
				else
				{
					cboExpPlanet.Items.Add(new NVItem(_res[i].Location, _res[i].ColonyID));

					if ((expInfo != null) && (expInfo.PlanetCoordinate == _res[i].Location))
						selectedIndex = i + 1;
				}
			}
			cboExpPlanet.SelectedIndex = selectedIndex;

			// 탐사 시간 채우기
			if ((expInfo != null) && (expInfo.ExpeditionTime > 0))
				txtExpTime.Value = expInfo.ExpeditionTime;

			// 함대 구성 채우기
			if ((expInfo != null) && (expInfo.Fleets.Count > 0))
				initFleetDropdown(cboExpFleet1.Parent, expInfo.Fleets.Count);
			else
				initFleetDropdown(cboExpFleet1.Parent, 0);

			// 이벤트 체크 채우기
			chkExpEvent.Checked = expInfo.AddEvent;

			expFormLoaded = true;
		}

		private void setFleetSavingForm()
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
			cboFSPlanet.Items.Clear();

			bool isExist = false;
			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				cboFSPlanet.Items.Add(_res[i].Location);

				if ((flSavInfo != null) && (flSavInfo.PlanetCoordinate == _res[i].Location))
				{
					selectedIndex = i;
					isExist = true;
				}
			}
			if (isExist) cboFSPlanet.SelectedIndex = selectedIndex;
			if (!isExist && (flSavInfo != null) && !string.IsNullOrEmpty(flSavInfo.PlanetCoordinate))
			{
				cboFSPlanet.Items.Add(flSavInfo.PlanetCoordinate);
				cboFSPlanet.SelectedIndex = cboFSPlanet.Items.Count - 1;
			}

			// 행성 형태
			cboFSPlanetType.Items.Clear();
			cboFSPlanetType.Items.Add(new NVItem("행성", "1"));
			cboFSPlanetType.Items.Add(new NVItem("파편지대", "2"));
			cboFSPlanetType.Items.Add(new NVItem("달", "3"));
			cboFSPlanetType.SelectedIndex = 0;

			if (flSavInfo != null)
			{
				for (int i = 0; i < cboFSPlanetType.Items.Count; i++)
				{
					if (((NVItem) cboFSPlanetType.Items[i]).Value == flSavInfo.PlanetType.ToString())
					{
						cboFSPlanetType.SelectedIndex = i;
						break;
					}
				}
			}

			// 플릿 세이빙 형태 채우기
			cboFSMoveType.Items.Clear();
			cboFSMoveType.Items.Add(new NVItem("운송", "3"));
			cboFSMoveType.Items.Add(new NVItem("배치", "4"));
			cboFSMoveType.Items.Add(new NVItem("공격", "1"));
			cboFSMoveType.Items.Add(new NVItem("정탐", "6"));
			cboFSMoveType.Items.Add(new NVItem("수확", "8"));
			cboFSMoveType.SelectedIndex = 0;

			if (flSavInfo != null)
			{
				for (int i = 0; i < cboFSMoveType.Items.Count; i++)
				{
					if (((NVItem) cboFSMoveType.Items[i]).Value == flSavInfo.MoveType.ToString())
					{
						cboFSMoveType.SelectedIndex = i;
						break;
					}
				}
			}

			// 속도 항목 채우기
			cboFSSpeed.Items.Clear();
			cboFSSpeed.Items.Add(new NVItem("100%", "10"));
			cboFSSpeed.Items.Add(new NVItem("90%", "9"));
			cboFSSpeed.Items.Add(new NVItem("80%", "8"));
			cboFSSpeed.Items.Add(new NVItem("70%", "7"));
			cboFSSpeed.Items.Add(new NVItem("60%", "6"));
			cboFSSpeed.Items.Add(new NVItem("50%", "5"));
			cboFSSpeed.Items.Add(new NVItem("40%", "4"));
			cboFSSpeed.Items.Add(new NVItem("30%", "3"));
			cboFSSpeed.Items.Add(new NVItem("20%", "2"));
			cboFSSpeed.Items.Add(new NVItem("10%", "1"));
			cboFSSpeed.SelectedIndex = 9;

			if (flSavInfo != null)
			{
				for (int i = 0; i < cboFSSpeed.Items.Count; i++)
				{
					if (((NVItem) cboFSSpeed.Items[i]).Value == flSavInfo.Speed.ToString())
					{
						cboFSSpeed.SelectedIndex = i;
						break;
					}
				}
			}

			fsFormLoaded = true;
		}

		private void setFleetMoveForm()
		{
			if ((naviURL == null) || (_res == null))
			{
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대 기동 설정을 할 수 없는 상태입니다.",
								  "함대 기동 설정 장애 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				Close();
				return;
			}

			// 환경설정에서 원정값 읽기
			loadFleetMoveSettings();

			// 출발 행성 좌표 목록 채우기
			cboFMPlanet.Items.Clear();
			cboFMPlanet.Items.Add(new NVItem("", ""));

			int selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				if ((_res[i].ColonyName.IndexOf("(달)") > 0) || (_res[i].ColonyName.Trim() == "달"))
				{
					cboFMPlanet.Items.Add(new NVItem(_res[i].Location + " (달)", _res[i].ColonyID));

					if ((fleetInfo != null) && (fleetInfo.PlanetCoordinate == _res[i].Location + " (달)"))
						selectedIndex = i + 1;
				}
				else
				{
					cboFMPlanet.Items.Add(new NVItem(_res[i].Location, _res[i].ColonyID));

					if ((fleetInfo != null) && (fleetInfo.PlanetCoordinate == _res[i].Location))
						selectedIndex = i + 1;
				}
			}
			cboFMPlanet.SelectedIndex = selectedIndex;

			// 도착 행성 좌표 목록 채우기
			cboFMTarget.Items.Clear();

			bool isExist = false;
			selectedIndex = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) continue;

				cboFMTarget.Items.Add(_res[i].Location);

				if ((fleetInfo != null) && (fleetInfo.TargetCoords == _res[i].Location))
				{
					selectedIndex = i;
					isExist = true;
				}
			}
			if (isExist) cboFMTarget.SelectedIndex = selectedIndex;
			if (!isExist && (fleetInfo != null) && !string.IsNullOrEmpty(fleetInfo.TargetCoords))
			{
				cboFMTarget.Items.Add(fleetInfo.TargetCoords);
				cboFMTarget.SelectedIndex = cboFMTarget.Items.Count - 1;
			}

			// 도착 행성 형태
			cboFMTargetType.Items.Clear();
			cboFMTargetType.Items.Add(new NVItem("행성", "1"));
			cboFMTargetType.Items.Add(new NVItem("파편지대", "2"));
			cboFMTargetType.Items.Add(new NVItem("달", "3"));
			cboFMTargetType.SelectedIndex = 0;

			if (fleetInfo != null)
			{
				for (int i = 0; i < cboFMTargetType.Items.Count; i++)
				{
					if (((NVItem)cboFMTargetType.Items[i]).Value == fleetInfo.TargetType.ToString())
					{
						cboFMTargetType.SelectedIndex = i;
						break;
					}
				}
			}

			// 비행 형태 채우기
			cboFMMoveType.Items.Clear();
			cboFMMoveType.Items.Add(new NVItem("운송", "3"));
			cboFMMoveType.Items.Add(new NVItem("배치", "4"));
			cboFMMoveType.Items.Add(new NVItem("공격", "1"));
			cboFMMoveType.Items.Add(new NVItem("정탐", "6"));
			cboFMMoveType.Items.Add(new NVItem("식민", "7"));
			cboFMMoveType.Items.Add(new NVItem("수확", "8"));
			cboFMMoveType.Items.Add(new NVItem("원정", "15"));
			cboFMMoveType.SelectedIndex = 0;

			if (fleetInfo != null)
			{
				for (int i = 0; i < cboFMMoveType.Items.Count; i++)
				{
					if (((NVItem)cboFMMoveType.Items[i]).Value == fleetInfo.MoveType.ToString())
					{
						cboFMMoveType.SelectedIndex = i;
						break;
					}
				}
			}

			// 속도 항목 채우기
			cboFMSpeed.Items.Clear();
			cboFMSpeed.Items.Add(new NVItem("100%", "10"));
			cboFMSpeed.Items.Add(new NVItem("90%", "9"));
			cboFMSpeed.Items.Add(new NVItem("80%", "8"));
			cboFMSpeed.Items.Add(new NVItem("70%", "7"));
			cboFMSpeed.Items.Add(new NVItem("60%", "6"));
			cboFMSpeed.Items.Add(new NVItem("50%", "5"));
			cboFMSpeed.Items.Add(new NVItem("40%", "4"));
			cboFMSpeed.Items.Add(new NVItem("30%", "3"));
			cboFMSpeed.Items.Add(new NVItem("20%", "2"));
			cboFMSpeed.Items.Add(new NVItem("10%", "1"));
			cboFMSpeed.SelectedIndex = 0;

			if (fleetInfo != null)
			{
				for (int i = 0; i < cboFMSpeed.Items.Count; i++)
				{
					if (((NVItem)cboFMSpeed.Items[i]).Value == fleetInfo.Speed.ToString())
					{
						cboFMSpeed.SelectedIndex = i;
						break;
					}
				}
			}

			// 함대 구성 채우기
			if ((fleetInfo != null) && (fleetInfo.Fleets.Count > 0))
				initFMFleetDropdown(cboFMFleet1.Parent, fleetInfo.Fleets.Count);
			else
				initFMFleetDropdown(cboFMFleet1.Parent, 0);

			if (fleetInfo != null)
			{
				// 자원 운송 여부 채우기
				chkMoveRes.Checked = fleetInfo.MoveResource;

				// 남길 듀테륨 채우기
				txtFMRDeuterium.Value = fleetInfo.RemainDeuterium;

				// 이벤트 체크 채우기
				chkMoveEvent.Checked = fleetInfo.AddEvent;
			}

			fmFormLoaded = true;
		}

		private void cmdOpenFireFox_Click(object sender, EventArgs e)
		{
			openFileDialog1.Title = "파이어폭스";
			openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			openFileDialog1.Filter = "파이어폭스 실행파일|Firefox*.exe";
			if (DialogResult.OK == openFileDialog1.ShowDialog())
			{
				txtFireFoxDir.Text = openFileDialog1.FileName;
			}
		}

		private void saveOptions()
		{
			SettingsHelper helper = SettingsHelper.Current;
			helper.RefreshRate = int.Parse(txtRate.Text);
			helper.RefreshRateMax = int.Parse(txtRateMax.Text);
			helper.OGameDomain = this.txtOGDomain.Text;
			helper.ApplySummerTime = applySummerTime.Checked;
			helper.UseFireFox = useFireFox.Checked;
			helper.FireFoxDirectory = txtFireFoxDir.Text;
			helper.ShowInLeftBottom = !chkShowInLeftBottom.Checked;

			SmtpMailInfo info = new SmtpMailInfo();
			info.MailAddress = txtMailAddress.Text;
			info.MailServer = txtMailServer.Text;
			info.UserID = txtMailUserID.Text;
			info.Pwd = txtMailPassword.Text;

			helper.SmtpMail = SmtpMailInfo.ToString(info);

			helper.Changed = true;
			// helper.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
		}

		private void useFireFox_CheckedChanged(object sender, EventArgs e)
		{
			txtFireFoxDir.Enabled = useFireFox.Checked;
			cmdOpenFireFox.Enabled = useFireFox.Checked;
		}

		private void saveResCollectingSettings()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(cboResPlanet.Text).Append("|");
			sb.Append(((NVItem) cboResPlanetType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboResMoveType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboResSpeed.SelectedItem).Value).Append("|");
			sb.Append(txtResRDeuterium.Value.ToString()).Append("|");

			string fleet1 = ((NVItem) cboResFleet1.SelectedItem).Value;
			if (!string.IsNullOrEmpty(fleet1)) sb.Append(fleet1);

			string fleet2 = ((NVItem) cboResFleet2.SelectedItem).Value;
			if (!string.IsNullOrEmpty(fleet2))
			{
				if (!string.IsNullOrEmpty(fleet1)) sb.Append("^");
				sb.Append(fleet2);
			}

			sb.Append("|").Append(chkResEvent.Checked ? "1" : "0");

			SettingsHelper settings = SettingsHelper.Current;
			settings.ResourceCollecting = sb.ToString();
			settings.Changed = true;
			// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
		}

		private void loadResCollectingSettings()
		{
			string resCollect = SettingsHelper.Current.ResourceCollecting;
			if (string.IsNullOrEmpty(resCollect)) return;

			string[] s1 = resCollect.Split(new char[] {'|'});
			if (s1.Length < 6) return;

			if (resInfo == null) resInfo = new ResCollectingInfo();

			resInfo.PlanetCoordinate = s1[0];
			resInfo.PlanetType = s1[1].Trim().Length > 0 ? int.Parse(s1[1]) : 1; // 기본값: 1-행성
			resInfo.MoveType = s1[2].Trim().Length > 0 ? int.Parse(s1[2]) : 3; // 기본값: 3-운송
			resInfo.Speed = s1[3].Trim().Length > 0 ? int.Parse(s1[3]) : 10; // 기본값: 100%-10
			resInfo.RemainDeuterium = s1[4].Trim().Length > 0 ? int.Parse(s1[4]) : 0; // 기본값: 0
			resInfo.Fleets = s1[5].Split(new char[] {'^'});

			// 자원 운송 완료 이벤트 추가 여부
			if (s1.Length == 7) resInfo.AddEvent = (s1[6] == "1");
		}

		private bool getResourcesOnPlanet(string sHtml)
		{
			string contextHtml = sHtml.ToLower();

			// 함대1 페이지가 맞는지 확인
			int pos = sHtml.IndexOf("Fleet dispatch I -");
			if (pos >= 0)
			{
				pos = sHtml.IndexOf("metal_box");

				for (int i = 0; i < 3; i++)
				{
					pos = contextHtml.IndexOf("</b> <br><span class=", pos + 1) + 21;
					pos = contextHtml.IndexOf(">", pos + 1);
					int pos2 = contextHtml.IndexOf("</span", pos + 1);
					string sTemp = contextHtml.Substring(pos + 1, pos2 - pos - 1).Trim();
					string[] ss = sTemp.Replace(".", "").Split('/');
					sTemp = ss[0];

					switch (i)
					{
						case 0:
							r_metal = sTemp;
							break;
						case 1:
							r_crystal = sTemp;
							break;
						case 2:
							r_deuterium = sTemp;
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

		private int getTotalResources(int remainDeuterium)
		{
			int total = 0;
			int r;
			if (!int.TryParse(r_metal, out r)) r = 0;
			total += r;
			if (!int.TryParse(r_crystal, out r)) r = 0;
			total += r;
			if (!int.TryParse(r_deuterium, out r)) r = 0;
			if (r < remainDeuterium) r = remainDeuterium;
			total += r - remainDeuterium;
			return total;
		}

		// 행성의 모든 자원을 실어 나르기
		private bool sendFleets(string sessID, NameValueCollection allFleets, string fleets, int[] useFleet,
								int sIdx, int totalResources, int remainResources, out string errMsg, out int selectedDuration)
		{
			errMsg = "";
			selectedDuration = 0;

			string referer = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID;
			string postURL1 = "http://" + naviURL.Host + "/game/index.php?page=fleet2&session=" + sessID;
			string postURL_check = "http://" + naviURL.Host + "/game/index.php?page=fleetcheck&session=" + sessID +
			                       "&ajax=1&espionage=";
			string postURL2 = "http://" + naviURL.Host + "/game/index.php?page=fleet3&session=" + sessID;
			string postURL3 = "http://" + naviURL.Host + "/game/index.php?page=movement&session=" + sessID;

			string[] fromCoords = fromLocation.Split(new char[] {':'});
			string[] toCoords = resInfo.PlanetCoordinate.Split(new char[] {':'});

			string postData1 = string.Format("galaxy={0}&system={1}&position={2}&type={3}&mission=0&speed=10&{4}",
			                                 fromCoords[0],
			                                 fromCoords[1],
			                                 fromCoords[2],
			                                 m_planetType,
			                                 OB2Util.GetFleetsAllOnPlanetText(allFleets, resInfo.Fleets, useFleet));

			string postData_check = string.Format("galaxy={0}&system={1}&planet={2}&type={3}",
			                                      toCoords[0],
			                                      toCoords[1],
			                                      toCoords[2],
			                                      resInfo.PlanetType);

			string postData2 = string.Format("type={0}&mission=0&union=0&{1}&galaxy={2}&system={3}&position={4}" +
			                                 "&speed={5}",
			                                 resInfo.PlanetType,
			                                 fleets,
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 resInfo.Speed);

			Uri newUri = new Uri(postURL1);

			string preCookie = string.Copy(contextCookies);
			string ret = WebCall.Post(newUri, referer, ref contextCookies, postData1);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}


			// 함대별 속도와 듀테륨 소모량을 얻는다.
			string contextHtml = ret.ToLower();
			int pStart = contextHtml.IndexOf("shipids[");
			if (pStart < 0)
			{
				string s = contextHtml;
				if (s.Length > 1000) s = s.Substring(0, 1000);
				errMsg = s.Replace("\r\n", "\n");
				return false;
			}
			while (pStart > 0)
			{
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				int pEnd = contextHtml.IndexOf(";", pStart);
				string fID = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("completeconsumptions[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fConsumption = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("speeds[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fSpeed = contextHtml.Substring(pStart, pEnd - pStart);
				if (fSpeed.IndexOf("e+") > 0)
				{
					fSpeed = decimal.Parse(fSpeed, System.Globalization.NumberStyles.Any).ToString();
				}

				allFleets["consumption" + fID] = fConsumption;
				allFleets["speed" + fID] = fSpeed;

				pStart = contextHtml.IndexOf("shipids[", pStart + 1);
			}

			// 거리, 속도, 시간을 계산한다. --> 듀테륨 소모량 계산
			int[] maxSpeed = OB2Util.GetMaxSpeed2(allFleets, resInfo.Fleets);
			int distance = OB2Util.GetDistance(fromLocation, resInfo.PlanetCoordinate);
			int[] duration = OB2Util.GetDuration(maxSpeed, resInfo.Speed, distance);
			Logger.Log("[자원 운송] 최대속도" + (sIdx + 1) + ": " + maxSpeed[sIdx] + ", 거리:" + distance +
			           ", 시간: " + duration[sIdx] + " (" + OB2Util.GetTime(duration[sIdx]) + ")");

			selectedDuration = duration[sIdx];

			// 선택된 함대의 듀테륨 소모량 계산
			double consumption = 0;
			for (int i = 0; i < resInfo.Fleets.Length; i++)
			{
				consumption += OB2Util.GetConsumption(allFleets, resInfo.Fleets[i], useFleet[i], distance, selectedDuration);
				if (selectedDuration == duration[i]) break;
			}
			consumption = Math.Round(consumption) + 1;

			if (remainResources < 0) remainResources = 0;
			Logger.Log("[자원 운송] 듀테륨 소모량: " + consumption + ", 남는 자원 총량: " + (remainResources + resInfo.RemainDeuterium));
			Logger.Log("[자원 운송] 실제 운송 자원 총량: " + (totalResources - remainResources - resInfo.RemainDeuterium - (int) consumption));
			Logger.Log("---------------------------------------------------------------");

			int met;
			int cryst;
			int deut;
			if (!int.TryParse(r_metal, out met)) met = 0;
			if (!int.TryParse(r_crystal, out cryst)) cryst = 0;
			if (!int.TryParse(r_deuterium, out deut)) deut = 0;

			deut -= (resInfo.RemainDeuterium + (int) consumption);
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

			string postData3 = string.Format("holdingtime=1&expeditiontime=1&galaxy={0}&system={1}&position={2}" +
			                                 "&type={3}&mission={4}&union2=0&holdingOrExpTime=0&speed={5}&{6}" +
			                                 "&metal={7}&crystal={8}&deuterium={9}",
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 resInfo.PlanetType,
			                                 resInfo.MoveType,
			                                 resInfo.Speed,
			                                 fleets,
			                                 met,
			                                 cryst,
			                                 deut);



			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL_check);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.PostAjax(newUri, postURL1, ref contextCookies, postData_check);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			newUri = new Uri(postURL2);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL1, ref contextCookies, postData2);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL3);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL2, ref contextCookies, postData3);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
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

		private static string getOrderString(int moveType)
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

		public bool MoveResource(ref bool retry)
		{
			if ((naviURL == null) || (_res == null) || string.IsNullOrEmpty(fromLocation))
			{
				Logger.Log("WARNING: 자원 운송 장애1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 운송을 할 수 없는 상태입니다.",
								  "자원 운송 장애1 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			loadResCollectingSettings();
			Logger.Log("[자원 운송] 출발지: " + fromLocation + (m_planetType == 3 ? " (달)" : "") +
			           ", 목적지: " + resInfo.PlanetCoordinate + (resInfo.PlanetType == 3 ? " (달)" : "") +
			           ", 미션: " + getOrderString(resInfo.MoveType));

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if ((_res[i] == null) || (resInfo == null)) break;

				if (fromLocation == _res[i].Location)
				{
					if ((m_planetType == 3) && (!_res[i].ColonyName.EndsWith("(달)")) && (_res[i].ColonyName.Trim() != "달")) continue;

					planetID = _res[i].ColonyID;
					break;
				}
			}

			if (planetID == "")
			{
				Logger.Log("WARNING: 자원 운송 장애2-1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 운송을 할 수 없는 상태입니다.",
								  "자원 운송 장애2-1 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			if ((fromLocation == resInfo.PlanetCoordinate) && (m_planetType == resInfo.PlanetType))
			{
				// 은하 단위로 자원 운송을 시킬때는 출발지=목적지의 경우 에러 메시지 생략...
				if (galaxyMethod)
				{
					Logger.Log("[자원 운송] 출발지=목적지, 건너뜀");
					Logger.Log("---------------------------------------------------------------");
					return true;
				}

				Logger.Log("WARNING: 자원 운송 장애2-2");
				MessageBoxEx.Show("출발지와 목적지의 좌표가 같습니다.",
								  "자원 운송 장애2-2 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			string sessID = OB2Util.GetSessID(naviURL);
			string url = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID + "&cp=" + planetID;
			Uri newUri = new Uri(url);

			string preCookie = string.Copy(contextCookies);
			string sHtml = WebCall.GetHtml(newUri, ref contextCookies);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);

			if (fromLocation != OB2Util.GetCoordinate(sHtml))
			{
				Logger.Log("WARNING: 자원 운송 장애3");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 운송을 할 수 없는 상태입니다.",
								  "자원 운송 장애3 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			if (!getResourcesOnPlanet(sHtml))
			{
				Logger.Log("WARNING: 자원 운송 장애4");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 운송을 할 수 없는 상태입니다.",
								  "자원 운송 장애4 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			NameValueCollection allFleets = OB2Util.GetAllFleetsOnPlanet(sHtml);

			int sIdx = 0;
			int totalRes = getTotalResources(0);
			int resQuantity = getTotalResources(resInfo.RemainDeuterium);
			Logger.Log("[자원 운송] 운송 자원량: " + resQuantity + " / 행성 자원 총량: " + totalRes);

			int[] useFleet = new int[resInfo.Fleets.Length];
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < resInfo.Fleets.Length; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string fleetID = resInfo.Fleets[i];
				int fleetNum = OB2Util.GetFleetNum(fleetID, resQuantity);

				string fleets = OB2Util.GetFleetsText(allFleets, fleetID, fleetNum, out useFleet[i]);
				sb.Append(fleets);

				string fleetName = fleetID;
				for (int kk = 0; kk < OB2Util.FLEET_IDS.Length; kk++)
				{
					if (OB2Util.FLEET_IDS[kk].Equals(fleetID))
					{
						fleetName = OB2Util.FLEET_NAMES[kk];
						break;
					}
				}
				Logger.Log("[자원 운송] 함대: " + fleetName + "(" + fleetID + ") - " + useFleet[i] + " 대");

				// 남은 자원을 다시 계산
				resQuantity -= OB2Util.GetCapacity(fleetID, useFleet[i]);
				sIdx = i;

				// 1순위 함대 수를 다 채웠으면 2순위 함대를 사용하지 않는다
				if (useFleet[i] == fleetNum) break;
			}

			if (sb.Length == 0)
			{
				Logger.Log("WARNING: 자원 운송 - 출발할 함대가 없습니다.");
				MessageBoxEx.Show("출발할 함대가 없습니다.",
								  "자원 운송 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			string errMsg;
			int selectedDuration;
			if (!sendFleets(sessID, allFleets, sb.ToString(), useFleet, sIdx, totalRes, resQuantity, out errMsg, out selectedDuration))
			{
				Logger.Log("WARNING: 자원 운송 장애5");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 자원 운송을 할 수 없는 상태입니다.\r\n\r\n상세정보:[" + errMsg + "]",
								  "자원 운송 장애5 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				retry = true;
				return false;
			}

			// 자원 운송 완료 이벤트 추가 여부
			if (resInfo.AddEvent)
			{
				SortedList<DateTime, string> list = OB2Util.LoadAlarmSettings();
				DateTime picker = DateTime.Now.AddSeconds(selectedDuration);
				DateTime key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute);

				string evt = "자원 운송 완료";
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

		private void saveExpeditionSettings()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(((NVItem) cboExpPlanet.SelectedItem).Text).Append("|");
			sb.Append(txtExpTime.Value.ToString()).Append("|");

			int num = (int) txtExpFleetNum1.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet1.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtExpFleetNum2.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet2.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtExpFleetNum3.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet3.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtExpFleetNum4.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet4.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtExpFleetNum5.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet5.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtExpFleetNum6.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet6.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtExpFleetNum7.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboExpFleet7.SelectedItem).Value;
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

		// 원정 보내기
		private bool sendExpedition(string sessID, string from, NameValueCollection allFleets, string fleets, int[] useFleet,
								out string errMsg, out int duration)
		{
			errMsg = "";
			duration = 0;

			string referer = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID;
			string postURL1 = "http://" + naviURL.Host + "/game/index.php?page=fleet2&session=" + sessID;
			string postURL_check = "http://" + naviURL.Host + "/game/index.php?page=fleetcheck&session=" + sessID +
			                       "&ajax=1&espionage=";
			string postURL2 = "http://" + naviURL.Host + "/game/index.php?page=fleet3&session=" + sessID;
			string postURL3 = "http://" + naviURL.Host + "/game/index.php?page=movement&session=" + sessID;

			string[] coords = from.Split(new char[] {':'});

			string postData1 = string.Format("galaxy={0}&system={1}&position={2}&type={3}&mission=0&speed=10&{4}",
			                                 coords[0],
			                                 coords[1],
			                                 coords[2],
			                                 m_planetType,
			                                 OB2Util.GetFleetsAllOnPlanetText(allFleets, expInfo.Fleets, useFleet));

			string postData_check = string.Format("galaxy={0}&system={1}&planet=16&type=1",
			                                      coords[0],
			                                      coords[1]);

			string postData2 = string.Format("type=1&mission=0&union=0&{0}&galaxy={1}&system={2}&position=16" +
			                                 "&speed=10",
			                                 fleets,
			                                 coords[0],
			                                 coords[1]);

			string postData3 = string.Format("holdingtime=1&expeditiontime={0}&galaxy={1}&system={2}&position=16" +
			                                 "&type=1&mission=15&union2=0&holdingOrExpTime={0}&speed=10&{3}" +
			                                 "&metal=&crystal=&deuterium=",
			                                 expInfo.ExpeditionTime,
			                                 coords[0],
			                                 coords[1],
			                                 fleets);

			Uri newUri = new Uri(postURL1);
			string preCookie = string.Copy(contextCookies);
			string ret = WebCall.Post(newUri, referer, ref contextCookies, postData1);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}


			// 함대별 속도와 듀테륨 소모량을 얻는다.
			string contextHtml = ret.ToLower();
			int pStart = contextHtml.IndexOf("shipids[");
			if (pStart < 0)
			{
				string s = contextHtml;
				if (s.Length > 1000) s = s.Substring(0, 1000);
				errMsg = s.Replace("\r\n", "\n");
				return false;
			}
			while (pStart > 0)
			{
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				int pEnd = contextHtml.IndexOf(";", pStart);
				string fID = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("completeconsumptions[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fConsumption = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("speeds[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fSpeed = contextHtml.Substring(pStart, pEnd - pStart);
				if (fSpeed.IndexOf("e+") > 0)
				{
					fSpeed = decimal.Parse(fSpeed, System.Globalization.NumberStyles.Any).ToString();
				}

				allFleets["consumption" + fID] = fConsumption;
				allFleets["speed" + fID] = fSpeed;

				pStart = contextHtml.IndexOf("shipids[", pStart + 1);
			}

			// 거리, 속도, 시간을 계산한다. --> 듀테륨 소모량 계산
			int pos1 = fromLocation.LastIndexOf(":");
			string toLocation = fromLocation.Substring(0, pos1 + 1) + "16";
			int distance = OB2Util.GetDistance(fromLocation, toLocation);
			int maxSpeed = OB2Util.GetMaxSpeed(allFleets, expInfo.Fleets);
			duration = OB2Util.GetDuration(maxSpeed, 10, distance);

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



			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL_check);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.PostAjax(newUri, postURL1, ref contextCookies, postData_check);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			newUri = new Uri(postURL2);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL1, ref contextCookies, postData2);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
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
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL2, ref contextCookies, postData3);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
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

		public bool GoExpedition(ref bool retry)
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
					m_planetType = 3;
				}
			}
			Logger.Log("[원정] 출발지: " + fromLocation + (m_planetType == 3 ? " (달)" : ""));

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) break;

				if (fromLocation == _res[i].Location)
				{
					if ((m_planetType == 3) && (!_res[i].ColonyName.EndsWith("(달)")) && (_res[i].ColonyName.Trim() != "달")) continue;

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
			string url = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID + "&cp=" + planetID;
			Uri newUri = new Uri(url);

			string preCookie = string.Copy(contextCookies);
			string sHtml = WebCall.GetHtml(newUri, ref contextCookies);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);

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
			int duration;
			if (!sendExpedition(sessID, fromLocation, allFleets, sb.ToString(), useFleet, out errMsg, out duration))
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

		private void cmdExpGo_Click(object sender, EventArgs e)
		{
			saveExpeditionSettings();
			bool retry = false;
			if (GoExpedition(ref retry))
			{
				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("원정함대가 출발했습니다.",
								  "원정 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Information,
				                  3*1000);
			}
		}

		private void initFleetDropdown(Control parent, int count)
		{
			for (int i = 1; i <= 7; i++)
			{
				ComboBox cboFleet = null;
				NumericUpDown txtFleetNum = null;

				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("cboExpFleet" + i))
					{
						cboFleet = (ComboBox) c;
						break;
					}
				}

				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("txtExpFleetNum" + i))
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
								if ((expInfo != null) && (expInfo.Fleets.GetKey(j) == ((NVItem) cboFleet.Items[k]).Value))
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
					if (c.Name.Equals("cboExpFleet" + i))
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
					if (c.Name.Equals("cboExpFleet" + last))
					{
						cboFleet = (ComboBox) c;
						if (cboFleet.SelectedIndex > 0) selectedItem = ((NVItem) cboFleet.Items[cboFleet.SelectedIndex]).Value;
						break;
					}
				}

				int num = 0;
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("txtExpFleetNum" + last))
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

		private void cboExpFleets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!expFormLoaded) return;

			ComboBox cbo = (ComboBox) sender;
			string name = cbo.Name;
			int last = int.Parse(name.Substring(name.Length - 1));

			for (int i = 7; i >= last; i--)
			{
				foreach (Control c in cbo.Parent.Controls)
				{
					if (c.Name.Equals("txtExpFleetNum" + i))
					{
						NumericUpDown txtFleetNum = (NumericUpDown) c;
						fillNextFleetDropdown(txtFleetNum.Parent, txtFleetNum.Name, (int) txtFleetNum.Value);
						break;
					}
				}
			}
		}

		private void txtExpFleetNums_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown num = (NumericUpDown) sender;
			if (num.Text == "") num.Value = 0;
			fillNextFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void txtExpFleetNums_KeyUp(object sender, KeyEventArgs e)
		{
			NumericUpDown num = (NumericUpDown) sender;
			if (num.Text == "") num.Value = 0;
			fillNextFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void saveFleetSavingSettings()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(cboFSPlanet.Text).Append("|");
			sb.Append(((NVItem) cboFSPlanetType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboFSMoveType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboFSSpeed.SelectedItem).Value);

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

		// 플릿 세이빙
		private bool startFleetSaving(string sessID, NameValueCollection allFleets, string fleets, int[] useFleet,
									  int remainResources, out string errMsg)
		{
			errMsg = "";
			string referer = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID;
			string postURL1 = "http://" + naviURL.Host + "/game/index.php?page=fleet2&session=" + sessID;
			string postURL_check = "http://" + naviURL.Host + "/game/index.php?page=fleetcheck&session=" + sessID +
			                       "&ajax=1&espionage=";
			string postURL2 = "http://" + naviURL.Host + "/game/index.php?page=fleet3&session=" + sessID;
			string postURL3 = "http://" + naviURL.Host + "/game/index.php?page=movement&session=" + sessID;

			string[] fromCoords = fromLocation.Split(new char[] {':'});
			string[] toCoords = flSavInfo.PlanetCoordinate.Split(new char[] {':'});

			string postData1 = string.Format("galaxy={0}&system={1}&position={2}&type={3}&mission=0&speed=10&{4}",
			                                 fromCoords[0],
			                                 fromCoords[1],
			                                 fromCoords[2],
			                                 m_planetType,
			                                 OB2Util.GetFleetsAllOnPlanetText(allFleets, flSavInfo.Fleets, useFleet));

			string postData_check = string.Format("galaxy={0}&system={1}&planet={2}&type={3}",
			                                      toCoords[0],
			                                      toCoords[1],
			                                      toCoords[2],
			                                      flSavInfo.PlanetType);

			string postData2 = string.Format("type={0}&mission=0&union=0&{1}&galaxy={2}&system={3}&position={4}" +
			                                 "&speed={5}",
			                                 flSavInfo.PlanetType,
			                                 fleets,
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 flSavInfo.Speed);


			Uri newUri = new Uri(postURL1);
			string preCookie = string.Copy(contextCookies);
			string ret = WebCall.Post(newUri, referer, ref contextCookies, postData1);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}


			// 함대별 속도와 듀테륨 소모량을 얻는다.
			string contextHtml = ret.ToLower();
			int pStart = contextHtml.IndexOf("shipids[");
			if (pStart < 0)
			{
				string s = contextHtml;
				if (s.Length > 1000) s = s.Substring(0, 1000);
				errMsg = s.Replace("\r\n", "\n");
				return false;
			}
			while (pStart > 0)
			{
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				int pEnd = contextHtml.IndexOf(";", pStart);
				string fID = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("completeconsumptions[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fConsumption = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("speeds[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fSpeed = contextHtml.Substring(pStart, pEnd - pStart);
				if (fSpeed.IndexOf("e+") > 0)
				{
					fSpeed = decimal.Parse(fSpeed, System.Globalization.NumberStyles.Any).ToString();
				}

				allFleets["consumption" + fID] = fConsumption;
				allFleets["speed" + fID] = fSpeed;

				pStart = contextHtml.IndexOf("shipids[", pStart + 1);
			}

			// 거리, 속도, 시간을 계산한다. --> 듀테륨 소모량 계산
			int maxSpeed = OB2Util.GetMaxSpeed(allFleets, flSavInfo.Fleets);
			int distance = OB2Util.GetDistance(fromLocation, flSavInfo.PlanetCoordinate);
			int duration = OB2Util.GetDuration(maxSpeed, flSavInfo.Speed, distance);
			Logger.Log("[플릿 세이빙] 최대속도: " + maxSpeed + ", 거리:" + distance + ", 시간: " + duration);

			double consumption = 0;
			for (int i = 0; i < flSavInfo.Fleets.Length; i++)
			{
				// 선택된 함대의 듀테륨 소모량 계산
				consumption += OB2Util.GetConsumption(allFleets, flSavInfo.Fleets[i], useFleet[i], distance, duration);
			}
			consumption = Math.Round(consumption) + 1;
			if (remainResources < 0) remainResources = 0;
			Logger.Log("[플릿 세이빙] 듀테륨 소모량: " + consumption + ", 남는 자원 총량: " + remainResources);

			int met;
			int cryst;
			int deut;
			if (!int.TryParse(r_metal, out met)) met = 0;
			if (!int.TryParse(r_crystal, out cryst)) cryst = 0;
			if (!int.TryParse(r_deuterium, out deut)) deut = 0;

			deut -= (int) consumption;
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

			// 원정 옵션 추가
			int expTime = 1;
			int hexpTime = 0;
			if (flSavInfo.MoveType == 15)
			{
				// 환경설정에서 원정값 읽기
				loadExpeditionSettings();
				if (expInfo.ExpeditionTime > 0)
				{
					expTime = expInfo.ExpeditionTime;
					hexpTime = expInfo.ExpeditionTime;
				}
				else
				{
					expTime = 2;
					hexpTime = 2;
				}
			}

			string postData3 = string.Format("holdingtime=1&expeditiontime={6}&galaxy={0}&system={1}&position={2}" +
			                                 "&type={3}&mission={4}&union2=0&holdingOrExpTime={5}&speed={7}&{8}" +
			                                 "&metal={9}&crystal={10}&deuterium={11}",
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 flSavInfo.PlanetType,
			                                 flSavInfo.MoveType,
			                                 hexpTime,
			                                 expTime,
			                                 flSavInfo.Speed,
			                                 fleets,
			                                 met,
			                                 cryst,
			                                 deut);



			// 최대 0.5초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 10);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL_check);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.PostAjax(newUri, postURL1, ref contextCookies, postData_check);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			newUri = new Uri(postURL2);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL1, ref contextCookies, postData2);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
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
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL2, ref contextCookies, postData3);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
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
					if ((m_planetType == 3) && (!_res[i].ColonyName.EndsWith("(달)")) && (_res[i].ColonyName.Trim() != "달")) continue;

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
			string url = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID + "&cp=" + planetID;
			Uri newUri = new Uri(url);

			string preCookie = string.Copy(contextCookies);
			string sHtml = WebCall.GetHtml(newUri, ref contextCookies);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);

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
			flSavInfo.Fleets = OB2Util.GetAllFleets(allFleets);
			//string[] fls = OB2Util.GetAllFleets(allFleets);

			int resQuantity = getTotalResources(0);
			Logger.Log("[플릿 세이빙] 행성 자원 총량: " + resQuantity);

			int[] useFleet = new int[flSavInfo.Fleets.Length];
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < flSavInfo.Fleets.Length; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				int fleetNum;
				string maxShip = allFleets["maxship" + flSavInfo.Fleets[i]];
				if (!int.TryParse(maxShip, out fleetNum)) fleetNum = 0;

				string fleets = OB2Util.GetFleetsText(allFleets, flSavInfo.Fleets[i], fleetNum, out useFleet[i]);
				sb.Append(fleets);

				string fleetName = flSavInfo.Fleets[i];
				for (int kk = 0; kk < OB2Util.FLEET_IDS.Length; kk++)
				{
					if (OB2Util.FLEET_IDS[kk].Equals(flSavInfo.Fleets[i]))
					{
						fleetName = OB2Util.FLEET_NAMES[kk];
						break;
					}
				}
				Logger.Log("[플릿 세이빙] 함대: " + fleetName + "(" + flSavInfo.Fleets[i] + ") - " + useFleet[i] + " 대");

				// 남은 자원을 다시 계산
				resQuantity -= OB2Util.GetCapacity(flSavInfo.Fleets[i], useFleet[i]);
			}

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
			if (!startFleetSaving(sessID, allFleets, sb.ToString(), useFleet, resQuantity, out errMsg))
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

		private void saveFleetMoveSettings()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(((NVItem) cboFMPlanet.SelectedItem).Text).Append("|");
			sb.Append(cboFMTarget.Text).Append("|");
			sb.Append(((NVItem) cboFMTargetType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboFMMoveType.SelectedItem).Value).Append("|");
			sb.Append(((NVItem) cboFMSpeed.SelectedItem).Value).Append("|");
			sb.Append(chkMoveRes.Checked ? "1" : "0").Append("|");
			sb.Append(txtFMRDeuterium.Value.ToString()).Append("|");
			sb.Append(chkMoveEvent.Checked ? "1" : "0").Append("|");

			int num = (int) txtFMFleetNum1.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet1.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFMFleetNum2.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet2.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFMFleetNum3.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet3.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFMFleetNum4.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet4.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFMFleetNum5.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet5.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFMFleetNum6.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet6.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}
			num = (int) txtFMFleetNum7.Value;
			if (num > 0)
			{
				string fleet = ((NVItem) cboFMFleet7.SelectedItem).Value;
				if (fleet.Length > 0)
					sb.Append("^").Append(fleet).Append("&").Append(num.ToString());
			}

			SettingsHelper settings = SettingsHelper.Current;
			settings.FleetMoving = sb.ToString();
			settings.Changed = true;
			// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로

			if (((NVItem) cboFMMoveType.SelectedItem).Value == "15") // 원정
			{
				MessageBoxEx.Show("함대 기동을 '원정'으로 설정하면 탐사시간은 원정 옵션의 시간이 적용됩니다.",
								  "함대 기동 설정 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Information,
								  10 * 1000);
			}
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

		// 함대 기동
		private bool sendFMFleets(string sessID, NameValueCollection allFleets, string fleets, int[] useFleet,
								  int totalResources, int remainResources, FleetMoveInfo fleetInfo,
								  out string errMsg, out int duration)
		{
			errMsg = "";
			duration = 0;

			string referer = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID;
			string postURL1 = "http://" + naviURL.Host + "/game/index.php?page=fleet2&session=" + sessID;
			string postURL_check = "http://" + naviURL.Host + "/game/index.php?page=fleetcheck&session=" + sessID +
			                       "&ajax=1&espionage=";
			string postURL2 = "http://" + naviURL.Host + "/game/index.php?page=fleet3&session=" + sessID;
			string postURL3 = "http://" + naviURL.Host + "/game/index.php?page=movement&session=" + sessID;

			string[] fromCoords = fromLocation.Split(new char[] {':'});
			string[] toCoords = fleetInfo.TargetCoords.Split(new char[] {':'});

			string postData1 = string.Format("galaxy={0}&system={1}&position={2}&type={3}&mission=0&speed=10&{4}",
			                                 fromCoords[0],
			                                 fromCoords[1],
			                                 fromCoords[2],
			                                 m_planetType,
			                                 OB2Util.GetFleetsAllOnPlanetText(allFleets, fleetInfo.Fleets, useFleet));

			string postData_check = string.Format("galaxy={0}&system={1}&planet={2}&type={3}",
			                                      toCoords[0],
			                                      toCoords[1],
			                                      toCoords[2],
			                                      fleetInfo.TargetType);

			string postData2 = string.Format("type={0}&mission=0&union=0&{1}&galaxy={2}&system={3}&position={4}" +
			                                 "&speed={5}",
			                                 fleetInfo.TargetType,
			                                 fleets,
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 fleetInfo.Speed);

			Uri newUri = new Uri(postURL1);
			string preCookie = string.Copy(contextCookies);
			string ret = WebCall.Post(newUri, referer, ref contextCookies, postData1);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}


			// 함대별 속도와 듀테륨 소모량을 얻는다.
			string contextHtml = ret.ToLower();
			int pStart = contextHtml.IndexOf("shipids[");
			if (pStart < 0)
			{
				string s = contextHtml;
				if (s.Length > 1000) s = s.Substring(0, 1000);
				errMsg = s.Replace("\r\n", "\n");
				return false;
			}
			while (pStart > 0)
			{
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				int pEnd = contextHtml.IndexOf(";", pStart);
				string fID = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("completeconsumptions[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fConsumption = contextHtml.Substring(pStart, pEnd - pStart);

				pStart = contextHtml.IndexOf("speeds[", pEnd);
				pStart = contextHtml.IndexOf("= ", pStart) + 2;
				pEnd = contextHtml.IndexOf(";", pStart);
				string fSpeed = contextHtml.Substring(pStart, pEnd - pStart);
				if (fSpeed.IndexOf("e+") > 0)
				{
					fSpeed = decimal.Parse(fSpeed, System.Globalization.NumberStyles.Any).ToString();
				}

				allFleets["consumption" + fID] = fConsumption;
				allFleets["speed" + fID] = fSpeed;

				pStart = contextHtml.IndexOf("shipids[", pStart + 1);
			}

			// 거리, 속도, 시간을 계산한다. --> 듀테륨 소모량 계산
			int maxSpeed = OB2Util.GetMaxSpeed(allFleets, fleetInfo.Fleets);
			int distance = OB2Util.GetDistance(fromLocation, fleetInfo.TargetCoords);
			duration = OB2Util.GetDuration(maxSpeed, fleetInfo.Speed, distance);
			Logger.Log("[함대 기동] 최대속도: " + maxSpeed + ", 거리:" + distance + ", 시간: " +
			           duration + " (" + OB2Util.GetTime(duration) + ")");

			// 선택된 함대의 듀테륨 소모량 계산
			double consumption = 0;
			for (int i = 0; i < fleetInfo.Fleets.Count; i++)
			{
				string fleetID = fleetInfo.Fleets.GetKey(i);
				consumption += OB2Util.GetConsumption(allFleets, fleetID, useFleet[i], distance, duration);
			}
			consumption = Math.Round(consumption) + 1;

			if (remainResources < 0) remainResources = 0;
			if (fleetInfo.MoveResource)
			{
				Logger.Log("[함대 기동] 듀테륨 소모량: " + consumption + ", 남는 자원 총량: " + (remainResources + fleetInfo.RemainDeuterium));
				Logger.Log("[함대 기동] 실제 운송 자원 총량: " +
				           (totalResources - remainResources - fleetInfo.RemainDeuterium - (int) consumption));
			}
			else
				Logger.Log("[함대 기동] 듀테륨 소모량: " + consumption);
			Logger.Log("------------------------------------------------");

			int met = 0;
			int cryst = 0;
			int deut = 0;

			if (fleetInfo.MoveResource)
			{
				if (!int.TryParse(r_metal, out met)) met = 0;
				if (!int.TryParse(r_crystal, out cryst)) cryst = 0;
				if (!int.TryParse(r_deuterium, out deut)) deut = 0;

				deut -= (fleetInfo.RemainDeuterium + (int) consumption);
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
			}

			// 원정 옵션 추가
			int expTime = 1;
			int hexpTime = 0;
			if (fleetInfo.MoveType == 15)
			{
				// 환경설정에서 원정값 읽기
				loadExpeditionSettings();
				if (expInfo.ExpeditionTime > 0)
				{
					expTime = expInfo.ExpeditionTime;
					hexpTime = expInfo.ExpeditionTime;
				}
				else
				{
					expTime = 2;
					hexpTime = 2;
				}
			}

			string postData3 = string.Format("holdingtime=1&expeditiontime={6}&galaxy={0}&system={1}&position={2}" +
			                                 "&type={3}&mission={4}&union2=0&holdingOrExpTime={5}&speed={7}&{8}" +
			                                 "&metal={9}&crystal={10}&deuterium={11}",
			                                 toCoords[0],
			                                 toCoords[1],
			                                 toCoords[2],
			                                 fleetInfo.TargetType,
			                                 fleetInfo.MoveType,
			                                 hexpTime,
			                                 expTime,
			                                 fleetInfo.Speed,
			                                 fleets,
			                                 met,
			                                 cryst,
			                                 deut);



			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			Random r = new Random();
			int num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL_check);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.PostAjax(newUri, postURL1, ref contextCookies, postData_check);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			newUri = new Uri(postURL2);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL1, ref contextCookies, postData2);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
			if (ret.StartsWith("ERROR"))
			{
				errMsg = ret;
				return false;
			}

			// 최대 3초간 쉰다: 일정한 시간 간격으로 발생하는 이벤트 회피(봇 탐지 회피)
			num = r.Next(0, 60);
			for (int i = 0; i < num; i++)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}

			newUri = new Uri(postURL3);
			preCookie = string.Copy(contextCookies);
			ret = WebCall.Post(newUri, postURL2, ref contextCookies, postData3);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
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

		public bool MoveFleet(ref bool retry)
		{
			if ((naviURL == null) || (_res == null))
			{
				Logger.Log("WARNING: 함대 기동 장애1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애1 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			loadFleetMoveSettings();

			if (string.IsNullOrEmpty(fromLocation) && (fleetInfo != null))
			{
				fromLocation = fleetInfo.PlanetCoordinate;
				if (fromLocation.IndexOf("(달)") > 0)
				{
					fromLocation = fromLocation.Replace(" (달)", "");
					m_planetType = 3;
				}
			}
			Logger.Log("[함대 기동] 출발지: " + fromLocation + (m_planetType == 3 ? " (달)" : "") +
			           ", 목적지: " + fleetInfo.TargetCoords + (fleetInfo.TargetType == 3 ? " (달)" : "") +
			           ", 미션: " + getOrderString(fleetInfo.MoveType));

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) break;

				if (fromLocation == _res[i].Location)
				{
					if ((m_planetType == 3) && (!_res[i].ColonyName.EndsWith("(달)"))) continue;

					planetID = _res[i].ColonyID;
					break;
				}
			}

			if (planetID == "")
			{
				Logger.Log("WARNING: 함대 기동 장애2");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애2 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			string sessID = OB2Util.GetSessID(naviURL);
			string url = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID + "&cp=" + planetID;
			Uri newUri = new Uri(url);

			string preCookie = string.Copy(contextCookies);
			string sHtml = WebCall.GetHtml(newUri, ref contextCookies);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);

			if (fromLocation != OB2Util.GetCoordinate(sHtml))
			{
				Logger.Log("WARNING: 함대 기동 장애3");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애3 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			if (!getResourcesOnPlanet(sHtml))
			{
				Logger.Log("WARNING: 함대 기동 장애4");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애4 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			NameValueCollection allFleets = OB2Util.GetAllFleetsOnPlanet(sHtml);

			int totalRes = getTotalResources(0);
			int resQuantity = fleetInfo.MoveResource ? getTotalResources(fleetInfo.RemainDeuterium) : 0;
			Logger.Log("[함대 기동] 운송 자원량: " + resQuantity + " / 행성 자원 총량: " + totalRes);

			int[] useFleet = new int[fleetInfo.Fleets.Count];
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < fleetInfo.Fleets.Count; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string fleetID = fleetInfo.Fleets.GetKey(i);
				int num;
				if (!int.TryParse(fleetInfo.Fleets[fleetID], out num)) continue;

				string fleets = OB2Util.GetFleetsText(allFleets, fleetID, num, out useFleet[i]);
				sb.Append(fleets);

				string fleetName = fleetID;
				for (int kk = 0; kk < OB2Util.FLEET_IDS.Length; kk++)
				{
					if (OB2Util.FLEET_IDS[kk].Equals(fleetID))
					{
						fleetName = OB2Util.FLEET_NAMES[kk];
						break;
					}
				}
				Logger.Log("[함대 기동] 함대: " + fleetName + "(" + fleetID + ") - " + useFleet[i] + " 대");

				// 남은 자원을 다시 계산
				if (fleetInfo.MoveResource) resQuantity -= OB2Util.GetCapacity(fleetID, useFleet[i]);
			}

			if (sb.Length == 0)
			{
				Logger.Log("WARNING: 함대 기동 - 출발할 함대가 없습니다.");
				MessageBoxEx.Show("출발할 함대가 없습니다.",
								  "함대 기동 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				return false;
			}

			string errMsg;
			int duration;
			if (!sendFMFleets(sessID, allFleets, sb.ToString(), useFleet, totalRes, resQuantity, fleetInfo, out errMsg, out duration))
			{
				Logger.Log("WARNING: 함대 기동 장애5");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.\r\n\r\n상세정보:[" + errMsg + "]",
								  "함대 기동 장애5 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Exclamation,
				                  10*1000);
				retry = true;
				return false;
			}

			// 기동함대 귀환 이벤트 추가 여부
			if (fleetInfo.AddEvent)
			{
				SortedList<DateTime, string> list = OB2Util.LoadAlarmSettings();
				DateTime picker = DateTime.Now.AddSeconds(duration);
				DateTime key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute);

				string evt = "*기동함대 목적지 도착";
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

				if (fleetInfo.MoveType != 4) // 배치
				{
					picker = DateTime.Now.AddSeconds(duration*2);
					key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute);

					evt = "*기동함대 귀환";
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
				}

				OB2Util.SaveAlarmSettings(list);
			}
			return true;
		}

		internal bool MoveFleet2(FleetMoveInfo fleetMoveInfo, ref bool retry)
		{
			if ((naviURL == null) || (_res == null) || (fleetMoveInfo == null))
			{
				Logger.Log("WARNING: 함대 기동 장애1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애1 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			if (string.IsNullOrEmpty(fromLocation))
			{
				fromLocation = fleetMoveInfo.PlanetCoordinate;
				if (fromLocation.IndexOf("(달)") > 0)
				{
					fromLocation = fromLocation.Replace(" (달)", "");
					m_planetType = 3;
				}
			}
			Logger.Log("[함대 기동] 출발지: " + fromLocation + (m_planetType == 3 ? " (달)" : "") +
					   ", 목적지: " + fleetMoveInfo.TargetCoords + (fleetMoveInfo.TargetType == 3 ? " (달)" : "") +
					   ", 미션: " + getOrderString(fleetMoveInfo.MoveType));

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) break;

				if (fromLocation == _res[i].Location)
				{
					if ((m_planetType == 3) && (!_res[i].ColonyName.EndsWith("(달)")) && (_res[i].ColonyName.Trim() != "달")) continue;

					planetID = _res[i].ColonyID;
					break;
				}
			}

			if (planetID == "")
			{
				Logger.Log("WARNING: 함대 기동 장애2");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애2 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			string sessID = OB2Util.GetSessID(naviURL);
			string url = "http://" + naviURL.Host + "/game/index.php?page=fleet1&session=" + sessID + "&cp=" + planetID;
			Uri newUri = new Uri(url);

			string preCookie = string.Copy(contextCookies);
			string sHtml = WebCall.GetHtml(newUri, ref contextCookies);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);

			if (fromLocation != OB2Util.GetCoordinate(sHtml))
			{
				Logger.Log("WARNING: 함대 기동 장애3");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애3 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			if (!getResourcesOnPlanet(sHtml))
			{
				Logger.Log("WARNING: 함대 기동 장애4");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.",
								  "함대 기동 장애4 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			NameValueCollection allFleets = OB2Util.GetAllFleetsOnPlanet(sHtml);

			int totalRes = getTotalResources(0);
			int resQuantity = fleetMoveInfo.MoveResource ? getTotalResources(fleetMoveInfo.RemainDeuterium) : 0;
			Logger.Log("[함대 기동] 운송 자원량: " + resQuantity + " / 행성 자원 총량: " + totalRes);

			int[] useFleet = new int[fleetMoveInfo.Fleets.Count];
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < fleetMoveInfo.Fleets.Count; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string fleetID = fleetMoveInfo.Fleets.GetKey(i);
				int num;
				if (!int.TryParse(fleetMoveInfo.Fleets[fleetID], out num)) continue;

				string fleets = OB2Util.GetFleetsText(allFleets, fleetID, num, out useFleet[i]);
				sb.Append(fleets);

				string fleetName = fleetID;
				for (int kk = 0; kk < OB2Util.FLEET_IDS.Length; kk++)
				{
					if (OB2Util.FLEET_IDS[kk].Equals(fleetID))
					{
						fleetName = OB2Util.FLEET_NAMES[kk];
						break;
					}
				}
				Logger.Log("[함대 기동] 함대: " + fleetName + "(" + fleetID + ") - " + useFleet[i] + " 대");

				// 남은 자원을 다시 계산
				if (fleetMoveInfo.MoveResource) resQuantity -= OB2Util.GetCapacity(fleetID, useFleet[i]);
			}

			if (sb.Length == 0)
			{
				Logger.Log("WARNING: 함대 기동 - 출발할 함대가 없습니다.");
				MessageBoxEx.Show("출발할 함대가 없습니다.",
								  "함대 기동 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			string errMsg;
			int duration;
			if (!sendFMFleets(sessID, allFleets, sb.ToString(), useFleet, totalRes, resQuantity, fleetMoveInfo, out errMsg, out duration))
			{
				Logger.Log("WARNING: 함대 기동 장애5");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 함대가 출발할 수 없는 상태입니다.\r\n\r\n상세정보:[" + errMsg + "]",
								  "함대 기동 장애5 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				retry = true;
				return false;
			}

			// 기동함대 귀환 이벤트 추가 여부
			if (fleetMoveInfo.AddEvent)
			{
				SortedList<DateTime, string> list = OB2Util.LoadAlarmSettings();
				int addHour = ((expInfo != null) && (expInfo.ExpeditionTime > 0)) ? expInfo.ExpeditionTime : 2;
				DateTime picker = (fleetMoveInfo.MoveType == 15) ? DateTime.Now.AddHours(addHour).AddSeconds(duration) : DateTime.Now.AddSeconds(duration);
				DateTime key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute);

				string evt = "*기동함대 목적지 도착";
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

				if (fleetMoveInfo.MoveType != 4) // 배치
				{
					picker = (fleetMoveInfo.MoveType == 15) ? DateTime.Now.AddHours(addHour).AddSeconds(duration * 2) : DateTime.Now.AddSeconds(duration * 2);
					key = picker.Date.AddHours(picker.Hour).AddMinutes(picker.Minute);

					evt = "*기동함대 귀환";
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
				}

				OB2Util.SaveAlarmSettings(list);
			}
			return true;
		}

		private void cmdFMGo_Click(object sender, EventArgs e)
		{
			saveFleetMoveSettings();
			bool retry = false;
			if (MoveFleet(ref retry))
			{
				SoundPlayer.PlaySound(Application.StartupPath + @"\newalert.wav");
				MessageBoxEx.Show("함대가 출발했습니다.",
								  "함대 기동 - oBrowser2: " + _uniName,
				                  MessageBoxButtons.OK,
				                  MessageBoxIcon.Information,
				                  3*1000);
			}
		}

		private void initFMFleetDropdown(Control parent, int count)
		{
			for (int i = 1; i <= 7; i++)
			{
				ComboBox cboFleet = null;
				NumericUpDown txtFleetNum = null;

				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("cboFMFleet" + i))
					{
						cboFleet = (ComboBox) c;
						break;
					}
				}

				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("txtFMFleetNum" + i))
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

		private static void fillNextFMFleetDropdown(Control parent, string name, int count)
		{
			int last = int.Parse(name.Substring(name.Length - 1));
			if (last >= 7) return;

			string[] reged = new string[last];
			for (int i = 1; i <= last; i++)
			{
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("cboFMFleet" + i))
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
					if (c.Name.Equals("cboFMFleet" + last))
					{
						cboFleet = (ComboBox) c;
						if (cboFleet.SelectedIndex > 0) selectedItem = ((NVItem) cboFleet.Items[cboFleet.SelectedIndex]).Value;
						break;
					}
				}

				int num = 0;
				foreach (Control c in parent.Controls)
				{
					if (c.Name.Equals("txtFMFleetNum" + last))
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

		private void cboFMFleets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!fmFormLoaded) return;

			ComboBox cbo = (ComboBox) sender;
			string name = cbo.Name;
			int last = int.Parse(name.Substring(name.Length - 1));

			for (int i = 7; i >= last; i--)
			{
				foreach (Control c in cbo.Parent.Controls)
				{
					if (c.Name.Equals("txtFMFleetNum" + i))
					{
						NumericUpDown txtFleetNum = (NumericUpDown) c;
						fillNextFMFleetDropdown(txtFleetNum.Parent, txtFleetNum.Name, (int) txtFleetNum.Value);
						break;
					}
				}
			}
		}

		private void txtFMFleetNums_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown num = (NumericUpDown) sender;
			if (num.Text == "") num.Value = 0;
			fillNextFMFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void txtFMFleetNums_KeyUp(object sender, KeyEventArgs e)
		{
			NumericUpDown num = (NumericUpDown) sender;
			if (num.Text == "") num.Value = 0;
			fillNextFMFleetDropdown(num.Parent, num.Name, (int)num.Value);
		}

		private void chkMoveRes_CheckedChanged(object sender, EventArgs e)
		{
			txtFMRDeuterium.Enabled = chkMoveRes.Checked;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((tabControl1.SelectedTab == tabPage2) && tabPage2.Enabled)
				setResourceForm();
			else if ((tabControl1.SelectedTab == tabPage3) && tabPage3.Enabled)
				setExpeditionForm();
			else if ((tabControl1.SelectedTab == tabPage4) && tabPage4.Enabled)
				setFleetSavingForm();
			else if ((tabControl1.SelectedTab == tabPage5) && tabPage5.Enabled)
				setFleetMoveForm();
		}

		private void frmOption_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!doClose) e.Cancel = true;
			this.Hide();
		}

		internal bool DoPlanetJob(string action, string actionID, ref bool retry)
		{
			string jobURL1 = "";
			string jobURL2 = "";

			string jobName = OB2Util.GetActionName(action);
			string bName = string.Empty;
			for (int kk = 0; kk < OB2Util.BUILDING1_IDS.Length; kk++)
			{
				if (OB2Util.BUILDING1_IDS[kk].Equals(actionID))
				{
					bName = OB2Util.BUILDING1_NAMES[kk];
					switch (action)
					{
						case "BB":
							jobURL1 = OB2Util.BUILDING1_INIT;
							jobURL2 = OB2Util.BUILDING1_CONSTRUCT;
							break;
						case "BC":
							jobURL1 = OB2Util.BUILDING1_INIT;
							jobURL2 = OB2Util.BUILDING1_CANCEL;
							break;
						case "BD":
							jobURL1 = OB2Util.BUILDING1_INIT;
							jobURL2 = OB2Util.BUILDING1_DESTRUCT;
							break;
					}
					break;
				}
			}
			for (int kk = 0; kk < OB2Util.BUILDING2_IDS.Length; kk++)
			{
				if (OB2Util.BUILDING2_IDS[kk].Equals(actionID))
				{
					bName = OB2Util.BUILDING2_NAMES[kk];
					switch (action)
					{
						case "BB":
							jobURL1 = OB2Util.BUILDING2_INIT;
							jobURL2 = OB2Util.BUILDING2_CONSTRUCT;
							break;
						case "BC":
							jobURL1 = OB2Util.BUILDING2_INIT;
							jobURL2 = OB2Util.BUILDING2_CANCEL;
							break;
						case "BD":
							jobURL1 = OB2Util.BUILDING2_INIT;
							jobURL2 = OB2Util.BUILDING2_DESTRUCT;
							break;
					}
					break;
				}
			}
			for (int kk = 0; kk < OB2Util.RESEARCH_IDS.Length; kk++)
			{
				if (OB2Util.RESEARCH_IDS[kk].Equals(actionID))
				{
					bName = OB2Util.RESEARCH_NAMES[kk];
					switch (action)
					{
						case "RB":
							jobURL1 = OB2Util.RESEARCH_INIT;
							jobURL2 = OB2Util.RESEARCH_START;
							break;
						case "RC":
							jobURL1 = OB2Util.RESEARCH_INIT;
							jobURL2 = OB2Util.RESEARCH_STOP;
							break;
					}
					break;
				}
			}

			Logger.Log("[" + jobName + "] 좌표: " + fromLocation + (m_planetType == 3 ? " (달)" : "") +
					   ", 작업 대상: " + bName);

			if ((naviURL == null) || (_res == null))
			{
				Logger.Log("WARNING: " + jobName + " 장애1");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 " + jobName + " 작업을 할 수 없는 상태입니다.",
								  jobName + " 장애1 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			string planetID = "";
			for (int i = 0; i < _res.Length; i++)
			{
				if (_res[i] == null) break;

				if (fromLocation == _res[i].Location)
				{
					if ((m_planetType == 3) && (!_res[i].ColonyName.EndsWith("(달)")) && (_res[i].ColonyName.Trim() != "달")) continue;

					planetID = _res[i].ColonyID;
					break;
				}
			}

			if (planetID == "")
			{
				Logger.Log("WARNING: " + jobName + " 장애2");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 " + jobName + " 작업을 할 수 없는 상태입니다.",
								  jobName + " 장애2 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			string sessID = OB2Util.GetSessID(naviURL);
			string url = string.Format(jobURL1,
			                           naviURL.Host,
			                           sessID,
			                           planetID);
			Uri newUri = new Uri(url);

			string preCookie = string.Copy(contextCookies);
			string sHtml = WebCall.GetHtml(newUri, ref contextCookies);
			if (contextCookies != preCookie)
				Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);

			if (fromLocation != OB2Util.GetCoordinate(sHtml))
			{
				Logger.Log("WARNING: " + jobName + " 장애3");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 " + jobName + " 작업을 할 수 없는 상태입니다.",
								  jobName + " 장애3 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				return false;
			}

			preCookie = string.Copy(contextCookies);
			switch (action)
			{
				case "BB":
					url = string.Format(jobURL2,
					                    naviURL.Host,
					                    sessID);
					newUri = new Uri(url);
					sHtml = WebCall.PostAjax(newUri, jobURL2, ref contextCookies, "type=" + actionID);
					sHtml = WebCall.Post(newUri, jobURL2, ref contextCookies, "token=" + OB2Util.GetToken(sHtml) + "&modus=1&type=" + actionID);
                    if (contextCookies != preCookie)
                        Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
                    break;
				default:
					url = string.Format(jobURL2,
					                    naviURL.Host,
					                    sessID,
					                    actionID);
					newUri = new Uri(url);

                    sHtml = WebCall.GetHtml(newUri, ref contextCookies);
                    if (contextCookies != preCookie)
                        Logger.Log("쿠키변경: " + preCookie + " --> " + contextCookies);
                    break;
			}

			if (fromLocation != OB2Util.GetCoordinate(sHtml))
			{
				Logger.Log("WARNING: " + jobName + " 장애4");
				MessageBoxEx.Show("오게임에 접속되지 않았거나 " + jobName + " 작업을 할 수 없는 상태입니다.",
								  jobName + " 장애4 - oBrowser2: " + _uniName,
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Exclamation,
								  10 * 1000);
				retry = true;
				return false;
			}

			if ((action == "BB") || (action == "BD") || (action == "RB"))
			{
				if (!OB2Util.GetProcessingStatus(sHtml))
				{
					Logger.Log("WARNING: " + jobName + " 장애5");
					MessageBoxEx.Show("오게임에 접속되지 않았거나 " + jobName + " 작업을 할 수 없는 상태입니다.",
					                  jobName + " 장애5 - oBrowser2: " + _uniName,
					                  MessageBoxButtons.OK,
					                  MessageBoxIcon.Exclamation,
					                  10*1000);
					retry = true;
					return false;
				}
			}
			return true;
		}
	}
}