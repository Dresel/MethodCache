namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Attributes.Cache(Members.Methods)]
	public class TestClassPropertiesExcluded
	{
		public TestClassPropertiesExcluded(ICache cache)
		{
			Cache = cache;
		}

		public ICache Cache { get; private set; }

		[Attributes.Cache]
		public string ExplicitlyCachedProperty
		{
			get { return "some value"; }
		}

		public string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		public int Method(int x)
		{
			return x * x;
		}
	}
}