namespace AgileWallaby.Ehcache
{
    internal interface IEhcacheServerRequest
    {
        string GetElement(string cache, string key);
        void PutElement(string cache, string key, string serializedValue, string contentType, int? timeToLive = null);
        int Timeout { get; set; }
        int GetCount(string cache);
        bool Contains(string cache, string key);
        object Remove(string cache, string key);
    }
}