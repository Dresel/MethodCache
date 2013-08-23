namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.TestAssembly.Cache;

	[Cache]
	public class TestClass8
	{
		#region Public Properties

		public static ICache Cache { get; set; }

		#endregion

		#region Public Methods and Operators

		public static int MethodOne(int x)
		{
			return x * x;
		}

		public static MethodResult MethodTwo(string x)
		{
			return new MethodResult();
		}

		#endregion
	}
}