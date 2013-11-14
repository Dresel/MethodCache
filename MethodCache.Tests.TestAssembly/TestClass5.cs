namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;

	[Cache]
	public class TestClass5
	{
		// Wrong CacheType - ModuleWeaver should check Getter Type and should skip weaving of this class

		public int Cache { get; set; }

		public int MethodOne(int x)
		{
			return x * x;
		}
	}
}