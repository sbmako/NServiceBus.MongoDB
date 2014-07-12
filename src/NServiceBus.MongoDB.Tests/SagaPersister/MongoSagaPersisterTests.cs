// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSagaPersisterTests.cs" company="SharkByte Software Inc.">
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
//   Defines the MongoSagaPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SagaPersister
{
    using System;
    using FluentAssertions;
    using global::MongoDB.Driver;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.Tests.Sample;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using Xunit.Extensions;

    public class MongoSagaPersisterTests
    {
        [Theory, IntegrationTest]
        [AutoDatabase]
        public void BasicMongoSagaPersisterConstruction(MongoDatabaseFactory factory)
        {
            var sut = new MongoSagaPersister(factory);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SavingSagaWithoutUniqueProperty(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithoutUniqueProperties sagaData)
        {
            sut.Save(sagaData);

            var entity = factory.RetrieveSagaData(sagaData);

            entity.Id.Should().Be(sagaData.Id);
            entity.NonUniqueProperty.Should().Be(sagaData.NonUniqueProperty);

            factory.RetrieveSagaUniqueIdentity(entity).Should().BeNull();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SavingSagaWithUniqueProperty(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData)
        {
            sut.Save(sagaData);

            var entity = factory.RetrieveSagaData(sagaData);

            entity.Id.Should().Be(sagaData.Id);
            entity.UniqueProperty.Should().Be(sagaData.UniqueProperty);
            entity.NonUniqueProperty.Should().Be(sagaData.NonUniqueProperty);

            var uniqueIdentity = factory.RetrieveSagaUniqueIdentity(entity);
            uniqueIdentity.Should().NotBeNull();
            uniqueIdentity.SagaId.Should().Be(sagaData.Id);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void InterleavedSavingSagaShouldThrowException(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData)
        {
            sut.Save(sagaData);
            sut.Invoking(s => s.Save(sagaData)).ShouldThrow<MongoDuplicateKeyException>();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UpdatingSagaWithoutUniqueProperty(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithoutUniqueProperties sagaData,
            string newValue)
        {
            sut.Save(sagaData);

            sagaData.NonUniqueProperty = newValue;
            sut.Update(sagaData);

            var entity = factory.RetrieveSagaData(sagaData);
            entity.NonUniqueProperty.Should().Be(newValue);

            factory.RetrieveSagaUniqueIdentity(entity).Should().BeNull();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UpdatingSagaWithUniqueProperty(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData,
            string newValue)
        {
            sut.Save(sagaData);

            sagaData.NonUniqueProperty = newValue;
            sut.Update(sagaData);

            var entity = factory.RetrieveSagaData(sagaData);
            entity.NonUniqueProperty.Should().Be(newValue);

            var uniqueIdentity = factory.RetrieveSagaUniqueIdentity(entity);
            uniqueIdentity.Should().NotBeNull();
            uniqueIdentity.SagaId.Should().Be(sagaData.Id);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UpdatingNonExistantSagaWithoutUniqueProperty(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithoutUniqueProperties sagaData)
        {
            sut.Invoking(s => s.Update(sagaData)).ShouldThrow<InvalidOperationException>();

            factory.RetrieveSagaData(sagaData).Should().BeNull();
            factory.RetrieveSagaUniqueIdentity(sagaData).Should().BeNull();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UpdatingNonExistantSagaWithUniqueProperty(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData)
        {
            sut.Invoking(s => s.Update(sagaData)).ShouldThrow<InvalidOperationException>();

            factory.RetrieveSagaData(sagaData).Should().BeNull();
            factory.RetrieveSagaUniqueIdentity(sagaData).Should().BeNull();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UpdateCollisionShouldFail(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData)
        {
            sut.Save(sagaData);
            var saga1 = factory.RetrieveSagaData(sagaData);
            var saga2 = factory.RetrieveSagaData(sagaData);

            saga1.UniqueProperty = Guid.NewGuid().ToString();
            sut.Update(saga1);

            saga2.UniqueProperty = Guid.NewGuid().ToString();
            sut.Invoking(s => s.Update(saga2)).ShouldThrow<InvalidOperationException>();
        }
    }
}
