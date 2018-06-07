// -------------------------------------------------------------------------------------------------
// <copyright file="RedisConstants.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using ServiceStack.Redis;

    /// <summary>
    /// Provides constants for the <see cref="Redis"/> database infrastructure.
    /// </summary>
    public static class RedisConstants
    {
        private const string LocalIpString = "127.0.0.1";
        private const int LocalPortInt = 6379;

        /// <summary>
        /// Gets the <see cref="Redis"/> local host internet protocol string.
        /// </summary>
        public static string LocalIp => LocalIpString;

        /// <summary>
        /// Gets the <see cref="Redis"/> default port.
        /// </summary>
        public static int DefaultPort => LocalPortInt;

        /// <summary>
        /// Gets the <see cref="Redis"/> local host end point.
        /// </summary>
        public static RedisEndpoint LocalHost => new RedisEndpoint(LocalIpString, LocalPortInt);
    }
}
