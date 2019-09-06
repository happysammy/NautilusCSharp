//--------------------------------------------------------------------------------------------------
// <copyright file="InMemoryExecutionDatabaseTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Interfaces;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class InMemoryExecutionDatabaseTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly IExecutionDatabase database;

        public InMemoryExecutionDatabaseTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            this.logger = containerFactory.LoggingAdapter;
            var container = containerFactory.Create();
            this.database = new InMemoryExecutionDatabase(container);
        }

        [Fact]
        internal void AddOrder_WithNoOrdersInDatabase_CorrectlyAddsOrder()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            // Act
            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            // Assert
            LogDumper.Dump(this.logger, this.output);


        }
    }
}
