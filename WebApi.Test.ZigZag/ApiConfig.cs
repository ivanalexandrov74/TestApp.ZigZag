namespace ZigZag.Test
{
    public sealed class ApiConfig : BaseConfigMgr
    {
        public ApiConfig(IConfigurationRoot configuration) : base(configuration) { }


        public string mongoDbConnectionString => Get<string>("mongoDbConnectionString", true);

        public string defaultUserName => Get<string>("defaultUserName", false);

        public string defaultUserPassword => Get<string>("defaultUserPassword");

        public string externalApiUrl => Get<string>("externalApiUrl");

        public TimeSpan externalApiDataLifetime => GetJson<TimeSpan>("externalApiDataLifetime");

        public string graphQlServiceUrl => Get<string>("graphQlServiceUrl", false);

        public int accessTokenCacheCapacity => Get<int>("accessTokenCacheCapacity", false);

        public int susspiciousTokenUidCacheCapacity => Get<int>("susspiciousTokenUidCacheCapacity", false);
    }
}
