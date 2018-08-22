//--------------------------------------------------------------------------------------------------
// <copyright file="IMessageStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;

    /// <summary>
    /// Provides an interface for message stores.
    /// </summary>
    public interface IMessageStore
    {
        /// <summary>
        /// Gets a list of all stored command envelopes.
        /// </summary>
        IReadOnlyList<Envelope<Command>> CommandEnvelopes { get; }

        /// <summary>
        /// Gets a list of all stored event envelopes.
        /// </summary>
        IReadOnlyList<Envelope<Event>> EventEnvelopes { get; }

        /// <summary>
        /// Gets a list of all stored service envelopes.
        /// </summary>
        IReadOnlyList<Envelope<Document>> DocumentEnvelopes { get; }

        /// <summary>
        /// Stores the given envelope in the store.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        void Store(Envelope<Command> envelope);

        /// <summary>
        /// Stores the given envelope in the store.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        void Store(Envelope<Event> envelope);

        /// <summary>
        /// Stores the given envelope in the store.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        void Store(Envelope<Document> envelope);
    }
}
