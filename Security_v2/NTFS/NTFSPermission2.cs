using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace NTFS
{
	class NTFSPermission2
	{
		private const FileSystemRights USER_RIGHTS = FileSystemRights.Read | FileSystemRights.Write |
									  FileSystemRights.Delete | FileSystemRights.ExecuteFile;
		private const FileSystemRights READ_RIGHTS = FileSystemRights.Read;
		private const FileSystemRights ALL_RIGHTS = FileSystemRights.FullControl;
		private const InheritanceFlags INHERIT = InheritanceFlags.ContainerInherit;
		private const PropagationFlags PROPAGATE = PropagationFlags.InheritOnly;
		private const AccessControlType CONTROL_TYPE = AccessControlType.Allow;

		/// <summary>
		/// NTFS 접근 권한을 설정한다.
		/// </summary>
		/// <param name="sDirPath">물리적인 실제 경로</param>
		/// <param name="sServerName">서버명</param>
		/// <param name="sDomain">도메인명</param>
		/// <param name="sUser">계정 ID</param>
		/// <param name="isLocal">로컬인지 여부</param>
		/// <returns>성공하면 true</returns>
		public bool SetPermission(String sDirPath, String sServerName, String sDomain, String sUser, bool isLocal)
		{
			try
			{
				if (!isLocal)
					sDirPath = "\\\\" + sServerName + "\\" + sDirPath.Replace(":", "$");

				DirectoryInfo dir = new DirectoryInfo(sDirPath);
				DirectorySecurity dSec = dir.GetAccessControl();

				deleteEveryoneACE(dSec);

				string domainUser;
				if (sServerName.ToLower() != sDomain.ToLower())
					domainUser = sDomain + "\\" + sUser;
				else
					domainUser = sUser;

				FileSystemAccessRule rule = new FileSystemAccessRule(domainUser, USER_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
				dSec.AddAccessRule(rule);

				string computerName = Environment.MachineName;
				rule = new FileSystemAccessRule("IUSR_" + computerName, READ_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
				dSec.AddAccessRule(rule);

				if (sServerName.ToLower() != sDomain.ToLower())
				{
					domainUser = sDomain + "\\Administrator";
					rule = new FileSystemAccessRule(domainUser, ALL_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
					dSec.AddAccessRule(rule);
				}
				rule = new FileSystemAccessRule("Administrator", ALL_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
				dSec.AddAccessRule(rule);

				rule = new FileSystemAccessRule("System", ALL_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
				dSec.AddAccessRule(rule);

				rule = new FileSystemAccessRule("ASPNET", ALL_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
				dSec.AddAccessRule(rule);

				rule = new FileSystemAccessRule("Network Service", ALL_RIGHTS, INHERIT, PROPAGATE, CONTROL_TYPE);
				dSec.AddAccessRule(rule);
				
				dir.SetAccessControl(dSec);
				
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// 기본적으로 허용된 everyone 계정 접근권한(상속)을 제거한다.
		/// </summary>
		/// <param name="dSec">디렉터리 보안개체</param>
		private void deleteEveryoneACE(DirectorySecurity dSec)
		{
			NTAccount everyone = new NTAccount("Everyone");

			AuthorizationRuleCollection collection = dSec.GetAccessRules(true, true, typeof(NTAccount));
			foreach (FileSystemAccessRule rule in collection)
			{
				if (rule.IdentityReference.Value.Equals(everyone.Value))
					dSec.RemoveAccessRule(rule);
			}
		}
	}
}
