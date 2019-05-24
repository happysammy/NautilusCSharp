//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBusConnectedBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging;

    /// <summary>
    /// The base class for all components which are connected to the message bus.
    /// </summary>
    public abstract class ComponentBusConnectedBase : ComponentBase
    {
        private readonly IMessagingAdapter messagingAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBusConnectedBase"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        protected ComponentBusConnectedBase(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(container)
        {
            this.messagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Sends the given object to the given endpoint via the message bus.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(Address receiver, T message)
            where T : Message
        {
            this.messagingAdapter.Send(receiver, message, this.Address);
        }
    }
}
