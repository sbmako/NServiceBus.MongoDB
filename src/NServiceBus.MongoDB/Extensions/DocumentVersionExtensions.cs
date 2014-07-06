// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionExtensions.cs" company="SharkByte Software Inc.">
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
//   Defines the DocumentVersionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Extensions
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using global::MongoDB.Bson;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;
    using NServiceBus.Saga;

    internal static class DocumentVersionExtensions
    {
        public static IMongoQuery SagaMongoUpdateQuery(this IContainSagaData saga)
        {
            Contract.Requires(saga != null);
            Contract.Ensures(Contract.Result<IMongoQuery>() != null);

            var query = Query.EQ("_id", saga.Id);

            var versionedDocument = saga as IHaveDocumentVersion;
            return versionedDocument == null
                       ? query
                       : Query.And(
                           query,
                           Query.EQ(MongoPersistenceConstants.VersionPropertyName, versionedDocument.DocumentVersion));
        }

        public static IMongoUpdate SagaMongoUpdate<T>(this T saga)
            where T : IContainSagaData
        {
            Contract.Requires(saga != null);
            Contract.Ensures(Contract.Result<IMongoUpdate>() != null);

            var classMap = saga.ToBsonDocument();

            var versionedDocument = saga as IHaveDocumentVersion;
            if (versionedDocument == null)
            {
                return Update.Replace(saga);
            }

            classMap.Remove("_id");
            classMap.Remove(MongoPersistenceConstants.VersionPropertyName);
            var updateBuilder = Update.Inc(MongoPersistenceConstants.VersionPropertyName, 1);

            classMap.ToList().ForEach(f => updateBuilder.Set(f.Name, f.Value));

            return updateBuilder;
        }
    }
}
