namespace MethodCache.Tests.TestAssembly.Cache.ComplexCacheAttribute
{
	using System;

	public class CacheAttribute : Attribute
	{
		public bool ParameterBool { get; set; }

		public bool[] ParameterBoolArray { get; set; }

		public byte ParameterByte { get; set; }

		public byte[] ParameterByteArray { get; set; }

		public char ParameterCharacter { get; set; }

		public char[] ParameterCharacterArray { get; set; }

		public double ParameterDouble { get; set; }

		public double[] ParameterDoubleArray { get; set; }

		public EnumType ParameterEnum { get; set; }

		public EnumType[] ParameterEnumArray { get; set; }

		public float ParameterFloat { get; set; }

		public float[] ParameterFloatArray { get; set; }

		public int ParameterInt { get; set; }

		public int[] ParameterIntArray { get; set; }

		public long ParameterLong { get; set; }

		public long[] ParameterLongArray { get; set; }

		public object ParameterObject { get; set; }

		public object[] ParameterObjectArray { get; set; }

		public sbyte ParameterSByte { get; set; }

		public sbyte[] ParameterSByteArray { get; set; }

		public short ParameterShort { get; set; }

		public short[] ParameterShortArray { get; set; }

		public string ParameterString { get; set; }

		public string[] ParameterStringArray { get; set; }

		public Type ParameterType { get; set; }

		public Type[] ParameterTypeArray { get; set; }

		public uint ParameterUInt { get; set; }

		public uint[] ParameterUIntArray { get; set; }

		public ulong ParameterULong { get; set; }

		public ulong[] ParameterULongArray { get; set; }

		public ushort ParameterUShort { get; set; }

		public ushort[] ParameterUShortArray { get; set; }
	}
}