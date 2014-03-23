namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassStaticProperties
	{
	    private static int _someValue;

	    public static ICache Cache { get; set; }

	    public static string ReadOnlyProperty
	    {
            get { return "some value"; }
	    }

	    public static int ReadWriteProperty
	    {
            get { return _someValue; }
	        set { _someValue = value; }
	    }
	}
}