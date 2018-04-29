// -------------------------------------------------------------------------------------------------
// <copyright file="MessageWarehouse.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.MessageStore
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Represents an in-memory message store.
    /// </summary>
    public sealed class InMemoryMessageStore
    {
        private readonly IList<Envelope<CommandMessage>> commandEnvelopeList = new List<Envelope<CommandMessage>>();
        private readonly IList<Envelope<EventMessage>> eventEnvelopeList = new List<Envelope<EventMessage>>();
        private readonly IList<Envelope<DocumentMessage>> documentEnvelopeList = new List<Envelope<DocumentMessage>>();

        /// <summary>
        /// Gets a list of all stored command envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<CommandMessage>> CommandEnvelopes =>
              (IReadOnlyList<Envelope<CommandMessage>>)this.commandEnvelopeList;

        /// <summary>
        /// Gets a list of all stored event envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<EventMessage>> EventEnvelopes =>
              (IReadOnlyList<Envelope<EventMessage>>)this.eventEnvelopeList;

        /// <summary>
        /// Gets a list of all stored service envelopes.
        /// </summary>
        public IReadOnlyList<Envelope<DocumentMessage>> DocumentEnvelopes =>
              (IReadOnlyList<Envelope<DocumentMessage>>)this.documentEnvelopeList;

        /// <summary>
        /// Stores the given envelope in the warehouse.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <exception cref="ValidationException">Throws if the envelope is null.</exception>
        public void Store(Envelope<CommandMessage> envelope)
        {
            Validate.NotNull(envelope, nameof(envelope));

            this.commandEnvelopeList.Add(envelope);
        }

        /// <summary>
        /// Stores the given envelope in the warehouse.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <exception cref="ValidationException">Throws if the envelope is null.</exception>
        public void Store(Envelope<EventMessage> envelope)
        {
            Validate.NotNull(envelope, nameof(envelope));

            this.eventEnvelopeList.Add(envelope);
        }

        /// <summary>
        /// Stores the given envelope in the warehouse.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <exception cref="ValidationException">Throws if the envelope is null.</exception>
        public void Store(Envelope<DocumentMessage> envelope)
        {
            Validate.NotNull(envelope, nameof(envelope));

            this.documentEnvelopeList.Add(envelope);
        }
    }
}
