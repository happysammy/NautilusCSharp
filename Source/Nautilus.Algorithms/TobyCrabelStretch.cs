//--------------------------------------------------------------------------------------------------
// <copyright file="TobyCrabelStretch.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms
{
    using System;
    using System.Linq;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="TobyCrabelStretch"/> algorithm.
    /// </summary>
    public class TobyCrabelStretch
    {
        private readonly int period;

        /// <summary>
        /// Initializes a new instance of the <see cref="TobyCrabelStretch"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        public TobyCrabelStretch(int period)
        {
           Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);

            this.period = period;
        }

        /// <summary>
        /// The small percent.
        /// </summary>
        /// <param name="barStore">
        /// The bar Store.
        /// </param>
        /// <returns>
        /// The <see cref="Percentage"/>.
        /// </returns>
        public Percentage SmallPercent(IBarStore barStore)
        {
           Validate.NotNull(barStore, nameof(barStore));

            var stretchPercents = new decimal[this.period];

            for (int i = 0; i < Math.Min(barStore.Count, this.period); i++)
            {
                var stretchTop = this.GetStretchTop(barStore, i);
                var stretchBottom = this.GetStretchBottom(barStore, i);

                var stretch = Math.Min(stretchTop, stretchBottom);

                var barRange = barStore.GetBarRange(i);

                var stretchPercent = barRange > 0 ? Math.Round(stretch / barRange, 2) : 0;

                stretchPercents[i] = stretchPercent * 100;
            }

            var percent = Math.Ceiling(stretchPercents.Average());
            var percentage = percent > 0 ? Percentage.Create(percent) : Percentage.Zero();

            Debug.NotNull(percentage, nameof(percentage));

            return percentage;
        }

        /// <summary>
        /// The large percent.
        /// </summary>
        /// <param name="barStore">
        /// The bar Store.
        /// </param>
        /// <returns>
        /// The <see cref="Percentage"/>.
        /// </returns>
        public Percentage LargePercent(IBarStore barStore)
        {
           Validate.NotNull(barStore, nameof(barStore));

            var stretchPercents = new decimal[this.period];

            for (int i = 0; i < Math.Min(barStore.Count, this.period); i++)
            {
                var stretchTop = this.GetStretchTop(barStore, i);
                var stretchBottom = this.GetStretchBottom(barStore, i);

                var stretch = Math.Max(stretchTop, stretchBottom);

                var barRange = barStore.GetBarRange(i);

                var stretchPercent = barRange > 0 ? Math.Round(stretch / barRange, 2) : 0;

                stretchPercents[i] = stretchPercent * 100;
            }

            var percent = Math.Ceiling(stretchPercents.Average());
            var percentage = percent > 0 ? Percentage.Create(percent) : Percentage.Zero();

            Debug.NotNull(percentage, nameof(percentage));

            return percentage;
        }

        private decimal GetStretchTop(IBarStore barStore, int index)
        {
            Debug.NotNull(barStore, nameof(barStore));
            Debug.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            var stretchTop = barStore.GetHigh(index) - Math.Max(barStore.GetOpen(index).Value, barStore.GetClose(index).Value);

            Debug.DecimalNotOutOfRange(stretchTop, nameof(stretchTop), 0, int.MaxValue);

            return stretchTop;
        }

        private decimal GetStretchBottom(IBarStore barStore, int index)
        {
            Debug.NotNull(barStore, nameof(barStore));
            Debug.DecimalNotOutOfRange(index, nameof(index), 0, int.MaxValue);

            var stretchBottom = Math.Min(barStore.GetOpen(index).Value, barStore.GetClose(index).Value) - barStore.GetLow(index);

            Debug.DecimalNotOutOfRange(stretchBottom, nameof(stretchBottom), 0, int.MaxValue);

            return stretchBottom;
        }
    }
}