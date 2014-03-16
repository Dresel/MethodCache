namespace MethodCache.Attributes
{
	using System;

	/// <summary>
	///     Cache the output of this method or methods of this class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class CacheAttribute : Attribute
	{
	}
}