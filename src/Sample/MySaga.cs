// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySaga.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the MySaga type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NServiceBus.Logging;
using NServiceBus.Saga;
    
public class MySaga:Saga<MySagaData>, IAmStartedByMessages<MyMessage>
{
    static ILog logger = LogManager.GetLogger(typeof(MySaga));

    public void Handle(MyMessage message)
    {
        logger.Info("Hello from MySaga"); 
    }

    ////protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    ////{
    ////    mapper.ConfigureMapping<MyMessage>(m => m.SomeId)
    ////        .ToSaga(s => s.SomeId);
    ////}
}