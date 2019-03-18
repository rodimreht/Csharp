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
		public static string GetHtml(Uri targetURL, ref string cookies)
		{
			try
			{
				lock (typeof (WebCall))
				{
					HttpWebRequest req = (HttpWebRequest) WebRequest.Create(targetURL);
					req.Method = "GET";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept =
						"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
					//req.Headers["Accept-Language"] = "ko";
					req.Headers["Accept-Language"] = "ko-kr,en-us;q=0.7,en;q=0.3";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "EUC-KR,utf-8;q=0.7,*;q=0.7";
					req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ko; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.5 (.NET CLR 3.5.30729)";

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] {"; "}, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;
					req.KeepAlive = true;
					req.Timeout = 15000;

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
					req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Language"] = "ko";
					req.Headers["Accept-Charset"] = "EUC-KR";
					req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ko; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.5 (.NET CLR 3.5.30729)";

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

		public static string PostAjax(Uri targetURL, string referer, ref string cookies, string postData)
		{
			try
			{
				lock (typeof(WebCall))
				{
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(targetURL);
					req.Method = "POST";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Language"] = "ko";
					req.Headers["Accept-Charset"] = "EUC-KR";
					req.Headers["X-Requested-With"] = "XMLHttpRequest";
					req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ko; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.5 (.NET CLR 3.5.30729)";

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
	}
}
