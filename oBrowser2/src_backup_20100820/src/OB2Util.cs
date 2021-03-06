﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using oBrowser2.Properties;

namespace oBrowser2
{
	public class OB2Util
	{
		private const double speedfactor = 1.0;

		public static readonly int[] FLEET_CARGOS = {
		                                            	0, 5000, 25000, 50, 100, 800, 1500, 7500, 20000, 5,
		                                            	500, /*0,*/ 2000, 1000000, 750
		                                            };

		public static readonly string[] FLEET_IDS = {
		                                            	"", "202", "203", "204", "205", "206", "207", "208", "209", "210",
		                                            	"211", /*"212",*/ "213", "214", "215"
		                                            };

		public static readonly string[] FLEET_NAMES = {
		                                              	"", "소형 화물선", "대형 화물선", "전투기", "공격기", "구축함", "순양함", "이민선", "수확선",
		                                              	"무인 정찰기",
		                                              	"폭격기", /*"태양광 인공위성",*/ "전함", "죽음의 별", "순양전함"
		                                              };

		public static readonly string[] BUILDING1_IDS = {
		                                            	"1", "2", "3", "4", "12",
		                                            	"22", "23", "24"
		                                            };

		public static readonly string[] BUILDING1_NAMES = {
		                                                 	"메탈 광업소", "크리스탈 광업소", "듀테륨 정제소", "태양광 발전소", "핵 융합로",
		                                                 	"메탈 탱크", "크리스탈 탱크", "듀테륨 탱크"
		                                                 };

		public static readonly string BUILDING1_INIT = "http://{0}/game/index.php?page=resources&session={1}&cp={2}";
		public static readonly string BUILDING1_CONSTRUCT = "http://{0}/game/index.php?page=resources&session={1}";
		public static readonly string BUILDING1_DESTRUCT = "http://{0}/game/index.php?page=resources&session={1}&modus=3&techid={2}";
		public static readonly string BUILDING1_CANCEL = "http://{0}/game/index.php?page=resources&session={1}&modus=2&techid={2}&listid=1";

		public static readonly string[] BUILDING2_IDS = {
		                                            	"14", "15", "21", "31", "33",
														"34", "44", "41", "42", "43"
		                                            };

		public static readonly string[] BUILDING2_NAMES = {
		                                                 	"로봇 공장", "나노머신 공장", "군수 공장", "연구소", "테라포머",
															"우군보급기지", "미사일 사일로", "달 기지", "밀집 센서", "점프 게이트"
		                                                 };

		public static readonly string BUILDING2_INIT = "http://{0}/game/index.php?page=station&session={1}&cp={2}";
		public static readonly string BUILDING2_CONSTRUCT = "http://{0}/game/index.php?page=station&session={1}";
		public static readonly string BUILDING2_DESTRUCT = "http://{0}/game/index.php?page=station&session={1}&modus=3&techid={2}";
		public static readonly string BUILDING2_CANCEL = "http://{0}/game/index.php?page=station&session={1}&modus=2&techid={2}&listid=1";

		public static readonly string[] RESEARCH_IDS = {
		                                               	"106", "108", "109", "110", "111",
		                                               	"113", "114", "115", "117", "118",
		                                               	"120", "121", "122", "123", "124",
		                                               	"199"
		                                               };

		public static readonly string[] RESEARCH_NAMES = {
		                                                 	"정탐 기술", "컴퓨터 공학", "무기 공학", "보호막 기술", "장갑 기술",
		                                                 	"에너지 공학", "초공간 기술", "연소 엔진", "핵추진 엔진", "초공간 엔진",
		                                                 	"레이저 공학", "이온 공학", "플라스마 공학", "은하간 연구망", "원정 기술",
		                                                 	"중력자 기술"
		                                                 };

		public static readonly string RESEARCH_INIT = "http://{0}/game/index.php?page=research&session={1}&cp={2}";
		public static readonly string RESEARCH_START = "http://{0}/game/index.php?page=research&session={1}&modus=1&type={2}";
		public static readonly string RESEARCH_STOP = "http://{0}/game/index.php?page=research&session={1}&modus=2&type={2}";

		public static string GetSessID(Uri orgUri)
		{
			string url = orgUri.ToString();
			int pos = url.IndexOf("&session=");
			if (pos < 0) return "";

			int pos2 = url.IndexOf("&", pos + 1);
			return pos2 > 0 ? url.Substring(pos + 9, pos2 - pos - 9) : url.Substring(pos + 9);
		}

		public static string GetCoordinate(string sHtml)
		{
			string contextHtml = sHtml.ToLower();
			int pos = contextHtml.IndexOf("planetlink active");
			if (pos < 0) return "";

			string sPlanet = contextHtml.Substring(0, pos);
			int pStart = sPlanet.LastIndexOf("<div");
			int pEnd = contextHtml.IndexOf("</div", pStart + 1);
			sPlanet = contextHtml.Substring(pStart, pEnd - pStart);

			pos = sPlanet.IndexOf("[");
			int pos2 = sPlanet.IndexOf("]", pos + 1);
			return sPlanet.Substring(pos + 1, pos2 - pos - 1);
		}

		public static string GetToken(string sHtml)
		{
			int pos = sHtml.IndexOf("token' value=");
			if (pos < 0) return "";

			pos += 14;
			int pos2 = sHtml.IndexOf("'", pos + 1);
			return sHtml.Substring(pos, pos2 - pos);
		}

		public static bool GetProcessingStatus(string sHtml)
		{
			int pos = sHtml.ToLower().IndexOf("timelink");
			return pos >= 0;
		}

		public static NameValueCollection GetAllFleetsOnPlanet(string sHtml)
		{
			NameValueCollection fleets = new NameValueCollection(56);
			for (int i = 200; i < 220; i++)
			{
				string key = "am" + i;
				string num = getValue(sHtml, key);

				fleets.Add("maxship" + i, num);
				fleets.Add("capacity" + i, "");
				fleets.Add("speed" + i, "");
				fleets.Add("consumption" + i, "");
				
				if (num == "") continue;
				
				for (int k =0; k < FLEET_IDS.Length; k++)
				{
					if (FLEET_IDS[k] == i.ToString())
					{
						fleets["capacity" + i] = FLEET_CARGOS[k].ToString();
						break;
					}
				}
			}
			return fleets;
		}

		public static string[] GetAllFleets(NameValueCollection fleets)
		{
			ArrayList list = new ArrayList();
			for (int i = 200; i < 220; i++)
			{
				if (i == 212) continue; // 태양광 인공위성은 못 움직임

				string maxShip = fleets["maxship" + i];
				if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0) continue;
				list.Add(i.ToString());
			}
			return (string[]) list.ToArray(typeof (string));
		}

		private static string getValue(string sHtml, string key)
		{
			int pos = sHtml.IndexOf("shipsChosen." + key + ".value=");
			if (pos < 0) return "";

			pos = sHtml.IndexOf("value=", pos + 1);
			if (pos < 0) return "";

			pos += 6;
			int pos2 = sHtml.IndexOf(";", pos);

			return sHtml.Substring(pos, pos2 - pos);
		}

		/// <summary>
		/// 이동 가능한 모든 함대를 나열하고 실제 이동하는 함대는 함대 수를 입력한다.
		/// </summary>
		/// <param name="fleets"></param>
		/// <param name="expFleets"></param>
		/// <param name="useFleet"></param>
		/// <returns></returns>
		public static string GetFleetsAllOnPlanetText(NameValueCollection fleets, NameValueCollection expFleets, int[] useFleet)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 200; i < 220; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string maxShip = fleets["maxship" + i];
				if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0) continue;

				bool isAdded = false;
				for (int k = 0; k < expFleets.Count; k++)
				{
					if (expFleets.GetKey(k) == i.ToString())
					{
						sb.Append(string.Format("am{0}={1}",
						                        i,
						                        useFleet[k]));
						isAdded = true;
						break;
					}
				}

				if (!isAdded) sb.Append(string.Format("am{0}=", i));
			}
			return sb.ToString();
		}

		/// <summary>
		/// 이동 가능한 모든 함대를 나열하고 실제 이동하는 함대는 함대 수를 입력한다.
		/// </summary>
		/// <param name="fleets"></param>
		/// <param name="expFleets"></param>
		/// <param name="useFleet"></param>
		/// <returns></returns>
		public static string GetFleetsAllOnPlanetText(NameValueCollection fleets, string[] expFleets, int[] useFleet)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 200; i < 220; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string maxShip = fleets["maxship" + i];
				if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0) continue;

				bool isAdded = false;
				for (int k = 0; k < expFleets.Length; k++)
				{
					if (expFleets[k] == i.ToString())
					{
						sb.Append(string.Format("am{0}={1}",
												i,
												useFleet[k]));
						isAdded = true;
						break;
					}
				}

				if (!isAdded) sb.Append(string.Format("am{0}=", i));
			}
			return sb.ToString();
		}

		public static string GetFleetsText(NameValueCollection fleets, string fleetID, int count)
		{
			string maxShip = fleets["maxship" + fleetID];
			if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0 || count == 0) return "";
			if (count > int.Parse(maxShip)) count = int.Parse(maxShip);

			return string.Format("ship{0}={1}&consumption{0}={2}&speed{0}={3}&capacity{0}={4}",
			                     fleetID,
			                     count,
			                     fleets["consumption" + fleetID],
			                     fleets["speed" + fleetID],
			                     fleets["capacity" + fleetID]);
		}

		public static string GetFleetsText(NameValueCollection fleets, string fleetID, int count, out int useCount)
		{
			useCount = 0;
			string maxShip = fleets["maxship" + fleetID];
			if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0 || count == 0) return "";
			useCount = count > int.Parse(maxShip) ? int.Parse(maxShip) : count;

			return string.Format("am{0}={1}",
			                     fleetID,
								 useCount);
		}

		// 자원에 따른 함대 수 계산
		public static int GetFleetNum(string fleetID, int resQuantity)
		{
			int cargos = 0;
			for (int i = 0; i < FLEET_IDS.Length; i++)
			{
				if (fleetID == FLEET_IDS[i])
				{
					cargos = FLEET_CARGOS[i];
					break;
				}
			}
			if (cargos < 1) return 0;

			if (resQuantity%cargos == 0)
				return resQuantity/cargos;
			else
				return resQuantity/cargos + 1;
		}

		// 함대 수에 따른 자원 적재량
		public static int GetCapacity(string fleetID, int count)
		{
			int cargos = 0;
			for (int i = 0; i < FLEET_IDS.Length; i++)
			{
				if (fleetID == FLEET_IDS[i])
				{
					cargos = FLEET_CARGOS[i];
					break;
				}
			}
			if (cargos < 1) return 0;

			return cargos*count;
		}

		// 함대 구성에 따른 최대 속도를 구한다.
		public static int GetMaxSpeed(NameValueCollection allFleets, string[] fleets)
		{
			int result = 1000000000;
			for (int i = 0; i < fleets.Length; i++)
			{
				string maxSpeed = allFleets["speed" + fleets[i]];
				if (string.IsNullOrEmpty(maxSpeed) || int.Parse(maxSpeed) == 0) continue;

				result = Math.Min(result, int.Parse(maxSpeed));
			}
			return result;
		}

		// 함대 구성에 따른 최대 속도를 구한다.
		public static int GetMaxSpeed(NameValueCollection allFleets, NameValueCollection fleets)
		{
			int result = 1000000000;
			for (int i = 0; i < fleets.Count; i++)
			{
				string maxSpeed = allFleets["speed" + fleets.GetKey(i)];
				if (string.IsNullOrEmpty(maxSpeed) || int.Parse(maxSpeed) == 0 ||
				    string.IsNullOrEmpty(fleets[i]) || fleets[i] == "0") continue;

				result = Math.Min(result, int.Parse(maxSpeed));
			}
			return result;
		}

		// 우선순위 함대에 따른 최대 속도를 구한다.
		public static int[] GetMaxSpeed2(NameValueCollection allFleets, string[] fleets)
		{
			ArrayList list = new ArrayList();
			int result = 1000000000;
			for (int i = 0; i < fleets.Length; i++)
			{
				string maxSpeed = allFleets["speed" + fleets[i]];
				if (string.IsNullOrEmpty(maxSpeed) || int.Parse(maxSpeed) == 0) maxSpeed = result.ToString();

				result = Math.Min(result, int.Parse(maxSpeed));
				list.Add(result);
			}
			return (int[]) list.ToArray(typeof (int));
		}

		// 두 좌표 사이의 거리 계산
		public static int GetDistance(string from, string to)
		{
			string[] fromCoords = from.Split(new char[] {':'});
			string[] toCoords = to.Split(new char[] {':'});

			int dist;
			if ((int.Parse(toCoords[0]) - int.Parse(fromCoords[0])) != 0)
			{
				dist = Math.Abs(int.Parse(toCoords[0]) - int.Parse(fromCoords[0]))*20000;
			}
			else if ((int.Parse(toCoords[1]) - int.Parse(fromCoords[1])) != 0)
			{
				dist = Math.Abs(int.Parse(toCoords[1]) - int.Parse(fromCoords[1]))*5*19 + 2700;
			}
			else if ((int.Parse(toCoords[2]) - int.Parse(fromCoords[2])) != 0)
			{
				dist = Math.Abs(int.Parse(toCoords[2]) - int.Parse(fromCoords[2]))*5 + 1000;
			}
			else
			{
				dist = 5;
			}
			return dist;
		}

		// 두 좌표 사이를 이동하는데 걸리는 시간
		public static int GetDuration(int msp, int sp, int dist)
		{
			int ret = (int) Math.Round(((35000/(double) sp*Math.Sqrt(dist*10/(double) msp) + 10)/speedfactor));
			return ret;
		}

		// 두 좌표 사이를 이동하는데 걸리는 시간
		public static int[] GetDuration(int[] msp, int sp, int dist)
		{
			int[] ret = new int[msp.Length];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = (int) Math.Round(((35000/(double) sp*Math.Sqrt(dist*10/(double) msp[i]) + 10)/speedfactor));
			}
			return ret;
		}

		public static double GetConsumption(NameValueCollection allFleets, string fleetID, int count, int dist, int dur)
		{
			if (count == 0) return 0;
			Logger.Log("DEBUG: speed" + fleetID + "=" + allFleets["speed" + fleetID]);
			Logger.Log("DEBUG: consumption" + fleetID + "=" + allFleets["consumption" + fleetID]);
			int shipspeed = int.Parse(allFleets["speed" + fleetID]);
			double spd = 35000/(dur*speedfactor - 10)*Math.Sqrt(dist*10/(double) shipspeed);
			int basicConsumption = int.Parse(allFleets["consumption" + fleetID])*count;
			return basicConsumption*(double) dist/35000*Math.Pow((spd/10) + 1, 2);
		}

		// 탐사시간당 듀테륨 소모량
		public static double GetExpConsumption(NameValueCollection allFleets, string fleetID, int num, int expTime)
		{
			if (num == 0) return 0;
			double expDeut = double.Parse(allFleets["consumption" + fleetID]);
			return (expDeut * num / 10) * expTime;
		}

		public static void SaveAlarmSettings(SortedList<DateTime, string> list)
		{
			SettingsHelper settings = SettingsHelper.Current;

			if ((list != null) && (list.Count > 0))
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					if (i > 0) sb.Append("|");
					string text = list.Values[i];
					text = text.Replace("|", "\n").Replace("&", "\t");
					sb.Append(list.Keys[i].ToString("yyyy-MM-dd HH:mm") + "&" + list.Values[i]);
				}
				settings.EventSettings = sb.ToString();
				settings.Changed = true;
				// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
			}
			else
			{
				settings.EventSettings = "";
				settings.Changed = true;
				// settings.Save(); -- 환경설정 저장은 몰아서 한번에, 주기적으로
			}
		}

		public static SortedList<DateTime, string> LoadAlarmSettings()
		{
			string eventSettings = SettingsHelper.Current.EventSettings;
			if (string.IsNullOrEmpty(eventSettings)) return null;

			SortedList<DateTime, string> list = new SortedList<DateTime, string>();
			list.Clear();
			string[] s1 = eventSettings.Split(new char[] {'|'});
			for (int i = 0; i < s1.Length; i++)
			{
				string[] s2 = s1[i].Split(new char[] {'&'});

				if (s2.Length != 2) continue;
				DateTime sDate = DateTime.Parse(s2[0]);
				string text = s2[1].Replace("\t", "&").Replace("\n", "|");
				list.Add(sDate, text);
			}
			return list;
		}

		public static void LoadPrevSettings()
		{
			SettingsHelper settings = SettingsHelper.Current;
			Settings prevSettings = Settings.Default;

			if (!string.IsNullOrEmpty(settings.UserID) || !string.IsNullOrEmpty(settings.EventSettings)) return;

			string prevID = (string) prevSettings.GetPreviousVersion("userid");
			string prevSMS = (string) prevSettings.GetPreviousVersion("sms");
			string prevEventSettings = (string)prevSettings.GetPreviousVersion("eventSettings");

			if (string.IsNullOrEmpty(prevID) && string.IsNullOrEmpty(prevEventSettings)) return;

			settings.UserID = prevID;
			settings.Changed = true;

			// 패스워드 암호화
			string pwd = (string) prevSettings.GetPreviousVersion("pwd");
			if ((pwd.Length > 0) && (!pwd.StartsWith("enc:")))
			{
				if (OB2Security.IsStrongKey(prevID))
				{
					pwd = "enc:" + OB2Security.Encrypt(prevID, pwd);
				}
				else
				{
					string key = prevID + "12345678";
					pwd = "enc:" + OB2Security.Encrypt(key, pwd);
				}
			}
			settings.Password = pwd;

			settings.SMSphoneNum = prevSMS;
			settings.EventSettings = prevEventSettings;

			if (prevSettings.GetPreviousVersion("refreshRate") != null)
				settings.RefreshRate = (int)prevSettings.GetPreviousVersion("refreshRate");

			if (prevSettings.GetPreviousVersion("refreshRateMax") != null)
				settings.RefreshRateMax = (int)prevSettings.GetPreviousVersion("refreshRateMax");

			if (prevSettings.GetPreviousVersion("summerTimed") != null)
				settings.ApplySummerTime = (bool)prevSettings.GetPreviousVersion("summerTimed");

			if (prevSettings.GetPreviousVersion("useFireFox") != null)
				settings.UseFireFox = (bool) prevSettings.GetPreviousVersion("useFireFox");

			if (prevSettings.GetPreviousVersion("FireFoxDir") != null)
				settings.FireFoxDirectory = (string)prevSettings.GetPreviousVersion("FireFoxDir");

			if (prevSettings.GetPreviousVersion("showInLeftBottom") != null)
				settings.ShowInLeftBottom = (bool)prevSettings.GetPreviousVersion("showInLeftBottom");

			if (prevSettings.GetPreviousVersion("expedition") != null)
				settings.Expedition = (string)prevSettings.GetPreviousVersion("expedition");

			if (prevSettings.GetPreviousVersion("resCollecting") != null)
				settings.ResourceCollecting = (string)prevSettings.GetPreviousVersion("resCollecting");

			if (prevSettings.GetPreviousVersion("attackHash") != null)
				settings.AttackHash = (string)prevSettings.GetPreviousVersion("attackHash");

			if (prevSettings.GetPreviousVersion("smtpmail") != null)
			{
				SmtpMailInfo smtpInfo = SmtpMailInfo.ParseInfo((string)prevSettings.GetPreviousVersion("smtpmail"));
				if (smtpInfo != null) settings.SmtpMail = SmtpMailInfo.ToString(smtpInfo);
			}

			if (prevSettings.GetPreviousVersion("fleetSaving") != null)
				settings.FleetSaving = (string)prevSettings.GetPreviousVersion("fleetSaving");

			if (prevSettings.GetPreviousVersion("fleetMoving") != null)
				settings.FleetMoving = (string)prevSettings.GetPreviousVersion("fleetMoving");

			if (prevSettings.GetPreviousVersion("fleetMoving2") != null)
				settings.FleetMoving2 = (string)prevSettings.GetPreviousVersion("fleetMoving2");

#if AUTOFS
			if (prevSettings.GetPreviousVersion("autoFleetSaving") != null)
				settings.AutoFleetSaving = (string)prevSettings.GetPreviousVersion("autoFleetSaving");
#endif

			if (prevSettings.GetPreviousVersion("ogDomain") != null)
				settings.OGameDomain = (string)prevSettings.GetPreviousVersion("ogDomain");

			if (prevSettings.GetPreviousVersion("orgUniNames") != null)
				settings.OGameUniNames = ((string)prevSettings.GetPreviousVersion("orgUniNames")).Split('|');

			settings.Save(); // -- 이전 버전 환경설정에서 읽을 때는 바로 저장
		}

		public static string GetTime(int dur)
		{
			string theTime = "";
			int dd = dur / (3600 * 24);
			if (dd > 0) theTime += dd + "일 ";
			int hh = (dur - dd * 3600 * 24) / 3600;
			if (hh > 0) theTime += hh + "시간 ";
			int mm = (dur - hh * 3600) / 60;
			if (mm > 0) theTime += mm + "분 ";
			int ss = dur - hh * 3600 - mm * 60;
			theTime += ss + "초";
			return theTime;
		}

		public static NameObjectCollection GetFleetMoveInfo2()
		{
			NameObjectCollection fleetInfo2 = null;

			string fleetMoving2 = SettingsHelper.Current.FleetMoving2;
			if (string.IsNullOrEmpty(fleetMoving2)) return null;

			string[] ss = fleetMoving2.Split(new char[] { '$' });
			for (int cnt = 0; cnt < ss.Length; cnt++)
			{
				string[] s1 = ss[cnt].Split(new char[] { '|' });
				if (s1.Length < 10) return null;

				string key = s1[0];

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

				string[] s2 = s1[9].Split(new char[] { '^' });
				for (int i = 0; i < s2.Length; i++)
				{
					string[] s3 = s2[i].Split(new char[] { '&' });
					if (s3.Length == 2) info.Fleets.Add(s3[0], s3[1]);
				}

				if (fleetInfo2 == null) fleetInfo2 = new NameObjectCollection();
				if (fleetInfo2.ContainsKey(key))
					fleetInfo2[key] = info;
				else
					fleetInfo2.Add(key, info);
			}

			return fleetInfo2;
		}

		public static string FleetMoveInfo2ToString(NameObjectCollection fleetMoveInfo2)
		{
			if ((fleetMoveInfo2 == null) || (fleetMoveInfo2.Count == 0)) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < fleetMoveInfo2.Count; i++)
			{
				string id = fleetMoveInfo2.GetKey(i);
				FleetMoveInfo info = (FleetMoveInfo)fleetMoveInfo2[id];

				if (i > 0) sb.Append("$");

				sb.Append(id).Append("|");
				sb.Append(info.PlanetCoordinate).Append("|");
				sb.Append(info.TargetCoords).Append("|");
				sb.Append(info.TargetType).Append("|");
				sb.Append(info.MoveType).Append("|");
				sb.Append(info.Speed).Append("|");
				sb.Append(info.MoveResource ? "1" : "0").Append("|");
				sb.Append(info.RemainDeuterium).Append("|");
				sb.Append(info.AddEvent ? "1" : "0").Append("|");

				for (int k = 0; k < info.Fleets.Count; k++)
				{
					if (k > 0) sb.Append("^");
					sb.Append(info.Fleets.GetKey(k)).Append("&").Append(info.Fleets.Get(k));
				}
			}

			return sb.ToString();
		}

		public static string GetActionName(string action)
		{
			string actName = "";
			switch (action)
			{
				case "BB":
					actName = "건물: 건설";
					break;
				case "BC":
					actName = "건물: 취소";
					break;
				case "BD":
					actName = "건물: 파괴";
					break;
				case "RB":
					actName = "연구: 연구";
					break;
				case "RC":
					actName = "연구: 취소";
					break;
			}
			return actName;
		}
	}
}
