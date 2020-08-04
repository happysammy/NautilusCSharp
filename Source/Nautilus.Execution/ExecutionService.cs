//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd">
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
using System.Collections.Generic;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.Messaging;
using Nautilus.Scheduling;
using Nautilus.Service;

namespace Nautilus.Execution
{
    /// <summary>
    /// Provides an execution service.
    /// </summary>
    public sealed class ExecutionService : NautilusServiceBase
    {
        private readonly ITradingGateway tradingGateway;
        private readonly List<Address> managedComponents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="tradingGateway">The execution gateway.</param>
        /// <param name="config">The execution service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public ExecutionService(
            IComponentryContainer container,
            MessageBusAdapter messageBusAdapter,
            Scheduler scheduler,
            ITradingGateway tradingGateway,
            ServiceConfiguration config)
            : base(
                container,
                messageBusAdapter,
                scheduler,
                config.FixConfig)
        {
            this.tradingGateway = tradingGateway;

            this.managedComponents = new List<Address>
            {
                ComponentAddress.CommandServer,
                ComponentAddress.EventPublisher,
            };

            this.RegisterConnectionAddress(ComponentAddress.TradingGateway);
        }

        /// <inheritdoc />
        protected override void OnServiceStart(Start start)
        {
            // Forward start message
            this.Send(start, this.managedComponents);
        }

        /// <inheritdoc />
        protected override void OnServiceStop(Stop stop)
        {
            // Forward stop message
            this.Send(stop, this.managedComponents);
        }

        /// <inheritdoc />
        protected override void OnConnected()
        {
            this.tradingGateway.AccountInquiry();
            this.tradingGateway.SubscribeToPositionEvents();
        }
    }
}
