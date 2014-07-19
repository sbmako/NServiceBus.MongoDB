// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySaga.cs" company="Carlos Sandoval">
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
//   The my saga.
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

            this.RequestTimeout(TimeSpan.FromMinutes(10), new MyTimeout() { HowLong = 5 });
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