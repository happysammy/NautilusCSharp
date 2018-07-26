namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Akka.Actor;
    using Nautilus.Common.Enums;
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

        public void Send<T>(NautilusService receiver, T message, NautilusService sender)
            where T : Message
        {
            this.testActorRef.Tell(message);
        }

        public void Send<T>(ReadOnlyList<NautilusService> receivers, T message, NautilusService sender)
            where T : Message
        {
            this.testActorRef.Tell(message);
        }
    }
}
