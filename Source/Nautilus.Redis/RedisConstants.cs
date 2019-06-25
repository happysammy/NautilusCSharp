// -------------------------------------------------------------------------------------------------
// <copyright file="RedisConstants.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    /// <summary>
    /// Provides constants for the <see cref="Redis"/> database infrastructure.
    /// </summary>
    public static class RedisConstants
    {
        private const string LocalHostString = "localhost";
        private const int LocalPortInt = 6379;

        /// <summary>
        /// Gets the <see cref="Redis"/> local host internet protocol string.
        /// </summary>
        public static string LocalHost => LocalHostString;

        /// <summary>
        /// Gets the <see cref="Redis"/> default port.
        /// </summary>
        public static int DefaultPort => LocalPortInt;
    }
}
