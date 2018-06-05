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
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The base class for all Akka.NET components in the system.
    /// </summary>
    public class ActorComponentBusConnectedBase : ActorComponentBase
    {
        private readonly Enum service;
        private readonly IMessagingAdapter messagingAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorComponentBusConnectedBase"/> class.
        /// </summary>
        /// <param name="service">The components service context.</param>
        /// <param name="component">The components name.</param>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        protected ActorComponentBusConnectedBase(
            Enum service,
            Label component,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
                service,
                component,
                container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.service = service;
            this.messagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Sends the given message to the given service via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        protected void Send(
            Enum receiver,
            Message message)
        {
            Validate.NotNull(message, nameof(message));

            this.messagingAdapter.Send(receiver, message, this.service);
        }

        /// <summary>
        /// Sends the given messages to the given list of system services via the messaging system.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected void Send(
            IReadOnlyCollection<Enum> receivers,
            Message message)
        {
            Debug.NotNull(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            this.messagingAdapter.Send(receivers, message, this.service);
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
