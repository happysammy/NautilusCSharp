//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionEngineTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Interfaces;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ExecutionEngineTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly IMessageBusAdapter messageBusAdapter;
        private readonly MockTradingGateway tradingGateway;
        private readonly IEndpoint receiver;
        private readonly IExecutionDatabase database;
        private readonly ExecutionEngine engine;

        public ExecutionEngineTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            this.logger = containerFactory.LoggingAdapter;
            var container = containerFactory.Create();
            var service = new MockMessageBusFactory(container);
            this.messageBusAdapter = service.MessageBusAdapter;
            this.tradingGateway = new MockTradingGateway();
            this.receiver = new MockMessagingAgent().Endpoint;

            this.database = new InMemoryExecutionDatabase(container);

            this.engine = new ExecutionEngine(
                container,
                this.messageBusAdapter,
                this.database,
                this.tradingGateway,
                this.receiver);
        }
    }
}
