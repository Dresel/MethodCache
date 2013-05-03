## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Caches return values of methods decorated with a `[Cache]` Attribute.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage).

# Nuget package

There is a nuget package avaliable here http://nuget.org/packages/MethodCache.Fody.

## Your Code

    [Cache]
    public int Add(int a, int b)
    {
        return a + b;
    }

## What gets compiled

    [Cache]
    public int Add(int a, int b)
    {
        string cacheKey = string.Format("Namespace.Class.Add_{0}_{1}", new object[] { a, b });
    
        if(Cache.Contains(cacheKey))
        {
            return Cache.Retrieve<int>(cacheKey);
        }
        
        int result = a + b;
        
        Cache.Store(cacheKey, result);
        
        return result;
    }

# How to use

  * Install MethodCache.Fody via Nuget
  * Add a CacheAttribute to your Solution to decorate your methods or classes
  * Add an ICache Interface
  * Create an ICache Implementation (MemCache, FileCache, DBCache, ...)

## Example

CacheAttribute

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheAttribute : Attribute
    {
    }

ICacheInterface

    public interface ICache
    {
        bool Contains(string key);

        T Retrieve<T>(string key);

        void Store(string key, object data);
    }
    
DictionaryCache (ICache memory implementation)

    public class DictionaryCache : ICache
    {
        public DictionaryCache()
        {
            Storage = new Dictionary<string, object>();
        }

        private Dictionary<string, object> Storage { get; set; }

        public bool Contains(string key)
        {
            return Storage.ContainsKey(key);
        }

        public T Retrieve<T>(string key)
        {
            return (T)Storage[key];
        }

        public void Store(string key, object data)
        {
            Storage[key] = data;
        }
    }

Now all the preparation is done and you can start with the real work. The classes you want to cache must contain an ICache Getter (can also be inherited from a baseclass). Let's start decorating ...

    // Mark the class to enable caching of every method ...

    [Cache]
    public class ClassToCache
    {
        public ClassToCache()
        {
            // Consider using constructor or property injection instead
            Cache = new DictionaryCache();
        }

        private ICache Cache { get; set; }

        // This method will be cached
        public int Add(int a, int b)
        {
            return a + b;
        }
        
        // This method will be cached too
        public string Concat(string a, string b)
        {
            return a + b;
        }   
    }
    
    // or mark the methods you want to cache explicitly.
    
    public class ClassToCache
    {
        public ClassToCache(ICache cache)
        {
            // Consider using constructor or property injection instead
            Cache = new DictionaryCache();
        }

        private ICache Cache { get; set; }

        [Cache] // Only this method will be cached
        public int Add(int a, int b)
        {
            return a + b;
        }

        public string Concat(string a, string b)
        {
            return a + b;
        }
    }

... and let MethodCache do the rest.
