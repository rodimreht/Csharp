using System.Collections.Specialized;

namespace oBrowser2
{
	public class ResourceInfo
	{
		private readonly NameValueCollection resourceList = new NameValueCollection();
		private string colonyID = "";
		private string colonyName = "";
		private string fieldsDeveloped;
		private bool isInitialColony;
		private string location = "";

		/// <summary>
		/// 식민지 행성 이름
		/// </summary>
		public string ColonyName
		{
			get { return colonyName; }
			set { colonyName = value; }
		}

		/// <summary>
		/// 자원(메탈, 크리스탈, 듀테륨) 목록
		/// </summary>
		public NameValueCollection ResourceList
		{
			get { return resourceList; }
		}

		/// <summary>
		/// 식민지 행성 고유ID
		/// </summary>
		public string ColonyID
		{
			get { return colonyID; }
			set { colonyID = value; }
		}

		/// <summary>
		/// 식민지 행성 좌표
		/// </summary>
		public string Location
		{
			get { return location; }
			set { location = value; }
		}

		/// <summary>
		/// 최초 페이지 여부
		/// </summary>
		public bool IsInitialColony
		{
			get { return isInitialColony; }
			set { isInitialColony = value; }
		}

		/// <summary>
		/// 식민지 행성 필드 점유수
		/// </summary>
		public string FieldsDeveloped
		{
			get { return fieldsDeveloped; }
			set { fieldsDeveloped = value; }
		}
	}
}