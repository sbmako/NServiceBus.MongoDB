// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoPersistence.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Carlos Sandoval
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
//   The configure mongo persistence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using global::MongoDB.Driver;
    using NServiceBus.Logging;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.MongoDB.Utils;

    /// <summary>
    /// The configure mongo persistence.
    /// </summary>
    public static class ConfigureMongoPersistence
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigureMongoPersistence));

        /// <summary>
        /// The mongo persistence.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoPersistence(this Configure config)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config");
            Contract.Ensures(Contract.Result<Configure>() != null);

            if (Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>())
            {
                return config;
            }

            var connectionStringSettings = GetConnectionString();

            var connectionString = connectionStringSettings != null
                                       ? connectionStringSettings.ConnectionString
                                       : MongoPersistenceConstants.DefaultConnectionString;

            return
                config.InternalMongoPersistence(
                    new MongoClientAccessor(
                        new MongoClient(connectionString),
                        Configure.EndpointName.AssumedNotNullOrWhiteSpace().EndpointNameAsDatabaseName()));
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
        public static Configure MongoPersistence(this Configure config, string connectionStringName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(connectionStringName), "connectionStringName");
            Contract.Ensures(Contract.Result<Configure>() != null);

            var connectionString = GetConnectionString(connectionStringName);

            return
                config.InternalMongoPersistence(
                    new MongoClientAccessor(
                        new MongoClient(connectionString),
                        Configure.EndpointName.AssumedNotNullOrWhiteSpace().EndpointNameAsDatabaseName()));
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
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoPersistence(this Configure config, string connectionStringName, string databaseName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(connectionStringName), "connectionStringName");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(databaseName), "databaseName");
            Contract.Ensures(Contract.Result<Configure>() != null);

            var connectionString = GetConnectionString(connectionStringName);
            return config.InternalMongoPersistence(new MongoClientAccessor(new MongoClient(connectionString), databaseName));
        }

        /// <summary>
        /// The mongo persistence.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="getConnectionString">
        /// The get connection string.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoPersistence(this Configure config, Func<string> getConnectionString)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config");
            Contract.Requires<ArgumentException>(getConnectionString != null, "getConnectionString");
            Contract.Ensures(Contract.Result<Configure>() != null);

            var connectionString = getConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException(
                    "Cannot configure Mongo Persister. Connection string can not be null or empty.");
            }

            return
                config.InternalMongoPersistence(
                    new MongoClientAccessor(
                        new MongoClient(getConnectionString()),
                        Configure.EndpointName.AssumedNotNullOrWhiteSpace().EndpointNameAsDatabaseName()));
        }

        /// <summary>
        /// The mongo persistence.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="getConnectionString">
        /// The get connection string.
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoPersistence(this Configure config, Func<string> getConnectionString, string databaseName)
        {
            Contract.Requires<ArgumentNullException>(config != null, "config");
            Contract.Requires<ArgumentException>(getConnectionString != null, "getConnectionString");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(databaseName), "databaseName");
            Contract.Ensures(Contract.Result<Configure>() != null);

            var connectionString = getConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException(
                    "Cannot configure Mongo Persister. Connection string can not be null or empty.");
            }

            return
                config.InternalMongoPersistence(
                    new MongoClientAccessor(new MongoClient(connectionString), databaseName));
        }

        internal static Configure InternalMongoPersistence(this Configure config, MongoClientAccessor clientAccessor)
        {
            Contract.Requires(config != null);
            Contract.Requires(clientAccessor != null);

            return config.InternalMongoPersistence(() =>
            {
                VerifyConnectionToMongoServer(clientAccessor);
                return clientAccessor;
            });
        }

        internal static Configure InternalMongoPersistence(this Configure config, Func<MongoClientAccessor> clientAccessorFactory)
        {
            Contract.Requires(config != null);
            Contract.Requires(clientAccessorFactory != null);

            config.Configurer.ConfigureComponent(clientAccessorFactory, DependencyLifecycle.SingleInstance);
            config.Configurer.ConfigureComponent<MongoDatabaseFactory>(DependencyLifecycle.SingleInstance);

            return config;
        }

        internal static void VerifyConnectionToMongoServer(MongoClientAccessor mongoClientAccessor)
        {
            Contract.Requires(mongoClientAccessor != null);

            var server = mongoClientAccessor.MongoClient.GetServer();

            try
            {
                server.Ping();
            }
            catch (Exception ex)
            {
                ShowUncontactableMongoWarning(mongoClientAccessor.MongoClient, ex);
                return;
            }

            Logger.InfoFormat("Connection to MongoDB at {0} verified.", string.Join(", ", GetMongoServers(mongoClientAccessor.MongoClient)));
        }

        internal static void ShowUncontactableMongoWarning(MongoClient mongoClient, Exception exception)
        {
            Contract.Requires(mongoClient != null);
            Contract.Requires(exception != null);

            var serverSettings = string.Join(", ", GetMongoServers(mongoClient));

            var sb = new StringBuilder();
            sb.AppendFormat("Mongo could not be contacted using: {0}.", serverSettings);
            sb.AppendLine();
            sb.AppendFormat("If you are using a Replica Set, please ensure that all the Mongo instance(s) {0} are available.", serverSettings);
            sb.AppendLine();
            sb.AppendLine(
                @"To configure NServiceBus to use a different connection string add a connection string named ""NServiceBus/Persistence"" in your config file.");
            sb.AppendLine("Reason: " + exception);

            Logger.Warn(sb.ToString());
        }

        private static IEnumerable<MongoServerAddress> GetMongoServers(MongoClient mongoClient)
        {
            Contract.Requires<ArgumentNullException>(mongoClient != null);
            Contract.Ensures(Contract.Result<IEnumerable<MongoServerAddress>>() != null);

            if (mongoClient.Settings.Servers != null && mongoClient.Settings.Servers.Any())
            {
                return mongoClient.Settings.Servers.ToArray();
            }

            return new[] { mongoClient.Settings.Server };
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
