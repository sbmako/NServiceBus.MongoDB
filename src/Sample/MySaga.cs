// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySaga.cs" company="SharkByte Software Inc.">
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
//   Defines the MySaga type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sample
{
    using System;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Saga;

    /// <summary>
    /// The my saga.
    /// </summary>
    public class MySaga : Saga<MySagaData>,
        IAmStartedByMessages<MyMessage>,
        IHandleMessages<AnotherSagaCommand>,
        IHandleTimeouts<MyTimeout>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MySaga));

        /// <summary>
        /// The configure how to find saga.
        /// </summary>
        public override void ConfigureHowToFindSaga()
        {
            Logger.Info("Configuring now to find saga");
            this.ConfigureMapping<MyMessage>(message => message.SomeId).ToSaga(saga => saga.SomeId);
            this.ConfigureMapping<AnotherSagaCommand>(message => message.SomeId).ToSaga(saga => saga.SomeId);
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(MyMessage message)
        {
            Logger.Info("Hello from MySaga");

            this.Data.SomeId = message.SomeId;
            this.Data.Count = 0;

            this.RequestTimeout(TimeSpan.FromMinutes(1), new MyTimeout() { HowLong = 5 });
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(AnotherSagaCommand message)
        {
            this.Data.Count += 1;
            Logger.InfoFormat("Hello from AnotherSagaCommand: {0}", this.Data.Count);
        }

        /// <summary>
        /// The timeout.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        public void Timeout(MyTimeout state)
        {
            Logger.InfoFormat("Timeout reached with a count of: {0}", this.Data.Count);
            this.MarkAsComplete();
        }
    }
}