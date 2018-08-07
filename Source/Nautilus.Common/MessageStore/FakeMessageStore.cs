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
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides a fake message store implementation.
    /// </summary>
    [Stateless]
    public class FakeMessageStore : IMessageStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMessageStore"/> class.
        /// </summary>
        public FakeMessageStore()
        {
            this.CommandEnvelopes = new List<Envelope<CommandMessage>>();
            this.EventEnvelopes = new List<Envelope<EventMessage>>();
            this.DocumentEnvelopes = new List<Envelope<DocumentMessage>>();
        }

        /// <summary>
        /// Gets the message stores command envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<CommandMessage>> CommandEnvelopes { get; }

        /// <summary>
        /// Gets the message stores event envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<EventMessage>> EventEnvelopes { get; }

        /// <summary>
        /// Gets the message stores document envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<DocumentMessage>> DocumentEnvelopes { get; }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        public void Store(Envelope<CommandMessage> envelope)
        {
            // Does nothing.
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        public void Store(Envelope<EventMessage> envelope)
        {
            // Does nothing.
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        public void Store(Envelope<DocumentMessage> envelope)
        {
            // Does nothing.
        }
    }
}
