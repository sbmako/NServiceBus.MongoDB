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
    using NServiceBus.Saga;

    /// <summary>
    /// The Mongo saga persister.
    /// </summary>
    public class MongoSagaPersister : ISagaPersister
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MongoSagaPersister));

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
            collection.Insert(saga);

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
                return;
            }

            ////var uniqueProperty = p.Value;

            ////var metadata = Session.Advanced.GetMetadataFor(saga);

            ////if the user just added the unique property to a saga with existing data we need to set it
            ////if (!metadata.ContainsKey(UniqueValueMetadataKey))
            ////{
            ////    StoreUniqueProperty(saga);
            ////    return;
            ////}

            ////var storedValue = metadata[UniqueValueMetadataKey].ToString();

            ////var currentValue = uniqueProperty.Value.ToString();

            ////if (currentValue == storedValue)
            ////{
            ////    return;
            ////}

            ////this.DeleteUniqueProperty(saga, new KeyValuePair<string, object>(uniqueProperty.Key, storedValue));
            this.StoreUniqueProperty(saga);

            var collection = this.mongoDatabase.GetCollection(saga.GetType().Name);
            collection.Save(saga);
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
            ////if (IsUniqueProperty<T>(property))
            ////    return GetByUniqueProperty<T>(property, value);

            ////return GetByQuery<T>(property, value).FirstOrDefault();
            return default(T);
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

            if (result.DocumentsAffected == 0)
            {
                Logger.Error("Unable to find and remove saga");
                throw new InvalidOperationException(
                    string.Format("Unable to find and remove saga for with id {0}", saga.Id));
            }
        }

        ////private void DeleteUniqueProperty(IContainSagaData saga, KeyValuePair<string, object> uniqueProperty)
        ////{
        ////    var id = SagaUniqueIdentity.FormatId(saga.GetType(), uniqueProperty);

        ////    ////Session.Advanced.Defer(new DeleteCommandData { Key = id });
        ////}

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
