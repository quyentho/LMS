namespace TodoWeb.Service.Services.CacheService
{
    public class CacheData
    {
        public object Value { get; set; }
        public DateTime Expiration { get; set; }
        public CacheData(object cacheValue, DateTime expirationTime)
        {
            Value = cacheValue;
            Expiration = expirationTime;
        }
    }
}
