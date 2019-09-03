//--------------------------------------------------------------------------------------------------
// <copyright file="AtomicOrderId.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid atomic order identifier. The identifier value must be unique at the fund
    /// level.
    ///
    /// <para>
    /// It is expected that the identifier value starts with 'AO-'.
    /// </para>
    /// </summary>
    [Immutable]
    public sealed class AtomicOrderId : Identifier<AtomicOrderId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrderId"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public AtomicOrderId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
            Debug.True(value.StartsWith("AO-"), $"The value did not start with 'AO-', was {value}.");
        }
    }
}
