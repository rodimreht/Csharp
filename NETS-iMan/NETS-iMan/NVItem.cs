namespace NETS_iMan
{
	public class NVItem
	{
		private string m_text;
		private string m_value;
		private string m_info;

		public NVItem(string text, string value)
		{
			Text = text;
			Value = value;
		}

		public string Text
		{
			get { return m_text; }
			set { m_text = value; }
		}

		public string Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		public string Info
		{
			get { return m_info; }
			set { m_info = value; }
		}

		public override string ToString()
		{
			return Text;
		}
	}
}