using System;
using System.Collections.Generic;
using System.Net;
using Lextm.SharpSnmpLib;

namespace SNMPTrapSender
{
	/// <summary>
	/// SNMPv1, SNMPv2c 트랩 메시지를 전송하는 클래스
	/// </summary>
	public class TrapSender
	{
		private const VersionCode SNMP_V1 = VersionCode.V1;
		private const VersionCode SNMP_V2 = VersionCode.V2;

		private static long m_tick1;
		private VersionCode m_ver;
		private GenericCode m_gCode;
		private int m_specCode;

		static TrapSender()
		{
			m_tick1 = DateTime.Now.Ticks;
		}

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="ver">SNMP 버전</param>
		public TrapSender(VersionCode ver)
		{
			m_ver = ver;
			m_gCode = GenericCode.EnterpriseSpecific;
			m_specCode = 1;
		}

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="ver">SNMP 버전</param>
		/// <param name="genericCode">일반코드</param>
		/// <param name="specificCode">특정코드</param>
		public TrapSender(VersionCode ver, GenericCode genericCode, int specificCode)
		{
			m_ver = ver;
			m_gCode = genericCode;
			m_specCode = specificCode;
		}

		/// <summary>
		/// 트랩 메시지를 전송한다.
		/// </summary>
		/// <param name="trapOID">uint 정수 배열로 이루어진 트랩 메시지 OID</param>
		/// <param name="varBind">Variable Bindings: Variable 형식의 List 객체</param>
		public void Send(uint[] trapOID, IList<Variable> varBind)
		{
			IPAddress addr = Dns.GetHostAddresses(Dns.GetHostName())[0];
			ObjectIdentifier trapOid = new ObjectIdentifier(trapOID);
			string community = "public";

			Send(addr, addr, community, trapOid, varBind);
		}

		/// <summary>
		/// 트랩 메시지를 전송한다.
		/// </summary>
		/// <param name="managerIPAddress">SNMP 관리자 IP 주소</param>
		/// <param name="trapOID">uint 정수 배열로 이루어진 트랩 메시지 OID</param>
		/// <param name="varBind">Variable Bindings: Variable 형식의 List 객체</param>
		public void Send(string managerIPAddress, uint[] trapOID, IList<Variable> varBind)
		{
			string community = "public";

			Send(managerIPAddress, community, trapOID, varBind);
		}

		/// <summary>
		/// 트랩 메시지를 전송한다.
		/// </summary>
		/// <param name="managerIPAddress">SNMP 관리자(서버) IP 주소</param>
		/// <param name="community">커뮤니티 이름</param>
		/// <param name="trapOID">uint 정수 배열로 이루어진 트랩 메시지 OID</param>
		/// <param name="varBind">Variable Bindings: Variable 형식의 List 객체</param>
		public void Send(string managerIPAddress, string community, uint[] trapOID, IList<Variable> varBind)
		{
			IPAddress addr1 = IPAddress.Parse(managerIPAddress);
			IPAddress addr2 = Dns.GetHostAddresses(Dns.GetHostName())[0];
			ObjectIdentifier trapOid = new ObjectIdentifier(trapOID);

			Send(addr1, addr2, community, trapOid, varBind);
		}

		/// <summary>
		/// 트랩 메시지를 전송한다.
		/// </summary>
		/// <param name="managerIPAddress">SNMP 관리자(서버) IP 주소</param>
		/// <param name="agentIPAddress">SNMP 대리자(클라이언트) IP 주소</param>
		/// <param name="community">커뮤니티 이름</param>
		/// <param name="trapOID">uint 정수 배열로 이루어진 트랩 메시지 OID</param>
		/// <param name="varBind">Variable Bindings: Variable 형식의 List 객체</param>
		public void Send(string managerIPAddress, string agentIPAddress, string community, uint[] trapOID, IList<Variable> varBind)
		{
			IPAddress addr1 = IPAddress.Parse(managerIPAddress);
			IPAddress addr2 = IPAddress.Parse(agentIPAddress);
			ObjectIdentifier trapOid = new ObjectIdentifier(trapOID);

			Send(addr1, addr2, community, trapOid, varBind);
		}

		/// <summary>
		/// 트랩 메시지를 전송한다.
		/// </summary>
		/// <param name="managerIPAddress">SNMP 관리자(서버) IP 주소</param>
		/// <param name="agentIPAddress">SNMP 대리자(클라이언트) IP 주소</param>
		/// <param name="community">커뮤니티 이름</param>
		/// <param name="trapOID">uint 정수 배열로 이루어진 트랩 메시지 OID</param>
		/// <param name="varBind">Variable Bindings: Variable 형식의 List 객체</param>
		public void Send(IPAddress managerIPAddress, IPAddress agentIPAddress, string community, ObjectIdentifier trapOID, IList<Variable> varBind)
		{
			if (m_ver == VersionCode.V1)
			{
				long tick2 = DateTime.Now.Ticks;

				TrapV1Message msg = new TrapV1Message(SNMP_V1,
																agentIPAddress,
																community,
																trapOID,
																m_gCode,
																m_specCode,
																(int)((tick2 - m_tick1) / 100000),
																varBind);
				msg.Send(managerIPAddress, 162);
			}
			else if (m_ver == VersionCode.V2)
			{
				long tick2 = DateTime.Now.Ticks;

				TrapV2Message msg2 = new TrapV2Message(SNMP_V2,
													   community,
													   trapOID,
													   (int)((tick2 - m_tick1) / 100000),
													   varBind);
				msg2.Send(managerIPAddress, 162);
			}
		}
	}
}
