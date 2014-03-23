namespace MethodCache.Tests
{
    using System;
    using System.Reflection;
    using System.Xml.Linq;
    using MethodCache.Tests.TestAssembly;
    using MethodCache.Tests.TestAssembly.Cache;
    using NUnit.Framework;

    public class MethodCacheDisabledAtConfigTests : ModuleWeaverTestsBase
    {
        protected override XElement WeaverConfig
        {
            get
            {
                return new XElement("MethodCache",
                                    new XAttribute("CacheMethods", false)
                    );
            }
        }

        [Test]
        public void IndividualCache_ShouldNotCacheAnyMethodsWhenDisabledInXmlConfig()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClass6>(Assembly, cache);

            // Act
            var value = instance.MethodThree(10);
            value = instance.MethodThree(10);

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
        }

        [Test]
        public void ClassLevelCache_ShouldNotCacheAnyMethodsWhenDisabledInXmlConfig()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClass7>(Assembly, cache);

            // Act
            var value = instance.MethodOne(10);
            value = instance.MethodOne(10);
            value = instance.MethodTwo("a");
            value = instance.MethodTwo("a");

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
        }

        [Test]
        public void ClassLevelCache_ShouldNotCacheStaticMethodsWhenDisabledInXmlConfig()
        {
            // Arrange
            dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

            Type testClassType = Assembly.GetType(typeof(TestClass8).FullName);
            Type cacheType = Assembly.GetType(typeof(ICache).FullName);

            PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
            cacheProperty.SetValue(null, cache, null);

            MethodInfo method = testClassType.GetMethod("MethodTwo", new[] { typeof(string) });

            // Act
            method.Invoke(null, new object[] { "1337" });
            method.Invoke(null, new object[] { "1337" });

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
        }
    }
}