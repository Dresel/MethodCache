namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassWithProperties
	{
	    public TestClassWithProperties(ICache cache)
	    {
	        Cache = cache;
	    }

	    public ICache Cache { get; private set; }

	    public string ReadOnlyProperty
	    {
            get { return "some value"; }
	    }

        [NoCache]
	    public string ReadOnlyNoCache
	    {
	        get { return "no cache value"; }
	    }
	}
}