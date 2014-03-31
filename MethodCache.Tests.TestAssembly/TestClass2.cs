namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	public class TestClass2
	{
		public TestClass2(ICache cache)
		{
			Cache = cache;
		}

		public ICache Cache { get; set; }

		[Cache]
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