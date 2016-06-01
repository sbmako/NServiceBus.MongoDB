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
        public Task Subscribe(Subscriber subscriber, MessageType messageType, ContextBag context)
        {
            //// note: taken from NServiceBus.RavenDB persistence: When the subscriber is running V6 and UseLegacyMessageDrivenSubscriptionMode is enabled at the subscriber the 'subcriber.Endpoint' value is null
            var endpoint = subscriber.Endpoint != null ? subscriber.Endpoint.ToString() : subscriber.TransportAddress.Split('@').First();
            var subscriptionClient = new SubscriptionClient { TransportAddress = subscriber.TransportAddress, Endpoint = endpoint };

            var messageTypeLookup = Subscription.FormatId(messageType.AssumedNotNull());
            var existingSubscription = this.GetSubscription(messageTypeLookup).ToArray();

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();

            if (!existingSubscription.Any())
            {
                var newSubscription = new Subscription(messageType);
                newSubscription.Clients.Add(subscriptionClient);

                var insertResult = collection.Insert(newSubscription);
                if (insertResult.HasLastErrorMessage)
                {
                    throw new InvalidOperationException(
                        $"Unable to save {subscriptionClient.TransportAddress} subscription because: {insertResult.LastErrorMessage}");
                }

                return Task.FromResult(0);
            }
            
            var theSubscription = existingSubscription.First();

            var query = theSubscription.MongoUpdateQuery();
            var update = theSubscription.MongoUpdate();
            var updateResult = collection.Update(query, update, UpdateFlags.None);
            if (!updateResult.UpdatedExisting)
            {
                throw new InvalidOperationException(
                    $"Unable to update subscription with id {existingSubscription.First().Id}");
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Unsubscribes the given client from messages of given type.
        /// </summary>
        public Task Unsubscribe(Subscriber subscriber, MessageType messageType, ContextBag context)
        {
            var query = Query<Subscription>.EQ(s => s.Id, Subscription.FormatId(messageType));

            var update = Update<Subscription>.Pull(
                subscription => subscription.Clients,
                new SubscriptionClient(subscriber.TransportAddress, subscriber.Endpoint.ToString()));

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();
            var result = collection.Update(query, update);

            if (result.HasLastErrorMessage)
            {
                throw new InvalidOperationException(
                    $"Unable to unsubscribe {subscriber.TransportAddress} from one of its subscriptions");
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns a list of addresses for subscribers currently subscribed to the given message type.
        /// </summary>
        public Task<IEnumerable<Subscriber>> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes, ContextBag context)
        {
            throw new NotImplementedException();
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

        internal IEnumerable<Subscription> GetSubscription(string messageTypeLookup)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(messageTypeLookup));
            Contract.Ensures(Contract.Result<IEnumerable<Subscription>>() != null);

            var collection = this.mongoDatabase.GetCollection(SubscriptionName).AssumedNotNull();

            var query = Query<Subscription>.EQ(p => p.Id, messageTypeLookup);
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
