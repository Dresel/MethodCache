namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Attributes.Cache]
	public class TestClassGeneric<TClass>
	{
		private TClass property;

		public TestClassGeneric(ICacheWithRemove cache)
		{
			Cache = cache;
		}

		public static ICacheWithRemove Cache { get; private set; }

		// ReSharper disable once ConvertToAutoProperty
		public TClass Property
		{
			get { return this.property; }
			set { this.property = value; }
		}

		public TMethod Method1<TMethod>(TMethod parameter)
		{
			return parameter;
		}

		public TClass Method2(TClass parameter)
		{
			return parameter;
		}
	}
}