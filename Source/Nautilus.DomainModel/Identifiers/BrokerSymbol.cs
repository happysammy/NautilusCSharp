//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerSymbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a broker symbol.
    /// </summary>
    [Immutable]
    public sealed class BrokerSymbol : Identifier<BrokerSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerSymbol"/> class.
        /// </summary>
        /// <param name="value">The broker symbol value.</param>
        public BrokerSymbol(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
