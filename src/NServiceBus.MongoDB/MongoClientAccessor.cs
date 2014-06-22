// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoClientAccessor.cs" company="SharkByte Software Inc.">
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
//   Defines the MongoClientAccessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    using System;
    using System.Diagnostics.Contracts;
    using global::MongoDB.Driver;

    /// <summary>
    /// The mongo client accessor.
    /// </summary>
    public sealed class MongoClientAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoClientAccessor"/> class.
        /// </summary>
        /// <param name="mongoClient">
        /// The mongo client.
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        public MongoClientAccessor(MongoClient mongoClient, string databaseName)
        {
            Contract.Requires<ArgumentNullException>(mongoClient != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(databaseName));

            this.MongoClient = mongoClient;
            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// Gets the mongo client.
        /// </summary>
        public MongoClient MongoClient { get; private set; }

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public string DatabaseName { get; private set; }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.MongoClient != null);
            Contract.Invariant(!string.IsNullOrWhiteSpace(this.DatabaseName));
        }
    }
}
