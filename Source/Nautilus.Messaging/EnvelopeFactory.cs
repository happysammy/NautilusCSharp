//--------------------------------------------------------------------------------------------------
// <copyright file="EnvelopeFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a factory for creating generic envelopes of type T.
    /// </summary>
    public static class EnvelopeFactory
    {
        private static readonly Type GenericEnvelope = typeof(Envelope<>);

        /// <summary>
        /// Returns a new <see cref="Envelope{T}"/> based on the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">The message to wrap.</param>
        /// <param name="receiver">The envelope receiver.</param>
        /// <param name="sender">The envelope sender.</param>
        /// <param name="timestamp">The envelope creation timestamp.</param>
        /// <returns>The created envelope.</returns>
        public static IEnvelope Create(
            Message message,
            Address? receiver,
            Address? sender,
            ZonedDateTime timestamp)
        {
            var typedEnvelope = GenericEnvelope.MakeGenericType(message.Type);
            return (IEnvelope)Activator.CreateInstance(typedEnvelope, message, receiver, sender, timestamp);
        }
    }
}
