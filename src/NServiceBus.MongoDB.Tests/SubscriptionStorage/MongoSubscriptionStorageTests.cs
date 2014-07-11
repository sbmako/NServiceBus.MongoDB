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
        public void BasicMessageSubscription(
            MongoSubscriptionStorage storage,
            MongoDatabaseFactory factory,
            Address client,
            string messageTypeString)
        {
            var sut = storage as ISubscriptionStorage;
            var messageTypes = new List<MessageType>() { new MessageType(messageTypeString, "1.0.0.0") };

            sut.Subscribe(client, messageTypes);

            var subscriptions = storage.GetSubscriptions(messageTypes).ToList();
            subscriptions.Count.Should().Be(messageTypes.Count);

            var subscription = subscriptions.First();
            subscription.Id.Should().Be(Subscription.FormatId(messageTypes.First()));
            subscription.MessageType.Should().Be(messageTypes.First());
            subscription.DocumentVersion.Should().Be(0);
            subscription.Clients.Count.Should().Be(1);
            subscription.Clients.First().Should().Be(client);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void BasicMessageSubscriptionTwice(
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
            subscriptions.Count.Should().Be(messageTypes.Count);

            var subscription = subscriptions.First();
            subscription.Id.Should().Be(Subscription.FormatId(messageTypes.First()));
            subscription.MessageType.Should().Be(messageTypes.First());
            subscription.DocumentVersion.Should().Be(0);
            subscription.Clients.Count.Should().Be(1);
            subscription.Clients.First().Should().Be(client);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void MessageSubscriptionTwoMessageTypes(
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
            subscriptions.Count.Should().Be(messageTypes.Count);

            var ids = messageTypes.Select(Subscription.FormatId).ToList();

            subscriptions.ForEach(s =>
                {
                    s.Id.Should().BeOneOf(ids);
                    s.DocumentVersion.Should().Be(0);
                    s.Clients.Count.Should().Be(1);
                    s.Clients.First().Should().Be(client);
                });
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void MessageSubscriptionTwoClientsOneMessageTypes(
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
            clients.Should().ContainSingle(a => client1 == a);
            clients.Should().ContainSingle(a => client2 == a);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UnsubscribeWhenThereIsNoSubscription(
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
    }
}
