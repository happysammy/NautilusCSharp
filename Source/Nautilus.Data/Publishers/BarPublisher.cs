//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Network.Encryption;

    /// <summary>
    /// Provides a publisher for <see cref="Bar"/> data.
    /// </summary>
    public sealed class BarPublisher : DataPublisher<Bar>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="serializer">The bar serializer.</param>
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The port.</param>
        public BarPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Bar> serializer,
            ICompressor compressor,
            EncryptionSettings encryption,
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
            this.RegisterHandler<BarData>(this.OnMessage);

            this.Subscribe<BarData>();
        }

        private void OnMessage(BarData data)
        {
            this.Publish(data.BarType.ToString(), data.Bar);
        }
    }
}
