// -------------------------------------------------------------------------------------------------
// <copyright file="OrderExpiryController.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Extensions;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Base;

    /// <summary>
    /// The sealed <see cref="OrderExpiryController"/> class. Contains and processes all
    /// <see cref="OrderExpiryCounter"/>(s) for a <see cref="SecurityPortfolio"/>.
    /// </summary>
    public sealed class OrderExpiryController : BusConnectedComponentBase
    {
        private readonly IList<OrderExpiryCounter> orderExpiryCounters = new List<OrderExpiryCounter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpiryController"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public OrderExpiryController(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            Symbol symbol)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(OrderExpiryController), symbol),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(symbol, nameof(symbol));
        }

        /// <summary>
        /// Gets the timer count.
        /// </summary>
        public int TotalCounters => this.orderExpiryCounters.Count;

        /// <summary>
        /// Processes all timers held by the controller.
        /// </summary>
        /// <param name="orderIdList">The order identifier list.</param>
        /// <exception cref="ValidationException">Throws if the order identifier list is null.</exception>
        public void ProcessCounters(IReadOnlyList<EntityId> orderIdList)
        {
            Validate.NotNull(orderIdList, nameof(orderIdList));

            if (this.TotalCounters > 0)
            {
                this.ForceRemoveExpiredCounters(orderIdList);

                foreach (var counter in this.orderExpiryCounters)
                {
                    counter.IncrementCount();

                    if (counter.IsOrderExpired())
                    {
                        var cancelOrder = new CancelOrder(
                            counter.Order,
                            $"EntryOrder expired at {this.TimeNow()}",
                            this.NewGuid(),
                            this.TimeNow());

                        this.MessagingAdapter.Send<CommandMessage>(
                            BlackBoxService.Execution,
                            cancelOrder,
                            this.Service);
                    }
                }
            }

            this.Log(LogLevel.Debug, $"Processed OrderExpiryCounters (TotalCounters={this.TotalCounters})");
        }

        /// <summary>
        /// Adds <see cref="OrderExpiryCounter"/>(s) based on the given inputs.
        /// </summary>
        /// <param name="orderPacket">The order packet.</param>
        /// <param name="barsValid">The bars valid.</param>
        /// <exception cref="ValidationException">Throws if the order packet is null, or if the
        /// bars valid is zero or negative.</exception>
        public void AddCounters(AtomicOrderPacket orderPacket, int barsValid)
        {
            Validate.NotNull(orderPacket, nameof(orderPacket));
            Validate.Int32NotOutOfRange(barsValid, nameof(barsValid), 0, int.MaxValue, RangeEndPoints.Exclusive);

            foreach (var atomicOrder in orderPacket.Orders)
            {
                if (barsValid > 0)
                {
                    this.orderExpiryCounters.Add(new OrderExpiryCounter(atomicOrder.EntryOrder, barsValid));

                    this.Log(
                        LogLevel.Debug,
                        $"Added OrderExpiryCounter ({atomicOrder.EntryOrder.OrderId}) "
                      + $"BarsValid={barsValid}");
                }
            }
        }

        /// <summary>
        /// Removes the <see cref="OrderExpiryCounter"/> corresponding to the given order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <exception cref="ValidationException">Throws if the order identifier is null.</exception>
        public void RemoveCounter(EntityId orderId)
        {
            Validate.NotNull(orderId, nameof(orderId));

            foreach (var counter in this.orderExpiryCounters.ToList())
            {
                if (counter.Order.OrderId.Equals(orderId))
                {
                    this.orderExpiryCounters.Remove(counter);

                    this.Log(
                        LogLevel.Debug,
                        $"Removed OrderExpiryCounter ({orderId}) "
                      + $"TotalCounters={this.TotalCounters}");
                }
            }
        }

        /// <summary>
        /// A backup method which removes any <see cref="OrderExpiryCounter"/> which no longer has a
        /// pending order associated with it.
        /// </summary>
        /// <param name="orderIdList">The list of current active order identifiers.</param>
        private void ForceRemoveExpiredCounters(IReadOnlyList<EntityId> orderIdList)
        {
            Debug.NotNull(orderIdList, nameof(orderIdList));

            var countersToRemove = this.GetCounterIdList().Where(orderId => !orderIdList.Contains(orderId)).ToList();

            foreach (var orderId in countersToRemove)
            {
                foreach (var counter in this.orderExpiryCounters.ToList())
                {
                    if (counter.Order.OrderId.Equals(orderId))
                    {
                        this.orderExpiryCounters.Remove(counter);

                        this.Log(
                            LogLevel.Warning,
                            $"ForceRemoveExpiredCounter() "
                          + $"({counter} at {this.TimeNow().ToStringFormattedIsoUtc()}) "
                          + $"TotalCounters={this.TotalCounters}");
                    }
                }
            }

            Debug.NotNull(this.orderExpiryCounters, nameof(this.orderExpiryCounters));
        }

        private IReadOnlyCollection<EntityId> GetCounterIdList()
        {
            return this.orderExpiryCounters
               .Select(counter => counter.Order.OrderId)
               .ToImmutableList();
        }
    }
}
