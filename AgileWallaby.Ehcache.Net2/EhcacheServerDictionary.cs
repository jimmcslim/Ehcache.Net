using System;
using System.Collections;
using System.Collections.Generic;

namespace AgileWallaby.Ehcache
{
    public class EhcacheServerDictionary: IDictionary
    {
        private readonly IEhcacheServerRequest serverRequest;
        private readonly string cache;

        public EhcacheServerDictionary(Uri endpoint, string cache)
        {
            serverRequest = new EhcacheServerRequest(endpoint, cache, SerializerServiceFactory.GetSerializerService());
            this.cache = cache;
        }

        public bool Contains(object keyObj)
        {
            var keyStr = RejectKeyIfNotString(keyObj);
            return serverRequest.Contains(cache, keyStr);
        }

        public void Add(object keyObj, object value)
        {
            var keyStr = RejectKeyIfNotString(keyObj);
            serverRequest.PutElement(cache, keyStr, value.ToString(), "text/plain");
        }

        public void Clear()
        {
            serverRequest.Remove(cache, "*");
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException("Not implemented as Ehcache Server does not provide a way of listing all keys in a cache.");
        }

        public void Remove(object keyObj)
        {
            var keyStr = RejectKeyIfNotString(keyObj);
            serverRequest.Remove(cache, keyStr);
        }

        public object this[object keyObj]
        {
            get
            {
                var keyStr = RejectKeyIfNotString(keyObj);
                return serverRequest.GetElement(cache, keyStr);
            }
            set
            {
                var keyStr = RejectKeyIfNotString(keyObj);
                serverRequest.PutElement(cache, keyStr, value.ToString(), "text/plain");
            }
        }

        public ICollection Keys
        {
            get { throw new NotImplementedException("Not implemented as Ehcache Server does not provide a way of listing all keys in a cache."); }
        }

        public ICollection Values
        {
            get { throw new NotImplementedException("Not implemented as Ehcache Server does not provide a way of listing all values in a cache."); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException("Not implemented as Ehcache Server does not provide a way of listing all keys/values in a cache.");
        }

        public int Count
        {
            get
            {
                return serverRequest.GetCount(cache);
            }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        private string RejectKeyIfNotString(object keyObj)
        {
            if (!(keyObj is string))
            {
                throw new ArgumentException("Only keys of type string can be provided.", "key");
            }
            return (string)keyObj;
        }
    }
}
