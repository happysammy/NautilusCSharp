//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides new order identifiers.
    /// </summary>
    public sealed class OrderIdFactory
    {
        private readonly Symbol symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderIdFactory"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public OrderIdFactory(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            this.symbol = symbol;
        }

        /// <summary>
        /// Gets the factories order count.
        /// </summary>
        public int OrderCount { get; private set; }

        /// <summary>
        /// Creates and returns a new <see cref="OrderId"/>.
        /// </summary>
        /// <param name="signalTime">The signal time.</param>
        /// <returns>A <see cref="OrderId"/>.</returns>
        public OrderId Create(ZonedDateTime signalTime)
        {
            Debug.NotDefault(signalTime, nameof(signalTime));

            this.OrderCount++;

            return EntityIdFactory.Order(signalTime, this.symbol, this.OrderCount);
        }
    }
}
