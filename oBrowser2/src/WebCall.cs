using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace oBrowser2
{
	public class WebCall
	{
		// 13개 배열
		private static string[] _uas = 
		{
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.67 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.67 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1944.0 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.47 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.131 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.116 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1664.3 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.16 Safari/537.36",
			"Mozilla/5.0 (Windows NT {0}; Trident/7.0; rv:11.0) like Gecko",
			"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT {0}; Trident/6.0)",
			"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT {0}; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET CLR 1.1.4322)",
		};
		// 5개 배열
		private static string[] _winvers = { "6.3; WOW64", "6.3", "6.1; WOW64", "6.1", "5.1" };

		private static string _userAgent = null;
		public static string UserAgent
		{
			get { return _userAgent ?? getUserAgent(); }
		}

		private static string getUserAgent()
		{
			SettingsHelper helper = SettingsHelper.Current;
			if (string.IsNullOrEmpty(helper.UserAgent))
			{
				Random r = new Random();
				int ua = r.Next(0, 13);
				int ver = r.Next(0, 5);
				_userAgent = helper.UserAgent = string.Format(_uas[ua], _winvers[ver]);
				helper.Changed = true;
				helper.Save();
			}
			else
			{
				_userAgent = helper.UserAgent;
			}
			return _userAgent;
		}

		public static string GetHtml(Uri targetURL, ref string cookies, string referer = null)
		{
			try
			{
				lock (typeof (WebCall))
				{
					HttpWebRequest req = (HttpWebRequest) WebRequest.Create(targetURL);
					req.Method = "GET";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, */*";
					req.Headers["Accept-Language"] = "ko-KR";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
					req.UserAgent = UserAgent;

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] {"; "}, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;
					req.KeepAlive = true;
					req.Timeout = 15000;
					if (referer != null) req.Referer = referer;

					HttpWebResponse resp = (HttpWebResponse) req.GetResponse();

					string sBody = "";
					int status = resp.StatusCode.GetHashCode();
					if (status == 200)
					{
                        string setCookie = resp.Headers["Set-Cookie"];
                        if ((setCookie != null) && (setCookie.IndexOf("prsess_") >= 0)) // 쿠키 내용만 교체
                        {
                            int pos1 = setCookie.IndexOf("prsess_");
                            int pos2 = setCookie.IndexOf(";", pos1);
                            setCookie = setCookie.Substring(pos1, pos2 - pos1);
                            Logger.Log("Set-Cookie: " + setCookie);
                            pos1 = cookies.IndexOf("prsess_");
                            pos2 = cookies.IndexOf(";", pos1);
                            string org = cookies.Substring(pos1, pos2 - pos1);
                            cookies = cookies.Replace(org, setCookie);
                        }

						ArrayList arrBuff = new ArrayList(4096);
						string encoding = resp.Headers["Content-Encoding"];
						if (encoding != null)
						{
							if (encoding.Equals("gzip"))
							{
								GZipStream stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
							else if (encoding.Equals("deflate"))
							{
								DeflateStream stream = new DeflateStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
						}
						else
						{
							Stream stream = resp.GetResponseStream();
							readAllBytesFromStream(stream, arrBuff);
							stream.Close();
						}
						byte[] decompressedBuffer = (byte[]) arrBuff.ToArray(typeof (byte));
						sBody = Encoding.UTF8.GetString(decompressedBuffer);
						sBody = sBody.TrimEnd('\0');
					}
					resp.Close();

					return sBody;
				}
			}
			catch (WebException e)
			{
				string errString = "ERROR: ";
				HttpWebResponse resp = (HttpWebResponse) e.Response;
				if (resp != null)
				{
					if (resp.StatusCode == HttpStatusCode.Unauthorized)
					{
						string challenge = resp.GetResponseHeader("WWW-Authenticate");
						if (challenge != null)
							errString += "서버에서 다음의 요청이 발생했습니다: " + challenge;
					}
					else
						errString += "WebException 발생: " + e.Message;
				}
				else
					errString += "응답 내용이 없어 대기시간이 초과되었습니다.";

				return errString;
			}
			catch (Exception ex)
			{
				return "ERROR: [" + ex + "] " + ex.Message;
			}
		}

		private static void readAllBytesFromStream(Stream stream, ArrayList arrBuff)
		{
			while (true)
			{
				byte[] buffer = new byte[100];
				int bytesRead = stream.Read(buffer, 0, 100);
				if (bytesRead == 0)
				{
					break;
				}

				if (bytesRead < 100)
				{
					byte[] buff2 = new byte[bytesRead];
					Array.Copy(buffer, buff2, bytesRead);
					arrBuff.AddRange(buff2);
				}
				else
					arrBuff.AddRange(buffer);
			}
		}

		public static string Post(Uri targetURL, string referer, ref string cookies, string postData)
		{
			try
			{
				lock (typeof(WebCall))
				{
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(targetURL);
					req.Method = "POST";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, */*";
					req.Headers["Cache-Control"] = "max-age=0";
					req.Headers["Accept-Language"] = "ko-KR";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
					req.UserAgent = UserAgent;

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;

					req.Headers["Keep-Alive"] = "300";
					req.KeepAlive = true;
					req.Timeout = 7000;
					req.Referer = referer;
					req.ContentType = "application/x-www-form-urlencoded";
					req.ContentLength = postData.Length;

					StreamWriter sw = new StreamWriter(req.GetRequestStream());
					sw.Write(postData);
					sw.Close();

					HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

					string sBody = "";
					int status = resp.StatusCode.GetHashCode();
					if (status == 200)
					{
                        string setCookie = resp.Headers["Set-Cookie"];
                        if ((setCookie != null) && (setCookie.IndexOf("prsess_") >= 0)) // 쿠키 내용만 교체
                        {
                            int pos1 = setCookie.IndexOf("prsess_");
                            int pos2 = setCookie.IndexOf(";", pos1);
                            setCookie = setCookie.Substring(pos1, pos2 - pos1);
                            Logger.Log("Set-Cookie: " + setCookie);
                            pos1 = cookies.IndexOf("prsess_");
                            pos2 = cookies.IndexOf(";", pos1);
                            string org = cookies.Substring(pos1, pos2 - pos1);
                            cookies = cookies.Replace(org, setCookie);
                        }
                        
                        ArrayList arrBuff = new ArrayList(4096);
						string encoding = resp.Headers["Content-Encoding"];
						if (encoding != null)
						{
							if (encoding.Equals("gzip"))
							{
								GZipStream stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
							else if (encoding.Equals("deflate"))
							{
								DeflateStream stream = new DeflateStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
						}
						else
						{
							Stream stream = resp.GetResponseStream();
							readAllBytesFromStream(stream, arrBuff);
							stream.Close();
						}
						byte[] decompressedBuffer = (byte[])arrBuff.ToArray(typeof(byte));
						sBody = Encoding.UTF8.GetString(decompressedBuffer);
						sBody = sBody.TrimEnd('\0');
					}
					resp.Close();

					return sBody;
				}
			}
			catch (WebException e)
			{
				string errString = "ERROR: ";
				HttpWebResponse resp = (HttpWebResponse)e.Response;
				if (resp != null)
				{
					if (resp.StatusCode == HttpStatusCode.Unauthorized)
					{
						string challenge = resp.GetResponseHeader("WWW-Authenticate");
						if (challenge != null)
							errString += "서버에서 다음의 요청이 발생했습니다: " + challenge;
					}
					else
						errString += "WebException 발생: " + e.Message;
				}
				else
					errString += "응답 내용이 없어 대기시간이 초과되었습니다.";

				return errString;
			}
			catch (Exception ex)
			{
				return "ERROR: [" + ex + "] " + ex.Message;
			}
		}

		public static string GetAjax(Uri targetURL, string referer, ref string cookies)
		{
			try
			{
				lock (typeof(WebCall))
				{
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(targetURL);
					req.Method = "GET";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept = "*/*";
					req.Headers["Accept-Language"] = "ko-KR";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
					req.Headers["X-Requested-With"] = "XMLHttpRequest";
					req.UserAgent = UserAgent;

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;

					req.Headers["Keep-Alive"] = "300";
					req.KeepAlive = true;
					req.Timeout = 7000;
					req.Referer = referer;

					HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

					string sBody = "";
					int status = resp.StatusCode.GetHashCode();
					if (status == 200)
					{
                        string setCookie = resp.Headers["Set-Cookie"];
                        if ((setCookie != null) && (setCookie.IndexOf("prsess_") >= 0)) // 쿠키 내용만 교체
                        {
                            int pos1 = setCookie.IndexOf("prsess_");
                            int pos2 = setCookie.IndexOf(";", pos1);
                            setCookie = setCookie.Substring(pos1, pos2 - pos1);
                            Logger.Log("Set-Cookie: " + setCookie);
                            pos1 = cookies.IndexOf("prsess_");
                            pos2 = cookies.IndexOf(";", pos1);
                            string org = cookies.Substring(pos1, pos2 - pos1);
                            cookies = cookies.Replace(org, setCookie);
                        }

                        ArrayList arrBuff = new ArrayList(4096);
						string encoding = resp.Headers["Content-Encoding"];
						if (encoding != null)
						{
							if (encoding.Equals("gzip"))
							{
								GZipStream stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
							else if (encoding.Equals("deflate"))
							{
								DeflateStream stream = new DeflateStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
						}
						else
						{
							Stream stream = resp.GetResponseStream();
							readAllBytesFromStream(stream, arrBuff);
							stream.Close();
						}
						byte[] decompressedBuffer = (byte[])arrBuff.ToArray(typeof(byte));
						sBody = Encoding.UTF8.GetString(decompressedBuffer);
						sBody = sBody.TrimEnd('\0');
					}
					resp.Close();

					return sBody;
				}
			}
			catch (WebException e)
			{
				string errString = "ERROR: ";
				HttpWebResponse resp = (HttpWebResponse)e.Response;
				if (resp != null)
				{
					if (resp.StatusCode == HttpStatusCode.Unauthorized)
					{
						string challenge = resp.GetResponseHeader("WWW-Authenticate");
						if (challenge != null)
							errString += "서버에서 다음의 요청이 발생했습니다: " + challenge;
					}
					else
						errString += "WebException 발생: " + e.Message;
				}
				else
					errString += "응답 내용이 없어 대기시간이 초과되었습니다.";

				return errString;
			}
			catch (Exception ex)
			{
				return "ERROR: [" + ex + "] " + ex.Message;
			}
		}

		public static string PostAjax(Uri targetURL, string referer, ref string cookies, string postData)
		{
			try
			{
				lock (typeof(WebCall))
				{
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(targetURL);
					req.Method = "POST";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept = "*/*";
					req.Headers["Accept-Language"] = "ko-KR";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
					req.Headers["X-Requested-With"] = "XMLHttpRequest";
					req.UserAgent = UserAgent;

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;

					req.Headers["Keep-Alive"] = "300";
					req.KeepAlive = true;
					req.Timeout = 7000;
					req.Referer = referer;
					req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					req.ContentLength = postData.Length;

					StreamWriter sw = new StreamWriter(req.GetRequestStream());
					sw.Write(postData);
					sw.Close();

					HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

					string sBody = "";
					int status = resp.StatusCode.GetHashCode();
					if (status == 200)
					{
						string setCookie = resp.Headers["Set-Cookie"];
						if ((setCookie != null) && (setCookie.IndexOf("prsess_") >= 0)) // 쿠키 내용만 교체
						{
							int pos1 = setCookie.IndexOf("prsess_");
							int pos2 = setCookie.IndexOf(";", pos1);
							setCookie = setCookie.Substring(pos1, pos2 - pos1);
							Logger.Log("Set-Cookie: " + setCookie);
							pos1 = cookies.IndexOf("prsess_");
							pos2 = cookies.IndexOf(";", pos1);
							string org = cookies.Substring(pos1, pos2 - pos1);
							cookies = cookies.Replace(org, setCookie);
						}

						ArrayList arrBuff = new ArrayList(4096);
						string encoding = resp.Headers["Content-Encoding"];
						if (encoding != null)
						{
							if (encoding.Equals("gzip"))
							{
								GZipStream stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
							else if (encoding.Equals("deflate"))
							{
								DeflateStream stream = new DeflateStream(resp.GetResponseStream(), CompressionMode.Decompress);
								readAllBytesFromStream(stream, arrBuff);
								stream.Close();
							}
						}
						else
						{
							Stream stream = resp.GetResponseStream();
							readAllBytesFromStream(stream, arrBuff);
							stream.Close();
						}
						byte[] decompressedBuffer = (byte[])arrBuff.ToArray(typeof(byte));
						sBody = Encoding.UTF8.GetString(decompressedBuffer);
						sBody = sBody.TrimEnd('\0');
					}
					resp.Close();

					return sBody;
				}
			}
			catch (WebException e)
			{
				string errString = "ERROR: ";
				HttpWebResponse resp = (HttpWebResponse)e.Response;
				if (resp != null)
				{
					if (resp.StatusCode == HttpStatusCode.Unauthorized)
					{
						string challenge = resp.GetResponseHeader("WWW-Authenticate");
						if (challenge != null)
							errString += "서버에서 다음의 요청이 발생했습니다: " + challenge;
					}
					else
						errString += "WebException 발생: " + e.Message;
				}
				else
					errString += "응답 내용이 없어 대기시간이 초과되었습니다.";

				return errString;
			}
			catch (Exception ex)
			{
				return "ERROR: [" + ex + "] " + ex.Message;
			}
		}
	}
}
