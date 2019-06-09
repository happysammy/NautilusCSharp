// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;
    using NodaTime;

    /// <inheritdoc />
    [Immutable]
    public sealed class MessagingAdapter : IMessagingAdapter
    {
        private readonly MessageBus<Command> cmdBus;
        private readonly MessageBus<Event> evtBus;
        private readonly MessageBus<Document> docBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAdapter"/> class.
        /// </summary>
        /// <param name="cmdBus">The command bus endpoint.</param>
        /// <param name="evtBus">The event bus endpoint.</param>
        /// <param name="docBus">The document bus endpoint.</param>
        public MessagingAdapter(
            MessageBus<Command> cmdBus,
            MessageBus<Event> evtBus,
            MessageBus<Document> docBus)
        {
            this.cmdBus = cmdBus;
            this.evtBus = evtBus;
            this.docBus = docBus;
        }

        /// <summary>
        /// Sends the message containing the <see cref="Switchboard"/> to the message bus(s) for
        /// initialization.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(InitializeSwitchboard message)
        {
            this.cmdBus.Endpoint.Send(message);
            this.evtBus.Endpoint.Send(message);
            this.docBus.Endpoint.Send(message);
        }

        /// <inheritdoc />
        public void Send<T>(
            T message,
            Address receiver,
            Address sender,
            ZonedDateTime timestamp)
            where T : Message
        {
            this.WrapAndSend(message, receiver, sender, timestamp);
        }

        /// <inheritdoc/>
        public void SendToBus<T>(T message, Address sender, ZonedDateTime timestamp)
            where T : Message
        {
            this.WrapAndSend(message, null, sender, timestamp);
        }

        private void WrapAndSend<T>(
            T message,
            Address? receiver,
            Address sender,
            ZonedDateTime timestamp)
            where T : Message
        {
            var envelope = EnvelopeFactory.Create(message, receiver, sender, timestamp);

            switch (message)
            {
                case Command cmd:
                    this.cmdBus.Endpoint.Send(envelope);
                    break;
                case Event evt:
                    this.evtBus.Endpoint.Send(envelope);
                    break;
                case Document doc:
                    this.docBus.Endpoint.Send(envelope);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(message, nameof(message));
            }
        }
    }
}
