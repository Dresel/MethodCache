namespace MethodCache.Tests
{
    using System.Xml.Linq;
    using MethodCache.Tests.TestAssembly;
    using MethodCache.Tests.TestAssembly.Cache;
    using NUnit.Framework;

    public class PropertyCacheDisabledAtConfigTests : ModuleWeaverTestsBase
    {
        protected override XElement WeaverConfig
        {
            get
            {
                return new XElement("MethodCache",
                                    new XAttribute("CacheProperties", false)
                    );
            }
        }

        [Test]
        public void ClassLevelCache_ShouldNotCacheAnyPropertiesWhenDisabledInXmlConfig()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            var value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyNoCache;
            value = instance.ReadOnlyNoCache;
            value = instance.AutoProperty;
            value = instance.AutoProperty;
            value = instance.ReadWriteProperty;
            value = instance.ReadWriteProperty;

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
        }

        [Test]
        public void IndividualCache_ShouldNotCacheAnyPropertiesWhenDisabledInXmlConfig()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

            // Act
            var value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyProperty;
            value = instance.AutoProperty;
            value = instance.AutoProperty;
            value = instance.ReadWriteProperty;
            value = instance.ReadWriteProperty;

            // Assert
            Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
            Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
        }
    }
}