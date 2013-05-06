namespace MethodCache.ReferenceAssembly
{
	using MethodCache.ReferenceAssembly.Cache;

	[Cache]
	public class TestClass5
	{
		#region Public Properties

		// Wrong CacheType - ModuleWeaver should check Getter Type and should skip weaving of this class
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