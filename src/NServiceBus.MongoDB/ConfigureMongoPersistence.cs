// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoPersistence.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigureMongoPersistence type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    /// <summary>
    /// The configure mongo persistence.
    /// </summary>
    public static class ConfigureMongoPersistence
    {
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
            return config;
        }
    }
}
