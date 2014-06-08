// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoSubscriptionStorage.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigureMongoSubscriptionStorage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionStorage
{
    /// <summary>
    /// The configure mongo subscription storage.
    /// </summary>
    public static class ConfigureMongoSubscriptionStorage
    {
        /// <summary>
        /// The mongo subscription storage.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoSubscriptionStorage(this Configure config)
        {
            return config;
        }
    }
}
