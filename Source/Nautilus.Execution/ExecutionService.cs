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
        private readonly IEndpoint orderBusRef;

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

            this.orderBusRef = new ActorEndpoint(
                Context.ActorOf(
                Props.Create(() => new OrderBus(container, messagingAdapter))));

            // Setup message handling.
            this.Receive<InitializeGateway>(msg => this.OnMessage(msg));
            this.Receive<CollateralInquiry>(msg => this.OnMessage(msg));
            this.Receive<SubmitOrder>(msg => this.OnMessage(msg));
            this.Receive<SubmitTrade>(msg => this.OnMessage(msg));
            this.Receive<ModifyOrder>(msg => this.OnMessage(msg));
            this.Receive<CloseTradeUnit>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Actions to be performed after the actor base is stopped.
        /// </summary>
        protected override void PostStop()
        {
            this.Execute(() =>
            {
                this.orderBusRef.Send(PoisonPill.Instance);
                base.PostStop();
            });
        }

        private void OnMessage(InitializeGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }

        private void OnMessage(CollateralInquiry message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }

        private void OnMessage(SubmitTrade message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }

        private void OnMessage(ModifyOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }

        private void OnMessage(CloseTradeUnit message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }

        private void OnMessage(CancelOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.orderBusRef.Send(message);
            });
        }
    }
}
