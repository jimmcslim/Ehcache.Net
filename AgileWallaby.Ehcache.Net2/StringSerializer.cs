using System;
using System.IO;

namespace AgileWallaby.Ehcache
{
    public class StringSerializer: ISerializer
    {
        public const string StringContentType = "text/plain";
    
        public string ContentType
        {
            get { return StringContentType; }
        }

        public T Deserialize<T>(Stream s) where T : class
        {
            if (!(typeof(T) == typeof(string)))
            {
                throw new ArgumentException("Cannot deserialize anything other than string.");
            }
            var sr = new StreamReader(s);
            var content = sr.ReadToEnd();
            return content as T;
        }

        public object Deserialize(Stream s)
        {
            StreamReader sr = new StreamReader(s);
            var content = sr.ReadToEnd();
            return content;            
        }

        public void Serialize<T>(Stream s, T value) where T : class
        {
            var sw = new StreamWriter(s);
            sw.Write(value.ToString());
            sw.Flush();
        }
    }
}