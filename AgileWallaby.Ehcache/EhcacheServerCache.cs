using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace AgileWallaby.Ehcache
{
    public class EhcacheServerCache: ObjectCache
    {
        public const string RemoveAllKey = "*";

        private readonly IEhcacheServerRequest serverRequest;
        private readonly string defaultCache;

        public EhcacheServerCache(Uri endpoint, string defaultCache = null)
        {
            serverRequest = new EhcacheServerRequest(endpoint, defaultCache, SerializerServiceFactory.GetSerializerService());
            this.defaultCache = defaultCache;
            Timeout = 10*1000;
        }

        /// <summary>
        /// The timeout period in milliseconds that the client will grant the Ehcache Server to respond
        /// to requests.
        /// </summary>
        public int Timeout { get; set; }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName)
        {
            throw new NotImplementedException("Not implemented as Ehcache Server does not provide a way of listening to cache changes.");
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException("Not implemented as Ehcache Server does not provide a way of listing all keys in a cache.");
        }

        public override bool Contains(string key, string cache)
        {
            return serverRequest.Contains(cache, key);
        }

        public override CacheItem AddOrGetExisting(CacheItem item, CacheItemPolicy policy)
        {
            if (Contains(item.Key, item.RegionName))
            {
                var value = Get(item.Key, item.RegionName);
                return new CacheItem(item.Key, value, item.RegionName);
            }
            PutElement(item.RegionName, item.Key, item.Value.ToString(), "text/plain");
            return null;
        }

        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName)
        {
            var item = new CacheItem(key, value, regionName);
            var policy = new CacheItemPolicy
                             {
                                 AbsoluteExpiration = absoluteExpiration
                             };
            return AddOrGetExisting(item, policy);
        }

        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName)
        {
            var item = new CacheItem(key, value, regionName);
            return AddOrGetExisting(item, policy);
        }

        public override object Get(string key, string regionName)
        {
            return GetElement(regionName, key);
        }

        public override CacheItem GetCacheItem(string key, string regionName)
        {
            var value = GetElement(regionName, key);
            return new CacheItem(key, value,  regionName == null ? defaultCache : regionName);
        }

        public EhcacheItem<T> GetCacheItem<T>(string key, string regionName)
        {
            var value = GetElement(regionName, key);
            return new EhcacheItem<T>(key, (T)value, regionName);
        }

        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName)
        {
            var item = new CacheItem(key, value, regionName);
            var policy = new CacheItemPolicy
                             {
                                 AbsoluteExpiration = absoluteExpiration
                             };
            Set(item, policy);
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName)
        {
            var item = new CacheItem(key, value);
            Set(item, policy);
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            PutElement(item.RegionName, item.Key, item.Value.ToString(), null);
        }

        public void Set<T>(EhcacheItem<T> item, CacheItemPolicy policy)
        {
            
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName)
        {
            var results = new Dictionary<string, object>();
            foreach (var key in keys)
            {
                try
                {
                    var result = Get(key, regionName);
                    results[key] = result;
                }
                catch (KeyNotFoundException)
                {
                    // Deliberately ignored, we will not add a value to the result dictionary if the key does not exist.
                }
            }
            return results;
        }

        public override object Remove(string key, string regionName)
        {
            return serverRequest.Remove(regionName, key);
        }

        /// <summary>
        /// Convenience method for removing all keys from the default cache.
        /// </summary>
        public void RemoveAll()
        {
            RemoveAll(defaultCache);    
        }

        /// <summary>
        /// Convenience method for removing all keys from the named cache.
        /// </summary>
        /// <param name="regionName"></param>
        public void RemoveAll(string regionName)
        {
            Remove(RemoveAllKey, regionName);
        }

        public override long GetCount(string regionName)
        {
            return serverRequest.GetCount(regionName);
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { return defaultCache; }
        }

        public override object this[string key]
        {
            get { return GetElement(defaultCache, key); }
            set { PutElement(defaultCache, key, value.ToString(), "text/plain"); }
        }

        private void PutElement(string regionName, string key, string serializedValue, string contentType, int? timeToLive = null)
        {
            serverRequest.PutElement(regionName, key, serializedValue, contentType, timeToLive);
        }

        private object GetElement(string regionName, string key)
        {
            return serverRequest.GetElement(regionName, key);
        }
    }
}
