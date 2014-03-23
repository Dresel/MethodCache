namespace MethodCache.Tests
{
    using System.Reflection;
    using System.Xml.Linq;
    using NUnit.Framework;

    [TestFixture]
    public abstract class ModuleWeaverTestsBase
    {
        private Assembly _assembly;

        protected Assembly Assembly
        {
            get { return _assembly; }
        }

        [SetUp]
        public void ClassInitialize()
        {
            if (_assembly == null)
            {
                _assembly = WeaverHelper.WeaveAssembly(GetType().Name, WeaverConfig);
            }
        }

        protected virtual XElement WeaverConfig
        {
            get { return null; }
        }
    }
}