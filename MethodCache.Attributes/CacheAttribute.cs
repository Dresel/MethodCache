namespace MethodCache.Attributes
{
	using System;

	/// <summary>
	///     Cache the output of this member or members of this class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class CacheAttribute : Attribute
	{
	    public CacheAttribute()
            : this(CacheMembers.All)
        {
	    }

	    public CacheAttribute(CacheMembers membersToCache)
	    {
	    }
	}
}