// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="SharkByte Software Inc.">
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
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sample
{
    using System;
    using System.Threading;

    using NServiceBus;
    using NServiceBus.Config;
    using NServiceBus.Logging;

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup : IWantToRunWhenConfigurationIsComplete
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Startup));

        private readonly IBus bus;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        public Startup(IBus bus)
        {
            this.bus = bus;
        }

        /// <summary>
        /// The run method
        /// </summary>
        public void Run()
        {
            Logger.Info("Statup.Run()");

            var initMessage = new MyMessage { SomeId = "carlos" };
            var anotherMessage = new AnotherSagaCommand { SomeId = initMessage.SomeId, SleepHowLong = 2000 };

            ////Thread.Sleep(5000);

            this.bus.Send(initMessage);

            ////Thread.Sleep(1000);

            ////for (int i = 0; i < 2000; i++)
            ////{
            ////    anotherMessage.SleepHowLong = i;
            ////    this.bus.SendLocal(anotherMessage);
            ////}

            ////anotherMessage.SleepHowLong = 0;
            ////this.bus.SendLocal(anotherMessage);
            ////this.bus.SendLocal(anotherMessage);
            ////this.bus.SendLocal(anotherMessage);
        }
    }
}
