using MethodCache.Attributes;
using MethodCache.Tests.TestAssembly.Cache;

namespace MethodCache.Tests.TestAssembly
{
    public class TestClassIndividualProperties
    {
        private int _someValue;
        private string _field;

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

        [Cache]
        public int ReadWriteProperty
        {
            get { return _someValue; }
            set { _someValue = value; }
        }

        [Cache]
        public string SetOnlyProperty
        {
            set { _field = value; }
        }
    }
}