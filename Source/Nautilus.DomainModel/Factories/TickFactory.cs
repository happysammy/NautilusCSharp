//--------------------------------------------------------------------------------------------------
// <copyright file="TickFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a factory for creating <see cref="Tick"/>(s).
    /// </summary>
    public static class TickFactory
    {
        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbol">The symbol to create the tick with.</param>
        /// <param name="tickString">The string containing tick values.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static Tick Create(Symbol symbol, string tickString)
        {
            Debug.NotEmptyOrWhiteSpace(tickString, nameof(tickString));

            var values = tickString.Split(',');

            return new Tick(
                symbol,
                Convert.ToDecimal(values[0]),
                Convert.ToDecimal(values[1]),
                values[2].ToZonedDateTimeFromIso());
        }
    }
}
