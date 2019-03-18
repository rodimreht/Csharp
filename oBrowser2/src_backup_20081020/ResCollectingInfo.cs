using System;
using System.Collections.Generic;
using System.Text;

namespace oBrowser2
{
	public class ResCollectingInfo
	{
		private string planetCoords = "";
		private int planetType;
		private int moveType;
		private int speed;
		private string[] fleets;
		private int remainDeuterium;
		private bool addEvent;

		/// <summary>
		/// 도착 행성 좌표
		/// </summary>
		public string PlanetCoordinate
		{
			get { return planetCoords; }
			set { planetCoords = value; }
		}

		/// <summary>
		/// 도착 행성 유형
		/// </summary>
		public int PlanetType
		{
			get { return planetType; }
			set { planetType = value; }
		}

		/// <summary>
		/// 자원 운송 유형
		/// </summary>
		public int MoveType
		{
			get { return moveType; }
			set { moveType = value; }
		}

		/// <summary>
		/// 비행 속도
		/// </summary>
		public int Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		/// <summary>
		/// 함대 구성
		/// </summary>
		public string[] Fleets
		{
			get { return fleets; }
			set { fleets = value; }
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
		/// 자원 운송 완료 이벤트 추가 여부
		/// </summary>
		public bool AddEvent
		{
			get { return addEvent; }
			set { addEvent = value; }
		}
	}
}
