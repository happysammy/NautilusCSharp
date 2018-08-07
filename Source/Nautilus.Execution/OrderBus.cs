//--------------------------------------------------------------------------------------------------
// <copyright file="OrderBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides an order bus for the execution service.
    /// </summary>
    [Stateless]
    public sealed class OrderBus : ActorComponentBusConnectedBase
    {
        private IExecutionGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBus"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        public OrderBus(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
            NautilusService.Execution,
            new Label(nameof(OrderBus)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            // Command message handling.
            this.Receive<InitializeGateway>(msg => this.OnMessage(msg));
            this.Receive<SubmitOrder>(msg => this.OnMessage(msg));
            this.Receive<SubmitTrade>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
            this.Receive<ModifyOrder>(msg => this.OnMessage(msg));
            this.Receive<ClosePosition>(msg => this.OnMessage(msg));
        }

        private void OnMessage(InitializeGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.gateway = message.ExecutionGateway;
            this.Log.Information($"{this.gateway} initialized.");
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.True(this.IsConnectedToBroker(), nameof(this.gateway));

            this.gateway.SubmitOrder(message.Order);
        }

        private void OnMessage(SubmitTrade message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.True(this.IsConnectedToBroker(), nameof(this.gateway));

            foreach (var atomicOrder in message.OrderPacket.Orders)
            {
                this.RouteOrder(atomicOrder);

                this.Log.Debug($"Routing ELS Order {atomicOrder.Symbol} => {this.gateway.Broker}");
            }
        }

        private void OnMessage(CancelOrder message)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.gateway));

            this.gateway.CancelOrder(message.Order);
        }

        private void OnMessage(ModifyOrder message)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.gateway));

            this.gateway.ModifyOrder(message.Order, message.ModifiedPrice);

            this.Log.Debug($"Routing StopLossReplaceRequest {message.Order.Symbol} => {this.gateway.Broker}");
        }

        private void OnMessage(ClosePosition message)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.gateway));

            this.gateway.ClosePosition(message.Position);

            this.Log.Debug($"Routing ClosePosition ({message.Position} => {this.gateway.Broker}");
        }

        private void RouteOrder(AtomicOrder atomicOrder)
        {
            Validate.True(this.IsConnectedToBroker(), nameof(this.gateway));
            Debug.NotNull(atomicOrder, nameof(atomicOrder));

            this.gateway.SubmitOrder(atomicOrder);
        }

        private bool IsConnectedToBroker()
        {
            Debug.NotNull(this.gateway, nameof(this.gateway));

            if (!this.gateway.IsConnected)
            {
                this.Log.Warning("Cannot process orders (not connected to broker).");

                return false;
            }

            return true;
        }
    }
}
