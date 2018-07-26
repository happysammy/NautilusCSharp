//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Execution
{
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Messages.Commands;
    using Nautilus.Common.Commands;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
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
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public ExecutionService(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
            NautilusService.Execution,
            LabelFactory.Component(nameof(ExecutionService)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.orderBusRef = Context.ActorOf(Props.Create(() => new OrderBus(container, messagingAdapter)));

            this.SetupCommandMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            // Setup system commands.
            this.Receive<InitializeGateway>(msg => this.OnMessage(msg));
            this.Receive<ShutdownSystem>(msg => this.OnMessage(msg));

            // Setup trade commands.
            this.Receive<SubmitOrder>(msg => this.OnMessage(msg));
            this.Receive<SubmitTrade>(msg => this.OnMessage(msg));
            this.Receive<ModifyOrder>(msg => this.OnMessage(msg));
            this.Receive<CloseTradeUnit>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(SubmitTrade message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(ModifyOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(CloseTradeUnit message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(CancelOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(InitializeGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Tell(message, this.Self);
            });
        }

        private void OnMessage(ShutdownSystem message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                // TODO
            });
        }
    }
}
