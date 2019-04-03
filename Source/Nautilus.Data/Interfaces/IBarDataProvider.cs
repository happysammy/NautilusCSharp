//--------------------------------------------------------------------------------------------------
// <copyright file="IBarDataProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides the meta-data for a market data provider.
    /// </summary>
    public interface IBarDataProvider
    {
        /// <summary>
        /// Gets a list of all bar specifications provided by the data provider.
        /// </summary>
        /// <returns>The read-only collection of bar types.</returns>
        IReadOnlyCollection<BarType> SymbolBarDatas { get; }

        /// <summary>
        /// Gets the directory info for the CSV data path.
        /// </summary>
        DirectoryInfo DataPath { get; }

        /// <summary>
        /// Gets the market data providers <see cref="ZonedDateTime"/> parse pattern.
        /// </summary>
        string TimestampParsePattern { get; }

        /// <summary>
        /// Gets the market data providers volume multiple for the bars data.
        /// </summary>
        /// <returns>The volume multiple.</returns>
        int VolumeMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the bar data integrity checker is active.
        /// </summary>
        /// <returns>True if bar data checks are on, otherwise false.</returns>
        bool IsBarDataCheckOn { get; }

        /// <summary>
        /// Gets the market data providers label for the given <see cref="Resolution"/>.
        /// </summary>
        /// <param name="resolution">The bar resolution.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GetResolutionLabel(Resolution resolution);
    }
}
