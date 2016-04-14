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
			ReadOnlyAutoProperty = "some value";
		}

		public int AutoProperty { get; set; }

		public ICacheWithRemove Cache { get; private set; }

		[NoCache]
		public string ReadOnlyNoCache
		{
			get { return "no cache value"; }
		}

		public string ReadOnlyAutoProperty { get; }

		public string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		// ReSharper disable once ConvertToAutoProperty
		public int ReadWriteProperty
		{
			get { return someValue; }
			set { someValue = value; }
		}

		public string SetOnlyProperty
		{
			set { field = value; }
		}
	}
}