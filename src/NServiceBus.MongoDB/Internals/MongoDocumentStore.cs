// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDocumentStore.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2017 SharkByte Software
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
    using System.Diagnostics.Contracts;

    using global::MongoDB.Driver;

    using NServiceBus.Features;
    using NServiceBus.MongoDB.Extensions;

    /// <summary>
    /// The MongoDB document store.
    /// </summary>
    public class MongoDocumentStore : Feature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDocumentStore"/> class.
        /// </summary>
        public MongoDocumentStore()
        {
            this.Defaults(
                s =>
                    {
                        s.SetDefault(MongoPersistenceConstants.DatabaseNameKey, s.DatabaseNameFromEndpointName());
                    });
        }

        /// <summary>
        /// The setup.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Setup(FeatureConfigurationContext context)
        {
            var databaseName = context.Settings.Get<string>(MongoPersistenceConstants.DatabaseNameKey);

            context.Container.ConfigureComponent(
                () =>
                    {
                        var connectionString = MongoHelpers.GetConnectionString(context.Settings);
                        return InternalMongoClientAccessorFactory(connectionString, databaseName);
                    }, 
                DependencyLifecycle.SingleInstance);

            context.Container.ConfigureComponent<MongoDatabaseFactory>(DependencyLifecycle.SingleInstance);
        }

        private static MongoClientAccessor InternalMongoClientAccessorFactory(
            string connectionString, 
            string databaseName)
        {
            Contract.Requires(connectionString != null);
            Contract.Requires(databaseName != null);
            Contract.Ensures(Contract.Result<MongoClientAccessor>() != null);

            var client = new MongoClient(connectionString);
            var clientAccessor = new MongoClientAccessor(client, databaseName);

            return clientAccessor;
        }
    }
}
