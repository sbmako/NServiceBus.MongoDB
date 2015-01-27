NServiceBus.MongoDB
===================

MongoDB persistence for NServicBus 5.x

Build Status
-

[![Build status](https://ci.appveyor.com/api/projects/status/49hk227un4haesop)](https://ci.appveyor.com/project/sbmako/nservicebus-mongodb)

Installation
-
* Get the source and build locally

 	 or

* Install the [`NServiceBus.MongoDB`](https://www.nuget.org/packages/NServiceBus.MongoDB/) NuGet package using the Visual Studio NuGet Package Manager

### Configuration

This is an example of the simplest way to use MongoDB Persistence. This will enable, saga, timeout and subscription storage.
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
  }
}
```

Add connection string in app.config.
```xml
<connectionStrings>
  <add name="NServiceBus.Persistence" connectionString="mongodb://localhost:27017" />
</connectionStrings>
  ```
### Sample

See https://github.com/sbmako/NServiceBus.MongoDB/tree/master/src/Sample
