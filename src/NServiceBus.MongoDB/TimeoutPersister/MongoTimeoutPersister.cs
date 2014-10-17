// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersister.cs" company="Carlos Sandoval">
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
//   The mongo timeout persister.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.TimeoutPersister
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;
    using global::MongoDB.Driver.Linq;
    using NServiceBus.MongoDB.Utils;
    using NServiceBus.Timeout.Core;

    /// <summary>
    /// The mongo timeout persister.
    /// </summary>
    public class MongoTimeoutPersister : IPersistTimeouts
    {
        internal static readonly string TimeoutDataName = typeof(TimeoutData).Name;

        private readonly MongoDatabase mongoDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoTimeoutPersister"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The mongo factory.
        /// </param>
        public MongoTimeoutPersister(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);
            this.mongoDatabase = mongoFactory.GetDatabase();
            this.EnsureTimeoutIndexes();
        }

        /// <summary>
        /// Retrieves the next range of timeouts that are due.
        /// </summary>
        /// <param name="startSlice"> The time where to start retrieving the next slice, the slice should exclude this date.</param>
        /// <param name="nextTimeToRunQuery">Returns the next time we should query again.</param>
        /// <returns>
        /// Returns the next range of timeouts that are due.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Ok here.")]
        IEnumerable<Tuple<string, DateTime>> IPersistTimeouts.GetNextChunk(DateTime startSlice, out DateTime nextTimeToRunQuery)
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutData>(TimeoutDataName);

            var results = from data in collection.AsQueryable().NullChecked()
                          where data.Time > startSlice && data.Time <= DateTime.UtcNow
                          where
                              data.OwningTimeoutManager == string.Empty
                              ////|| data.OwningTimeoutManager == Configure.EndpointName
                          orderby data.Time
                          select new Tuple<string, DateTime>(data.Id, data.Time);

            var nextTimeout = from data in collection.AsQueryable().NullChecked()
                              where data.Time > DateTime.UtcNow
                              orderby data.Time
                              select data;

            nextTimeToRunQuery = nextTimeout.Any()
                                     ? nextTimeout.First().Time
                                     : DateTime.UtcNow.AddMinutes(
                                         MongoPersistenceConstants.DefaultNextTimeoutIncrementMinutes);

            return results.ToList();
        }

        /// <summary>
        /// Adds a new timeout.
        /// </summary>
        /// <param name="timeout">Timeout data.</param>
        public void Add(TimeoutData timeout)
        {
            timeout.Id = Guid.NewGuid().ToString();

            var collection = this.mongoDatabase.GetCollection<TimeoutData>(TimeoutDataName);
            var result = collection.Save(timeout);

            if (!result.Ok)
            {
                throw new InvalidOperationException(string.Format("Unable to save timeout [{0}]", timeout));
            }
        }

        /// <summary>
        /// Removes the timeout if it hasn't been previously removed.
        /// </summary>
        /// <param name="timeoutId">The timeout id to remove.</param>
        /// <param name="timeoutData">The timeout data of the removed timeout.</param>
        /// <returns>
        /// <c>true</c> it the timeout was successfully removed.
        /// </returns>
        public bool TryRemove(string timeoutId, out TimeoutData timeoutData)
        {
            var findAndRemoveArgs = new FindAndRemoveArgs { Query = Query<TimeoutData>.EQ(t => t.Id, timeoutId) };

            var collection = this.mongoDatabase.GetCollection<TimeoutData>(TimeoutDataName);
            var result = collection.FindAndRemove(findAndRemoveArgs);

            if (!result.Ok)
            {
                throw new InvalidOperationException(string.Format("Unable to remove timeout for id {0}: {1}", timeoutId, result.ErrorMessage));
            }

            timeoutData = result.GetModifiedDocumentAs<TimeoutData>();
            return timeoutData != null;
        }

        /// <summary>
        /// Removes the time by saga id.
        /// </summary>
        /// <param name="sagaId">The saga id of the timeouts to remove.</param>
        public void RemoveTimeoutBy(Guid sagaId)
        {
            var query = Query<TimeoutData>.EQ(t => t.SagaId, sagaId);

            var collection = this.mongoDatabase.GetCollection<TimeoutData>(TimeoutDataName);
            var result = collection.Remove(query);

            if (!result.Ok)
            {
                throw new InvalidOperationException(string.Format("Unable to remove timeouts for saga id {0}", sagaId));
            }
        }

        private void EnsureTimeoutIndexes()
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutData>(TimeoutDataName);

            var indexOptions = IndexOptions.SetName(MongoPersistenceConstants.OwningTimeoutManagerAndTimeName);
            var result =
                collection.CreateIndex(
                    IndexKeys<TimeoutData>.Ascending(t => t.OwningTimeoutManager, t => t.Time), indexOptions);

            if (!result.Ok)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to create {0} index", MongoPersistenceConstants.OwningTimeoutManagerAndTimeName));
            }

            indexOptions.SetName(MongoPersistenceConstants.OwningTimeoutManagerAndSagaIdAndTimeName);
            result =
                collection.CreateIndex(
                    IndexKeys<TimeoutData>.Ascending(t => t.OwningTimeoutManager, t => t.SagaId, t => t.Time),
                    indexOptions);

            if (!result.Ok)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to create {0} index", MongoPersistenceConstants.OwningTimeoutManagerAndSagaIdAndTimeName));
            }
        }
    }
}
