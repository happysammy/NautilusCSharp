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

namespace Nautilus.Execution
{
    /// <summary>
    /// Provides execution service component messaging addresses.
    /// </summary>
    public static class ComponentAddress
    {
        /// <summary>
        /// Gets the <see cref="ExecutionService"/> component messaging address.
        /// </summary>
        public static Address ExecutionService { get; } = new Address(nameof(ExecutionService));

        /// <summary>
        /// Gets the <see cref="Scheduler"/> component messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="TradingGateway"/> component messaging address.
        /// </summary>
        public static Address TradingGateway { get; } = new Address(nameof(TradingGateway));

        /// <summary>
        /// Gets the <see cref="CommandServer"/> component messaging address.
        /// </summary>
        public static Address CommandServer { get; } = new Address(nameof(CommandServer));

        /// <summary>
        /// Gets the <see cref="EventPublisher"/> component messaging address.
        /// </summary>
        public static Address EventPublisher { get; } = new Address(nameof(EventPublisher));

        /// <summary>
        /// Gets the <see cref="ExecutionEngine"/> component messaging address.
        /// </summary>
        public static Address ExecutionEngine { get; } = new Address(nameof(ExecutionEngine));

        /// <summary>
        /// Gets the <see cref="ExecutionEngine"/> component messaging address.
        /// </summary>
        public static Address ExecutionDatabase { get; } = new Address(nameof(ExecutionDatabase));
    }
}
