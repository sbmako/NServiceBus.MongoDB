// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoHelpersTests.cs" company="Carlos Sandoval">
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

namespace NServiceBus.MongoDB.Tests.Internals
{
    using System;
    using System.Configuration;

    using FluentAssertions;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Settings;

    using Xunit;

    public class MongoHelpersTests
    {
        [Fact, UnitTest]
        public void GetConnectionStringFromConfigUsingValidConnectionString()
        {
            var result = MongoHelpers.GetConnectionStringFromConfig("My.Persistence");
            result.Should().Be("mongodb://ultratinef:27017");
        }

        [Fact, UnitTest]
        public void GetConnectionStringFromConfigUsingInValidConnectionStringName()
        {
            Action sut = () => MongoHelpers.GetConnectionStringFromConfig("My.MissingPersistence");
            sut.ShouldThrow<ConfigurationErrorsException>();
        }

        [Fact, UnitTest]
        public void GetConnectionUsingSettingsFromConnectionStringName()
        {
            var settings = new SettingsHolder();
            settings.Set(MongoPersistenceConstants.ConnectionStringNameKey, "My.Persistence");
            var readonlySettings = (ReadOnlySettings)settings;

            var result = MongoHelpers.GetConnectionString(readonlySettings);
            result.Should().Be("mongodb://ultratinef:27017");
        }


        [Fact, UnitTest]
        public void GetConnectionUsingSettingsFromConnectionString()
        {
            var settings = new SettingsHolder();
            settings.Set(MongoPersistenceConstants.ConnectionStringKey, "mongodb://ultratinef:27017");
            var readonlySettings = (ReadOnlySettings)settings;

            var result = MongoHelpers.GetConnectionString(readonlySettings);
            result.Should().Be("mongodb://ultratinef:27017");
        }

        [Fact, UnitTest]
        public void GetConnectionUsingSettingsWithInvalidConnectionStringName()
        {
            var settings = new SettingsHolder();
            settings.Set(MongoPersistenceConstants.ConnectionStringNameKey, "My.MissingPersistence");
            var readonlySettings = (ReadOnlySettings)settings;

            Action sut = () => MongoHelpers.GetConnectionString(readonlySettings);
            sut.ShouldThrow<ConfigurationErrorsException>();
        }

        [Fact, UnitTest]
        public void GetConnectionStringUsingSettingsWhenBothConnectionStringAndConnectionStringNameExist()
        {
            var settings = new SettingsHolder();
            settings.Set(MongoPersistenceConstants.ConnectionStringNameKey, "NServiceBus.Persistence");
            settings.Set(MongoPersistenceConstants.ConnectionStringKey, "mongodb://ultratinef:27017");

            var readonlySettings = (ReadOnlySettings)settings;

            var result = MongoHelpers.GetConnectionString(readonlySettings);
            result.Should().Be("mongodb://ultratinef:27017");
        }
    }
}
