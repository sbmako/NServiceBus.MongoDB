// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoHelpers.cs" company="Carlos Sandoval">
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
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using global::MongoDB.Driver;
    using NServiceBus.Logging;
    using NServiceBus.Settings;

    internal static class MongoHelpers
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MongoHelpers));

        public static void VerifyConnectionToMongoServer(MongoClientAccessor mongoClientAccessor)
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

            Logger.InfoFormat(
                "Connection to MongoDB at {0} verified.",
                string.Join(", ", GetMongoServers(mongoClientAccessor.MongoClient)));
        }

        public static void ShowUncontactableMongoWarning(MongoClient mongoClient, Exception exception)
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

        public static IEnumerable<MongoServerAddress> GetMongoServers(MongoClient mongoClient)
        {
            Contract.Requires(mongoClient != null);
            Contract.Ensures(Contract.Result<IEnumerable<MongoServerAddress>>() != null);

            if (mongoClient.Settings.Servers != null && mongoClient.Settings.Servers.Any())
            {
                return mongoClient.Settings.Servers;
            }

            return new[] { mongoClient.Settings.Server };
        }

        public static string GetConnectionString(ReadOnlySettings settings)
        {
            Contract.Requires(settings != null);
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var result = settings.HasSetting(MongoPersistenceConstants.ConnectionStringKey)
                             ? settings.Get<string>(MongoPersistenceConstants.ConnectionStringKey)
                                   .AssumedNotNullOrWhiteSpace()
                             : GetConnectionStringFromConfig(
                                 settings.Get<string>(MongoPersistenceConstants.ConnectionStringNameKey)
                                   .AssumedNotNullOrWhiteSpace());

            return result;
        }

        public static string GetConnectionStringFromConfig(string connectionStringName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionStringName));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        "Cannot configure Mongo Persister. No connection string named {0} was found",
                        connectionStringName));
            }

            if (string.IsNullOrWhiteSpace(connectionStringSettings.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Connection string named {0} has a null or empty value", connectionStringName));
            }

            return connectionStringSettings.ConnectionString;
        }
    }
}
