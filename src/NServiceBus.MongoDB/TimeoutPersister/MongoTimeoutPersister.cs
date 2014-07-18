// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersister.cs" company="SharkByte Software Inc.">
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
//   Defines the MongoTimeoutPersister type.
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
        public List<Tuple<string, DateTime>> GetNextChunk(DateTime startSlice, out DateTime nextTimeToRunQuery)
        {
            var collection = this.mongoDatabase.GetCollection<TimeoutData>(TimeoutDataName);

            var results = from data in collection.AsQueryable<TimeoutData>()
                          where data.Time > startSlice && data.Time <= DateTime.UtcNow
                          orderby data.Time
                          select new Tuple<string, DateTime>(data.Id, data.Time);

            var nextTimeout =
                (from data in collection.AsQueryable<TimeoutData>()
                 where data.Time > DateTime.UtcNow
                 orderby data.Time
                 select data).FirstOrDefault();

            nextTimeToRunQuery = nextTimeout != null
                                     ? nextTimeout.Time
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
                timeoutData = null;
                return false;
            }

            timeoutData = result.GetModifiedDocumentAs<TimeoutData>();
            return true;
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
    }
}
