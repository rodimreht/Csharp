using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace oBrowser2
{
	public class SendSMTP
	{
		public static void Send(string uniName, string hash, string msg)
		{
			SmtpMailInfo info = SmtpMailInfo.ParseInfo(SettingsHelper.Current.SmtpMail);
			if (info == null) return;

			string pEmail = info.MailAddress;
			string pServer = info.MailServer;
			string pUser = info.UserID;
			string pPwd = info.Pwd;

			if (pEmail.Trim().Length == 0) return;

			MailMessage mail = new MailMessage();

			//보낸사람
			mail.From = new MailAddress("OBrowser2<" + pEmail + ">");
			//받는사람
			mail.To.Add(new MailAddress(pEmail));

			//제목
			mail.Subject = "경고: 오게임[" + uniName + "] 공격함대 발견";
			mail.SubjectEncoding = Encoding.UTF8;

			//내용
			mail.Body =
				@"<html>
	<head>
		<basefont size='3' color='red' />
	</head>
	<body bgcolor='yellow'>
		<b><font size='5'>
		당신의 행성으로 공격해오는 적의 함대가 발견되었습니다!<br/>
		즉시 적절한 조치를 취하시기 바랍니다.
		</font></b><br/><br/>
		- 메시지 내용: <br/><div>&nbsp;&nbsp;&nbsp;<span style='color: #cccccc; font-size: 10pt; background-color: black; height: 100px; padding: 5px; vertical-align: middle'>...... " +
				msg.Replace("<", "&lt;").Replace(">", "&gt;") + @" ......</span></div><br/>
		- 메시지 해시값: " + hash +
				@"<br/>
		- 공격함대 발견 시각: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"<br/>
	</body>
</html>";
			mail.BodyEncoding = Encoding.UTF8;
			mail.IsBodyHtml = true;

			//우선순위
			mail.Priority = MailPriority.High;

			SmtpClient sc = new SmtpClient(pServer);
			sc.Credentials = new NetworkCredential(pUser, pPwd);

			try
			{
				//발송
				sc.Send(mail);
			}
			catch (Exception ex)
			{
				Logger.Log("메일 보내기 오류: " + ex);
			}
		}
	}
}
