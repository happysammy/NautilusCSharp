//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdBroker.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid broker order identifier. This identifier value must be unique at fund level.
    /// </summary>
    [Immutable]
    public sealed class OrderIdBroker : Identifier<OrderIdBroker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderIdBroker"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public OrderIdBroker(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
