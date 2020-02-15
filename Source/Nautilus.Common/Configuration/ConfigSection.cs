//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigSection.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Configuration
{
    /// <summary>
    /// Provides JSON configuration section strings.
    /// </summary>
    public static class ConfigSection
    {
        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Logging { get; } = nameof(Logging);

        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Network { get; } = nameof(Network);

        /// <summary>
        /// Gets the FIX configuration section string.
        /// </summary>
        // ReSharper disable once InconsistentNaming (correct name)
        public static string FIX44 { get; } = nameof(FIX44);

        /// <summary>
        /// Gets the database configuration section string.
        /// </summary>
        public static string Data { get; } = nameof(Data);
    }
}
