using System;
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
			int pos = sHtml.IndexOf(" selected>");
			if (pos < 0) return "";

			pos = sHtml.IndexOf("[", pos + 1);
			int pos2 = sHtml.IndexOf("]", pos + 1);
			return sHtml.Substring(pos + 1, pos2 - pos - 1);
		}

		public static NameValueCollection GetAllFleetsOnPlanet(string sHtml)
		{
			NameValueCollection fleets = new NameValueCollection(56);
			for (int i = 200; i < 220; i++)
			{
				string key = "maxship" + i;
				fleets.Add(key, getValue(sHtml, key));
				key = "consumption" + i;
				fleets.Add(key, getValue(sHtml, key));
				key = "speed" + i;
				fleets.Add(key, getValue(sHtml, key));
				key = "capacity" + i;
				fleets.Add(key, getValue(sHtml, key));
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
			int pos = sHtml.IndexOf(key);
			if (pos < 0) return "";

			pos = sHtml.IndexOf("value=\"", pos + 1);
			if (pos < 0) return "";

			pos += 7;
			int pos2 = sHtml.IndexOf("\"", pos);

			return sHtml.Substring(pos, pos2 - pos);
		}

		public static string GetAllFleetsOnPlanetText(NameValueCollection fleets)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 200; i < 220; i++)
			{
				if (sb.Length > 0) sb.Append("&");

				string maxShip = fleets["maxship" + i];
				if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0) continue;

				sb.Append(string.Format("maxship{0}={1}&consumption{0}={2}&speed{0}={3}&capacity{0}={4}",
				                        i,
				                        fleets["maxship" + i],
				                        fleets["consumption" + i],
				                        fleets["speed" + i],
				                        fleets["capacity" + i]));
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
			useCount = count;
			string maxShip = fleets["maxship" + fleetID];
			if (string.IsNullOrEmpty(maxShip) || int.Parse(maxShip) == 0 || count == 0) return "";
			if (count > int.Parse(maxShip))
			{
				count = int.Parse(maxShip);
				useCount = count;
			}

			return string.Format("ship{0}={1}&consumption{0}={2}&speed{0}={3}&capacity{0}={4}",
			                     fleetID,
			                     count,
			                     fleets["consumption" + fleetID],
			                     fleets["speed" + fleetID],
			                     fleets["capacity" + fleetID]);
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
				if (string.IsNullOrEmpty(maxSpeed) || int.Parse(maxSpeed) == 0) continue;

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
			int shipspeed = int.Parse(allFleets["speed" + fleetID]);
			double spd = 35000/(dur*speedfactor - 10)*Math.Sqrt(dist*10/(double) shipspeed);
			int basicConsumption = int.Parse(allFleets["consumption" + fleetID])*count;
			return basicConsumption*(double) dist/35000*Math.Pow((spd/10) + 1, 2);
		}

		public static void SaveAlarmSettings(SortedList<DateTime, string> list)
		{
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
				SettingsHelper settings = SettingsHelper.Current;
				settings.EventSettings = sb.ToString();
				settings.Save();
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

			settings.Save();
		}
	}
}
