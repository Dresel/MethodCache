namespace MethodCache.Tests.TestAssembly.Cache
{
	using System.Collections.Generic;

	public interface ICacheWithStoreParameters
	{
		bool Contains(string key);

		T Retrieve<T>(string key);

		void Store(string key, object data, IDictionary<string, object> parameters);
	}
}