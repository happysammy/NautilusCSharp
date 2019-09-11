//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageBusProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Message;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockMessageBusProvider
    {
        public MockMessageBusProvider(IComponentryContainer container)
        {
            var messagingAdapter = new MessageBusAdapter(
                new MessageBus<Command>(container).Endpoint,
                new MessageBus<Event>(container).Endpoint,
                new MessageBus<Document>(container).Endpoint);

            this.MessageBusAdapter = messagingAdapter;

            var initializeSwitchboard = new InitializeSwitchboard(
                Switchboard.Empty(),
                Guid.NewGuid(),
                container.Clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);
        }

        public IMessageBusAdapter MessageBusAdapter { get; }
    }
}
