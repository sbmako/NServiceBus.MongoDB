// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaUniqueIdentityTests.cs" company="SharkByte Software Inc.">
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
