// -------------------------------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Enums
{
    /// <summary>
    /// The black box <see cref="LogLevel"/> enumeration.
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