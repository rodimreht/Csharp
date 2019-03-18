using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace iManP2PService
{
	[RunInstaller(true)]
	public partial class iManInstaller : Installer
	{
		private readonly ServiceProcessInstaller processInstaller;
		private readonly ServiceInstallerEx iManServiceInstaller;

		public iManInstaller()
		{
			InitializeComponent();

			processInstaller = new ServiceProcessInstaller();
			processInstaller.Account = ServiceAccount.LocalSystem;
			processInstaller.Password = null;
			processInstaller.Username = null;

			iManServiceInstaller = new ServiceInstallerEx();
			iManServiceInstaller.Description = "NETS-iMan P2P 파일 전송 지원 서비스 프로그램 입니다.";
			iManServiceInstaller.DisplayName = "NETSiMan P2P Service";
			iManServiceInstaller.ServiceName = "NETSIMAN_P2PSVC";
			iManServiceInstaller.StartType = ServiceStartMode.Automatic;

			// 확장 속성: 하루에 한번씩 실패횟수 초기화
			iManServiceInstaller.FailCountResetTime = 60 * 60 * 24;

			// 확장 속성: 두 번 실패 시만 서비스 재시작
			iManServiceInstaller.FailureActions.Add(new FailureAction(RecoverAction.Restart, 120000));
			iManServiceInstaller.FailureActions.Add(new FailureAction(RecoverAction.Restart, 120000));
			iManServiceInstaller.FailureActions.Add(new FailureAction(RecoverAction.None, 0));

			// 확장 속성: 서비스 설치 후 즉시 시작
			iManServiceInstaller.StartOnInstall = true;

			Installers.AddRange(new Installer[]
			                    	{
			                    		processInstaller,
			                    		iManServiceInstaller
			                    	});
		}
	}
}