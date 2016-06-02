// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSubscriptionPersisterTests.cs" company="SharkByte Software">
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
//   Defines the MongoSubscriptionPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SubscriptionPersister
{
    using System.Collections.Generic;
    using System.Linq;

    using CategoryTraits.Xunit2;

    using FluentAssertions;

    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SubscriptionPersister;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    using Xunit;

    public class MongoSubscriptionPersisterTests
    {
        public MongoSubscriptionPersisterTests()
        {
            SubscriptionClassMaps.ConfigureClassMaps();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SingleSubscriptionShouldOnlyCreateOneSubscription(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Subscriber subscriber,
            ContextBag context,
            string messageTypeString)
        {
            var sut = storage as ISubscriptionStorage;
            var messageType = new MessageType(messageTypeString, "1.0.0.0");

            sut.Subscribe(subscriber, messageType, context);

            var subscriptions = storage.GetSubscription(Subscription.FormatId(messageType.AssumedNotNull())).ToList();
            subscriptions.Should().HaveCount(1);

            var subscription = subscriptions.First();
            subscription.MessageType.Should().Be(messageType);
            subscription.Subscribers.Should().HaveCount(1);

            var firstSubscriber = subscription.Subscribers.First();
            firstSubscriber.TransportAddress.Should().Be(subscriber.TransportAddress);
            firstSubscriber.Endpoint.ToString().Should().Be(subscriber.Endpoint.ToString());
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SameClientSubscribesTwiceShouldOnlyCreateOneSubscribtion(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Subscriber subscriber,
            ContextBag context,
            string messageTypeString)
        {
            var sut = storage as ISubscriptionStorage;
            var messageType = new MessageType(messageTypeString, "1.0.0.0");

            sut.Subscribe(subscriber, messageType, context);
            sut.Subscribe(subscriber, messageType, context);

            var subscriptions = storage.GetSubscription(Subscription.FormatId(messageType.AssumedNotNull())).ToList();
            subscriptions.Should().HaveCount(1);

            var subscription = subscriptions.First();
            subscription.MessageType.Should().Be(messageType);
            subscription.Subscribers.Should().HaveCount(1);

            var firstSubscriber = subscription.Subscribers.First();
            firstSubscriber.TransportAddress.Should().Be(subscriber.TransportAddress);
            firstSubscriber.Endpoint.ToString().Should().Be(subscriber.Endpoint.ToString());
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SubscribeTwoMessageTypesShouldCreateTwoDifferentSubscriptions(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Subscriber subscriber,
            ContextBag context,
            string messageTypeString1,
            string messageTypeString2)
        {
            var sut = storage as ISubscriptionStorage;
            var messageType1 = new MessageType(messageTypeString1, "1.0.0.0");
            var messageType2 = new MessageType(messageTypeString2, "1.0.0.0");

            sut.Subscribe(subscriber, messageType1, context);
            sut.Subscribe(subscriber, messageType2, context);

            var subscriptions = storage.GetSubscription(Subscription.FormatId(messageType1.AssumedNotNull())).ToList();
            subscriptions.Should().HaveCount(1);

            var subscription = subscriptions.First();
            subscription.MessageType.Should().Be(messageType1);
            subscription.Subscribers.Should().HaveCount(1);


            subscriptions = storage.GetSubscription(Subscription.FormatId(messageType2.AssumedNotNull())).ToList();
            subscriptions.Should().HaveCount(1);

            subscription = subscriptions.First();
            subscription.MessageType.Should().Be(messageType2);
            subscription.Subscribers.Should().HaveCount(1);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SubscribeTwoClientsOneMessageTypeShouldCreateOneSubscriptionWithMultipleAddresses(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Subscriber subscriber1,
            Subscriber subscriber2,
            ContextBag context,
            string messageTypeString)
        {
            var sut = storage as ISubscriptionStorage;
            var messageType = new MessageType(messageTypeString, "1.0.0.0");

            sut.Subscribe(subscriber1, messageType, context);
            sut.Subscribe(subscriber2, messageType, context);

            var subscribers =
                sut.GetSubscriberAddressesForMessage(new List<MessageType> { messageType }, context).Result.ToList();
            subscribers.Should().HaveCount(2);

            subscribers.Should().ContainSingle(a => subscriber1.TransportAddress == a.TransportAddress);
            subscribers.Should().ContainSingle(a => subscriber2.TransportAddress == a.TransportAddress);
        }

        ////[Theory, IntegrationTest]
        ////[AutoDatabase]
        ////public void UnsubscribeWhenThereIsNoSubscriptionShouldNotCreateSubscription(
        ////    MongoSubscriptionPersister storage,
        ////    MongoDatabaseFactory factory,
        ////    Address client,
        ////    string messageTypeString1)
        ////{
        ////    var sut = storage as ISubscriptionStorage;
        ////    var messageTypes = new List<MessageType>()
        ////                           {
        ////                               new MessageType(messageTypeString1, "1.0.0.0"),
        ////                           };

        ////    sut.Unsubscribe(client, messageTypes);
        ////    storage.GetSubscriptions(messageTypes).Should().BeEmpty();
        ////}

        ////[Theory, IntegrationTest]
        ////[AutoDatabase]
        ////public void UnsubscribeFromAllMessages(
        ////    MongoSubscriptionPersister storage,
        ////    MongoDatabaseFactory factory,
        ////    Address client,
        ////    string messageTypeString1,
        ////    string messageTypeString2,
        ////    string messageTypeString3)
        ////{
        ////    var sut = storage as ISubscriptionStorage;
        ////    var messageTypes = new List<MessageType>()
        ////                           {
        ////                               new MessageType(messageTypeString1, "1.0.0.0"),
        ////                               new MessageType(messageTypeString2, "1.0.0.0"),
        ////                               new MessageType(messageTypeString3, "1.0.0.0"),
        ////                           };

        ////    sut.Subscribe(client, messageTypes);
        ////    storage.GetSubscriptions(messageTypes).Should().HaveCount(3);

        ////    sut.Unsubscribe(client, messageTypes);
        ////    storage.GetSubscriptions(messageTypes).ToList().ForEach(s => s.Clients.Should().HaveCount(0));
        ////}

        ////[Theory, IntegrationTest]
        ////[AutoDatabase]
        ////public void UnsubscribeWhenClientSubscriptionIsTheOnlyOneShouldRemoveOnlyClient(
        ////    MongoSubscriptionPersister storage,
        ////    MongoDatabaseFactory factory,
        ////    Address client,
        ////    string messageTypeString1)
        ////{
        ////    var sut = storage as ISubscriptionStorage;
        ////    var messageTypes = new List<MessageType>()
        ////                           {
        ////                               new MessageType(messageTypeString1, "1.0.0.0"),
        ////                           };

        ////    sut.Subscribe(client, messageTypes);
        ////    storage.GetSubscriptions(messageTypes).Should().HaveCount(1);
        ////    sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(1);

        ////    sut.Unsubscribe(client, messageTypes);
        ////    storage.GetSubscriptions(messageTypes).Should().HaveCount(1);

        ////    sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(0);
        ////}

        ////[Theory, IntegrationTest]
        ////[AutoDatabase]
        ////public void UnsubscribeWhenThereAreSubscriptionsButNotClientsShouldNotChangeAnything(
        ////    MongoSubscriptionPersister storage,
        ////    MongoDatabaseFactory factory,
        ////    Address client,
        ////    Address otherClient1,
        ////    Address otherClient2,
        ////    string messageTypeString1)
        ////{
        ////    var sut = storage as ISubscriptionStorage;
        ////    var messageTypes = new List<MessageType>()
        ////                           {
        ////                               new MessageType(messageTypeString1, "1.0.0.0"),
        ////                           };

        ////    sut.Subscribe(otherClient1, messageTypes);
        ////    sut.Subscribe(otherClient2, messageTypes);
        ////    sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(2);

        ////    sut.Unsubscribe(client, messageTypes);
        ////    var clients = sut.GetSubscriberAddressesForMessage(messageTypes).ToList();
        ////    clients.Should().HaveCount(2);
        ////    clients.First().Should().Be(otherClient1);
        ////    clients.Last().Should().Be(otherClient2);
        ////}

        ////[Theory, IntegrationTest]
        ////[AutoDatabase]
        ////public void UnsubscribeWhenThereAreSubscriptionsShouldRemoveClientsAddress(
        ////    MongoSubscriptionPersister storage,
        ////    MongoDatabaseFactory factory,
        ////    Address client,
        ////    Address otherClient1,
        ////    Address otherClient2,
        ////    string messageTypeString1)
        ////{
        ////    var sut = storage as ISubscriptionStorage;
        ////    var messageTypes = new List<MessageType>()
        ////                           {
        ////                               new MessageType(messageTypeString1, "1.0.0.0"),
        ////                           };

        ////    sut.Subscribe(client, messageTypes);
        ////    sut.Subscribe(otherClient1, messageTypes);
        ////    sut.Subscribe(otherClient2, messageTypes);
        ////    sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(3);

        ////    sut.Unsubscribe(client, messageTypes);
        ////    var clients = sut.GetSubscriberAddressesForMessage(messageTypes).ToList();
        ////    clients.Should().HaveCount(2);
        ////    clients.First().Should().Be(otherClient1);
        ////    clients.Last().Should().Be(otherClient2);
        ////}
    }
}