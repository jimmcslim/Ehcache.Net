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
using System.IO;
using System.Net;

namespace AgileWallaby.Ehcache
{
    internal class EhcacheServerRequest : IEhcacheServerRequest
    {
        private readonly Uri endpoint;
        private readonly IMetadataSerializationService serializerService;
        private readonly string defaultCache;
        
        internal EhcacheServerRequest(Uri endpoint, string defaultCache, IMetadataSerializationService serializerService)
        {
            this.endpoint = endpoint;
            this.defaultCache = defaultCache;
            this.serializerService = serializerService;
            Timeout = 10*1000;
        }

        public int Timeout { get; set; }

        public int GetCount(string cache)
        {
            var req = CreateWebRequestForCache("GET", cache);
            var resp = req.GetResponse();
            using (var respStream = resp.GetResponseStream())
            {
                CacheMetadata cacheMetadata = serializerService.GetCacheMetadata(respStream);
                return cacheMetadata.Statistics.Size;
            }
        }

        public bool Contains(string cache, string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var req = CreateWebRequestForCacheElement("HEAD", cache, key);
            WebResponse resp = null;
            try
            {
                req.ContentLength = 0;
                resp = req.GetResponse();
                resp.GetResponseStream().Close();
                return true;
            }
            catch (WebException e)
            {
                if (IsResponseExceptionHttp404NotFound(e))
                {
                    return false;
                }
                throw e;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
        }

        public string GetElement(string cache, string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            var request = CreateWebRequestForCacheElement("GET", cache, key);
            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)request.GetResponse();

                //TODO: Check if the key is there.

                using (var str = resp.GetResponseStream())
                using (var sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                if (IsResponseExceptionHttp404NotFound(e))
                {
                    throw new KeyNotFoundException(string.Format("Key {0} is not present in cache {1}.", key, cache));
                }
                throw e;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
        }

        public void PutElement(string cache, string key, string serializedValue, string contentType, int? timeToLive = null)
        {
            var request = CreateWebRequestForCacheElement("PUT", cache, key);
            if (timeToLive != null)
            {
                request.Headers.Add("ehcacheTimeToLive", timeToLive.ToString());
            }

            request.ContentType = contentType;
            request.ContentLength = serializedValue.Length;
            using (var stream = request.GetRequestStream())
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(serializedValue);
            }

            try
            {
                request.GetResponse().GetResponseStream().Close();
            }
            catch (WebException e)
            {
                if (IsResponseExceptionHttp404NotFound(e))
                {
                    var ex = new ArgumentOutOfRangeException("cache", cache,
                        string.Format("The cache server could not respond to the Uri {0} as there is no cache {1} configured.", request.RequestUri, cache));
                    ex.Data["Uri"] = request.RequestUri;
                    throw ex;
                }
                throw e;
            }
        }

        public object Remove(string cache, string key)
        {
            var req = CreateWebRequestForCacheElement("DELETE", cache, key);
            req.ContentLength = 0;
            var resp = (HttpWebResponse)req.GetResponse();
            resp.GetResponseStream().Close();
            if (resp.StatusCode != HttpStatusCode.NoContent)
            {
                throw new ApplicationException(
                    string.Format("Unexpected response from EhcacheServer; expected Http Status Code 201 (No Content) but was {0}",
                    resp.StatusCode));
            }

            // TODO: If the key DID exist before it was removed, we could potentially return its previous value.
            return null;
        }

        private bool IsResponseExceptionHttp404NotFound(WebException e)
        {
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
                var resp = (HttpWebResponse)e.Response;
                return resp.StatusCode == HttpStatusCode.NotFound;
            }
            return false;
        }

        private HttpWebRequest CreateWebRequestForCacheElement(string method, string cache, string key)
        {
            cache = cache ?? defaultCache;
            var uri = new Uri(endpoint + "/" + cache + "/" + key);
            var req = WebRequest.Create(uri);
            req.Method = method;
            req.Timeout = Timeout;
            return (HttpWebRequest)req;
        }

        private HttpWebRequest CreateWebRequestForCache(string method, string cache)
        {
            cache = cache ?? defaultCache;
            var uri = new Uri(endpoint + "/" + cache);
            var req = WebRequest.Create(uri);
            req.Method = method;
            req.Timeout = Timeout;
            return (HttpWebRequest)req;
        }
    }
}
