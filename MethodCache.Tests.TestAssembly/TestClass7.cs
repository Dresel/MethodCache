namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClass7
	{
		public TestClass7(ICache cache)
		{
			Cache = cache;
		}

		public ICache Cache { get; set; }

		public int MethodOne(int x)
		{
			return x * x;
		}

		[NoCache]
		public MethodResult MethodTwo(string x)
		{
			return new MethodResult();
		}
	}
}