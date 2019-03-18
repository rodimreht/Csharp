using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text;
using Nets.IM.Common;

namespace iManService
{
	/// <summary>
	/// ADUserInfo에 대한 요약 설명입니다.
	/// </summary>
	public class ADUserInfo
	{
		private static DirectoryEntry rootDE;
		private static ADBase adBase;
		private static readonly string[] excludes;

		static ADUserInfo()
		{
			adBase = new ADBase("mgtada");

			string excls = ConfigurationManager.AppSettings["excludeGroups"];
			excludes = excls.Split(new char[] {';'});
		}

		private static DirectoryEntry getRootDE()
		{
			if (rootDE == null)
			{
				if (adBase == null)
					adBase = new ADBase("mgtada");

				rootDE = adBase.BindToAD(true);
			}
			return rootDE;
		}

		/// <summary>
		/// 사용자 목록을 이름 순으로 조회한다.
		/// </summary>
		/// <returns></returns>
		private static ICollection getUserList()
		{
			try
			{
				using (DirectorySearcher ds = new DirectorySearcher(getRootDE()))
				{
					ds.Filter = "(objectClass=user)";
					ds.SearchScope = SearchScope.OneLevel;
					ds.PropertiesToLoad.Add("cn");
					ds.PropertiesToLoad.Add("displayName");
					ds.PropertiesToLoad.Add("title");
					ds.PropertiesToLoad.Add("employeeID");

					ds.Sort.Direction = SortDirection.Ascending;
					ds.Sort.PropertyName = "displayName";

					return createData(ds.FindAll());
				}
			}
			catch (COMException)
			{
			}
			return null;
		}

		/// <summary>
		/// 사용자의 문자열 속성을 조회한다.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		public static string GetAttribute(string userID, string attrName)
		{
			string retVal = null;
			try
			{
				using (DirectoryEntry user = getRootDE().Children.Find("cn=" + userID, "user"))
				{
					try
					{
						retVal = user.Properties[attrName].Value.ToString();
					}
					catch (Exception)
					{
						retVal = "";
					}
					user.Close();
				}
			}
			catch (COMException)
			{
			}
			return retVal;
		}

		/// <summary>
		/// 임시/계약직 사용자의 문자열 속성을 조회한다.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		public static string GetTempAttribute(string userID, string attrName)
		{
			string retVal = null;
			try
			{
				using (DirectoryEntry ou = adBase.BindToChild("ou=임시계약직"))
				{
					using (DirectoryEntry user = ou.Children.Find("cn=" + userID, "user"))
					{
						try
						{
							retVal = user.Properties[attrName].Value.ToString();
						}
						catch (Exception)
						{
							retVal = "";
						}
						user.Close();
					}
					ou.Close();
				}
			}
			catch (COMException)
			{
			}
			return retVal;
		}

		private static string getStringAttribute(DirectoryEntry entry, string prop, string defaultValue)
		{
			string val = adBase.GetStringProperty(entry, prop);
			return val ?? defaultValue;
		}

		private static ICollection createData(SearchResultCollection srCollection)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("cn", typeof(String)));
			dt.Columns.Add(new DataColumn("displayName", typeof(String)));
			dt.Columns.Add(new DataColumn("title", typeof(String)));
			dt.Columns.Add(new DataColumn("employeeID", typeof(String)));

			foreach (SearchResult result in srCollection)
			{
				DataRow dr = dt.NewRow();
				DirectoryEntry entry = result.GetDirectoryEntry();

				string uid = getStringAttribute(entry, "cn", "");
				dr[0] = uid;
				string name = getStringAttribute(entry, "displayName", "");
				if (name.Length > 8)
					name = name.Substring(0, 8) + "…";
				dr[1] = name;
				dr[2] = getStringAttribute(entry, "title", "");
				dr[3] = getStringAttribute(entry, "employeeID", "");
				dt.Rows.Add(dr);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		/// <summary>
		/// 그룹 목록을 순차적으로 얻는다.
		/// </summary>
		/// <returns></returns>
		public static string GetGroupHierarchy()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<root>");

			using (DirectorySearcher ds = new DirectorySearcher(getRootDE(),
					   "(&(objectClass=group)(cn=NETSALL))",
					   new string[] { "cn", "displayName" },
					   SearchScope.OneLevel))
			{
				SearchResult sr = ds.FindOne();
				using (DirectoryEntry root = sr.GetDirectoryEntry())
				{
					sb.Append(getRecursiveGroup(root));
					root.Close();
				}
			}

			sb.Append("</root>");
			return sb.ToString();
		}

		private static string getRecursiveGroup(DirectoryEntry entry)
		{
			StringBuilder sb = new StringBuilder();

			if (entry.Properties["member"].Count > 0)
			{
				foreach (string member in entry.Properties["member"])
				{
					string cn = member.Substring(0, member.IndexOf(","));

					try
					{
						using (DirectoryEntry group = getRootDE().Children.Find(cn, "group"))
						{
							if (!isInExcludes(cn))
							{
								sb.Append("<group id=\"").Append(group.Properties["cn"].Value).Append("\" name=\"");
								sb.Append(group.Properties["displayName"].Value).Append("\">");
								sb.Append(getRecursiveGroup(group));
								group.Close();
								sb.Append("</group>");
							}
						}
					}
					catch (COMException)
					{
					}
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// 그룹에 소속된 사용자 목록을 얻는다.
		/// </summary>
		/// <returns></returns>
		public static string GetUsersFromGroup(string groupCN)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<group id=\"" + groupCN + "\" name=\"");

			try
			{
				using (DirectoryEntry group = getRootDE().Children.Find("cn=" + groupCN, "group"))
				{
					sb.Append(group.Properties["displayName"].Value).Append("\">");
					PropertyValueCollection pvc = group.Properties["member"];
					group.Close();

					if (pvc.Count > 0)
					{
						SortedList list = new SortedList();
						DataView view = (DataView)getUserList();
						foreach (string member in pvc)
						{
							string cn = member.Substring(3, member.IndexOf(",") - 3);

							for (int i = 0; i < view.Count; i++)
							{
								string theCN = (string) view[i]["cn"];
								if (theCN.Equals(cn, StringComparison.CurrentCultureIgnoreCase))
								{
									string name = (string) view[i]["displayName"];
									string title = (string) view[i]["title"];
									string empID = (string) view[i]["employeeID"];

									int status = UserProfile.GetLoginStatus(theCN);

									StringBuilder innerString = new StringBuilder();
									innerString.Append("<user id=\"").Append(theCN);
									innerString.Append("\" name=\"").Append(name);
									innerString.Append("\" title=\"").Append(title);
									innerString.Append("\" employeeID=\"").Append(empID);
									innerString.Append("\" status=\"").Append(status).Append("\" />");
									list.Add(name, innerString.ToString());
									break;
								}
							}
						}
						for (int i = 0; i < list.Count; i++)
						{
							sb.Append((string)list[list.GetKey(i)]);
						}
					}
				}
			}
			catch (COMException)
			{
			}

			sb.Append("</group>");
			return sb.ToString();
		}

		/// <summary>
		/// 임시/계약직 사용자 목록을 얻는다.
		/// </summary>
		/// <returns></returns>
		public static string GetTempUsers()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<group id=\"TEMP\" name=\"");

			try
			{
				using (DirectorySearcher ds = new DirectorySearcher(adBase.BindToChild("ou=임시계약직")))
				{
					sb.Append("임시/계약직\">");

					ds.Filter = "(objectClass=user)";
					ds.SearchScope = SearchScope.OneLevel;
					ds.PropertiesToLoad.Add("cn");
					ds.PropertiesToLoad.Add("displayName");
					ds.PropertiesToLoad.Add("title");

					ds.Sort.Direction = SortDirection.Ascending;
					ds.Sort.PropertyName = "displayName";

					SearchResultCollection srCollection = ds.FindAll();
					foreach (SearchResult result in srCollection)
					{
						DirectoryEntry user = result.GetDirectoryEntry();

						string uid = getStringAttribute(user, "cn", "");
						string name = getStringAttribute(user, "displayName", "");
						string title = getStringAttribute(user, "title", "");

						int status = UserProfile.GetLoginStatus(uid);

						sb.Append("<user id=\"").Append(uid);
						sb.Append("\" name=\"").Append(name);
						sb.Append("\" title=\"").Append(title);
						sb.Append("\" status=\"").Append(status).Append("\" />");

						user.Close();
					}
				}
			}
			catch (COMException)
			{
			}

			sb.Append("</group>");
			return sb.ToString();
		}

		/// <summary>
		/// 사용자 직급+이름를 얻는다.
		/// </summary>
		/// <returns></returns>
		public static string GetUserName(string userCN)
		{
			try
			{
				using (DirectorySearcher ds = new DirectorySearcher(getRootDE()))
				{
					ds.Filter = "(&(objectClass=user)(cn=" + userCN + "))";
					ds.SearchScope = SearchScope.Subtree;
					ds.PropertiesToLoad.Add("cn");
					ds.PropertiesToLoad.Add("displayName");
					ds.PropertiesToLoad.Add("title");

					SearchResult result = ds.FindOne();
					if (result == null) return "";

					DirectoryEntry user = result.GetDirectoryEntry();
					string name = getStringAttribute(user, "displayName", "");
					string title = getStringAttribute(user, "title", "");
					user.Close();

					if (title.Trim().Length == 0) title = "사원";
					return title + " " + name;
				}
			}
			catch (COMException)
			{
				return "";
			}
		}

		/// <summary>
		/// 사용자의 패스워드를 가져온다.
		/// </summary>
		/// <param name="userCN"></param>
		/// <returns></returns>
		public static string GetUserPassword(string userCN)
		{
			try
			{
				using (DirectorySearcher ds = new DirectorySearcher(getRootDE()))
				{
					ds.Filter = "(&(objectClass=user)(cn=" + userCN + "))";
					ds.SearchScope = SearchScope.Subtree;
					ds.PropertiesToLoad.Add("cn");
					ds.PropertiesToLoad.Add("nsPWD");

					SearchResult result = ds.FindOne();
					if (result == null) return "";

					DirectoryEntry user = result.GetDirectoryEntry();
					string pwd = getStringAttribute(user, "nsPWD", "");
					user.Close();

					return pwd;
				}
			}
			catch (COMException)
			{
				return "";
			}
		}

		/// <summary>
		/// 사용자 정보를 얻는다.
		/// </summary>
		/// <returns></returns>
		public static string GetUserInfo(string userCN)
		{
			StringBuilder sb = new StringBuilder();

			try
			{
				using (DirectoryEntry user = getRootDE().Children.Find("cn=" + userCN, "user"))
				{
					string uid = getStringAttribute(user, "cn", "");
					string name = getStringAttribute(user, "displayName", "");
					string mobile = getStringAttribute(user, "mobile", "");
					string empID = getStringAttribute(user, "employeeID", "");
					string title = getStringAttribute(user, "title", "");
					string depart = getStringAttribute(user, "department", "");
					string photo = getStringAttribute(user, "personalTitle", "");
					string homePage = getStringAttribute(user, "wWWHomePage", "");

					int status = UserProfile.GetLoginStatus(userCN);

					sb.Append("<user id=\"").Append(uid);
					sb.Append("\" name=\"").Append(name);
					sb.Append("\" mobile=\"").Append(mobile);
					sb.Append("\" employeeID=\"").Append(empID);
					sb.Append("\" title=\"").Append(title);
					sb.Append("\" department=\"").Append(depart);
					sb.Append("\" status=\"").Append(status);
					sb.Append("\" photo=\"").Append((photo.Length > 0) ? "1" : "0");
					sb.Append("\" homePage=\"").Append(homePage).Append("\" />");

					user.Close();
				}
			}
			catch (COMException)
			{
			}

			return sb.ToString();
		}

		/// <summary>
		/// 임시/계약직 사용자 정보를 얻는다.
		/// </summary>
		/// <returns></returns>
		public static string GetTempUserInfo(string userCN)
		{
			StringBuilder sb = new StringBuilder();

			try
			{
				using (DirectoryEntry ou = getRootDE().Children.Find("ou=임시계약직", "organizationalUnit"))
				{
					using (DirectoryEntry user = ou.Children.Find("cn=" + userCN, "user"))
					{
						string uid = getStringAttribute(user, "cn", "");
						string name = getStringAttribute(user, "displayName", "");
						string mobile = getStringAttribute(user, "mobile", "");
						string empID = getStringAttribute(user, "employeeID", "");
						string title = getStringAttribute(user, "title", "");
						string depart = getStringAttribute(user, "department", "");
						string photo = getStringAttribute(user, "personalTitle", "");
						string homePage = getStringAttribute(user, "wWWHomePage", "");

						int status = UserProfile.GetLoginStatus(userCN);

						sb.Append("<user id=\"").Append(uid);
						sb.Append("\" name=\"").Append(name);
						sb.Append("\" mobile=\"").Append(mobile);
						sb.Append("\" employeeID=\"").Append(empID);
						sb.Append("\" title=\"").Append(title);
						sb.Append("\" department=\"").Append(depart);
						sb.Append("\" status=\"").Append(status);
						sb.Append("\" photo=\"").Append((photo.Length > 0) ? "1" : "0");
						sb.Append("\" homePage=\"").Append(homePage).Append("\" />");

						user.Close();
					}
					ou.Close();
				}
			}
			catch (COMException)
			{
			}

			return sb.ToString();
		}

		/// <summary>
		/// 사용자가 속한 그룹 목록을 조회한다.
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static NameValueCollection GetUserGroup(string userID)
		{
			NameValueCollection returns = new NameValueCollection();
			try
			{
				using (DirectoryEntry user = getRootDE().Children.Find("cn=" + userID, "user"))
				{
					if (user.Properties["memberOf"].Count > 0)
					{
						foreach (string member in user.Properties["memberOf"])
						{
							string cn = member.Substring(0, member.IndexOf(","));

							try
							{
								using (DirectoryEntry group = getRootDE().Children.Find(cn, "group"))
								{
									returns.Add(group.Properties["cn"].Value.ToString(),
										group.Properties["displayName"].Value.ToString());
									group.Close();
								}
							}
							catch (COMException)
							{
							}
						}
					}
					user.Close();
				}
			}
			catch (COMException)
			{
			}
			return returns;
		}

		/// <summary>
		/// 사용자가 속한 그룹을 조회한다.
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static string GetUserDepartment(string userID)
		{
			string returns = "";
			try
			{
				using (DirectoryEntry user = getRootDE().Children.Find("cn=" + userID, "user"))
				{
					returns = getStringAttribute(user, "department", "");
					user.Close();
				}

				bool found = false;

				NameValueCollection nvc = GetUserGroup(userID);
				if (nvc.Count > 0)
				{
					for (int i = 0; i < nvc.Count; i++)
					{
						if (nvc[i].Equals(returns))
						{
							found = true;
							break;
						}
					}

					if (!found) returns = nvc[0];
				}
				else
					returns = "";
			}
			catch (COMException)
			{
			}
			return returns;
		}

		private static bool isInExcludes(string group)
		{
			for (int i = 0; i < excludes.Length; i++)
			{
				if (group.ToUpper().IndexOf(excludes[i].ToUpper()) >= 0)
					return true;
			}
			return false;
		}
	}
}
