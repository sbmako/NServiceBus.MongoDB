// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySaga.cs" company="SharkByte Software">
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
//   The my saga.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sample
{
    using System;
    using System.Threading.Tasks;

    using NServiceBus;
    using NServiceBus.Logging;

    /// <summary>
    /// The my saga.
    /// </summary>
    public class MySaga : Saga<MySagaData>,
        IAmStartedByMessages<MyMessage>,
        IHandleMessages<AnotherSagaCommand>,
        IHandleTimeouts<MyTimeout>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MySaga));

        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            Logger.Info("Hello from MySaga");

            this.Data.SomeId = message.SomeId;
            this.Data.Count = 0;

            return this.RequestTimeout(context, TimeSpan.FromMinutes(1), new MyTimeout() { HowLong = 5 });
        }

        public Task Handle(AnotherSagaCommand message, IMessageHandlerContext context)
        {
            this.Data.Count += 1;
            Logger.InfoFormat("Hello from AnotherSagaCommand: {0}", this.Data.Count);

            return Task.FromResult(0);
        }

        public Task Timeout(MyTimeout state, IMessageHandlerContext context)
        {
            Logger.InfoFormat("Timeout reached with a count of: {0}", this.Data.Count);
            this.MarkAsComplete();

            return Task.FromResult(0);
        }

        /// <summary>
        /// A generic version of <see cref="M:NServiceBus.Saga.Saga`1.ConfigureHowToFindSaga(NServiceBus.Saga.IConfigureHowToFindSagaWithMessage)"/> wraps <see cref="T:NServiceBus.Saga.IConfigureHowToFindSagaWithMessage"/> in a generic helper class (<see cref="T:NServiceBus.Saga.SagaPropertyMapper`1"/>) to provide mappings specific to <typeparamref name="TSagaData"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="T:NServiceBus.Saga.SagaPropertyMapper`1"/> that wraps the <see cref="T:NServiceBus.Saga.IConfigureHowToFindSagaWithMessage"/></param>
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            Logger.Info("Configuring now to find saga");
            mapper.ConfigureMapping<MyMessage>(message => message.SomeId).ToSaga(saga => saga.SomeId);
            mapper.ConfigureMapping<AnotherSagaCommand>(message => message.SomeId).ToSaga(saga => saga.SomeId);
        }
    }
}