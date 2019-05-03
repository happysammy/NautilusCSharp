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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks.Dataflow;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using NautilusMQ;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockMessagingServiceFactory
    {
        public IMessagingAdapter MessagingAdapter { get; private set; }

        public InMemoryMessageStore InMemoryMessageStore { get; private set; }

        public void Create(IComponentryContainer container)
        {
            var messageWarehouse = new InMemoryMessageStore();
            var messageStorer = new MessageStorer(container, messageWarehouse);
            var commandBus = new MessageBus<Command>(container, messageStorer.Endpoint);
            var eventBus = new MessageBus<Event>(container, messageStorer.Endpoint);
            var documentBus = new MessageBus<Document>(container, messageStorer.Endpoint);

            var messagingAdapter = new MessagingAdapter(
                commandBus.Endpoint,
                eventBus.Endpoint,
                documentBus.Endpoint);

            var mockEndpoint = new Endpoint(new ActionBlock<object>(n => n.ToString()));
            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Alpha, mockEndpoint },
                { ServiceAddress.Data, mockEndpoint },
                { ServiceAddress.Portfolio, mockEndpoint },
                { ServiceAddress.Risk, mockEndpoint },
                { ServiceAddress.Execution, mockEndpoint },
            };

            var switchboard = Switchboard.Create(addresses);
            var initializeSwitchboard = new InitializeSwitchboard(
                switchboard,
                Guid.NewGuid(),
                container.Clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);

            this.MessagingAdapter = messagingAdapter;
            this.InMemoryMessageStore = messageWarehouse;
        }
    }
}
