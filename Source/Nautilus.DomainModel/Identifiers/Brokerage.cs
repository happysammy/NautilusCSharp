//--------------------------------------------------------------------------------------------------
// <copyright file="Brokerage.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid brokerage identifier. The identifier value must be unique at the global
    /// level.
    /// </summary>
    [Immutable]
    public sealed class Brokerage : Identifier<Brokerage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Brokerage"/> class.
        /// </summary>
        /// <param name="name">The brokerage name identifier value.</param>
        public Brokerage(string name)
            : base(name)
        {
            Debug.NotEmptyOrWhiteSpace(name, nameof(name));
        }
    }
}
