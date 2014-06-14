// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySaga.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
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
//   Defines the MySaga type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NServiceBus.Logging;
using NServiceBus.Saga;
    
public class MySaga:Saga<MySagaData>, IAmStartedByMessages<MyMessage>
{
    static ILog logger = LogManager.GetLogger(typeof(MySaga));

    public void Handle(MyMessage message)
    {
        logger.Info("Hello from MySaga"); 
    }

    ////protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    ////{
    ////    mapper.ConfigureMapping<MyMessage>(m => m.SomeId)
    ////        .ToSaga(s => s.SomeId);
    ////}
}