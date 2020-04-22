namespace NServiceBus.MongoDB.Tests.Issues.SagaPersister
{
    using System;
    using FluentAssertions;
    using NServiceBus.Extensibility;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.MongoDB.Tests.Sample;
    using NServiceBus.MongoDB.Tests.TestingUtilities;
    using NServiceBus.Persistence;
    using NServiceBus.Sagas;
    using Xunit;

    public class SagaIssueWIthLocalMongoOnMacTests
    {
        [Theory]
        [AutoDatabase]
        public void CompleteSagaThenStartShouldSaveSecondSaga(
            MongoSagaPersister sut,
            MongoDatabaseFactory factory,
            SagaWithUniqueProperty sagaData,
            SagaCorrelationProperty correlationProperty,
            SynchronizedStorageSession session,
            ContextBag context)
        {
            sut.Save(sagaData, correlationProperty, session, context).Wait();
            var entity = factory.RetrieveSagaData(sagaData);
            entity.Id.Should().Be(sagaData.Id);
            entity.SomeValue.Should().Be(sagaData.SomeValue);

            sut.Complete(sagaData, session, context).ConfigureAwait(false);
            factory.RetrieveSagaData(sagaData).Should().BeNull();

            sagaData.SomeValue = Guid.NewGuid().ToString();
            sut.Save(sagaData, correlationProperty, session, context).Wait();
            entity = factory.RetrieveSagaData(sagaData);
            entity.Id.Should().Be(sagaData.Id);
            entity.SomeValue.Should().Be(sagaData.SomeValue);
        }
    }
}
