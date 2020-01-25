// -------------------------------------------------------------------------------------------------
// <copyright file="RedisConstants.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides constants for the <see cref="Redis"/> database infrastructure.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public static class RedisConstants
    {
        private const string LOCAL_HOST = "localhost";
        private const int DEFAULT_PORT = 6379;

        /// <summary>
        /// Gets the <see cref="Redis"/> local host internet protocol string.
        /// </summary>
        public static string LocalHost => LOCAL_HOST;

        /// <summary>
        /// Gets the <see cref="Redis"/> default port.
        /// </summary>
        public static int DefaultPort => DEFAULT_PORT;
    }
}
