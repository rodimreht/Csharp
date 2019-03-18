using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.AccessControl;

namespace NTFS
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = "D:\\Temp\\NTFSTemp";
			string user = "everyone";
			
			NTFSPermission perm = new NTFSPermission();
			perm.SetPermission(path, "localhost", "localhost", user, true);
			
			NTFSPermission2 perm2 = new NTFSPermission2();
			perm2.SetPermission(path, "localhost", "localhost", user, true);
		}
	}
}
