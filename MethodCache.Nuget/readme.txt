Since 1.4.0.0 it is now possible to also cache properties (see https://github.com/Dresel/MethodCache for details).

If you want this package to behave as previous packages (let class level [Cache] attribute only cache methods), modify ModuleWeavers.xml to:

<MethodCache CacheMethods="true" CacheProperties="false" />