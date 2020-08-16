//--------------------------------------------------------------------------------------------------
// <copyright file="IDataBusAdapter.cs" company="Nautech Systems Pty Ltd">
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

using System;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Messaging;
using NodaTime;

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// Provides a means for components to subscribe to and unsubscribe from various data types.
    /// </summary>
    public interface IDataBusAdapter
    {
        /// <summary>
        /// Subscribe the given endpoint to data of type T.
        /// </summary>
        /// <typeparam name="T">The data type to subscribe to.</typeparam>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="timestamp">The subscription timestamp.</param>
        void Subscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp);

        /// <summary>
        /// Unsubscribe the given endpoint from data of type T.
        /// </summary>
        /// <typeparam name="T">The data type to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="timestamp">The subscription timestamp.</param>
        void Unsubscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp);

        /// <summary>
        /// Send the given quote tick to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        void SendToBus(QuoteTick data);

        /// <summary>
        /// Send the given trade tick to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        void SendToBus(TradeTick data);

        /// <summary>
        /// Send the given bar data to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        void SendToBus(BarData data);

        /// <summary>
        /// Send the given instrument to the data bus.
        /// </summary>
        /// <param name="data">The data to send.</param>
        void SendToBus(Instrument data);
    }
}
