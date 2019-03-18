using System;
using System.Deployment.Application;
using System.Windows.Forms;
using NETS_iMan.chatWebsvc;

namespace NETS_iMan
{
	internal class ErrorReport
	{
		private static readonly ChatService chatSvc;

		static ErrorReport()
		{
			chatSvc = new ChatService();
		}

		internal static void SendReport(string header, Exception ex)
		{
#if !DEBUG
			string version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

			DialogResult dr =
				MessageBoxEx.Show(
					"[" + ex.Message.Replace("\r\n", " ").Replace("\n", " ") +
					"]오류가 발생했습니다.\r\n오류 보고를 하시겠습니까?\r\n(오류 보고를 하시면 프로그램의 빠른 업데이트와 안정화에 도움이 됩니다.)",
					"오류 발생(버전: " + version + ")",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Asterisk,
					MessageBoxDefaultButton.Button1,
					30*1000);

			if (dr == DialogResult.Yes)
			{
				if (frmMain.LOGIN_INFO != null)
					chatSvc.ErrorReport(frmMain.LOGIN_INFO.LoginID, "[" + version + ":" + header + "] " + ex);
				else
					chatSvc.ErrorReport("(비로그인)", "[" + version + ":" + header + "] " + ex);
			}
#endif
		}
	}
}