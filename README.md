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
### Sample

See https://github.com/sbmako/NServiceBus.MongoDB/tree/master/src/Sample
