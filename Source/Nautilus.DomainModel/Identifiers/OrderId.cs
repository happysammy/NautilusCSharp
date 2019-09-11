//--------------------------------------------------------------------------------------------------
// <copyright file="OrderId.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid order identifier. The identifier value must be unique at the fund level.
    ///
    /// <para>
    /// It is expected that the identifier value starts with 'O-'.
    /// </para>
    /// </summary>
    [Immutable]
    public sealed class OrderId : Identifier<OrderId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderId"/> class.
        /// </summary>
        /// <param name="value">The order identifier value.</param>
        public OrderId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));

            // Debug.True(value.StartsWith("O-"), $"The value did not start with 'O-', was {value}.");
        }

        /// <summary>
        /// Return a new <see cref="OrderId"/> parsed from the given string value.
        /// </summary>
        /// <param name="value">The order identifier value.</param>
        /// <returns>The order identifier.</returns>
        public static OrderId FromString(string value)
        {
            return new OrderId(value);
        }
    }
}
