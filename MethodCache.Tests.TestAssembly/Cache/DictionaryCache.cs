namespace MethodCache.Tests.TestAssembly.Cache
{
	using System.Collections.Generic;

	public class DictionaryCache : ICache, ICacheWithRemove, ICacheWithStoreParameters
	{
		public DictionaryCache()
		{
			Storage = new Dictionary<string, object>();
		}

		public int NumRemoveCalls { get; private set; }

		public int NumRetrieveCalls { get; private set; }

		public int NumStoreCalls { get; private set; }

		public int NumStoreParameterCalls { get; private set; }

		public IDictionary<string, object> ParametersPassedToLastStoreCall { get; set; }

		private Dictionary<string, object> Storage { get; }

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

		void ICacheWithStoreParameters.Store(string key, object data, IDictionary<string, object> parameters)
		{
			NumStoreParameterCalls++;

			ParametersPassedToLastStoreCall = new Dictionary<string, object>(parameters);

			Storage[key] = data;
		}
	}
}