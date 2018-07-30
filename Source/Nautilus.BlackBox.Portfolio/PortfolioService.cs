//--------------------------------------------------------------------------------------------------
// <copyright file="PortfolioService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Messages.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The sealed <see cref="PortfolioService"/> class.
    /// </summary>
    public sealed class PortfolioService : ActorComponentBusConnectedBase
    {
        private readonly BlackBoxContainer storedContainer;
        private readonly SecurityPortfolioStore portfolioStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="portfolioStore">The security portfolio store.</param>
        public PortfolioService(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            SecurityPortfolioStore portfolioStore)
            : base(
            NautilusService.Portfolio,
            LabelFactory.Component(nameof(PortfolioService)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(portfolioStore, nameof(portfolioStore));

            this.storedContainer = container;
            this.portfolioStore = portfolioStore;

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<CreatePortfolio>(msg => this.OnMessage(msg));
            this.Receive<TradeApproved>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<EventMessage>(msg => this.Self.Tell(msg.Event));
            this.Receive<BarDataEvent>(msg => this.OnMessage(msg));
            this.Receive<SignalEvent>(msg => this.OnMessage(msg));
            this.Receive<OrderEvent>(msg => this.OnMessage(msg));
        }

        private void OnMessage(CreatePortfolio message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var portfolioRef = SecurityPortfolioFactory.Create(
                    this.storedContainer,
                    this.GetMessagingAdapter(),
                    message.Instrument,
                    Context);

                this.portfolioStore.AddPortfolio(message.Symbol, portfolioRef);
            });
        }

        private void OnMessage(TradeApproved message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }

        private void OnMessage(BarDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.portfolioStore.Tell(message.BarType.Symbol, message);
            });
        }

        private void OnMessage(SignalEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }

        private void OnMessage(OrderEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.portfolioStore.Tell(message.Symbol, message);
            });
        }
    }
}
