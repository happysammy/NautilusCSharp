// -------------------------------------------------------------------------------------------------
// <copyright file="EventPublisher.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using Nautilus.Common.Interfaces;
using Nautilus.Core.Message;
using Nautilus.DomainModel.Events;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.Network.Nodes;

namespace Nautilus.Execution.Network
{
    /// <summary>
    /// Provides an event publisher for the messaging server.
    /// </summary>
    public sealed class EventPublisher : MessagePublisher<Event>
    {
        private const string Trade = nameof(Trade);
        private const string Account = nameof(Account);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The event serializer.</param>
        /// <param name="compressor">The event compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The publishers port.</param>
        public EventPublisher(
            IComponentryContainer container,
            ISerializer<Event> serializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Port port)
            : base(
                container,
                serializer,
                compressor,
                encryption,
                Nautilus.Network.NetworkAddress.LocalHost,
                port)
        {
            this.RegisterHandler<TradeEvent>(this.OnEvent);
            this.RegisterHandler<AccountStateEvent>(this.OnEvent);
        }

        private void OnEvent(TradeEvent @event)
        {
            this.Publish($"{nameof(Event)}:{Trade}:{@event.TraderId.Value}", @event.Event);
        }

        private void OnEvent(AccountStateEvent @event)
        {
            this.Publish($"{nameof(Event)}:{Account}:{@event.AccountId.Value}", @event);
        }
    }
}
