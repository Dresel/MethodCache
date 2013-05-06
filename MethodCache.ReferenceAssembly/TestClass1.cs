namespace MethodCache
{
	using MethodCache.Cache;

	[Cache]
	public class TestClass1
	{
		#region Constructors and Destructors

		public TestClass1(ICache cache)
		{
			Cache = cache;
		}

		#endregion

		#region Public Properties

		public ICache Cache { get; set; }

		#endregion

		#region Public Methods and Operators

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