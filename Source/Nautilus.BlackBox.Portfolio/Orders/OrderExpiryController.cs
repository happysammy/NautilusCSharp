//--------------------------------------------------------------------------------------------------
// <copyright file="OrderExpiryController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Commands;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Collections;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="OrderExpiryController"/> class. Contains and processes all
    /// <see cref="OrderExpiryCounter"/>(s) for a <see cref="SecurityPortfolio"/>.
    /// </summary>
    public sealed class OrderExpiryController : ComponentBusConnectedBase
    {
        // Concrete list for performance reasons.
        private readonly List<OrderExpiryCounter> orderExpiryCounters = new List<OrderExpiryCounter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpiryController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public OrderExpiryController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            Symbol symbol)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(OrderExpiryController), symbol),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
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
        public void ProcessCounters(ReadOnlyList<EntityId> orderIdList)
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

                        this.Send(BlackBoxService.Execution, cancelOrder);
                    }
                }
            }

            this.Log.Debug($"Processed OrderExpiryCounters (TotalCounters={this.TotalCounters})");
        }

        /// <summary>
        /// Adds <see cref="OrderExpiryCounter"/>(s) based on the given inputs.
        /// </summary>
        /// <param name="orderPacket">The order packet.</param>
        /// <param name="barsValid">The bars valid.</param>
        /// <exception cref="ValidationException">Throws if the order packet is null, or if the
        /// bars valid is zero or negative.</exception>
        public void AddCounters(AtomicOrdersPacket orderPacket, int barsValid)
        {
            Validate.NotNull(orderPacket, nameof(orderPacket));
            Validate.Int32NotOutOfRange(barsValid, nameof(barsValid), 0, int.MaxValue, RangeEndPoints.Exclusive);

            foreach (var atomicOrder in orderPacket.Orders)
            {
                if (barsValid > 0)
                {
                    this.orderExpiryCounters.Add(new OrderExpiryCounter(atomicOrder.Entry, barsValid));

                    this.Log.Debug(
                        $"Added OrderExpiryCounter ({atomicOrder.Entry.Id}) "
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
                if (counter.Order.Id.Equals(orderId))
                {
                    this.orderExpiryCounters.Remove(counter);

                    this.Log.Debug(
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
        private void ForceRemoveExpiredCounters(ReadOnlyList<EntityId> orderIdList)
        {
            Debug.NotNull(orderIdList, nameof(orderIdList));

            var countersToRemove = this.GetCounterIdList().Where(orderId => !orderIdList.Contains(orderId)).ToList();

            foreach (var orderId in countersToRemove)
            {
                foreach (var counter in this.orderExpiryCounters.ToList())
                {
                    if (counter.Order.Id.Equals(orderId))
                    {
                        this.orderExpiryCounters.Remove(counter);

                        this.Log.Warning(
                            $"ForceRemoveExpiredCounter() "
                          + $"({counter} at {this.TimeNow().ToIsoString()}) "
                          + $"TotalCounters={this.TotalCounters}");
                    }
                }
            }

            Debug.NotNull(this.orderExpiryCounters, nameof(this.orderExpiryCounters));
        }

        private IEnumerable<EntityId> GetCounterIdList()
        {
            return this.orderExpiryCounters.Select(counter => counter.Order.Id);
        }
    }
}
