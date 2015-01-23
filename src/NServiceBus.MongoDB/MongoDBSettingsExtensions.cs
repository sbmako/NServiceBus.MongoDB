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
    using System.Configuration;
    using System.Diagnostics.Contracts;

    using global::MongoDB.Driver;

    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Settings;

    /// <summary>
    /// The MongoDB settings extensions.
    /// </summary>
    public static class MongoDBSettingsExtensions
    {
        /// <summary>
        /// The MongoDB persistence.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="PersistenceExtentions"/>.
        /// </returns>
        public static PersistenceExtentions<MongoDBPersistence> SetDefaultMongoPersistence(
            this PersistenceExtentions<MongoDBPersistence> config)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Ensures(Contract.Result<PersistenceExtentions<MongoDBPersistence>>() != null);

            config.GetSettings()
                .Set<MongoClientAccessor>(InternalMongoPersistence(config.GetSettings().GetDefaultClientAccessor()));

            return config;
        }

        /// <summary>
        /// The mongo persistence.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="connectionStringName">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static PersistenceExtentions<MongoDBPersistence> MongoPersistence(
            this PersistenceExtentions<MongoDBPersistence> config, string connectionStringName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Requires<ArgumentNullException>(
                !string.IsNullOrWhiteSpace(connectionStringName),
                "!string.IsNullOrWhiteSpace(connectionStringName)");
            Contract.Ensures(Contract.Result<PersistenceExtentions<MongoDBPersistence>>() != null);

            var connectionString = GetConnectionString(connectionStringName);
            var client = new MongoClient(connectionString);
            var clientAccessor = new MongoClientAccessor(client, config.GetSettings().EndpointNameAsDatabaseName());

            config.GetSettings().Set<MongoClientAccessor>(InternalMongoPersistence(clientAccessor));

            return config;
        }

        /// <summary>
        /// The MongoDB persistence.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="connectionStringName">
        /// The connection string name.
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <returns>
        /// The <see cref="PersistenceExtentions"/>.
        /// </returns>
        public static PersistenceExtentions<MongoDBPersistence> MongoPersistence(
            this PersistenceExtentions<MongoDBPersistence> config,
            string connectionStringName,
            string databaseName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config != null");
            Contract.Requires<ArgumentNullException>(
                !string.IsNullOrWhiteSpace(connectionStringName),
                "!string.IsNullOrWhiteSpace(connectionStringName)");
            Contract.Requires<ArgumentNullException>(
                !string.IsNullOrWhiteSpace(databaseName),
                "!string.IsNullOrWhiteSpace(databaseName)");
            Contract.Ensures(Contract.Result<PersistenceExtentions<MongoDBPersistence>>() != null);

            var connectionString = GetConnectionString(connectionStringName);

            var client = new MongoClient(connectionString);
            var clientAccessor = new MongoClientAccessor(client, databaseName);
            
            config.GetSettings().Set<MongoClientAccessor>(InternalMongoPersistence(clientAccessor));

            return config;
        }

        /// <summary>
        /// The get default client accessor.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="MongoClientAccessor"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed")]
        public static MongoClientAccessor GetDefaultClientAccessor(this SettingsHolder settings)
        {
            Contract.Requires<ArgumentNullException>(settings != null, "settings != null");
            Contract.Ensures(Contract.Result<MongoClientAccessor>() != null);

            var connectionStringSettings = GetConnectionString();

            var connectionString = connectionStringSettings != null
                                       ? connectionStringSettings.ConnectionString
                                       : MongoPersistenceConstants.DefaultConnectionString;

            var client = new MongoClient(connectionString);
            return new MongoClientAccessor(client, settings.EndpointNameAsDatabaseName());
        }

        internal static MongoClientAccessor InternalMongoPersistence(MongoClientAccessor clientAccessor)
        {
            Contract.Requires(clientAccessor != null);
            Contract.Ensures(Contract.Result<MongoClientAccessor>() != null);

            ConnectionVerifier.VerifyConnectionToMongoServer(clientAccessor);
            return clientAccessor;
        }

        internal static string EndpointNameAsDatabaseName(this SettingsHolder settings)
        {
            Contract.Requires(settings != null);
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            return settings.EndpointName().NullOrWhiteSpaceChecked().EndpointNameAsDatabaseName();
        }

        private static ConnectionStringSettings GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["NServiceBus.Persistence"]
                   ?? ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"];
        }

        private static string GetConnectionString(string connectionStringName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionStringName));
            Contract.Ensures(Contract.Result<string>() != null);

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        "Cannot configure Mongo Persister. No connection string named {0} was found",
                        connectionStringName));
            }

            return !string.IsNullOrWhiteSpace(connectionStringSettings.ConnectionString)
                       ? connectionStringSettings.ConnectionString
                       : string.Empty;
        }
    }
}
