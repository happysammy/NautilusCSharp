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
            switch (message)
            {
                    case Command cmd:
                        var cmdEnvelope = new Envelope<Command>(
                            cmd,
                            receiver,
                            sender,
                            timestamp);
                        this.cmdBus.Endpoint.Send(cmdEnvelope);
                        break;
                    case Event evt:
                        var evtEnvelope = new Envelope<Event>(
                            evt,
                            receiver,
                            sender,
                            timestamp);
                        this.evtBus.Endpoint.Send(evtEnvelope);
                        break;
                    case Document doc:
                        var docEnvelope = new Envelope<Document>(
                            doc,
                            receiver,
                            sender,
                            timestamp);
                        this.docBus.Endpoint.Send(docEnvelope);
                        break;
                    default:
                        throw ExceptionFactory.InvalidSwitchArgument(message, nameof(message));
            }
        }
    }
}
