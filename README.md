NServiceBus.MongoDB
==
MongoDB persistence for NServicBus 5.x

### Build Status
[![Build status](https://ci.appveyor.com/api/projects/status/49hk227un4haesop)](https://ci.appveyor.com/project/sbmako/nservicebus-mongodb)

### Installation
* Get the source and build locally

or

* Install the [`NServiceBus.MongoDB`](https://www.nuget.org/packages/NServiceBus.MongoDB/) NuGet package using the Visual Studio NuGet Package Manager

### Configuration
After adding a reference to it from your project, specify `MongoDBPersistence` to be used for persistence.

```csharp
using NServiceBus;
using NServiceBus.MongoDB;

/// <summary>
/// The endpoint config.
/// </summary>
public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
{
  public void Customize(BusConfiguration configuration)
  {
    configuration.UsePersistence<MongoDBPersistence>()
        .SetDatabaseName("MyDatabase");
  }
}
```

This base configuration connects using  `NServiceBus.Persistence` connection string in the app.config and `MyDatabase` as the database.

```xml
<connectionStrings>
  <add name="NServiceBus.Persistence" connectionString="mongodb://localhost:27017" />
</connectionStrings>
  ```
Alternatively, you can specify the connection string name to use as follows:

```csharp
using NServiceBus;
using NServiceBus.MongoDB;

/// <summary>
/// The endpoint config.
/// </summary>
public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
{
  public void Customize(BusConfiguration configuration)
  {
    configuration.UsePersistence<MongoDBPersistence>()
        .SetConnectionStringName("My.ConnectionString")
        .SetDatabaseName("MyDatabase");
  }
}
```
or specify the connection string to use:

```csharp
using NServiceBus;
using NServiceBus.MongoDB;

/// <summary>
/// The endpoint config.
/// </summary>
public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
{
  public void Customize(BusConfiguration configuration)
  {
    configuration.UsePersistence<MongoDBPersistence>()
        .SetConnectionString("mongodb://localhost:27017")
        .SetDatabaseName("MyDatabase");
  }
}
```

### DataBus Configuration
To configure NServiceBus to use MongoDB GridFS as the persistence for DataBus use the following configuration.

```csharp
using NServiceBus;
using NServiceBus.MongoDB;

/// <summary>
/// The endpoint config.
/// </summary>
public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
{
  public void Customize(BusConfiguration configuration)
  {
    configuration.UsePersistence<MongoDBPersistence>();
    configuration.UseDataBus<MongoDBDataBus>();
  }
}
```

### Sagas
Saga data needs to be defined the normal way NSB requires with the additional interface `IHaveDocumentVersion` to work appropriatly with `NServiceBus.MongoDB`.  All this interface adds is a version property.  Alternatively you can just inherit from ContainMongoSagaData as follows:

```csharp
using NServiceBus.MongoDB;
using NServiceBus.Saga;

public class MySagaData : ContainMongoSagaData
{
    [Unique]
    public string SomeId { get; set; }

    public int Count { get; set; }
}
```
### Sample
See https://github.com/sbmako/NServiceBus.MongoDB/tree/master/src/Sample
