// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoSagaPersisterTests.cs" company="SharkByte Software Inc.">
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
//   Defines the ConfigureMongoSagaPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SagaPersister
{
    using FluentAssertions;

    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using Xunit.Extensions;

    public class ConfigureMongoSagaPersisterTests
    {
        [Theory]
        [AutoConfigureData]
        public void MongoSagaPersistence(Configure config)
        {
            config.MongoSagaPersister();

            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoSagaPersister>().Should().BeTrue();
        }

        [Theory]
        [AutoConfigureData]
        public void MongoSagaPersistenceAfterPersistenceConfigure(Configure config)
        {
            config.MongoPersistence();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            config.MongoSagaPersister();
            Configure.Instance.Configurer.HasComponent<MongoSagaPersister>().Should().BeTrue();
        }
    }
}
