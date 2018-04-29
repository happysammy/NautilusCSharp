//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="OrderIdFactory"/> class.
    /// </summary>
    public sealed class OrderIdFactory
    {
        private readonly Symbol symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderIdFactory"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
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
        /// Creates and returns a new order identifier <see cref="EntityId"/>.
        /// </summary>
        /// <param name="signalTime">The signal time.</param>
        /// <returns>A <see cref="EntityId"/>.</returns>
        /// <exception cref="ValidationException">Throws if the signal time is the default value.</exception>
        public EntityId Create(ZonedDateTime signalTime)
        {
            Validate.NotDefault(signalTime, nameof(signalTime));

            this.OrderCount++;

            return EntityIdFactory.Order(signalTime, this.symbol, this.OrderCount);
        }
    }
}
