// -------------------------------------------------------------------------------------------------
// <copyright file="RedisConstants.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    /// <summary>
    /// Provides constants for the <see cref="Redis"/> database infrastructure.
    /// </summary>
    public static class RedisConstants
    {
        /// <summary>
        /// Gets the <see cref="Redis"/> localhost constant.
        /// </summary>
        public static string Localhost => "localhost";

        /// <summary>
        /// Gets the <see cref="Redis"/> default port constant.
        /// </summary>
        public static int DefaultPort => 6379;
    }
}
