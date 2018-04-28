//--------------------------------------------------------------
// <copyright file="PortfolioService.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The sealed <see cref="PortfolioService"/> class.
    /// </summary>
    public sealed class PortfolioService : ActorComponentBusConnectedBase
    {
        private readonly BlackBoxSetupContainer storedSetupContainer;
        private readonly SecurityPortfolioStore portfolioStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="portfolioStore">The security portfolio store.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public PortfolioService(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            SecurityPortfolioStore portfolioStore)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Service(BlackBoxService.Portfolio),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(portfolioStore, nameof(portfolioStore));

            this.storedSetupContainer = setupContainer;
            this.portfolioStore = portfolioStore;

            this.SetupEventMessageHandling();
            this.SetupServiceMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<EventMessage>(msg => this.Self.Tell(msg.Event));
            this.Receive<MarketDataEvent>(msg => this.OnMessage(msg));
            this.Receive<SignalEvent>(msg => this.OnMessage(msg));
            this.Receive<OrderEvent>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="ServiceMessage"/> handling methods.
        /// </summary>
        private void SetupServiceMessageHandling()
        {
            this.Receive<CreatePortfolio>(msg => this.OnMessage(msg));
            this.Receive<TradeApproved>(msg => this.OnMessage(msg));
        }

        private void OnMessage(CreatePortfolio message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                var portfolioRef = SecurityPortfolioFactory.Create(
                    this.storedSetupContainer,
                    this.MessagingAdapter,
                    message.Instrument,
                    Context);

                this.portfolioStore.AddPortfolio(message.Symbol, portfolioRef);
            });
        }

        private void OnMessage(TradeApproved message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }

        private void OnMessage(MarketDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }

        private void OnMessage(SignalEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }

        private void OnMessage(OrderEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }
    }
}
