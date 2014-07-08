// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSubscriptionStorage.cs" company="SharkByte Software Inc.">
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
//   The mongo subscription storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionStorage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    /// <summary>
    /// The mongo subscription storage.
    /// </summary>
    public sealed class MongoSubscriptionStorage : ISubscriptionStorage
    {
        private static readonly string SubscriptionName = typeof(Subscription).Name;

        private readonly MongoDatabase mongoDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSubscriptionStorage"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The mongo factory.
        /// </param>
        public MongoSubscriptionStorage(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);
            this.mongoDatabase = mongoFactory.GetDatabase();
        }

        /// <summary>
        /// The initialization method.
        /// </summary>
        public void Init()
        {
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
        void ISubscriptionStorage.Subscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            var messageTypeLookup = messageTypes.ToDictionary(Subscription.FormatId);

            var existingSubscriptions = this.GetSubscriptions(messageTypeLookup.Values).ToDictionary(m => m.Id);
            var newSubscriptions = new List<Subscription>();

            messageTypeLookup.ToList().ForEach(
                mt =>
                    {
                        if (!existingSubscriptions.ContainsKey(mt.Key))
                        {
                            newSubscriptions.Add(new Subscription(mt.Value, new List<Address>() { client }));
                            return;
                        }

                        var existing = existingSubscriptions[mt.Key];

                        if (existing.Clients.All(c => c != client))
                        {
                            existing.Clients.Add(client);
                        }
                    });

            var collection = this.mongoDatabase.GetCollection(SubscriptionName);

            existingSubscriptions.Values.ToList().ForEach(
                s =>
                    {
                        var query = s.MongoUpdateQuery();
                        var update = s.MongoUpdate();
                        var result = collection.Update(query, update, UpdateFlags.None);
                        if (!result.UpdatedExisting)
                        {
                            throw new InvalidOperationException(
                                string.Format("Unable to update subscription with id {0}", s.Id));
                        }
                    });

            newSubscriptions.ForEach(
                s =>
                    {
                        var result = collection.Insert(s);

                        if (!result.Ok)
                        {
                            throw new InvalidOperationException(
                                string.Format("Unable to save subscription with id {0}", s.Id));
                        }
                    });
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
        void ISubscriptionStorage.Unsubscribe(Address client, IEnumerable<MessageType> messageTypes)
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
        IEnumerable<Address> ISubscriptionStorage.GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes)
        {
            var subscriptions = this.GetSubscriptions(messageTypes);
            return subscriptions.SelectMany(s => s.Clients).Distinct().ToArray();
        }

        internal IEnumerable<Subscription> GetSubscriptions(IEnumerable<MessageType> messageTypes)
        {
            Contract.Requires(messageTypes != null);
            Contract.Ensures(Contract.Result<IEnumerable<Subscription>>() != null);

            var ids = messageTypes.Select(Subscription.FormatId);
            var query = Query<Subscription>.In(p => p.Id, ids);
            return this.mongoDatabase.GetCollection<Subscription>(SubscriptionName).Find(query);
        }
    }
}
