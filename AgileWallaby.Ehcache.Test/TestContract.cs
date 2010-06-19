using System.Runtime.Serialization;

namespace AgileWallaby.Ehcache.Test
{
    [DataContract]
    public class TestContract
    {
        [DataMember]
        public string PropertyOne { get; set; }

        [DataMember]
        public int PropertyTwo { get; set; }        
    }
}