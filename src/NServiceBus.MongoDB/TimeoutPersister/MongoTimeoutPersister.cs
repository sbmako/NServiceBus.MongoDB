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
        internal static readonly string TimeoutEntityName = "TimeoutData";

        private readonly IMongoCollection<TimeoutEntity> collection; 

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

            this.collection = mongoFactory.GetDatabase().GetCollection<TimeoutEntity>(TimeoutEntityName);

            this.endpointName = endpointName;
            this.EnsureTimeoutIndexes().Wait();
        }

        /// <summary>
        /// Retrieves the next range of timeouts that are due.
        /// </summary>
        /// <param name="startSlice">The time where to start retrieving the next slice, the slice should exclude this date.</param>
        /// <returns>
        /// Returns the next range of timeouts that are due.
        /// </returns>
        public async Task<TimeoutsChunk> GetNextChunk(DateTime startSlice)
        {
            var now = DateTime.UtcNow;

            var builder = Builders<TimeoutEntity>.Filter;
            var query = builder.Eq(t => t.OwningTimeoutManager, this.endpointName) & builder.Gt(t => t.Time, startSlice)
                        & builder.Lte(t => t.Time, now);

            var results =
                await
                this.collection.Find(query)
                    .Sort(Builders<TimeoutEntity>.Sort.Ascending(t => t.Time))
                    .ToListAsync()
                    .ConfigureAwait(false);

            var nextTimeoutQuery = Builders<TimeoutEntity>.Filter.Gt(t => t.Time, now);
            var nextTimeout =
                await
                this.collection.Find(nextTimeoutQuery)
                    .Sort(Builders<TimeoutEntity>.Sort.Ascending(t => t.Time))
                    .ToListAsync()
                    .ConfigureAwait(false);

            var nextTimeTorunQuery = nextTimeout.Any()
                                         ? nextTimeout.First().Time
                                         : now.AddMinutes(MongoPersistenceConstants.DefaultNextTimeoutIncrementMinutes);

            return new TimeoutsChunk(
                results.Select(data => new TimeoutsChunk.Timeout(data.Id, data.Time)),
                nextTimeTorunQuery);
        }

        /// <summary>
        /// Reads timeout data.
        /// </summary>
        /// <param name="timeoutId">The timeout id to read.</param>
        /// <param name="context">The context</param>
        /// <returns>
        /// <see cref="T:NServiceBus.Timeout.Core.TimeoutEntity"/> of the timeout if it was found. <c>null</c> otherwise.
        /// </returns>
        public async Task<TimeoutData> Peek(string timeoutId, ContextBag context)
        {
            var data =
                await this.collection.AsQueryable().SingleOrDefaultAsync(e => e.Id == timeoutId).ConfigureAwait(false);
            if (data != null)
            {
                return new TimeoutData
                            {
                                Id = data.Id,
                                Destination = data.Destination,
                                SagaId = data.SagaId,
                                State = data.State,
                                Time = data.Time,
                                Headers = data.Headers,
                                OwningTimeoutManager = data.OwningTimeoutManager
                            };
            }

            return null;
        }
        
        /// <summary>
        /// Add timeout id
        /// </summary>
        /// <param name="timeout">The timeout data to add</param>
        /// <param name="context">The context</param>
        /// <returns>The task</returns>
        public async Task Add(TimeoutData timeout, ContextBag context)
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

            await this.collection.InsertOneAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        /// The IPersistTimeoutsV2 implementation of the TryRemove method
        /// </summary>
        /// <param name="timeoutId">The timeout id to remove.</param>
        /// <param name="context">The context</param>
        /// <returns>
        /// <c>true</c> it the timeout was successfully removed.
        /// </returns>
        public async Task<bool> TryRemove(string timeoutId, ContextBag context)
        {
            var query = Builders<TimeoutEntity>.Filter.Eq(e => e.Id, timeoutId);

            var result = await this.collection.DeleteOneAsync(query).ConfigureAwait(false);
            return result.DeletedCount != 0;
        }

        /// <summary>
        /// Removes the time by saga id.
        /// </summary>
        /// <param name="sagaId">The saga id of the timeouts to remove.</param>
        /// <param name="context">The context</param>
        /// <returns>The task</returns>
        public async Task RemoveTimeoutBy(Guid sagaId, ContextBag context)
        {
            await this.collection.DeleteManyAsync(t => t.SagaId == sagaId).ConfigureAwait(false);
        }

        private async Task EnsureTimeoutIndexes()
        {
            await
                this.collection.Indexes.CreateOneAsync(
                    Builders<TimeoutEntity>.IndexKeys.Ascending(t => t.SagaId),
                    new CreateIndexOptions { Background = true }).ConfigureAwait(false);

            await
                this.collection.Indexes.CreateOneAsync(
                    Builders<TimeoutEntity>.IndexKeys.Ascending(t => t.OwningTimeoutManager),
                    new CreateIndexOptions { Background = true }).ConfigureAwait(false);
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.collection != null);
            Contract.Invariant(this.endpointName != null);
        }
    }
}
