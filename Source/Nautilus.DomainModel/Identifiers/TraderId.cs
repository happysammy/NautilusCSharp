//--------------------------------------------------------------------------------------------------
// <copyright file="TraderId.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Represents a valid trader identifier. This identifier value must be unique at fund level.
    /// </summary>
    [Immutable]
    public sealed class TraderId : Identifier<Execution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraderId"/> class.
        /// </summary>
        /// <param name="name">The traders name (not set to broker).</param>
        /// <param name="orderIdTag">The traders order identifier tag.</param>
        public TraderId(string name, string orderIdTag)
            : base($"{name}-{orderIdTag}")
        {
            Debug.NotEmptyOrWhiteSpace(name, nameof(name));
            Debug.NotEmptyOrWhiteSpace(orderIdTag, nameof(orderIdTag));

            this.Name = new Label(name);
            this.OrderIdTag = new IdTag(orderIdTag);
        }

        /// <summary>
        /// Gets the trader identifiers name label.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the trader identifiers order identifier tag.
        /// </summary>
        public IdTag OrderIdTag { get; }

        /// <summary>
        /// Return a new <see cref="TraderId"/> from the given string.
        /// </summary>
        /// <param name="value">The trader identifier value.</param>
        /// <returns>The trader identifier.</returns>
        public static TraderId FromString(string value)
        {
            var splitString = value.Split("-", 2);

            return new TraderId(splitString[0], splitString[1]);
        }
    }
}
