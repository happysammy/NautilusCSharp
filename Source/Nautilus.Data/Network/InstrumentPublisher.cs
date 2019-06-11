//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Network
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;

    /// <summary>
    /// Provides a publisher for <see cref="Instrument"/> data.
    /// </summary>
    public sealed class InstrumentPublisher : DataPublisher<Instrument>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="serializer">The instrument serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public InstrumentPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            ISerializer<Instrument> serializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                dataBusAdapter,
                serializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<Instrument>(this.OnMessage);

            this.Subscribe<Instrument>();
        }

        private void OnMessage(Instrument instrument)
        {
            this.Publish(instrument.Symbol.ToString(), instrument);
        }
    }
}
