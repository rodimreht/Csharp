using System.ComponentModel;
using System.Web.Services;
using Nets.IM.Common;

namespace iManService
{
	/// <summary>
	/// Service1의 요약 설명입니다.
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class DirService : WebService
	{
		[WebMethod]
		public string GetGroupTree()
		{
			return ADUserInfo.GetGroupHierarchy();
		}

		[WebMethod]
		public string GetUsers(string groupID)
		{
			return ADUserInfo.GetUsersFromGroup(groupID);
		}

		[WebMethod]
		public string GetUserDepartment(string userID)
		{
			return ADUserInfo.GetUserDepartment(userID);
		}

		[WebMethod]
		public string GetUserInfo(string userID)
		{
			return ADUserInfo.GetUserInfo(userID);
		}

		[WebMethod]
		public string GetTempUsers()
		{
			return ADUserInfo.GetTempUsers();
		}

		[WebMethod]
		public string GetTempUserInfo(string userID)
		{
			return ADUserInfo.GetTempUserInfo(userID);
		}

		[WebMethod]
		public bool Login(string userID)
		{
			return UserProfile.Login(userID, Context.Request.UserHostAddress);
		}

		[WebMethod]
		public bool Logout(string userID)
		{
			return UserProfile.Logout(userID);
		}

		[WebMethod]
		public bool Absent(string userID)
		{
			return UserProfile.Absent(userID);
		}

		[WebMethod]
		public bool Busy(string userID)
		{
			return UserProfile.Busy(userID);
		}

		[WebMethod]
		public bool UserLogin(string userID, string password)
		{
			try
			{
				// 사용자 암호 가져오기
				string realPassword = ADUserInfo.GetUserPassword(userID);
				if (string.IsNullOrEmpty(realPassword))
				{
					// 해당 사용자가 없음				
					return false;
				}

				// 암호화된 사용자 암호를 비교하기 위하여 복호화한다.
				// 암호화 모듈 업그레이드로 기존 암호화 문자열과 새 암호화 문자열이 달라짐.
				// --> 암호화해서 비교하면 다를 수 있음; 복호화하여 비교한다.
				string orgPwd;
				if (password.StartsWith("enc:"))
				{
					if (NISecurity.IsStrongKey(userID))
					{
						string pwd = password.Replace("enc:", "");
						orgPwd = NISecurity.Decrypt(userID, pwd);
					}
					else
					{
						string key = userID + "12345678";
						string pwd = password.Replace("enc:", "");
						orgPwd = NISecurity.Decrypt(key, pwd);
					}
				}
				else
					orgPwd = password;

				if (orgPwd != Security.GetInstance("password").Decrypt(realPassword))
				{
					// 패스워드가 틀림
					return false;
				}
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				// 사용자가 없는 경우 COMException 발생
				return false;
			}
			return true;
		}
	}
}