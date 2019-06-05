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
    using Nautilus.Messaging.Interfaces;

    /// <inheritdoc />
    [Immutable]
    public sealed class MessagingAdapter : IMessagingAdapter
    {
        private readonly IEndpoint commandBus;
        private readonly IEndpoint eventBus;
        private readonly IEndpoint documentBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAdapter"/> class.
        /// </summary>
        /// <param name="commandBus">The command bus endpoint.</param>
        /// <param name="eventBus">The event bus endpoint.</param>
        /// <param name="documentBus">The document bus endpoint.</param>
        public MessagingAdapter(
            IEndpoint commandBus,
            IEndpoint eventBus,
            IEndpoint documentBus)
        {
            this.commandBus = commandBus;
            this.eventBus = eventBus;
            this.documentBus = documentBus;
        }

        /// <summary>
        /// Sends the message containing the <see cref="Switchboard"/> to the message bus(s) for
        /// initialization.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(InitializeSwitchboard message)
        {
            this.commandBus.Send(message);
            this.eventBus.Send(message);
            this.documentBus.Send(message);
        }

        /// <inheritdoc />
        public void Send<T>(Address receiver, T message, Address sender)
            where T : Message
        {
            switch (message)
            {
                    case Command msg:
                        var commandEnvelope = new Envelope<Command>(
                            msg,
                            receiver,
                            sender,
                            message.Timestamp);
                        this.commandBus.Send(commandEnvelope);
                        break;
                    case Event msg:
                        var eventEnvelope = new Envelope<Event>(
                            msg,
                            receiver,
                            sender,
                            message.Timestamp);
                        this.eventBus.Send(eventEnvelope);
                        break;
                    case Document msg:
                        var serviceEnvelope = new Envelope<Document>(
                            msg,
                            receiver,
                            sender,
                            message.Timestamp);
                        this.documentBus.Send(serviceEnvelope);
                        break;
                    default:
                        throw ExceptionFactory.InvalidSwitchArgument(message, nameof(message));
            }
        }
    }
}
