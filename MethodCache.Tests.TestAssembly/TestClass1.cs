namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClass1
	{
		public TestClass1(ICache cache)
		{
			Cache = cache;
		}

		public ICache Cache { get; set; }

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