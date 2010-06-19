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