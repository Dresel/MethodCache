namespace MethodCache.Tests
{
	using System;
	using System.Linq;
	using System.Reflection;
	using MethodCache.Fody;
	using MethodCache.Tests.TestAssembly;
	using MethodCache.Tests.TestAssembly.Cache;
	using NUnit.Framework;

	public class ModuleWeaverTests : ModuleWeaverTestsBase
	{
		[Test]
		public void ClassLevelCacheMethodsExcluded()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassMethodsExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.Method(10);
			value = instance.Method(10);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void ClassLevelCacheMethodsExcludedChooseSelectively()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassMethodsExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.MethodIncluded(10);
			value = instance.MethodIncluded(10);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ClassLevelCacheMethodsWhenPropertiesExcluded()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassPropertiesExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.Method(10);
			value = instance.Method(10);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void ModuleWeaverExecuteWeavesCorrectIL()
		{
			string assemblyPath = Assembly.CodeBase.Remove(0, 8);
			string result = Verifier.Verify(assemblyPath);

			Assert.IsTrue(result.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath)));
		}

		[Test]
		public void ModuleWeaverRemovesMethodCacheAttributeReference()
		{
			Assert.IsFalse(Assembly.GetReferencedAssemblies().Any(x => x.Name == "MethodCache.Attributes"));
		}

		[Test]
		public void TestClass1ClassLevelCacheAttributeCachedMethodTwoReturnsSameInstance()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(Assembly, cache);

			// Act
			dynamic instance1 = testClass1.MethodTwo("1337");
			dynamic instance2 = testClass1.MethodTwo("1337");

			// Assert
			Assert.IsTrue(ReferenceEquals(instance1, instance2));
		}

		[Test]
		public void TestClass1ClassLevelCacheAttributeCachesMethodOne()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(Assembly, cache);

			// Act
			testClass1.MethodOne(1337);
			testClass1.MethodOne(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass1ClassLevelCacheAttributeCachesMethodTwo()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(Assembly, cache);

			// Act
			testClass1.MethodTwo("1337");
			testClass1.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass1ClassLevelCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(Assembly, cache);

			Type classType = testClass1.GetType();

			Assert.IsFalse(classType.GetCustomAttributes(true).Any(x => x.GetType().Name == ModuleWeaver.CacheAttributeName));
		}

		[Test]
		public void TestClass2MethodLevelCacheAttributeCachesMethodOne()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(Assembly, cache);

			// Act
			testClass2.MethodOne(1337);
			testClass2.MethodOne(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass2MethodLevelCacheAttributeDoesNotCachesMethodTwo()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(Assembly, cache);

			// Act
			testClass2.MethodTwo("1337");
			testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void TestClass2MethodLevelCacheAttributeNotCachedMethodTwoReturnsDifferentInstances()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(Assembly, cache);

			// Act
			dynamic instance1 = testClass2.MethodTwo("1337");
			dynamic instance2 = testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(!ReferenceEquals(instance1, instance2));
		}

		[Test]
		public void TestClass2MethodLevelCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(Assembly, cache);

			Type classType = testClass2.GetType();
			MethodInfo methodInfo = classType.GetMethod("MethodOne");

			Assert.IsFalse(methodInfo.GetCustomAttributes(true).Any(x => x.GetType().Name == ModuleWeaver.CacheAttributeName));
		}

		[Test]
		public void TestClass3ConcreteCacheGetterCachesMethodOne()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass3>(Assembly, cache);

			// Act
			testClass2.MethodOne(1337);
			testClass2.MethodOne(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass3ConcreteCacheGetterCachesMethodTwo()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass3>(Assembly, cache);

			// Act
			testClass2.MethodTwo("1337");
			testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass4MissingCacheGetterSuccessfullyIgnored()
		{
			// Arrange
			dynamic testClass4 = WeaverHelper.CreateInstance<TestClass4>(Assembly);

			// Act
			testClass4.MethodOne(1337);

			// Assert (test should not throw any exceptions)
		}

		[Test]
		public void TestClass5WrongCacheGetterTypeSuccessfullyIgnored()
		{
			// Arrange
			dynamic testClass5 = WeaverHelper.CreateInstance<TestClass5>(Assembly);

			// Act
			testClass5.MethodOne(1337);

			// Assert (test should not throw any exceptions)
		}

		[Test]
		public void TestClass6InheritedCacheGetterCanBeUsed()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass6 = WeaverHelper.CreateInstance<TestClass6>(Assembly, cache);

			// Act
			testClass6.MethodThree(1337);
			testClass6.MethodThree(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass7MethodLevelNoCacheAttributeDoesNotCachesMethodTwo()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(Assembly, cache);

			// Act
			testClass7.MethodTwo("1337");
			testClass7.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[Test]
		public void TestClass7MethodLevelNoCacheAttributeNotCachedMethodTwoReturnsDifferentInstances()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(Assembly, cache);

			// Act
			dynamic instance1 = testClass7.MethodTwo("1337");
			dynamic instance2 = testClass7.MethodTwo("1337");

			// Assert
			Assert.IsTrue(!ReferenceEquals(instance1, instance2));
		}

		[Test]
		public void TestClass7MethodLevelNoCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(Assembly, cache);

			Type classType = testClass7.GetType();
			MethodInfo methodInfo = classType.GetMethod("MethodTwo");

			Assert.IsFalse(methodInfo.GetCustomAttributes(true).Any(x => x.GetType().Name == ModuleWeaver.NoCacheAttributeName));
		}

		[Test]
		public void TestClass8ClassLevelCacheAttributeCachedMethodTwoReturnsSameInstance()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClass8).FullName);
			Type cacheType = Assembly.GetType(typeof(ICache).FullName);

			PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
			cacheProperty.SetValue(null, cache, null);

			MethodInfo method = testClassType.GetMethod("MethodTwo", new[] { typeof(string) });

			// Act
			dynamic instance1 = method.Invoke(null, new object[] { "1337" });
			dynamic instance2 = method.Invoke(null, new object[] { "1337" });

			// Assert
			Assert.IsTrue(ReferenceEquals(instance1, instance2));
		}

		[Test]
		public void TestClass8ClassLevelCacheAttributeCachesMethodOne()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClass8).FullName);
			Type cacheType = Assembly.GetType(typeof(ICache).FullName);

			PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
			cacheProperty.SetValue(null, cache, null);

			MethodInfo method = testClassType.GetMethod("MethodOne", new[] { typeof(int) });

			// Act
			method.Invoke(null, new object[] { 1337 });
			method.Invoke(null, new object[] { 1337 });

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[Test]
		public void TestClass8ClassLevelCacheAttributeCachesMethodTwo()
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
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}
	}
}