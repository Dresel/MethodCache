namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassWithProperties
	{
		private string field;

		private int someValue;

		public TestClassWithProperties(ICacheWithRemove cache)
		{
			Cache = cache;
		}

		public int AutoProperty { get; set; }

		public ICacheWithRemove Cache { get; private set; }

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