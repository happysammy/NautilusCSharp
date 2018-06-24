//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigSection.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Configuration
{
    /// <summary>
    /// Provides configuration sections.
    /// </summary>
    public static class ConfigSection
    {
        /// <summary>
        /// The database configuration section.
        /// </summary>
        public static string Database => "database";

        /// <summary>
        /// The service stack configuration section.
        /// </summary>
        public static string ServiceStack => "serviceStack";

        /// <summary>
        /// The FXCM configuration section.
        /// </summary>
        public static string Fxcm => "fxcm";

        /// <summary>
        /// The Dukascopy configuration section.
        /// </summary>
        public static string Dukascopy => "dukascopy";
    }
}
