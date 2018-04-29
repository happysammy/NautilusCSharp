//--------------------------------------------------------------------------------------------------
// <copyright file="ActorComponentBase.cs" company="Nautech Systems Pty Ltd">
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

    public class ActorComponentBusConnectedBase : ActorComponentBase
    {
        public ActorComponentBusConnectedBase(
            Enum service,
            Label component,
            ComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
                service,
                component,
                container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.MessagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Gets the components messaging adapter.
        /// </summary>
        protected IMessagingAdapter MessagingAdapter { get; }

        /// <summary>
        /// Sends the given message to the given black box service via the messaging system.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the message is null.</exception>
        protected void SendMessage(
            Enum receiver,
            Message message)
        {
            Validate.NotNull(message, nameof(message));

            this.MessagingAdapter.Send(receiver, message, this.Service);
        }

        /// <summary>
        /// Sends the given messages to the given list of black box services via the messaging system.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected void SendMessage(
            IReadOnlyCollection<Enum> receivers,
            Message message)
        {
            Debug.NotNull(receivers, nameof(receivers));
            Debug.NotNull(message, nameof(message));

            this.MessagingAdapter.Send(receivers, message, this.Service);
        }
    }
}
