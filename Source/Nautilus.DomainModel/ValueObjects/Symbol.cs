//--------------------------------------------------------------------------------------------------
// <copyright file="Symbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Primitives;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents a financial market instruments symbol.
    /// </summary>
    [Immutable]
    public sealed class Symbol : ValidString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> class.
        /// </summary>
        /// <param name="code">The symbols code.</param>
        /// <param name="venue">The symbols exchange.</param>
        public Symbol(string code, Venue venue)
            : base($"{code}.{venue}")
        {
            Debug.NotNull(code, nameof(code));

            this.Code = code.ToUpper();
            this.Venue = venue;
        }

        /// <summary>
        /// Gets the symbols code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the symbols exchange.
        /// </summary>
        public Venue Venue { get; }
    }
}
