﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoSubscriptionStorage.cs" company="SharkByte Software Inc.">
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
//   The configure mongo subscription storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionStorage
{
    using System;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>(config != null);
            return config;
        }
    }
}
