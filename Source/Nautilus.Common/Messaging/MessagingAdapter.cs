// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The immutable sealed <see cref="MessagingAdapter"/> class.
    /// </summary>
    [Immutable]
    public sealed class MessagingAdapter : IMessagingAdapter
    {
        private readonly IEndpoint commandBus;
        private readonly IEndpoint eventBus;
        private readonly IEndpoint documentBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAdapter"/> class.
        /// </summary>
        /// <param name="commandBus">The command bus actor address.</param>
        /// <param name="eventBus">The event bus actor address.</param>
        /// <param name="documentBus">The document bus actor address.</param>
        public MessagingAdapter(
            IEndpoint commandBus,
            IEndpoint eventBus,
            IEndpoint documentBus)
        {
            Precondition.NotNull(commandBus, nameof(commandBus));
            Precondition.NotNull(eventBus, nameof(eventBus));
            Precondition.NotNull(documentBus, nameof(documentBus));

            this.commandBus = commandBus;
            this.eventBus = eventBus;
            this.documentBus = documentBus;
        }

        /// <summary>
        /// Sends the message switchboard to the message bus(s) for initialization.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(InitializeSwitchboard message)
        {
            Debug.NotNull(message, nameof(message));

            this.commandBus.Send(message);
            this.eventBus.Send(message);
            this.documentBus.Send(message);
        }

        /// <summary>
        /// Sends the given message to the given receivers (<see cref="Envelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        public void Send<T>(Address receiver, T message, Address sender)
            where T : Message
        {
            Debug.NotNull(receiver, nameof(receiver));
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            switch (message as Message)
            {
                    case Command commandMessage:
                        var commandEnvelope = new Envelope<Command>(
                            receiver,
                            sender,
                            commandMessage,
                            message.Id,
                            message.Timestamp);
                        this.commandBus.Send(commandEnvelope);
                        break;

                    case Event eventMessage:
                        var eventEnvelope = new Envelope<Event>(
                            receiver,
                            sender,
                            eventMessage,
                            message.Id,
                            message.Timestamp);
                        this.eventBus.Send(eventEnvelope);
                        break;

                    case Document serviceMessage:
                        var serviceEnvelope = new Envelope<Document>(
                            receiver,
                            sender,
                            serviceMessage,
                            message.Id,
                            message.Timestamp);
                        this.documentBus.Send(serviceEnvelope);
                        break;

                    default: throw new InvalidOperationException(
                        $"Cannot send message ({message.GetType()} message type is not supported).");
            }
        }
    }
}
