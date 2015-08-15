// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoPersistenceConstants.cs" company="SharkByte Software">
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
//   Defines the MongoPersistenceConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Internals
{
    using System.Diagnostics.Contracts;

    internal static class MongoPersistenceConstants
    {
        public const int DefaultNextTimeoutIncrementMinutes = 10;

        public const string VersionPropertyName = "DocumentVersion";

        public const string OwningTimeoutManagerAndTimeName = "OwningTimeoutManagerAndTime";

        public const string OwningTimeoutManagerAndSagaIdAndTimeName = "OwningTimeoutManagerAndSagaIdAndTime";
        
        public const int DefaultPort = 27017;

        public const string DefaultHost = "localhost";

        public const string FallbackConnectionStringName = "NServiceBus/Persistence";

        public const string DefaultConnectionStringName = "NServiceBus/Persistence/MongoDB";

        public const string ConnectionStringKey = "MongoConnectionStringKey";

        public const string DatabaseNameKey = "MongoDatabaseNameKey";

        public static string DefaultConnectionString
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                var connectionString = string.Format("mongodb://{0}:{1}", DefaultHost, DefaultPort);
                return connectionString.AssumedNotNullOrWhiteSpace();
            }
        }
    }
}
