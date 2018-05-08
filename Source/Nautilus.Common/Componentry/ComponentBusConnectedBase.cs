//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBusConnectedBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The base class for all components which are connected to the messaging service.
    /// </summary>
    public abstract class ComponentBusConnectedBase : ComponentBase
    {
        /// <summary>
        /// Gets the components messaging adapter.
        /// </summary>
        private readonly IMessagingAdapter messagingAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBusConnectedBase"/> class.
        /// </summary>
        /// <param name="service">The service context.</param>
        /// <param name="component">The component label.</param>
        /// <param name="container">The container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected ComponentBusConnectedBase(
            Enum service,
            Label component,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(service, component, container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.messagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Sends the given message to the given black box service via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        protected void Send(
            Enum receiver,
            Message message)
        {
            Debug.NotNull(message, nameof(message));

            this.messagingAdapter.Send(receiver, message, this.Service);
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

            this.messagingAdapter.Send(receivers, message, this.Service);
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
