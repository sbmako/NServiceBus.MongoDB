// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSubscriptionStorage.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
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
//   The mongo subscription storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionStorage
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    /// <summary>
    /// The mongo subscription storage.
    /// </summary>
    public class MongoSubscriptionStorage : ISubscriptionStorage
    {
        /// <summary>
        /// The initialization method.
        /// </summary>
        public void Init()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="messageTypes">
        /// The message types.
        /// </param>
        public void Subscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The unsubscribe.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="messageTypes">
        /// The message types.
        /// </param>
        public void Unsubscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
        }

        /// <summary>
        /// The get subscriber addresses for message.
        /// </summary>
        /// <param name="messageTypes">
        /// The message types.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Address> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes)
        {
            throw new NotImplementedException();
        }
    }
}
