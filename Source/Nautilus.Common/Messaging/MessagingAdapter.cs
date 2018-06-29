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
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Collections;

    /// <summary>
    /// The immutable sealed <see cref="MessagingAdapter"/> class.
    /// </summary>
    [Immutable]
    public sealed class MessagingAdapter : IMessagingAdapter
    {
        private readonly IActorRef commandBusRef;
        private readonly IActorRef eventBusRef;
        private readonly IActorRef documentBusRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAdapter"/> class.
        /// </summary>
        /// <param name="commandBusRef">The command bus actor address.</param>
        /// <param name="eventBusRef">The event bus actor address.</param>
        /// <param name="documentBusRef">The document bus actor address.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public MessagingAdapter(
            IActorRef commandBusRef,
            IActorRef eventBusRef,
            IActorRef documentBusRef)
        {
            Validate.NotNull(commandBusRef, nameof(commandBusRef));
            Validate.NotNull(eventBusRef, nameof(eventBusRef));
            Validate.NotNull(documentBusRef, nameof(documentBusRef));

            this.commandBusRef = commandBusRef;
            this.eventBusRef = eventBusRef;
            this.documentBusRef = documentBusRef;
        }

        /// <summary>
        /// Sends the message switchboard to the message bus(s) for initialization.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        public void Send(InitializeMessageSwitchboard message)
        {
            Validate.NotNull(message, nameof(message));

            this.commandBusRef.Tell(message);
            this.eventBusRef.Tell(message);
            this.documentBusRef.Tell(message);
        }

        /// <summary>
        /// Sends the given message to the given receiver (<see cref="Envelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        public void Send<T>(Enum receiver, T message, Enum sender)
            where T : Message
        {
            Debug.NotNull(message, nameof(message));

            this.Send(new ReadOnlyList<Enum>(receiver), message, sender);
        }

        /// <summary>
        /// Sends the given message to the given receivers (<see cref="Envelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <exception cref="ValidationException">Throws if the receivers are null, or if the message
        /// is null.</exception>
        /// <exception cref="InvalidOperationException">Throws if a receiver is unknown.</exception>
        public void Send<T>(ReadOnlyList<Enum> receivers, T message, Enum sender)
            where T : Message
        {
            Debug.NotNull(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            switch (message as Message)
            {
                    case CommandMessage commandMessage:
                        var commandEvelope = new Envelope<CommandMessage>(
                            receivers,
                            sender,
                            commandMessage,
                            message.Id,
                            message.Timestamp);
                    this.commandBusRef.Tell(commandEvelope);
                    break;

                    case EventMessage eventMessage:
                        var eventEnvelope = new Envelope<EventMessage>(
                            receivers,
                            sender,
                            eventMessage,
                            message.Id,
                            message.Timestamp);
                    this.eventBusRef.Tell(eventEnvelope);
                    break;

                    case DocumentMessage serviceMessage:
                        var serviceEnvelope = new Envelope<DocumentMessage>(
                            receivers,
                            sender,
                            serviceMessage,
                            message.Id,
                            message.Timestamp);
                    this.documentBusRef.Tell(serviceEnvelope);
                    break;

                default: throw new InvalidOperationException($"{message.GetType()} message type not supported.");
            }
        }
    }
}
