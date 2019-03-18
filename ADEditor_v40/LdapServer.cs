using System;
using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ActiveDs;

namespace ADEditor
{
	// Active Directory 계정에 사용되는 상수 목록
	public enum ADS_USER_FLAG_ENUM
	{
		ADS_UF_SCRIPT = 0x0001,
		ADS_UF_ACCOUNTDISABLE = 0x0002,
		ADS_UF_HOMEDIR_REQUIRED = 0x0008,
		ADS_UF_LOCKOUT = 0x0010,
		ADS_UF_PASSWD_NOTREQD = 0x0020,
		ADS_UF_PASSWD_CANT_CHANGE = 0x0040,
		ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x0080,
		ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x0100,
		ADS_UF_NORMAL_ACCOUNT = 0x0200,
		ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
		ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
		ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000,
		ADS_UF_DONT_EXPIRE_PASSWD = 0x10000,
		ADS_UF_MNS_LOGON_ACCOUNT = 0x20000,
		ADS_UF_SMARTCARD_REQUIRED = 0x40000,
		ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000,
		ADS_UF_NOT_DELEGATED = 0x100000,
		ADS_UF_USE_DES_KEY_ONLY = 0x200000,
		ADS_UF_DONT_REQUIRE_PREAUTH = 0x400000,
		ADS_UF_PASSWORD_EXPIRED = 0x800000,
		ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000
	};

	/// <summary>
	/// Summary description for LdapServer.
	/// </summary>
	public class LdapServer
	{
		public const char delim = '|';

		// ADAM Password 옵션
		public const int ADS_OPTION_PASSWORD_PORTNUMBER = 6;
		public const int ADS_OPTION_PASSWORD_METHOD = 7;

		public const int ADS_LDAP_PORT = 389;
		public const int ADS_PASSWORD_ENCODE_REQUIRE_SSL = 0;
		public const int ADS_PASSWORD_ENCODE_CLEAR = 1;

		public static DirectoryEntry SearchAD(DirectoryEntry AD, string uid)
		{
			try
			{
				using (DirectorySearcher searcher = new DirectorySearcher(AD))
				{
					// contact는 제외, user만 대상
					searcher.Filter = "(&(objectClass=user)(cn=" + uid + "))";
					searcher.SearchScope = SearchScope.Subtree;
					searcher.CacheResults = false;
					SearchResult result = searcher.FindOne();

					return result.GetDirectoryEntry();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static string GetRootDN(string dn)
		{
			int pos1 = dn.LastIndexOf("/");
			int pos2 = dn.ToLower().IndexOf("dc=");
			return dn.Substring(0, pos1 + 1) + dn.Substring(pos2);
		}

		/// <summary>
		/// AD에서 값을 읽어 문자열로 리턴한다.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="text">The text.</param>
		/// <param name="tag">The tag.</param>
		/// <returns></returns>
		public static string GetStringProperty(DirectoryEntry entry, string text, out string tag)
		{
			tag = null;
			try
			{
				if (entry.Properties[text].Value != null)
				{
					Type type = entry.Properties[text].Value.GetType();
					if (type.FullName == "System.__ComObject")
					{
						tag = type.FullName;
						return DateTime.FromFileTime(GetInt64Property(entry, text)).ToString("yyyy-MM-dd HH:mm:ss");
					}
					else
					{
						tag = null;
						return entry.Properties[text].Value.ToString();
					}
				}
			}
			catch (COMException)
			{
			}
			return null;
		}

		/// <summary>
		/// AD에서 값을 읽어 정수로 리턴한다.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static int GetIntProperty(DirectoryEntry entry, string property)
		{
			try
			{
				if (entry.Properties[property].Value != null)
					return (int)entry.Properties[property].Value;
			}
			catch (COMException)
			{
			}
			return 0;
		}

		/// <summary>
		/// AD에서 값을 읽어 64비트 정수로 리턴한다.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static long GetInt64Property(DirectoryEntry entry, string property)
		{
			try
			{
				if (entry.Properties[property].Value != null)
				{
					LargeInteger li = (LargeInteger)entry.Properties[property].Value;
					int highPart = li.HighPart;
					int lowPart = li.LowPart;
					if (lowPart == 0)	// 하위 32비트 정수가 0이면 값 설정 안된 상태
						return 0;
					else
						return highPart * (long)Math.Pow(2, 32) + lowPart;
				}
			}
			catch (COMException)
			{
			}
			return 0;
		}

		/// <summary>
		/// AD에서 DateTime 값을 64비트 정수 형태로 값을 설정한다.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="property"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static void SetInt64Property(DirectoryEntry entry, string property, DateTime val)
		{
			try
			{
				long lngValue = val.ToFileTimeUtc();
				/*
				entry.Properties[property].Value = lngValue;
				*/
				LargeInteger li = new LargeIntegerClass();
				li.HighPart = (int)(lngValue / (long)Math.Pow(2, 32));
				li.LowPart = (int)(lngValue - li.HighPart * (long)Math.Pow(2, 32));
				entry.Properties[property].Value = li;
			}
			catch (COMException)
			{
			}
			return;
		}

		/// <summary>
		/// LDAP 서버의 루트 패스를 가져온다.
		/// </summary>
		/// <returns></returns>
		public static string GetRootEntry(string reptype, string server, string uid, string pwd)
		{
			try
			{
				AuthenticationTypes rootAuthTypes = reptype.ToUpper().Equals("ADAM")
				                                    	? AuthenticationTypes.Signing | AuthenticationTypes.Sealing |
				                                    	  AuthenticationTypes.Secure
				                                    	: AuthenticationTypes.ServerBind;

				DirectoryEntry entry = new DirectoryEntry("LDAP://" + server + "/RootDSE", uid, pwd, rootAuthTypes);
				string defaultNamingContext = reptype.ToUpper().Equals("ADAM")
				                              	? (string) entry.Properties["NamingContexts"][2]
				                              	: (string) entry.Properties["defaultNamingContext"][0];

				entry.Close();
				return defaultNamingContext;
			}
			catch (DirectoryServicesCOMException ex)
			{
				if ((uint)ex.ErrorCode == 0x8007203b) // Local Exception
				{
					return null;
				}
				return "ERROR" + delim + ex;
			}
			catch (Exception err)
			{
				return "ERROR" + delim + err;
			}
		}

		/// <summary>
		/// LDAP 서버의 특정 경로의 하위 (One Depth) 디렉토리 목록을 가져와서 스트링으로 반환한다.
		/// </summary>
		/// <returns></returns>
		public static string GetChildEntry(string reptype, string server, string uid, string pwd, string parent)
		{
			string retval = "";
			IList list;

			AuthenticationTypes childAuthTypes = reptype.ToUpper().Equals("ADAM") ? AuthenticationTypes.ServerBind | AuthenticationTypes.Sealing | AuthenticationTypes.Secure : AuthenticationTypes.ServerBind;

			string ldapPath = "LDAP://" + server + "/" + parent;
			list = getChildEntries(ldapPath, uid, pwd, childAuthTypes);
			for (int i = 0; i < list.Count; i++)
				retval += list[i].ToString() + delim;

			return retval;
		}

		/// <summary>
		/// LDAP 서버의 특정 경로의 하위 (One Depth) 디렉토리 목록을 가져온다.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private static IList getChildEntries(string parent, string uid, string pwd, AuthenticationTypes authTypes)
		{
			IList list = new ArrayList();
			try
			{
				DirectoryEntry entry = new DirectoryEntry(parent, uid, pwd, authTypes);
				DirectorySearcher searcher = new DirectorySearcher(entry, "(|(objectClass=organizationalUnit)(objectClass=container))");
				searcher.SearchScope = SearchScope.OneLevel;
				SearchResultCollection result = searcher.FindAll();

				for (int i = 0, iend = result.Count; i < iend; i++)
					list.Add(result[i].GetDirectoryEntry().Path);
				entry.Close();
				return list;
			}
			catch (Exception)
			{
				//string errmsg = err.ToString();
				return list;
			}
		}
	}
}
