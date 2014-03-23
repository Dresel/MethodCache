namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;

	[Cache]
	public class TestClassWithProperties
	{
	    private int _someValue;
	    private string _field;

        public TestClassWithProperties(ICacheWithRemove cache)
	    {
	        Cache = cache;
	    }

	    public ICacheWithRemove Cache { get; private set; }

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

	    public string SetOnlyProperty
	    {
	        set { _field = value; }
	    }
	}
}