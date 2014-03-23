namespace MethodCache.Tests.TestAssembly.Cache
{
	using System.Collections.Generic;

	public class DictionaryCache : ICache, ICacheWithRemove
	{
		public int NumRetrieveCalls { get; private set; }

        public int NumStoreCalls { get; private set; }

        public int NumRemoveCalls { get; private set; }

		private Dictionary<string, object> Storage { get; set; }

		public DictionaryCache()
		{
			Storage = new Dictionary<string, object>();
		}

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

	    public void Remove(string key)
	    {
	        NumRemoveCalls++;

	        Storage.Remove(key);
	    }
	}
}