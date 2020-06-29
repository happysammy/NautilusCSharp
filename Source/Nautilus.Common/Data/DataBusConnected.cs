//--------------------------------------------------------------------------------------------------
// <copyright file="DataBusConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
    public abstract class DataBusConnected : MessagingComponent
    {
        private readonly IDataBusAdapter dataBusAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusConnected"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        protected DataBusConnected(IComponentryContainer container, IDataBusAdapter dataBusAdapter)
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
