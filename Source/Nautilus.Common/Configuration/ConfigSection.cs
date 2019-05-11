//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigSection.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
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
        public static string Logging { get; } = nameof(Logging).ToLower();

        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Network { get; } = nameof(Network).ToLower();

        /// <summary>
        /// Gets the FIX configuration section string.
        /// </summary>
        public static string Fix44 { get; } = nameof(Fix44).ToLower();

        /// <summary>
        /// Gets the database configuration section string.
        /// </summary>
        public static string Data { get; } = nameof(Data).ToLower();

        /// <summary>
        /// Gets the symbols to subscribe to configuration section string.
        /// </summary>
        public static string Symbols { get; } = nameof(Symbols).ToLower();
    }
}
