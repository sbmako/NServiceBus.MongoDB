// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoTimeoutPersister.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 Carlos Sandoval. All rights reserved.
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Affero General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Affero General Public License for more details.
//   
//   You should have received a copy of the GNU Affero General Public License
//   along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
//   Defines the ConfigureMongoTimeoutPersister type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.TimeoutPersister
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The configure mongo timeout persister.
    /// </summary>
    public static class ConfigureMongoTimeoutPersister
    {
        /// <summary>
        /// The mongo timeout persister.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="Configure"/>.
        /// </returns>
        public static Configure MongoTimeoutPersister(this Configure config)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Ensures(Contract.Result<Configure>() != null);

            if (!config.Configurer.HasComponent<MongoClientAccessor>())
            {
                config.MongoPersistence();
            }

            config.Configurer.ConfigureComponent<MongoTimeoutPersister>(DependencyLifecycle.SingleInstance);

            return config;
        }
    }
}
