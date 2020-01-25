//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    /// Represents a valid instrument identifier. The identifier value must be unique at the fund
    /// level.
    /// </summary>
    [Immutable]
    public sealed class InstrumentId : Identifier<InstrumentId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentId"/> class.
        /// </summary>
        /// <param name="value">The instrument identifier value.</param>
        public InstrumentId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
