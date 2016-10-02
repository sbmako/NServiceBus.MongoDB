// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionExtensions.cs" company="SharkByte Software">
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
//   Defines the DocumentVersionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using global::MongoDB.Bson;
    using global::MongoDB.Driver;

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SubscriptionPersister;

    internal static class DocumentVersionExtensions
    {
        public static FilterDefinition<BsonDocument> MongoUpdateQuery(this IContainSagaData saga)
        {
            Contract.Requires(saga != null);
            Contract.Ensures(Contract.Result<FilterDefinition<BsonDocument>>() != null);

            var versionedDocument = saga as IHaveDocumentVersion;

            if (versionedDocument == null)
            {
                throw new InvalidOperationException(
                    string.Format("Saga type {0} does not implement IHaveDocumentVersion", saga.GetType().Name));
            }

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", saga.Id)
                         & builder.Eq(MongoPersistenceConstants.VersionPropertyName, versionedDocument.DocumentVersion);

            return filter.AssumedNotNull();
        }

        public static UpdateDefinition<BsonDocument> MongoUpdate<T>(this T saga) where T : IContainSagaData
        {
            Contract.Requires(saga != null);
            Contract.Ensures(Contract.Result<UpdateDefinition<BsonDocument>>() != null);

            var classMap = saga.ToBsonDocument();

            classMap.Remove("_id");
            classMap.Remove(MongoPersistenceConstants.VersionPropertyName);

            var builder = Builders<BsonDocument>.Update;
            var update = builder.Inc(MongoPersistenceConstants.VersionPropertyName, 1);

            classMap.ToList().ForEach(f => update.Set(f.Name, f.Value));

            return update.AssumedNotNull();
        }

        public static FilterDefinition<Subscription> MongoUpdateQuery(this Subscription subscription)
        {
            Contract.Requires(subscription != null);
            Contract.Ensures(Contract.Result<FilterDefinition<Subscription>>() != null);

            return
                Builders<Subscription>.Filter.And(
                    Builders<Subscription>.Filter.Eq(s => s.Id, subscription.Id),
                    Builders<Subscription>.Filter.Eq(s => s.DocumentVersion, subscription.DocumentVersion))
                    .AssumedNotNull();
        }
    }
}
