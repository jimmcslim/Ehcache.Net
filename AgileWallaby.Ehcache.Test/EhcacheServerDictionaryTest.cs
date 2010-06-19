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
