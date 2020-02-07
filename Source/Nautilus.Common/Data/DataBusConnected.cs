//--------------------------------------------------------------------------------------------------
// <copyright file="DataBusConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Data
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// The base class for all components which are connected to the message bus.
    /// </summary>
    public abstract class DataBusConnected : Component
    {
        private readonly IDataBusAdapter dataBusAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusConnected"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        protected DataBusConnected(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter)
            : base(container)
        {
            this.dataBusAdapter = dataBusAdapter;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Subscribes to the data type with the data bus.
        /// </summary>
        /// <typeparam name="T">The data type to subscribe to.</typeparam>
        protected void Subscribe<T>()
        {
            this.dataBusAdapter.Subscribe<T>(
                this.Mailbox,
                this.NewGuid(),
                this.TimeNow());
        }

        /// <summary>
        /// Unsubscribe from the data type with the data bus.
        /// </summary>
        /// <typeparam name="T">The data type to unsubscribe from.</typeparam>
        protected void Unsubscribe<T>()
        {
            this.dataBusAdapter.Subscribe<T>(
                this.Mailbox,
                this.NewGuid(),
                this.TimeNow());
        }

        /// <summary>
        /// Send the given tick to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        protected void SendToBus(Tick data)
        {
            this.dataBusAdapter.SendToBus(data);
        }

        /// <summary>
        /// Send the given bar data to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        protected void SendToBus(BarData data)
        {
            this.dataBusAdapter.SendToBus(data);
        }

        /// <summary>
        /// Send the given instrument to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        protected void SendToBus(Instrument data)
        {
            this.dataBusAdapter.SendToBus(data);
        }
    }
}
