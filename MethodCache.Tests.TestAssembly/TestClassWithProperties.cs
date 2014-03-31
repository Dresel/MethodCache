namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassWithProperties
	{
		private string _field;

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

		public int ReadWriteProperty { get; set; }

		public string SetOnlyProperty
		{
			set { this._field = value; }
		}
	}
}