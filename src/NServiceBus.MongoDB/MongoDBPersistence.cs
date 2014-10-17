// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDBPersistence.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Carlos Sandoval
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
//   The mongo db persistence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace NServiceBus.MongoDB
{
    using NServiceBus.Features;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.TimeoutPersister;
    using NServiceBus.Persistence;

    /// <summary>
    /// The mongo DB persistence.
    /// </summary>
    public class MongoDBPersistence : PersistenceDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBPersistence"/> class.
        /// </summary>
        public MongoDBPersistence()
        {
            this.Defaults(s =>
            {
                ////RavenLogManager.CurrentLogManager = new NoOpLogManager();

                ////s.EnableFeatureByDefault<RavenDbStorageSession>();
                ////s.EnableFeatureByDefault<SharedDocumentStore>();
            });

            ////Supports(Storage.GatewayDeduplication, s => s.EnableFeatureByDefault<RavenDbGatewayDeduplication>());
            this.Supports(Storage.Timeouts, s => s.EnableFeatureByDefault<MongoDBTimeoutStorage>());
            this.Supports(Storage.Sagas, s => s.EnableFeatureByDefault<MongoDBSagaStorage>());
            ////Supports(Storage.Subscriptions, s => s.EnableFeatureByDefault<RavenDbSubscriptionStorage>());
        }
    }}
