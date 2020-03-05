// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using NodaTime;

    /// <inheritdoc />
    [Immutable]
    public sealed class MessageBusAdapter : IMessageBusAdapter
    {
        private readonly MessageBus<Command> cmdBus;
        private readonly MessageBus<Event> evtBus;
        private readonly MessageBus<Message> msgBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusAdapter"/> class.
        /// </summary>
        /// <param name="cmdBus">The command bus endpoint.</param>
        /// <param name="evtBus">The event bus endpoint.</param>
        /// <param name="msgBus">The general message bus endpoint.</param>
        public MessageBusAdapter(
            MessageBus<Command> cmdBus,
            MessageBus<Event> evtBus,
            MessageBus<Message> msgBus)
        {
            this.cmdBus = cmdBus;
            this.evtBus = evtBus;
            this.msgBus = msgBus;
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
            this.msgBus.Endpoint.Send(message);
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

        /// <summary>
        /// Starts the message bus.
        /// </summary>
        public void Start()
        {
            Task.WaitAll(this.cmdBus.Start(), this.evtBus.Start(), this.msgBus.Start());
        }

        /// <summary>
        /// Stops the message bus.
        /// </summary>
        public void Stop()
        {
            Task.WaitAll(this.cmdBus.Stop(), this.evtBus.Stop(), this.msgBus.Stop());
        }

        // TODO: Below methods can be combined somehow
        private void SendToBus(Type type, Message message)
        {
            if (type == typeof(Command) || type.IsSubclassOf(typeof(Command)))
            {
                this.cmdBus.Endpoint.Send(message);
                return;
            }

            if (type == typeof(Event) || type.IsSubclassOf(typeof(Event)))
            {
                this.evtBus.Endpoint.Send(message);
                return;
            }

            if (type == typeof(Message) || type.IsSubclassOf(typeof(Message)))
            {
                this.msgBus.Endpoint.Send(message);
                return;
            }

            // Design time error
            throw ExceptionFactory.InvalidSwitchArgument(type, nameof(type));
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
                case Command _:
                    this.cmdBus.Endpoint.Send(envelope);
                    break;
                case Event _:
                    this.evtBus.Endpoint.Send(envelope);
                    break;
                default:
                    this.msgBus.Endpoint.Send(envelope);
                    break;
            }
        }
    }
}
