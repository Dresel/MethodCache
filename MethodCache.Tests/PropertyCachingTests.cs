namespace MethodCache.Tests
{
	using System;
	using System.Reflection;
	using MethodCache.Tests.TestAssembly;
	using MethodCache.Tests.TestAssembly.Cache;
	using NUnit.Framework;

	public class PropertyCachingTests : ModuleWeaverTestsBase
	{
		[Test]
		public void ClassLevelCacheWithoutRemove_ShouldCacheReadOnlyProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithPropertiesWithoutRemove>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCacheWithoutRemove_ShouldNotCacheReadWriteProperty()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithPropertiesWithoutRemove>(Assembly, cache);

			// Act
			dynamic value = instance.ReadWriteProperty;
			value = instance.ReadWriteProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void ClassLevelCache_SettingPropertyShouldInvalidateCache()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadWriteProperty;
			instance.ReadWriteProperty = 10;

			// Assert
			Assert.That(cache.Contains("MethodCache.Tests.TestAssembly.TestClassWithProperties.ReadWriteProperty"), Is.False);
		}

		[Test]
		public void ClassLevelCache_ShouldBePossibleToDisablePropertyWeaving()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassPropertiesExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void ClassLevelCache_ShouldBePossibleToSelectivelyEnablePropertyWeaving()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassPropertiesExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.ExplicitlyCachedProperty;
			value = instance.ExplicitlyCachedProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCache_ShouldCachePropertyWhenMethodsAreExcluded()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassMethodsExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.Property;
			value = instance.Property;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCache_ShouldCacheReadOnlyProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCache_ShouldCacheReadWriteProperty()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadWriteProperty;
			value = instance.ReadWriteProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCache_ShouldCacheStaticReadOnlyProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
			Type cacheType = Assembly.GetType(typeof(ICacheWithRemove).FullName);

			PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
			cacheProperty.SetValue(null, cache, null);

			PropertyInfo property = testClassType.GetProperty("ReadOnlyProperty");

			// Act
			object value = property.GetValue(null, null);
			value = property.GetValue(null, null);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCache_ShouldCacheStaticReadWriteProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
			Type cacheType = Assembly.GetType(typeof(ICacheWithRemove).FullName);

			PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
			cacheProperty.SetValue(null, cache, null);

			PropertyInfo property = testClassType.GetProperty("ReadWriteProperty");

			// Act
			object value = property.GetValue(null, null);
			value = property.GetValue(null, null);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCache_ShouldInvalidateCacheWhenSettingReadWriteProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
			Type cacheType = Assembly.GetType(typeof(ICacheWithRemove).FullName);

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
		public void ClassLevelCache_ShouldNotCacheAutoproperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			dynamic value = instance.AutoProperty;
			value = instance.AutoProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void ClassLevelCache_ShouldNotWeaveWriteOnlyProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			instance.SetOnlyProperty = "some text";

			// Assert
			Assert.That(cache.NumRemoveCalls, Is.EqualTo(0));
		}

		[Test]
		public void ClassLevelCache_ShouldRemoveCacheAttributesFromAutoproperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);
			Type type = instance.GetType();
			PropertyInfo property = type.GetProperty("AutoProperty");

			// Act
			object[] customAttributes = property.GetCustomAttributes(true);

			// Assert
			Assert.IsEmpty(customAttributes);
		}

		[Test]
		public void ClassLevelCache_ShouldRemoveClassCacheAttribute()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);
			Type type = instance.GetType();

			// Act
			object[] customAttributes = type.GetCustomAttributes(true);

			// Assert
			Assert.IsEmpty(customAttributes);
		}

		[Test]
		public void ClassLevelCache_ShouldRemoveNoCacheAttribute()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);
			Type type = instance.GetType();
			PropertyInfo property = type.GetProperty("ReadOnlyNoCache");

			// Act
			object[] customAttributes = property.GetCustomAttributes(true);

			// Assert
			Assert.IsEmpty(customAttributes);
		}

		[Test]
		public void ClassLevelCache_ShouldRespectNoCacheAttribute()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyNoCache;
			value = instance.ReadOnlyNoCache;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void IndividualCache_SettingPropertyShouldInvalidateCache()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadWriteProperty;
			instance.ReadWriteProperty = 10;

			// Assert
			Assert.That(cache.Contains("MethodCache.Tests.TestAssembly.TestClassIndividualProperties.ReadWriteProperty"),
				Is.False);
		}

		[Test]
		public void IndividualCache_ShouldCacheReadOnlyProperty()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void IndividualCache_ShouldCacheReadWriteProperty()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadWriteProperty;
			value = instance.ReadWriteProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void IndividualCache_ShouldNeverWeaveWriteOnlyProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

			// Act
			instance.SetOnlyProperty = "some text";

			// Assert
			Assert.That(cache.NumRemoveCalls, Is.EqualTo(0));
		}

		[Test]
		public void IndividualCache_ShouldNotCacheAutoproperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

			// Act
			dynamic value = instance.AutoProperty;
			value = instance.AutoProperty;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}
	}
}