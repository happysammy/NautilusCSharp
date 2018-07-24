//--------------------------------------------------------------------------------------------------
// <copyright file="DocumentMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all document messages.
    /// </summary>
    [Immutable]
    public sealed class DocumentMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMessage"/> class.
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public DocumentMessage(
            Document document,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
            Debug.NotNull(Document, nameof(Document));

            this.Document = document;
        }

        /// <summary>
        /// Gets the messages document.
        /// </summary>
        public Document Document { get; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override Type Type => this.Document.GetType();

        /// <summary>
        /// Returns a string representation of the <see cref="DocumentMessage"/>.
        /// </summary>
        /// <returns>The event name.</returns>
        public override string ToString() => this.Document.ToString();
    }
}
