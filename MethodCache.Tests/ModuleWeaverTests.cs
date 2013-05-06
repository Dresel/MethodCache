namespace MethodCache.Tests
{
	using System.Reflection;
	using MethodCache.ReferenceAssembly;
	using MethodCache.ReferenceAssembly.Cache;
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
		public void TestClass2ClassLevelCacheAttributeNotCachedMethodTwoReturnsDifferentInstances()
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

		#endregion
	}
}