using System;
using System.Text;
using System.Windows.Forms;

namespace oBrowser2
{
	public partial class frmFleetSaving : Form
	{
		private ResourceInfo[] _res;
		private string _uniName;
		private ResCollectingInfo flSavInfo;
		private Uri naviURL;

		public frmFleetSaving()
		{
			InitializeComponent();

			naviURL = null;
			_res = null;
			flSavInfo = null;
		}

		public Uri NaviURL
		{
			set { naviURL = value; }
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

				// 좌표가 없는 경우에만 채워줌(달 고려)
				if (!cboPlanet.Items.Contains(_res[i].Location))
				{
					cboPlanet.Items.Add(_res[i].Location);
					if ((flSavInfo != null) && (flSavInfo.PlanetCoordinate == _res[i].Location))
					{
						selectedIndex = cboPlanet.Items.Count - 1;
						isExist = true;
					}
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
	}
}