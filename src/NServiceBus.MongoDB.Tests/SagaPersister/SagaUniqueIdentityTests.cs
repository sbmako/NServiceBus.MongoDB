// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaUniqueIdentityTests.cs" company="Carlos Sandoval">
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
//   Defines the SagaUniqueIdentityTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SagaPersister
{
    using System.Collections.Generic;
    using FluentAssertions;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.MongoDB.Utils;
    using Ploeh.AutoFixture.Xunit;
    using Xunit;
    using Xunit.Extensions;

    public class SagaUniqueIdentityTests
    {
        [Theory, UnitTest]
        [AutoData]
        public void SagaUniqueIdentityFormatIdTest(KeyValuePair<string, object> keyValuePair)
        {
            var id = SagaUniqueIdentity.FormatId(typeof(SagaUniqueIdentityTests), keyValuePair);

            Assert.Equal(
                id,
                string.Format(
                    "{0}/{1}/{2}",
                    typeof(SagaUniqueIdentityTests).FullName,
                    keyValuePair.Key.ToString(),
                    DeterministicGuid.Create(keyValuePair.Value.ToString())));
        }

        [Theory, UnitTest]
        [AutoData]
        public void SagaUniqueIdentityFormatIdTheSame(KeyValuePair<string, object> keyValuePair)
        {
            var id1 = SagaUniqueIdentity.FormatId(typeof(SagaUniqueIdentityTests), keyValuePair);
            var id2 = SagaUniqueIdentity.FormatId(typeof(SagaUniqueIdentityTests), keyValuePair);

            id1.Should().Be(id2);
        }

        [Theory, UnitTest]
        [AutoData]
        public void SagaUniqueIdentityFormatIdDifferently(
            KeyValuePair<string, object> keyValuePair1, 
            KeyValuePair<string, object> keyValuePair2)
        {
            var id1 = SagaUniqueIdentity.FormatId(typeof(SagaUniqueIdentityTests), keyValuePair1);
            var id2 = SagaUniqueIdentity.FormatId(typeof(SagaUniqueIdentityTests), keyValuePair2);

            id1.Should().NotBe(id2);
        }
    }
}
