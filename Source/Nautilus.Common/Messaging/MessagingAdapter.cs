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
    using Nautilus.Core.Enums;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <inheritdoc />
    [Immutable]
    public sealed class MessagingAdapter : IMessagingAdapter
    {
        private readonly IEndpoint cmdBus;
        private readonly IEndpoint evtBus;
        private readonly IEndpoint docBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingAdapter"/> class.
        /// </summary>
        /// <param name="cmdBus">The command bus endpoint.</param>
        /// <param name="evtBus">The event bus endpoint.</param>
        /// <param name="docBus">The document bus endpoint.</param>
        public MessagingAdapter(
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
        public void Send<T>(
            T message,
            Address receiver,
            Address sender,
            ZonedDateTime timestamp)
            where T : Message
        {
            var envelope = new Envelope<T>(
                message,
                receiver,
                sender,
                timestamp);

            switch (message.BaseType)
            {
                    case MessageType.Command:
                        this.cmdBus.Send(envelope);
                        break;
                    case MessageType.Event:
                        this.evtBus.Send(envelope);
                        break;
                    case MessageType.Document:
                        this.docBus.Send(envelope);
                        break;
                    case MessageType.Unknown:
                        goto default;
                    case MessageType.Request:
                        goto default;
                    case MessageType.Response:
                        goto default;
                    default:
                        throw ExceptionFactory.InvalidSwitchArgument(message, nameof(message));
            }
        }
    }
}
