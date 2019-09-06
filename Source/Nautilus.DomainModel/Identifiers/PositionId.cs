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

    /// <summary>
    /// Represents a valid position identifier. This identifier value must be unique at the fund
    /// level.
    ///
    /// <para>
    /// It is expected that the identifier value starts with 'P-'.
    /// </para>
    /// </summary>
    [Immutable]
    public sealed class PositionId : Identifier<PositionId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionId"/> class.
        /// </summary>
        /// <param name="value">The position identifier value.</param>
        public PositionId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
            Debug.True(value.StartsWith("P-"), $"The value did not start with 'P-', was {value}.");
        }

        /// <summary>
        /// Return a new <see cref="PositionId"/> parsed from the given string value.
        /// </summary>
        /// <param name="value">The position identifier value.</param>
        /// <returns>The position identifier.</returns>
        public static PositionId FromString(string value)
        {
            return new PositionId(value);
        }
    }
}
