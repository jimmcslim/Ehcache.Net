using System.IO;

namespace AgileWallaby.Ehcache
{
    internal interface IMetadataSerializationService
    {
        CacheMetadata GetCacheMetadata(Stream str);
    }
}