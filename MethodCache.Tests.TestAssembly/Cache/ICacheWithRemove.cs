namespace MethodCache.Tests.TestAssembly.Cache
{
	public interface ICacheWithRemove
	{
		bool Contains(string key);

		void Remove(string key);

		T Retrieve<T>(string key);

		void Store(string key, object data);
	}
}