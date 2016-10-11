// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSubscriptionPersister.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 SharkByte Software
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   The MongoDB subscription persister.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionPersister
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    /// <summary>
    /// The MongoDB subscription persister.
    /// </summary>
    public sealed class MongoSubscriptionPersister : ISubscriptionStorage
    {
        private static readonly string SubscriptionName = typeof(Subscription).Name;

        private readonly IMongoCollection<Subscription> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSubscriptionPersister"/> class. 
        /// </summary>
        /// <param name="mongoFactory">
        /// The MongoDB factory.
        /// </param>
        public MongoSubscriptionPersister(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);

            this.collection = mongoFactory.GetDatabase().GetCollection<Subscription>(SubscriptionName).AssumedNotNull();
        }

        /// <summary>
        /// The initialization method.
        /// </summary>
        public void Init()
        {
            // does not need to initialize anything
        }

        /// <summary>
        /// Subscribes the given client to messages of a given type.
        /// </summary>
        /// <param name="subscriber">
        /// The subscriber.
        /// </param>
        /// /// <param name="messageType">
        /// The message type.
        /// </param>
        /// /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The task
        /// </returns>
        public async Task Subscribe(Subscriber subscriber, MessageType messageType, ContextBag context)
        {
            var subscriptionKey = new SubscriptionKey(messageType);
            var update = new UpdateDefinitionBuilder<Subscription>().AddToSet(s => s.Subscribers, subscriber);

            await
                this.collection.UpdateOneAsync(
                    s => s.Id == subscriptionKey,
                    update,
                    new UpdateOptions() { IsUpsert = true }).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribes the given client from messages of given type.
        /// </summary>
        /// <param name="subscriber">
        /// The subscriber.
        /// </param>
        /// /// <param name="messageType">
        /// The message type.
        /// </param>
        /// /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The task
        /// </returns>
        public async Task Unsubscribe(Subscriber subscriber, MessageType messageType, ContextBag context)
        {
            var subscriptionKey = new SubscriptionKey(messageType);

            var update = new UpdateDefinitionBuilder<Subscription>().Pull(s => s.Subscribers, subscriber);

            await
                this.collection.UpdateOneAsync(
                    s => s.Id == subscriptionKey && s.Subscribers.Contains(subscriber),
                    update,
                    new UpdateOptions() { IsUpsert = false }).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a list of addresses for subscribers currently subscribed to the given message type.
        /// </summary>
        /// /// <param name="messageTypes">
        /// The message types.
        /// </param>
        /// /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The collection of subscribers
        /// </returns>
        public async Task<IEnumerable<Subscriber>> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes, ContextBag context)
        {
            var subscriptions = await this.GetSubscriptions(messageTypes.AssumedNotNull());
            var subscribers = subscriptions.SelectMany(s => s.Subscribers).Distinct();
            return subscribers;
        }

        internal async Task<IEnumerable<Subscription>> GetSubscriptions(IEnumerable<MessageType> messageTypes)
        {
            Contract.Requires(messageTypes != null);
            Contract.Ensures(Contract.Result<IEnumerable<Subscription>>() != null);

            var ids = messageTypes.Select(mt => new SubscriptionKey(mt));
            var query = Builders<Subscription>.Filter.In(p => p.Id, ids);
            var result = await this.collection.Find(query).ToListAsync().ConfigureAwait(false);

            return result.AssumedNotNull();
        }

        internal async Task<IEnumerable<Subscription>> GetSubscription(MessageType messageType)
        {
            Contract.Requires(messageType != null);
            Contract.Ensures(Contract.Result<IEnumerable<Subscription>>() != null);

            var query = Builders<Subscription>.Filter.Eq(p => p.Id, new SubscriptionKey(messageType));
            var subscription = await this.collection.Find(query).ToListAsync().ConfigureAwait(false);

            return subscription ?? new List<Subscription>();
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.collection != null);
        }
    }
}
