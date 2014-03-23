namespace MethodCache.Attributes
{
    using System;

    [Flags]
    public enum Members
    {
        Methods = 1,
        Properties = 2,
        All = 3
    }
}