namespace HeroTCPRelay
{
	/// <summary>
	/// 로깅시 필요한 레벨 (copy from NETS*IM 2.6)
	/// </summary>
	public enum LogLevel
	{
		/// <summary>로깅 레벨 없음</summary>
		NOTSET,
		/// <summary>디버그용 로그</summary>
		DEBUG,
		/// <summary>단순 정보 로그</summary>
		INFORMATION,
		/// <summary>시스템 작동에 지장은 없으나 에러상황 발생 로그</summary>
		WARNING,
		/// <summary>시스템 작동에 치명적인 오류 로그</summary>
		ERROR
	}
}
