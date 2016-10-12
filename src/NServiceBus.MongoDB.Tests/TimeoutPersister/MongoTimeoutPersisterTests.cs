// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersisterTests.cs" company="SharkByte Software">
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
//   Defines the MongoTimeoutPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NServiceBus.Extensibility;

namespace NServiceBus.MongoDB.Tests.TimeoutPersister
{
    using System;
    using System.Linq;

    using CategoryTraits.Xunit2;

    using FluentAssertions;

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.MongoDB.TimeoutPersister;

    using Xunit;

    public class MongoTimeoutPersisterTests
    {
        [Theory, IntegrationTest]
        [AutoDatabase]
        public void BasicMongoTimeoutPersisterConstruction(MongoDatabaseFactory factory)
        {
            var sut = new MongoTimeoutPersister(factory, "UnitTests");
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsEmptyListWhenCollectionDoesNotExist(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory)
        {
            factory.ResetTimeoutCollection();

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            var result = sut.GetNextChunk(startSlice).Result;

            result.DueTimeouts.Should().BeEmpty();
            result.NextTimeToQuery.Should()
                .BeOnOrAfter(startSlice.AddMinutes(MongoPersistenceConstants.DefaultNextTimeoutIncrementMinutes));
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsOneTimeoutWhenCollectionHasOneTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            timeoutData.Time = DateTime.UtcNow.AddMinutes(-1);
            sut.Add(timeoutData, context).Wait();

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            var result = sut.GetNextChunk(startSlice).Result;

            result.DueTimeouts.Should().HaveCount(1);
            result.NextTimeToQuery.Should().BeOnOrAfter(timeoutData.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsOneTimeoutWhenCollectionHasOneTimeoutBetweenStartSliceAndUtcNowAndOneAfterUtcNow(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData1,
            Timeout.Core.TimeoutData timeoutData2,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            timeoutData1.Time = DateTime.UtcNow.AddMinutes(-1);
            sut.Add(timeoutData1, context).Wait();
            timeoutData2.Time = DateTime.UtcNow.AddMinutes(1);
            sut.Add(timeoutData2, context).Wait();

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            var result = sut.GetNextChunk(startSlice).Result;

            result.DueTimeouts.Should().HaveCount(1);
            result.NextTimeToQuery.Should().BeOnOrBefore(timeoutData2.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsEmptyListWhenCollectionHasTwoTimeoutsAfterUtcNow(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData1,
            Timeout.Core.TimeoutData timeoutData2,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            timeoutData1.Time = DateTime.UtcNow.AddMinutes(1);
            timeoutData2.Time = DateTime.UtcNow.AddMinutes(1);
            sut.Add(timeoutData1, context).Wait();
            sut.Add(timeoutData2, context).Wait();

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            var result = sut.GetNextChunk(startSlice).Result;

            result.DueTimeouts.Should().HaveCount(0);
            result.NextTimeToQuery.Should().Be(timeoutData2.Time);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void AddOneTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData, context).Wait();

            var result = factory.RetrieveAllTimeouts();

            result.Should().HaveCount(1);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void AddTwoDifferentTimeouts(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeout1,
            Timeout.Core.TimeoutData timeout2,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeout1, context).Wait();
            sut.Add(timeout2, context).Wait();

            var result = factory.RetrieveAllTimeouts();

            result.Should().HaveCount(2);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void PeekExistingTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeout1,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeout1, context).Wait();

            var peeked = sut.Peek(timeout1.Id, context).Result;
            var result = factory.RetrieveAllTimeouts();

            peeked.ShouldBeEquivalentTo(timeout1);
            result.Should().HaveCount(1);
        }



        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveShouldSucceedAndReturnData(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData, context).Wait();

            var timeouts = factory.RetrieveAllTimeouts();

            var result = sut.TryRemove(timeouts.First().Id.ToString(), context).Result;

            result.Should().BeTrue();
            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveEmptyTimeoutCollectionShouldReturnFalseAndNullData(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            string timeoutId,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            var result = sut.TryRemove(timeoutId, context).Result;

            result.Should().BeFalse();
            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void TryRemoveShouldSucceedAndReturnDataForOneTimeoutAndLeaveTheOther(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData1,
            Timeout.Core.TimeoutData timeoutData2,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData1, context).Wait();
            sut.Add(timeoutData2, context).Wait();

            var timeouts = factory.RetrieveAllTimeouts();

            var result = sut.TryRemove(timeouts.First().Id, context).Result;

            result.Should().BeTrue();

            var remainingTimeout = factory.RetrieveAllTimeouts().ToList();
            remainingTimeout.Should().HaveCount(1);
            remainingTimeout.First().Id.Should().Be(timeoutData2.Id);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdRemovesTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData, context).Wait();

            sut.RemoveTimeoutBy(timeoutData.SagaId, context).Wait();
            
            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdOnEmptyTimeoutCollection(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.RemoveTimeoutBy(timeoutData.SagaId, context).Wait();

            factory.RetrieveAllTimeouts().Should().HaveCount(0);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdOnNonExistantIdDoesNotRemoveOtherTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData, context).Wait();

            sut.RemoveTimeoutBy(Guid.NewGuid(), context).Wait();

            factory.RetrieveAllTimeouts().Should().HaveCount(1);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void RemoveTimeoutByIdRemovesCorrectTimeoutAndDoesNotRemoveOtherTimeout(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            Timeout.Core.TimeoutData timeoutData1,
            Timeout.Core.TimeoutData timeoutData2,
            ContextBag context)
        {
            factory.ResetTimeoutCollection();

            sut.Add(timeoutData1, context).Wait();
            sut.Add(timeoutData2, context).Wait();

            sut.RemoveTimeoutBy(timeoutData2.SagaId, context).Wait();

            var remainingTimeouts = factory.RetrieveAllTimeouts().ToList();
            remainingTimeouts.Should().HaveCount(1);

            remainingTimeouts.First().SagaId.Should().Be(timeoutData1.SagaId);
        }
    }
}
