using System.ServiceProcess;

namespace iManP2PService
{
	public partial class iManService : ServiceBase
	{
		private P2PMain service;

		public iManService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			service = new P2PMain();
			service.Start();
		}

		protected override void OnStop()
		{
			if (service != null)
				service.Stop();
		}
	}
}
