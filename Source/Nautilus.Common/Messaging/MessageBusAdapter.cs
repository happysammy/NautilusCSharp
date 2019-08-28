// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Messages;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <inheritdoc />
    [Immutable]
    public sealed class MessageBusAdapter : IMessageBusAdapter
    {
        private readonly IEndpoint cmdBus;
        private readonly IEndpoint evtBus;
        private readonly IEndpoint docBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusAdapter"/> class.
        /// </summary>
        /// <param name="cmdBus">The command bus endpoint.</param>
        /// <param name="evtBus">The event bus endpoint.</param>
        /// <param name="docBus">The document bus endpoint.</param>
        public MessageBusAdapter(
            IEndpoint cmdBus,
            IEndpoint evtBus,
            IEndpoint docBus)
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
            this.cmdBus.Send(message);
            this.evtBus.Send(message);
            this.docBus.Send(message);
        }

        /// <inheritdoc />
        public void Subscribe<T>(
            Mailbox subscriber,
            Guid id,
            ZonedDateTime timestamp)
            where T : Message
        {
            var type = typeof(T);
            var message = new Subscribe<Type>(type, subscriber, id, timestamp);

            this.SendToBus(type, message);
        }

        /// <inheritdoc />
        public void Unsubscribe<T>(
            Mailbox subscriber,
            Guid id,
            ZonedDateTime timestamp)
        where T : Message
        {
            var type = typeof(T);
            var message = new Unsubscribe<Type>(type, subscriber, id, timestamp);

            this.SendToBus(type, message);
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
        public void SendToBus<T>(T message, Address? sender, ZonedDateTime timestamp)
            where T : Message
        {
            this.WrapAndSend(message, null, sender, timestamp);
        }

        private void SendToBus(Type type, Message message)
        {
            if (type == typeof(Command) || type.IsSubclassOf(typeof(Command)))
            {
                this.cmdBus.Send(message);
                return;
            }

            if (type == typeof(Event) || type.IsSubclassOf(typeof(Event)))
            {
                this.evtBus.Send(message);
                return;
            }

            if (type == typeof(Document) || type.IsSubclassOf(typeof(Document)))
            {
                this.docBus.Send(message);
                return;
            }

            throw ExceptionFactory.InvalidSwitchArgument(type, nameof(type));  // Design time error
        }

        private void WrapAndSend<T>(
            T message,
            Address? receiver,
            Address? sender,
            ZonedDateTime timestamp)
            where T : Message
        {
            var envelope = EnvelopeFactory.Create(message, receiver, sender, timestamp);

            switch (message)
            {
                case Command cmd:
                    this.cmdBus.Send(envelope);
                    break;
                case Event evt:
                    this.evtBus.Send(envelope);
                    break;
                case Document doc:
                    this.docBus.Send(envelope);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(message, nameof(message));
            }
        }
    }
}
