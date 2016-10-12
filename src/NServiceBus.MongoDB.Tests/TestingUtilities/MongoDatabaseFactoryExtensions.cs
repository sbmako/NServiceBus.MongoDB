// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDatabaseFactoryExtensions.cs" company="SharkByte Software">
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
//   Defines the MongoDatabaseFactoryExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TestingUtilities
{
    using System.Collections.Generic;
    using System.Linq;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.TimeoutPersister;

    internal static class MongoDatabaseFactoryExtensions
    {
        public static T RetrieveSagaData<T>(this MongoDatabaseFactory factory, T sagaData)
            where T : IContainSagaData
        {
            var query = Builders<T>.Filter.Eq(e => e.Id, sagaData.Id);
            var entity = factory.GetDatabase().GetCollection<T>(typeof(T).Name).FindAsync(query).Result.ToList();

            return entity.Any() ? entity.First() : default(T);
        }

        public static IEnumerable<TimeoutEntity> RetrieveAllTimeouts(this MongoDatabaseFactory factor)
        {
            var timeouts =
                from t in
                    factor.GetDatabase()
                    .GetCollection<TimeoutEntity>(MongoTimeoutPersister.TimeoutEntityName)
                    .AsQueryable()
                select t;

            return timeouts;
        }

        public static void ResetTimeoutCollection(this MongoDatabaseFactory factory)
        {
            var database = factory.GetDatabase();

            database.GetCollection<TimeoutEntity>(MongoTimeoutPersister.TimeoutEntityName)
                .DeleteManyAsync(Builders<TimeoutEntity>.Filter.Empty)
                .Wait();
        }
    }
}
