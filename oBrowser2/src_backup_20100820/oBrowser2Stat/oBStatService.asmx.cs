using System.ComponentModel;
using System.Web.Services;

namespace oBrowser2Stat
{
	/// <summary>
	/// Service1의 요약 설명입니다.
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class oBStatService : WebService
	{
		static oBStatService()
		{
			Logger.Log("oBStatService 클래스 초기화 됨.");
		}

		[WebMethod]
		public void StatLog(string uni, string id, string msg)
		{
			string client = Context.Request.UserHostAddress;
			Logger.Log("[" + client + "] 우주: " + uni + ", 사용자: " + id + ", [" + msg + "]");
		}
	}
}