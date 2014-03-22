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
        public void ClassLevelCache_ShouldRemoveClassAttribute()
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
    }
}