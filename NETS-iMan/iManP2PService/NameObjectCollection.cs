using System;
using System.Collections.Specialized;

namespace iManP2PService
{
	/// <summary>
	/// 
	/// </summary>
	public class NameObjectCollection : NameObjectCollectionBase
	{
		/// <summary>
		/// 
		/// </summary>
		public object this[string name]
		{
			get { return base.BaseGet( name ); }
			set { base.BaseSet( name, value ); }
		}

		public void Add(NameObjectCollection noc)
		{
			for (int i = 0; i < noc.Count; i++)
				base.BaseAdd(noc.BaseGetKey(i), noc.BaseGet(i));
		}

		public void Add(string name, object value)
		{
			base.BaseAdd(name, value);
		}

		public string GetKey(int index)
		{
			return base.BaseGetKey(index);
		}

		public object Get(int index)
		{
			return base.BaseGet(index);
		}

		public object Get(string name)
		{
			return base.BaseGet(name);
		}

		public bool HasKeys()
		{
			return base.BaseHasKeys();
		}

		public bool ContainsKey(string name)
		{
			if (this[name] != null) return true;
			return false;
		}

		public void Clear()
		{
			base.BaseClear();
		}

		public string[] GetAllKeys()
		{
			return base.BaseGetAllKeys();
		}

		public object[] GetAllValues()
		{
			Array arr = base.BaseGetAllValues(typeof(object));
			return (object[])arr;
		}

		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}
	}
}