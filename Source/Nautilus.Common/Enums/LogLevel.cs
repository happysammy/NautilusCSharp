//--------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// The category level of log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The verbose log level.
        /// </summary>
        Verbose,

        /// <summary>
        /// The information log level.
        /// </summary>
        Information,

        /// <summary>
        /// The debug log level.
        /// </summary>
        Debug,

        /// <summary>
        /// The warning log level.
        /// </summary>
        Warning,

        /// <summary>
        /// The error log level.
        /// </summary>
        Error,

        /// <summary>
        /// The fatal log level.
        /// </summary>
        Fatal
    }
}
