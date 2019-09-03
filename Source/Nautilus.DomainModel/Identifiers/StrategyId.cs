//--------------------------------------------------------------------------------------------------
// <copyright file="StrategyId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a valid strategy identifier. The <see cref="Name"/> and <see cref="OrderIdTag"/>
    /// combination must be unique at the trader level.
    ///
    /// <para>
    /// The string representation is the name of the strategy class with an order identifier tag
    /// separated by a hyphen '-'.
    /// </para>
    ///
    /// <para>
    /// The design specification will use the <see cref="OrderIdTag"/> in identifiers sent to a
    /// brokerage, and will not use the <see cref="Name"/>.
    /// </para>
    /// </summary>
    [Immutable]
    public sealed class StrategyId : Identifier<StrategyId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrategyId"/> class.
        /// </summary>
        /// <param name="name">The strategies name (not set to broker).</param>
        /// <param name="orderIdTag">The strategies order identifier tag.</param>
        public StrategyId(string name, string orderIdTag)
            : base($"{name}-{orderIdTag}")
        {
            Debug.NotEmptyOrWhiteSpace(name, nameof(name));
            Debug.NotEmptyOrWhiteSpace(orderIdTag, nameof(orderIdTag));

            this.Name = new Label(name);
            this.OrderIdTag = new IdTag(orderIdTag);
        }

        /// <summary>
        /// Gets the strategy identifiers name label.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the strategy identifiers order identifier tag.
        /// </summary>
        public IdTag OrderIdTag { get; }

        /// <summary>
        /// Return a new <see cref="StrategyId"/> parsed from the given string value.
        /// It is expected a strategy identifier is the class name of the strategy with
        /// an order identifier tag separated by a hyphen '-'.
        /// </summary>
        /// <param name="value">The strategy identifier value.</param>
        /// <returns>The strategy identifier.</returns>
        public static StrategyId FromString(string value)
        {
            var splitString = value.Split("-", 2);

            return new StrategyId(splitString[0], splitString[1]);
        }
    }
}
