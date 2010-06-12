using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AgileWallaby.Ehcache
{
    internal class XmlMetadataSerializationService: IMetadataSerializationService
    {
        public CacheMetadata GetCacheMetadata(Stream str)
        {
            XmlSerializer ser = new XmlSerializer(typeof (CacheMetadata));
            return (CacheMetadata) ser.Deserialize(str);
        }
    }
}
