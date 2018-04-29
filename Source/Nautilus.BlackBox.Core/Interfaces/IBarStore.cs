//--------------------------------------------------------------------------------------------------
// <copyright file="IBarStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NautechSystems.CSharp;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="IBarStore"/> interface. Stores and performs calculations on trade
    /// <see cref="Bar"/>(s).
    /// </summary>
    public interface IBarStore
    {
        /// <summary>
        /// Gets the bar stores bar specification.
        /// </summary>
        BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the bar stores capacity.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the open price of the last closed bar.
        /// </summary>
        Price Open { get; }

        /// <summary>
        /// Gets the high price of the last closed bar.
        /// </summary>
        Price High { get; }

        /// <summary>
        /// Gets the low price of the last closed bar.
        /// </summary>
        Price Low { get; }

        /// <summary>
        /// Gets the close price of the last closed bar.
        /// </summary>
        Price Close { get; }

        /// <summary>
        /// Gets the volume of the last closed bar.
        /// </summary>
        Quantity Volume { get; }

        /// <summary>
        /// Gets the timestamp of the last closed bar.
        /// </summary>
        ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Gets the number of elements in the list of bars.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        Bar GetBar(int index);

        /// <summary>
        /// Returns the bar which closed at the given date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        Option<Bar> GetBar(ZonedDateTime dateTime);

        /// <summary>
        /// Returns the timestamp of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        ZonedDateTime GetTimestamp(int index);

        /// <summary>
        /// Returns the open price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price GetOpen(int index);

        /// <summary>
        /// Returns the high price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price GetHigh(int index);

        /// <summary>
        /// Returns the low price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price GetLow(int index);

        /// <summary>
        /// Returns the close price of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price GetClose(int index);

        /// <summary>
        /// Returns the volume of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        Quantity GetVolume(int index);

        /// <summary>
        /// Returns the range of the bar at the given index (0 for the last closed bar).
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        decimal GetBarRange(int index);

        /// <summary>
        /// Returns the largest range within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        decimal GetLargestRange(int period, int shift);

        /// <summary>
        /// Returns the smallest range within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        decimal GetSmallestRange(int period, int shift);

        /// <summary>
        /// Returns the highest high price within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price GetMaxHigh(int period, int shift);

        /// <summary>
        /// Returns the lowest low price within the given period of bars minus the given shift.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price GetMinLow(int period, int shift);
    }
}
