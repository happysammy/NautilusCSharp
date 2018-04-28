//--------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Execution
{
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The sealed <see cref="ExecutionService"/> class. The <see cref="BlackBox"/> service context
    /// which handles all execution related operations.
    /// </summary>
    public sealed class ExecutionService : ActorComponentBusConnectedBase
    {
        private readonly IActorRef orderBusRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public ExecutionService(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Execution,
            LabelFactory.Service(BlackBoxService.Execution),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.orderBusRef = Context.ActorOf(Props.Create(() => new OrderBus(setupContainer, messagingAdapter)));

            this.SetupCommandMessageHandling();
            this.SetupServiceMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<SubmitTrade>(msg => this.OnMessage(msg));
            this.Receive<ModifyStopLoss>(msg => this.OnMessage(msg));
            this.Receive<ClosePosition>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="ServiceMessage"/> handling methods.
        /// </summary>
        private void SetupServiceMessageHandling()
        {
            this.Receive<InitializeBrokerageGateway>(msg => this.OnMessage(msg));
            this.Receive<ShutdownSystem>(msg => this.OnMessage(msg));
        }

        private void OnMessage(SubmitTrade message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(ModifyStopLoss message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(ClosePosition message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(CancelOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(InitializeBrokerageGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(ShutdownSystem message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                // TODO
            });
        }
    }
}
