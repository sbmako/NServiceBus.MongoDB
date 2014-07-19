// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Carlos Sandoval
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
//   The startup.
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
