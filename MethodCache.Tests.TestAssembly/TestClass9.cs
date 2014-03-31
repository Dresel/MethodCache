namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClass9
	{
		// MethodOne is static, Getter is not - ModuleWeaver should check this and should skip weaving of this method
		public ICache Cache { get; set; }

		public static int MethodOne(int x)
		{
			return x * x;
		}
	}
}