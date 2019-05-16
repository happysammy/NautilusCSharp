namespace Nautilus.Scheduler
{
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// INTERNAL API.
    /// </summary>
    internal sealed class ScheduledSend : IRunnable
    {
        private readonly IEndpoint receiver;
        private readonly object message;
        private readonly IEndpoint sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledSend"/> class.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="sender">The message sender.</param>
        internal ScheduledSend(IEndpoint receiver, object message, IEndpoint sender)
        {
            this.receiver = receiver;
            this.message = message;
            this.sender = sender;
        }

        public void Run()
        {
            this.receiver.Send(this.message);
        }

        public override string ToString()
        {
            return $"[{this.receiver}.Tell({this.message}, {this.sender})]";
        }
    }
}
