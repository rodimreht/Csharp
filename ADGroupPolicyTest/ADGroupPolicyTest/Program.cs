using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using GPMGMTLib;
using Nets.IM.Application.Accessibility.Policy;

namespace ADGroupPolicyTest
{
	class Program
	{
		[DllImport("advapi32.dll")]
		internal static extern int GetUserName(StringBuilder lpBuffer, ref int nSize);

		//[DllImport("advapi32.dll")]
		//internal static extern int LookupAccountName(string lpSystemName, string lpAccountName, int Sid, int cbSid, string ReferencedDomainName, int cbReferencedDomainName, int peUse);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool LookupAccountName(
			[In, MarshalAs(UnmanagedType.LPTStr)] string systemName,
			[In, MarshalAs(UnmanagedType.LPTStr)] string accountName,
			IntPtr sid,
			ref int cbSid,
			StringBuilder referencedDomainName,
			ref int cbReferencedDomainName,
			out int use);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool LookupAccountSid(
			[In, MarshalAs(UnmanagedType.LPTStr)] string systemName,
			IntPtr sid,
			[Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder name,
			ref int cbName,
			StringBuilder referencedDomainName,
			ref int cbReferencedDomainName,
			out int use);
		
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool ConvertSidToStringSid(
			IntPtr sid, 
			[In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool ConvertStringSidToSid(
			[In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid,
			ref IntPtr sid);
		
		#region GPMgmt.GPM 직접 참조
		
		static void temp()
		{
			// ------ SCRIPT CONFIGURATION ------
			string strGPO = "Default Domain Policy";	// e.g. SalesGPO
			string strForest = "soldev.net";				// e.g. rallencorp.com
			string strDomain = "soldev.net";				// e.g. rallencorp.com
			// ------ END CONFIGURATION ---------

			GPM objGPM = new GPM();
			GPMConstants objGPMConstants = objGPM.GetConstants();

			// Initialize the Domain object
			GPMDomain objGPMDomain = objGPM.GetDomain(strDomain, "", objGPMConstants.UseAnyDC);

			// Initialize the Sites Container object
			GPMSitesContainer objGPMSitesContainer = objGPM.GetSitesContainer(strForest,
				strDomain, "", objGPMConstants.UseAnyDC);

			// Find the specified GPO
			GPMSearchCriteria objGPMSearchCriteria = objGPM.CreateSearchCriteria();
			objGPMSearchCriteria.Add(objGPMConstants.SearchPropertyGPODisplayName,
				objGPMConstants.SearchOpEquals, strGPO);

			GPMGPOCollection objGPOList = objGPMDomain.SearchGPOs(objGPMSearchCriteria);
			if (objGPOList.Count == 0)
			{
				Console.WriteLine("Did not find GPO: " + strGPO);
				Console.WriteLine("Exiting.");
				return;
			}
			else if (objGPOList.Count > 1)
			{
				Console.WriteLine("Found more than one matching GPO. Count: " +
					objGPOList.Count);
				Console.WriteLine("Exiting.");
				return;
			}
			else
				Console.WriteLine("Found GPO: " + ((GPMGPO)objGPOList[1]).DisplayName);

			// Search for all SOM links for this GPO
			objGPMSearchCriteria = objGPM.CreateSearchCriteria();
			objGPMSearchCriteria.Add(objGPMConstants.SearchPropertySOMLinks,
				objGPMConstants.SearchOpContains, objGPOList[0]);
			GPMSOMCollection objSOMList = objGPMDomain.SearchSOMs(objGPMSearchCriteria);
			GPMSOMCollection objSiteLinkList = objGPMSitesContainer.SearchSites(objGPMSearchCriteria);

			if ((objSOMList.Count == 0) && (objSiteLinkList.Count == 0))
				Console.WriteLine("No Site, Domain or OU links found for this GPO");
			else
			{
				Console.WriteLine("Links:");
				foreach (GPMSOM objSOM in objSOMList)
				{
					/*
					GPMGPOLinksCollection cols = objSOM.GetGPOLinks();
					foreach (GPMGPOLink o in cols)
					{
						objGPMSearchCriteria = objGPM.CreateSearchCriteria();
						objGPMSearchCriteria.Add(objGPMConstants.SearchPropertyGPOID,
							objGPMConstants.SearchOpEquals, o.GPOID);

						objGPOList = objGPMDomain.SearchGPOs(objGPMSearchCriteria);
						if (objGPOList.Count == 0)
						{
							Console.WriteLine("Did not find GPO: " + strGPO);
							Console.WriteLine("Exiting.");
							return;
						}
						else if (objGPOList.Count > 1)
						{
							Console.WriteLine("Found more than one matching GPO. Count: " +
								objGPOList.Count);
							Console.WriteLine("Exiting.");
							return;
						}
						else
							Console.WriteLine("Found GPO: " + ((GPMGPO)objGPOList[1]).DisplayName);
					}
					*/
					
					string strSOMType = "";
					if (objSOM.Type == objGPMConstants.somDomain)
						strSOMType = "Domain";
					else if (objSOM.Type == objGPMConstants.somOU)
						strSOMType = "OU";

					// Print GPO Domain and OU links
					Console.WriteLine("  " + objSOM.Name + " (" + strSOMType + ")");
				}

				// Print GPO Site Links
				foreach (GPMSOM objSiteLink in objSiteLinkList)
					Console.WriteLine("  " + objSiteLink.Name + " (Site)");
			}
		}

		#endregion

		static void Main()
		{
			showAccount();
			
			// ------ SCRIPT CONFIGURATION ------
			string strGPO = "nets policy";	// e.g. SalesGPO
			string strForest = "soldev.net";				// e.g. rallencorp.com
			string strDomain = "soldev.net";				// e.g. rallencorp.com
			string strServer = "61.74.137.22";
			string strOU = "ou=solmkt,ou=nets,dc=soldev,dc=net";
			// ------ END CONFIGURATION ---------
			
			//test1(strForest, strDomain, strServer, strGPO);

			//test2(strDomain, strServer, strGPO);

			//test3(strDomain);

			test4(strDomain, strGPO, strOU);
			test5(strDomain, strGPO, strOU);
		}
		
		static void test1(string s1, string s2, string s3, string gpo)
		{
			ADGroupPolicy gp = new ADGroupPolicy(s1, s2, s3);
			try
			{
				string[] s = gp.GetGPOLinks(gpo);
				if (s != null)
					for (int i = 0; i < s.Length; i++)
						Console.WriteLine(s[i]);
				else 
					Console.WriteLine("결과가 없습니다.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		static void test2(string s1, string s2, string gpo)
		{
			ADGroupPolicy gp = new ADGroupPolicy(s1, s2);
			try
			{
				string[] s = gp.GetGPOLinks(gpo);
				if (s != null)
					for (int i = 0; i < s.Length; i++)
						Console.WriteLine(s[i]);
				else
					Console.WriteLine("결과가 없습니다.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		static void test3(string s1)
		{
			ADGroupPolicy gp = new ADGroupPolicy(s1);
			try
			{
				string[] s = gp.GetAllGPOs();
				for (int i = 0; i < s.Length; i++)
				{
					Console.WriteLine(s[i] + ":");

					string[] ss = gp.GetGPOLinks(s[i]);
					if (ss != null)
						for (int ii = 0; ii < ss.Length; ii++)
							Console.WriteLine(ss[ii]);
					else
						Console.WriteLine("결과가 없습니다.");

					Console.WriteLine("---------------------------------");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		static void test4(string s1, string gpo, string target)
		{
			ADGroupPolicy gp = new ADGroupPolicy(s1);
			try
			{
				string[] ss = gp.GetLinkedGPOs(target);
				bool isExist = false;
				for (int i = 0; i < ss.Length; i++)
				{
					Console.WriteLine(ss[i]);
					if (ss[i].Equals(gpo)) isExist = true;
				}

				if (!isExist)
				{
					gp.CreateGPOLink(target, gpo);

					string[] s = gp.GetGPOLinks(gpo);
					if (s != null)
						for (int i = 0; i < s.Length; i++)
							Console.WriteLine(s[i]);
					else
						Console.WriteLine("결과가 없습니다.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		static void test5(string s1, string gpo, string target)
		{
			ADGroupPolicy gp = new ADGroupPolicy(s1);
			try
			{
				string[] ss = gp.GetLinkedGPOs(target);
				for (int i = 0; i < ss.Length; i++)
				{
					if (ss[i].Equals(gpo))
						gp.DeleteGPOLink(target, gpo);
				}
				
				string[] s = gp.GetGPOLinks(gpo);
				if (s != null)
					for (int i = 0; i < s.Length; i++)
						Console.WriteLine(s[i]);
				else
					Console.WriteLine("결과가 없습니다.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		
		static void showAccount()
		{
			StringBuilder userName = new StringBuilder(256);
			int size = userName.Capacity;
			GetUserName(userName, ref size);
			
			Console.WriteLine(userName.ToString());
			
			string sid = GetSid(userName.ToString());
			Console.WriteLine(sid);
			Console.WriteLine(GetName(sid));
			
			string userName2 = WindowsIdentity.GetCurrent().Name;

			Console.WriteLine(userName2);
		}
		
		public static string GetSid(string name)
		{
			IntPtr _sid = IntPtr.Zero;    //pointer to binary form of SID string.
			int _sidLength = 0;            //size of SID buffer.
			int _domainLength = 0;        //size of domain name buffer.
			int _use;                    //type of object.
			//stringBuilder for domain name.
			StringBuilder _domain = new StringBuilder();
			int _error = 0;
			string _sidString = "";

			//first call of the function only returns the size 
			//of buffers (SID, domain name)
			LookupAccountName(null, name, _sid, ref _sidLength, _domain,
							   ref _domainLength, out _use);
			_error = Marshal.GetLastWin32Error();

			//error 122 (The data area passed to a system call is too small) 
			// normal behaviour.
			if (_error != 122)
			{
				throw (new Exception(new Win32Exception(_error).Message));
			}
			else
			{
				//allocates memory for domain name
				_domain = new StringBuilder(_domainLength);
				//allocates memory for SID
				_sid = Marshal.AllocHGlobal(_sidLength);
				bool _rc = LookupAccountName(null, name, _sid, ref _sidLength, _domain,
										  ref _domainLength, out _use);

				if (_rc == false)
				{
					_error = Marshal.GetLastWin32Error();
					Marshal.FreeHGlobal(_sid);
					throw (new Exception(new Win32Exception(_error).Message));
				}
				else
				{
					// converts binary SID into string
					_rc = ConvertSidToStringSid(_sid, ref _sidString);

					if (_rc == false)
					{
						_error = Marshal.GetLastWin32Error();
						Marshal.FreeHGlobal(_sid);
						throw (new Exception(new Win32Exception(_error).Message));
					}
					else
					{
						Marshal.FreeHGlobal(_sid);
						return _sidString;
					}
				}
			}
		}

		public static string GetName(string sid)
		{
			IntPtr _sid = IntPtr.Zero;    //pointer to binary form of SID string.
			int _nameLength = 0;        //size of object name buffer
			int _domainLength = 0;        //size of domain name buffer
			int _use;                    //type of object
			StringBuilder _domain = new StringBuilder();    //domain name variable
			int _error = 0;
			StringBuilder _name = new StringBuilder();        //object name variable

			//converts SID string into the binary form
			bool _rc0 = ConvertStringSidToSid(sid, ref _sid);

			if (_rc0 == false)
			{
				_error = Marshal.GetLastWin32Error();
				Marshal.FreeHGlobal(_sid);
				throw (new Exception(new Win32Exception(_error).Message));
			}

			//first call of method returns the size of domain name 
			//and object name buffers
			bool _rc = LookupAccountSid(null, _sid, _name, ref _nameLength, _domain,
							 ref _domainLength, out _use);
			
			_domain = new StringBuilder(_domainLength);    //allocates memory for domain name
			_name = new StringBuilder(_nameLength);        //allocates memory for object name
			_rc = LookupAccountSid(null, _sid, _name, ref _nameLength, _domain,
							 ref _domainLength, out _use);

			if (_rc == false)
			{
				_error = Marshal.GetLastWin32Error();
				Marshal.FreeHGlobal(_sid);
				throw (new Exception(new Win32Exception(_error).Message));
			}
			else
			{
				Marshal.FreeHGlobal(_sid);
				return _domain.ToString() + "\\" + _name.ToString();
			}
		}
	}
}
