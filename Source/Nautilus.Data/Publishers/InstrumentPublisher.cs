//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Publishers
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;
    using Nautilus.Network.Configuration;

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
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The port.</param>
        public InstrumentPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Instrument> serializer,
            ICompressor compressor,
            EncryptionConfiguration encryption,
            NetworkPort port)
            : base(
                container,
                dataBusAdapter,
                serializer,
                compressor,
                encryption,
                Network.NetworkAddress.LocalHost,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<Instrument>(this.OnMessage);

            this.Subscribe<Instrument>();
        }

        private void OnMessage(Instrument instrument)
        {
            this.Publish(instrument.Symbol.Value, instrument);
        }
    }
}
