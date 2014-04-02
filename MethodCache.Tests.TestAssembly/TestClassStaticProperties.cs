namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassStaticProperties
	{
		private static int someValue;

		public static ICacheWithRemove Cache { get; set; }

		public static string ReadOnlyProperty
		{
			get { return "some value"; }
		}

		// ReSharper disable once ConvertToAutoProperty
		public static int ReadWriteProperty
		{
			get { return someValue; }
			set { someValue = value; }
		}
	}
}