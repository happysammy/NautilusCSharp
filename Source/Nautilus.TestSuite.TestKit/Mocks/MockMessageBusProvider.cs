//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageBusProvider.cs" company="Nautech Systems Pty Ltd">
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
using System.Diagnostics.CodeAnalysis;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.Core.Message;
using Nautilus.Core.Types;

namespace Nautilus.TestSuite.TestKit.Mocks
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockMessageBusProvider
    {
        public MockMessageBusProvider(IComponentryContainer container)
        {
            var adapter = new MessageBusAdapter(
                new MessageBus<Command>(container),
                new MessageBus<Event>(container),
                new MessageBus<Message>(container));

            this.Adapter = adapter;

            var initializeSwitchboard = new InitializeSwitchboard(
                Switchboard.Empty(),
                Guid.NewGuid(),
                container.Clock.TimeNow());

            adapter.Send(initializeSwitchboard);
        }

        public IMessageBusAdapter Adapter { get; }
    }
}
