using System;

namespace HeroTCPRelay
{
	public class ConsoleLogDriver : LogDriver
	{
		public override void Log(string channelName, LogLevel lvl, string systemID, DateTime time, string msg)
		{
			if (CheckLevel(lvl))
				Console.WriteLine(FormatMessage(lvl, systemID, time, msg));
		}

		public override void Close()
		{
			return;
		}
	}
}
