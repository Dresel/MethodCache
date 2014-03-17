namespace MethodCache.Tests
{
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public abstract class ModuleWeaverTestsBase
    {
        private static Assembly _assembly;

        protected Assembly Assembly
        {
            get { return _assembly; }
        }

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            if (_assembly == null)
            {
                _assembly = WeaverHelper.WeaveAssembly();
            }
        }
    }
}