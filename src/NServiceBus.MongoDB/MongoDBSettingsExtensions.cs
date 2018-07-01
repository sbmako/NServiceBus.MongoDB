// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDBSettingsExtensions.cs" company="SharkByte Software">
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

namespace NServiceBus.MongoDB
{
    using System;
    using System.Diagnostics.Contracts;

    using NServiceBus.Configuration.AdvancedExtensibility;
    using NServiceBus.MongoDB.Internals;

    /// <summary>
    /// The MongoDB settings extensions.
    /// </summary>
    public static class MongoDBSettingsExtensions
    {
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
        /// The <see cref="PersistenceExtensions"/>.
        /// </returns>
        public static PersistenceExtensions<MongoDBPersistence> SetDatabaseName(
            this PersistenceExtensions<MongoDBPersistence> config, 
            string databaseName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Requires<ArgumentNullException>(databaseName != null, "databaseName != null");
            Contract.Ensures(Contract.Result<PersistenceExtensions<MongoDBPersistence>>() != null);

            config.GetSettings().Set(MongoPersistenceConstants.DatabaseNameKey, databaseName);

            return config;
        }

        /// <summary>
        /// The set connection string.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="connectionString">
        /// The database name.
        /// </param>
        /// <returns>
        /// The <see cref="PersistenceExtensions"/>.
        /// </returns>
        public static PersistenceExtensions<MongoDBPersistence> SetConnectionString(
            this PersistenceExtensions<MongoDBPersistence> config,
            string connectionString)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Requires<ArgumentNullException>(connectionString != null, "connectionString != null");
            Contract.Ensures(Contract.Result<PersistenceExtensions<MongoDBPersistence>>() != null);

            config.GetSettings().Set(MongoPersistenceConstants.ConnectionStringKey, connectionString);
            
            return config;
        }
    }
}
