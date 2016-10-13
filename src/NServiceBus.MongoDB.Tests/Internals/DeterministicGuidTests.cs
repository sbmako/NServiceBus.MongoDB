// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeterministicGuidTests.cs" company="SharkByte Software">
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
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.Internals
{
    using System.Collections.Generic;

    using CategoryTraits.Xunit2;

    using FluentAssertions;

    using NServiceBus.MongoDB.Internals;

    using Ploeh.AutoFixture.Xunit2;

    using Xunit;

    public class DeterministicGuidTests
    {
        [Theory, UnitTest]
        [AutoData]
        public void SameProducesSameGuid(KeyValuePair<string, int> testObject)
        {
            var first = DeterministicGuid.Create(testObject);
            var second = DeterministicGuid.Create(testObject);

            first.Should().Be(second);
        }

        [Theory, UnitTest]
        [AutoData]
        public void DifferntProduceDifferentGuids(KeyValuePair<string, int> firstObject, KeyValuePair<string, int> secondObject)
        {
            var first = DeterministicGuid.Create(firstObject);
            var second = DeterministicGuid.Create(secondObject);

            first.Should().NotBe(second);
        }
    }
}
