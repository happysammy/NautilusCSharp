// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
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
            Validate.NotNull(commandBus, nameof(commandBus));
            Validate.NotNull(eventBus, nameof(eventBus));
            Validate.NotNull(documentBus, nameof(documentBus));

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
        /// Sends the given message to the given receiver (<see cref="Envelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        public void Send<T>(NautilusService receiver, T message, NautilusService sender)
            where T : Message
        {
            Debug.NotNull(receiver, nameof(receiver));
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            this.Send(new ReadOnlyList<NautilusService>(receiver), message, sender);
        }

        /// <summary>
        /// Sends the given message to the given receivers (<see cref="Envelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        public void Send<T>(ReadOnlyList<NautilusService> receivers, T message, NautilusService sender)
            where T : Message
        {
            Debug.NotNull(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            switch (message as Message)
            {
                    case CommandMessage commandMessage:
                        var commandEnvelope = new Envelope<CommandMessage>(
                            receivers,
                            sender,
                            commandMessage,
                            message.Id,
                            message.Timestamp);
                    this.commandBus.Send(commandEnvelope);
                    break;

                    case EventMessage eventMessage:
                        var eventEnvelope = new Envelope<EventMessage>(
                            receivers,
                            sender,
                            eventMessage,
                            message.Id,
                            message.Timestamp);
                    this.eventBus.Send(eventEnvelope);
                    break;

                    case DocumentMessage serviceMessage:
                        var serviceEnvelope = new Envelope<DocumentMessage>(
                            receivers,
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
