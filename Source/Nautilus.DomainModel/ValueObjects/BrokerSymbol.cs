//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerSymbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Primitives;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a validated broker symbol.
    /// </summary>
    [Immutable]
    public sealed class BrokerSymbol : ValidString<BrokerSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerSymbol"/> class.
        /// </summary>
        /// <param name="symbol">The broker symbol.</param>
        public BrokerSymbol(string symbol)
            : base(symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));
        }
    }
}
