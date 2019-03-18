using System.Collections.Specialized;

namespace oBrowser2
{
	public class ExpeditionInfo
	{
		private readonly NameValueCollection fleets = new NameValueCollection();
		private int expTime = 1;
		private string planetCoords = "";
		private bool addEvent = false;
		private int speed = 10;

		/// <summary>
		/// 원정 출발 행성 좌표
		/// </summary>
		public string PlanetCoordinate
		{
			get { return planetCoords; }
			set { planetCoords = value; }
		}

		/// <summary>
		/// 원정 함대 구성
		/// </summary>
		public NameValueCollection Fleets
		{
			get { return fleets; }
		}

		/// <summary>
		/// 탐사 시간
		/// </summary>
		public int ExpeditionTime
		{
			get { return expTime; }
			set { expTime = value; }
		}

		/// <summary>
		/// 원정 귀환 이벤트 추가 여부
		/// </summary>
		/// <value><c>true</c> if [add event]; otherwise, <c>false</c>.</value>
		public bool AddEvent
		{
			get { return addEvent; }
			set { addEvent = value; }
		}

		/// <summary>
		/// 속도
		/// </summary>
		public int Speed
		{
			get { return speed; }
			set { speed = value; }
		}
	}
}