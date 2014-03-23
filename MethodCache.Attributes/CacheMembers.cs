namespace MethodCache.Attributes
{
    using System;

    [Flags]
    public enum CacheMembers
    {
        Methods = 1,
        Properties = 2,
        All = 3
    }
}