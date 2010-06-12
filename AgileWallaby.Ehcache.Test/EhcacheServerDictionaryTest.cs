using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileWallaby.Ehcache.Test
{
    [TestClass]
    public class EhcacheServerDictionaryTest
    {
        private Uri endpoint = new Uri("http://nebuchadnezzar.local:8080/ehcache/rest");

        [TestMethod]
        public void Can_Get_And_Set_Values_On_Server()
        {
            var dictionary = (IDictionary)new EhcacheServerDictionary(endpoint, "sampleCache1");
            dictionary.Clear();

            dictionary["testKey"] = "testValue";

            Assert.AreEqual("testValue", dictionary["testKey"]);

            dictionary = new EhcacheServerDictionary(endpoint, "sampleCache1");
            Assert.AreEqual("testValue", dictionary["testKey"]);
        }

        [TestMethod]
        public void Can_Get_Count()
        {
            var dictionary1 = (IDictionary) new EhcacheServerDictionary(endpoint, "sampleCache1");
            dictionary1.Clear();

            Assert.AreEqual(0, dictionary1.Count);
            dictionary1["testKey1"] = "testValue1";
            Assert.AreEqual(1, dictionary1.Count);
            dictionary1["testKey2"] = "testValue2";
            Assert.AreEqual(2, dictionary1.Count);

            var dictionary2 = new EhcacheServerDictionary(endpoint, "sampleCache2");
            dictionary2.Clear();

            dictionary2["testKey1"] = "testValue1";
            Assert.AreEqual(2, dictionary1.Count);
            Assert.AreEqual(1, dictionary2.Count);
        }

        [TestMethod]
        public void Can_Remove_And_Clear()
        {
            var dictionary = new EhcacheServerDictionary(endpoint, "sampleCache1");
            dictionary.Clear();

            dictionary["testKey1"] = "testValue";
            dictionary["testKey2"] = "testValue";
            dictionary["testKey3"] = "testValue";

            Assert.AreEqual(3, dictionary.Count);
            dictionary.Remove("testKey1");
            Assert.AreEqual(2, dictionary.Count);
            dictionary.Clear();
            Assert.AreEqual(0, dictionary.Count);
        }

        [TestMethod]
        public void Cannot_Iterate_Through_Keys_As_Not_Supported_By_Ehcache_Server()
        {
            var dictionary = new EhcacheServerDictionary(endpoint, "sampleCache1");
            
            try
            {
                var keys = dictionary.Keys;
                Assert.Fail("Should have thrown a NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // This is to be expected.
            }
        }

        [TestMethod]
        public void Cannot_Iterate_Through_Values_As_Not_Supported_By_Ehcache_Server()
        {
            var dictionary = new EhcacheServerDictionary(endpoint, "sampleCache1");

            try
            {
                var values = dictionary.Values;
                Assert.Fail("Should have thrown a NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // This is to be expected.
            }
        }

        [TestMethod]
        public void Cannot_Enumerate_As_Not_Supported_By_Ehcache_Server()
        {
            var dictionary = new EhcacheServerDictionary(endpoint, "sampleCache1");
            try
            {
                dictionary.GetEnumerator();
                Assert.Fail("Should have thrown a NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // This is to be expected.
            }
        }
    }
}
