//--------------------------------------------------------------------------------------------------
// <copyright file="TradeProfileId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Represents a valid and unique identifier for trade profiles.
    /// </summary>
    [Immutable]
    public sealed class TradeProfileId : EntityId<TradeProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeProfileId"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public TradeProfileId(string value)
            : base(value)
        {
            Debug.NotNull(value, nameof(value));
        }
    }
}
