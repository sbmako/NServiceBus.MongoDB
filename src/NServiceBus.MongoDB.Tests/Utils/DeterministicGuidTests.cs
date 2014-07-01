// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeterministicGuidTests.cs" company="SharkByte Software Inc.">
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
//   Defines the DeterministicGuidTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.Utils
{
    using System.Collections.Generic;
    using FluentAssertions;
    using NServiceBus.MongoDB.Utils;
    using Ploeh.AutoFixture.Xunit;
    using Xunit.Extensions;

    public class DeterministicGuidTests
    {
        [Theory, AutoData]
        public void SameProducesSameGuid(KeyValuePair<string, int> testObject)
        {
            var first = DeterministicGuid.Create(testObject);
            var second = DeterministicGuid.Create(testObject);

            first.Should().Be(second);
        }

        [Theory, AutoData]
        public void DifferntProduceDifferentGuids(KeyValuePair<string, int> firstObject, KeyValuePair<string, int> secondObject)
        {
            var first = DeterministicGuid.Create(firstObject);
            var second = DeterministicGuid.Create(secondObject);

            first.Should().NotBe(second);
        }
    }
}
