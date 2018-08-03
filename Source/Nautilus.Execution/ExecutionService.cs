//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Akka.Actor;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The service context which handles all execution related operations.
    /// </summary>
    public sealed class ExecutionService : ActorComponentBusConnectedBase
    {
        private readonly IActorRef orderBusRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        public ExecutionService(
            IComponentryContainer container,
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
            this.Receive<SystemShutdown>(msg => this.OnMessage(msg));

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

        private void OnMessage(SystemShutdown message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                // TODO
            });
        }
    }
}
