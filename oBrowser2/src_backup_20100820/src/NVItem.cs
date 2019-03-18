namespace oBrowser2
{
	public class NVItem
	{
		private string m_text;
		private string m_value;

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

		public override string ToString()
		{
			return Text;
		}
	}
}