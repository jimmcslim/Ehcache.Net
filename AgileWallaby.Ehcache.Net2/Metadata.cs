using System.Xml.Serialization;

namespace AgileWallaby.Ehcache
{
    [XmlRoot("caches")]
    public class CacheManagerGetResponse
    {
        [XmlElement("cache")]
        public CacheMetadata[] Caches { get; set; }
    }

    [XmlRoot("cache")]
    public class CacheMetadata
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("cacheConfiguration")]
        public CacheConfiguration Configuration { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("statistics")]
        public CacheStatistics Statistics { get; set; }

        [XmlElement("uri")]
        public string Uri { get; set; }
    }

    public class CacheConfiguration
    {
        [XmlElement("clearOnFlush")]
        public bool ClearOnFlash { get; set; }

        [XmlElement("diskAccessStripes")]
        public int DiskAccessStripes { get; set; }

        [XmlElement("diskExpiryThreadIntervalSeconds")]
        public int DiskExpiryThreadIntervalSeconds { get; set; }

        [XmlElement("diskPersistent")]
        public bool DiskPersistent { get; set; }

        [XmlElement("diskSpoolBufferSizeMB")]
        public int DiskSpoolBufferSizeMb { get; set; }

        [XmlElement("diskStorePath")]
        public string DiskStorePath { get; set; }

        [XmlElement("eternal")]
        public bool Eternal { get; set; }

        [XmlElement("loggingEnabled")]
        public bool LoggingEnabled { get; set; }

        [XmlElement("maxElementsInMemory")]
        public int MaxElementsInMemory { get; set; }

        [XmlElement("maxElementsOnDisk")]
        public int MaxElementsOnDisk { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("overflowToDisk")]
        public bool OverflowToDisk { get; set; }

        [XmlElement("statistics")]
        public bool Statistics { get; set; }

        [XmlElement("timeToIdleSeconds")]
        public int TimeToIdleSeconds { get; set; }

        [XmlElement("timeToLiveSeconds")]
        public int TimeToLiveSeconds { get; set; }
    }

    public class CacheStatistics
    {
        [XmlElement("averageGetTime")]
        public float AverageGetTime { get; set; }

        [XmlElement("cacheHits")]
        public int CacheHits { get; set; }

        [XmlElement("diskStoreSize")]
        public int DiskStoreSize { get; set; }

        [XmlElement("evictionCount")]
        public int EvictionCount { get; set; }

        [XmlElement("inMemoryHits")]
        public int InMemoryHits { get; set; }

        [XmlElement("memoryStoreSize")]
        public int MemoryStoreSize { get; set; }

        [XmlElement("misses")]
        public int Misses { get; set; }

        [XmlElement("onDiskHits")]
        public int OnDiskHits { get; set; }

        [XmlElement("size")]
        public int Size { get; set; }

        [XmlElement("statisticsAccuracy")]
        public string StatisticsAccuracy { get; set; }
    }

}
