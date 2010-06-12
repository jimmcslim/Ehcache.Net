namespace AgileWallaby.Ehcache
{
    internal class SerializerServiceFactory
    {
        internal static IMetadataSerializationService GetSerializerService()
        {
            return new XmlMetadataSerializationService();       
        }
    }
}
