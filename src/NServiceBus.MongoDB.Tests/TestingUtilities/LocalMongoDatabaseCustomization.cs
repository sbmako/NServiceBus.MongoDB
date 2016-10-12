// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalMongoDatabaseCustomization.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2016 SharkByte Software
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
// <summary>
//   Defines the LocalMongoDatabaseCustomization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TestingUtilities
{
    using System;

    using global::MongoDB.Driver;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.TimeoutPersister;
    using NServiceBus.Sagas;

    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;

    using TimeoutData = NServiceBus.Timeout.Core.TimeoutData;

    public class LocalMongoDatabaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var client = new MongoClient(MongoPersistenceConstants.DefaultConnectionString);
            var clientAccessor = new MongoClientAccessor(client, "UnitTest");
            var databaseFactory = new MongoDatabaseFactory(clientAccessor);

            fixture.Register(() => client);
            fixture.Register(() => clientAccessor);
            fixture.Register(() => databaseFactory);
            fixture.Register(() => new ContextBag());
            fixture.Register(
                () => new SagaCorrelationProperty("NonUniqueProperty", fixture.Create("NonUniqueProperty")));

            fixture.Customize(new AutoMoqCustomization());

            fixture.Customize<TimeoutData>(
                c => c.With(t => t.OwningTimeoutManager, "UnitTests").With(t => t.Time, DateTime.UtcNow));

            fixture.Register(() => new MongoTimeoutPersister(databaseFactory, "UnitTests"));

            fixture.Customize(new SupportMutableValueTypesCustomization());

            TimeoutClassMaps.ConfigureClassMaps();
        }
    }
}
