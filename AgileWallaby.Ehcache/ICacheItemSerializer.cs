using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace AgileWallaby.Ehcache
{
    public interface ICacheItemSerializer
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string serializedValue);
    }

    public class JsonCacheItemSerializer: ICacheItemSerializer
    {
        public string Serialize<T>(T value)
        {
            var ser = new DataContractJsonSerializer(typeof (T));
            var st = new MemoryStream();
            ser.WriteObject(st, value);
            return new StreamReader(st).ReadToEnd();
        }

        public T Deserialize<T>(string serializedValue)
        {
            var ser = new DataContractJsonSerializer(typeof (T));
            var st = new MemoryStream();
            var sr = new StreamWriter(st);
            sr.Write(serializedValue);
            sr.Close();
            return (T) ser.ReadObject(st);
        }
    }

    public class XmlCacheItemSerializer: ICacheItemSerializer
    {
        public string Serialize<T>(T value)
        {
            var ser = new XmlSerializer(typeof (T));
            TextWriter tw = new StringWriter();
            ser.Serialize(tw, value);
            tw.Close();
            return tw.ToString();
        }

        public T Deserialize<T>(string serializedValue)
        {
            var ser = new XmlSerializer(typeof (T));
            using (var sr = new StringReader(serializedValue))
            {
                return (T) ser.Deserialize(sr);
            }
        }
    }
}
