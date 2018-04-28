//--------------------------------------------------------------
// <copyright file="VolatilityChecker.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Algorithms
{
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// The volatility checker.
    /// </summary>
    public class VolatilityChecker
    {
        private readonly decimal minVolatilityAverageSpreadMultiple;

        /// <summary>
        /// Initializes a new instance of the <see cref="VolatilityChecker"/> class.
        /// </summary>
        /// <param name="minVolatilityAverageSpreadMultiple">
        /// The min volatility average spread multiple.
        /// </param>
        public VolatilityChecker(decimal minVolatilityAverageSpreadMultiple)
        {
           Validate.DecimalNotOutOfRange(minVolatilityAverageSpreadMultiple, nameof(minVolatilityAverageSpreadMultiple), decimal.Zero, decimal.MaxValue);

            this.minVolatilityAverageSpreadMultiple = minVolatilityAverageSpreadMultiple;
        }

        /// <summary>
        /// The is tradable.
        /// </summary>
        /// <param name="averageSpread">
        /// The average Spread.
        /// </param>
        /// <param name="volatility">
        /// The volatility.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsTradable(decimal averageSpread, decimal volatility)
        {
           Validate.DecimalNotOutOfRange(averageSpread, nameof(averageSpread), decimal.Zero, decimal.MaxValue);
           Validate.DecimalNotOutOfRange(volatility, nameof(volatility), decimal.Zero, decimal.MaxValue);

            return volatility >= averageSpread * this.minVolatilityAverageSpreadMultiple;
        }
    }
}