//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageServer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Messages;
    using Nautilus.Messaging;
    using Nautilus.Network;
    using Nautilus.Serialization;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockMessageServer : MessageServer<MockMessage, Response>
    {
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

        public List<MockMessage> ReceivedMessages { get; }

        private void OnMessage(Envelope<MockMessage> envelope)
        {
            var received = envelope.Message;
            this.ReceivedMessages.Add(received);

            this.SendReceived(received, envelope.Sender);
        }
    }
}
