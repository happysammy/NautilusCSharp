//--------------------------------------------------------------------------------------------------
// <copyright file="LabelFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System;
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
            return new Label(componentName + nameof(componentName));
        }

        /// <summary>
        /// Creates and returns a new and valid service <see cref="Label"/> from the given input.
        /// </summary>
        /// <param name="service">The black box service.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label Service(Enum service)
        {
            return new Label(service + nameof(Service));
        }

        /// <summary>
        /// Creates and returns a new and valid strategy <see cref="Label"/> from the given inputs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label StrategyLabel(Symbol symbol, TradeType tradeType)
        {
            return new Label($"{symbol}({tradeType})");
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
        /// <param name="symbolBarSpec">The symbol bar specification.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label Component(string component, SymbolBarSpec symbolBarSpec)
        {
            return new Label($"{component}-{symbolBarSpec}");
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
        /// Creates and returns a new and valid entry order <see cref="Label"/> from the given
        /// inputs.
        /// </summary>
        /// <param name="signalLabel">The signal label.</param>
        /// <param name="unitNumber">The trade unit number.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label EntryOrder(Label signalLabel, int unitNumber)
        {
            return new Label($"{signalLabel}_U{unitNumber}");
        }

        /// <summary>
        /// Creates and returns a new and valid stop-loss order <see cref="Label"/> from the given
        /// inputs.
        /// </summary>
        /// <param name="signalLabel">The signal label.</param>
        /// <param name="unitNumber">The trade unit number.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label StopLossOrder(Label signalLabel, int unitNumber)
        {
            return new Label($"{signalLabel}_U{unitNumber}_SL");
        }

        /// <summary>
        /// Creates and returns a new and valid profit target order <see cref="Label"/> from the
        /// given inputs.
        /// </summary>
        /// <param name="signalLabel">The signal label.</param>
        /// <param name="unitNumber">The unit number.</param>
        /// <returns>A <see cref="Label"/>.</returns>
        public static Label ProfitTargetOrder(Label signalLabel, int unitNumber)
        {
            return new Label($"{signalLabel}_U{unitNumber}_PT");
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

        /// <summary>
        /// Creates and returns a new and valid exit <see cref="Label"/> from the given inputs.
        /// </summary>
        /// <param name="labels">The aggregated exit labels.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static Label Exit(IList<Label> labels)
        {
            if (labels.Count == 1)
            {
                return labels[0];
            }

            var firstLabel = labels[0];

            labels.RemoveAt(0);

            var moreLabels = string.Empty;

            foreach (var label in labels)
            {
                moreLabels = moreLabels + "_" + label;
            }

            return new Label(firstLabel + moreLabels);
        }

        /// <summary>
        /// Creates and returns a new and valid trailing stop <see cref="Label"/> from the given
        /// inputs.
        /// </summary>
        /// <param name="signalString">The signal string.</param>
        /// <param name="responseLabel">The signal response label.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string TrailingStop(string signalString, Label responseLabel)
        {
            if (signalString == string.Empty)
            {
                return responseLabel.ToString();
            }

            return signalString + "_" + responseLabel;
        }
    }
}
