// -------------------------------------------------------------------------------------------------
// <copyright file="InMemoryMessageStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.MessageStore
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging;

    /// <summary>
    /// Represents an in-memory message store.
    /// </summary>
    public sealed class InMemoryMessageStore : IMessageStore
    {
        private readonly IList<Envelope<Command>> commandEnvelopeList = new List<Envelope<Command>>();
        private readonly IList<Envelope<Event>> eventEnvelopeList = new List<Envelope<Event>>();
        private readonly IList<Envelope<Document>> documentEnvelopeList = new List<Envelope<Document>>();

        /// <summary>
        /// Gets a list of all stored command envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<Command>> CommandEnvelopes =>
              (IReadOnlyList<Envelope<Command>>)this.commandEnvelopeList;

        /// <summary>
        /// Gets a list of all stored event envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<Event>> EventEnvelopes =>
              (IReadOnlyList<Envelope<Event>>)this.eventEnvelopeList;

        /// <summary>
        /// Gets a list of all stored service envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<Document>> DocumentEnvelopes =>
              (IReadOnlyList<Envelope<Document>>)this.documentEnvelopeList;

        /// <summary>
        /// Stores the given envelope in the store.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        public void Store(Envelope<Command> envelope)
        {
            this.commandEnvelopeList.Add(envelope);
        }

        /// <summary>
        /// Stores the given envelope in the store.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        public void Store(Envelope<Event> envelope)
        {
            this.eventEnvelopeList.Add(envelope);
        }

        /// <summary>
        /// Stores the given envelope in the store.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        public void Store(Envelope<Document> envelope)
        {
            this.documentEnvelopeList.Add(envelope);
        }
    }
}
