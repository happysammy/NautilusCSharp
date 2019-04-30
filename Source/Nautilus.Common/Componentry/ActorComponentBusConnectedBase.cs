//--------------------------------------------------------------------------------------------------
// <copyright file="ActorComponentBusConnectedBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The base class for all Akka.NET components in the system.
    /// </summary>
    public class ActorComponentBusConnectedBase : ActorComponentBase
    {
        private readonly IMessagingAdapter messagingAdapter;
        private readonly Address address;

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
            Precondition.NotNull(component, nameof(component));
            Precondition.NotNull(container, nameof(container));
            Precondition.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.messagingAdapter = messagingAdapter;
            this.address = new Address(this.GetType().Name);
        }

        /// <summary>
        /// Sends the given message to the given service via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(Address receiver, T message)
            where T : Message
        {
            Debug.NotNull(message, nameof(message));

            this.messagingAdapter.Send(receiver, message, this.address);
        }

        /// <summary>
        /// Sends the given messages to the given list of system services via the messaging system.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(ReadOnlyList<Address> receivers, T message)
            where T : Message
        {
            Debug.NotNullOrEmpty(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            foreach (var receiver in receivers)
            {
                this.Send(receiver, message);
            }
        }
    }
}
