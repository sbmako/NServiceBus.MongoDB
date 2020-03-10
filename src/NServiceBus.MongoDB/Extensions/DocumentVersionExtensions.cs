// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionExtensions.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2018 SharkByte Software
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
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Extensions
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using global::MongoDB.Bson;
    using global::MongoDB.Driver;

    using NServiceBus.MongoDB.Internals;

    static class DocumentVersionExtensions
    {
        public static FilterDefinition<BsonDocument> MongoUpdateQuery<T>(this T sagaData, int oldDocumentVersion)
            where T : IContainSagaData
        {
            Contract.Requires(sagaData != null);
            Contract.Ensures(Contract.Result<FilterDefinition<BsonDocument>>() != null);

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq(MongoPersistenceConstants.IdPropertyName, sagaData.Id)
                         & builder.Eq(MongoPersistenceConstants.VersionPropertyName, oldDocumentVersion);

            return filter.AssumedNotNull();
        }

        public static UpdateDefinition<BsonDocument> MongoUpdate<T>(this T sagaData)
            where T : IContainSagaData
        {
            Contract.Requires(sagaData != null);
            Contract.Ensures(Contract.Result<UpdateDefinition<BsonDocument>>() != null);

            var classMap = sagaData.ToBsonDocument();

            classMap.Remove(MongoPersistenceConstants.IdPropertyName);
            classMap.Remove(MongoPersistenceConstants.VersionPropertyName);
            classMap.Remove(MongoPersistenceConstants.ETagPropertyName);

            var builder = Builders<BsonDocument>.Update;
            var update = builder.Inc(MongoPersistenceConstants.VersionPropertyName, 1);

            classMap.ToList().ForEach(f => update = update.Set(f.Name, f.Value));

            return update.AssumedNotNull();
        }
    }
}
