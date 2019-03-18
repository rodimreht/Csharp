using System.Collections.Specialized;

namespace oBrowser2
{
	internal class FleetMoveInfo
	{
		private string planetCoords = "";
		private string targetCoords = "";
		private int targetType;
		private int moveType;
		private int speed;
		private readonly NameValueCollection fleets = new NameValueCollection();
		private bool moveResource;
		private int remainDeuterium;
		private bool addEvent;

		/// <summary>
		/// 도착 행성 좌표
		/// </summary>
		/// <value>The target coords.</value>
		public string TargetCoords
		{
			get { return targetCoords; }
			set { targetCoords = value; }
		}

		/// <summary>
		/// 도착 행성 유형
		/// </summary>
		/// <value>The type of the target.</value>
		public int TargetType
		{
			get { return targetType; }
			set { targetType = value; }
		}

		/// <summary>
		/// 출발 행성 좌표
		/// </summary>
		public string PlanetCoordinate
		{
			get { return planetCoords; }
			set { planetCoords = value; }
		}

		/// <summary>
		/// 함대 이동 유형
		/// </summary>
		public int MoveType
		{
			get { return moveType; }
			set { moveType = value; }
		}

		/// <summary>
		/// 속도
		/// </summary>
		public int Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		/// <summary>
		/// 함대 구성
		/// </summary>
		public NameValueCollection Fleets
		{
			get { return fleets; }
		}

		/// <summary>
		/// 행성에 있는 모든 자원을 싣고 갈 것인지 여부
		/// </summary>
		/// <value><c>true</c> if [move resource]; otherwise, <c>false</c>.</value>
		public bool MoveResource
		{
			get { return moveResource; }
			set { moveResource = value; }
		}

		/// <summary>
		/// 남길 듀테륨 량
		/// </summary>
		public int RemainDeuterium
		{
			get { return remainDeuterium; }
			set { remainDeuterium = value; }
		}

		/// <summary>
		/// 함대 이동 완료 이벤트 추가 여부
		/// </summary>
		public bool AddEvent
		{
			get { return addEvent; }
			set { addEvent = value; }
		}
	}
}