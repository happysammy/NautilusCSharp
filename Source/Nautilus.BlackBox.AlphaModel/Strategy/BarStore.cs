//--------------------------------------------------------------------------------------------------
// <copyright file="BarStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="BarStore"/> class. Stores and performs calculations on trade
    /// <see cref="Bar"/>(s).
    /// </summary>
    public class BarStore : IBarStore
    {
        private readonly IList<Bar> bars = new List<Bar>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BarStore"/> class.
        /// </summary>
        /// <param name="symbol">The bar store symbol.</param>
        /// <param name="barSpec">The bar store bar specification.</param>
        /// <param name="capacity">The bar store capacity.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// capacity is negative.</exception>
        public BarStore(Symbol symbol, BarSpecification barSpec, int capacity = 1000)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.Int32NotOutOfRange(capacity, nameof(capacity), 0, int.MaxValue);

            this.Symbol = symbol;
            this.BarSpecification = barSpec;
            this.Capacity = capacity; // TODO: implement rolling list.
        }

        /// <summary>
        /// Gets the bar stores symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the bar stores bar profile.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the bar stores capacity.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Gets the open price of the last closed bar.
        /// </summary>
        public Price Open => this.GetOpen(0);

        /// <summary>
        /// Gets the high price of the last closed bar.
        /// </summary>
        public Price High => this.GetHigh(0);

        /// <summary>
        /// Gets the low price of the last closed bar.
        /// </summary>
        public Price Low => this.GetLow(0);

        /// <summary>
        /// Gets the close price of the last closed bar.
        /// </summary>
        public Price Close => this.GetClose(0);

        /// <summary>
        /// Gets the volume of the last closed bar.
        /// </summary>
        public Quantity Volume => this.GetVolume(0);

        /// <summary>
        /// Gets the timestamp of the last closed bar.
        /// </summary>
        public ZonedDateTime Timestamp => this.GetTimestamp(0);

        /// <summary>
        /// Gets the number of elements in the list of bars.
        /// </summary>
        public int Count => this.bars.Count;

        /// <summary>
        /// Updates the bar store with the given bar.
        /// </summary>
        /// <param name="bar">The trade bar.</param>
        /// <exception cref="ValidationException">Throws if the bar is null.</exception>
        public void Update(Bar bar)
        {
            Validate.NotNull(bar, nameof(bar));

            this.bars.Add(bar);
        }

        /// <summary>
        /// Clears all <see cref="Bar"/>(s) from the store.
        /// </summary>
        public void Reset()
        {
            this.bars.Clear();
        }

        /// <summary>
        /// Returns the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public Bar GetBar(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index);
        }

        /// <summary>
        /// Returns the bar which closed at the given date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if argument is the default value.</exception>
        public Option<Bar> GetBar(ZonedDateTime dateTime)
        {
            Validate.NotDefault(dateTime, nameof(dateTime));

            return this.bars.FirstOrDefault(b => b.Timestamp == dateTime);
        }

        /// <summary>
        /// Returns the timestamp of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public ZonedDateTime GetTimestamp(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index).Timestamp;
        }

        /// <summary>
        /// Returns the open price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public Price GetOpen(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index).Open;
        }

        /// <summary>
        /// Returns the high price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public Price GetHigh(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index).High;
        }

        /// <summary>
        /// Returns the low price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public Price GetLow(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index).Low;
        }

        /// <summary>
        /// Returns the close price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public Price GetClose(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index).Close;
        }

        /// <summary>
        /// Returns the volume of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        /// <exception cref="ValidationException">Throws if the index is negative.</exception>
        public Quantity GetVolume(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.bars.GetByReverseIndex(index).Volume;
        }

        /// <summary>
        /// Returns the range of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public decimal GetBarRange(int index)
        {
            Validate.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

            return this.GetHigh(index) - this.GetLow(index);
        }

        /// <summary>
        /// Returns the largest range within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period (cannot be negative).</param>
        /// <param name="shift">The shift (must be >= 0).</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the period is zero or negative, or if
        /// the shift is negative.</exception>
        public decimal GetLargestRange(int period, int shift)
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(shift, nameof(shift), 0, int.MaxValue);

            return this.bars
               .Skip(this.Count - period - shift)
               .Take(period)
               .Select(b => b.High - b.Low)
               .Max();
        }

        /// <summary>
        /// Returns the smallest range within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the period is zero or negative, or if
        /// the shift is negative.</exception>
        public decimal GetSmallestRange(int period, int shift)
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(shift, nameof(shift), 0, int.MaxValue);

            return this.bars
               .Skip(this.Count - period - shift)
               .Take(period)
               .Select(b => b.High - b.Low)
               .Min();
        }

        /// <summary>
        /// Returns the highest high price within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the period is zero or negative, or if
        /// the shift is negative.</exception>
        public Price GetMaxHigh(int period, int shift)
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(shift, nameof(shift), 0, int.MaxValue);

            return this.bars
               .Skip(this.Count - period - shift)
               .Take(period)
               .Select(b => b.High)
               .Max();
        }

        /// <summary>
        /// Returns the lowest low price within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the period is zero or negative, or if
        /// the shift is negative.</exception>
        public Price GetMinLow(int period, int shift)
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(shift, nameof(shift), 0, int.MaxValue);

            return this.bars
               .Skip(this.Count - period - shift)
               .Take(period)
               .Select(b => b.Low)
               .Min();
        }
    }
}
