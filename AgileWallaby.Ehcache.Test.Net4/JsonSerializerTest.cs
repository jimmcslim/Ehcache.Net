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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileWallaby.Ehcache.Test
{
    [TestClass]
    public class JsonSerializerTest
    {
        [TestMethod]
        public void Content_Type_Is_Application_Json()
        {
            var ser = (ISerializer)new JsonSerializer();
            Assert.AreEqual("application/json", ser.ContentType);
        }

        [TestMethod]
        public void Can_Serialize_And_Deserialize_DataContract_Classes()
        {
            var ser = new JsonSerializer();
            var str = new MemoryStream();
            var contract = new TestContract {PropertyOne = "propertyOne", PropertyTwo = 42};
            ser.Serialize(str, contract);

            str.Seek(0, SeekOrigin.Begin);
            var resultContract = ser.Deserialize<TestContract>(str);
            Assert.AreEqual(contract.PropertyOne, resultContract.PropertyOne);
            Assert.AreEqual(contract.PropertyTwo, resultContract.PropertyTwo);
        }

        [TestMethod]
        public void Cannot_Deserialize_Without_Type_Parameter()
        {
            var ser = new JsonSerializer();
            var str = new MemoryStream();
            var contract = new TestContract { PropertyOne = "propertyOne", PropertyTwo = 42 };
            ser.Serialize(str, contract);

            str.Seek(0, SeekOrigin.Begin);
            try
            {
                ser.Deserialize(str);
                Assert.Fail("Should have thrown an exception.");
            }
            catch (NotImplementedException)
            {
                // Expected result.
            }
        }

        [TestMethod]
        public void Cannot_Serialize_If_Not_A_DataContract()
        {
            var ser = new JsonSerializer();
            var str = new MemoryStream();
            try
            {
                ser.Serialize(str, new object());
                Assert.Fail("Should have thrown an exception.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("System.Object is not a DataContract, cannot serialize to JSON.", e.Message);
            }
        }

        [TestMethod]
        public void Cannot_Deserialize_If_Not_A_DataContract()
        {
            var ser = new JsonSerializer();
            var str = new MemoryStream();
            try
            {
                ser.Deserialize<object>(str);
                Assert.Fail("Should have thrown an exception");                
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("System.Object is not a DataContract, cannot deserialize from JSON.", e.Message);
            }
        }
    }
}