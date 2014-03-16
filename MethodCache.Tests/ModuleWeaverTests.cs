namespace MethodCache.Tests
{
	using System;
	using System.Linq;
	using System.Reflection;
	using MethodCache.Fody;
	using MethodCache.Tests.TestAssembly;
    using MethodCache.Tests.TestAssembly.Cache;
    using NUnit.Framework;

	[TestFixture]
	public class ModuleWeaverTests
	{
		private static Assembly assembly;

		[TestFixtureSetUp]
		public static void ClassInitialize()
		{
			assembly = WeaverHelper.WeaveAssembly();
		}

		[Test]
		public void ModuleWeaverExecuteWeavesCorrectIL()
		{
			string assemblyPath = assembly.CodeBase.Remove(0, 8);
			string result = Verifier.Verify(assemblyPath);

			Assert.IsTrue(result.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath)));
		}

		[Test]
		public void ModuleWeaverRemovesMethodCacheAttributeReference()
		{
			Assert.IsFalse(assembly.GetReferencedAssemblies().Any(x => x.Name == "MethodCache.Attributes"));
		}

		[Test]
		public void TestClass1ClassLevelCacheAttributeCachedMethodTwoReturnsSameInstance()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

			Type classType = testClass1.GetType();

			Assert.IsFalse(classType.GetCustomAttributes(true).Any(x => x.GetType().Name == ModuleWeaver.CacheAttributeName));
		}

		[Test]
		public void TestClass2MethodLevelCacheAttributeCachesMethodOne()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

			// Act
			dynamic instance1 = testClass2.MethodTwo("1337");
			dynamic instance2 = testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(!ReferenceEquals(instance1, instance2));
		}

		[Test]
		public void TestClass2MethodLevelCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

			Type classType = testClass2.GetType();
			MethodInfo methodInfo = classType.GetMethod("MethodOne");
            
			Assert.IsFalse(methodInfo.GetCustomAttributes(true).Any(x => x.GetType().Name == ModuleWeaver.CacheAttributeName));
		}

		[Test]
		public void TestClass3ConcreteCacheGetterCachesMethodOne()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass3>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass3>(assembly, cache);

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
			dynamic testClass4 = WeaverHelper.CreateInstance<TestClass4>(assembly);

			// Act
			testClass4.MethodOne(1337);

			// Assert (test should not throw any exceptions)
		}

		[Test]
		public void TestClass5WrongCacheGetterTypeSuccessfullyIgnored()
		{
			// Arrange
			dynamic testClass5 = WeaverHelper.CreateInstance<TestClass5>(assembly);

			// Act
			testClass5.MethodOne(1337);

			// Assert (test should not throw any exceptions)
		}

		[Test]
		public void TestClass6InheritedCacheGetterCanBeUsed()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass6 = WeaverHelper.CreateInstance<TestClass6>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(assembly, cache);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(assembly, cache);

			// Act
			dynamic instance1 = testClass7.MethodTwo("1337");
			dynamic instance2 = testClass7.MethodTwo("1337");

			// Assert
			Assert.IsTrue(!ReferenceEquals(instance1, instance2));
		}

		[Test]
		public void TestClass7MethodLevelNoCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(assembly, cache);

			Type classType = testClass7.GetType();
			MethodInfo methodInfo = classType.GetMethod("MethodTwo");

			Assert.IsFalse(methodInfo.GetCustomAttributes(true).Any(x => x.GetType().Name == ModuleWeaver.NoCacheAttributeName));
		}

		[Test]
		public void TestClass8ClassLevelCacheAttributeCachedMethodTwoReturnsSameInstance()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);

			Type testClassType = assembly.GetType(typeof(TestClass8).FullName);
			Type cacheType = assembly.GetType(typeof(ICache).FullName);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);

			Type testClassType = assembly.GetType(typeof(TestClass8).FullName);
			Type cacheType = assembly.GetType(typeof(ICache).FullName);

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
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);

			Type testClassType = assembly.GetType(typeof(TestClass8).FullName);
			Type cacheType = assembly.GetType(typeof(ICache).FullName);

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

	    [Test]
	    public void CachingReadOnlyProperties()
	    {
	        // Arrange
            var cache = new DictionaryCache();
	        var instance = new TestClassWithProperties(cache);

	        // Act
            var value = instance.ReadOnlyProperty;
            value = instance.ReadOnlyProperty;

	        // Assert
	        Assert.IsTrue(cache.NumStoreCalls == 1);
	        Assert.IsTrue(cache.NumRetrieveCalls == 1);
	    }
	}
}