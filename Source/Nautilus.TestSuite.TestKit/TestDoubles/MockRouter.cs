//--------------------------------------------------------------------------------------------------
// <copyright file="MockRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;

    /// <summary>
    /// Provides a mock router for testing.
    /// </summary>
    public sealed class MockRouter : Router
    {
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockRouter"/> class.
        /// </summary>
        /// <param name="receiver">The test receiver.</param>
        /// <param name="container">The container.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="id">The identifier.</param>
        public MockRouter(
            IEndpoint receiver,
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(
                container,
                host,
                port,
                id)
        {
            this.receiver = receiver;

            this.RegisterHandler<byte[]>(this.OnMessage);
        }

        private void OnMessage(byte[] message)
        {
            this.Log.Debug($"Received {message}.");

            this.receiver.Send(message);
        }
    }
}
