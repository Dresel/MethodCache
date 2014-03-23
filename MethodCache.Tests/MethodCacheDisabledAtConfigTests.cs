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
        public void IndividualCache_ShouldCacheMethods()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClass6>(Assembly, cache);

            // Act
            var value = instance.MethodThree(10);
            value = instance.MethodThree(10);

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(1));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(1));
        }

        [Test]
        public void ClassLevelCache_ShouldNotCacheMethods()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClass7>(Assembly, cache);

            // Act
            var value = instance.MethodOne(10);
            value = instance.MethodOne(10);

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
        }

        [Test]
        public void ClassLevelCache_ShouldNotCacheStaticMethods()
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

        [Test]
        public void ClassLevelCache_ShouldCacheMethodWhenSelectedExplicitlyOnClassAttribute()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassPropertiesExcluded>(Assembly, cache);

            // Act
            var value = instance.Method(10);
            value = instance.Method(10);

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(1));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(1));
        }

        [Test]
        public void ClassLevelCache_ShouldCacheMethodWhenAllMembersSelectedExplicitlyOnClassAttribute()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassAllExplicitlyIncluded>(Assembly, cache);

            // Act
            var value = instance.Method(10);
            value = instance.Method(10);

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(1));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(1));
        }
    }
}