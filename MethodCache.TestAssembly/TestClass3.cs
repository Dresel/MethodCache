namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.TestAssembly.Cache;

	[Cache]
	public class TestClass3
	{
		#region Constructors and Destructors

		public TestClass3(DictionaryCache cache)
		{
			Cache = cache;
		}

		#endregion

		#region Public Properties

		public DictionaryCache Cache { get; set; }

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