//--------------------------------------------------------------------------------------------------
// <copyright file="TraderId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Identifiers
{
    using Nautilus.Core.Types;
    using Nautilus.Execution.Types;

    /// <summary>
    /// Represents a <see cref="Trader"/> identifier.
    /// </summary>
    public sealed class TraderId : Identifier<Trader>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraderId"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public TraderId(string value)
        : base(value)
        {
        }
    }
}
