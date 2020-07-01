//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceAddress.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Messaging;

namespace Nautilus.Data
{
    /// <summary>
    /// Provides data service component messaging addresses.
    /// </summary>
    public static class ServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="DataService"/> component messaging address.
        /// </summary>
        public static Address DataService { get; } = new Address(nameof(DataService));

        /// <summary>
        /// Gets the <see cref="Scheduler"/> component messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="DataGateway"/> component messaging address.
        /// </summary>
        public static Address DataGateway { get; } = new Address(nameof(DataGateway));

        /// <summary>
        /// Gets the <see cref="DataServer"/> component messaging address.
        /// </summary>
        public static Address DataServer { get; } = new Address(nameof(DataServer));

        /// <summary>
        /// Gets the <see cref="DataPublisher"/> component messaging address.
        /// </summary>
        public static Address DataPublisher { get; } = new Address(nameof(DataPublisher));

        /// <summary>
        /// Gets the <see cref="TickPublisher"/> component messaging address.
        /// </summary>
        public static Address TickPublisher { get; } = new Address(nameof(TickPublisher));

        /// <summary>
        /// Gets the <see cref="TickProvider"/> component messaging address.
        /// </summary>
        public static Address TickProvider { get; } = new Address(nameof(TickProvider));

        /// <summary>
        /// Gets the <see cref="TickRepository"/> component messaging address.
        /// </summary>
        public static Address TickRepository { get; } = new Address(nameof(TickRepository));

        /// <summary>
        /// Gets the <see cref="BarAggregationController"/> component messaging address.
        /// </summary>
        public static Address BarAggregationController { get; } = new Address(nameof(BarAggregationController));

        /// <summary>
        /// Gets the <see cref="BarProvider"/> component messaging address.
        /// </summary>
        public static Address BarProvider { get; } = new Address(nameof(BarProvider));

        /// <summary>
        /// Gets the <see cref="BarRepository"/> component messaging address.
        /// </summary>
        public static Address BarRepository { get; } = new Address(nameof(BarRepository));

        /// <summary>
        /// Gets the <see cref="InstrumentProvider"/> component messaging address.
        /// </summary>
        public static Address InstrumentProvider { get; } = new Address(nameof(InstrumentProvider));

        /// <summary>
        /// Gets the <see cref="InstrumentRepository"/> component messaging address.
        /// </summary>
        public static Address InstrumentRepository { get; } = new Address(nameof(InstrumentRepository));
    }
}
