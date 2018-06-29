namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;

    public class MockMessagingAdapter : IMessagingAdapter
    {
        private readonly IActorRef testActorRef;

        public MockMessagingAdapter(IActorRef testActorRef)
        {
            this.testActorRef = testActorRef;
        }

        public void Send<T>(Enum receiver, T message, Enum sender) where T : Message
        {
            this.testActorRef.Tell(message);
        }

        public void Send<T>(ReadOnlyList<Enum> receivers, T message, Enum sender) where T : Message
        {
            this.testActorRef.Tell(message);
        }
    }
}
