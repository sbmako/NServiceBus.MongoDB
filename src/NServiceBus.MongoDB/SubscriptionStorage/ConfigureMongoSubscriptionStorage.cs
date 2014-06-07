namespace NServiceBus.MongoDB.SubscriptionStorage
{
    public static class ConfigureMongoSubscriptionStorage
    {
        public static Configure RavenSubscriptionStorage(this Configure config)
        {
            return config;
        }
    }
}
