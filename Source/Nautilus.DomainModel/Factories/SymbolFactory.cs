//--------------------------------------------------------------------------------------------------
// <copyright file="SymbolFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a factory for creating <see cref="Symbol"/>(s).
    /// </summary>
    public static class SymbolFactory
    {
        /// <summary>
        /// Returns a new <see cref="Symbol"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbolString">The symbol string.</param>
        /// <returns>The created <see cref="Symbol"/>.</returns>
        public static Symbol Create(string symbolString)
        {
            Debug.NotEmptyOrWhiteSpace(symbolString, nameof(symbolString));

            var symbolSplit = symbolString.Split('.');

            return new Symbol(symbolSplit[0], symbolSplit[1].ToEnum<Venue>());
        }
    }
}
