//--------------------------------------------------------------------------------------------------
// <copyright file="RiskService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Risk
{
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Messages.Commands;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Interfaces;

    /// <summary>
    /// The sealed <see cref="RiskService "/> class.
    /// </summary>
    public sealed class RiskService : ActorComponentBusConnectedBase
    {
        private IBrokerageAccount account;
        private IRiskModel riskModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiskService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public RiskService(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Risk,
            LabelFactory.Service(BlackBoxService.Risk),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Set up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<InitializeRiskModel>(msg => this.OnMessage(msg));
            this.Receive<RequestTradeApproval>(msg => this.OnMessage(msg));
            this.Receive<AccountEvent>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Set up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<EventMessage>(msg => this.Self.Tell(msg.Event));
            this.Receive<BarDataEvent>(msg => this.OnMessage(msg));
        }

        private void OnMessage(InitializeRiskModel message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.account = message.Account;
                this.riskModel = message.RiskModel;

                this.Log.Information($"BrokerageAccount and RiskModel initialized");
            });
        }

        private void OnMessage(RequestTradeApproval message)
        {
            this.Execute(() =>
            {
                Debug.NotNull(message, nameof(message));

                if (this.account.FreeEquity.Value == decimal.Zero)
                {
                    this.Log.Warning($"{message} ignored... (Free Equity <= zero)");

                    return;
                }

                var tradeApproval = new TradeApproved(
                    message.OrderPacket,
                    message.Signal.TradeProfile.BarsValid,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, tradeApproval);
            });
        }

        private void OnMessage(AccountEvent @event)
        {
            this.Execute(() =>
            {
                Debug.NotNull(@event, nameof(@event));

                this.account.Apply(@event);

                this.Log.Information("BrokerageAccount updated");
            });
        }

        private void OnMessage(BarDataEvent message)
        {
            this.Execute(() =>
            {
                Debug.NotNull(message, nameof(message));
                // TODO
            });
        }
    }
}
