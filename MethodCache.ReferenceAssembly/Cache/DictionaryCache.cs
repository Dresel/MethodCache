namespace MethodCache.ReferenceAssembly.Cache
{
	using System.Collections.Generic;

	public class DictionaryCache : ICache
	{
		#region Constructors and Destructors

		public DictionaryCache()
		{
			this.Storage = new Dictionary<string, object>();
		}

		#endregion

		#region Public Properties

		public int NumContainsCalls { get; private set; }

		public int NumRetrieveCalls { get; private set; }

		public int NumStoreCalls { get; private set; }

		#endregion

		#region Properties

		private Dictionary<string, object> Storage { get; set; }

		#endregion

		#region Public Methods and Operators

		public bool Contains(string key)
		{
			this.NumContainsCalls++;

			return this.Storage.ContainsKey(key);
		}

		public T Retrieve<T>(string key)
		{
			this.NumRetrieveCalls++;

			return (T)this.Storage[key];
		}

		public void Store(string key, object data)
		{
			this.NumStoreCalls++;

			this.Storage[key] = data;
		}

		#endregion
	}
}