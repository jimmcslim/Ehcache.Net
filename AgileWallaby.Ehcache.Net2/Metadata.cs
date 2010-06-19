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
