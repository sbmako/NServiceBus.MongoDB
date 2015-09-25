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
        public void BasicMongoSubscriptionStorageConstruction(MongoDatabaseFactory factory)
        {
            var sut = new MongoSubscriptionPersister(factory);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SingleSubscriptionShouldOnlyCreateOneSubscription(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            string messageTypeString)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>() { new MessageType(messageTypeString, "1.0.0.0") };

            sut.Subscribe(client, messageTypes);

            var subscriptions = storage.GetSubscriptions(messageTypes).ToList();
            subscriptions.Should().HaveCount(messageTypes.Count);

            var subscription = subscriptions.First();
            subscription.MessageType.Should().Be(messageTypes.First());
            subscription.Clients.Should().HaveCount(1);
            subscription.Clients.First().Should().Be(client);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SameClientSubscribesTwiceShouldOnlyCreateOneSubscribtion(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            string messageTypeString)
        {
            var client = new Address("testqueue.publisher", "localhost");
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>() { new MessageType(messageTypeString, "1.0.0.0") };

            sut.Subscribe(client, messageTypes);
            sut.Subscribe(client, messageTypes);

            var subscriptions = storage.GetSubscriptions(messageTypes).ToList();
            subscriptions.Should().HaveCount(messageTypes.Count);

            var subscription = subscriptions.First();
            subscription.MessageType.Should().Be(messageTypes.First());
            subscription.Clients.Should().HaveCount(1);
            subscription.Clients.First().Should().Be(client);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SubscribeTwoMessageTypesShouldCreateTwoSubscriptions(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            string messageTypeString1,
            string messageTypeString2)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                       new MessageType(messageTypeString2, "1.0.0.0")
                                   };

            sut.Subscribe(client, messageTypes);

            var subscriptions = storage.GetSubscriptions(messageTypes).ToList();
            subscriptions.Should().HaveCount(messageTypes.Count);

            subscriptions.ForEach(s =>
                {
                    s.Clients.Should().HaveCount(1);
                    s.Clients.First().Should().Be(client);
                });
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SubscribeTwoClientsOneMessageTypeShouldCreateOneSubscriptionWithMultipleAddresses(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client1,
            Address client2,
            string messageTypeString1)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                   };

            sut.Subscribe(client1, messageTypes);
            sut.Subscribe(client2, messageTypes);

            var clients = sut.GetSubscriberAddressesForMessage(messageTypes).ToList();
            clients.Should().HaveCount(2);
            clients.Should().ContainSingle(a => client1 == a);
            clients.Should().ContainSingle(a => client2 == a);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UnsubscribeWhenThereIsNoSubscriptionShouldNotCreateSubscription(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            string messageTypeString1)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                   };

            sut.Unsubscribe(client, messageTypes);
            storage.GetSubscriptions(messageTypes).Should().BeEmpty();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UnsubscribeFromAllMessages(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            string messageTypeString1,
            string messageTypeString2,
            string messageTypeString3)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                       new MessageType(messageTypeString2, "1.0.0.0"),
                                       new MessageType(messageTypeString3, "1.0.0.0"),
                                   };

            sut.Subscribe(client, messageTypes);
            storage.GetSubscriptions(messageTypes).Should().HaveCount(3);

            sut.Unsubscribe(client, messageTypes);
            storage.GetSubscriptions(messageTypes).ToList().ForEach(s => s.Clients.Should().HaveCount(0));
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UnsubscribeWhenClientSubscriptionIsTheOnlyOneShouldRemoveOnlyClient(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            string messageTypeString1)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                   };

            sut.Subscribe(client, messageTypes);
            storage.GetSubscriptions(messageTypes).Should().HaveCount(1);
            sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(1);

            sut.Unsubscribe(client, messageTypes);
            storage.GetSubscriptions(messageTypes).Should().HaveCount(1);

            sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UnsubscribeWhenThereAreSubscriptionsButNotClientsShouldNotChangeAnything(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            Address otherClient1,
            Address otherClient2,
            string messageTypeString1)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                   };

            sut.Subscribe(otherClient1, messageTypes);
            sut.Subscribe(otherClient2, messageTypes);
            sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(2);

            sut.Unsubscribe(client, messageTypes);
            var clients = sut.GetSubscriberAddressesForMessage(messageTypes).ToList();
            clients.Should().HaveCount(2);
            clients.First().Should().Be(otherClient1);
            clients.Last().Should().Be(otherClient2);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UnsubscribeWhenThereAreSubscriptionsShouldRemoveClientsAddress(
            MongoSubscriptionPersister storage,
            MongoDatabaseFactory factory,
            Address client,
            Address otherClient1,
            Address otherClient2,
            string messageTypeString1)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>()
                                   {
                                       new MessageType(messageTypeString1, "1.0.0.0"),
                                   };

            sut.Subscribe(client, messageTypes);
            sut.Subscribe(otherClient1, messageTypes);
            sut.Subscribe(otherClient2, messageTypes);
            sut.GetSubscriberAddressesForMessage(messageTypes).Should().HaveCount(3);

            sut.Unsubscribe(client, messageTypes);
            var clients = sut.GetSubscriberAddressesForMessage(messageTypes).ToList();
            clients.Should().HaveCount(2);
            clients.First().Should().Be(otherClient1);
            clients.Last().Should().Be(otherClient2);
        }
    }
}