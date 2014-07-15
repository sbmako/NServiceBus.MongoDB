// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndpointConfig.cs" company="SharkByte Software Inc.">
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
//   Defines the EndpointConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sample
{
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.SubscriptionStorage;
    using NServiceBus.MongoDB.TimeoutPersister;

    /// <summary>
    /// The endpoint config.
    /// </summary>
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        /// <summary>
        /// The initialization method.
        /// </summary>
        public void Init()
        {
            Configure.Serialization.Json();

            Configure.Features.Disable<Audit>();

            Configure.With()
                     .DefaultBuilder()
                     .UnicastBus()
                     .MongoSagaPersister()
                     .MongoSubscriptionStorage()
                     .MongoTimeoutPersister();
        }
    }
}
