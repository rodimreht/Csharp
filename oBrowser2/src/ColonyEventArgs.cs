using System;
using System.Collections.Generic;
using System.Text;

namespace oBrowser2
{
	public class ColonyEventArgs : EventArgs
	{
		private string colonyName;

		public string ColonyName
		{
			get { return colonyName; }
			set { colonyName = value; }
		}
	}
}
