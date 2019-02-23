//--------------------------------------------------------------------------------------------------
// <copyright file="LabelFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// A factory which creates valid <see cref="Label"/>(s) for the system.
    /// </summary>
    [Immutable]
    public static class LabelFactory
    {
        /// <summary>
        /// Creates and returns a new and valid service <see cref="Label"/> from the given input.
        /// </summary>
        /// <param name="componentName">The components name.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label Component(string componentName)
        {
            return new Label(componentName);
        }

        /// <summary>
        /// Creates and returns a new and valid component <see cref="Label"/> from the given inputs.
        /// </summary>
        /// <param name="component">The component name.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label Component(string component, Symbol symbol)
        {
            return new Label($"{component}-{symbol}");
        }

        /// <summary>
        /// Creates and returns a new and valid component <see cref="Label"/> from the given inputs.
        /// </summary>
        /// <param name="component">The component name.</param>
        /// <param name="barType">The symbol bar specification.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label Component(string component, BarType barType)
        {
            return new Label($"{component}-{barType}");
        }

        /// <summary>
        /// Creates and returns a new and valid component <see cref="Label"/> from the given inputs.
        /// </summary>
        /// <param name="component">The component name.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label Component(string component, Symbol symbol, TradeType tradeType)
        {
            return new Label($"{component}-{symbol}-{tradeType}");
        }

        /// <summary>
        /// Creates and returns a new and valid trade unit <see cref="Label"/> from the given inputs.
        /// </summary>
        /// <param name="tradeUnitCount">The trade unit count.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label TradeUnit(int tradeUnitCount)
        {
            return new Label($"U{tradeUnitCount}");
        }
    }
}
