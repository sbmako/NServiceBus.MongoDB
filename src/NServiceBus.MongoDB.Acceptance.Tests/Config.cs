using NServiceBus;
using NServiceBus.MongoDB;

public class ConfigureMongoDBPersistence
{
    public void Configure(BusConfiguration config)
    {
        config.UsePersistence<MongoDBPersistence>()
            .SetConnectionString("mongodb://localhost:27017")
            .SetDatabaseName("AcceptanceTests");
    }
}