//--------------------------------------------------------------------------------------------------
// <copyright file="PositionIdBroker.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid broker position identifier.
    /// </summary>
    [Immutable]
    public sealed class PositionIdBroker : Identifier<PositionIdBroker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionIdBroker"/> class.
        /// </summary>
        /// <param name="value">The execution ticket identifier value.</param>
        public PositionIdBroker(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
