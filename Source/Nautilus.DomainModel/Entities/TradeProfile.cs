//--------------------------------------------------------------------------------------------------
// <copyright file="TradeProfile.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A collection of properties specifying how to manage trades of a given unique trade type.
    /// </summary>
    [Immutable]
    public sealed class TradeProfile : Entity<TradeProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeProfile"/> class.
        /// </summary>
        /// <param name="tradeType">The trade profile trade type.</param>
        /// <param name="barSpecification">The trade profile bar profile.</param>
        /// <param name="tradePeriod">The trade profile trade period.</param>
        /// <param name="units">The trade profile units.</param>
        /// <param name="unitBatches">The trade profile unit batches.</param>
        /// <param name="startOffsetMins">The trade profile start offset minutes.</param>
        /// <param name="stopOffsetMins">The trade profile stop offset minutes.</param>
        /// <param name="minStoplossDirectSpreadMultiple">The trade profile minimum stop-loss direct spread multiple.</param>
        /// <param name="minVolatilitySpreadMultiple">The trade profile minimum volatility spread multiple.</param>
        /// <param name="barsValid">The trade profile bars valid.</param>
        /// <param name="timestamp">The trade profile creation timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails (see constructor).</exception>
        public TradeProfile(
            TradeType tradeType,
            BarSpecification barSpecification,
            int tradePeriod,
            int units,
            int unitBatches,
            int startOffsetMins,
            int stopOffsetMins,
            decimal minStoplossDirectSpreadMultiple,
            decimal minVolatilitySpreadMultiple,
            int barsValid,
            ZonedDateTime timestamp)
            : base(
                  new TradeProfileId(tradeType.ToString()),
                  timestamp)
        {
            // Validate all trade profiles.
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.NotNull(barSpecification, nameof(barSpecification));
            Validate.Int32NotOutOfRange(tradePeriod, nameof(tradePeriod), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
            Validate.Int32NotOutOfRange(units, nameof(units), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
            Validate.Int32NotOutOfRange(unitBatches, nameof(unitBatches), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
            Validate.Int32NotOutOfRange(startOffsetMins, nameof(startOffsetMins), 0, int.MaxValue);
            Validate.Int32NotOutOfRange(stopOffsetMins, nameof(stopOffsetMins), 0, int.MaxValue);
            Validate.DecimalNotOutOfRange(minStoplossDirectSpreadMultiple, nameof(minStoplossDirectSpreadMultiple), 0, int.MaxValue);
            Validate.DecimalNotOutOfRange(minVolatilitySpreadMultiple, nameof(minVolatilitySpreadMultiple), 0, int.MaxValue);
            Validate.Int32NotOutOfRange(barsValid, nameof(barsValid), 0, int.MaxValue);

            this.TradeType = tradeType;
            this.BarSpecification = barSpecification;
            this.TradePeriod = tradePeriod;
            this.Units = Quantity.Create(units);
            this.UnitBatches = Quantity.Create(unitBatches);
            this.StartOffset = Duration.FromMinutes(startOffsetMins);
            this.StopOffset = Duration.FromMinutes(stopOffsetMins);
            this.MinVolatilityAverageSpreadMultiple = minVolatilitySpreadMultiple;
            this.MinStopLossDirectSpreadMultiple = minStoplossDirectSpreadMultiple;
            this.BarsValid = barsValid;
        }

        /// <summary>
        /// Gets the trade profiles trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the trade profiles bar profile.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the trade profiles trade period.
        /// </summary>
        public int TradePeriod { get; }

        /// <summary>
        /// Gets the trade profiles units (individual positions) per trade.
        /// </summary>
        public Quantity Units { get; }

        /// <summary>
        /// Gets trade profiles unit batches per position (for rounding).
        /// </summary>
        public Quantity UnitBatches { get; }

        /// <summary>
        /// Gets trade profiles offset from session open.
        /// </summary>
        public Duration StartOffset { get; }

        /// <summary>
        /// Gets trade profiles offset from session close.
        /// </summary>
        public Duration StopOffset { get; }

        /// <summary>
        /// Gets the trade profiles minimum volatility average spread multiple for entry.
        /// </summary>
        public decimal MinVolatilityAverageSpreadMultiple { get; }

        /// <summary>
        /// Gets the trade profiles stop-loss minimum direct spread multiple for entry.
        /// </summary>
        public decimal MinStopLossDirectSpreadMultiple { get; }

        /// <summary>
        /// Gets trade profiles number of bars valid for entry signals.
        /// </summary>
        public int BarsValid { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="TradeProfile"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(TradeProfile)}({this.TradeType})";
    }
}
