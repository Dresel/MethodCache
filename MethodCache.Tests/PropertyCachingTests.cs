using System;
using System.Reflection;
using MethodCache.Tests.TestAssembly;
using MethodCache.Tests.TestAssembly.Cache;
using NUnit.Framework;

namespace MethodCache.Tests
{
    public class PropertyCachingTests : ModuleWeaverTestsBase
    {
        [Test]
        public void ClassLevelCache_ShouldCacheReadOnlyProperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            var value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void IndividualCache_ShouldCacheReadOnlyProperty()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

            // Act
            var value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void ClassLevelCache_ShouldRemoveClassCacheAttribute()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);
            Type type = instance.GetType();

            // Act
            object[] customAttributes = type.GetCustomAttributes(true);

            // Assert
            Assert.IsEmpty(customAttributes);
        }

        [Test]
        public void ClassLevelCache_ShouldRespectNoCacheAttribute()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            var value = instance.ReadOnlyNoCache;
            value = instance.ReadOnlyNoCache;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 0);
            Assert.IsTrue(cache.NumRetrieveCalls == 0);
        }

        [Test]
        public void ClassLevelCache_ShouldRemoveNoCacheAttribute()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty("ReadOnlyNoCache");

            // Act
            object[] customAttributes = property.GetCustomAttributes(true);

            // Assert
            Assert.IsEmpty(customAttributes);
        }

        [Test]
        public void ClassLevelCache_ShouldNotCacheAutoproperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            var value = instance.AutoProperty;
            value = instance.AutoProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 0);
            Assert.IsTrue(cache.NumRetrieveCalls == 0);
        }

        [Test]
        public void IndividualCache_ShouldNotCacheAutoproperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

            // Act
            var value = instance.AutoProperty;
            value = instance.AutoProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 0);
            Assert.IsTrue(cache.NumRetrieveCalls == 0);
        }

        [Test]
        public void ClassLevelCache_ShouldRemoveCacheAttributesFromAutoproperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty("AutoProperty");

            // Act
            object[] customAttributes = property.GetCustomAttributes(true);

            // Assert
            Assert.IsEmpty(customAttributes);
        }

        [Test]
        public void ClassLevelCache_ShouldCacheReadWriteProperty()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            var value = instance.ReadWriteProperty;
            value = instance.ReadWriteProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void IndividualCache_ShouldCacheReadWriteProperty()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

            // Act
            var value = instance.ReadWriteProperty;
            value = instance.ReadWriteProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void ClassLevelCache_SettingPropertyShouldInvalidateCache()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            var value = instance.ReadWriteProperty;
            instance.ReadWriteProperty = 10;

            // Assert
            Assert.That(cache.Contains("MethodCache.Tests.TestAssembly.TestClassWithProperties.ReadWriteProperty"), Is.False);
        }

        [Test]
        public void IndividualCache_SettingPropertyShouldInvalidateCache()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

            // Act
            var value = instance.ReadWriteProperty;
            instance.ReadWriteProperty = 10;

            // Assert
            Assert.That(cache.Contains("MethodCache.Tests.TestAssembly.TestClassIndividualProperties.ReadWriteProperty"), Is.False);
        }

        [Test]
        public void ClassLevelCache_ShouldCacheStaticReadOnlyProperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

            Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
            Type cacheType = Assembly.GetType(typeof(ICache).FullName);

            PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
            cacheProperty.SetValue(null, cache, null);

            PropertyInfo property = testClassType.GetProperty("ReadOnlyProperty");

            // Act
            var value = property.GetValue(null, null);
            value = property.GetValue(null, null);

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void ClassLevelCache_ShouldCacheStaticReadWriteProperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

            Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
            Type cacheType = Assembly.GetType(typeof(ICache).FullName);

            PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
            cacheProperty.SetValue(null, cache, null);

            PropertyInfo property = testClassType.GetProperty("ReadWriteProperty");

            // Act
            var value = property.GetValue(null, null);
            value = property.GetValue(null, null);

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void ClassLevelCache_ShouldInvalidateCacheWhenSettingReadWriteProperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

            Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
            Type cacheType = Assembly.GetType(typeof(ICache).FullName);

            PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
            cacheProperty.SetValue(null, cache, null);

            PropertyInfo property = testClassType.GetProperty("ReadWriteProperty");

            // Act
            property.GetValue(null, null);
            property.SetValue(null, 5, null);

            // Assert
            Assert.That(cache.Contains("MethodCache.Tests.TestAssembly.TestClassStaticProperties.ReadWriteProperty"), Is.False);
        }

        [Test]
        public void ClassLevelCache_ShouldBePossibleToDisablePropertyWeaving()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassPropertiesExcluded>(Assembly, cache);

            // Act
            var value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 0);
            Assert.IsTrue(cache.NumRetrieveCalls == 0);
        }

        [Test]
        public void ClassLevelCache_ShouldBePossibleToSelectivelyEnablePropertyWeaving()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassPropertiesExcluded>(Assembly, cache);

            // Act
            var value = instance.ExplicitlyCachedProperty;
            value = instance.ExplicitlyCachedProperty;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void ClassLevelCache_ShouldCachePropertyWhenMethodsAreExcluded()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassMethodsExcluded>(Assembly, cache);

            // Act
            var value = instance.Property;
            value = instance.Property;

            // Assert
            Assert.IsTrue(cache.NumStoreCalls == 1);
            Assert.IsTrue(cache.NumRetrieveCalls == 1);
        }

        [Test]
        public void ClassLevelCache_ShouldNotWeaveWriteOnlyProperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

            // Act
            instance.SetOnlyProperty = "some text";

            // Assert
            Assert.That(cache.NumRemoveCalls, Is.EqualTo(0));
        }

        [Test]
        public void IndividualCache_ShouldNeverWeaveWriteOnlyProperties()
        {
            // Arrange
            var cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
            var instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

            // Act
            instance.SetOnlyProperty = "some text";

            // Assert
            Assert.That(cache.NumRemoveCalls, Is.EqualTo(0));
        }
    }
}