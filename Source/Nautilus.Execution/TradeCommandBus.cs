//--------------------------------------------------------------------------------------------------
// <copyright file="TradeCommandBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a trade command bus for the execution service.
    /// </summary>
    [Stateless]
    public sealed class TradeCommandBus : ActorComponentBusConnectedBase
    {
        private readonly IFixGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeCommandBus"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The execution gateway.</param>
        public TradeCommandBus(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway)
            : base(
            NautilusService.Execution,
            LabelFactory.Component(nameof(TradeCommandBus)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(gateway, nameof(gateway));

            this.gateway = gateway;

            // Setup message handling.
            this.Receive<CollateralInquiry>(this.OnMessage);
            this.Receive<SubmitOrder>(this.OnMessage);
            this.Receive<SubmitTrade>(this.OnMessage);
            this.Receive<CancelOrder>(this.OnMessage);
            this.Receive<ModifyOrder>(this.OnMessage);
            this.Receive<ClosePosition>(this.OnMessage);
        }

        private void OnMessage(CollateralInquiry message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            this.gateway.CollateralInquiry();

            this.Log.Debug($"Routing {message} => {this.gateway.Broker}");
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            this.gateway.SubmitOrder(message.Order);
        }

        private void OnMessage(SubmitTrade message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            foreach (var atomicOrder in message.OrderPacket.Orders)
            {
                this.RouteOrder(atomicOrder);

                this.Log.Debug($"Routing ELS Order {atomicOrder.Symbol} => {this.gateway.Broker}");
            }
        }

        private void OnMessage(CancelOrder message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            this.gateway.CancelOrder(message.Order);
        }

        private void OnMessage(ModifyOrder message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            this.gateway.ModifyOrder(message.Order, message.ModifiedPrice);

            this.Log.Debug($"Routing StopLossReplaceRequest {message.Order.Symbol} => {this.gateway.Broker}");
        }

        private void OnMessage(ClosePosition message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            this.gateway.ClosePosition(message.Position);

            this.Log.Debug($"Routing ClosePosition {message.Position} => {this.gateway.Broker}");
        }

        private void RouteOrder(AtomicOrder atomicOrder)
        {
            Debug.NotNull(atomicOrder, nameof(atomicOrder));
            Validate.True(this.IsConnected(), nameof(this.gateway.IsConnected));

            this.gateway.SubmitOrder(atomicOrder);
        }

        private bool IsConnected()
        {
            if (!this.gateway.IsConnected)
            {
                this.Log.Warning("Cannot process orders (not connected to broker).");

                return false;
            }

            return true;
        }
    }
}
