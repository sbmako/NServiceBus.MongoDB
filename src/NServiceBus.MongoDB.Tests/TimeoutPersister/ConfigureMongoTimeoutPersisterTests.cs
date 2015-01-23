// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoTimeoutPersisterTests.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Carlos Sandoval
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
//   Defines the ConfigureMongoTimeoutPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TimeoutPersister
{
    using FluentAssertions;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.MongoDB.TimeoutPersister;
    using Xunit.Extensions;

    public class ConfigureMongoTimeoutPersisterTests
    {
        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoTimeoutPersisterSingleCall(Configure config)
        {
            ////config.MongoTimeoutPersister();

            ////Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoTimeoutPersister>().Should().BeTrue();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoTimeoutPersisterAfterPersistenceConfigure(Configure config)
        {
            ////config.MongoPersistence();
            ////Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoTimeoutPersister>().Should().BeFalse();

            ////config.MongoTimeoutPersister();
            ////Configure.Instance.Configurer.HasComponent<MongoTimeoutPersister>().Should().BeTrue();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void MongoTimeoutPersisterCalledTwice(Configure config)
        {
            ////config.MongoTimeoutPersister();

            ////Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoTimeoutPersister>().Should().BeTrue();

            ////config.MongoTimeoutPersister();

            ////Configure.Instance.Configurer.HasComponent<MongoClientAccessor>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoDatabaseFactory>().Should().BeTrue();
            ////Configure.Instance.Configurer.HasComponent<MongoTimeoutPersister>().Should().BeTrue();
        }
    }
}
