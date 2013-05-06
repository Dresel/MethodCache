namespace MethodCache.TestAssembly.Cache
{
	using System.Collections.Generic;

	public class DictionaryCache : ICache
	{
		#region Constructors and Destructors

		public DictionaryCache()
		{
			Storage = new Dictionary<string, object>();
		}

		#endregion

		#region Public Properties

		public int NumRetrieveCalls { get; private set; }

		public int NumStoreCalls { get; private set; }

		#endregion

		#region Properties

		private Dictionary<string, object> Storage { get; set; }

		#endregion

		#region Public Methods and Operators

		public bool Contains(string key)
		{
			return Storage.ContainsKey(key);
		}

		public T Retrieve<T>(string key)
		{
			NumRetrieveCalls++;

			return (T)Storage[key];
		}

		public void Store(string key, object data)
		{
			NumStoreCalls++;

			Storage[key] = data;
		}

		#endregion
	}
}