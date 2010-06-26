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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileWallaby.Ehcache.Test
{
    [TestClass]
    public class EhcacheServerCacheTest
    {
        private readonly Uri ehcacheUri = new Uri("http://nebuchadnezzar.local:8080/ehcache/rest");

        [TestMethod]
        public void Name_Of_Cache_Is_Equal_To_Default_Cache()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            Assert.AreEqual("sampleCache1", cache.Name);

            cache = new EhcacheServerCache(ehcacheUri, "sampleCache2");
            Assert.AreEqual("sampleCache2", cache.Name);
        }

        [TestMethod]
        public void Can_Put_And_Get_Into_Default_Cache_Via_Indexer_Or_Get_Method()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            cache["testKey2"] = "testValue";
            Assert.AreEqual("testValue", cache["testKey2"]);
            Assert.AreEqual("testValue", cache.Get("testKey2", null));
            Assert.AreEqual("testValue", cache.Get("testKey2", "sampleCache1"));
        }

        [TestMethod]
        public void Can_Get_Cache_Item()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            cache["testKey1"] = "testValue";
            var cacheItem = cache.GetCacheItem("testKey1", null);

            Assert.AreEqual("testKey1", cacheItem.Key);
            Assert.AreEqual("testValue", cacheItem.Value);
            Assert.AreEqual("sampleCache1", cacheItem.RegionName);

            cache.AddOrGetExisting("testKey2", "anotherValue", null, "sampleCache2");
            cacheItem = cache.GetCacheItem("testKey2", "sampleCache2");

            Assert.AreEqual("testKey2", cacheItem.Key);
            Assert.AreEqual("anotherValue", cacheItem.Value);
            Assert.AreEqual("sampleCache2", cacheItem.RegionName);
        }

        [TestMethod]
        public void Can_Check_Size_Of_Cache()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            Assert.AreEqual(0, cache.GetCount(null));

            cache["testKey1"] = "aTestValue";
            Assert.AreEqual(1, cache.GetCount(null));
            Assert.AreEqual(1, cache.GetCount("sampleCache1"));
            Assert.AreEqual(0, cache.GetCount("sampleCache2"));
            cache["testKey2"] = "anotherTestValue";
            Assert.AreEqual(2, cache.GetCount(null));

            cache.Add("testKey1", "testValue", null, "sampleCache2");
            Assert.AreEqual(1, cache.GetCount("sampleCache2"));
        }

        [TestMethod]
        public void Can_Check_If_The_Cache_Contains_A_Key()
        {
            Console.WriteLine("Cleaning up before test");
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");

            cache.RemoveAll();
            cache.RemoveAll("sampleCache2");

            Assert.IsFalse(cache.Contains("thisKey", null));

            cache["thisKey"] = "aTestValue";
            Assert.IsTrue(cache.Contains("thisKey", null));
            Assert.IsFalse(cache.Contains("thisKey", "sampleCache2"));

            cache.Add("thisKey", "aTestValue", null, "sampleCache2");
            Assert.IsTrue(cache.Contains("thisKey", "sampleCache2"));

            Console.WriteLine("Cleaning up after test");

            cache.Remove("thisKey", null);
            cache.Remove("thisKey", "sampleCache2");
        }

        [TestMethod]
        public void Can_Result_Multiple_Keys_Including_Non_Extant()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            cache.RemoveAll();

            cache["key1"] = "apple";
            cache["key2"] = "pear";
            cache["key3"] = "orange";

            var keys = new string[] {"key1", "key2", "key3", "key4"};
            var results = cache.GetValues(keys, null);
  
            Assert.AreEqual(results.Count, 3);
            Assert.AreEqual(results["key1"], "apple");
            Assert.AreEqual(results["key2"], "pear");
            Assert.AreEqual(results["key3"], "orange");

            cache.Add("key1", "apple", null, "sampleCache2");
            cache.Add("key2", "pear", null, "sampleCache2");
            cache.Add("key3", "orange", null, "sampleCache2");

            results = cache.GetValues("sampleCache2", "key1", "key2", "key3", "key4");

            Assert.AreEqual(results.Count, 3);
            Assert.AreEqual(results["key1"], "apple");
            Assert.AreEqual(results["key2"], "pear");
            Assert.AreEqual(results["key3"], "orange");
        }

        [TestMethod]
        public void Can_Add_Cache_Items()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            cache.RemoveAll();
            cache.RemoveAll("sampleCache2");

            var item = new CacheItem("key1", "value1");
            var policy = new CacheItemPolicy();
          
            Assert.IsTrue(cache.Add(item, policy));
            Assert.IsFalse(cache.Add(item, policy));
            Assert.AreEqual(cache.GetCount(null), 1);
            Assert.AreEqual(cache.GetCount("sampleCache1"), 1);

            item.Key = "key2";
            item.Value = "value2";
            item.RegionName = "sampleCache2";

            Assert.IsTrue(cache.Add(item, policy));
            Assert.IsFalse(cache.Add(item, policy));
            Assert.AreEqual(cache.GetCount(null), 1);
            Assert.AreEqual(cache.GetCount("sampleCache2"), 1);
        }

        [TestMethod]
        public void Get_Throws_KeyNotFoundException_If_Key_Does_Not_Exist()
        {
            var cache = new EhcacheServerCache(ehcacheUri, "sampleCache1");
            cache.RemoveAll();

            try
            {
                var value = cache["testKey"];
                Assert.Fail("Should have thrown an exception.");
            }
            catch (KeyNotFoundException e)
            {
                Assert.AreEqual("Key testKey is not present in cache sampleCache1.", e.Message);
            }
            
        }
    }
}
