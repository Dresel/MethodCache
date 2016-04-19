namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	public class TestClassIndividualProperties
	{
		private string field;

		private int someValue;

		public TestClassIndividualProperties(ICacheWithRemove cache)
		{
			Cache = cache;
		}

		[Attributes.Cache]
		public int AutoProperty { get; set; }

		public ICacheWithRemove Cache { get; private set; }

		[Attributes.Cache]
		public string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		[Attributes.Cache]

		// ReSharper disable once ConvertToAutoProperty
		public int ReadWriteProperty
		{
			get { return this.someValue; }
			set { this.someValue = value; }
		}

		[Attributes.Cache]
		public string SetOnlyProperty
		{
			set { this.field = value; }
		}
	}
}