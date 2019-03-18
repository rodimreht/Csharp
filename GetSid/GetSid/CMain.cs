using System;
using System.Net;

namespace GetSid
{
	/// <summary>
	/// CMain에 대한 요약 설명입니다.
	/// </summary>
	class CMain
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// 실제 Active Directory 소속 도메인
			string userDomain = Environment.GetEnvironmentVariable("USERDOMAIN").ToLower();
			// 실제 컴퓨터 이름
			string hostName = Dns.GetHostName();
			// 도메인 컨트롤러: 도메인, 기타: 컴퓨터 이름
			string domain = Environment.UserDomainName.ToLower();
			
			// 도메인에 소속되지 않은 일반 컴퓨터
			if (hostName.Equals(domain))
				Console.WriteLine(hostName + ": " + SidTranslator.GetSid(hostName));
			else if (!userDomain.Equals(domain)) // 도메인에 소속된 일반 컴퓨터
				Console.WriteLine(hostName + ": " + SidTranslator.GetSid(hostName));
			else // 도메인 컨트롤러
				Console.WriteLine(hostName + ": " + SidTranslator.GetSid(domain));
		}
	}
}
