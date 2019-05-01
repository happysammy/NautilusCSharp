//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCommandBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a trade command bus for the execution service.
    /// </summary>
    public sealed class OrderCommandBus : ActorComponentBusConnectedBase
    {
        private readonly IFixGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCommandBus"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The execution gateway.</param>
        public OrderCommandBus(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway)
            : base(
            NautilusService.Execution,
            LabelFactory.Create(nameof(OrderCommandBus)),
            container,
            messagingAdapter)
        {
            Precondition.NotNull(container, nameof(container));
            Precondition.NotNull(messagingAdapter, nameof(messagingAdapter));
            Precondition.NotNull(gateway, nameof(gateway));

            this.gateway = gateway;

            // Command messages.
            this.Receive<CollateralInquiry>(this.OnMessage);
            this.Receive<SubmitOrder>(this.OnMessage);
            this.Receive<CancelOrder>(this.OnMessage);
            this.Receive<ModifyOrder>(this.OnMessage);
        }

        private void OnMessage(CollateralInquiry message)
        {
            Debug.NotNull(message, nameof(message));

            this.gateway.CollateralInquiry();

            this.Log.Debug($"Routing {message} => {this.gateway.Broker}");
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.gateway.SubmitOrder(message.Order);
        }

        private void OnMessage(CancelOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.gateway.CancelOrder(message.Order);
        }

        private void OnMessage(ModifyOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.gateway.ModifyOrder(message.Order, message.ModifiedPrice);

            this.Log.Debug($"Routing StopLossReplaceRequest {message.Order.Symbol} => {this.gateway.Broker}");
        }

        private void RouteOrder(AtomicOrder atomicOrder)
        {
            Debug.NotNull(atomicOrder, nameof(atomicOrder));

            this.gateway.SubmitOrder(atomicOrder);
        }
    }
}
