// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDatabaseFactoryExtensions.cs" company="SharkByte Software Inc.">
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
//   Defines the MongoDatabaseFactoryExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.TestingUtilities
{
    using global::MongoDB.Driver.Builders;
    using NServiceBus.MongoDB.SagaPersister;
    using NServiceBus.Saga;

    internal static class MongoDatabaseFactoryExtensions
    {
        public static T RetrieveSagaData<T>(this MongoDatabaseFactory factory, T sagaData)
            where T : IContainSagaData
        {
            var query = Query<T>.EQ(e => e.Id, sagaData.Id);
            var entity = factory.GetDatabase()
                                .GetCollection<T>(typeof(T).Name)
                                .FindOne(query);
            return entity;
        }

        public static SagaUniqueIdentity RetrieveSagaUniqueIdentity<T>(this MongoDatabaseFactory factory, T sagaData)
            where T : IContainSagaData
        {
            var property = UniqueAttribute.GetUniqueProperty(sagaData);

            if (!property.HasValue)
            {
                return null;
            }

            var id = SagaUniqueIdentity.FormatId(typeof(T), property.Value);
            var query = Query<SagaUniqueIdentity>.EQ(e => e.Id, id);
            var entity =
                factory.GetDatabase().GetCollection<SagaUniqueIdentity>(typeof(SagaUniqueIdentity).Name).FindOne(query);

            return entity;
        }
    }
}
