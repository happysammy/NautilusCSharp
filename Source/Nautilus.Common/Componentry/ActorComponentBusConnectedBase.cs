//--------------------------------------------------------------------------------------------------
// <copyright file="ActorComponentBusConnectedBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The base class for all Akka.NET components in the system.
    /// </summary>
    [Stateless]
    public class ActorComponentBusConnectedBase : ActorComponentBase
    {
        private readonly IMessagingAdapter messagingAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorComponentBusConnectedBase"/> class.
        /// </summary>
        /// <param name="serviceContext">The components service context.</param>
        /// <param name="component">The components name.</param>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        protected ActorComponentBusConnectedBase(
            NautilusService serviceContext,
            Label component,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
                serviceContext,
                component,
                container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.messagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Sends the given message to the given service via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(NautilusService receiver, ISendable<T> message)
            where T : class
        {
            Debug.NotNull(message, nameof(message));

            switch (message)
            {
                case Command command:
                    var commandMessage = new CommandMessage(
                        command,
                        this.NewGuid(),
                        this.TimeNow());
                    this.messagingAdapter.Send(receiver, commandMessage, this.Service);
                    break;

                case Event @event:
                    var eventMessage = new EventMessage(
                        @event,
                        this.NewGuid(),
                        this.TimeNow());
                    this.messagingAdapter.Send(receiver, eventMessage, this.Service);
                    break;

                case Document document:
                    var documentMessage = new DocumentMessage(
                        document,
                        this.NewGuid(),
                        this.TimeNow());
                    this.messagingAdapter.Send(receiver, documentMessage, this.Service);
                    break;

                default:
                    throw new InvalidOperationException(
                        "Cannot send message (message type not recognized.)");
            }
        }

        /// <summary>
        /// Sends the given messages to the given list of system services via the messaging system.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(ReadOnlyList<NautilusService> receivers, ISendable<T> message)
            where T : class
        {
            Debug.CollectionNotNullOrEmpty(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            receivers.ForEach(r => this.Send(r, message));
        }

        /// <summary>
        /// Returns the <see cref="IMessagingAdapter"/> held by the base class.
        /// </summary>
        /// <returns>A <see cref="IMessagingAdapter"/>.</returns>
        protected IMessagingAdapter GetMessagingAdapter()
        {
            return this.messagingAdapter;
        }
    }
}
