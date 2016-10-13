// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoHelpers.cs" company="SharkByte Software">
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
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Internals
{
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using NServiceBus.Logging;
    using NServiceBus.Settings;

    internal static class MongoHelpers
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MongoHelpers));

        public static string GetConnectionString(ReadOnlySettings settings)
        {
            Contract.Requires(settings != null);
            Contract.Ensures(Contract.Result<string>() != null);

            if (settings.HasSetting(MongoPersistenceConstants.ConnectionStringKey))
            {
                var connectionString =
                    settings.Get<string>(MongoPersistenceConstants.ConnectionStringKey).AssumedNotNull();

                return connectionString;
            }

            if (ConfigurationManager.ConnectionStrings[MongoPersistenceConstants.DefaultConnectionStringName] != null)
            {
                Logger.InfoFormat(
                    "Using connection string from {0}",
                    MongoPersistenceConstants.DefaultConnectionStringName);
                return GetConnectionStringFromConfig(MongoPersistenceConstants.DefaultConnectionStringName);    
            }

            Logger.InfoFormat("Using connection string from {0}", MongoPersistenceConstants.FallbackConnectionStringName);

            return GetConnectionStringFromConfig(MongoPersistenceConstants.FallbackConnectionStringName);
        }

        public static string GetConnectionStringFromConfig(string connectionStringName)
        {
            Contract.Requires(connectionStringName != null);
            Contract.Ensures(Contract.Result<string>() != null);

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        "Cannot configure Mongo Persister. No connection string named {0} was found",
                        connectionStringName));
            }

            if (connectionStringSettings.ConnectionString == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Connection string named {0} has a null or empty value", connectionStringName));
            }

            return connectionStringSettings.ConnectionString;
        }
    }
}
