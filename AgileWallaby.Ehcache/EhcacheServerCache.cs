#region License
/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Ehcache .Net Wrapper..
 *
 * The Initial Developer of the Original Code is
 * James Webster.
 * Portions created by the Initial Developer are Copyright (C) 2010
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * ***** END LICENSE BLOCK ***** */
#endregion License

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
            serverRequest = new EhcacheServerRequest(endpoint, defaultCache, new XmlMetadataSerializationService());
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
            string contentType;
            return GetElement(regionName, key, out contentType);
        }

        public override CacheItem GetCacheItem(string key, string regionName)
        {
            string contentType;
            var value = GetElement(regionName, key, out contentType);
            return new CacheItem(key, value,  regionName == null ? defaultCache : regionName);
        }

        public EhcacheItem<T> GetCacheItem<T>(string key, string regionName)
        {
            string contentType;
            var value = GetElement(regionName, key, out contentType);
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

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
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

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
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
            get
            {
                string contentType;
                return GetElement(defaultCache, key, out contentType);
            }
            set { PutElement(defaultCache, key, value.ToString(), "text/plain"); }
        }

        private void PutElement(string regionName, string key, string serializedValue, string contentType, int? timeToLive = null)
        {
            serverRequest.PutElement(regionName, key, serializedValue, contentType, timeToLive);
        }

        private object GetElement(string regionName, string key, out string contentType)
        {
            return serverRequest.GetElement(regionName, key, out contentType);
        }
    }
}
