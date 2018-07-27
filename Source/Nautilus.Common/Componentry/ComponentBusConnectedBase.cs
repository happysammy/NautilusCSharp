//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBusConnectedBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using Nautilus.Common.Enums;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Collections;
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
        /// <param name="serviceContext">The service context.</param>
        /// <param name="component">The component label.</param>
        /// <param name="container">The container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected ComponentBusConnectedBase(
            NautilusService serviceContext,
            Label component,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(serviceContext, component, container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.messagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Sends the given object to the given endpoint via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
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
            }
        }

        /// <summary>
        /// Sends the given messages to the given list of system services via the messaging system.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        protected void Send<T>(ReadOnlyList<NautilusService> receivers, ISendable<T> message)
            where T : class
        {
            Debug.NotNull(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            switch (message)
            {
                case Command command:
                    var commandMessage = new CommandMessage(
                        command,
                        this.NewGuid(),
                        this.TimeNow());
                    this.messagingAdapter.Send(receivers, commandMessage, this.Service);
                    break;

                case Event @event:
                    var eventMessage = new EventMessage(
                        @event,
                        this.NewGuid(),
                        this.TimeNow());
                    this.messagingAdapter.Send(receivers, eventMessage, this.Service);
                    break;
            }
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
