namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;

	[Cache]
	public class TestClass4
	{
		// No CacheGetter - ModuleWeaver should check for Cache Getter and should skip weaving of this class

		public int MethodOne(int x)
		{
			return x * x;
		}
	}
}