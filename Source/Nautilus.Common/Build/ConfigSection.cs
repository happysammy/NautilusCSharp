//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigSection.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Build
{
    /// <summary>
    /// Provides JSON configuration section strings.
    /// </summary>
    public static class ConfigSection
    {
        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Logging => "logging";

        /// <summary>
        /// Gets the database configuration section string.
        /// </summary>
        public static string Database => "database";

        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Service => "service";

        /// <summary>
        /// Gets the Service Stack configuration section string.
        /// </summary>
        public static string ServiceStack => "serviceStack";

        /// <summary>
        /// Gets the FIX configuration section string.
        /// </summary>
        public static string Fix44 => "fix44";

        /// <summary>
        /// Gets the symbols to subscribe to configuration section string.
        /// </summary>
        public static string Symbols => "symbols";
    }
}
