namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClass3
	{
		public DictionaryCache Cache { get; set; }

		public TestClass3(DictionaryCache cache)
		{
			Cache = cache;
		}

		public int MethodOne(int x)
		{
			return x * x;
		}

		public MethodResult MethodTwo(string x)
		{
			return new MethodResult();
		}
	}
}