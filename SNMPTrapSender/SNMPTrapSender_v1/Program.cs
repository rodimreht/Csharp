using System;
using System.Collections;
using System.Threading;
using Lextm.SharpSnmpLib;

namespace SNMPTrapSenderv1
{
	class Program
	{
		private const string MANAGER_IP = "61.74.158.83";
		private const string COMMUNITY = "public";
		
		static void Main()
		{
			// 트랩OID
			uint[] trapOID = { 1, 3, 6, 1, 4, 1, 14008, 99999, 200, 1 };

			// 테스트: SNMPv2c 버전 메시지로 설정한다.
			TrapSender sender = new TrapSender(VersionCode.V2);

			int k = 0;
			while (true)
			{
				// 트랩 메시지로 전송할 변수 목록
				IList varBind = new ArrayList();

				// 테스트: 임의의 동일한 문자열을 11개 변수에 똑같이 채운다
				for (int i = 0; i < 11; i++)
				{
					string s = "TEST_DATA";
					if (i == 0) s += k;
					
					// Octet String 형식
					OctetString data = new OctetString(s);

					// 변수 OID
					uint[] arr2 = {1, 3, 6, 1, 4, 1, 14008, 99999, 100, (uint) i + 1};
					ObjectIdentifier oid = new ObjectIdentifier(arr2);
					
					varBind.Add(new Variable(oid, data));
				}

				// 트랩 메시지 전송
				sender.Send(MANAGER_IP, COMMUNITY, trapOID, varBind);
				Console.WriteLine("트랩 메시지가 전송되었습니다... " + k);

				// 100 밀리초에 한번씩만 보낸다
				Thread.Sleep(100);
				k++;

				// 1000번만 반복한다.
				if (k > 1000) break;
			}
		}
	}
}
