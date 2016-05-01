namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Attributes.Cache]
	public class TestClass8
	{
		public static ICache Cache { get; set; }

		public static int MethodOne(int x)
		{
			return x * x;
		}

		public static MethodResult MethodTwo(string x)
		{
			return new MethodResult();
		}
	}
}