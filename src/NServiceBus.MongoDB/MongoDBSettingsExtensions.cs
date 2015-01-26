// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDBSettingsExtensions.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Carlos Sandoval
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
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    using System;
    using System.Diagnostics.Contracts;

    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.MongoDB.Internals;

    /// <summary>
    /// The MongoDB settings extensions.
    /// </summary>
    public static class MongoDBSettingsExtensions
    {
        /// <summary>
        /// The set connection string name.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="connectionStringName">
        /// The connection string name.
        /// </param>
        /// <returns>
        /// The <see cref="PersistenceExtentions"/>.
        /// </returns>
        public static PersistenceExtentions<MongoDBPersistence> SetConnectionStringName(
            this PersistenceExtentions<MongoDBPersistence> config, string connectionStringName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Requires<ArgumentNullException>(
                !string.IsNullOrWhiteSpace(connectionStringName), 
                "!string.IsNullOrWhiteSpace(connectionStringName)");
            Contract.Ensures(Contract.Result<PersistenceExtentions<MongoDBPersistence>>() != null);

            config.GetSettings().Set(MongoPersistenceConstants.ConnectionStringNameKey, connectionStringName);

            return config;
        }

        /// <summary>
        /// The set database name.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <returns>
        /// The <see cref="PersistenceExtentions"/>.
        /// </returns>
        public static PersistenceExtentions<MongoDBPersistence> SetDatabaseName(
            this PersistenceExtentions<MongoDBPersistence> config, 
            string databaseName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Requires<ArgumentNullException>(
                !string.IsNullOrWhiteSpace(databaseName), 
                "!string.IsNullOrWhiteSpace(databaseName)");
            Contract.Ensures(Contract.Result<PersistenceExtentions<MongoDBPersistence>>() != null);

            config.GetSettings().Set(MongoPersistenceConstants.DatabaseNameKey, databaseName);

            return config;
        }
    }
}
