//--------------------------------------------------------------------------------------------------
// <copyright file="MessageBusConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Nautilus.Common.Componentry;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;
using Nautilus.Core.Types;
using Nautilus.Messaging;
using Nautilus.Messaging.Interfaces;

namespace Nautilus.Common.Messaging
{
    /// <summary>
    /// The base class for all components which are connected to the message bus.
    /// </summary>
    public abstract class MessageBusConnected : MessagingComponent
    {
        private readonly IMessageBusAdapter messageBusAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusConnected"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        protected MessageBusConnected(IComponentryContainer container, IMessageBusAdapter messageBusAdapter)
            : base(container)
        {
            this.messageBusAdapter = messageBusAdapter;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Subscribe to the given message type with the message bus.
        /// </summary>
        /// <typeparam name="T">The message type to subscribe to.</typeparam>
        protected void Subscribe<T>()
            where T : Message
        {
            this.messageBusAdapter.Subscribe<T>(
                this.Mailbox,
                this.NewGuid(),
                this.TimeNow());
        }

        /// <summary>
        /// Unsubscribe from the given message type with the message bus.
        /// </summary>
        /// <typeparam name="T">The message type to unsubscribe from.</typeparam>
        protected void Unsubscribe<T>()
            where T : Message
        {
            this.messageBusAdapter.Unsubscribe<T>(
                this.Mailbox,
                this.NewGuid(),
                this.TimeNow());
        }

        /// <summary>
        /// Sends the given message to the given receiver via the message bus.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="receiver">The message receiver.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(T message, Address receiver)
            where T : Message
        {
            this.messageBusAdapter.Send(
                message,
                receiver,
                this.Mailbox.Address,
                this.TimeNow());
        }

        /// <summary>
        /// Sends the given message to all given receivers via the message bus.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="receivers">The message receivers.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(T message, List<Address> receivers)
            where T : Message
        {
            Debug.NotEmpty(receivers, nameof(receivers));

            for (var i = 0; i < receivers.Count; i++)
            {
                this.messageBusAdapter.Send(
                    message,
                    receivers[i],
                    this.Mailbox.Address,
                    this.TimeNow());
            }
        }

        /// <summary>
        /// Sends the given message to the message bus to be published.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message.</param>
        protected void SendToBus<T>(T message)
            where T : Message
        {
            this.messageBusAdapter.SendToBus(
                message,
                this.Mailbox.Address,
                this.TimeNow());
        }
    }
}
