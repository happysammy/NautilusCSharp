//--------------------------------------------------------------
// <copyright file="IBarDataProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using NodaTime;

namespace Nautilus.Database.Core.Interfaces
{
    /// <summary>
    /// Provides the meta-data for a market data provider.
    /// </summary>
    public interface IBarDataProvider : ICsvDataConfigEditor
    {
        /// <summary>
        /// Gets a list of all bar specifications provided by the data provider.
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<BarSpecification> BarSpecifications { get; }

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
        /// <returns></returns>
        int VolumeMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the bar data integrity checker is active.
        /// </summary>
        /// <returns></returns>
        bool IsBarDataCheckOn { get; }

        /// <summary>
        /// Gets the market data providers label for the given <see cref="BarResolution"/>.
        /// </summary>
        /// <param name="resolution">The bar resolution.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GetResolutionLabel(BarResolution resolution);
    }
}