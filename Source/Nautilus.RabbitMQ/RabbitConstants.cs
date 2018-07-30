// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitConstants.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using global::RabbitMQ.Client;

    /// <summary>
    /// Provides constants for the <see cref="RabbitMQ"/> messaging infrastructure.
    /// </summary>
    public static class RabbitConstants
    {
        private const string LocalHostString = "localhost";
        private const int DefaultPortInt = 5672;
        private const string UsernameString = "guest";
        private const string PasswordString = "guest";
        private const string ExecutionCommandsString = "nautilus.execution.commands";
        private const string ExecutionEventsString = "nautilus.execution.events";
        private const string InvarianceTraderQueueString = "inv_trader";

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> local host string.
        /// </summary>
        public static string LocalHost => LocalHostString;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> default port.
        /// </summary>
        public static int DefaultPort => DefaultPortInt;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> username.
        /// </summary>
        public static string Username => UsernameString;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> password.
        /// </summary>
        public static string Password => PasswordString;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> execution commands exchange name.
        /// </summary>
        public static string ExecutionCommandsExchange => ExecutionCommandsString;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> execution events exchange name.
        /// </summary>
        public static string ExecutionEventsExchange => ExecutionEventsString;

        /// <summary>
        /// Gets the <see cref="RabbitMQ"/> Invariance Trader client queue name.
        /// </summary>
        public static string InvarianceTraderQueue => InvarianceTraderQueueString;
    }
}
