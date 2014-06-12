// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoSagaPersister.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigureMongoSagaPersister type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SagaPersister
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The configure mongo saga persister.
    /// </summary>
    public static class ConfigureMongoSagaPersister
    {
        /// <summary>
        /// The mongo saga persister.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoSagaPersister(this Configure config)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            return config;
        }
    }
}
