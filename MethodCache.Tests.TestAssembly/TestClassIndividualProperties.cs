using MethodCache.Attributes;
using MethodCache.Tests.TestAssembly.Cache;

namespace MethodCache.Tests.TestAssembly
{
    public class TestClassIndividualProperties
    {
        public TestClassIndividualProperties(ICache cache)
	    {
	        Cache = cache;
	    }

	    public ICache Cache { get; private set; }

        [Cache]
	    public string ReadOnlyProperty
	    {
            get { return "some value"; }
	    }

        [Cache]
        public int AutoProperty { get; set; }
    }
}