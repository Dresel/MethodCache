namespace MethodCache.Attributes
{
	using System;

	/// <summary>
	///     Prevents caching the output of this method when CacheAttribute is used on class level.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class NoCacheAttribute : Attribute
	{
	}
}