//--------------------------------------------------------------------------------------------------
// <copyright file="Venue.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    /// Represents valid venue identifier. A venue is the tradeable location for financial market
    /// instruments. The identifier value must be unique at the fund level.
    /// </summary>
    [Immutable]
    public class Venue : Identifier<Venue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Venue"/> class.
        /// </summary>
        /// <param name="name">The venue name identifier value.</param>
        public Venue(string name)
            : base(name.ToUpperInvariant())
        {
            Debug.NotEmptyOrWhiteSpace(name, nameof(name));
            Debug.True(name.IsAllUpperCase(), $"The venue value '{name}' was not all upper case.");
        }
    }
}
