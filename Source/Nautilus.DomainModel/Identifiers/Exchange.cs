//--------------------------------------------------------------------------------------------------
// <copyright file="Exchange.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a valid exchange identifier. An exchange is a type of trading venue.
    /// </summary>
    [Immutable]
    public sealed class Exchange : Venue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Exchange"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public Exchange(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
