namespace MethodCache.Tests
{
	using System;
	using System.Linq;
	using System.Reflection;
	using MethodCache.Fody;
	using MethodCache.TestAssembly;
	using MethodCache.TestAssembly.Cache;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class ModuleWeaverTests
	{
		#region Static Fields

		private static Assembly assembly;

		#endregion

		#region Public Methods and Operators

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			assembly = WeaverHelper.WeaveAssembly();
		}

		[TestMethod]
		public void ModuleWeaverExecuteWeavesCorrectIL()
		{
			string assemblyPath = assembly.CodeBase.Remove(0, 8);
			string result = Verifier.Verify(assemblyPath);

			Assert.IsTrue(result.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath)));
		}

		[TestMethod]
		public void ModuleWeaverRemovesMethodCacheAttributeReference()
		{
			Assert.IsFalse(assembly.GetReferencedAssemblies().Any(x => x.Name == "MethodCache.Attributes"));
		}

		[TestMethod]
		public void TestClass1ClassLevelCacheAttributeCachedMethodTwoReturnsSameInstance()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

			// Act
			dynamic instance1 = testClass1.MethodTwo("1337");
			dynamic instance2 = testClass1.MethodTwo("1337");

			// Assert
			Assert.IsTrue(ReferenceEquals(instance1, instance2));
		}

		[TestMethod]
		public void TestClass1ClassLevelCacheAttributeCachesMethodOne()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

			// Act
			testClass1.MethodOne(1337);
			testClass1.MethodOne(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[TestMethod]
		public void TestClass1ClassLevelCacheAttributeCachesMethodTwo()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

			// Act
			testClass1.MethodTwo("1337");
			testClass1.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[TestMethod]
		public void TestClass1ClassLevelCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClass1>(assembly, cache);

			Type classType = testClass1.GetType();

			Assert.IsFalse(classType.CustomAttributes.Any(x => x.AttributeType.Name == ModuleWeaver.CacheAttributeName));
		}

		[TestMethod]
		public void TestClass2MethodLevelCacheAttributeCachesMethodOne()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

			// Act
			testClass2.MethodOne(1337);
			testClass2.MethodOne(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[TestMethod]
		public void TestClass2MethodLevelCacheAttributeDoesNotCachesMethodTwo()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

			// Act
			testClass2.MethodTwo("1337");
			testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[TestMethod]
		public void TestClass2MethodLevelCacheAttributeNotCachedMethodTwoReturnsDifferentInstances()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

			// Act
			dynamic instance1 = testClass2.MethodTwo("1337");
			dynamic instance2 = testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(!ReferenceEquals(instance1, instance2));
		}

		[TestMethod]
		public void TestClass2MethodLevelCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass2>(assembly, cache);

			Type classType = testClass2.GetType();
			MethodInfo methodInfo = classType.GetMethod("MethodOne");

			Assert.IsFalse(methodInfo.CustomAttributes.Any(x => x.AttributeType.Name == ModuleWeaver.CacheAttributeName));
		}

		[TestMethod]
		public void TestClass3ConcreteCacheGetterCachesMethodOne()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass3>(assembly, cache);

			// Act
			testClass2.MethodOne(1337);
			testClass2.MethodOne(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[TestMethod]
		public void TestClass3ConcreteCacheGetterCachesMethodTwo()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass2 = WeaverHelper.CreateInstance<TestClass3>(assembly, cache);

			// Act
			testClass2.MethodTwo("1337");
			testClass2.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[TestMethod]
		public void TestClass4MissingCacheGetterSuccessfullyIgnored()
		{
			// Arrage
			dynamic testClass4 = WeaverHelper.CreateInstance<TestClass4>(assembly);

			// Act
			testClass4.MethodOne(1337);

			// Assert (test should not throw any exceptions)
		}

		[TestMethod]
		public void TestClass5WrongCacheGetterTypeSuccessfullyIgnored()
		{
			// Arrage
			dynamic testClass5 = WeaverHelper.CreateInstance<TestClass5>(assembly);

			// Act
			testClass5.MethodOne(1337);

			// Assert (test should not throw any exceptions)
		}

		[TestMethod]
		public void TestClass6InheritedCacheGetterCanBeUsed()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass6 = WeaverHelper.CreateInstance<TestClass6>(assembly, cache);

			// Act
			testClass6.MethodThree(1337);
			testClass6.MethodThree(1337);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
		}

		[TestMethod]
		public void TestClass7MethodLevelNoCacheAttributeDoesNotCachesMethodTwo()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(assembly, cache);

			// Act
			testClass7.MethodTwo("1337");
			testClass7.MethodTwo("1337");

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 0);
			Assert.IsTrue(cache.NumRetrieveCalls == 0);
		}

		[TestMethod]
		public void TestClass7MethodLevelNoCacheAttributeNotCachedMethodTwoReturnsDifferentInstances()
		{
			// Arrage
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(assembly, cache);

			// Act
			dynamic instance1 = testClass7.MethodTwo("1337");
			dynamic instance2 = testClass7.MethodTwo("1337");

			// Assert
			Assert.IsTrue(!ReferenceEquals(instance1, instance2));
		}

		[TestMethod]
		public void TestClass7MethodLevelNoCacheAttributeSuccessfullyRemoved()
		{
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(assembly);
			dynamic testClass7 = WeaverHelper.CreateInstance<TestClass7>(assembly, cache);

			Type classType = testClass7.GetType();
			MethodInfo methodInfo = classType.GetMethod("MethodTwo");

			Assert.IsFalse(methodInfo.CustomAttributes.Any(x => x.AttributeType.Name == ModuleWeaver.NoCacheAttributeName));
		}

		#endregion
	}
}