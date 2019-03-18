using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace oBrowser2
{
	class IEControl
	{
		/// <summary>
		/// Internet Explorer 컨트롤 버전을 명시적으로 설정한다.
		/// </summary>
		public static void SetVersion()
		{
			try
			{
				int IEvalue = 9999; // can be: 9999, 9000, 8888, 8000, 7000
				string targetApplication = Process.GetCurrentProcess().ProcessName + ".exe";
				string parentKeyLocation = @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl";
				string keyName = "FEATURE_BROWSER_EMULATION";

				RegistryKey localMachine = Registry.LocalMachine.OpenSubKey(parentKeyLocation);
				Logger.Log(string.Format("opening up Key: {0} at {1}", keyName, parentKeyLocation));

				RegistryKey subKey = localMachine.OpenSubKey(keyName, true);
				if (subKey == null) subKey = localMachine.CreateSubKey(keyName);
				subKey.SetValue(targetApplication, IEvalue, RegistryValueKind.DWord);
			}
			catch (Exception ex)
			{
				Logger.Log("NOTE: you need to run this under no UAC: " + ex);
			}
		}
	}
}
