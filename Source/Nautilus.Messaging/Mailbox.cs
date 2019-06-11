//--------------------------------------------------------------------------------------------------
// <copyright file="Mailbox.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging.Interfaces;

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

        /// <summary>
        /// Sends the given message to the mailboxes endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(object message) => this.Endpoint.Send(message);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Mailbox"/> is equal to the given
        /// <see cref="Mailbox"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(Mailbox other) => this.Address == other.Address;

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
