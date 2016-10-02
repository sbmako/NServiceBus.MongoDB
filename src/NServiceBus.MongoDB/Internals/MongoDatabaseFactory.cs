// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDatabaseFactory.cs" company="SharkByte Software">
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
//   The mongo database factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Internals
{
    using System;
    using System.Diagnostics.Contracts;

    using global::MongoDB.Driver;

    /// <summary>
    /// The MongoDB database factory.
    /// </summary>
    public class MongoDatabaseFactory
    {
        private readonly MongoClientAccessor mongoClientAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDatabaseFactory"/> class.
        /// </summary>
        /// <param name="clientAccessor">
        /// The clientAccessor.
        /// </param>
        public MongoDatabaseFactory(MongoClientAccessor clientAccessor)
        {
            Contract.Requires<ArgumentNullException>(clientAccessor != null, "clientAccessor != null");
            this.mongoClientAccessor = clientAccessor;
        }

        /// <summary>
        /// The get database.
        /// </summary>
        /// <returns>
        /// The <see cref="MongoDatabase"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Ok here")]
        public IMongoDatabase GetDatabase()
        {
            Contract.Ensures(Contract.Result<IMongoDatabase>() != null);

            var databaseName = this.mongoClientAccessor.DatabaseName;
            return this.mongoClientAccessor.MongoClient.GetDatabase(databaseName).AssumedNotNull();
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.mongoClientAccessor != null);
        }
    }
}
