// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoClientAccessor.cs" company="SharkByte Software">
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
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Internals
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
            Contract.Requires<ArgumentNullException>(mongoClient != null, "mongoClient != null");
            Contract.Requires<ArgumentNullException>(databaseName != null, "databaseName != null");

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
            Contract.Invariant(this.DatabaseName != null);
        }
    }
}
