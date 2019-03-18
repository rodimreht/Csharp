using System;
using System.Threading;

namespace oBrowser2
{
	public class ResourceCollector
	{
		private string contextHtml;
		private Uri naviURL;
		private string contextCookies;

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
			naviURL = url;
			contextHtml = sHtml;
			contextCookies = cookies;

			int count = 0;
			int pos = contextHtml.ToLower().IndexOf("<option");
			while ((pos >= 0) && (count < 18))
			{
				_res[count] = new ResourceInfo();
				
				string sTemp = contextHtml.Substring(pos, contextHtml.ToLower().IndexOf("</option", pos) - pos);
				if (sTemp.ToLower().IndexOf("selected") >= 0) _res[count].IsInitialColony = true;

				sTemp = sTemp.Replace("&amp;", "&");
				int p1 = sTemp.IndexOf("&cp=");
				int p2 = sTemp.IndexOf("&mode=", p1 + 1);
				_res[count].ColonyID = sTemp.Substring(p1 + 4, p2 - p1 - 4);

				p1 = sTemp.IndexOf(">", p2 + 1);
				p2 = sTemp.ToUpper().IndexOf("[", p1 + 1);
				string sTemp2 = sTemp.Substring(p1 + 1, p2 - p1 - 1).Trim();
				if (sTemp2.IndexOf("<a") > 0)
				{
					int pTemp = sTemp.ToUpper().IndexOf("<", p1 + 1);
					sTemp2 = sTemp.Substring(p1 + 1, pTemp - p1 - 1).Trim();
				}
				_res[count].ColonyName = sTemp2;

				int p3 = sTemp.IndexOf("]", p2 + 1);
				_res[count].Location = sTemp.Substring(p2 + 1, p3 - p2 - 1).Trim();
				
				_res[count].ResourceList.Add("M", null);
				_res[count].ResourceList.Add("C", null);
				_res[count++].ResourceList.Add("D", null);

				pos = contextHtml.ToLower().IndexOf("<option", pos + 1);
			}
			
			if (count < 18)
			{
				for (int i = count; i < 18; i++)
					_res[i] = null;
			}

			Thread mainThread = new Thread(new ThreadStart(threadMain));
			mainThread.IsBackground = true;
			mainThread.Priority = ThreadPriority.AboveNormal;
			mainThread.Start();
		}
		
		private void threadMain()
		{
			int lastCount = 0;
			for (int i = 0; i < _res.Length; i++)
			{
				// 순차적으로 각 식민지 페이지를 호출한다.
				if (_res[i] != null)
				{
					if (!_res[i].IsInitialColony)
					{
						Uri newUri = new Uri(naviURL.OriginalString + "&cp=" + _res[i].ColonyID);
						processHtml(WebCall.GetHtml(newUri, contextCookies));
						Thread.Sleep(200);
					}
					else
					{
						lastCount = i;
					}
				}
			}
			
			// 마지막으로 처음 열려있던 페이지를 호출한다.
			Uri lastURL = new Uri(naviURL.OriginalString + "&cp=" + _res[lastCount].ColonyID);
			int errCount = 0;
			while (!processHtml(WebCall.GetHtml(lastURL, contextCookies)))
			{
				errCount++;
				Thread.Sleep(200);
				if (errCount >= 5)
				{
					Logger.Log("ERROR: 페이지 읽기 에러 - 페이지 에러가 지속적으로 발생하여 더이상 모니터링을 할 수 없습니다.");
					break;
				}
			}

			OnCollectCompleted(null);
		}
		
		private bool processHtml(string sHtml)
		{
			// 메인 페이지가 맞는지 다시 확인
			int pos = sHtml.ToLower().IndexOf("행성 매뉴");
			if (pos >= 0)
			{
				// 같은 이름의 행성 구분을 위한 장치
				int prePos = sHtml.Substring(0, pos).LastIndexOf("\n");
				string colonyText;
				if (prePos > 0)
					colonyText = sHtml.Substring(prePos, sHtml.IndexOf("\n", pos) - prePos);
				else
					colonyText = sHtml.Substring(pos, sHtml.IndexOf("\n", pos) - pos);
				
				string colonyName = "";
				
				pos = sHtml.IndexOf("&nbsp;&nbsp;");
				pos = sHtml.IndexOf("&nbsp;&nbsp;", pos + 1);

                int colonyIndex = -1;
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
					else if (sTemp.ToLower().IndexOf("<font >") >= 0)	// 새로 변경된 페이지 자원 표시
					{
						int p1 = sTemp.IndexOf(">");
						int p2 = sTemp.IndexOf("</", p1 + 1);
						sTemp = sTemp.Substring(p1 + 1, p2 - p1 - 1);
					}

                    if (colonyIndex == -1)
                    {
                        for (int index = 0; index < _res.Length; index++)
                        {
							if (_res[i] == null) continue;

                            // 행성 이름이 맞거나 좌표가 맞을 때
                            if (((colonyText.IndexOf(_res[index].ColonyName) > 0) && (colonyText.IndexOf(_res[index].ColonyID) > 0)) ||
								((colonyText.IndexOf(_res[index].ColonyName.Replace(" (달)", "")) > 0) && (colonyText.IndexOf(_res[index].ColonyID) > 0)))
                            {
                                colonyIndex = index;
                                colonyName = _res[index].ColonyName;
                                switch (i)
                                {
                                    case 0:
                                        _res[index].ResourceList["M"] = sTemp.Replace(".", ",");
                                        break;
                                    case 1:
                                        _res[index].ResourceList["C"] = sTemp.Replace(".", ",");
                                        break;
                                    case 2:
                                        _res[index].ResourceList["D"] = sTemp.Replace(".", ",");
                                        break;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                _res[colonyIndex].ResourceList["M"] = sTemp.Replace(".", ",");
                                break;
                            case 1:
                                _res[colonyIndex].ResourceList["C"] = sTemp.Replace(".", ",");
                                break;
                            case 2:
                                _res[colonyIndex].ResourceList["D"] = sTemp.Replace(".", ",");
                                break;
                        }
                    }
				}

                pos = sHtml.ToLower().IndexOf("developed fields\">", pos + 1);
                int pos3 = sHtml.ToLower().IndexOf("</a", pos + 1);
                string s1 = sHtml.Substring(pos + 18, pos3 - pos - 18);

                pos = sHtml.ToLower().IndexOf("developed fields\">", pos3 + 1);
                pos3 = sHtml.ToLower().IndexOf("</a", pos + 1);
                string s2 = sHtml.Substring(pos + 18, pos3 - pos - 18);

                _res[colonyIndex].FieldsDeveloped = s1.Trim() + " / " + s2.Trim();

				ColonyEventArgs args = new ColonyEventArgs();
				args.ColonyName = colonyName;
				OnPartialCollectCompleted(args);
				return true;
			}
			else
			{
				Logger.Log("자원수집 페이지 처리 실패: " + sHtml);
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
