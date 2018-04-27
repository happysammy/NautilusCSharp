// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingAdapter.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Messaging
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.Messaging.Base;

    /// <summary>
    /// The immutable sealed <see cref="MessagingAdapter"/> class.
    /// </summary>
    [Immutable]
    public sealed class MessagingAdapter : IMessagingAdapter
    {
        private readonly IActorRef commandBus;
        private readonly IActorRef eventBus;
        private readonly IActorRef serviceBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAdapter"/> class.
        /// </summary>
        /// <param name="commandBusRef">The command bus actor address.</param>
        /// <param name="eventBusRef">The event bus actor address.</param>
        /// <param name="serviceBusRef">The service bus actor address.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public MessagingAdapter(
            IActorRef commandBusRef,
            IActorRef eventBusRef,
            IActorRef serviceBusRef)
        {
            Validate.NotNull(commandBusRef, nameof(commandBusRef));
            Validate.NotNull(eventBusRef, nameof(eventBusRef));
            Validate.NotNull(serviceBusRef, nameof(serviceBusRef));

            this.commandBus = commandBusRef;
            this.eventBus = eventBusRef;
            this.serviceBus = serviceBusRef;
        }

        /// <summary>
        /// Sends the message switchboard to the message bus(s) for initialization.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        public void Send(InitializeMessageSwitchboard message)
        {
            Validate.NotNull(message, nameof(message));

            this.commandBus.Tell(message);
            this.eventBus.Tell(message);
            this.serviceBus.Tell(message);
        }

        /// <summary>
        /// Sends the given message to the given receiver (<see cref="MessageEnvelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        public void Send<T>(Enum receiver, T message, Enum sender) where T : Message
        {
            Validate.NotNull(message, nameof(message));

            this.Send(new List<Enum> { receiver }, message, sender);
        }

        /// <summary>
        /// Sends the given message to the given receivers (<see cref="MessageEnvelope{T}"/> marked
        /// from the given sender).
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <exception cref="ValidationException">Throws if the receivers are null, or if the message
        /// is null.</exception>
        /// <exception cref="InvalidOperationException">Throws if a receiver is unknown.</exception>
        public void Send<T>(IReadOnlyCollection<Enum> receivers, T message, Enum sender) where T : Message
        {
            Validate.NotNull(receivers, nameof(receivers));
            Validate.NotNull(message, nameof(message));

            switch (message as Message)
            {
                    case CommandMessage commandMessage:
                        var commandEvelope = new Envelope<CommandMessage>(
                            receivers,
                            sender,
                            commandMessage,
                            message.Id,
                            message.Timestamp);
                    this.commandBus.Tell(commandEvelope);
                    break;

                    case EventMessage eventMessage:
                        var eventEnvelope = new Envelope<EventMessage>(
                            receivers,
                            sender,
                            eventMessage,
                            message.Id,
                            message.Timestamp);
                    this.eventBus.Tell(eventEnvelope);
                    break;

                    case DocumentMessage serviceMessage:
                        var serviceEnvelope = new Envelope<DocumentMessage>(
                            receivers,
                            sender,
                            serviceMessage,
                            message.Id,
                            message.Timestamp);
                    this.serviceBus.Tell(serviceEnvelope);
                    break;

                default: throw new InvalidOperationException($"{message.GetType()} message type not supported.");
            }
        }
    }
}
