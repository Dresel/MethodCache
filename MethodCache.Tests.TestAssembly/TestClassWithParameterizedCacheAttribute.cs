namespace MethodCache.Tests.TestAssembly
{
	using System;
	using System.Collections.Generic;
	using MethodCache.Tests.TestAssembly.Cache;
	using MethodCache.Tests.TestAssembly.Cache.ComplexCacheAttribute;

	[Cache.Cache(Parameter1 = "CacheParameter", Parameter2 = 1)]
	public class TestClassWithParameterizedCacheAttribute
	{
		public TestClassWithParameterizedCacheAttribute(ICacheWithStoreParameters cache)
		{
			Cache = cache;
		}

		public ICacheWithStoreParameters Cache { get; set; }

		public string CacheParameterMethod(string argument1, int argument2)
		{
			return argument1 + argument2;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterBoolArray = new bool[] { false, true },
			ParameterByteArray = new byte[] { Byte.MinValue, Byte.MaxValue },
			ParameterCharacterArray = new char[] { Char.MinValue, Char.MaxValue },
			ParameterDoubleArray = new double[] { Double.MinValue, Double.MaxValue },
			ParameterFloatArray = new float[] { Single.MinValue, Single.MaxValue },
			ParameterIntArray = new int[] { Int32.MinValue, Int32.MaxValue },
			ParameterLongArray = new long[] { Int64.MinValue, Int64.MaxValue },
			ParameterSByteArray = new sbyte[] { SByte.MinValue, SByte.MaxValue },
			ParameterShortArray = new short[] { Int16.MinValue, Int16.MaxValue },
			ParameterUIntArray = new uint[] { UInt32.MinValue, UInt32.MaxValue },
			ParameterULongArray = new ulong[] { UInt64.MinValue, UInt64.MaxValue },
			ParameterUShortArray = new ushort[] { UInt16.MinValue, UInt16.MaxValue })]
		public int ComplexCacheParameterMethodArrays()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterEnum = EnumType.Entry1 | EnumType.Entry3)]
		public int ComplexCacheParameterMethodEnum()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(
			ParameterEnumArray = new EnumType[] { EnumType.Entry1 | EnumType.Entry3, EnumType.Entry2 })]
		public int ComplexCacheParameterMethodEnumArray()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterBool = false, ParameterByte = Byte.MaxValue,
			ParameterCharacter = Char.MaxValue, ParameterDouble = Double.MaxValue, ParameterFloat = Single.MaxValue,
			ParameterInt = Int32.MaxValue, ParameterLong = Int64.MaxValue, ParameterSByte = SByte.MaxValue,
			ParameterShort = Int16.MaxValue, ParameterUInt = UInt32.MaxValue, ParameterULong = UInt64.MaxValue,
			ParameterUShort = UInt16.MaxValue)]
		public int ComplexCacheParameterMethodMax()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterBool = false, ParameterByte = Byte.MinValue,
			ParameterCharacter = Char.MinValue, ParameterDouble = Double.MinValue, ParameterFloat = Single.MinValue,
			ParameterInt = Int32.MinValue, ParameterLong = Int64.MinValue, ParameterSByte = SByte.MinValue,
			ParameterShort = Int16.MinValue, ParameterUInt = UInt32.MinValue, ParameterULong = UInt64.MinValue,
			ParameterUShort = UInt16.MinValue)]
		public int ComplexCacheParameterMethodMin()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(
			ParameterObjectArray = new object[] { 1, "2", EnumType.Entry3, typeof(List<int>), new object[] { 1, "2" } })]
		public int ComplexCacheParameterMethodObjectArray()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterObject = new string[] { "1", "2" })]
		public int ComplexCacheParameterMethodObjectArrayString()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterObject = EnumType.Entry1 | EnumType.Entry3)]
		public int ComplexCacheParameterMethodObjectEnum()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterObject = typeof(List<int>))]
		public int ComplexCacheParameterMethodObjectType()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterObject = true)]
		public int ComplexCacheParameterMethodObjectValueType()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterStringArray = new[] { "1", "2" })]
		public int ComplexCacheParameterMethodStringArray()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterType = typeof(List<int>))]
		public int ComplexCacheParameterMethodType()
		{
			return 0;
		}

		[Cache.ComplexCacheAttribute.Cache(ParameterTypeArray = new[] { typeof(List<int>), typeof(List<string>) })]
		public int ComplexCacheParameterMethodTypeArray()
		{
			return 0;
		}

		[Cache.Cache(Parameter2 = 2, Parameter3 = true)]
		public string OverridenCacheParameterMethod(string argument1, int argument2)
		{
			return argument1 + argument2;
		}
	}
}