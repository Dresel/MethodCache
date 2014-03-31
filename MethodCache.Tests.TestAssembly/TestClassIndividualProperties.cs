namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	public class TestClassIndividualProperties
	{
		private string _field;

		public TestClassIndividualProperties(ICacheWithRemove cache)
		{
			Cache = cache;
		}

		[Cache]
		public int AutoProperty { get; set; }

		public ICacheWithRemove Cache { get; private set; }

		[Cache]
		public string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		[Cache]
		public int ReadWriteProperty { get; set; }

		[Cache]
		public string SetOnlyProperty
		{
			set { this._field = value; }
		}
	}
}