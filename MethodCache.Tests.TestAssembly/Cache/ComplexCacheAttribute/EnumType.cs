namespace MethodCache.Tests.TestAssembly.Cache.ComplexCacheAttribute
{
	using System;

	[Flags()]
	public enum EnumType
	{
		Entry1 = 1,

		Entry2 = 2,

		Entry3 = 4
	}
}