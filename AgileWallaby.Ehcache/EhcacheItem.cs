using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace AgileWallaby.Ehcache
{
    public class EhcacheItem<T>: CacheItem
    {
        public EhcacheItem(string key):base(key)
        {
            
        }

        public EhcacheItem(string key, T value)
            : base(key, value)
        {

        }

        public EhcacheItem(string key, T value, string regionName) : base(key, value, regionName)
        {
            
        }

        public T TypedValue
        {
            get
            {
                return (T)this.Value;
            }
            set
            {
                this.Value = value;
            }
        }

    }
}
