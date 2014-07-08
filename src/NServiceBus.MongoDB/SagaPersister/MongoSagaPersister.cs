// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSagaPersister.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 Carlos Sandoval. All rights reserved.
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Affero General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Affero General Public License for more details.
//   
//   You should have received a copy of the GNU Affero General Public License
//   along with this program.  If not, see http://www.gnu.org/licenses/.
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
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;
    using NServiceBus.Logging;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.Saga;

    /// <summary>
    /// The Mongo saga persister.
    /// </summary>
    public class MongoSagaPersister : ISagaPersister
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MongoSagaPersister));

        private static readonly string SagaUniqueIdentityName = typeof(SagaUniqueIdentity).Name;

        private readonly MongoDatabase mongoDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSagaPersister"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The mongo factory.
        /// </param>
        public MongoSagaPersister(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);
            this.mongoDatabase = mongoFactory.GetDatabase();
        }

        /// <summary>
        /// Saves the saga entity to the persistence store.
        /// </summary>
        /// <param name="saga">The saga entity to save.</param>
        public void Save(IContainSagaData saga)
        {
            var collection = this.mongoDatabase.GetCollection(saga.GetType().Name);
            var result = collection.Insert(saga);

            if (!result.Ok)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to save with id {0}", saga.Id));
            }

            this.StoreUniqueProperty(saga);
        }

        /// <summary>
        /// Updates an existing saga entity in the persistence store.
        /// </summary>
        /// <param name="saga">The saga entity to updated.</param>
        public void Update(IContainSagaData saga)
        {
            var p = UniqueAttribute.GetUniqueProperty(saga);

            if (!p.HasValue)
            {
                //// TODO: check if unique property has changed 
                Logger.Debug("Update unique property stuff goes here");
            }

            var collection = this.mongoDatabase.GetCollection(saga.GetType().Name);

            var query = saga.MongoUpdateQuery();
            var update = saga.MongoUpdate();
            var result = collection.Update(query, update, UpdateFlags.None);
            if (!result.UpdatedExisting)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to update saga with id {0}", saga.Id));
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
        /// Looks up a saga entity by a given property.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get<T>(string property, object value) where T : IContainSagaData
        {
            return this.GetByUniqueProperty<T>(property, value);
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

            if (!result.Ok)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to find and remove saga with id {0}", saga.Id));
            }

            var uniqueProperty = UniqueAttribute.GetUniqueProperty(saga);

            if (!uniqueProperty.HasValue)
            {
                return;
            }

            this.DeleteUniqueProperty(saga, uniqueProperty.Value);
        }

        private T GetByUniqueProperty<T>(string property, object value) where T : IContainSagaData
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(property));
            Contract.Requires(value != null);

            var lookupId = SagaUniqueIdentity.FormatId(typeof(T), new KeyValuePair<string, object>(property, value));

            var query = Query.EQ("_id", lookupId);
            var result = this.mongoDatabase.GetCollection<SagaUniqueIdentity>(SagaUniqueIdentityName).FindOne(query);

            if (result == null)
            {
                return default(T);
            }

            return this.Get<T>(result.SagaId);
        }

        private void DeleteUniqueProperty(IContainSagaData saga, KeyValuePair<string, object> uniqueProperty)
        {
            Contract.Requires(saga != null);

            var uniqueId = SagaUniqueIdentity.FormatId(saga.GetType(), uniqueProperty);

            var query = Query.EQ("_id", uniqueId);
            var result = this.mongoDatabase.GetCollection(SagaUniqueIdentityName).Remove(query);

            if (!result.Ok)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to find and remove saga unique identity with id {0}", uniqueId));
            }
        }

        private void StoreUniqueProperty(IContainSagaData saga)
        {
            Contract.Requires(saga != null);

            var uniqueProperty = UniqueAttribute.GetUniqueProperty(saga);

            if (!uniqueProperty.HasValue)
            {
                return;
            }

            var id = SagaUniqueIdentity.FormatId(saga.GetType(), uniqueProperty.Value);
            var sagaUniqueIdentity = new SagaUniqueIdentity
                                         {
                                             Id = id,
                                             SagaId = saga.Id,
                                             UniqueValue = uniqueProperty.Value.Value,
                                         };

            var collection = this.mongoDatabase.GetCollection<SagaUniqueIdentity>(sagaUniqueIdentity.GetType().Name);
            collection.Insert(sagaUniqueIdentity);

            this.SetUniqueValueMetadata(saga, uniqueProperty.Value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TBD")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "saga", Justification = "TBD")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "uniqueProperty", Justification = "TBD")]
        private void SetUniqueValueMetadata(IContainSagaData saga, KeyValuePair<string, object> uniqueProperty)
        {
            ////Session.Advanced.GetMetadataFor(saga)[UniqueValueMetadataKey] = uniqueProperty.Value.ToString();
        }
    }
}
