// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoGridFSDataBus.cs" company="SharkByte Software">
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

namespace NServiceBus.MongoDB.DataBus
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    using global::MongoDB.Driver.GridFS;
    using NServiceBus.DataBus;
    using NServiceBus.MongoDB.Internals;

    /// <summary>
    /// The MongoDB GridFS data bus.
    /// </summary>
    public class MongoGridFSDataBus : IDataBus
    {
        private readonly MongoGridFS gridFS;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoGridFSDataBus"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The MongoDB factory.
        /// </param>
        public MongoGridFSDataBus(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null, "mongoFactory != null");
            this.gridFS = mongoFactory.GetDatabase().GridFS.AssumedNotNull();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Task<Stream> Get(string key)
        {
            Contract.Ensures(Contract.Result<Stream>() != null);

            return Task.FromResult(this.gridFS.OpenRead(key).AssumedNotNull() as Stream);
        }

        /// <summary>
        /// The put.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="timeToBeReceived">
        /// The time to be received.
        /// </param>
        /// <returns>
        /// The file key.
        /// </returns>
        public Task<string> Put(Stream stream, TimeSpan timeToBeReceived)
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var key = Guid.NewGuid().ToString();
            this.gridFS.Upload(stream, key);

            return Task.FromResult(key.AssumedNotNullOrWhiteSpace());
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task Start()
        {
            return Task.FromResult(0);
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.gridFS != null);
        }
    }
}
