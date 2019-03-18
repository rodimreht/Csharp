using System.Runtime.InteropServices;

namespace SNMPTrapSender
{
	public static class TrapTrigger
	{
		private const int EVENT_ALL_ACCESS = 0x1F0003;
		private const int EVENT_MODIFY_STATE = 0x0002;

		[DllImport("kernel32.dll")]
		private static extern int OpenEvent(int dwDesiredAccess, int bInheritHandle, string lpName);

		[DllImport("kernel32.dll")]
		private static extern int SetEvent(int hEvent);

		/// <summary>
		/// SNMP Trap 이벤트 발생
		/// </summary>
		/// <param name="eventName"></param>
		public static void SetEvent(string eventName)
		{
			int hTrapEvent = OpenEvent(EVENT_MODIFY_STATE, 0, eventName);
			if (hTrapEvent == 0) return; // 이벤트 열기 실패

			SetEvent(hTrapEvent);
		}
	}
}
