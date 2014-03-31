namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache(Members.Properties)]
	public class TestClassMethodsExcluded
	{
		public TestClassMethodsExcluded(ICache cache)
		{
			Cache = cache;
		}

		public ICache Cache { get; private set; }

		public string Property
		{
			get { return "some value"; }
		}

		public int Method(int x)
		{
			return x * x;
		}

		[Cache]
		public int MethodIncluded(int x)
		{
			return x * x;
		}
	}
}