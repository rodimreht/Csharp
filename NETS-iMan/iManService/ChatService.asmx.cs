using System;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.Timers;
using System.Collections;
using System.Web.Services;
using System.ComponentModel;
using System.Text;
using Nets.IM.Common;

namespace iManService
{
	/// <summary>
	/// ChatService의 요약 설명입니다.
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class ChatService : WebService
	{
		private static readonly ArrayList USER_LIST = ArrayList.Synchronized(new ArrayList());
		private static readonly ArrayList GROUP_LIST = ArrayList.Synchronized(new ArrayList());
		private static readonly ArrayList MESSAGE_STORE = ArrayList.Synchronized(new ArrayList());
		private static readonly ArrayList MESSAGE_GROUP_STORE = ArrayList.Synchronized(new ArrayList());
		private static readonly Timer timer = new Timer();

		static ChatService()
		{
			DataSet ds = UserProfile.GetAllLoginStatus();
			DataRowCollection rows = ds.Tables[0].Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				string id = (string)rows[i]["cn"];
				string name = ADUserInfo.GetUserName(id);
				int status = 0;
				if (!rows[i].IsNull("loginStatus"))
					status = (int)rows[i]["loginStatus"];
				string ipAddress = string.Empty;
				if (!rows[i].IsNull("loginIP"))
					ipAddress = (string)rows[i]["loginIP"];

				UserItem user = new UserItem(id, name, ipAddress, status);
				USER_LIST.Add(user);
			}

			timer.Interval = 5 * 1000;
			timer.Elapsed += timer_Elapsed;
			timer.Start();

			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "===== ChatService 초기화됨.");
		}

		private static void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				lock (USER_LIST)
				{
					for (int i = 0; i < USER_LIST.Count; i++)
					{
						UserItem user = (UserItem)USER_LIST[i];

						// 1분 이상 액션이 없으면 자동 로그아웃
						if ((user.LoginStatus > 0) && (user.LastUpdate < DateTime.Now.AddMinutes(-2)))
						{
							Logger.Log("iManService", LogLevel.INFORMATION, "ChatService", "자동 로그아웃 처리: " + user.UserID);

							user.LoginStatus = 0;
							UserProfile.Logout(user.UserID);
							removeFromGroups(user.UserID);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log("iManService", LogLevel.WARNING, "ChatService", "ERROR: timer_Elapsed(): " + ex);
			}
		}

		private static void removeFromGroups(string userID)
		{
			lock (GROUP_LIST)
			{
				for (int k = 0; k < GROUP_LIST.Count; k++)
				{
					GroupItem group = (GroupItem)GROUP_LIST[k];

					// 회의실에 해당 직원이 있는지 확인한다.
					UserItem theUser = null;
					for (int i = 0; i < group.UserList.Count; i++)
					{
						UserItem user = (UserItem)group.UserList[i];
						if (user.UserID != userID) continue;

						theUser = user;
						break;
					}

					// 해당 직원이 있으면 나머지 직원들에게 알린다.
					if (theUser != null)
					{
						for (int i = 0; i < group.UserList.Count; i++)
						{
							UserItem user = (UserItem)group.UserList[i];
							if (user.UserID != userID)
							{
								lock (MESSAGE_GROUP_STORE)
								{
									MESSAGE_GROUP_STORE.Add(user.Name + "(" + user.UserID + "):" + group.GroupID + ":Gro@up:127.0.0.1:" + theUser.Name + "(" + userID + ")님이 회의실에서 나가셨습니다.");
								}
							}
						}

						// 다 알렸으면 회의실에서 제거한다.
						group.UserList.Remove(theUser);
					}

					if (group.UserList.Count > 0) continue;

					// 회의실에 사람이 아무도 없으면 회의실을 폐쇄한다.
					GROUP_LIST.Remove(group);
					k--;
				}
			}
		}

		[WebMethod]
		public string GetAllUsers(string adminID)
		{
			string userHostAddress = Context.Request.UserHostAddress;
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "GetAllUsers: request - " + adminID + "[from " + userHostAddress + "]");

			// 관리자만 사용자 목록 조회 가능
			string test = ConfigurationManager.AppSettings["admins"];
			if (string.IsNullOrEmpty(test)) return "";

			string[] admins = test.Split(new char[] {';'});
			bool hasRights = false;
			foreach (string s in admins)
			{
				string ip = "";
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem)USER_LIST[i];
					if (user.UserID.Equals(s))
					{
						ip = user.IPAddress;
						break;
					}
				}
				if (s.Equals(adminID) && userHostAddress.Equals(ip))
				{
					hasRights = true;
					break;
				}
			}
			if (!hasRights) return "";

			// 사용자 목록
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < USER_LIST.Count; i++)
			{
				if (sb.Length > 0) sb.Append("^");

				UserItem user = (UserItem)USER_LIST[i];
				sb.Append(user.Name).Append("&").Append(user.UserID).Append("&").Append(user.IPAddress).Append("&").Append(user.LoginStatus);
			}
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "GetAllUsers: " + sb);
			return sb.ToString();
		}

		[WebMethod]
		public string GetAllGroups(string adminID)
		{
			string userHostAddress = Context.Request.UserHostAddress;
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService",
			           "GetAllGroups: request - " + adminID + "[from " + userHostAddress + "]");

			// 관리자만 회의실 목록 조회 가능
			string test = ConfigurationManager.AppSettings["admins"];
			if (string.IsNullOrEmpty(test)) return "";

			string[] admins = test.Split(new char[] {';'});
			bool hasRights = false;
			foreach (string s in admins)
			{
				string ip = "";
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem) USER_LIST[i];
					if (user.UserID.Equals(s))
					{
						ip = user.IPAddress;
						break;
					}
				}
				if (s.Equals(adminID) && userHostAddress.Equals(ip))
				{
					hasRights = true;
					break;
				}
			}
			if (!hasRights) return "";

			// 사용자 목록
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < GROUP_LIST.Count; i++)
			{
				if (sb.Length > 0) sb.Append("|");

				GroupItem group = (GroupItem)GROUP_LIST[i];
				sb.Append(group.GroupID).Append("^");

				for (int j = 0; j < group.UserList.Count; j++)
				{
					if (j > 0) sb.Append("&");

					UserItem user = (UserItem) group.UserList[j];
					sb.Append(user.Name).Append("(").Append(user.UserID).Append(")");
				}
			}

			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "GetAllGroups: " + sb);
			return sb.ToString();
		}

		[WebMethod]
		public string GetGroupUsers(string groupID)
		{
			// 사용자 목록
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < GROUP_LIST.Count; i++)
			{
				GroupItem group = (GroupItem)GROUP_LIST[i];
				if (group.GroupID == groupID)
				{
					sb.Append(group.GroupID).Append("^");

					for (int j = 0; j < group.UserList.Count; j++)
					{
						if (j > 0) sb.Append("&");

						UserItem user = (UserItem) group.UserList[j];
						sb.Append(user.Name).Append("(").Append(user.UserID).Append(")");
					}
					break;
				}
			}

			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "GetGroupUsers: " + sb);
			return sb.ToString();
		}

		[WebMethod]
		public string GetLoginUsers()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < USER_LIST.Count; i++)
			{
				UserItem user = (UserItem) USER_LIST[i];
				if (user.LoginStatus == 1) // 로그인
				{
					if (sb.Length > 0) sb.Append("|");
					sb.Append(user.UserID);
				}
				else if (user.LoginStatus == 2) // 다른 용무 중
				{
					if (sb.Length > 0) sb.Append("|");
					sb.Append("/" + user.UserID);
				}
				else if (user.LoginStatus == 9) // 자리비움
				{
					if (sb.Length > 0) sb.Append("|");
					sb.Append("*" + user.UserID);
				}
			}

			// 매분 정각에만 로그를 남긴다.
			if (DateTime.Now.Second == 0) Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "GetLoginUsers: " + sb);
			return sb.ToString();
		}

		[WebMethod]
		public void AddUser(string userID)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "AddUser: " + userID);

			string userName = ADUserInfo.GetUserName(userID);

			bool bFlag = false;
			lock (USER_LIST)
			{
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem)USER_LIST[i];
					if (user.UserID == userID)
					{
						user.Name = userName;
						user.IPAddress = Context.Request.UserHostAddress;
						user.LoginStatus = 1;
						bFlag = true;
					}
					else if (user.LoginStatus == 1)
					{
						SendMessage("Ser@ver", user.Name + "(" + user.UserID + ")", userName + "(" + userID + ")님이 로그인했습니다.");
					}
				}
				if (!bFlag)
				{
					UserItem user = new UserItem(userID, userName, Context.Request.UserHostAddress, 1);
					USER_LIST.Add(user);
				}
			}
		}

		[WebMethod]
		public void RemoveUser(string userID)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "RemoveUser: " + userID);

			bool bFlag = false;
			lock (USER_LIST)
			{
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem)USER_LIST[i];
					if (user.UserID == userID)
					{
						user.LoginStatus = 0;
						bFlag = true;
						break;
					}
				}
				if (!bFlag)
				{
					string userName = ADUserInfo.GetUserName(userID);
					UserItem user = new UserItem(userID, userName, Context.Request.UserHostAddress, 0);
					USER_LIST.Add(user);
				}
			}
		}

		[WebMethod]
		public void AbsentUser(string userID, bool isAbsent)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "AbsentUser: " + userID + " [" + isAbsent  + "]");

			bool bFlag = false;
			lock (USER_LIST)
			{
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem)USER_LIST[i];
					if (user.UserID == userID)
					{
						user.LoginStatus = isAbsent ? 9 : 1;
						bFlag = true;
						break;
					}
				}
				if (!bFlag)
				{
					string userName = ADUserInfo.GetUserName(userID);
					UserItem user = new UserItem(userID, userName, Context.Request.UserHostAddress, isAbsent ? 9 : 1);
					USER_LIST.Add(user);
				}
			}
		}

		[WebMethod]
		public void BusyUser(string userID, bool isBusy)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "BusyUser: " + userID + " [" + isBusy + "]");

			bool bFlag = false;
			lock (USER_LIST)
			{
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem)USER_LIST[i];
					if (user.UserID == userID)
					{
						user.LoginStatus = isBusy ? 2 : 1;
						bFlag = true;
						break;
					}
				}
				if (!bFlag)
				{
					string userName = ADUserInfo.GetUserName(userID);
					UserItem user = new UserItem(userID, userName, Context.Request.UserHostAddress, isBusy ? 2 : 1);
					USER_LIST.Add(user);
				}
			}
		}

		[WebMethod]
		public void SendMessage(string strFromUser, string strToUser, string strMess)
		{
			// 파일 전송 수락의 경우 원격 IP주소를 붙여서 보낸다.
			string sTemp = strMess;
			int pos = strMess.IndexOf("<file>OK");
			if (pos >= 0)
			{
				int pos2 = strMess.IndexOf(@"\par", pos);
				if (pos2 > pos) sTemp = strMess.Substring(0, pos2) + "|" + Context.Request.UserHostAddress + strMess.Substring(pos2);
			}

			lock (MESSAGE_STORE)
			{
				MESSAGE_STORE.Add(strToUser + ":" + strFromUser + ":" + Context.Request.UserHostAddress + ":" + sTemp);
				Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "SendMessage: " + strToUser + ":" + strFromUser + ":" + Context.Request.UserHostAddress + ":" + getTrimmedString(sTemp));
			}
		}

		[WebMethod]
		public string ReceiveMessage(string strUser)
		{
			// 최종 로그인 상태 갱신
			string ip;
			if (!refreshUser(strUser, out ip)) return "LOGOUT:" + ip;

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < MESSAGE_STORE.Count; i++)
			{
				string[] strTo = MESSAGE_STORE[i].ToString().Split(':');
				if ((strTo[0] == strUser) || strUser.IndexOf("(" + strTo[0] + ")") >= 0)
				{
					if (sb.Length > 0) sb.Append("\n\n\t\n");
					for (int j = 1; j < strTo.Length; j++)
					{
						if (j > 1) sb.Append(":");
						sb.Append(strTo[j]);
					}

					lock (MESSAGE_STORE)
					{
						MESSAGE_STORE.RemoveAt(i);
						i--;
					}
				}
			}

			if (sb.Length > 0) Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "ReceiveMessage-return(" + strUser + "): " + getTrimmedString(sb));
			return sb.ToString();
		}

		private bool refreshUser(string strUser, out string ip)
		{
			ip = "";
			string ipAddr = Context.Request.UserHostAddress;

			lock (USER_LIST)
			{
				for (int i = 0; i < USER_LIST.Count; i++)
				{
					UserItem user = (UserItem)USER_LIST[i];
					if ((user.UserID == strUser) || strUser.IndexOf("(" + user.UserID + ")") >= 0)
					{
						ip = user.IPAddress;
						if (!user.IPAddress.Equals(ipAddr)) return false;

						user.Refresh();

						// 로그아웃 상태이면 자동 로그인 처리
						if (user.LoginStatus == 0)
						{
							user.LoginStatus = 1;
							UserProfile.Login(user.UserID, ipAddr);
						}
						return true;
					}
				}
			}
			return false;
		}

		[WebMethod]
		public void SendOfflineMessage(string strFromUser, string strToUser, string strMess)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "SendOfflineMessage: " + strFromUser + ":" + strToUser + ":" + getTrimmedString(strMess));
			MQSender.Send(DateTime.Now, strFromUser, strToUser, strMess);
		}

		[WebMethod]
		public string ReceiveOfflineMessage(string strUser)
		{
			string ip;
			if (!refreshUser(strUser, out ip)) return "LOGOUT:" + ip;

			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "ReceiveOfflineMessage: " + strUser);
			string s = MQSender.Receive(strUser);
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "ReceiveOfflineMessage-return: " + getTrimmedString(s));
			return s;
		}

		[WebMethod]
		public string CreateGroup()
		{
			string groupID = Guid.NewGuid().ToString();
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "CreateGroup: " + groupID);

			lock (GROUP_LIST)
			{
				GroupItem group = new GroupItem(groupID);
				GROUP_LIST.Add(group);
			}
			return groupID;
		}

		[WebMethod]
		public void AddUserGroup(string userID, string groupID)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "AddUserGroup: " + userID + " - " + groupID);

			string userName = ADUserInfo.GetUserName(userID);

			bool gFlag = false;
			lock (GROUP_LIST)
			{
				for (int k = 0; k < GROUP_LIST.Count; k++)
				{
					GroupItem group = (GroupItem)GROUP_LIST[k];
					if (group.GroupID == groupID)
					{
						bool uFlag = false;
						for (int i = 0; i < group.UserList.Count; i++)
						{
							UserItem user = (UserItem)group.UserList[i];
							if (user.UserID == userID)
							{
								uFlag = true;
								break;
							}
						}

						SendGroupMessage("Gro@up", groupID, userName + "(" + userID + ")님이 회의실에 입장하셨습니다.");
						
						if (!uFlag)
						{
							UserItem user = new UserItem(userID, userName, Context.Request.UserHostAddress, 1);
							group.UserList.Add(user);
						}
						gFlag = true;
						break;
					}
				}
				if (!gFlag)
				{
					GroupItem group = new GroupItem(groupID);
					UserItem user = new UserItem(userID, userName, Context.Request.UserHostAddress, 1);
					group.UserList.Add(user);
					GROUP_LIST.Add(group);
				}
			}
		}

		[WebMethod]
		public void RemoveUserGroup(string userID, string groupID)
		{
			Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "RemoveUserGroup: " + userID + " - " + groupID);

			string userName = ADUserInfo.GetUserName(userID);

			lock (GROUP_LIST)
			{
				for (int k = 0; k < GROUP_LIST.Count; k++)
				{
					GroupItem group = (GroupItem)GROUP_LIST[k];
					if (group.GroupID == groupID)
					{
						UserItem theUser = null;
						for (int i = 0; i < group.UserList.Count; i++)
						{
							UserItem user = (UserItem)group.UserList[i];
							if (user.UserID == userID)
							{
								theUser = user;
								break;
							}
						}
						if (theUser != null) group.UserList.Remove(theUser);

						SendGroupMessage("Gro@up", groupID, userName + "(" + userID + ")님이 회의실에서 나가셨습니다.");
						if (group.UserList.Count == 0) GROUP_LIST.Remove(group);
						break;
					}
				}
			}
		}

		[WebMethod]
		public void SendGroupMessage(string strFromUser, string groupID, string strMess)
		{
			for (int k = 0; k < GROUP_LIST.Count; k++)
			{
				GroupItem group = (GroupItem)GROUP_LIST[k];
				if (group.GroupID == groupID)
				{
					for (int i = 0; i < group.UserList.Count; i++)
					{
						UserItem user = (UserItem)group.UserList[i];
						if ((user.UserID != strFromUser) && (strFromUser.IndexOf("(" + user.UserID + ")") < 0))
						{
							string strToUser = user.Name + "(" + user.UserID + ")";
							lock (MESSAGE_GROUP_STORE)
							{
								MESSAGE_GROUP_STORE.Add(strToUser + ":" + groupID + ":" + strFromUser + ":" + Context.Request.UserHostAddress + ":" + strMess);
							}
						}
					}
					Logger.Log("iManService", LogLevel.DEBUG, "ChatService",
							   "SendGroupMessage: " + groupID + ":" + strFromUser + ":" + Context.Request.UserHostAddress + ":" + getTrimmedString(strMess));
				}
			}
		}

		[WebMethod]
		public string ReceiveGroupMessage(string strUser, string groupID)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < MESSAGE_GROUP_STORE.Count; i++)
			{
				string[] strTo = MESSAGE_GROUP_STORE[i].ToString().Split(':');
				if ((groupID == strTo[1]) && ((strTo[0] == strUser) || strUser.IndexOf("(" + strTo[0] + ")") >= 0))
				{
					if (sb.Length > 0) sb.Append("\n\n\t\n");
					for (int j = 1; j < strTo.Length; j++)
					{
						if (j > 1) sb.Append(":");
						sb.Append(strTo[j]);
					}

					lock (MESSAGE_GROUP_STORE)
					{
						MESSAGE_GROUP_STORE.RemoveAt(i);
						i--;
					}
				}
			}
			if (sb.Length > 0) Logger.Log("iManService", LogLevel.DEBUG, "ChatService", "ReceiveGroupMessage-return(" + strUser + "): " + getTrimmedString(sb));
			return sb.ToString();
		}

		private static string getTrimmedString(string msg)
		{
			const int len = 200;

			if (msg.Length < len)
				return msg;

			return msg.Substring(0, len) + "...(총" + msg.Length + "문자)";
		}

		private static string getTrimmedString(StringBuilder sb)
		{
			const int len = 200;

			string msg = sb.ToString();
			if (msg.Length < len)
				return msg;

			return msg.Substring(0, len) + "...(총" + msg.Length + "문자)";
		}

		[WebMethod]
		public void ErrorReport(string userID, string errorMsg)
		{
			try
			{
				MailAddress from = new MailAddress("administrator@nets.co.kr", "계정관리자", Encoding.UTF8);

				string sendTo = ConfigurationManager.AppSettings["errorReportTo"];
				if (string.IsNullOrEmpty(sendTo)) sendTo = "thermidor@nets.co.kr";
				MailAddress to = new MailAddress(sendTo);

				MailMessage mail = new MailMessage(from, to);

				mail.Subject = "NETS-ⓘMan 오류 보고 from " + Context.Request.UserHostAddress;
				mail.SubjectEncoding = Encoding.UTF8;

				mail.Body = String.Format("<font size=2>다음 직원으로부터 오류 보고를 받았습니다:</br>" +
					"<b>ID={0}</b></br>" +
					"오류 내용: {1}</font>",
					userID,
					errorMsg.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "</br>").Replace("\n", "</br>"));
				mail.BodyEncoding = Encoding.UTF8;
				mail.IsBodyHtml = true;

				SmtpClient client = new SmtpClient("127.0.0.1");
				client.Send(mail);

			}
			catch (Exception ex)
			{
				Logger.Log("iManService", LogLevel.WARNING, "ChatService", "ERROR: ErrorReport(): " + ex);
			}
		}
	}

	public class UserItem
	{
		private string m_name;
		private string m_id;
		private string m_ip;
		private int m_status;
		private DateTime m_lastUpdate;

		public string Name
		{
			get { return m_name; }
			set
			{
				m_name = value;
				m_lastUpdate = DateTime.Now;
			}
		}

		public string UserID
		{
			get { return m_id; }
			set
			{
				m_id = value;
				m_lastUpdate = DateTime.Now;
			}
		}

		public string IPAddress
		{
			get { return m_ip; }
			set
			{
				m_ip = value;
				m_lastUpdate = DateTime.Now;
			}
		}

		public int LoginStatus
		{
			get { return m_status; }
			set
			{
				m_status = value;
				m_lastUpdate = DateTime.Now;
			}
		}

		public DateTime LastUpdate
		{
			get { return m_lastUpdate; }
		}

		public UserItem(string id, string name, string ip, int status)
		{
			m_id = id;
			m_name = name;
			m_ip = ip;
			m_status = status;
			m_lastUpdate = DateTime.Now;
		}

		public void Refresh()
		{
			m_lastUpdate = DateTime.Now;
		}
	}

	public class GroupItem
	{
		private string m_id;
		private readonly ArrayList m_userList = ArrayList.Synchronized(new ArrayList());

		public string GroupID
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public ArrayList UserList
		{
			get { return m_userList; }
		}

		public GroupItem(string id)
		{
			m_id = id;
		}
	}
}
