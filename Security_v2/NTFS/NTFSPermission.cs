/*********************************************************
 * Title		: NTFS 클래스 (NTFSPermission.cs)
 * 
 * Description	: NTFS 접근권한 설정으로 사용자의 허용되지
 *				  않은 접근을 차단하는 기능을 제공하는 클래스
 * Methods
 * -- SetPermission			: NTFS 접근 권한 설정
 * -- ResumePermission		: NTFS 접근 권한 초기화
 * -- (DeleteEveryoneACE)	: 기본적으로 허용된 everyone 계정 접근권한(상속)을 제거한다.
 * -- (ReorderDACL) 		: NTFS 접근 권한을 원래대로 돌린다.
 * -- (Logging) 			: 오류발생시 로그를 남긴다.
 * 
 * Created by Thermidor
 * -- (주) 넷츠 이세현(thermidor@nets.co.kr)
 * -- 연락처 : 0505-268-2382
**********************************************************/

using System;
using ADSSECURITYLib;
using ActiveDSLib;
using System.IO;

namespace NTFS
{
	/// <summary>
	/// NTFS 접근 권한을 제어하는 클래스
	/// </summary>
	public class NTFSPermission
	{
		public NTFSPermission()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

		/// <summary>
		/// NTFS 접근 권한을 설정한다.
		/// </summary>
		/// <param name="sDirPath">물리적인 실제 경로</param>
		/// <param name="sServerName">서버명</param>
		/// <param name="sDomain">도메인명</param>
		/// <param name="sUser">계정 ID</param>
		/// <param name="isLocal">로컬인지 여부</param>
		/// <returns>성공하면 true</returns>
		public bool SetPermission(String sDirPath, String sServerName, String sDomain, String sUser, bool isLocal)
		{
			String strDebug = null;
			try
			{
				// Platform SDK의 ADSSecurity COM DLL을 사용한다.
				ADsSecurity oADsSecurity = new ADsSecurityClass();

				if (isLocal)
				{
					sDirPath = "file://" + sDirPath;
				}
				else
				{
					sDirPath = "file://\\\\" + sServerName + "\\" + sDirPath.Replace(":", "$");
				}
				strDebug = "oFileSD";
				SecurityDescriptor oFileSD = (SecurityDescriptor)oADsSecurity.GetSecurityDescriptor(sDirPath);

				strDebug = "DeleteEveryoneACE";
				DeleteEveryoneACE(ref oFileSD);

				AccessControlList oDACL = (AccessControlList)oFileSD.DiscretionaryAcl;
				bool isExists = false;
				strDebug = "foreach";
				foreach (AccessControlEntry ace in oDACL)
				{
					if (ace.Trustee.ToLower() == sUser.ToLower())
					{
						ace.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_READ |
							(int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_WRITE |
							(int)ADS_RIGHTS_ENUM.ADS_RIGHT_DELETE |
							(int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_EXECUTE;
						ace.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
						ace.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

						isExists = true;
					}
				}

				// 사용자가 이미 있으면 그냥 통과한다.
				if (!isExists)
				{
					AccessControlEntry newACE = new AccessControlEntryClass();

					// 해당 사용자 추가
					if (sServerName.ToLower() != sDomain.ToLower())
					{
						newACE.Trustee = sDomain + "\\" + sUser;
					}
					else
					{
						newACE.Trustee = sUser;
					}
					newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_READ |
						(int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_WRITE |
						(int)ADS_RIGHTS_ENUM.ADS_RIGHT_DELETE |
						(int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_EXECUTE;
					newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

					strDebug = "AddAce:" + sDomain + "\\" + sUser;
					oDACL.AddAce(newACE);

					// 웹 접속을 위해 몇 가지 권한만 부여
					// 읽기 특성					: ADS_RIGHTS_ENUM.ADS_RIGHT_DS_LIST_OBJECT
					// 읽기 확장 특성				: ADS_RIGHTS_ENUM.ADS_RIGHT_DS_SELF
					// 폴더 만들기 / 데이터 추가	: ADS_RIGHTS_ENUM.ADS_RIGHT_ACTRL_DS_LIST
					// 읽기 권한					: ADS_RIGHTS_ENUM.ADS_RIGHT_READ_CONTROL
					/*
					newACE = new AccessControlEntryClass();
					newACE.Trustee = "everyone";
					newACE.AccessMask = (int) ADS_RIGHTS_ENUM.ADS_RIGHT_DS_SELF |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_ACTRL_DS_LIST |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_READ_CONTROL |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_DS_LIST_OBJECT |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_READ;
					newACE.AceFlags = 1 | (int) ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int) ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;
					
					strDebug = "AddAce:everyone";
					oDACL.AddAce(newACE);
					*/

					// Everyone 대신 인터넷 게스트 계정에만 권한 부여
					newACE = new AccessControlEntryClass();
					newACE.Trustee = "IUSR_PWS";
					newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_READ;
					newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

					strDebug = "AddAce:IUSR_PWS";
					oDACL.AddAce(newACE);

					// 도메인 관리자 추가
					if (sServerName.ToLower() != sDomain.ToLower())
					{
						newACE = new AccessControlEntryClass();
						newACE.Trustee = sDomain + "\\Administrator";
						newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_ALL;
						newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
						newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

						strDebug = "AddAce:" + sDomain + "\\Administrator";
						oDACL.AddAce(newACE);
					}

					// 웹서버(FTP서버 아님!!) 로컬 관리자 추가
					newACE = new AccessControlEntryClass();
					newACE.Trustee = "Administrator";
					newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_ALL;
					newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

					strDebug = "AddAce:Administrator";
					oDACL.AddAce(newACE);

					// 웹서버(FTP서버 아님!!) ASP 프로세스 실행계정:SYSTEM 계정 추가
					newACE = new AccessControlEntryClass();
					newACE.Trustee = "SYSTEM";
					newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_ALL;
					newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

					strDebug = "AddAce:SYSTEM";
					oDACL.AddAce(newACE);

					// 웹서버(FTP서버 아님!!) ASP 프로세스 실행계정:ASPNET 계정 추가
					newACE = new AccessControlEntryClass();
					newACE.Trustee = "ASPNET";
					newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_ALL;
					newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

					strDebug = "AddAce:ASPNET";
					oDACL.AddAce(newACE);

					strDebug = "ReorderDACL";
					ReorderDACL(ref oDACL);

					oFileSD.DiscretionaryAcl = oDACL;
					strDebug = "SetSecurityDescriptor1";
					oADsSecurity.SetSecurityDescriptor(oFileSD, sDirPath);
				}
				else
				{
					oFileSD.DiscretionaryAcl = oDACL;
					strDebug = "SetSecurityDescriptor2";
					oADsSecurity.SetSecurityDescriptor(oFileSD, sDirPath);
				}
				return true;
			}
			catch (Exception ex)
			{
				Logging("step[" + strDebug + "] " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// NTFS 접근 권한을 원래대로 돌린다.
		/// </summary>
		/// <param name="sDirPath">물리적인 실제 경로</param>
		/// <param name="sServerName">서버명</param>
		/// <param name="sDomain">도메인명</param>
		/// <param name="sUser">계정 ID</param>
		/// <param name="isLocal">로컬인지 여부</param>
		/// <returns>성공하면 true</returns>
		public bool ResumePermission(String sDirPath, String sServerName, String sDomain, bool isLocal)
		{
			String strDebug = null;
			try
			{
				// Platform SDK의 ADSSecurity COM DLL을 사용한다.
				ADsSecurity oADsSecurity = new ADsSecurityClass();

				if (isLocal)
				{
					sDirPath = "file://" + sDirPath;
				}
				else
				{
					sDirPath = "file://\\\\" + sServerName + "\\" + sDirPath.Replace(":", "$");
				}
				strDebug = "oFileSD";
				SecurityDescriptor oFileSD = (SecurityDescriptor)oADsSecurity.GetSecurityDescriptor(sDirPath);

				AccessControlList oDACL = (AccessControlList)oFileSD.DiscretionaryAcl;

				strDebug = "foreach";
				foreach (AccessControlEntry ace in oDACL)	// 모두 다 삭제한다.
				{
					oDACL.RemoveAce(ace);
				}

				AccessControlEntry newACE = new AccessControlEntryClass();

				// everyone에 상속가능한(모든) 권한을 부여한다.
				newACE.Trustee = "everyone";
				newACE.AccessMask = (int)ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_ALL;
				newACE.AceFlags = 1 | (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE
					| (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERITED_ACE;
				newACE.AceType = (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

				strDebug = "AddAce:everyone";
				oDACL.AddAce(newACE);

				strDebug = "ReorderDACL";
				ReorderDACL(ref oDACL);

				oFileSD.DiscretionaryAcl = oDACL;
				strDebug = "SetSecurityDescriptor1";
				oADsSecurity.SetSecurityDescriptor(oFileSD, sDirPath);

				return true;
			}
			catch (Exception ex)
			{
				Logging("step[" + strDebug + ":" + sDirPath + ":" + sServerName + ":" + sDomain + "] " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// 기본적으로 허용된 everyone 계정 접근권한(상속)을 제거한다.
		/// </summary>
		/// <param name="oSD">보안설명자</param>
		private void DeleteEveryoneACE(ref SecurityDescriptor oSD)
		{
			AccessControlList DACL = (AccessControlList)oSD.DiscretionaryAcl;

			foreach (AccessControlEntry ace in DACL)
			{
				// Remove all existing ACEs
				//				DACL.RemoveAce(ace);
				if (ace.Trustee.ToUpper() == "EVERYONE")
				{
					DACL.RemoveAce(ace);
				}
			}

			oSD.DiscretionaryAcl = DACL;
		}

		/// <summary>
		/// ACL을 순서에 맞게 재배열한다.
		/// </summary>
		/// <param name="DACL">재배열할 ACL</param>
		private void ReorderDACL(ref AccessControlList DACL)
		{
			AccessControlList newDACL = new AccessControlListClass();
			AccessControlList inheritedDACL = new AccessControlListClass();
			AccessControlList impDenyDACL = new AccessControlListClass();
			AccessControlList impDenyObjectDACL = new AccessControlListClass();
			AccessControlList impAllowDACL = new AccessControlListClass();
			AccessControlList impAllowObjectDACL = new AccessControlListClass();

			foreach (AccessControlEntry ace in DACL)
			{
				if ((ace.AceFlags & (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERITED_ACE) ==
					(int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERITED_ACE)
				{
					inheritedDACL.AddAce(ace);
				}
				else
				{
					switch (ace.AceType)
					{
						case (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED:
							impAllowDACL.AddAce(ace);
							break;
						case (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_DENIED:
							impDenyDACL.AddAce(ace);
							break;
						case (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED_OBJECT:
							impAllowObjectDACL.AddAce(ace);
							break;
						case (int)ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_DENIED_OBJECT:
							impDenyObjectDACL.AddAce(ace);
							break;
						default:
							break;
					}
				}
			}

			// 순서에 맞게 재배열한다.
			//   
			// 1. Implicit Deny
			// 2. Implicit Deny Object
			// 3. Implicit Allow
			// 4. Implicit Allow Object
			// 5. Inherited aces

			foreach (AccessControlEntry ace in impDenyDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach (AccessControlEntry ace in impDenyObjectDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach (AccessControlEntry ace in impAllowDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach (AccessControlEntry ace in impAllowObjectDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach (AccessControlEntry ace in inheritedDACL)
			{
				newDACL.AddAce(ace);
			}

			newDACL.AclRevision = DACL.AclRevision;
			DACL = (AccessControlList)newDACL;
		}

		/// <summary>
		/// 오류발생시 로그를 남긴다.
		/// </summary>
		/// <param name="strValue">로그 문자열</param>
		private void Logging(String strValue)
		{
			FileStream fs = new FileStream(@"C:\NTFSPermission.log", FileMode.Append, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs);
			sw.WriteLine(strValue);
			sw.Close();
			fs.Close();
		}
	}
}
