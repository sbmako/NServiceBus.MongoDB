// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersisterTests.cs" company="Carlos Sandoval">
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
//   Defines the MongoTimeoutPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TimeoutPersister
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.MongoDB.TimeoutPersister;
    using NServiceBus.Timeout.Core;
    using Xunit.Extensions;

    public class MongoTimeoutPersisterTests
    {
        [Theory, IntegrationTest]
        [AutoDatabase]
        public void BasicMongoTimeoutPersisterConstruction(MongoDatabaseFactory factory)
        {
            var sut = new MongoTimeoutPersister(factory);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsEmptyListWhenCollectionDoesNotExist(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory)
        {
            sut.Initialize();
            factory.ResetTimeoutCollection();

            var startSlice = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5));
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().BeEmpty();
            nextTimeToRunQuery.Should()
                              .BeOnOrAfter(
                                  startSlice.AddMinutes(MongoPersistenceConstants.DefaultNextTimeoutIncrementMinutes));
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsOneTimeoutWhenCollectionHasOneTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
        {
            sut.Initialize();
            factory.ResetTimeoutCollection();
            timeoutData.Time = DateTime.UtcNow.AddMinutes(-1);
            sut.Add(timeoutData);

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().HaveCount(1);
            nextTimeToRunQuery.Should().BeOnOrAfter(timeoutData.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsOneTimeoutWhenCollectionHasOneTimeoutBetweenStartSliceAndUtcNowAndOneAfterUtcNow(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
        {
            sut.Initialize();
            factory.ResetTimeoutCollection();
            timeoutData.Time = DateTime.UtcNow.AddMinutes(-1);
            sut.Add(timeoutData);
            timeoutData.Time = DateTime.UtcNow.AddMinutes(1);
            sut.Add(timeoutData);

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().HaveCount(1);
            nextTimeToRunQuery.Should().BeOnOrBefore(timeoutData.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsEmptyListWhenCollectionHasTwoTimeoutsAfterUtcNow(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
        {
            sut.Initialize();
            factory.ResetTimeoutCollection();
            timeoutData.Time = DateTime.UtcNow.AddMinutes(1);
            sut.Add(timeoutData);
            sut.Add(timeoutData);

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().HaveCount(0);
            nextTimeToRunQuery.Should().BeOnOrBefore(timeoutData.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void AddOneTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
        {
            sut.Initialize();
            factory.ResetTimeoutCollection();
            sut.Add(timeoutData);

            var result = factory.RetrieveAllTimeouts();

            result.Should().HaveCount(1);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void AddTwoDifferentTimeouts(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeout1,
            TimeoutData timeout2)
        {
            sut.Initialize();
            factory.ResetTimeoutCollection();
            sut.Add(timeout1);
            sut.Add(timeout2);

            var result = factory.RetrieveAllTimeouts();

            result.Should().HaveCount(2);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveShouldSucceedAndReturnData(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
        {      
            sut.Initialize();
            factory.ResetTimeoutCollection();
            sut.Add(timeoutData);
            TimeoutData returnedTimeoutData;

            var timeouts = factory.RetrieveAllTimeouts();

            var result = sut.TryRemove(timeouts.First().Id, out returnedTimeoutData);

            result.Should().BeTrue();
            returnedTimeoutData.ShouldBeEquivalentTo(timeoutData);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveEmptyTimeoutCollectionShouldReturnFalseAndNullData(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            string timeoutId)
        {
            factory.ResetTimeoutCollection();
            TimeoutData timeoutData;

            var result = sut.TryRemove(timeoutId, out timeoutData);

            result.Should().BeFalse();
            timeoutData.Should().BeNull();
        }
    }
}
