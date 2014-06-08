// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSagaPersister.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the MongoSagaPersister type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SagaPersister
{
    using System;
    using NServiceBus.Saga;

    /// <summary>
    /// The Mongo saga persister.
    /// </summary>
    public class MongoSagaPersister : ISagaPersister
    {
        /// <summary>
        /// Saves the saga entity to the persistence store.
        /// </summary>
        /// <param name="saga">The saga entity to save.</param>
        public void Save(IContainSagaData saga)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing saga entity in the persistence store.
        /// </summary>
        /// <param name="saga">The saga entity to updated.</param>
        public void Update(IContainSagaData saga)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a saga entity from the persistence store by its Id.
        /// </summary>
        public T Get<T>(Guid sagaId) where T : IContainSagaData
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks up a saga entity by a given property.
        /// </summary>
        public T Get<T>(string property, object value) where T : IContainSagaData
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a saga as completed and removes it from the active saga list
        ///             in the persistence store.
        /// </summary>
        /// <param name="saga">The saga to complete.</param>
        public void Complete(IContainSagaData saga)
        {
            throw new NotImplementedException();
        }
    }
}
