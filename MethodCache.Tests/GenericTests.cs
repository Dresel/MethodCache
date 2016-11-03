namespace MethodCache.Tests
{
	using System;
	using MethodCache.Tests.TestAssembly;
	using MethodCache.Tests.TestAssembly.Cache;
	using NUnit.Framework;

	public class GenericTests : ModuleWeaverTestsBase
	{
		[Test]
		public void GenericClassReferenceTypeExecutesCorrectly()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassGeneric<Tuple<int, double>>>(Assembly, cache);

			// Act
			dynamic value1 = instance.Method1<Tuple<int, double>>(new Tuple<int, double>(1, 2.0));
			value1 = instance.Method1<Tuple<int, double>>(new Tuple<int, double>(1, 2.0));
			dynamic value2 = instance.Method2(new Tuple<int, double>(1, 2.0));
			value2 = instance.Method2(new Tuple<int, double>(1, 2.0));

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 2);
			Assert.IsTrue(cache.NumRetrieveCalls == 2);
		}

		[Test]
		public void GenericClassValueTypeExecutesCorrectly()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassGeneric<int>>(Assembly, cache);

			// Act
			dynamic value1 = instance.Method1<int>(10);
			value1 = instance.Method1<int>(10);
			dynamic value2 = instance.Method2(10);
			value2 = instance.Method2(10);

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 2);
			Assert.IsTrue(cache.NumRetrieveCalls == 2);
		}

		[Test]
		public void GenericClassValueTypePropertyExecutesCorrectly()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassGeneric<int>>(Assembly, cache);

			// Act
			dynamic value = instance.Property;
			value = instance.Property;
			instance.Property = value;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
			Assert.IsTrue(cache.NumRemoveCalls == 1);
		}

		[Test]
		public void GenericClassReferenceTypePropertyExecutesCorrectly()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassGeneric<Tuple<int, double>>>(Assembly, cache);

			// Act
			instance.Property = new Tuple<int, double>(1, 2.0);
			dynamic value = instance.Property;
			value = instance.Property;
			instance.Property = value;

			// Assert
			Assert.IsTrue(cache.NumStoreCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
			Assert.IsTrue(cache.NumRemoveCalls == 2);
		}

		[Test]
		public void GenericMethod()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassGeneric<Tuple<int, double>>>(Assembly, cache);

			// Act
			dynamic value1 = instance.Method1<int>(0);

			// Assert
			Assert.IsTrue(cache.Contains(string.Format("MethodCache.Tests.TestAssembly.TestClassGeneric`1.Method1_{0}_{1}", typeof(int), 0)));
		}
	}
}