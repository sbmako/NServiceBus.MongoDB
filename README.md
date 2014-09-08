NServiceBus.MongoDB
===================

MongoDB persistence for NServicBus

Build Status
-

[![Build status](https://ci.appveyor.com/api/projects/status/49hk227un4haesop)](https://ci.appveyor.com/project/sbmako/nservicebus-mongodb)

Installation
-
* Get the source and build locally

 	 or
 	  
* Install the [`NServiceBus.MongoDB`](https://www.nuget.org/packages/NServiceBus.MongoDB/) NuGet package using the Visual Studio NuGet Package Manager

Getting Started
-
### ConfigureMongoPersistence Members

|Name | Description |
|:-----|:-------------|
| `.MongoPersistence(Configure config)` | Default MongoDB persistence using `localhost:27172`. |
| `.MongoPersistence(Configure config, string connectionString)` | Configures persistence using the given connection string.  |
| `.MongoPersistence(Configure config, string connectionStringName, string databaseName)` | Configures persistence using the given connection string and uses the provided database. |
| `.MongoPersistence(this Configure config, Func<string> getConnectionString)` | Configures persistence using the connection string returned from `Func<string>`. |
| `.MongoPersistence(this Configure config, Func<string> getConnectionString, string databaseName)`. | Configures persistence using the connection string returned from `Func<string>` and uses the provided database. |

### ConfigureMongoSagaPersister Members

|Name | Description |
|:-----|:-------------|
| `.MongoSagaPersister(Configure config)` | Enables MongoDB saga persistence. |

### ConfigureMongoSubscriptionStorage Members

|Name | Description |
|:-----|:-------------|
| `.MongoSubscriptionStorage(Configure config)` | Enables MongoDB subscription storage. |

### ConfigureMongoTimeoutPersister Members

|Name | Description |
|:-----|:-------------|
| `.MongoTimeoutPersister(Configure config)` | Enables MongoDB timeout persister. |

#### Example Configuration

```csharp
namespace Sample
{
    using NServiceBus;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.SubscriptionStorage;
    using NServiceBus.MongoDB.TimeoutPersister;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
                     .DefaultBuilder()
                     .UnicastBus()
                     .MongoSagaPersister()
                     .MongoSubscriptionStorage()
                     .MongoTimeoutPersister();
        }
    }
}
```
### Sagas

``` TBD ```

### Sample

See https://github.com/sbmako/NServiceBus.MongoDB/tree/master/src/Sample
