//--------------------------------------------------------------------------------------------------
// <copyright file="DataPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Network
{
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Nodes;

    /// <summary>
    /// Provides a publisher for <see cref="Bar"/> data.
    /// </summary>
    public sealed class DataPublisher : PublisherDataBus
    {
        private readonly IDataSerializer<Bar> barSerializer;
        private readonly IDataSerializer<Instrument> instrumentSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="barSerializer">The bar serializer.</param>
        /// <param name="instrumentSerializer">The instrument serializer.</param>
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The port.</param>
        public DataPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Bar> barSerializer,
            IDataSerializer<Instrument> instrumentSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Port port)
            : base(
                container,
                dataBusAdapter,
                compressor,
                encryption,
                Nautilus.Network.NetworkAddress.LocalHost,
                port)
        {
            this.barSerializer = barSerializer;
            this.instrumentSerializer = instrumentSerializer;

            this.RegisterHandler<BarData>(this.OnMessage);
            this.RegisterHandler<Instrument>(this.OnMessage);

            this.Subscribe<BarData>();
            this.Subscribe<Instrument>();
        }

        private void OnMessage(BarData data)
        {
            var topic = $"{nameof(Bar)}:{data.BarType.ToString()}";
            this.Publish(topic, this.barSerializer.Serialize(data.Bar));
        }

        private void OnMessage(Instrument data)
        {
            var topic = $"{nameof(Instrument)}:{data.Symbol.Value}";
            this.Publish(topic, this.instrumentSerializer.Serialize(data));
        }
    }
}
