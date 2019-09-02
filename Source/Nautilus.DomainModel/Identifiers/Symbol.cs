//--------------------------------------------------------------------------------------------------
// <copyright file="Symbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Annotations;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents a valid symbol identifier. A symbol is the unique identity of a tradeable instrument.
    /// </summary>
    [Immutable]
    [IdentifierUniqueness(Uniqueness.Global)]
    public sealed class Symbol : Identifier<Symbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> class.
        /// </summary>
        /// <param name="code">The symbols code.</param>
        /// <param name="venue">The symbols venue.</param>
        public Symbol(string code, Venue venue)
            : base($"{code.ToUpper()}.{venue.Value}")
        {
            Debug.NotEmptyOrWhiteSpace(code, nameof(code));
            Debug.True(code.IsAllUpperCase(), $"The symbol code value '{code}' was not all upper case.");

            this.Code = code;
            this.Venue = venue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> class.
        /// </summary>
        /// <param name="code">The symbols code.</param>
        /// <param name="venue">The symbols venue.</param>
        public Symbol(string code, string venue)
            : this(code, new Venue(venue))
        {
            Debug.NotEmptyOrWhiteSpace(code, nameof(code));
            Debug.NotEmptyOrWhiteSpace(venue, nameof(venue));
        }

        /// <summary>
        /// Gets the symbols code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the symbols venue.
        /// </summary>
        public Venue Venue { get; }

        /// <summary>
        /// Returns a new <see cref="Symbol"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbolString">The symbol string.</param>
        /// <returns>The created <see cref="Symbol"/>.</returns>
        public static Symbol FromString(string symbolString)
        {
            Debug.NotEmptyOrWhiteSpace(symbolString, nameof(symbolString));

            var symbolSplit = symbolString.Split('.');

            return new Symbol(symbolSplit[0], new Venue(symbolSplit[1]));
        }
    }
}
