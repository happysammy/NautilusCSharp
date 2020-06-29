//--------------------------------------------------------------------------------------------------
// <copyright file="IMessageStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.Message;
    using Nautilus.Messaging;

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
