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
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Provides a trade command bus for the execution service.
    /// </summary>
    public sealed class OrderCommandBus : ComponentBusConnectedBase
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
            container,
            messagingAdapter)
        {
            this.gateway = gateway;

            this.RegisterHandler<CollateralInquiry>(this.OnMessage);
            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<AtomicOrder>(this.RouteOrder);
        }

        private void OnMessage(CollateralInquiry message)
        {
            this.gateway.CollateralInquiry();

            this.Log.Debug($"Routing {message} => {this.gateway.Broker}");
        }

        private void OnMessage(SubmitOrder message)
        {
            this.gateway.SubmitOrder(message.Order);
        }

        private void OnMessage(CancelOrder message)
        {
            this.gateway.CancelOrder(message.Order);
        }

        private void OnMessage(ModifyOrder message)
        {
            this.gateway.ModifyOrder(message.Order, message.ModifiedPrice);

            this.Log.Debug($"Routing StopLossReplaceRequest {message.Order.Symbol} => {this.gateway.Broker}");
        }

        private void RouteOrder(AtomicOrder atomicOrder)
        {
            this.gateway.SubmitOrder(atomicOrder);
        }
    }
}
