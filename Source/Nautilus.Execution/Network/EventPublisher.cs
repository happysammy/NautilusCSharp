// -------------------------------------------------------------------------------------------------
// <copyright file="EventPublisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Events;
    using Nautilus.Network;

    /// <summary>
    /// Provides an event publisher for the messaging server.
    /// </summary>
    public sealed class EventPublisher : MessagePublisher<Event>
    {
        private const string EVENTS = "EVENTS";
        private const string TRADE = "TRADE";
        private const string ACCOUNT = "ACCOUNT";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The event serializer.</param>
        /// <param name="port">The publishers port.</param>
        public EventPublisher(
            IComponentryContainer container,
            ISerializer<Event> serializer,
            NetworkPort port)
            : base(
                container,
                serializer,
                NetworkHost.LocalHost,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<TradeEvent>(this.OnEvent);
            this.RegisterHandler<AccountStateEvent>(this.OnEvent);
        }

        private void OnEvent(TradeEvent @event)
        {
            this.Publish($"{EVENTS}:{TRADE}:{@event.TraderId.Value}", @event.Event);
        }

        private void OnEvent(AccountStateEvent @event)
        {
            this.Publish($"{EVENTS}:{ACCOUNT}:{@event.AccountId.Value}", @event);
        }
    }
}
