// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDatabaseFactory.cs" company="SharkByte Software Inc.">
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
//   The mongo session factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    using System;
    using System.Diagnostics.Contracts;
    using global::MongoDB.Driver;

    /// <summary>
    /// The mongo database factory.
    /// </summary>
    public class MongoDatabaseFactory
    {
        [ThreadStatic]
        private static MongoClientAccessor mongoClientAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDatabaseFactory"/> class.
        /// </summary>
        /// <param name="clientAccessor">
        /// The clientAccessor.
        /// </param>
        public MongoDatabaseFactory(MongoClientAccessor clientAccessor)
        {
            Contract.Requires<ArgumentNullException>(clientAccessor != null, "clientAccessor");
            mongoClientAccessor = clientAccessor;
        }

        /// <summary>
        /// The get database.
        /// </summary>
        /// <returns>
        /// The <see cref="MongoDatabase"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Ok here")]
        public MongoDatabase GetDatabase()
        {
            Contract.Ensures(Contract.Result<MongoDatabase>() != null);

            var databaseName = mongoClientAccessor.DatabaseName;
            var server = mongoClientAccessor.MongoClient.GetServer();
            var database = server.GetDatabase(databaseName);

            Contract.Assume(database != null);
            return database;
        }
    }
}
