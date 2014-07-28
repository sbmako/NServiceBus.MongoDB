// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDatabaseFactoryExtensions.cs" company="Carlos Sandoval">
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
//   Defines the MongoDatabaseFactoryExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TestingUtilities
{
    using System.Collections.Generic;
    using System.Linq;

    using NServiceBus.Timeout.Core;

    using global::MongoDB.Driver.Builders;
    using NServiceBus.MongoDB.TimeoutPersister;
    using NServiceBus.Saga;

    using global::MongoDB.Driver.Linq;

    internal static class MongoDatabaseFactoryExtensions
    {
        public static T RetrieveSagaData<T>(this MongoDatabaseFactory factory, T sagaData)
            where T : IContainSagaData
        {
            var query = Query<T>.EQ(e => e.Id, sagaData.Id);
            var entity = factory.GetDatabase()
                                .GetCollection<T>(typeof(T).Name)
                                .FindOne(query);
            return entity;
        }

        public static IEnumerable<TimeoutData> RetrieveAllTimeouts(this MongoDatabaseFactory factor)
        {
            var timeouts = from t in factor.GetDatabase()
                               .GetCollection<TimeoutData>(MongoTimeoutPersister.TimeoutDataName).AsQueryable()
                            select t;

            return timeouts;
        }

        public static void ResetTimeoutCollection(this MongoDatabaseFactory factory)
        {
            var collection = factory.GetDatabase().GetCollection<TimeoutData>(MongoTimeoutPersister.TimeoutDataName);
            collection.Drop();
        }
    }
}
