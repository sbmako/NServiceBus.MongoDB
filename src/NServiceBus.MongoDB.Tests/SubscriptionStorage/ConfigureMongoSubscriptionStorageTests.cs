// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoSubscriptionStorageTests.cs" company="SharkByte Software Inc.">
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
//   Defines the ConfigureMongoSubscriptionStorageTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SubscriptionStorage
{
    using FluentAssertions;
    using NServiceBus.MongoDB.SubscriptionStorage;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using Xunit.Extensions;

    public class ConfigureMongoSubscriptionStorageTests
    {
        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoSubscriptionStorage(Configure config)
        {
            config.MongoSubscriptionStorage();

            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoSubscriptionStorage>().Should().BeTrue();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoSubscriptionStorageAfterPersistenceConfigure(Configure config)
        {
            config.MongoPersistence();
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoSubscriptionStorage>().Should().BeFalse();

            config.MongoSubscriptionStorage();
            Configure.Instance.Configurer.HasComponent<MongoSubscriptionStorage>().Should().BeTrue();
        }
    }
}
