using oBrowser2.Properties;

namespace oBrowser2
{
	/// <summary>
	/// This class is used for obtaining and storing settings
	/// </summary>
	/// <remarks>
	/// This is a single instance class, so that there is only one
	/// instance needed of the <see cref="Settings"/> class
	/// </remarks>
	public class SettingsHelper
	{
		/// <summary>
		/// Creates a new instance of the <see cref="SettingsHelper"/> class
		/// </summary>
		private SettingsHelper()
		{
			_mySettings = new Settings();
		}

		/// <summary>
		/// Stores the instance of the <see cref="Settings"/> class
		/// </summary>
		private Settings _mySettings;

		/// <summary>
		/// Stores the instance of the <see cref="SettingsHelper"/> class
		/// </summary>
		private static SettingsHelper _instance;

		/// <summary>
		/// An object for locking the thread, when needed
		/// </summary>
		private static object _lockObject = new object();

		/// <summary>
		/// Obtains the current instance of the <see cref="SettingsHelper"/> class.
		/// </summary>
		/// <remarks>
		/// If there is no instance of the <see cref="SettingsHelper"/> class, one will be created
		/// </remarks>
		public static SettingsHelper Current
		{
			get
			{
				if (_instance == null)
				{
					lock (_lockObject)
					{
						if (_instance == null)
							_instance = new SettingsHelper();
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// Gets or sets the minimal refresh rate.
		/// </summary>
		/// <value>The minimal refresh rate.</value>
		public int RefreshRate
		{
			get { return _mySettings.refreshRate; }
			set { _mySettings.refreshRate = value; }
		}

		/// <summary>
		/// Gets or sets the maximal refresh rate.
		/// </summary>
		/// <value>The maximal refresh rate.</value>
		public int RefreshRateMax
		{
			get { return _mySettings.refreshRateMax; }
			set { _mySettings.refreshRateMax = value; }
		}

		/// <summary>
		/// Gets or sets the SMS phone num.
		/// </summary>
		/// <value>The SMS phone num.</value>
		public string SMSphoneNum
		{
			get { return _mySettings.sms; }
			set { _mySettings.sms = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [apply summer time].
		/// </summary>
		/// <value><c>true</c> if [apply summer time]; otherwise, <c>false</c>.</value>
		public bool ApplySummerTime
		{
			get { return _mySettings.summerTimed; }
			set { _mySettings.summerTimed = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use FireFox].
		/// </summary>
		/// <value><c>true</c> if [use FireFox]; otherwise, <c>false</c>.</value>
		public bool UseFireFox
		{
			get { return _mySettings.useFireFox; }
			set { _mySettings.useFireFox = value; }
		}

		/// <summary>
		/// Gets or sets the FireFox directory.
		/// </summary>
		/// <value>The FireFox directory.</value>
		public string FireFoxDirectory
		{
			get { return _mySettings.FireFoxDir; }
			set { _mySettings.FireFoxDir = value; }
		}

		/// <summary>
		/// Gets or sets the user ID.
		/// </summary>
		/// <value>The user ID.</value>
		public string UserID
		{
			get { return _mySettings.userid; }
			set { _mySettings.userid = value; }
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password
		{
			get { return _mySettings.pwd; }
			set { _mySettings.pwd = value; }
		}

		/// <summary>
		/// Gets or sets the event settings.
		/// </summary>
		/// <value>The event settings.</value>
		public string EventSettings
		{
			get { return _mySettings.eventSettings; }
			set { _mySettings.eventSettings = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [show in left bottom].
		/// </summary>
		/// <value><c>true</c> if [show in left bottom]; otherwise, <c>false</c>.</value>
		public bool ShowInLeftBottom
		{
			get { return _mySettings.showInLeftBottom; }
			set { _mySettings.showInLeftBottom = value; }
		}

		/// <summary>
		/// Gets or sets the expedition.
		/// </summary>
		/// <value>The expedition.</value>
		public string Expedition
		{
			get { return _mySettings.expedition; }
			set { _mySettings.expedition = value; }
		}

		/// <summary>
		/// Gets or sets the resource collecting.
		/// </summary>
		/// <value>The resource collecting.</value>
		public string ResourceCollecting
		{
			get { return _mySettings.resCollecting; }
			set { _mySettings.resCollecting = value; }
		}

		/// <summary>
		/// Gets or sets the fleet saving.
		/// </summary>
		/// <value>The fleet saving.</value>
		public string FleetSaving
		{
			get { return _mySettings.fleetSaving; }
			set { _mySettings.fleetSaving = value; }
		}

		/// <summary>
		/// Gets or sets the attack hash.
		/// </summary>
		/// <value>The attack hash.</value>
		public string AttackHash
		{
			get { return _mySettings.attackHash; }
			set { _mySettings.attackHash = value; }
		}

		/// <summary>
		/// Gets or sets the SMTP mail.
		/// </summary>
		/// <value>The SMTP mail.</value>
		public string SmtpMail
		{
			get { return _mySettings.smtpmail; }
			set { _mySettings.smtpmail = value; }
		}

		/// <summary>
		/// Saves the <see cref="Settings"/>
		/// </summary>
		public void Save()
		{
			_mySettings.Save();
		}

	}
}
