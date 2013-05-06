namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;

	[Cache]
	public class TestClass5
	{
		// Wrong CacheType - ModuleWeaver should check Getter Type and should skip weaving of this class

		#region Public Properties

		public int Cache { get; set; }

		#endregion

		#region Public Methods and Operators

		public int MethodOne(int x)
		{
			return x * x;
		}

		#endregion
	}
}