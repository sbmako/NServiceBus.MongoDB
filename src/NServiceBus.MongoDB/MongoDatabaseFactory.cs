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
        private static MongoClient mongoClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDatabaseFactory"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public MongoDatabaseFactory(MongoClient client)
        {
            mongoClient = client;
            GetDatabaseName = context => string.Empty;
        }

        /// <summary>
        /// Gets or sets the get database name.
        /// </summary>
        public static Func<IMessageContext, string> GetDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the bus.
        /// </summary>
        public IBus Bus { get; set; }

        /// <summary>
        /// The get database.
        /// </summary>
        /// <returns>
        /// The <see cref="MongoDatabase"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Ok here")]
        public MongoDatabase GetDatabase()
        {
            Contract.Ensures(Contract.Result<MongoDatabase>() != null);

            IMessageContext context = null;

            if (this.Bus != null)
            {
                context = this.Bus.CurrentMessageContext;
            }

            var databaseName = GetDatabaseName(context);

            var server = mongoClient.GetServer();

            var database = string.IsNullOrEmpty(databaseName)
                               ? server.GetDatabase(databaseName)
                               : server.GetDatabase(databaseName);

            return database;
        }

        /// <summary>
        /// The release session.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TBD")]
        public void ReleaseServer()
        {
        }

        /// <summary>
        /// The save changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TBD")]
        public void SaveChanges()
        {
        }
    }
}
