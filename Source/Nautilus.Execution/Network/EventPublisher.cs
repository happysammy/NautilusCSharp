// -------------------------------------------------------------------------------------------------
// <copyright file="EventPublisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Events;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Nodes;

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
