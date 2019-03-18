using System;
using ActiveDSLib;
using ADSSECURITYLib;

namespace IISWriteAllower
{
	/// <summary>
	/// Controller에 대한 요약 설명입니다.
	/// </summary>
	class Controller
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				PrintUsage();
				return;
			}

			SetWriteAllowed(args[0], args[1]);
		}

		private static void SetWriteAllowed(string iisPath, string allowedUser)
		{
			try
			{
				ADsSecurity oADsSecurity = new ADsSecurityClass();

				String sDirPath = "file://" + iisPath.ToLower();
				IADsSecurityDescriptor oFileSD = (IADsSecurityDescriptor) oADsSecurity.GetSecurityDescriptor(sDirPath);

				IADsAccessControlList oDACL = (IADsAccessControlList) oFileSD.DiscretionaryAcl;
				bool isExists = false;
				foreach(IADsAccessControlEntry ace in oDACL)
				{
					if (ace.Trustee.ToLower() == allowedUser.ToLower())
					{
						ace.AccessMask = (int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_READ |
							(int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_WRITE |
							(int) ADS_RIGHTS_ENUM.ADS_RIGHT_DELETE |
							(int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_EXECUTE;
						ace.AceFlags = 1 | (int) ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
						ace.AceType = (int) ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

						isExists = true;
					}
				}

				if (!isExists)
				{
					IADsAccessControlEntry newACE = new AccessControlEntryClass();
					newACE.Trustee = allowedUser.Trim();
					newACE.AccessMask = (int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_READ |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_WRITE |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_DELETE |
						(int) ADS_RIGHTS_ENUM.ADS_RIGHT_GENERIC_EXECUTE;
					newACE.AceFlags = 1 | (int) ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERIT_ACE;
					newACE.AceType = (int) ADS_ACETYPE_ENUM.ADS_ACETYPE_ACCESS_ALLOWED;

					oDACL.AddAce(newACE);

					ReorderDACL(ref oDACL);

					oFileSD.DiscretionaryAcl = oDACL;
					oADsSecurity.SetSecurityDescriptor(oFileSD, sDirPath);
				}
				else
				{
					oFileSD.DiscretionaryAcl = oDACL;
					oADsSecurity.SetSecurityDescriptor(oFileSD, sDirPath);
				}

				Console.WriteLine("\"" + iisPath + "\" is write-allowed to " + allowedUser + ".");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private static void ReorderDACL(ref IADsAccessControlList DACL)
		{
			IADsAccessControlList newDACL = new AccessControlListClass();
			IADsAccessControlList inheritedDACL = new AccessControlListClass();
			IADsAccessControlList impDenyDACL = new AccessControlListClass();
			IADsAccessControlList impDenyObjectDACL = new AccessControlListClass();
			IADsAccessControlList impAllowDACL = new AccessControlListClass();
			IADsAccessControlList impAllowObjectDACL = new AccessControlListClass();

			foreach(IADsAccessControlEntry ace in DACL)
			{
				if ((ace.AceFlags & (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERITED_ACE) == (int)ADS_ACEFLAG_ENUM.ADS_ACEFLAG_INHERITED_ACE)
				{
					inheritedDACL.AddAce(ace);
				}
				else
				{
					switch(ace.AceType)
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

			foreach(IADsAccessControlEntry ace in impDenyDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach(IADsAccessControlEntry ace in impDenyObjectDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach(IADsAccessControlEntry ace in impAllowDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach(IADsAccessControlEntry ace in impAllowObjectDACL)
			{
				newDACL.AddAce(ace);
			}

			foreach(IADsAccessControlEntry ace in inheritedDACL)
			{
				newDACL.AddAce(ace);
			}

			newDACL.AclRevision = DACL.AclRevision;
			DACL = newDACL;
		}

		/// <summary>
		/// Print the description of the program.
		/// Print the usage string.
		/// Provide version and author info.
		/// </summary>
		private static void PrintUsage()
		{
			Console.WriteLine(
				"Description: IISWriteAllower adds IIS_WPG or ASPNET to write-allowed group in ACL.\r\n" +
				"\r\n\r\n"+
				"IISWriteAllower <IIS virtual directory> <User or Group>"+
				"\r\n\r\n" + "Eg. IISWriteAllower \"c:\\Temp\\Temporal Diretory\" IIS_WPG" + 
				"\r\n\r\n"+
				"Version: 1.0" +
				"\r\n"+
				"Written by Thermidor(thermidor@nets.co.kr)"
				);
		}
	}
}
