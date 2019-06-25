//--------------------------------------------------------------------------------------------------
// <copyright file="PositionId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Represents a valid and unique identifier for positions.
    /// </summary>
    [Immutable]
    public sealed class PositionId : Identifier<Position>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionId"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public PositionId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
