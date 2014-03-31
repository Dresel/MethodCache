namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	public class TestClass6 : TestClass1
	{
		public TestClass6(ICache cache)
			: base(cache)
		{
		}

		[Cache]
		public int MethodThree(int x)
		{
			return x * x * x;
		}
	}
}