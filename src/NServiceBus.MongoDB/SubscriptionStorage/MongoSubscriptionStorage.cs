// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSubscriptionStorage.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the MongoSubscriptionStorage type.
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
