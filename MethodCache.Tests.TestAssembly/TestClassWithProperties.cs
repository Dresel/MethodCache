namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassWithProperties
	{
	    private int _someValue;

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

        public int AutoProperty { get; set; }

	    public int ReadWriteProperty
	    {
            get { return _someValue; }
	        set { _someValue = value; }
	    }
	}
}