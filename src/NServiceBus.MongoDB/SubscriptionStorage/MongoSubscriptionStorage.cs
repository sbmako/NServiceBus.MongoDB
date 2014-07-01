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
    using global::MongoDB.Driver;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    /// <summary>
    /// The mongo subscription storage.
    /// </summary>
    public class MongoSubscriptionStorage : ISubscriptionStorage
    {
        ////private readonly MongoDatabase mongoDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSubscriptionStorage"/> class.
        /// </summary>
        /// <param name="mongoFactory">
        /// The mongo factory.
        /// </param>
        public MongoSubscriptionStorage(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);
            ////this.mongoDatabase = mongoFactory.GetDatabase();
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
        public void Subscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            ////var messageTypeLookup = messageTypes.ToDictionary(Subscription.FormatId);

            ////var query = Query<Subscription>.EQ(e => e.Id, sagaId);
            ////var entity = this.mongoDatabase.GetCollection<T>(typeof(T).Name).FindOne(query);

            ////using (var session = OpenSession())
            ////{
            ////var existingSubscriptions =
            ////    GetSubscriptions(messageTypeLookup.Values, this.mongoDatabase).ToLookup(m => m.Id);

            ////var newAndExistingSubscriptions = messageTypeLookup
            ////        .Select(id => existingSubscriptions[id.Key].SingleOrDefault() ?? StoreNewSubscription(session, id.Key, id.Value))
            ////        .Where(subscription => subscription.Clients.All(c => c != client)).ToArray();

            ////    foreach (var subscription in newAndExistingSubscriptions)
            ////    {
            ////        subscription.Clients.Add(client);
            ////    }

            ////    session.SaveChanges();
            ////}
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

        ////private static IEnumerable<Subscription> GetSubscriptions(IEnumerable<MessageType> messageTypes, MongoDatabase database)
        ////{
        ////    var ids = messageTypes
        ////        .Select(Subscription.FormatId);

        ////    return database.Load<Subscription>(ids).Where(s => s != null);
        ////}

        ////private static Subscription StoreNewSubscription(IDocumentSession session, string id, MessageType messageType)
        ////{
        ////    var subscription = new Subscription { Clients = new List<Address>(), Id = id, MessageType = messageType };
        ////    session.Store(subscription);

        ////    return subscription;
        ////}
    }
}
