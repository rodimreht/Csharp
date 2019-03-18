using System;
using System.Collections;
using System.Reflection;

namespace Nets.IM.Application.Accessibility.Policy
{
	internal class ADGroupPolicy
	{
		private string forestName;
		private string domainName;
		private string serverName;

		/// <summary>
		/// <see cref="T:Nets.IM.Application.Accessibility.Policy.ADGroupPolicy"/> Ŭ������ �� �ν��Ͻ��� �ʱ�ȭ�Ѵ�.
		/// </summary>
		/// <param name="forestName">������Ʈ��</param>
		/// <param name="domainName">�����θ�</param>
		/// <param name="serverName">������(IP�ּ�)</param>
		public ADGroupPolicy(string forestName, string domainName, string serverName)
		{
			this.forestName = forestName;
			this.domainName = domainName;
			this.serverName = serverName;
		}

		/// <summary>
		/// <see cref="T:Nets.IM.Application.Accessibility.Policy.ADGroupPolicy"/> Ŭ������ �� �ν��Ͻ��� �ʱ�ȭ�Ѵ�.
		/// </summary>
		/// <param name="domainName">�����θ�</param>
		/// <param name="serverName">������(IP�ּ�)</param>
		public ADGroupPolicy(string domainName, string serverName)
		{
			this.forestName = domainName;
			this.domainName = domainName;
			this.serverName = serverName;
		}

		/// <summary>
		/// <see cref="T:Nets.IM.Application.Accessibility.Policy.ADGroupPolicy"/> Ŭ������ �� �ν��Ͻ��� �ʱ�ȭ�Ѵ�.
		/// </summary>
		/// <param name="domainName">�����θ�</param>
		public ADGroupPolicy(string domainName)
		{
			this.forestName = domainName;
			this.domainName = domainName;
			this.serverName = "";
		}

		/// <summary>
		/// GPO�� ���� ������ ����� ��´�.
		/// </summary>
		/// <param name="gpoName">GPO ǥ���̸�</param>
		/// <returns></returns>
		public string[] GetGPOLinks(string gpoName)
		{
			try
			{
				object objGPM = createInstance("GPMgmt.GPM");
				object objGPMConstants = getObject(objGPM, "GetConstants", null);

				// Initialize the Domain object
				int useAnyDC = (int)getProperty(objGPMConstants, "UseAnyDC");
				int doNotValidateDC = (int)getProperty(objGPMConstants, "DoNotValidateDC");

				object objGPMDomain = (serverName.Length > 0) ? getObject(objGPM, "GetDomain", new object[] { domainName, serverName, doNotValidateDC })
					: getObject(objGPM, "GetDomain", new object[] { domainName, "", useAnyDC });

				// Initialize the Sites Container object
				object objGPMSitesContainer = (serverName.Length > 0) ? getObject(objGPM, "GetSitesContainer", new object[] { forestName, domainName, serverName, doNotValidateDC })
					: getObject(objGPM, "GetSitesContainer", new object[] { forestName, domainName, "", useAnyDC });

				// Find the specified GPO
				object searchProp = getProperty(objGPMConstants, "SearchPropertyGPODisplayName");
				object searchOperation = getProperty(objGPMConstants, "SearchOpEquals");
				object objGPMSearchCriteria = getObject(objGPM, "CreateSearchCriteria", null);
				call(objGPMSearchCriteria, "Add", new object[] { searchProp, searchOperation, gpoName });

				object objGPOList = getObject(objGPMDomain, "SearchGPOs", new object[] { objGPMSearchCriteria });
				int count = (int) getProperty(objGPOList, "Count");
				if (count == 0)
					throw new Exception("Did not find GPO: " + gpoName);
				else if (count > 1)
					throw new Exception("Found more than one matching GPO. Count: " + count);

				// Search for all SOM links for this GPO
				searchProp = getProperty(objGPMConstants, "SearchPropertySOMLinks");
				searchOperation = getProperty(objGPMConstants, "SearchOpContains");
				objGPMSearchCriteria = getObject(objGPM, "CreateSearchCriteria", null);
				
				object objGPO = getProperty(objGPOList, "Item", new object[] { 1 });
				call(objGPMSearchCriteria, "Add", new object[] { searchProp, searchOperation, objGPO });

				object objSOMList = getObject(objGPMDomain, "SearchSOMs", new object[] { objGPMSearchCriteria });
				object objSiteLinkList = getObject(objGPMSitesContainer, "SearchSites", new object[] { objGPMSearchCriteria });
				int count2 = (int)getProperty(objSOMList, "Count");
				int count3 = (int)getProperty(objSiteLinkList, "Count");
				if ((count2 == 0) && (count3 == 0))
					throw new Exception("No Site, Domain or OU links found for this GPO");
				else
				{
					ArrayList retList = new ArrayList();

					int somDomain = (int)getProperty(objGPMConstants, "somDomain");
					int somOU = (int)getProperty(objGPMConstants, "somOU");
					for (int i = 1; i <= count2; i++)
					{
						object objSOM = getProperty(objSOMList, "Item", new object[] { i });

						int type = (int)getProperty(objSOM, "Type");
						string strSOMType = "";
						if (type == somDomain)
							strSOMType = "[Domain]";
						else if (type == somOU)
							strSOMType = "[OU]";

						// Print GPO Domain and OU links
						retList.Add(strSOMType + getProperty(objSOM, "Path"));
					}

					// Print GPO Site Links
					for (int i = 1; i <= count3; i++)
					{
						object objSiteLink = getProperty(objSiteLinkList, "Item", new object[] { i });
						retList.Add("[Site]" + getProperty(objSiteLink, "Name"));
					}

					return (string[])retList.ToArray(typeof(string));
				}
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("������ ���������� GPMC�� ������ ������ �����ϴ�. �ش� �����ο� �Ҽӵ� ��ǻ�Ϳ��� �����ؾ� �մϴ�.");
			}
		}

		/// <summary>
		/// �� �������� ��� GPO�� ��´�
		/// </summary>
		/// <returns></returns>
		public string[] GetAllGPOs()
		{
			try
			{
				object objGPM = createInstance("GPMgmt.GPM");
				object objGPMConstants = getObject(objGPM, "GetConstants", null);

				// Initialize the Domain object
				int useAnyDC = (int)getProperty(objGPMConstants, "UseAnyDC");
				int doNotValidateDC = (int)getProperty(objGPMConstants, "DoNotValidateDC");

				object objGPMDomain = (serverName.Length > 0) ? getObject(objGPM, "GetDomain", new object[] { domainName, serverName, doNotValidateDC })
					: getObject(objGPM, "GetDomain", new object[] { domainName, "", useAnyDC });

				object objGPMSearchCriteria = getObject(objGPM, "CreateSearchCriteria", null);
				object objGPOList = getObject(objGPMDomain, "SearchGPOs", new object[] { objGPMSearchCriteria });
				int count = (int)getProperty(objGPOList, "Count");
				if (count == 0)
					throw new Exception("No GPOs in this Domain");
				else
				{
					ArrayList retList = new ArrayList();

					for (int i = 1; i <= count; i++)
					{
						object objGPO = getProperty(objGPOList, "Item", new object[] { i });
						retList.Add(getProperty(objGPO, "DisplayName"));
					}
					return (string[])retList.ToArray(typeof(string));
				}
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("������ ���������� GPMC�� ������ ������ �����ϴ�. �ش� �����ο� �Ҽӵ� ��ǻ�Ϳ��� �����ؾ� �մϴ�.");
			}
		}

		/// <summary>
		/// ��� ����� ��� GPO�� ��´�.
		/// </summary>
		/// <param name="target">���(OU Ȥ�� ������)�� ��ü DN</param>
		/// <returns></returns>
		public string[] GetLinkedGPOs(string target)
		{
			try
			{
				object objGPM = createInstance("GPMgmt.GPM");
				object objGPMConstants = getObject(objGPM, "GetConstants", null);

				// Initialize the Domain object
				int useAnyDC = (int)getProperty(objGPMConstants, "UseAnyDC");
				int doNotValidateDC = (int)getProperty(objGPMConstants, "DoNotValidateDC");

				object objGPMDomain = (serverName.Length > 0) ? getObject(objGPM, "GetDomain", new object[] { domainName, serverName, doNotValidateDC })
					: getObject(objGPM, "GetDomain", new object[] { domainName, "", useAnyDC });

				// Find the specified OU
				object objSOM = getObject(objGPMDomain, "GetSOM", new object[] { target });
				if (objSOM == null)
					throw new Exception("Did not find Target OU: " + target);

				object objGPOLinkList = getObject(objSOM, "GetGPOLinks", null);
				int count = (int)getProperty(objGPOLinkList, "Count");

				ArrayList retList = new ArrayList();
				for (int i = 1; i <= count; i++)
				{
					object objGPOLink = getProperty(objGPOLinkList, "Item", new object[] { i });

					object searchProp = getProperty(objGPMConstants, "SearchPropertyGPOID");
					object searchOperation = getProperty(objGPMConstants, "SearchOpEquals");
					string gpoID = (string)getProperty(objGPOLink, "GPOID");
					object objGPMSearchCriteria = getObject(objGPM, "CreateSearchCriteria", null);
					call(objGPMSearchCriteria, "Add", new object[] { searchProp, searchOperation, gpoID });

					object objGPOList = getObject(objGPMDomain, "SearchGPOs", new object[] { objGPMSearchCriteria });
					int count2 = (int)getProperty(objGPOList, "Count");
					if (count2 == 0)
						throw new Exception("Did not find GPO: " + gpoID);
					else if (count2 > 1)
						throw new Exception("Found more than one matching GPO. Count: " + count2);

					object objGPO = getProperty(objGPOList, "Item", new object[] { 1 });
					retList.Add(getProperty(objGPO, "DisplayName"));
				}
				return (string[])retList.ToArray(typeof(string));
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("������ ���������� GPMC�� ������ ������ �����ϴ�. �ش� �����ο� �Ҽӵ� ��ǻ�Ϳ��� �����ؾ� �մϴ�.");
			}
		}

		/// <summary>
		/// ��� ����� GPO ������ �����Ѵ�.
		/// </summary>
		/// <param name="target">���(OU Ȥ�� ������)�� ��ü DN</param>
		/// <param name="gpoName">GPO ǥ���̸�</param>
		public void DeleteGPOLink(string target, string gpoName)
		{
			try
			{
				object objGPM = createInstance("GPMgmt.GPM");
				object objGPMConstants = getObject(objGPM, "GetConstants", null);

				// Initialize the Domain object
				int useAnyDC = (int)getProperty(objGPMConstants, "UseAnyDC");
				int doNotValidateDC = (int)getProperty(objGPMConstants, "DoNotValidateDC");

				object objGPMDomain = (serverName.Length > 0) ? getObject(objGPM, "GetDomain", new object[] { domainName, serverName, doNotValidateDC })
					: getObject(objGPM, "GetDomain", new object[] { domainName, "", useAnyDC });

				// Find the specified OU
				object objSOM = getObject(objGPMDomain, "GetSOM", new object[] { target });
				if (objSOM == null)
					throw new Exception("Did not find Target OU: " + target);

				object objGPOLinkList = getObject(objSOM, "GetGPOLinks", null);
				int count = (int)getProperty(objGPOLinkList, "Count");
				for (int i = 1; i <= count; i++)
				{
					object objGPOLink = getProperty(objGPOLinkList, "Item", new object[] { i });
					string gpoID = (string)getProperty(objGPOLink, "GPOID");

					object searchProp = getProperty(objGPMConstants, "SearchPropertyGPOID");
					object searchOperation = getProperty(objGPMConstants, "SearchOpEquals");
					object objGPMSearchCriteria = getObject(objGPM, "CreateSearchCriteria", null);
					call(objGPMSearchCriteria, "Add", new object[] { searchProp, searchOperation, gpoID });

					object objGPOList = getObject(objGPMDomain, "SearchGPOs", new object[] { objGPMSearchCriteria });
					int count2 = (int)getProperty(objGPOList, "Count");
					if (count2 == 0)
						throw new Exception("Did not find GPO: " + gpoID);
					else if (count2 > 1)
						throw new Exception("Found more than one matching GPO. Count: " + count2);

					object objGPO = getProperty(objGPOList, "Item", new object[] { 1 });
					
					if ((string)getProperty(objGPO, "DisplayName") == gpoName)
						call(objGPOLink, "Delete", null);
				}
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("������ ���������� GPMC�� ������ ������ �����ϴ�. �ش� �����ο� �Ҽӵ� ��ǻ�Ϳ��� �����ؾ� �մϴ�.");
			}
		}

		/// <summary>
		/// ��� GPO ������ ���� �����.
		/// </summary>
		/// <param name="target">���(OU Ȥ�� ������)�� ��ü DN</param>
		/// <param name="gpoName">GPO ǥ���̸�</param>
		public void CreateGPOLink(string target, string gpoName)
		{
			// set this to the position the GPO evaluated at
            // a value of ? signifies appending it to the end of the list
			int intLinkPos = -1;
			
			try
			{
				object objGPM = createInstance("GPMgmt.GPM");
				object objGPMConstants = getObject(objGPM, "GetConstants", null);

				// Initialize the Domain object
				int useAnyDC = (int)getProperty(objGPMConstants, "UseAnyDC");
				int doNotValidateDC = (int)getProperty(objGPMConstants, "DoNotValidateDC");

				object objGPMDomain = (serverName.Length > 0) ? getObject(objGPM, "GetDomain", new object[] { domainName, serverName, doNotValidateDC })
					: getObject(objGPM, "GetDomain", new object[] { domainName, "", useAnyDC });

				// Find the specified GPO
				object searchProp = getProperty(objGPMConstants, "SearchPropertyGPODisplayName");
				object searchOperation = getProperty(objGPMConstants, "SearchOpEquals");
				object objGPMSearchCriteria = getObject(objGPM, "CreateSearchCriteria", null);
				call(objGPMSearchCriteria, "Add", new object[] { searchProp, searchOperation, gpoName });

				object objGPOList = getObject(objGPMDomain, "SearchGPOs", new object[] { objGPMSearchCriteria });
				int count = (int)getProperty(objGPOList, "Count");
				if (count == 0)
					throw new Exception("Did not find GPO: " + gpoName);
				else if (count > 1)
					throw new Exception("Found more than one matching GPO. Count: " + count);

				object objGPO = getProperty(objGPOList, "Item", new object[] { 1 });

				// Find the specified OU
				object objSOM = getObject(objGPMDomain, "GetSOM", new object[] { target });
				if (objSOM == null)
					throw new Exception("Did not find Target OU: " + target);

				call(objSOM, "CreateGPOLink", new object[] { intLinkPos, objGPO });
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("������ ���������� GPMC�� ������ ������ �����ϴ�. �ش� �����ο� �Ҽӵ� ��ǻ�Ϳ��� �����ؾ� �մϴ�.");
			}
		}
		
		// COM object�� �����Ѵ�.
		private object createInstance(string className)
		{
			Type type = Type.GetTypeFromProgID(className);
			return Activator.CreateInstance(type);
		}

		// �Ϲ� �Լ� ȣ��
		private void call(object objClass, string method, object[] args)
		{
			objClass.GetType().InvokeMember(method,
				BindingFlags.InvokeMethod,
				null,
				objClass,
				args);
		}

		// ��� ���� �Լ� ȣ��
		private object getObject(object objClass, string method, object[] args)
		{
			object rets = objClass.GetType().InvokeMember(method,
				BindingFlags.InvokeMethod,
				null,
				objClass,
				args);
			return rets;
		}

		// �Ӽ�
		private object getProperty(object objClass, string prop)
		{
			object rets = objClass.GetType().InvokeMember(prop,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField,
				null,
				objClass,
				null);
			return rets;
		}

		// �Ӽ�
		private object getProperty(object objClass, string prop, object[] args)
		{
			object rets = objClass.GetType().InvokeMember(prop,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField,
				null,
				objClass,
				args);
			return rets;
		}
	}
}
