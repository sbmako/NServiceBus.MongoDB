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
    using global::MongoDB.Driver.Builders;

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SubscriptionPersister;

    internal static class DocumentVersionExtensions
    {
        public static IMongoQuery MongoUpdateQuery(this IContainSagaData saga)
        {
            Contract.Requires(saga != null);
            Contract.Ensures(Contract.Result<IMongoQuery>() != null);

            var versionedDocument = saga as IHaveDocumentVersion;

            if (versionedDocument == null)
            {
                throw new InvalidOperationException(
                    string.Format("Saga type {0} does not implement IHaveDocumentVersion", saga.GetType().Name));
            }

            return
                Query.And(
                    Query.EQ("_id", saga.Id),
                    Query.EQ(MongoPersistenceConstants.VersionPropertyName, versionedDocument.DocumentVersion))
                     .AssumedNotNull();
        }

        public static IMongoUpdate MongoUpdate<T>(this T saga) where T : IContainSagaData
        {
            Contract.Requires(saga != null);
            Contract.Ensures(Contract.Result<IMongoUpdate>() != null);

            var classMap = saga.ToBsonDocument();

            var versionedDocument = saga as IHaveDocumentVersion;
            if (versionedDocument == null)
            {
                return Update.Replace(saga).AssumedNotNull();
            }

            classMap.Remove("_id");
            classMap.Remove(MongoPersistenceConstants.VersionPropertyName);
            var updateBuilder = Update.Inc(MongoPersistenceConstants.VersionPropertyName, 1);

            classMap.ToList().ForEach(f => updateBuilder.Set(f.Name, f.Value));

            return updateBuilder.AssumedNotNull();
        }

        public static IMongoQuery MongoUpdateQuery(this Subscription subscription)
        {
            Contract.Requires(subscription != null);
            Contract.Ensures(Contract.Result<IMongoQuery>() != null);

            return
                Query.And(
                    Query<Subscription>.EQ(s => s.Id, subscription.Id),
                    Query<Subscription>.EQ(s => s.DocumentVersion, subscription.DocumentVersion)).AssumedNotNull();
        }

        public static IMongoUpdate MongoUpdate(this Subscription subscription)
        {
            Contract.Requires(subscription != null);
            Contract.Ensures(Contract.Result<IMongoUpdate>() != null);

            return Update.Replace(subscription).AssumedNotNull();
        }
    }
}
