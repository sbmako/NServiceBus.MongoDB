﻿namespace NServiceBus.AcceptanceTests.Routing
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using NServiceBus.Pipeline;
    using ObjectBuilder;
    using Transport;

    class SubscriptionBehavior<TContext> : IBehavior<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext> where TContext : ScenarioContext
    {
        public SubscriptionBehavior(Action<SubscriptionEventArgs, TContext> action, TContext scenarioContext, MessageIntentEnum intentToHandle)
        {
            this.action = action;
            this.scenarioContext = scenarioContext;
            this.intentToHandle = intentToHandle;
        }

        public async Task Invoke(IIncomingPhysicalMessageContext context, Func<IIncomingPhysicalMessageContext, Task> next)
        {
            await next(context).ConfigureAwait(false);
            var subscriptionMessageType = GetSubscriptionMessageTypeFrom(context.Message);
            if (subscriptionMessageType != null)
            {
                string returnAddress;
                if (!context.Message.Headers.TryGetValue(Headers.SubscriberTransportAddress, out returnAddress))
                {
                    context.Message.Headers.TryGetValue(Headers.ReplyToAddress, out returnAddress);
                }

                var intent = (MessageIntentEnum)Enum.Parse(typeof(MessageIntentEnum), context.Message.Headers[Headers.MessageIntent], true);
                if (intent != intentToHandle)
                {
                    return;
                }

                action(new SubscriptionEventArgs
                {
                    MessageType = subscriptionMessageType,
                    SubscriberReturnAddress = returnAddress
                }, scenarioContext);
            }
        }

        static string GetSubscriptionMessageTypeFrom(IncomingMessage msg)
        {
            string headerValue;
            return msg.Headers.TryGetValue(Headers.SubscriptionMessageType, out headerValue) ? headerValue : null;
        }

        Action<SubscriptionEventArgs, TContext> action;
        TContext scenarioContext;
        MessageIntentEnum intentToHandle;

        internal class Registration : RegisterStep
        {
            public Registration(string id, Func<IBuilder, IBehavior> behaviorFactory)
                : base(id, typeof(SubscriptionBehavior<TContext>), "notify subscription events", behaviorFactory)
            {
                InsertBeforeIfExists("ProcessSubscriptionRequests");
            }
        }
    }
}