//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockMessagingServiceFactory
    {
        public MockMessagingServiceFactory(IComponentryContainer container)
        {
            var commandBus = new MessageBus<Command>(container);
            var eventBus = new MessageBus<Event>(container);
            var documentBus = new MessageBus<Document>(container);

            var messagingAdapter = new MessagingAdapter(
                commandBus.Endpoint,
                eventBus.Endpoint,
                documentBus.Endpoint);

            var receiver = new MockMessagingAgent();

            var initializeSwitchboard = new InitializeSwitchboard(
                Switchboard.Empty(),
                Guid.NewGuid(),
                container.Clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);

            this.MessagingAdapter = messagingAdapter;
        }

        public IMessagingAdapter MessagingAdapter { get; }
    }
}
