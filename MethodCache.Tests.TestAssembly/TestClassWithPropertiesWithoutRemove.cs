namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Attributes.Cache]
	public class TestClassWithPropertiesWithoutRemove
	{
		private string field;

		private int someValue;

		public TestClassWithPropertiesWithoutRemove(ICache cache)
		{
			Cache = cache;
		}

		public int AutoProperty { get; set; }

		public ICache Cache { get; private set; }

		[NoCache]
		public string ReadOnlyNoCache
		{
			get { return "no cache value"; }
		}

		public string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		// ReSharper disable once ConvertToAutoProperty
		public int ReadWriteProperty
		{
			get { return this.someValue; }
			set { this.someValue = value; }
		}

		public string SetOnlyProperty
		{
			set { this.field = value; }
		}
	}
}