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
using System.Collections;

namespace AgileWallaby.Ehcache
{
    public class EhcacheServerDictionary: IDictionary
    {
        private readonly IEhcacheServerRequest serverRequest;
        private readonly string cache;

        public EhcacheServerDictionary(Uri endpoint, string defaultCache)
        {
            var serverRequest = new EhcacheServerRequest(endpoint, defaultCache, new XmlMetadataSerializationService());
            serverRequest.ContentTypeToSerializer[XmlSerializer.XmlContentType] = new XmlSerializer();
            serverRequest.ContentTypeToSerializer[StringSerializer.StringContentType] = new StringSerializer();
            this.serverRequest = serverRequest;
            this.cache = defaultCache;
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
                string contentType;
                return serverRequest.GetElement(cache, keyStr, out contentType);
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
