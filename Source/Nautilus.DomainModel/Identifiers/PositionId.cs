//--------------------------------------------------------------------------------------------------
// <copyright file="PositionId.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Annotations;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents a valid position identifier. This identifier value must be unique at fund level.
    /// </summary>
    [Immutable]
    [IdentifierUniqueness(Uniqueness.Fund)]
    public sealed class PositionId : Identifier<PositionId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionId"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public PositionId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
