using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace GetSid
{
	/// <summary>
	/// This class provides two metods to convert object name (user, group) to SID and vice-versa.
	/// </summary>
	public class SidTranslator
	{

		[DllImport( "advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		private static extern bool LookupAccountSid([In,MarshalAs(UnmanagedType.LPTStr)] string systemName, IntPtr sid, [Out,MarshalAs(UnmanagedType.LPTStr)] StringBuilder name, ref int cbName, StringBuilder referencedDomainName, ref int cbReferencedDomainName, out int use );

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern bool LookupAccountName([In,MarshalAs(UnmanagedType.LPTStr)] string systemName, [In,MarshalAs(UnmanagedType.LPTStr)] string accountName, IntPtr sid, ref int cbSid, StringBuilder referencedDomainName, ref int cbReferencedDomainName, out int use);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool ConvertSidToStringSid(IntPtr sid, [In,Out,MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool ConvertStringSidToSid([In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid, ref IntPtr sid);

		/// <summary>
		/// The method converts object name (user, group) into SID string.
		/// </summary>
		/// <param name="name">Object name in form domain\object_name.</param>
		/// <returns>SID string.</returns>
		public static string GetSid(string name)
		{
			IntPtr _sid = IntPtr.Zero;	//pointer to binary form of SID string.
			int _sidLength = 0;			//size of SID buffer.
			int _domainLength = 0;		//size of domain name buffer.
			int _use;					//type of object.
			StringBuilder _domain = new StringBuilder(0);	//stringBuilder for domain name.
			int _error = 0;
			string _sidString = "";

			//first call of the function only returns the sizes of buffers (SDI, domain name)
			LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);
			_error = Marshal.GetLastWin32Error();

			if (_error != 122) //error 122 (The data area passed to a system call is too small) - normal behaviour.
			{
				throw(new Exception(new Win32Exception(_error).Message));
			}
			else
			{
				_domain = new StringBuilder(_domainLength); //allocates memory for domain name
				_sid = Marshal.AllocHGlobal(_sidLength);	//allocates memory for SID
				bool _rc = LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);

				if (_rc == false)
				{
					_error = Marshal.GetLastWin32Error();
					Marshal.FreeHGlobal(_sid);
					throw(new Exception(new Win32Exception(_error).Message));
				}
				else
				{
					// converts binary SID into string
					_rc = ConvertSidToStringSid(_sid, ref _sidString);

					if (_rc == false)
					{
						_error = Marshal.GetLastWin32Error();
						Marshal.FreeHGlobal(_sid);
						throw(new Exception(new Win32Exception(_error).Message));
					}
					else
					{
						Marshal.FreeHGlobal(_sid);
						return _sidString;
					}
				}
			}

		}

		/// <summary>
		/// The method converts SID string (user, group) into object name.
		/// </summary>
		/// <param name="sid">SID string.</param>
		/// <returns>Object name in form domain\object_name.</returns>
		public static string GetName(string sid)
		{
			IntPtr _sid = IntPtr.Zero;	//pointer to binary form of SID string.
			int _nameLength = 0;		//size of object name buffer
			int _domainLength = 0;		//size of domain name buffer
			int _use;					//type of object
			StringBuilder _domain = new StringBuilder();	//domain name variable
			int _error = 0;
			StringBuilder _name = new StringBuilder();		//object name variable

			bool _rc0 = ConvertStringSidToSid(sid, ref _sid);	//converts SID string into the binary form

			if (_rc0 == false)
			{
				_error = Marshal.GetLastWin32Error();
				Marshal.FreeHGlobal(_sid);
				throw(new Exception(new Win32Exception(_error).Message));
			}

			//first call of method returns the size of domain name and object name buffers
			bool _rc = LookupAccountSid(null, _sid, _name, ref _nameLength, _domain, ref _domainLength, out _use);
			_domain = new StringBuilder(_domainLength);	//allocates memory for domain name
			_name = new StringBuilder(_nameLength);		//allocates memory for object name
			_rc = LookupAccountSid(null, _sid,  _name, ref _nameLength, _domain, ref _domainLength, out _use);

			if (_rc == false)
			{
				_error = Marshal.GetLastWin32Error();
				Marshal.FreeHGlobal(_sid);
				throw(new Exception(new Win32Exception(_error).Message));
			}
			else
			{
				Marshal.FreeHGlobal(_sid);
				return _domain.ToString() + "\\" + _name.ToString();
			}
		}
	}
}
