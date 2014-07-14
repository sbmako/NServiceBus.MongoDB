// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoPersistenceTests.cs" company="SharkByte Software Inc.">
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
//   Defines the ConfigureMongoPersistenceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests
{
    using System.Configuration;
    using FluentAssertions;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Saga;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    using Xunit.Extensions;

    public class ConfigureMongoPersistenceTests
    {
        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithNoParameters(Configure config)
        {
            config.MongoPersistence();

            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            var clientAccessor = Configure.Instance.Builder.Build<MongoClientAccessor>();
            clientAccessor.DatabaseName.Should().Be(Configure.EndpointName);
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void DefaultMongoPersistenceWithNoParametersCalledTwice(Configure config)
        {
            config.MongoPersistence();
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            config.MongoPersistence();
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithConnectionStringName(Configure config)
        {
            config.MongoPersistence("My.Persistence");
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            var clientAccessor = Configure.Instance.Builder.Build<MongoClientAccessor>();
            clientAccessor.DatabaseName.Should().Be(Configure.EndpointName);
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithWrongConnectionStringName(Configure config)
        {
            config.Invoking(c => c.MongoPersistence("Missing.Persistence"))
                  .ShouldThrow<ConfigurationErrorsException>()
                  .WithMessage("Cannot configure Mongo Persister. No connection string named Missing.Persistence was found");

            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeFalse();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeFalse();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithConnectionStringNameAndDatabaseName(Configure config)
        {
            config.MongoPersistence("My.Persistence", "test");
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            var clientAccessor = Configure.Instance.Builder.Build<MongoClientAccessor>();
            clientAccessor.DatabaseName.Should().Be("test");
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithConnectionStringLambda(Configure config)
        {
            config.MongoPersistence(() => "mongodb://localhost:27017");
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            var clientAccessor = Configure.Instance.Builder.Build<MongoClientAccessor>();
            clientAccessor.DatabaseName.Should().Be(Configure.EndpointName);
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithConnectionStringLambdaAndDatabaseName(Configure config)
        {
            config.MongoPersistence(() => "mongodb://localhost:27017", "test");
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            var clientAccessor = Configure.Instance.Builder.Build<MongoClientAccessor>();
            clientAccessor.DatabaseName.Should().Be("test");
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithConnectionStringLambdaReturningNull(Configure config)
        {
            config.Invoking(c => c.MongoPersistence(() => null))
                  .ShouldThrow<ConfigurationErrorsException>()
                  .WithMessage("Cannot configure Mongo Persister. Connection string can not be null or empty.");

            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeFalse();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeFalse();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithConnectionStringLambdaReturningEmpty(Configure config)
        {
            config.Invoking(c => c.MongoPersistence(() => string.Empty))
                  .ShouldThrow<ConfigurationErrorsException>()
                  .WithMessage("Cannot configure Mongo Persister. Connection string can not be null or empty.");

            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeFalse();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeFalse();
        }
    }
}
