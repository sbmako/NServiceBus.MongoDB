// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NServiceBus;

class Program
{
    ////static IStartableBus bus;

    static void Main()
    {
        ////using (var documentStore = new DocumentStore().Initialize())
        ////{
        ////    var configure = Configure.With()
        ////        .DefaultBuilder()
        ////        .UsePersistence<RavenDB>(_ => _.SetDefaultDocumentStore(documentStore))
        ////        .Serialization.Json()
        ////        .Features(_ => _.Enable<Sagas>())
        ////        ;

        ////    configure.EnableInstallers();
        ////    bus = configure.CreateBus();
        ////    bus.Start();

        ////    bus.SendLocal(new MyMessage
        ////    {
        ////        SomeId = Guid.NewGuid()
        ////    });
        ////    Console.ReadLine();
        ////}
    }
}