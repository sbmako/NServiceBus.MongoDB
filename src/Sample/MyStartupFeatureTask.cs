// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyStartupFeatureTask.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2018 SharkByte Software
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
    using System.Threading.Tasks;

    using NServiceBus;
    using NServiceBus.Logging;

    /// <summary>
    /// The startup.
    /// </summary>
    public class MyStartupFeatureTask : IWantToRunWhenEndpointStartsAndStops 
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MyStartupFeatureTask));

        public Task Start(IMessageSession session)
        {
            Logger.Info("Statup.Start()");

            Logger.Info("Sending first message");
            var initMessage = new MyMessage
            {
                SomeId = Guid.NewGuid().ToString(),
                ////LargeBlob = new DataBusProperty<byte[]>(Guid.NewGuid().ToByteArray())
                LargeBlob = "Sandoval"
            };
            session.Send(initMessage).ConfigureAwait(false);

            Task.Delay(10000).Wait();

            Logger.Info("Sending other messages");
            var anotherMessage = new AnotherSagaCommand { SomeId = initMessage.SomeId, SleepHowLong = 1000 };
            for (var i = 0; i < 5; i++)
            {
                anotherMessage.SleepHowLong = i;
                session.Send(anotherMessage).ConfigureAwait(false);
            }

            anotherMessage.SleepHowLong = 0;
            session.Send(anotherMessage).ConfigureAwait(false);
            session.Send(anotherMessage).ConfigureAwait(false);
            session.Send(anotherMessage).ConfigureAwait(false);

            return Task.FromResult(0);
        }

        public Task Stop(IMessageSession session)
        {
            return Task.FromResult(0);
        }
    }
}
