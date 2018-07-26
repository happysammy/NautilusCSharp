// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitConstants.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    /// <summary>
    /// Provides constants for the <see cref="RabbitMQ"/> messaging infrastructure.
    /// </summary>
    public static class RabbitConstants
    {
        private const string LocalHostString = "localhost";
        private const int DefaultPortInt = 5672;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> local host string.
        /// </summary>
        public static string LocalIp => LocalHostString;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> default port.
        /// </summary>
        public static int DefaultPort => DefaultPortInt;
    }
}
