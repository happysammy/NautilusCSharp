//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionEngineTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Interfaces;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
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

        [Fact]
        internal void OnSubmitOrderCommand_OperatesDatabaseAndSendsToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var command = new SubmitOrder(
                traderId,
                strategyId,
                accountId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.Send(command);

            Task.Delay(300);
            LogDumper.Dump(this.logger, this.output);

            // Assert
            // Assert.Equal("", this.tradingGateway.CalledMethods[0]);
        }

        [Fact]
        internal void OnOrderSubmitted_OperatesDatabaseAndSendsToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitted = StubEventMessages.OrderSubmittedEvent(order);

            // Act
            this.engine.Endpoint.Send(submitted);

            Task.Delay(300);
            LogDumper.Dump(this.logger, this.output);

            // Assert
            // Assert.Equal("", this.tradingGateway.CalledMethods[0]);
        }
    }
}
