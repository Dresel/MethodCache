namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClass3
	{
		public TestClass3(DictionaryCache cache)
		{
			Cache = cache;
		}

		public DictionaryCache Cache { get; set; }

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