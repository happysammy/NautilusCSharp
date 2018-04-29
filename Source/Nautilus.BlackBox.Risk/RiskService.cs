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
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

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
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public RiskService(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Risk,
            LabelFactory.Service(BlackBoxService.Risk),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
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
            this.Receive<MarketDataEvent>(msg => this.OnMessage(msg));
        }

        private void OnMessage(InitializeRiskModel message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.account = message.Account;
                this.riskModel = message.RiskModel;

                this.Log(LogLevel.Information, $"BrokerageAccount and RiskModel initialized");
            });
        }

        private void OnMessage(RequestTradeApproval message)
        {
            Validate.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                if (this.account.FreeEquity.Value == decimal.Zero)
                {
                    this.Log(LogLevel.Warning, $"{message} ignored... (Free Equity <= zero)");

                    return;
                }

                var tradeApproval = new TradeApproved(
                    message.OrderPacket,
                    message.Signal.TradeProfile.BarsValid,
                    this.NewGuid(),
                    this.TimeNow());

                this.MessagingAdapter.Send<CommandMessage>(
                    BlackBoxService.Portfolio,
                    tradeApproval,
                    this.Service);
            });
        }

        private void OnMessage(AccountEvent @event)
        {
            Validate.NotNull(@event, nameof(@event));

            this.CommandHandler.Execute(() =>
            {
                this.account.Apply(@event);

                this.Log(LogLevel.Information, $"BrokerageAccount updated");
            });
        }

        private void OnMessage(MarketDataEvent message)
        {
            Validate.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                // TODO
            });
        }
    }
}
