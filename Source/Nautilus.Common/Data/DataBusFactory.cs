// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a factory to create the systems data bus.
    /// </summary>
    public static class DataBusFactory
    {
        /// <summary>
        /// Creates and returns a new message bus adapter.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <returns>The messaging adapter.</returns>
        public static DataBusAdapter Create(IComponentryContainer container)
        {
            // TODO: Make more generic
            var tickBus = new DataBus<Tick>(container);
            var barBus = new DataBus<BarData>(container);
            var instrumentBus = new DataBus<Instrument>(container);

            var endpoints = new Dictionary<Type, IEndpoint>
            {
                { tickBus.BusType, tickBus.Endpoint },
                { barBus.BusType, barBus.Endpoint },
                { instrumentBus.BusType, instrumentBus.Endpoint },
            };

            return new DataBusAdapter(
                endpoints,
                tickBus.Endpoint,
                barBus.Endpoint,
                instrumentBus.Endpoint);
        }
    }
}
