// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoPersistenceTests.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Carlos Sandoval
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
//   Defines the ConfigureMongoPersistenceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests
{
    using System.Configuration;
    using System.Linq;

    using FluentAssertions;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
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
            clientAccessor.DatabaseName.Should().Be("Unit_Tests");
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
            clientAccessor.DatabaseName.Should().Be("Unit_Tests");
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
            clientAccessor.DatabaseName.Should().Be("Unit_Tests");
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoPersistenceWithReplicaSetConnectionStringLambda(Configure config)
        {
            config.MongoPersistence(() => "mongodb://localhost:27017,localhost:27017,localhost:27017");
            Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();

            var clientAccessor = Configure.Instance.Builder.Build<MongoClientAccessor>();
            clientAccessor.MongoClient.Settings.Servers.Count().Should().BeGreaterThan(1);
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
