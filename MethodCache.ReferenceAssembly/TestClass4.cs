namespace MethodCache.ReferenceAssembly
{
	using MethodCache.ReferenceAssembly.Cache;

	[Cache]
	public class TestClass4
	{
		// No CacheGetter - ModuleWeaver should check for Cache Getter and should skip weaving of this class

		#region Public Methods and Operators

		public int MethodOne(int x)
		{
			return x * x;
		}

		#endregion
	}
}