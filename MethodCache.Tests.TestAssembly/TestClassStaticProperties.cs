namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassStaticProperties
	{
		public static ICacheWithRemove Cache { get; set; }

		public static string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		public static int ReadWriteProperty { get; set; }
	}
}