﻿using System;
using System.Threading;

namespace oBrowser2
{
	public class ResourceCollector
	{
		private Uri m_naviURL;
		private string m_contextCookies;

		private volatile ResourceInfo[] _res;

		/// <summary>
		/// Gets the resource infos.
		/// </summary>
		/// <value>The resource infos.</value>
		public ResourceInfo[] ResourceInfos
		{
			get { return _res; }
		}

		public event EventHandler<EventArgs> CollectCompleted;
		public event EventHandler<ColonyEventArgs> PartialCollectCompleted;

		public ResourceCollector()
		{
			_res = new ResourceInfo[18];
		}

		public void StartCollect(Uri url, string sHtml, string cookies)
		{
			try
			{
				m_naviURL = url;
				m_contextCookies = cookies;
				string contextHtml = sHtml.ToLower();

				int pos = sHtml.IndexOf("selectedPlanetName");
				if (pos < 0) return;
				pos = sHtml.IndexOf(">", pos + 1) + 1;
				int pos2 = sHtml.IndexOf("<", pos);
				string selectedPlanet = sHtml.Substring(pos, pos2 - pos);

				int count = 0;
				pos = sHtml.IndexOf("planet-name");
				while ((pos >= 0) && (count < 18))
				{
					_res[count] = new ResourceInfo();

					pos = sHtml.IndexOf(">", pos + 1) + 1; // ..."planet-name">Rod</div>
					pos2 = sHtml.IndexOf("<", pos);
					string sTemp = sHtml.Substring(pos, pos2 - pos);
					if (sTemp.Equals(selectedPlanet)) _res[count].IsInitialColony = true;
					_res[count].ColonyName = sTemp; // 대소문자 구분

					pos2 = sHtml.IndexOf("[", pos) + 1;
					int pos3 = sHtml.IndexOf("]", pos2 + 1);
					_res[count].Location = sHtml.Substring(pos2, pos3 - pos2).Trim();

					string sPlanet = contextHtml.Substring(0, pos);
					int pStart = sPlanet.LastIndexOf("<div");
					int pEnd = contextHtml.IndexOf("</div", pStart + 1);
					sPlanet = contextHtml.Substring(pStart, pEnd - pStart);

					int p1 = sPlanet.LastIndexOf("&amp;cp=");
					if (p1 > 0)
					{
						p1 += 8;
						int p2 = sPlanet.IndexOf("\"", p1);
						_res[count].ColonyID = sPlanet.Substring(p1, p2 - p1);
					}
					else
					{
						p1 = sPlanet.LastIndexOf("&cp=");
						if (p1 > 0)
						{
							p1 += 4;
							int p2 = sPlanet.IndexOf("\"", p1);
							_res[count].ColonyID = sPlanet.Substring(p1, p2 - p1);
						}
					}

					_res[count].ResourceList.Add("M", null);
					_res[count].ResourceList.Add("C", null);
					_res[count++].ResourceList.Add("D", null);

					pos = sHtml.IndexOf("planet-name", pos + 1);
				}

				if (count < 18)
				{
					for (int i = count; i < 18; i++)
						_res[i] = null;
				}

			}
			catch (Exception ex)
			{
				Logger.Log("ERROR: 자원 수집 에러(StartCollect) - " + ex);
			}

			Thread mainThread = new Thread(new ThreadStart(threadMain));
			mainThread.IsBackground = true;
			mainThread.Priority = ThreadPriority.AboveNormal;
			mainThread.Start();
		}
		
		private void threadMain()
		{
			try
			{
				int lastCount = 0;
				string preCookie = string.Empty;
				for (int i = 0; i < _res.Length; i++)
				{
					// 순차적으로 각 식민지 페이지를 호출한다.
					if (_res[i] != null)
					{
						if (!_res[i].IsInitialColony)
						{
							string url = m_naviURL.OriginalString + "&cp=" + _res[i].ColonyID;
							Logger.Log("DEBUG: [URL=" + url + "]");
							Uri newUri = new Uri(url);
							preCookie = string.Copy(m_contextCookies);
							processHtml(WebCall.GetHtml(newUri, ref m_contextCookies));
							if (m_contextCookies != preCookie)
								Logger.Log("쿠키변경: " + preCookie + " --> " + m_contextCookies);
							Thread.Sleep(200);
						}
						else
						{
							lastCount = i;
						}
					}
				}

				// 마지막으로 처음 열려있던 페이지를 호출한다.
				string lasturl = m_naviURL.OriginalString + "&cp=" + _res[lastCount].ColonyID;
				Logger.Log("DEBUG: [URL=" + lasturl + "]");
				Uri lastURL = new Uri(lasturl);
				int errCount = 0;
				preCookie = string.Copy(m_contextCookies);
				while (!processHtml(WebCall.GetHtml(lastURL, ref m_contextCookies)))
				{
					errCount++;
					Thread.Sleep(200);
					if (errCount >= 5)
					{
						Logger.Log("ERROR: 페이지 읽기 에러 - 페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.");
						break;
					}
				}
				if (m_contextCookies != preCookie)
					Logger.Log("쿠키변경: " + preCookie + " --> " + m_contextCookies);

				OnCollectCompleted(null);

			}
			catch (Exception ex)
			{
				Logger.Log("ERROR: 자원 수집 에러(threadMain) - " + ex);
			}
		}
		
		private bool processHtml(string sHtml)
		{
			string contextHtml = sHtml.ToLower();

			// 메인 페이지가 맞는지 다시 확인
			int pos = sHtml.IndexOf("Overview -");
			if (pos >= 0)
			{
				// 같은 이름의 행성 구분을 위한 장치
				int prePos = sHtml.IndexOf("selectedPlanetName");
				if (prePos < 0)
				{
					Logger.Log("자원수집 페이지 처리 실패2: " + sHtml);
					return false;
				}
				prePos = sHtml.IndexOf(">", prePos + 1) + 1;
				int prePos2 = sHtml.IndexOf("<", prePos);
				string colonyText = sHtml.Substring(prePos, prePos2 - prePos);

				prePos = sHtml.IndexOf("Temperature:");
				if (prePos < 0)
				{
					Logger.Log("자원수집 페이지 처리 실패3: " + sHtml);
					return false;
				}
				prePos = sHtml.IndexOf(">[", prePos + 1) + 2;
				prePos2 = sHtml.IndexOf("]<", prePos);
				string colonyCoords = sHtml.Substring(prePos, prePos2 - prePos);

				string colonyName = "";

                int colonyIndex = -1;
				for (int index = 0; index < _res.Length; index++)
				{
					if (_res[index] == null) continue;

//----------------->???? 행성 이름이 맞거나 좌표가 맞을 때
					if ((colonyText.Equals(_res[index].ColonyName) && colonyCoords.Equals(_res[index].Location)) ||
						(colonyText.Equals(_res[index].ColonyName.Replace(" (달)", "")) && colonyCoords.Equals(_res[index].Location)))
					{
						colonyIndex = index;
						colonyName = _res[index].ColonyName;
					}

					if (_res[index].ColonyID == "")
					{
						pos = sHtml.IndexOf("planet-name>" + colonyName);
						if (pos < 0) pos = sHtml.IndexOf("planet-name\">" + colonyName);
						if (pos < 0) continue;

						string sPlanet = contextHtml.Substring(0, pos);
						int pStart = sPlanet.LastIndexOf("<div");
						int pEnd = contextHtml.IndexOf("</div", pStart + 1);
						sPlanet = contextHtml.Substring(pStart, pEnd - pStart);

						int p1 = sPlanet.LastIndexOf("&amp;cp=");
						if (p1 > 0)
						{
							p1 += 8;
							int p2 = sPlanet.IndexOf("\"", p1);
							_res[index].ColonyID = sPlanet.Substring(p1, p2 - p1);
						}
						else
						{
							p1 = sPlanet.LastIndexOf("&cp=");
							if (p1 > 0)
							{
								p1 += 4;
								int p2 = sPlanet.IndexOf("\"", p1);
								_res[index].ColonyID = sPlanet.Substring(p1, p2 - p1);
							}
						}
						Logger.Log("DEBUG: [..._res.ColonyID=" + _res[index].ColonyID + "]");
					}
				}

				if (colonyIndex < 0)
				{
					Logger.Log("자원수집 페이지 처리 실패4: " + sHtml);
					return false;
				}

				pos = sHtml.IndexOf("metal_box");

				for (int i = 0; i < 3; i++)
				{
					pos = contextHtml.IndexOf("</b> <br><span class=", pos + 1) + 21;
					pos = contextHtml.IndexOf(">", pos + 1);
					int pos2 = contextHtml.IndexOf("</span", pos + 1);
					string sTemp = contextHtml.Substring(pos + 1, pos2 - pos - 1).Trim();
					string[] ss = sTemp.Replace(".", "").Split('/');

					sTemp = int.Parse(ss[0]).ToString("##,###,###,###");
					if (int.Parse(ss[0]) >= int.Parse(ss[1])) sTemp += "(*)";

                    switch (i)
                    {
                        case 0:
                            _res[colonyIndex].ResourceList["M"] = sTemp;
                            break;
                        case 1:
                            _res[colonyIndex].ResourceList["C"] = sTemp;
                            break;
                        case 2:
                            _res[colonyIndex].ResourceList["D"] = sTemp;
                            break;
                    }
				}

				pos = sHtml.IndexOf("Diameter:");
				if (pos < 0)
				{
					Logger.Log("자원수집 페이지 처리 실패5: " + sHtml);
					return false;
				}
				pos = contextHtml.IndexOf("(", pos + 1);
				int pos3 = contextHtml.IndexOf(")", pos + 1);
				string fields = contextHtml.Substring(pos + 1, pos3 - pos - 1).Replace("<span>", "").Replace("</span>", "");

                _res[colonyIndex].FieldsDeveloped = fields;

				ColonyEventArgs args = new ColonyEventArgs();
				args.ColonyName = colonyName;
				OnPartialCollectCompleted(args);
				return true;
			}
			else
			{
				Logger.Log("자원수집 페이지 처리 실패1: " + sHtml);
				return false;
			}
		}
		
		private void OnCollectCompleted(EventArgs args)
		{
			if (CollectCompleted != null)
				CollectCompleted(this, args);
		}

		private void OnPartialCollectCompleted(ColonyEventArgs args)
		{
			if (PartialCollectCompleted != null)
				PartialCollectCompleted(this, args);
		}
	}
}
