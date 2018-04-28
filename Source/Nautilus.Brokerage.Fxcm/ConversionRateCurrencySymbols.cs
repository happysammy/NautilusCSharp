//---------------------------------------------------------------------------------
// <copyright file="ConversionRateCurrencySymbols.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The conversion rate currency symbols required by the quote provider for position sizing.
    /// </summary>
    public static class ConversionRateCurrencySymbols
    {
        /// <summary>
        /// Returns the list of conversion rate currency symbols.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyCollection{Symbol}"/>.</returns>
        public static IReadOnlyCollection<Symbol> GetList()
        {
            return new List<Symbol>
            {
                new Symbol("AUDUSD", Exchange.FXCM),
                new Symbol("AUDJPY", Exchange.FXCM)
            };
        }
    }
}
