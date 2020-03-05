//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageBusProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;

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
