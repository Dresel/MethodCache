namespace MethodCache.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.TestAssembly.Cache;

	[Cache]
	public class TestClass9
	{
		// MethodOne is static, Getter is not - ModuleWeaver should check this and should skip weaving of this method

		#region Public Properties

		public ICache Cache { get; set; }

		#endregion

		#region Public Methods and Operators

		public static int MethodOne(int x)
		{
			return x * x;
		}

		#endregion
	}
}