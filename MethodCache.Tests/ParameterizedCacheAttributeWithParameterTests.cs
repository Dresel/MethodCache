namespace MethodCache.Tests
{
	using System;
	using System.Collections.Generic;
	using MethodCache.Tests.TestAssembly;
	using MethodCache.Tests.TestAssembly.Cache;
	using MethodCache.Tests.TestAssembly.Cache.ComplexCacheAttribute;
	using NUnit.Framework;

	public class ParameterizedCacheAttributeWithParameterTests : ModuleWeaverTestsBase
	{
		[Test]
		public void TestClassWithParameterizedCacheAttributeClassLevelCacheAttributeAddsParameters()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.CacheParameterMethod("1", 2);
			testClass1.CacheParameterMethod("1", 2);

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 2);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["Parameter1"] == "CacheParameter");
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["Parameter2"] == 1);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParameterArrays()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodArrays();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 12);

			bool[] boolArray = (bool[])cache.ParametersPassedToLastStoreCall["ParameterBoolArray"];
			Assert.IsTrue(boolArray[0] == false);
			Assert.IsTrue(boolArray[1] == true);

			byte[] byteArray = (byte[])cache.ParametersPassedToLastStoreCall["ParameterByteArray"];
			Assert.IsTrue(byteArray[0] == Byte.MinValue);
			Assert.IsTrue(byteArray[1] == Byte.MaxValue);

			char[] charArray = (char[])cache.ParametersPassedToLastStoreCall["ParameterCharacterArray"];
			Assert.IsTrue(charArray[0] == Char.MinValue);
			Assert.IsTrue(charArray[1] == Char.MaxValue);

			double[] doubleArray = (double[])cache.ParametersPassedToLastStoreCall["ParameterDoubleArray"];
			Assert.IsTrue(doubleArray[0] == Double.MinValue);
			Assert.IsTrue(doubleArray[1] == Double.MaxValue);

			float[] floatArray = (float[])cache.ParametersPassedToLastStoreCall["ParameterFloatArray"];
			Assert.IsTrue(floatArray[0] == Single.MinValue);
			Assert.IsTrue(floatArray[1] == Single.MaxValue);

			int[] intArray = (int[])cache.ParametersPassedToLastStoreCall["ParameterIntArray"];
			Assert.IsTrue(intArray[0] == Int32.MinValue);
			Assert.IsTrue(intArray[1] == Int32.MaxValue);

			long[] longArray = (long[])cache.ParametersPassedToLastStoreCall["ParameterLongArray"];
			Assert.IsTrue(longArray[0] == Int64.MinValue);
			Assert.IsTrue(longArray[1] == Int64.MaxValue);

			sbyte[] sByteArray = (sbyte[])cache.ParametersPassedToLastStoreCall["ParameterSByteArray"];
			Assert.IsTrue(sByteArray[0] == SByte.MinValue);
			Assert.IsTrue(sByteArray[1] == SByte.MaxValue);

			short[] shortArray = (short[])cache.ParametersPassedToLastStoreCall["ParameterShortArray"];
			Assert.IsTrue(shortArray[0] == Int16.MinValue);
			Assert.IsTrue(shortArray[1] == Int16.MaxValue);

			uint[] uIntArray = (uint[])cache.ParametersPassedToLastStoreCall["ParameterUIntArray"];
			Assert.IsTrue(uIntArray[0] == UInt32.MinValue);
			Assert.IsTrue(uIntArray[1] == UInt32.MaxValue);

			ulong[] uLongArray = (ulong[])cache.ParametersPassedToLastStoreCall["ParameterULongArray"];
			Assert.IsTrue(uLongArray[0] == UInt64.MinValue);
			Assert.IsTrue(uLongArray[1] == UInt64.MaxValue);

			ushort[] uShortArray = (ushort[])cache.ParametersPassedToLastStoreCall["ParameterUShortArray"];
			Assert.IsTrue(uShortArray[0] == UInt16.MinValue);
			Assert.IsTrue(uShortArray[1] == UInt16.MaxValue);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersEnum()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodEnum();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);

			var type = (dynamic)WeaverHelper.CreateType<EnumType>(Assembly);

			Assert.IsInstanceOfType(type, cache.ParametersPassedToLastStoreCall["ParameterEnum"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterEnum"] ==
				(Enum.GetValues(type)[0] | Enum.GetValues(type)[2]));
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersEnumArray()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodEnumArray();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);

			var type = (dynamic)WeaverHelper.CreateType<EnumType>(Assembly);
			var typeArray = (dynamic)WeaverHelper.CreateType<EnumType[]>(Assembly);

			Assert.IsInstanceOfType(typeArray, cache.ParametersPassedToLastStoreCall["ParameterEnumArray"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterEnumArray"][0] ==
				(Enum.GetValues(type)[0] | Enum.GetValues(type)[2]));
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterEnumArray"][1] == Enum.GetValues(type)[1]);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersMax()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodMax();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 12);
			Assert.IsTrue((bool)cache.ParametersPassedToLastStoreCall["ParameterBool"] == false);
			Assert.IsTrue((byte)cache.ParametersPassedToLastStoreCall["ParameterByte"] == Byte.MaxValue);
			Assert.IsTrue((char)cache.ParametersPassedToLastStoreCall["ParameterCharacter"] == Char.MaxValue);
			Assert.IsTrue((double)cache.ParametersPassedToLastStoreCall["ParameterDouble"] == Double.MaxValue);
			Assert.IsTrue((float)cache.ParametersPassedToLastStoreCall["ParameterFloat"] == Single.MaxValue);
			Assert.IsTrue((int)cache.ParametersPassedToLastStoreCall["ParameterInt"] == Int32.MaxValue);
			Assert.IsTrue((long)cache.ParametersPassedToLastStoreCall["ParameterLong"] == Int64.MaxValue);
			Assert.IsTrue((sbyte)cache.ParametersPassedToLastStoreCall["ParameterSByte"] == SByte.MaxValue);
			Assert.IsTrue((short)cache.ParametersPassedToLastStoreCall["ParameterShort"] == Int16.MaxValue);
			Assert.IsTrue((uint)cache.ParametersPassedToLastStoreCall["ParameterUInt"] == UInt32.MaxValue);
			Assert.IsTrue((ulong)cache.ParametersPassedToLastStoreCall["ParameterULong"] == UInt64.MaxValue);
			Assert.IsTrue((ushort)cache.ParametersPassedToLastStoreCall["ParameterUShort"] == UInt16.MaxValue);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersMin()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodMin();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 12);
			Assert.IsTrue((bool)cache.ParametersPassedToLastStoreCall["ParameterBool"] == false);
			Assert.IsTrue((byte)cache.ParametersPassedToLastStoreCall["ParameterByte"] == Byte.MinValue);
			Assert.IsTrue((char)cache.ParametersPassedToLastStoreCall["ParameterCharacter"] == Char.MinValue);
			Assert.IsTrue((double)cache.ParametersPassedToLastStoreCall["ParameterDouble"] == Double.MinValue);
			Assert.IsTrue((float)cache.ParametersPassedToLastStoreCall["ParameterFloat"] == Single.MinValue);
			Assert.IsTrue((int)cache.ParametersPassedToLastStoreCall["ParameterInt"] == Int32.MinValue);
			Assert.IsTrue((long)cache.ParametersPassedToLastStoreCall["ParameterLong"] == Int64.MinValue);
			Assert.IsTrue((sbyte)cache.ParametersPassedToLastStoreCall["ParameterSByte"] == SByte.MinValue);
			Assert.IsTrue((short)cache.ParametersPassedToLastStoreCall["ParameterShort"] == Int16.MinValue);
			Assert.IsTrue((uint)cache.ParametersPassedToLastStoreCall["ParameterUInt"] == UInt32.MinValue);
			Assert.IsTrue((ulong)cache.ParametersPassedToLastStoreCall["ParameterULong"] == UInt64.MinValue);
			Assert.IsTrue((ushort)cache.ParametersPassedToLastStoreCall["ParameterUShort"] == UInt16.MinValue);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersObjectArray()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodObjectArray();

			// Assert
			var type = (dynamic)WeaverHelper.CreateType<EnumType>(Assembly);

			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(object[]), cache.ParametersPassedToLastStoreCall["ParameterObjectArray"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"].Length == 5);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"][0] == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"][1] == "2");
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"][2] == Enum.GetValues(type)[2]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"][3] == typeof(List<int>));
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"][4][0] == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObjectArray"][4][1] == "2");
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersObjectArrayString()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodObjectArrayString();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(string[]), cache.ParametersPassedToLastStoreCall["ParameterObject"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObject"].Length == 2);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObject"][0] == "1");
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObject"][1] == "2");
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersObjectEnum()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodObjectEnum();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);

			var type = (dynamic)WeaverHelper.CreateType<EnumType>(Assembly);

			Assert.IsInstanceOfType(type, cache.ParametersPassedToLastStoreCall["ParameterObject"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObject"] ==
				(Enum.GetValues(type)[0] | Enum.GetValues(type)[2]));
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersObjectType()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodObjectType();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(Type), cache.ParametersPassedToLastStoreCall["ParameterObject"]);
			Assert.IsTrue((Type)cache.ParametersPassedToLastStoreCall["ParameterObject"] == typeof(List<int>));
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersObjectValueType()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodObjectValueType();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(bool), cache.ParametersPassedToLastStoreCall["ParameterObject"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterObject"] == true);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersStringArray()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodStringArray();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(string[]), cache.ParametersPassedToLastStoreCall["ParameterStringArray"]);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterStringArray"].Length == 2);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterStringArray"][0] == "1");
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["ParameterStringArray"][1] == "2");
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersType()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodType();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(Type), cache.ParametersPassedToLastStoreCall["ParameterType"]);
			Assert.IsTrue((Type)cache.ParametersPassedToLastStoreCall["ParameterType"] == typeof(List<int>));
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeComplexCacheAttributePassesParametersTypeArray()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.ComplexCacheParameterMethodTypeArray();

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 1);
			Assert.IsInstanceOfType(typeof(Type[]), cache.ParametersPassedToLastStoreCall["ParameterTypeArray"]);
			Assert.IsTrue((Type)cache.ParametersPassedToLastStoreCall["ParameterTypeArray"][0] == typeof(List<int>));
			Assert.IsTrue((Type)cache.ParametersPassedToLastStoreCall["ParameterTypeArray"][1] == typeof(List<string>));
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeMethodLevelCacheAttributeAlsoWorksForFields()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.CacheParameterMethodWithField("1", 2);
			testClass1.CacheParameterMethodWithField("1", 2);

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 2);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["Parameter3"] == true);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["parameter3"] == 0);
		}

		[Test]
		public void TestClassWithParameterizedCacheAttributeMethodLevelCacheAttributeOverridesParameters()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic testClass1 = WeaverHelper.CreateInstance<TestClassWithParameterizedCacheAttribute>(Assembly, cache);

			// Act
			testClass1.OverridenCacheParameterMethod("1", 2);
			testClass1.OverridenCacheParameterMethod("1", 2);

			// Assert
			Assert.IsTrue(cache.NumStoreParameterCalls == 1);
			Assert.IsTrue(cache.NumRetrieveCalls == 1);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall.Count == 2);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["Parameter2"] == 2);
			Assert.IsTrue(cache.ParametersPassedToLastStoreCall["Parameter3"] == true);
		}
	}
}