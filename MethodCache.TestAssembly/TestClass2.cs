namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.TestAssembly.Cache;

	public class TestClass2
	{
		#region Constructors and Destructors

		public TestClass2(ICache cache)
		{
			Cache = cache;
		}

		#endregion

		#region Public Properties

		public ICache Cache { get; set; }

		#endregion

		#region Public Methods and Operators

		[Cache]
		public int MethodOne(int x)
		{
			return x * x;
		}

		public MethodResult MethodTwo(string x)
		{
			return new MethodResult();
		}

		#endregion
	}
}