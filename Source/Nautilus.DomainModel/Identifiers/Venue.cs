//--------------------------------------------------------------------------------------------------
// <copyright file="Venue.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a financial market instruments tradeable venue.
    /// </summary>
    [Immutable]
    public sealed class Venue : Identifier<Venue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Venue"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public Venue(string value)
            : base(value.ToUpperInvariant())
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
            Debug.True(value.IsAllUpperCase(), nameof(value));
        }
    }
}
