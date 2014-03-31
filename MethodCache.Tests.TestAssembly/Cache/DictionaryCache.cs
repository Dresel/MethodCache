namespace MethodCache.Tests.TestAssembly.Cache
{
	using System.Collections.Generic;

	public class DictionaryCache : ICache, ICacheWithRemove
	{
		public DictionaryCache()
		{
			Storage = new Dictionary<string, object>();
		}

		public int NumRemoveCalls { get; private set; }

		public int NumRetrieveCalls { get; private set; }

		public int NumStoreCalls { get; private set; }

		private Dictionary<string, object> Storage { get; set; }

		public bool Contains(string key)
		{
			return Storage.ContainsKey(key);
		}

		public void Remove(string key)
		{
			NumRemoveCalls++;

			Storage.Remove(key);
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
	}
}