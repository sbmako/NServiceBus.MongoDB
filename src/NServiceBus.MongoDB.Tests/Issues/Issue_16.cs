// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Issue_16.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 SharkByte Software
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.Issues
{
    using System;

    using CategoryTraits.Xunit2;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Persistence;
    using NServiceBus.Sagas;

    using Xunit;

    public class Issue16
    {
        [Theory]
        [IntegrationTest]
        [AutoDatabase]
        public void SaveSagaData(
            MongoSagaPersister sut,
            DeviceCommandSagaState state,
            SagaCorrelationProperty correlationProperty,
            SynchronizedStorageSession session,
            ContextBag context)
        {
            sut.Save(state, correlationProperty, session, context);
        }

        [Theory]
        [IntegrationTest]
        [AutoDatabase]
        public void UpdateSagaData(
            MongoSagaPersister sut,
            DeviceCommandSagaState state,
            SagaCorrelationProperty correlationProperty,
            SynchronizedStorageSession session,
            ContextBag context)
        {
            sut.Save(state, correlationProperty, session, context).Wait();

            sut.Update(state, session, context).Wait();
        }
    }

    public sealed class DeviceCommandSagaState : IContainSagaData, IHaveDocumentVersion
    {
        public string SagaKey { get; set; }

        //IContainSagaData properties
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        //IHaveDocumentVersion properties
        public int DocumentVersion { get; set; }
    }
}
