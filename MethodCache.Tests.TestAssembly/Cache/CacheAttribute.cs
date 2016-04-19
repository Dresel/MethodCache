namespace MethodCache.Tests.TestAssembly.Cache
{
	using System;

	public class CacheAttribute : Attribute
	{
		public string Parameter1 { get; set; }

		public int Parameter2 { get; set; }

		public bool Parameter3 { get; set; }
	}
}