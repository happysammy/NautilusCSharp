﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TickPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Publishers
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;

    /// <summary>
    /// Provides a publisher for <see cref="Tick"/> data.
    /// </summary>
    public sealed class TickPublisher : DataPublisher<Tick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TickPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="serializer">The tick serializer.</param>
        /// <param name="port">The port.</param>
        public TickPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Tick> serializer,
            NetworkPort port)
            : base(
                container,
                dataBusAdapter,
                serializer,
                NetworkHost.LocalHost,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<Tick>(this.OnMessage);

            this.Subscribe<Tick>();
        }

        private void OnMessage(Tick tick)
        {
            this.Publish(tick.Symbol.Value, tick);

            // Temporary logging for debug purposes
            // this.Log.Information($"{tick.Symbol.Value} {tick}");
        }
    }
}
