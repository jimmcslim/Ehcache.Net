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

namespace AgileWallaby.Ehcache.Test.Silverlight
{
    [TestClass]
    public class ConnectivityTest
    {
        private Uri endpoint = new Uri("http://nebuchadnezzar.local:8080/ehcache/rest");

        [TestMethod]
        public void Can_Get_Count()
        {
            var dictionary = (IDictionary)new EhcacheServerDictionary(endpoint, "sampleCache1");

            Assert.AreEqual(0, dictionary.Count);
            dictionary["testKey1"] = "testValue1";
            Assert.AreEqual(1, dictionary.Count);
            dictionary["testKey2"] = "testValue2";
            Assert.AreEqual(2, dictionary.Count);

            var dictionary2 = new EhcacheServerDictionary(endpoint, "sampleCache2");
            dictionary2.Clear();

            dictionary2["testKey1"] = "testValue1";
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual(1, dictionary2.Count);
        }
    }
}