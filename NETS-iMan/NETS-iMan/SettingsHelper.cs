using NETS_iMan.Properties;

namespace NETS_iMan
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
		/// Stores the instance of the <see cref="Settings"/> class
		/// </summary>
		private readonly Settings _mySettings;

		/// <summary>
		/// Creates a new instance of the <see cref="SettingsHelper"/> class
		/// </summary>
		private SettingsHelper()
		{
			_mySettings = new Settings();
		}

		/// <summary>
		/// Stores the instance of the <see cref="SettingsHelper"/> class
		/// </summary>
		private static SettingsHelper _instance;

		/// <summary>
		/// An object for locking the thread, when needed
		/// </summary>
		private static readonly object _lockObject = new object();

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
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font.</value>
		public string Font
		{
			get { return _mySettings.font; }
			set { _mySettings.font = value; }
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public string Color
		{
			get { return _mySettings.color; }
			set { _mySettings.color = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [remember PWD].
		/// </summary>
		/// <value><c>true</c> if [remember PWD]; otherwise, <c>false</c>.</value>
		public bool RememberPwd
		{
			get { return _mySettings.remPwd; }
			set { _mySettings.remPwd = value; }
		}

		/// <summary>
		/// Gets the log path.
		/// </summary>
		/// <value>The log path.</value>
		public string LogPath
		{
			get { return _mySettings.logPath; }
			set { _mySettings.logPath = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [auto start].
		/// </summary>
		/// <value><c>true</c> if [auto start]; otherwise, <c>false</c>.</value>
		public bool AutoStart
		{
			get { return _mySettings.autoStart; }
			set { _mySettings.autoStart = value; }
		}

		/// <summary>
		/// Gets or sets the main window position.
		/// </summary>
		/// <value>The main window position.</value>
		public string MainWindowPosition
		{
			get { return _mySettings.mainWndPos; }
			set { _mySettings.mainWndPos = value; }
		}

		/// <summary>
		/// Gets or sets the size of the main window.
		/// </summary>
		/// <value>The size of the main window.</value>
		public string MainWindowSize
		{
			get { return _mySettings.mainWndSize; }
			set { _mySettings.mainWndSize = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [show online].
		/// </summary>
		/// <value><c>true</c> if [show online]; otherwise, <c>false</c>.</value>
		public bool ShowOnline
		{
			get { return _mySettings.showOnline; }
			set { _mySettings.showOnline = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [always top most].
		/// </summary>
		/// <value><c>true</c> if [always top most]; otherwise, <c>false</c>.</value>
		public bool AlwaysTopMost
		{
			get { return _mySettings.alwaysTopMost; }
			set { _mySettings.alwaysTopMost = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [off login alarm].
		/// </summary>
		/// <value><c>true</c> if [off login alarm]; otherwise, <c>false</c>.</value>
		public bool OffLoginAlarm
		{
			get { return _mySettings.offLoginAlarm; }
			set { _mySettings.offLoginAlarm = value; }
		}

		/// <summary>
		/// Gets or sets the NETSQA password.
		/// </summary>
		/// <value>The NETSQA password.</value>
		public string NETSQAPassword
		{
			get { return _mySettings.qaPwd; }
			set { _mySettings.qaPwd = value; }
		}

		/// <summary>
		/// Gets or sets the form's opacity.
		/// </summary>
		/// <value>The form's opacity.</value>
		public int FormOpacity
		{
			get { return _mySettings.formOpacity; }
			set { _mySettings.formOpacity = value; }
		}

		public int ChatOpacity
		{
			get { return _mySettings.chatOpacity; }
			set { _mySettings.chatOpacity = value; }
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
