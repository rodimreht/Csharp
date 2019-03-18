using System;
using System.Net;

namespace GetSid
{
	/// <summary>
	/// CMain�� ���� ��� �����Դϴ�.
	/// </summary>
	class CMain
	{
		/// <summary>
		/// �ش� ���� ���α׷��� �� �������Դϴ�.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// ���� Active Directory �Ҽ� ������
			string userDomain = Environment.GetEnvironmentVariable("USERDOMAIN").ToLower();
			// ���� ��ǻ�� �̸�
			string hostName = Dns.GetHostName();
			// ������ ��Ʈ�ѷ�: ������, ��Ÿ: ��ǻ�� �̸�
			string domain = Environment.UserDomainName.ToLower();
			
			// �����ο� �Ҽӵ��� ���� �Ϲ� ��ǻ��
			if (hostName.Equals(domain))
				Console.WriteLine(hostName + ": " + SidTranslator.GetSid(hostName));
			else if (!userDomain.Equals(domain)) // �����ο� �Ҽӵ� �Ϲ� ��ǻ��
				Console.WriteLine(hostName + ": " + SidTranslator.GetSid(hostName));
			else // ������ ��Ʈ�ѷ�
				Console.WriteLine(hostName + ": " + SidTranslator.GetSid(domain));
		}
	}
}
