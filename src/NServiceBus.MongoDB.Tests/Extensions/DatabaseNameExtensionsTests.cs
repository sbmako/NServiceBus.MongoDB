// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseNameExtensionsTests.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2016 SharkByte Software
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
//   Defines the DatabaseNameExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.Extensions
{
    using CategoryTraits.Xunit2;

    using FluentAssertions;
    using NServiceBus.MongoDB.Extensions;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Settings;

    using Xunit;
    using Xunit.Extensions;

    public class DatabaseNameExtensionsTests
    {
        [Fact, UnitTest]
        public void VerifyPeriodsAreReplacedWithUnderscores()
        {
            const string EndpointName = "test.endpoint.name";

            var databaseName = EndpointName.AsValidDatabaseName();

            databaseName.Should().Be("test_endpoint_name");
        }

        [Theory]
        [UnitTest]
        [AutoConfigureData]
        public void VerifyGettingEndpointNameFromSettings(SettingsHolder settings)
        {
            settings.DatabaseNameFromEndpointName().Should().Be("UnitTests");
        }
    }
}
