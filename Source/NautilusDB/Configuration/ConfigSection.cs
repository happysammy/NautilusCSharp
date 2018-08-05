//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigSection.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Configuration
{
    /// <summary>
    /// Provides configuration sections.
    /// </summary>
    public static class ConfigSection
    {
        /// <summary>
        /// The logging configuration section.
        /// </summary>
        public static string Logging => "logging";

        /// <summary>
        /// The database configuration section.
        /// </summary>
        public static string Database => "database";

        /// <summary>
        /// The service stack configuration section.
        /// </summary>
        public static string ServiceStack => "serviceStack";

        /// <summary>
        /// The FIX configuration section.
        /// </summary>
        public static string Fix => "fix_config";

        /// <summary>
        /// The bar specifications configuration section.
        /// </summary>
        public static string BarSpecifications => "barSpecifications";

        /// <summary>
        /// The symbols to subscribe to configuration section.
        /// </summary>
        public static string Symbols => "symbols";
    }
}
