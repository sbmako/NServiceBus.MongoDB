// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDBPersistence.cs" company="SharkByte Software">
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
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    using NServiceBus.Features;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.SubscriptionPersister;
    using NServiceBus.MongoDB.TimeoutPersister;
    using NServiceBus.Persistence;

    /// <summary>
    /// The MongoDB persistence.
    /// </summary>
    public class MongoDBPersistence : PersistenceDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBPersistence"/> class.
        /// </summary>
        public MongoDBPersistence()
        {
            this.Defaults(
                s =>
                    {
                        s.EnableFeatureByDefault<MongoDocumentStore>();
                    });

            this.Supports<StorageType.Sagas>(s => s.EnableFeatureByDefault<MongoSagaStorage>());
            this.Supports<StorageType.Timeouts>(s => s.EnableFeatureByDefault<MongoTimeoutStorage>());
            this.Supports<StorageType.Subscriptions>(s => s.EnableFeatureByDefault<MongoSubscriptionStorage>());
        }
    }
}
