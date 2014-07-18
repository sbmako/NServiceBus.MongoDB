// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutPersisterTests.cs" company="SharkByte Software Inc.">
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
//   Defines the MongoTimeoutPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TimeoutPersister
{
    using System;

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
            factory.ResetTimeoutCollection();
            timeoutData.Time = DateTime.UtcNow.AddMinutes(-1);
            sut.Add(timeoutData);

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().HaveCount(1);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsOneTimeoutWhenCollectionHasOneTimeoutBetweenStartSliceAndUtcNowAndOneAfterUtcNow(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
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
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetNextChunkReturnsEmptyListWhenCollectionHasTwoTimeoutsAfterUtcNow(
            MongoTimeoutPersister sut,
            MongoDatabaseFactory factory,
            TimeoutData timeoutData)
        {
            factory.ResetTimeoutCollection();
            timeoutData.Time = DateTime.UtcNow.AddMinutes(1);
            sut.Add(timeoutData);
            sut.Add(timeoutData);

            var startSlice = DateTime.UtcNow.AddMinutes(-5);
            DateTime nextTimeToRunQuery;
            var result = sut.GetNextChunk(startSlice, out nextTimeToRunQuery);

            result.Should().HaveCount(0);
        }
    }
}
