
using System;
using NServiceBus.Saga;

namespace NServiceBus.MongoDB.SagaPersister
{
    public class MongoSagaPersister : ISagaPersister
    {
        public void Save(IContainSagaData saga)
        {
            throw new NotImplementedException();
        }

        public void Update(IContainSagaData saga)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(Guid sagaId) where T : IContainSagaData
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string property, object value) where T : IContainSagaData
        {
            throw new NotImplementedException();
        }

        public void Complete(IContainSagaData saga)
        {
            throw new NotImplementedException();
        }
    }
}
