using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileWallaby.Ehcache.Test
{
    [TestClass]
    public class StringSerializerTest
    {
        [TestMethod]
        public void Content_Type_Is_Text_Plain()
        {
            var ser = (ISerializer) new StringSerializer();
            Assert.AreEqual("text/plain", ser.ContentType);
        }

        [TestMethod]
        public void Serialize_To_String()
        {
            var ser = new StringSerializer();
            var value = new object();
            MemoryStream str = new MemoryStream();
            ser.Serialize(str, value);
            str.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(str);
            Assert.AreEqual(value.ToString(), sr.ReadToEnd());
        }

        [TestMethod]
        public void Deserialize_From_String_Always_Returns_String()
        {
            var ser = new StringSerializer();
            MemoryStream st = new MemoryStream();
            StreamWriter str = new StreamWriter(st);
            str.Write(true);
            str.Write(false);
            str.Write(10);
            str.Write("hello");
            str.Flush();

            st.Seek(0, SeekOrigin.Begin);
            var valueString = ser.Deserialize<string>(st);
            Assert.AreEqual("TrueFalse10hello", valueString);

            st.Seek(0, SeekOrigin.Begin);
            var valueObject = ser.Deserialize(st);
            Assert.IsInstanceOfType(valueObject, typeof(string));
            Assert.AreEqual("TrueFalse10hello", valueObject);
        }

    }
}