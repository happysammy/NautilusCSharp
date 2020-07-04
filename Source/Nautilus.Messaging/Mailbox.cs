//--------------------------------------------------------------------------------------------------
// <copyright file="Mailbox.cs" company="Nautech Systems Pty Ltd">
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

using System;
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Messaging.Interfaces;

namespace Nautilus.Messaging
{
    /// <summary>
    /// Represents a messaging mailbox including an <see cref="Address"/> and <see cref="Endpoint"/>.
    /// </summary>
    [Immutable]
    public sealed class Mailbox : IEquatable<Mailbox>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mailbox"/> class.
        /// </summary>
        /// <param name="address">The mailbox address.</param>
        /// <param name="endpoint">The mailbox endpoint.</param>
        public Mailbox(Address address, IEndpoint endpoint)
        {
            this.Address = address;
            this.Endpoint = endpoint;
        }

        /// <summary>
        /// Gets the mailboxes address.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// Gets the mailboxes endpoint.
        /// </summary>
        public IEndpoint Endpoint { get; }

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="Mailbox"/> is equal to the given
        /// <see cref="Mailbox"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(Mailbox other) => other.Address == this.Address;

        /// <summary>
        /// Returns the hash code for this <see cref="Mailbox"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Address);

        /// <summary>
        /// Returns a string representation of this <see cref="Mailbox"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Address.ToString();
    }
}
