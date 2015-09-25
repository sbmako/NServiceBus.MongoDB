// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSagaPersisterTests.cs" company="SharkByte Software">
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
//   Defines the MongoSagaPersisterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.SagaPersister
{
    using System;

    using CategoryTraits.Xunit2;

    using FluentAssertions;
    using global::MongoDB.Driver;

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.Tests.Sample;
    using NServiceBus.MongoDB.Tests.TestingUtilities;

    using Xunit;

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
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SavingSagaWithNullUniquePropertyShouldThrowException(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData)
        {
            sagaData.UniqueProperty = null;
            sut.Invoking(s => s.Save(sagaData)).ShouldThrow<ArgumentNullException>();
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
        public void SavingDifferentSagaWithSameUniquePropertyShouldThrowDuplicateException(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData1,
            SagaWithUniqueProperty sagaData2)
        {
            var uniqueProperty = Guid.NewGuid().ToString();
            sagaData1.UniqueProperty = uniqueProperty;
            sagaData2.UniqueProperty = uniqueProperty;

            sut.Save(sagaData1);
            sut.Invoking(s => s.Save(sagaData2)).ShouldThrow<MongoDuplicateKeyException>();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SavingSagaWithSameUniquePropertyAsAnAlreadyCompletedSaga(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData1,
            SagaWithUniqueProperty sagaData2)
        {
            var uniqueProperty = Guid.NewGuid().ToString();
            sagaData1.UniqueProperty = uniqueProperty;
            sagaData2.UniqueProperty = uniqueProperty;

            sut.Save(sagaData1);
            sut.Complete(sagaData1);

            sut.Save(sagaData2);

            var entity = factory.RetrieveSagaData(sagaData2);
            entity.UniqueProperty.Should().Be(sagaData2.UniqueProperty);
            entity.NonUniqueProperty.Should().Be(sagaData2.NonUniqueProperty);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void SavingSagaWithoutDocumentVersionShouldThrowException(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithoutDocumentVersion sagaData)
        {
            sut.Invoking(s => s.Save(sagaData)).ShouldThrow<InvalidOperationException>();
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

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void UpdatingSagaWithoutDocumentVersion(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithoutUniqueProperties sagaData)
        {
            sut.Invoking(s => s.Update(sagaData)).ShouldThrow<InvalidOperationException>();
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void CompletingSagaShouldRemoveDocument(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithoutUniqueProperties sagaData)
        {
            sut.Save(sagaData);

            sut.Complete(sagaData);
            factory.RetrieveSagaData(sagaData).Should().BeNull();
        }

        [Theory]
        [IntegrationTest]
        [AutoDatabase]
        public void RetrievingSagaUsingId(
            MongoSagaPersister sut,
            SagaWithoutUniqueProperties sagaData)
        {
            sut.Save(sagaData);

            var result = sut.Get<SagaWithoutUniqueProperties>(sagaData.Id);

            result.ShouldBeEquivalentTo(sagaData);
        }

        [Theory]
        [IntegrationTest]
        [AutoDatabase]
        public void RetrievingSagaUsingGuidUniqueId(
            MongoSagaPersister sut,
            SagaWithUniqueGuidProperty sagaData)
        {
            sut.Save(sagaData);

            var result = sut.Get<SagaWithUniqueGuidProperty>("SomeId", sagaData.SomeId);

            result.ShouldBeEquivalentTo(sagaData);
        }

        [Theory]
        [IntegrationTest]
        [AutoDatabase]
        public void RetrievingSagaUsingStringUniqueId(
            MongoSagaPersister sut,
            SagaWithUniqueProperty sagaData)
        {
            sut.Save(sagaData);

            var result = sut.Get<SagaWithUniqueProperty>("UniqueProperty", sagaData.UniqueProperty);

            result.ShouldBeEquivalentTo(sagaData);
        }
    }
}
