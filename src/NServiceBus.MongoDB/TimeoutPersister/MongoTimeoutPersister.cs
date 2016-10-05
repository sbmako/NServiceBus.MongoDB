// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersister.cs" company="SharkByte Software">
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
//   The MongoDB timeout persister.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.TimeoutPersister
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Timeout.Core;

    /// <summary>
    /// The mongo timeout persister.
    /// </summary>
    public class MongoTimeoutPersister : IPersistTimeouts, IQueryTimeouts
    {
        internal static readonly string TimeoutDataName = typeof(TimeoutEntity).Name;

        private readonly IMongoDatabase mongoDatabase;

        private readonly string endpointName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoTimeoutPersister"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The mongo factory.
        /// </param>
        /// <param name="endpointName">
        /// The endpoint Name.
        /// </param>
        public MongoTimeoutPersister(MongoDatabaseFactory mongoFactory, string endpointName)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);

            this.mongoDatabase = mongoFactory.GetDatabase();
            this.endpointName = endpointName;
            this.EnsureTimeoutIndexes();
        }

        /// <summary>
        /// Retrieves the next range of timeouts that are due.
        /// </summary>
        /// <param name="startSlice">The time where to start retrieving the next slice, the slice should exclude this date.</param>
        /// <returns>
        /// Returns the next range of timeouts that are due.
        /// </returns>
        public Task<TimeoutsChunk> GetNextChunk(DateTime startSlice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the next range of timeouts that are due.
        /// </summary>
        /// <param name="startSlice"> The time where to start retrieving the next slice, the slice should exclude this date.</param>
        /// <param name="nextTimeTorunQuery">Returns the next time we should query again.</param>
        /// <returns>
        /// Returns the next range of timeouts that are due.
        /// </returns>
        public IEnumerable<Tuple<string, DateTime>> GetNextChunk(DateTime startSlice, out DateTime nextTimeTorunQuery)
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutEntity>(TimeoutDataName);

            var now = DateTime.UtcNow;

            var results = from data in collection.AsQueryable().AssumedNotNull()
                          where data.Time > startSlice && data.Time <= now
                          where
                              data.OwningTimeoutManager == string.Empty
                              || data.OwningTimeoutManager == this.endpointName
                          orderby data.Time ascending 
                          select new Tuple<string, DateTime>(data.Id.ToString(), data.Time);

            var nextTimeout = from data in collection.AsQueryable().AssumedNotNull()
                              where data.Time > now
                              orderby data.Time ascending 
                              select data;

            nextTimeTorunQuery = nextTimeout.Any()
                                     ? nextTimeout.First().Time
                                     : now.AddMinutes(
                                         MongoPersistenceConstants.DefaultNextTimeoutIncrementMinutes);

            return results.ToList();
        }

        /// <summary>
        /// Reads timeout data.
        /// </summary>
        /// <param name="timeoutId">The timeout id to read.</param>
        /// <param name="context">The context</param>
        /// <returns>
        /// <see cref="T:NServiceBus.Timeout.Core.TimeoutEntity"/> of the timeout if it was found. <c>null</c> otherwise.
        /// </returns>
        public Task<Timeout.Core.TimeoutData> Peek(string timeoutId, ContextBag context)
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutEntity>(TimeoutDataName);
            var data = collection.AsQueryable().SingleOrDefault(e => e.Id == timeoutId);
            if (data != null)
            {
                return Task.FromResult<Timeout.Core.TimeoutData>(new Timeout.Core.TimeoutData
                {
                    Id = data.Id,
                    Destination = data.Destination,
                    SagaId = data.SagaId,
                    State = data.State,
                    Time = data.Time,
                    Headers = data.Headers,
                    OwningTimeoutManager = data.OwningTimeoutManager
                });
            }

            return Task.FromResult<Timeout.Core.TimeoutData>(null);
        }
        
        /// <summary>
        /// Add timeout id
        /// </summary>
        /// <param name="timeout">The timeout data to add</param>
        /// <param name="context">The context</param>
        /// <returns>The task</returns>
        public async Task Add(Timeout.Core.TimeoutData timeout, ContextBag context)
        {
            var data = new TimeoutEntity()
            {
                Id = timeout.Id,
                Destination = timeout.Destination,
                SagaId = timeout.SagaId,
                State = timeout.State,
                Time = timeout.Time,
                Headers = timeout.Headers,
                OwningTimeoutManager = timeout.OwningTimeoutManager
            };

            var collection = this.mongoDatabase.GetCollection<TimeoutEntity>(TimeoutDataName);
            await collection.InsertOneAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        /// The IPersistTimeoutsV2 implementation of the TryRemove method
        /// </summary>
        /// <param name="timeoutId">The timeout id to remove.</param>
        /// <param name="context">The context</param>
        /// <returns>
        /// <c>true</c> it the timeout was successfully removed.
        /// </returns>
        public Task<bool> TryRemove(string timeoutId, ContextBag context)
        {
            var query = Builders<TimeoutEntity>.Filter.Eq(e => e.Id, timeoutId);

            var collection = this.mongoDatabase.GetCollection<TimeoutEntity>(TimeoutDataName);
            return Task.FromResult(collection.DeleteOneAsync(query).Result.DeletedCount != 0);
        }

        /// <summary>
        /// Removes the time by saga id.
        /// </summary>
        /// <param name="sagaId">The saga id of the timeouts to remove.</param>
        /// <param name="context">The context</param>
        /// <returns>The task</returns>
        public Task RemoveTimeoutBy(Guid sagaId, ContextBag context)
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutEntity>(TimeoutDataName);
            return collection.DeleteManyAsync(t => t.SagaId == sagaId);
        }

        private void EnsureTimeoutIndexes()
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutEntity>(TimeoutDataName);

            collection.Indexes.CreateOneAsync(
                Builders<TimeoutEntity>.IndexKeys.Ascending(t => t.SagaId),
                new CreateIndexOptions { Background = true }).Wait();

            collection.Indexes.CreateOneAsync(
                Builders<TimeoutEntity>.IndexKeys.Ascending(t => t.OwningTimeoutManager),
                new CreateIndexOptions { Background = true }).Wait();
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.mongoDatabase != null);
            Contract.Invariant(this.endpointName != null);
        }
    }
}
