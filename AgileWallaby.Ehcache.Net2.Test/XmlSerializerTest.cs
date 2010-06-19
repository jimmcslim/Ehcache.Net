using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileWallaby.Ehcache.Test
{
    [TestClass]
    public class XmlSerializerTest
    {
        [TestMethod]
        public void Content_Type_Is_Text_Xml()
        {
            var ser = (ISerializer) new XmlSerializer();
            Assert.AreEqual("text/xml", ser.ContentType);
        }

        [TestMethod]
        public void Can_Serialize_And_Deserialize_Xml()
        {
            var ser = new XmlSerializer();
            var testObject = new TestObject {PropertyOne = "propertyOne", PropertyTwo = 42};
            var str = new MemoryStream();
            ser.Serialize(str, testObject);

            str.Seek(0, SeekOrigin.Begin);

            var resultObject = ser.Deserialize<TestObject>(str);

            Assert.AreEqual(testObject.PropertyOne, resultObject.PropertyOne);
            Assert.AreEqual(testObject.PropertyTwo, resultObject.PropertyTwo);
        }

        [TestMethod]
        public void Cannot_Deserialize_Without_Type_Parameter()
        {
            var ser = new XmlSerializer();
            var str = new MemoryStream();
            var contract = new TestObject { PropertyOne = "propertyOne", PropertyTwo = 42 };
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
        
    }
}