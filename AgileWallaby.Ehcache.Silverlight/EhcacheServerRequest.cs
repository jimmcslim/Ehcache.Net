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
            ContentTypeToSerializer = new Dictionary<string, ISerializer>();
            TypeToSerializer = new Dictionary<Type, ISerializer>();
            Timeout = 10*1000;

            DefaultSerializer = new StringSerializer();
        }

        public int Timeout { get; set; }

        public Dictionary<string, ISerializer> ContentTypeToSerializer { get; private set; }
        public Dictionary<Type, ISerializer> TypeToSerializer { get; private set; }
        public ISerializer DefaultSerializer { get; set; }


        public int GetCount(string cache)
        {
            var req = CreateWebRequestForCache("GET", cache);
            var resp = GetResponse(req);
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
                resp = GetResponse(req);
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

        public string GetElement(string cache, string key, out string contentType)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            var req = CreateWebRequestForCacheElement("GET", cache, key);
            HttpWebResponse resp = null;
            try
            {
                resp = GetResponse(req);

                //TODO: Check if the key is there.
                contentType = resp.ContentType;

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
            var req = CreateWebRequestForCacheElement("PUT", cache, key);
            if (timeToLive != null)
            {
                req.Headers["ehcacheTimeToLive"] = timeToLive.ToString();
            }

            req.ContentType = contentType;
            req.ContentLength = serializedValue.Length;
            using (var stream = GetRequestStream(req))
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(serializedValue);
            }

            try
            {
                GetResponse(req).GetResponseStream().Close();
            }
            catch (WebException e)
            {
                if (IsResponseExceptionHttp404NotFound(e))
                {
                    var ex = new ArgumentOutOfRangeException("cache",
                        string.Format("The cache server could not respond to the Uri {0} as there is no cache {1} configured.", req.RequestUri, cache));
                    ex.Data["Uri"] = req.RequestUri;
                    throw ex;
                }
                throw e;
            }
        }

        public object Remove(string cache, string key)
        {
            var req = CreateWebRequestForCacheElement("DELETE", cache, key);
            req.ContentLength = 0;
            var resp = GetResponse(req);
            resp.GetResponseStream().Close();
            if (resp.StatusCode != HttpStatusCode.NoContent)
            {
                throw new CacheServerException(
                    string.Format("Unexpected response from EhcacheServer; expected Http Status Code 201 (No Content) but was {0}",
                    resp.StatusCode));
            }

            // TODO: If the key DID exist before it was removed, we could potentially return its previous value.
            return null;
        }

        private bool IsResponseExceptionHttp404NotFound(WebException e)
        {
            if (e.Status == WebExceptionStatus.ConnectFailure)
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
            return (HttpWebRequest)req;
        }

        private HttpWebRequest CreateWebRequestForCache(string method, string cache)
        {
            cache = cache ?? defaultCache;
            var uri = new Uri(endpoint + "/" + cache);
            var req = WebRequest.Create(uri);
            req.Method = method;
            return (HttpWebRequest)req;
        }

        private HttpWebResponse GetResponse(HttpWebRequest req)
        {
            var asyncResult = req.BeginGetResponse(null, null);
            var hasNotTimedOut = asyncResult.AsyncWaitHandle.WaitOne(Timeout*1000);
            if (!hasNotTimedOut)
            {
                throw new CacheServerException("Timed out");
            }

            return (HttpWebResponse) req.EndGetResponse(asyncResult);
        }

        private Stream GetRequestStream(HttpWebRequest req)
        {
            var asyncResult = req.BeginGetRequestStream(null, null);
            var hasNotTimedOut = asyncResult.AsyncWaitHandle.WaitOne(Timeout*1000);
            if (!hasNotTimedOut)
            {
                // TODO: Not really an issue with the cache server.
                throw new CacheServerException("Could not get request stream");
            }

            return req.EndGetRequestStream(asyncResult);
        }
    }
}
