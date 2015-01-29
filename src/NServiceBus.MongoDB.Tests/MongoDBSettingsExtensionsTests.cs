// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDBSettingsExtensionsTests.cs" company="Carlos Sandoval">
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
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests
{
    using FluentAssertions;

    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.Tests.TestingUtilities;

    using Xunit.Extensions;

    public class MongoDBSettingsExtensionsTests
    {
        [Theory, UnitTest]
        [AutoConfigureData]
        public void SetConnectionStringName(PersistenceExtentions<MongoDBPersistence> config)
        {
            config.SetConnectionStringName("My.Connection");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.ConnectionStringNameKey)
                .Should()
                .Be("My.Connection");

            config.GetSettings().HasSetting(MongoPersistenceConstants.DatabaseNameKey).Should().BeFalse();
            config.GetSettings().HasSetting(MongoPersistenceConstants.ConnectionStringKey).Should().BeFalse();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void SetDatabaseName(PersistenceExtentions<MongoDBPersistence> config)
        {
            config.SetDatabaseName("MyDatabase");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.DatabaseNameKey)
                .Should()
                .Be("MyDatabase");

            config.GetSettings().HasSetting(MongoPersistenceConstants.ConnectionStringNameKey).Should().BeFalse();
            config.GetSettings().HasSetting(MongoPersistenceConstants.ConnectionStringKey).Should().BeFalse();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void SetConnectionStringNameAndDatabaseName(PersistenceExtentions<MongoDBPersistence> config)
        {
            config.SetConnectionStringName("My.Connection");
            config.SetDatabaseName("MyDatabase");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.ConnectionStringNameKey)
                .Should()
                .Be("My.Connection");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.DatabaseNameKey)
                .Should()
                .Be("MyDatabase");

            config.GetSettings().HasSetting(MongoPersistenceConstants.ConnectionStringKey).Should().BeFalse();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void SetConnectionString(PersistenceExtentions<MongoDBPersistence> config)
        {
            config.SetConnectionString("mongodb://ultratinef:27017");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.ConnectionStringKey)
                .Should()
                .Be("mongodb://ultratinef:27017");
            config.GetSettings().HasSetting(MongoPersistenceConstants.ConnectionStringNameKey).Should().BeFalse();
            config.GetSettings().HasSetting(MongoPersistenceConstants.DatabaseNameKey).Should().BeFalse();
        }

        [Theory, UnitTest]
        [AutoConfigureData]
        public void SetConnectionStringAndDatabaseName(PersistenceExtentions<MongoDBPersistence> config)
        {
            config.SetConnectionString("mongodb://ultratinef:27017");
            config.SetDatabaseName("MyDatabase");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.ConnectionStringKey)
                .Should()
                .Be("mongodb://ultratinef:27017");

            config.GetSettings()
                .Get<string>(MongoPersistenceConstants.DatabaseNameKey)
                .Should()
                .Be("MyDatabase");

            config.GetSettings().HasSetting(MongoPersistenceConstants.ConnectionStringNameKey).Should().BeFalse();
        }
    }
}
