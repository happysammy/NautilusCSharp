namespace Nautilus.Fix
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;

    public class FixClientFactory : IBrokerageClientFactory
    {
        private readonly Broker broker;
        private readonly FixCredentials credentials;

        public FixClientFactory(
            Broker broker,
            FixCredentials credentials)
        {
            this.broker = broker;
            this.credentials = credentials;
        }
        public IBrokerageClient Create(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            return new FixClient(container, this.credentials, this.broker);
        }
    }
}
