// -------------------------------------------------------------------------------------------------
// <copyright file="CommandRouter.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;

    /// <summary>
    /// Provides a command consumer for the messaging server.
    /// </summary>
    public class CommandRouter : Router
    {
        private readonly ICommandSerializer serializer;
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRouter"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The command serializer.</param>
        /// <param name="receiver">The receiver endpoint.</param>
        /// <param name="host">The consumers host address.</param>
        /// <param name="port">The consumers port.</param>
        public CommandRouter(
            IComponentryContainer container,
            ICommandSerializer serializer,
            IEndpoint receiver,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                host,
                port,
                Guid.NewGuid())
        {
            this.serializer = serializer;
            this.receiver = receiver;

            this.RegisterHandler<byte[]>(this.OnMessage);
        }

        private void OnMessage(byte[] message)
        {
            var command = this.serializer.Deserialize(message);
            this.Log.Debug($"Received {command}.");

            this.receiver.Send(command);
        }
    }
}
