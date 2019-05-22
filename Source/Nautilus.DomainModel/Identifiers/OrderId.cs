//--------------------------------------------------------------------------------------------------
// <copyright file="OrderId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Represents a valid and unique identifier for orders.
    /// </summary>
    [Immutable]
    public sealed class OrderId : Identifier<Order>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderId"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public OrderId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
