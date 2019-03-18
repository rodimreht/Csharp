namespace oBrowser2
{
	public class SmtpMailInfo
	{
		private string mailAddress;
		private string mailServer;
		private string pwd;
		private string userID;

		public string MailAddress
		{
			get { return mailAddress; }
			set { mailAddress = value; }
		}

		public string MailServer
		{
			get { return mailServer; }
			set { mailServer = value; }
		}

		public string UserID
		{
			get { return userID; }
			set { userID = value; }
		}

		public string Pwd
		{
			get { return pwd; }
			set { pwd = value; }
		}

		public static string ToString(SmtpMailInfo info)
		{
			if (info == null) return "";

			string pp;
			if (OB2Security.IsStrongKey(info.userID))
			{
				pp = "enc:" + OB2Security.Encrypt(info.UserID, info.Pwd);
			}
			else
			{
				string key = info.UserID + "12345678";
				pp = "enc:" + OB2Security.Encrypt(key, info.Pwd);
			}

			return string.Format("{0}|{1}|{2}|{3}",
			                     info.MailAddress,
			                     info.MailServer,
			                     info.UserID,
			                     pp);
		}

		public static SmtpMailInfo ParseInfo(string settings)
		{
			string[] s = settings.Split(new char[] {'|'});
			if (s.Length != 4) return null;

			SmtpMailInfo info = new SmtpMailInfo();
			info.MailAddress = s[0];
			info.MailServer = s[1];
			info.UserID = s[2];

			string pwd = s[3].Replace("enc:", "");
			if (OB2Security.IsStrongKey(s[2]))
			{
				pwd = OB2Security.Decrypt(s[2], pwd);
			}
			else
			{
				string key = s[2] + "12345678";
				pwd = OB2Security.Decrypt(key, pwd);
			}
			info.Pwd = pwd;

			return info;
		}
	}
}