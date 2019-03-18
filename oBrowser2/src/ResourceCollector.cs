using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace oBrowser2
{
	public class ResourceCollector
	{
		private Uri _naviURL;
		private string _contextCookies;

		private volatile List<ResourceInfo> _res;

		/// <summary>
		/// Gets the resource infos.
		/// </summary>
		/// <value>The resource infos.</value>
		public ResourceInfo[] ResourceInfos
		{
			get { return _res.ToArray(); }
		}

		/// <summary>
		/// Gets the context cookies.
		/// </summary>
		public string ContextCookies
		{
			get { return _contextCookies; }
		}

		public event EventHandler<EventArgs> CollectCompleted;
		public event EventHandler<ColonyEventArgs> PartialCollectCompleted;

		public ResourceCollector()
		{
			_res = new List<ResourceInfo>();
		}

		public void StartCollect(Uri url, string sHtml, string cookies)
		{
			try
			{
				_naviURL = OB2Util.GetOverviewUri(url);
				_contextCookies = cookies;
				string contextHtml = sHtml.ToLower();

				int pos = sHtml.IndexOf("selectedPlanetName");
				if (pos < 0) return;
				pos = sHtml.IndexOf(">", pos + 1) + 1;
				int pos2 = sHtml.IndexOf("<", pos);
				string selectedPlanet = sHtml.Substring(pos, pos2 - pos);

				pos = sHtml.IndexOf("planet-name", pos);
				while (pos >= 0)
				{
					ResourceInfo resInfo = new ResourceInfo();

					pos = sHtml.IndexOf(">", pos + 1) + 1; // ..."planet-name">Rod</div>
					pos2 = sHtml.IndexOf("<", pos);
					string sTemp = sHtml.Substring(pos, pos2 - pos);
					if (sTemp.Equals(selectedPlanet)) resInfo.IsInitialColony = true;
					resInfo.ColonyName = sTemp; // 대소문자 구분

					pos2 = sHtml.IndexOf("[", pos) + 1;
					int pos3 = sHtml.IndexOf("]", pos2 + 1);
					resInfo.Location = sHtml.Substring(pos2, pos3 - pos2).Trim();

					string sPlanet = contextHtml.Substring(0, pos);
					string sPlanet2 = sHtml.Substring(0, pos);
					int pStart = sPlanet.LastIndexOf("<div");
					int pEnd = contextHtml.IndexOf("</div", pStart + 1);
					sPlanet = contextHtml.Substring(pStart, pEnd - pStart);
					sPlanet2 = sHtml.Substring(pStart, pEnd - pStart);

					int p1 = sPlanet.IndexOf("&cp=");
					if (p1 > 0)
					{
						p1 += 4;
						int p2 = sPlanet.IndexOf("\"", p1);
						resInfo.ColonyID = sPlanet.Substring(p1, p2 - p1);
					}
					else
					{
						p1 = sPlanet.IndexOf("&amp;cp=");
						if (p1 > 0)
						{
							p1 += 8;
							int p2 = sPlanet.IndexOf("\"", p1);
							resInfo.ColonyID = sPlanet.Substring(p1, p2 - p1);
						}
					}

					resInfo.ResourceList.Add("M", null);
					resInfo.ResourceList.Add("C", null);
					resInfo.ResourceList.Add("D", null);
					_res.Add(resInfo);

					// 달 찾기
					p1 = sPlanet.IndexOf("moonlink");
					if (p1 > 0)
					{
						resInfo = new ResourceInfo();

						p1 = sPlanet.Substring(0, p1).LastIndexOf("<a");
						int mPos = sPlanet.IndexOf("<b>", p1 + 1) + 3;
						if (mPos < 3) mPos = sPlanet.IndexOf("&lt;b&gt;", p1 + 1) + 9;
						int mPos2 = sPlanet.IndexOf(" [", mPos + 1);
						sTemp = sPlanet2.Substring(mPos, mPos2 - mPos);
						if (sTemp.Equals(selectedPlanet)) resInfo.IsInitialColony = true;
						resInfo.ColonyName = sTemp + " (달)";
						resInfo.Location = _res[_res.Count - 1].Location;

						p1 = sPlanet.IndexOf("&cp=", mPos2 + 1);
						if (p1 > 0)
						{
							p1 += 4;
							int p2 = sPlanet.IndexOf("\"", p1);
							resInfo.ColonyID = sPlanet.Substring(p1, p2 - p1);
						}
						else
						{
							p1 = sPlanet.IndexOf("&amp;cp=", mPos2 + 1);
							if (p1 > 0)
							{
								p1 += 8;
								int p2 = sPlanet.IndexOf("\"", p1);
								resInfo.ColonyID = sPlanet.Substring(p1, p2 - p1);
							}
						}

						resInfo.ResourceList.Add("M", null);
						resInfo.ResourceList.Add("C", null);
						resInfo.ResourceList.Add("D", null);
						_res.Add(resInfo);
					}

					pos = sHtml.IndexOf("planet-name", pos + 1);
				}
			}
			catch (Exception ex)
			{
				Logger.Log("ERROR: 자원 수집 에러(StartCollect) - " + ex);
			}

			Thread mainThread = new Thread(threadMain);
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
				for (int i = 0; i < _res.Count; i++)
				{
					// 순차적으로 각 식민지 페이지를 호출한다.
					if (!_res[i].IsInitialColony)
					{
						string url = _naviURL.OriginalString + "&cp=" + _res[i].ColonyID;
						Logger.Log("DEBUG: [URL=" + url + "]");
						Uri newUri = new Uri(url);
						preCookie = string.Copy(_contextCookies);
						processHtml(WebCall.GetHtml(newUri, ref _contextCookies));
						if (_contextCookies != preCookie)
							Logger.Log("쿠키변경: " + preCookie + " --> " + _contextCookies);
						Thread.Sleep(200);
					}
					else
					{
						lastCount = i;
					}
				}

				// 마지막으로 처음 열려있던 페이지를 호출한다.
				string lasturl = _naviURL.OriginalString + "&cp=" + _res[lastCount].ColonyID;
				Logger.Log("DEBUG: [URL=" + lasturl + "]");
				Uri lastURL = new Uri(lasturl);
				int errCount = 0;
				preCookie = string.Copy(_contextCookies);
				while (!processHtml(WebCall.GetHtml(lastURL, ref _contextCookies)))
				{
					errCount++;
					Thread.Sleep(200);
					if (errCount >= 5)
					{
						Logger.Log("ERROR: 페이지 읽기 에러 - 페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.");
						break;
					}
				}
				if (_contextCookies != preCookie)
					Logger.Log("쿠키변경: " + preCookie + " --> " + _contextCookies);

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

				prePos = sHtml.IndexOf("Temperature:"); // Position:은 여러개 있으므로 Temperature:로 검색
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
				for (int index = 0; index < _res.Count; index++)
				{
//----------------->???? 행성 이름이 맞거나 좌표가 맞을 때
					if ((colonyText.Equals(_res[index].ColonyName) && colonyCoords.Equals(_res[index].Location)) ||
						(colonyText.Equals(_res[index].ColonyName.Replace(" (달)", "")) && colonyCoords.Equals(_res[index].Location)))
					{
						colonyIndex = index;
						colonyName = _res[index].ColonyName;
					}

					if (_res[index].ColonyID == "")
					{
						pos = sHtml.IndexOf("planet-name  \">" + colonyName);
						if (pos < 0) pos = sHtml.IndexOf("planet-name\">" + colonyName);
						if (pos < 0) pos = sHtml.IndexOf("planet-name>" + colonyName);
						if (pos > 0)
						{
							string sPlanet = contextHtml.Substring(0, pos);
							int pStart = sPlanet.LastIndexOf("<div");
							int pEnd = contextHtml.IndexOf("</div", pStart + 1);
							sPlanet = contextHtml.Substring(pStart, pEnd - pStart);

							int p1 = sPlanet.IndexOf("&cp=");
							if (p1 > 0)
							{
								p1 += 4;
								int p2 = sPlanet.IndexOf("\"", p1);
								_res[index].ColonyID = sPlanet.Substring(p1, p2 - p1);
							}
							else
							{
								p1 = sPlanet.IndexOf("&amp;cp=");
								if (p1 > 0)
								{
									p1 += 8;
									int p2 = sPlanet.IndexOf("\"", p1);
									_res[index].ColonyID = sPlanet.Substring(p1, p2 - p1);
								}
							}
							Logger.Log("DEBUG: [..._res.ColonyID=" + _res[index].ColonyID + "]");
						}
						else
						{
							// 달 찾기
							int mPos = sHtml.IndexOf("<b>" + colonyName + " [");
							if (mPos < 0) mPos = sHtml.IndexOf("&lt;b&gt;" + colonyName + " [");
							if (mPos > 0)
							{
								string sPlanet = contextHtml.Substring(0, mPos);
								int pStart = sPlanet.LastIndexOf("<div");
								int pEnd = contextHtml.IndexOf("</div", pStart + 1);
								sPlanet = contextHtml.Substring(pStart, pEnd - pStart);

								int pp = sPlanet.IndexOf("&cp=");
								if (pp > 0)
								{
									pp += 4;
									int pp2 = sPlanet.IndexOf("\"", pp);
									_res[index].ColonyID = sPlanet.Substring(pp, pp2 - pp);
								}
								else
								{
									pp = sPlanet.IndexOf("&amp;cp=");
									if (pp > 0)
									{
										pp += 8;
										int pp2 = sPlanet.IndexOf("\"", pp);
										_res[index].ColonyID = sPlanet.Substring(pp, pp2 - pp);
									}
								}
							}
							Logger.Log("DEBUG: [..._res.ColonyID=" + _res[index].ColonyID + "] (달)");
						}
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
					pos = sHtml.IndexOf("&lt;table class=&quot;resourceTooltip", pos + 1);
					int pos2 = sHtml.IndexOf("/table", pos + 1);
					string resText = contextHtml.Substring(pos, pos2 - pos);
					resText = HttpUtility.HtmlDecode(resText);

					int resPos = resText.IndexOf("available:");
					if (resPos < 0) resPos = resText.IndexOf("Available:");
					resPos = resText.IndexOf("<span class=", resPos + 1);
					resPos = resText.IndexOf(">", resPos + 1);
					int resPos2 = resText.IndexOf("</span", resPos + 1);
					string sTemp = resText.Substring(resPos + 1, resPos2 - resPos - 1).Trim();
					string ss1 = sTemp.Replace(".", "");

					resPos = resText.IndexOf("storage capacity:");
					if (resPos < 0) resPos = resText.IndexOf("Storage capacity:");
					if (resPos < 0) resPos = resPos2 + 1;
					resPos = resText.IndexOf("<span class=", resPos + 1);
					resPos = resText.IndexOf(">", resPos + 1);
					resPos2 = resText.IndexOf("</span", resPos + 1);
					sTemp = resText.Substring(resPos + 1, resPos2 - resPos - 1).Trim();
					string ss2 = sTemp.Replace(".", "");

					sTemp = int.Parse(ss1).ToString("##,###,###,##0");
					if (int.Parse(ss1) >= int.Parse(ss2)) sTemp += "(*)";

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
				string fields = contextHtml.Substring(pos + 1, pos3 - pos - 1).Replace("<span>", "").Replace("\\/", "/").Replace("</span>", "");

                _res[colonyIndex].FieldsDeveloped = fields;

				ColonyEventArgs args = new ColonyEventArgs();
				args.ColonyName = colonyName;
				OnPartialCollectCompleted(args);
				return true;
			}
			else
			{
				if (sHtml.Length > 1000)
					Logger.Log("자원수집 페이지 처리 실패1: " + sHtml.Substring(0, 1000).Replace("\r\n", "\n") + "...(" + sHtml.Length + " bytes)");
				else
					Logger.Log("자원수집 페이지 처리 실패1: " + sHtml.Replace("\r\n", "\n"));

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
