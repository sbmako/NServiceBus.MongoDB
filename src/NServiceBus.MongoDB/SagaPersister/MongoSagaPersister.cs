// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSagaPersister.cs" company="SharkByte Software">
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
//   The Mongo saga persister.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SagaPersister
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using global::MongoDB.Bson;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Persistence;
    using NServiceBus.Sagas;

    /// <summary>
    /// The Mongo saga persister.
    /// </summary>
    public class MongoSagaPersister : ISagaPersister
    {
        ////private static ConcurrentDictionary<Type, string> indexes = new ConcurrentDictionary<Type, string>();

        private readonly MongoDatabase mongoDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSagaPersister"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The mongo factory.
        /// </param>
        public MongoSagaPersister(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null, "mongoFactory != null");
            this.mongoDatabase = mongoFactory.GetDatabase();
        }

        /// <summary>
        /// Saves the saga entity to the persistence store.
        /// </summary>
        /// <param name="saga">The saga entity to save.</param>
        public void Save(IContainSagaData saga)
        {
            var sagaTypeName = saga.GetType().Name;

            if (!(saga is IHaveDocumentVersion))
            {
                throw new InvalidOperationException($"Saga type {sagaTypeName} does not implement IHaveDocumentVersion");
            }

            ////var uniqueProperty = UniqueAttribute.GetUniqueProperty(saga);
            ////if (uniqueProperty.HasValue)
            ////{
            ////    this.EnsureUniqueIndex(saga, uniqueProperty.Value);
            ////    CheckUniqueProperty(saga, uniqueProperty.Value);
            ////}

            var collection = this.mongoDatabase.GetCollection(sagaTypeName);
            var result = collection.Insert(saga);

            if (result.HasLastErrorMessage)
            {
                throw new InvalidOperationException($"Unable to save with id {saga.Id}");
            }
        }

        /// <summary>
        /// Updates an existing saga entity in the persistence store.
        /// </summary>
        /// <param name="saga">The saga entity to updated.</param>
        public void Update(IContainSagaData saga)
        {
            var collection = this.mongoDatabase.GetCollection(saga.GetType().Name);

            var query = saga.MongoUpdateQuery();
            var update = saga.MongoUpdate();
            var result = collection.Update(query, update, UpdateFlags.None);
            if (!result.UpdatedExisting)
            {
                throw new InvalidOperationException($"Unable to update saga with id {saga.Id}");
            }
        }

        /// <summary>
        /// Gets a saga entity from the persistence store by its Id.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sagaId">
        /// The saga Id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get<T>(Guid sagaId) where T : IContainSagaData
        {
            var query = Query<T>.EQ(e => e.Id, sagaId);
            var entity = this.mongoDatabase.GetCollection<T>(typeof(T).Name).FindOne(query);

            return entity;
        }

        /// <summary>
        /// Looks up a saga entity by a given propertyName.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="propertyName">
        /// The propertyName.
        /// </param>
        /// <param name="propertyValue">
        /// The propertyValue.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get<T>(string propertyName, object propertyValue) where T : IContainSagaData
        {
            return this.GetByUniqueProperty<T>(propertyName.AssumedNotNullOrWhiteSpace(), propertyValue);
        }

        /// <summary>
        /// Sets a saga as completed and removes it from the active saga list
        ///             in the persistence store.
        /// </summary>
        /// <param name="saga">The saga to complete.</param>
        public void Complete(IContainSagaData saga)
        {
            var query = Query.EQ("_id", saga.Id);
            var result = this.mongoDatabase.GetCollection(saga.GetType().Name).Remove(query);

            if (result.HasLastErrorMessage)
            {
                throw new InvalidOperationException($"Unable to find and remove saga with id {saga.Id}");
            }
        }

        private static void CheckUniqueProperty(IContainSagaData sagaData, KeyValuePair<string, object> uniqueProperty)
        {
            Contract.Requires(sagaData != null);

            if (uniqueProperty.Value == null)
            {
                throw new ArgumentNullException(
                    "uniqueProperty",
                    $"Property {uniqueProperty.Key} is marked with the [Unique] attribute on {sagaData.GetType().Name} but contains a null propertyValue.");
            }
        }

        private T GetByUniqueProperty<T>(string property, object value) where T : IContainSagaData
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(property));

            var query = Query.EQ(property, BsonValue.Create(value));

            var entity = this.mongoDatabase.GetCollection<T>(typeof(T).Name).FindOne(query);

            return entity;
        }

        private void EnsureUniqueIndex(IContainSagaData saga, KeyValuePair<string, object> uniqueProperty)
        {
            Contract.Requires(saga != null);

            var collection = this.mongoDatabase.GetCollection(saga.GetType().Name);
            var indexOptions = IndexOptions.SetName(uniqueProperty.Key).SetUnique(true).SetSparse(true);
            var result = collection.CreateIndex(IndexKeys.Ascending(uniqueProperty.Key), indexOptions);

            if (result.HasLastErrorMessage)
            {
                throw new InvalidOperationException(
                    $"Unable to create unique index on {saga.GetType().Name}: {uniqueProperty.Key}");
            }
        }

        public Task Save(
            IContainSagaData sagaData,
            SagaCorrelationProperty correlationProperty,
            SynchronizedStorageSession session,
            ContextBag context)
        {
            throw new NotImplementedException();
        }

        public Task Update(IContainSagaData sagaData, SynchronizedStorageSession session, ContextBag context)
        {
            throw new NotImplementedException();
        }

        public Task<TSagaData> Get<TSagaData>(Guid sagaId, SynchronizedStorageSession session, ContextBag context) where TSagaData : IContainSagaData
        {
            throw new NotImplementedException();
        }

        public Task<TSagaData> Get<TSagaData>(string propertyName, object propertyValue, SynchronizedStorageSession session, ContextBag context) where TSagaData : IContainSagaData
        {
            throw new NotImplementedException();
        }

        public Task Complete(IContainSagaData sagaData, SynchronizedStorageSession session, ContextBag context)
        {
            throw new NotImplementedException();
        }
    }
}
