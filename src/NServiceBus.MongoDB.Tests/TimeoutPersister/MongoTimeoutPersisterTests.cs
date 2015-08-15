// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersisterTests.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 SharkByte Software
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

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.MongoDB.TimeoutPersister;
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
            factory.ResetTimeoutCollection();

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
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
            Timeout.Core.TimeoutData timeoutData)
        {
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
            Timeout.Core.TimeoutData timeoutData)
        {
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
            Timeout.Core.TimeoutData timeoutData)
        {
            factory.ResetTimeoutCollection();

            timeoutData.Time = DateTime.UtcNow.AddMinutes(1);
            sut.Add(timeoutData);
            sut.Add(timeoutData);

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().HaveCount(0);
            nextTimeToRunQuery.Should().Be(timeoutData.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void AddOneTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData)
        {
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
            Timeout.Core.TimeoutData timeout1,
            Timeout.Core.TimeoutData timeout2)
        {
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
            Timeout.Core.TimeoutData timeoutData)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData);
            Timeout.Core.TimeoutData returnedTimeoutData;

            var timeouts = factory.RetrieveAllTimeouts();

            var result = sut.TryRemove(timeouts.First().Id.ToString(), out returnedTimeoutData);

            result.Should().BeTrue();
            returnedTimeoutData.ShouldBeEquivalentTo(timeoutData);

            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveEmptyTimeoutCollectionShouldReturnFalseAndNullData(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            string timeoutId)
        {
            factory.ResetTimeoutCollection();

            Timeout.Core.TimeoutData timeoutData;

            var result = sut.TryRemove(timeoutId, out timeoutData);

            result.Should().BeFalse();
            timeoutData.Should().BeNull();

            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveShouldSucceedAndReturnDataForOneTimeoutAndLeaveTheOther(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData1,
            Timeout.Core.TimeoutData timeoutData2)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData1);
            sut.Add(timeoutData2);
            Timeout.Core.TimeoutData returnedTimeoutData;

            var timeouts = factory.RetrieveAllTimeouts();

            var result = sut.TryRemove(timeouts.First().Id.ToString(), out returnedTimeoutData);

            result.Should().BeTrue();
            returnedTimeoutData.ShouldBeEquivalentTo(timeoutData1);

            var remainingTimeout = factory.RetrieveAllTimeouts().ToList();
            remainingTimeout.Should().HaveCount(1);
            remainingTimeout.First().ShouldBeEquivalentTo(timeoutData2);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdRemovesTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData)
        {
            factory.ResetTimeoutCollection();
            
            sut.Add(timeoutData);

            sut.RemoveTimeoutBy(timeoutData.SagaId);
            
            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdOnEmptyTimeoutCollection(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData)
        {
            factory.ResetTimeoutCollection();

            sut.RemoveTimeoutBy(timeoutData.SagaId);

            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdOnNonExistantIdDoesNotRemoveOtherTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData);

            sut.RemoveTimeoutBy(Guid.NewGuid());

            factory.RetrieveAllTimeouts().Should().HaveCount(1);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdRemovesCorrectTimeoutAndDoesNotRemoveOtherTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData1,
            Timeout.Core.TimeoutData timeoutData2)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData1);
            sut.Add(timeoutData2);

            sut.RemoveTimeoutBy(timeoutData2.SagaId);

            var remainingTimeouts = factory.RetrieveAllTimeouts().ToList();
            remainingTimeouts.Should().HaveCount(1);

            remainingTimeouts.First().SagaId.Should().Be(timeoutData1.SagaId);
        }
    }
}
