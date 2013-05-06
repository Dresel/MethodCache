namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.TestAssembly.Cache;

	public class TestClass6 : TestClass1
	{
		#region Constructors and Destructors

		public TestClass6(ICache cache)
			: base(cache)
		{
		}

		#endregion

		#region Public Methods and Operators

		[Cache]
		public int MethodThree(int x)
		{
			return x * x * x;
		}

		#endregion
	}
}