//--------------------------------------------------------------
// <copyright file="OrderExpiryCounter.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// The sealed <see cref="OrderExpiryCounter"/> class.
    /// </summary>
    public sealed class OrderExpiryCounter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpiryCounter"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="barsValid">The bars valid.</param>
        /// <exception cref="ValidationException">Throws if the order is null, or if the bars valid
        /// is zero or negative.</exception>
        public OrderExpiryCounter(Order order, int barsValid)
        {
            Validate.NotNull(order, nameof(order));
            Validate.Int32NotOutOfRange(barsValid, nameof(barsValid), 0, int.MaxValue, RangeEndPoints.Exclusive);

            this.Order = order;
            this.BarsValid = barsValid;
        }

        /// <summary>
        /// Gets the order expiry counters order.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Gets the order expiry counters bars valid.
        /// </summary>
        public int BarsValid { get; }

        /// <summary>
        /// Gets the order expiry counters bars count.
        /// </summary>
        public int BarsCount { get; private set; }

        /// <summary>
        /// Increments the expiry counter.
        /// </summary>
        public void IncrementCount()
        {
            this.BarsCount++;
        }

        /// <summary>
        /// Returns a value indicating whether the order is expired.
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/>.
        /// </returns>
        public bool IsOrderExpired() => this.BarsCount >= this.BarsValid;

        /// <summary>
        /// Returns a string representation of the <see cref="OrderExpiryCounter"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"ExpiryCounter_{this.Order.OrderId}";
    }
}
