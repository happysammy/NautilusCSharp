// -------------------------------------------------------------------------------------------------
// <copyright file="FakeMessageStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.MessageStore
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;

    public class FakeMessageStore : IMessageStore
    {
        public IReadOnlyList<Envelope<CommandMessage>> CommandEnvelopes { get; }
        public IReadOnlyList<Envelope<EventMessage>> EventEnvelopes { get; }
        public IReadOnlyList<Envelope<DocumentMessage>> DocumentEnvelopes { get; }

        public FakeMessageStore()
        {
            this.CommandEnvelopes = new List<Envelope<CommandMessage>>();
            this.EventEnvelopes = new List<Envelope<EventMessage>>();
            this.DocumentEnvelopes = new List<Envelope<DocumentMessage>>();
        }

        public void Store(Envelope<CommandMessage> envelope)
        {
            // Does nothing.
        }

        public void Store(Envelope<EventMessage> envelope)
        {
            // Does nothing.
        }

        public void Store(Envelope<DocumentMessage> envelope)
        {
            // Does nothing.
        }
    }
}
