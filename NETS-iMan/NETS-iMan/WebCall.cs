using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace NETS_iMan
{
	public class WebCall
	{
		public static Stream GetStream(Uri targetURL, string cookies)
		{
			Stream stream = null;

			try
			{
				lock (typeof (WebCall))
				{
					HttpWebRequest req = (HttpWebRequest) WebRequest.Create(targetURL);
					req.Method = "GET";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept =
						"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
					req.Headers["Accept-Language"] = "ko-kr,en-us;q=0.7,en;q=0.3";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "EUC-KR,utf-8;q=0.7,*;q=0.7";
					req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] {"; "}, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;
					req.KeepAlive = true;
					req.Timeout = 15000;

					HttpWebResponse resp = (HttpWebResponse) req.GetResponse();

					int status = resp.StatusCode.GetHashCode();
					if (status == 200)
					{
						string encoding = resp.Headers["Content-Encoding"];
						if (encoding != null)
						{
							if (encoding.Equals("gzip"))
							{
								stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
							}
							else if (encoding.Equals("deflate"))
							{
								stream = new DeflateStream(resp.GetResponseStream(), CompressionMode.Decompress);
							}
						}
						else
						{
							stream = new BufferedStream(resp.GetResponseStream());
						}
					}
					return stream;
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

				Logger.Log(Logger.LogLevel.ERROR, "WebCall.GetStream(): " + errString);
			}
			catch (Exception ex)
			{
				Logger.Log(Logger.LogLevel.ERROR, "WebCall.GetStream(): " + ex);
			}

			return stream;
		}

		public static string GetHtml(Uri targetURL, string cookies)
		{
			try
			{
				lock (typeof(WebCall))
				{
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(targetURL);
					req.Method = "GET";
					req.ProtocolVersion = HttpVersion.Version11;
					req.Accept =
						"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
					//req.Headers["Accept-Language"] = "ko";
					req.Headers["Accept-Language"] = "ko-kr,en-us;q=0.7,en;q=0.3";
					req.Headers["Accept-Encoding"] = "gzip, deflate";
					req.Headers["Accept-Charset"] = "EUC-KR,utf-8;q=0.7,*;q=0.7";
					req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

					string domain = targetURL.Host;
					string[] arr = cookies.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

					CookieContainer cookieContainer = new CookieContainer(10);
					for (int i = 0; i < arr.Length; i++)
						cookieContainer.SetCookies(new Uri("http://" + domain + "/"), arr[i]);

					req.CookieContainer = cookieContainer;
					req.KeepAlive = true;
					req.Timeout = 15000;

					HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

					string sBody = "";
					int status = resp.StatusCode.GetHashCode();
					if (status == 200)
					{
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
						// 한글이 인식되지 않으면 utf-8 인코딩 --> euc-kr 인코딩으로 다시 읽기
						if (sBody.IndexOf("NETS-ⓘMan") < 0) sBody = Encoding.Default.GetString(decompressedBuffer);
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

		public static string Post(Uri targetURL, string referer, string cookies, string postData)
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
					req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

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
						sBody = Encoding.Default.GetString(decompressedBuffer);
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
