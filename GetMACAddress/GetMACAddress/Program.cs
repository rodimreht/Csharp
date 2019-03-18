using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace GetMACAddress
{
	class Program
	{
		static void Main(string[] args)
		{
			string a = getMACAddress();
			a = getMACAddressByIP("61.74.137.12");
		}

		static string getMACAddress()
		{
			ManagementClass myManagementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection moc = myManagementClass.GetInstances();

			string a = "";
			foreach (ManagementObject mo in moc)
			{
				if (mo["MacAddress"] != null)
				{
					Console.WriteLine(mo["MacAddress"].ToString());
					a = mo["MacAddress"].ToString();
				}
			}
			if (a.Length == 0) Console.WriteLine("No Mac Address Found");
			return a;
		}
		
		static string getMACAddressByIP(string ip)
		{
			try
			{
				ManagementObjectSearcher query = new ManagementObjectSearcher(@"SELECT * FROM
					Win32_NetworkAdapterConfiguration");
				ManagementObjectCollection queryCollection = query.Get();

				bool Found = false;

				foreach (ManagementObject mo in queryCollection)
				{
					if (mo["IPAddress"] != null)
					{
						string temp;
						temp = string.Join(".", (string[])mo["IPAddress"]);
						if (!temp.Equals(""))
						{
							if (!ip.Equals(""))
							{
								if (temp.Equals(ip.Trim()))
									Found = true;
							}
							else
								Found = true;
						}

						if (Found)
						{
							if (mo["macaddress"] != null)
							{
								if (!mo["macaddress"].Equals(""))
									return (string)mo["macaddress"];
							}
						}
						else
							Found = false;
					}
				}

				Console.WriteLine("No Mac Address Found");
				return "";

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return "";
			}
		}
	}
}

