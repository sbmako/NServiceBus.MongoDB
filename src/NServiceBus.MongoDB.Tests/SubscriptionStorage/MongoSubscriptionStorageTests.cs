// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSubscriptionStorageTests.cs" company="SharkByte Software Inc.">
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
//   Defines the MongoSubscriptionStorageTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SubscriptionStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using NServiceBus.MongoDB.SubscriptionStorage;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;
    using Xunit.Extensions;

    public class MongoSubscriptionStorageTests
    {
        public MongoSubscriptionStorageTests()
        {
            ConfigureMongoSubscriptionStorage.ConfigureClassMaps();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void BasicMongoSubscriptionStorageConstruction(MongoDatabaseFactory factory)
        {
            var sut = new MongoSubscriptionStorage(factory);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SingleSubscriptionShouldOnlyCreateOneSubscription(
            MongoSubscriptionStorage storage,
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
            MongoSubscriptionStorage storage,
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
            MongoSubscriptionStorage storage,
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
            MongoSubscriptionStorage storage,
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
            MongoSubscriptionStorage storage,
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
        public void UnsubscribeWhenClientSubscriptionIsTheOnlyOneShouldRemoveOnlyClient(
            MongoSubscriptionStorage storage,
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
            MongoSubscriptionStorage storage,
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
            MongoSubscriptionStorage storage,
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