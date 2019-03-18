using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace oBrowser2
{
	public class SendSMS
	{
		/***********************************
		 ***** 2008년 08월경 사용 막힘 *****
		 ***********************************
		 * 
		private static string[] REQUEST = new string[] { @"POST /servlets/NateonSparrow?receivenum={0}&receivename=%20&v_message=O-Game%20attack!!!({1})&sendnum=0105558282&rsvdate=000000000000&display=MMS&freeAmount=50&notFree=false HTTP/1.1
Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, *\/*
Referer: http://sms.nate.com/nateon_tui/sms.jsp
Accept-Language: ko
Content-Type: application/x-www-form-urlencoded
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)
Host: sms.nate.com
Content-Length: 18
Connection: Keep-Alive
Cache-Control: no-cache
Cookie: UVID=CQoLBgMKDgUHAgEEAQ0CBw==; pcid=115987105185957656; SITESERVER=ID=fb0650e061d1134a2340cf04bfeb131c; 4d41e294c59a35a07848fae6519b7922=3e93e53d436108f5f3943a7510be4e4ad056dd532198987b3eabe80e0f06116a1da4546aaa06a8db9758c7f9445e056dc205a826248bd54f0d9ccafe2b7de5b9; JSESSIONID=aCYuTCAN56AaFI9eDb

msg=&isFirst=first",
@"POST /servlets/NateonSparrow?receivenum={0}&receivename=%20&v_message=O-Game%20attack!!!({1})&sendnum=0105558282&rsvdate=000000000000&display=MMS&freeAmount=100&notFree=false HTTP/1.1
Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, *\/*
Referer: http://sms.nate.com/nateon_tui/sms.jsp
Accept-Language: ko
Content-Type: application/x-www-form-urlencoded
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)
Host: sms.nate.com
Content-Length: 18
Connection: Keep-Alive
Cache-Control: no-cache
Cookie: UVID=CQoLBgMKDgUHAgEEAQ0CBw==; pcid=115987105185957656; SITESERVER=ID=fb0650e061d1134a2340cf04bfeb131c; 4d41e294c59a35a07848fae6519b7922=63fa9347d135c3215f6a5fbbbbd46992a044d75d4147ba75a0ee10ef93881c591a7554483ed6e8d5b223a3ddb88f66b65c916b369d085335a9b850dd2db257caceac7a6e25ea2ac1da1328b9bb9f4e83; JSESSIONID=aCYuTCAN56AaFI9eDb

msg=&isFirst=first",
@"POST /servlets/NateonSparrow?receivenum={0}&receivename=%20&v_message=O-Game%20attack!!!({1})&sendnum=0105558282&rsvdate=000000000000&display=MMS&freeAmount=50&notFree=false HTTP/1.1
Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, *\/*
Referer: http://sms.nate.com/nateon_tui/sms.jsp
Accept-Language: ko
Content-Type: application/x-www-form-urlencoded
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)
Host: sms.nate.com
Content-Length: 18
Connection: Keep-Alive
Cache-Control: no-cache
Cookie: UVID=CQoLBgMKDgUHAgEEAQ0CBw==; pcid=115987105185957656; SITESERVER=ID=fb0650e061d1134a2340cf04bfeb131c; 4d41e294c59a35a07848fae6519b7922=894498255e9ba8862961baabbac217e78c65581ea45249912322623ffcd76d31b47923eb2848edf7f9b303330cc91129f1c93ef6e2e3b95a3048ca1b0048ac52; JSESSIONID=aCYuTCAN56AaFI9eDb

msg=&isFirst=first" };
		*/

		// 2008년 한국 오게임 서비스 종료로 네이트 SMS로 사용 중지
		/*
		private const string HOST = "sms.nate.com";
		// HttpWatch로 떠서 새로 적용: 2008-09-11
		private static readonly string[] POST_DATA = new string[]
		                                             	{
		                                             		"receivenum={0}&receivename=1&v_message=O%2DGame%20attack!!!({1})&sendnum=0105558282&rsvdate=000000000000&notFree=false"
		                                             		,
		                                             		"msg=&isFirst=first&receivenum={0}&receivename=1&v_message=O%2DGame%20attack!!!({1})&sendnum=0105558282&rsvdate=000000000000&display=MMS&freeAmount=100&notFree=false&gi_id=&gi_cnt="
		                                             		,
		                                             		"receivenum={0}&receivename=1&v_message=O%2DGame%20attack!!!({1})&sendnum=0105558282&rsvdate=000000000000&notFree=false"
		                                             	};

		private static readonly string[] REQUEST = new string[]
		                                           	{
		                                           		@"POST /servlets/NateonSparrow HTTP/1.1
Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/xaml+xml, application/vnd.ms-xpsdocument, application/x-ms-xbap, application/x-ms-application, \*\/\*
Referer: http://sms.nate.com/nateon2007/sms_ktf.jsp?mobile=1%7C{2}
Accept-Language: ko
Content-Type: application/x-www-form-urlencoded
UA-CPU: x86
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 1.1.4322; .NET CLR 3.0.04506.30)
Host: sms.nate.com
Content-Length: {1}
Connection: Keep-Alive
Cache-Control: no-cache
Cookie: dup_080901=11; UVID=CQoLBgMKDgUHAgEEAQ0CBw==; UD2=eff68568f29d3caa; 4d41e294c59a35a07848fae6519b7922=3e93e53d436108f5f3943a7510be4e4ad056dd532198987b3eabe80e0f06116aca67239219a379b8b445577620172581509bf3699a4eb2720e76f5d2e5457e683b14af0f50012d3d0a22a4db4448f3fa83c579d979e9a67d2436afd151f5701aba1fe7191d34a68806455bc407526d1e73c3559fb28501eb261d1c231aa64dd8258ae2982702b97f60f268664197f768; RDB=c802000000000000000100000022220000000000000000; ndr=dGhlcm1pZG9y|; GUID=074F4D3402279D1F5848A8E4E46DA0C6E978C987B23C0055; ENC=78D273754ED5FD7DC5A0C63C30C9ADE96F0BB68396F8100C36FEF80033CD43AA741297CAB1A40D66E31FB9E2C8AA488E68EDF9B51A5D6858EF142B3B64BDFC890E7CBF04E03FC2750B1FF88B12D02E9B91569C726036EB796C6B2F749EC5473DE186F0373155A9137BBED9A4C2EBCA9823A6332B8442775591C6B3E5CE4ABDE2A517D328B91B5245BF141D30CD35F9F5D5CDCBA3BB06189C3617A14EC762E72189571A29C48C994AEAE3CD2DD059B0B3EB227335C7B00216A5DEB89DBBA47E980621AC1E2272F7F63B38D4E39F1E21F23EC8F48CBED4A95076DD17E7A52C80E7B5ED04CCAC9A9DC77C85DEA07F5D90A5BED44A6520247AF373E02EF1B78CFE543E757C74A412336DF09466411CDBE7E09016F5543389D189A3C51F859B18D7A5625287A1CD5F306BCFA953FD4261C054663AD0FD8C6FD1E01A01C76097A2B7723A63C6ADA8FEB3B535DCEB93A06360E65F2BD96ED9144E5B904D5BB94F8A5DAD; ETC=cj=&nick=; n_=wMy8vMf2; RETVAL=2; CFN=683967127fb51caea1b0c8c2fa8a8f556fa6fc90459d65972c71b61bb6803720c1a07227872a33ddd4bc592e73a240a799b07ada1723a5f1cf2df0f6cf093d886287ab9f440b330a68039deb3c47aa4e9cde3f7d40f3c8651b66499150c581e03d54eaca09689c9eb927703cbea5ac70ad0b870a4c9802e77907d006c33751c52a55db592c6ba262c5c83cc634b51e3d88abb3df6a8ac009702ee8db8ab2c3e9; JSESSIONID=a6wmxiYbI6v_1qYWpX

{0}"
		                                           		,
		                                           		@"POST /servlets/NateonSparrow? HTTP/1.1
Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, \*\/\*
Referer: http://sms.nate.com/nateon2007/sms.jsp?mobile=1%7C{2}
Accept-Language: ko
Content-Type: application/x-www-form-urlencoded
UA-CPU: x86
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; InfoPath.1; .NET CLR 1.1.4322)
Host: sms.nate.com
Content-Length: {1}
Connection: Keep-Alive
Cache-Control: no-cache
Cookie: 4d41e294c59a35a07848fae6519b7922=894498255e9ba8862961baabbac217e78c65581ea45249912322623ffcd76d31b47923eb2848edf7f9b303330cc91129edffd226b4266129e509e1330f697f2f7e82e0b08c0557066c25d9075d64821347baf73b7a05032eea4d00797aac6039ad3bb223b90e39a127e5b761e93ee45b; dup_080901=11; JSESSIONID=aHlwCPJMq1leh52UpX

{0}"
		                                           		,
		                                           		@"POST /servlets/NateonSparrow HTTP/1.1
Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, \*\/\*
Referer: http://sms.nate.com/nateon2007/sms_ktf.jsp?mobile=1%7C{2}
Accept-Language: ko
Content-Type: application/x-www-form-urlencoded
UA-CPU: x86
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 1.1.4322)
Host: sms.nate.com
Content-Length: {1}
Connection: Keep-Alive
Cache-Control: no-cache
Cookie: pcid=12052225882183833; UVID=AQIEAAUPCQ0IDwMHCwoGBA==; UD2=3cda9ebc7145f64c; 4d41e294c59a35a07848fae6519b7922=63fa9347d135c3215f6a5fbbbbd46992a044d75d4147ba75a0ee10ef93881c59330a778cae121d8044e7719d1f5cfee56881169b7ff0f55bdde114c19013b65b02d3193c4557911c0b2547f19d240071b22e43832205a7edfb0317aa1864cc899a77e959468d25762784e9f38283c0ba666a166b9b9aca9617178d87cef30ee8f28181e6b7dba471ab8a7fb601b1c146; dup_080901=11; RDB=c802000000000000000100000024240000000000000000; ndr=YXRoZW5hNzI=|; GUID=074F4D3402279D1F5848A8E4EC6BD2B5FAE8DFF742127314; ENC=EC663615124F38E0774F3C280B16F79AC947B74322DB1D18792B6CAE0ED569D0605E4160131FFAB8AF94A43DD6605445F698A78C36BFE5C84A97EEC475619C6BE1490987B616446D6A20FA05A4EEBA3F1070E82F9FF3F13BB0CE5CCC8E82EF58065D4E70306E98CFDD8E98446B3E70EEA758BF76E865D810BF3AD5BCB996CCFE6BFD304D257FD062DD4D4BCA5182EDA2059C47683843D9DF129C63512288022A82956BE354F0C38A28FBDC542C74EEBB966AC08832E5F6F68066295411DCC5D2BFD375C6E779973E870EF7192BAE9456AE7AF81C8A66A1FACB0364E65BB917D00C50612BB329C6AC9B4A5224E45623D7CB050655023C6958CF9042381633B9C431D291AEE7BFD94C7560EDF44964DED47412BFDF2FD4B11865FF148223F1560A186F6A30672150CC1753C0C4F4ACEA6EE94747F9032D8D0A4D9DDA23E507E6B5BD35A658424B24902AA5C972F80F4F47EBDD7432BCB5DCF2DB55849A4E52EAD5; ETC=cj=&nick=; n_=uem787/4; RETVAL=2; CFN=57034af4327c811586bb5f69cea11fd7b58930c4a3a7f6c48942cd48db3

{0}"
		                                           	};

		public static void Send()
		{
			string targetPhone = SettingsHelper.Current.SMSphoneNum;
			targetPhone = targetPhone.Replace("-", "");
			if (targetPhone.Length == 0) return;

			IPHostEntry ipHostInfo = Dns.GetHostEntry(HOST);
			IPAddress remoteIP = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(remoteIP, 80);

			Socket sockHttp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				sockHttp.Connect(remoteEP);
				if (!sockHttp.Connected)
				{
					return;
				}

				string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				now = now.Replace(" ", "%20").Replace("-", "%2D").Replace(":", "%3A");

				Random r = new Random();
				int no = r.Next(0, 3);

				//***** 2008년 08월경 사용 막힘 *****
				//byte[] buffer = Encoding.ASCII.GetBytes(string.Format(REQUEST[no], targetPhone, now));

				string postData = string.Format(POST_DATA[no], targetPhone, now);
				byte[] buffer = Encoding.ASCII.GetBytes(string.Format(REQUEST[no], postData, postData.Length, targetPhone));

				sockHttp.Send(buffer, 0, buffer.Length, SocketFlags.None);

				byte[] sbuff = new Byte[sockHttp.Available];
				int bytesRec = sockHttp.Receive(sbuff, 0, sockHttp.Available, SocketFlags.None);
				string sTemp = Encoding.ASCII.GetString(sbuff, 0, bytesRec);
				Logger.Log("DEBUG: SMS 송신 결과 - " + sTemp);

				sockHttp.Shutdown(SocketShutdown.Both);
				sockHttp.Close();
			}
			catch (Exception ex)
			{
				Logger.Log("ERROR: SMS 송신 중 오류 발생 - " + ex);
			}
		}
		*/

		public static void Send(DateTime arrivalTime, string uniName, string description)
		{
			SettingsHelper helper = SettingsHelper.Current;
			if (!helper.UseGoogleSMS || helper.GMailAddress.Trim().Length == 0) return;

			GoogleCalendar.AddEvent(arrivalTime, helper.GMailAddress, helper.GoogleCalendarID, uniName, description);
		}
	}
}
