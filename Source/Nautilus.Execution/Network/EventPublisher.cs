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
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.Network;

    /// <summary>
    /// Provides an event publisher for the messaging server.
    /// </summary>
    public sealed class EventPublisher : MessagePublisher<Event>
    {
        private const string TRADE = "TRADE";
        private const string ACCOUNT = "ACCOUNT";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="serializer">The event serializer.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        public EventPublisher(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            ISerializer<Event> serializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                messageBusAdapter,
                serializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<Event>(this.OnEvent);

            this.Subscribe<Event>();
        }

        private void OnEvent(Event @event)
        {
            switch (@event)
            {
                case TradeEvent tradeEvent:
                    this.Publish($"{TRADE}:{tradeEvent.TraderId.Value}", tradeEvent.Event);
                    break;
                case AccountEvent accountEvent:
                    this.Publish($"{ACCOUNT}:{accountEvent.AccountId.Value}", accountEvent);
                    break;
                default:
                    this.Log.Verbose($"Filtering event {@event} (not published).");
                    break;
            }
        }
    }
}
