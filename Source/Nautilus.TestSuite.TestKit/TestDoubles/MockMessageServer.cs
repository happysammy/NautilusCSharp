//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageServer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization;

    /// <summary>
    /// Provides a mock server for testing.
    /// </summary>
    public sealed class MockMessageServer : MessageServer<MockMessage, Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockMessageServer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="id">The identifier.</param>
        public MockMessageServer(
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(
                container,
                new MockSerializer(),
                new MsgPackResponseSerializer(),
                host,
                port,
                id)
        {
            this.ReceivedMessages = new List<MockMessage>();

            this.RegisterHandler<Envelope<MockMessage>>(this.OnMessage);
        }

        /// <summary>
        /// Gets the list of received messages.
        /// </summary>
        public List<MockMessage> ReceivedMessages { get; }

        private void OnMessage(Envelope<MockMessage> envelope)
        {
            var received = envelope.Message;
            this.ReceivedMessages.Add(received);

            var response = new MessageReceived(
                received.Type.Name,
                received.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(envelope.Sender, response);
        }
    }
}
