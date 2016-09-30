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
    using global::MongoDB.Driver.Builders;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Routing;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    /// <summary>
    /// The MongoDB subscription persister.
    /// </summary>
    public sealed class MongoSubscriptionPersister : ISubscriptionStorage
    {
        private static readonly string SubscriptionName = typeof(Subscription).Name;

        private readonly MongoDatabase mongoDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSubscriptionPersister"/> class. 
        /// </summary>
        /// <param name="mongoFactory">
        /// The MongoDB factory.
        /// </param>
        public MongoSubscriptionPersister(MongoDatabaseFactory mongoFactory)
        {
            Contract.Requires<ArgumentNullException>(mongoFactory != null);
            this.mongoDatabase = mongoFactory.GetDatabase();
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
        public Task Subscribe(Subscriber subscriber, MessageType messageType, ContextBag context)
        {
            var existingSubscription = this.GetSubscription(messageType);

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();

            if (!existingSubscription.Any())
            {
                var newSubscription = new Subscription(messageType);
                newSubscription.Subscribers.Add(subscriber);

                var insertResult = collection.Insert(newSubscription);
                if (insertResult.HasLastErrorMessage)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Unable to save {0} subscription because: {1}",
                            subscriber.TransportAddress,
                            insertResult.LastErrorMessage));
                }

                return Task.FromResult(0);
            }
            
            var theSubscription = existingSubscription.First();

            if (
                theSubscription.Subscribers.Exists(
                    c => c.TransportAddress == subscriber.TransportAddress && c.Endpoint == subscriber.Endpoint))
            {
                return Task.FromResult(0);
            }

            theSubscription.Subscribers.Add(subscriber);

            var query = theSubscription.MongoUpdateQuery();
            var update = theSubscription.MongoUpdate();
            var updateResult = collection.Update(query, update, UpdateFlags.None);
            if (!updateResult.UpdatedExisting)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to update subscription with id {0}", existingSubscription.First().Id));
            }

            return Task.FromResult(0);
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
        public Task Unsubscribe(Subscriber subscriber, MessageType messageType, ContextBag context)
        {
            var query = Query<Subscription>.EQ(s => s.Id, Subscription.FormatId(messageType));

            var update = Update<Subscription>.Pull(subscription => subscription.Subscribers, subscriber);

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();
            var result = collection.Update(query, update);

            if (result.HasLastErrorMessage)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to unsubscribe {0} from one of its subscriptions",
                        subscriber.TransportAddress));
            }

            return Task.FromResult(0);
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
        public Task<IEnumerable<Subscriber>> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes, ContextBag context)
        {
            var subscriptions = this.GetSubscriptions(messageTypes.AssumedNotNull());
            var subscribers = subscriptions.SelectMany(s => s.Subscribers).Distinct();
            return Task.FromResult(subscribers);
        }

        internal IEnumerable<Subscription> GetSubscriptions(IEnumerable<MessageType> messageTypes)
        {
            Contract.Requires(messageTypes != null);
            Contract.Ensures(Contract.Result<IEnumerable<Subscription>>() != null);

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();

            var ids = messageTypes.Select(Subscription.FormatId);
            var query = Query<Subscription>.In(p => p.Id, ids);
            var result = collection.FindAs<Subscription>(query);

            return result.AssumedNotNull();
        }

        internal IEnumerable<Subscription> GetSubscription(MessageType messageType)
        {
            Contract.Requires(messageType != null);
            Contract.Ensures(Contract.Result<IEnumerable<Subscription>>() != null);

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();

            var query = Query<Subscription>.EQ(p => p.Id, Subscription.FormatId(messageType.AssumedNotNull()));
            var subscription = collection.FindOneAs<Subscription>(query);

            return subscription == null ? new List<Subscription>() : new List<Subscription>() { subscription };
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.mongoDatabase != null);
        }
    }
}
