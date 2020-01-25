//--------------------------------------------------------------------------------------------------
// <copyright file="InMemoryExecutionDatabaseTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
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

            var containerFactory = new StubComponentryContainerProvider();
            this.logger = containerFactory.LoggingAdapter;
            var container = containerFactory.Create();
            this.database = new InMemoryExecutionDatabase(container);
        }

        [Fact]
        internal void GetTraderIdWithOrderId_WhenNoTraderExists_ReturnsNull()
        {
            // Arrange
            var orderId = new OrderId("O-123456");

            // Act
            var result = this.database.GetTraderId(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void GetTraderIdWithPositionId_WhenNoTraderExists_ReturnsNull()
        {
            // Arrange
            var positionId = new PositionId("P-123456");

            // Act
            var result = this.database.GetTraderId(positionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void GetAccountIdWithPositionId_WhenNoAccountExists_ReturnsNull()
        {
            // Arrange
            var positionId = new PositionId("P-123456");

            // Act
            var result = this.database.GetAccountId(positionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void GetPositionId_WhenNoPositionExists_ReturnsNull()
        {
            // Arrange
            var orderId = new OrderId("O-123456");

            // Act
            var result = this.database.GetPositionId(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void GetPositionForOrder_WhenNoPositionExists_ReturnsNull()
        {
            // Arrange
            var orderId = new OrderId("O-123456");

            // Act
            var result = this.database.GetPositionForOrder(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void GetOrder_WhenNoOrderExists_ReturnsNull()
        {
            // Arrange
            var orderId = new OrderId("O-123456");

            // Act
            var result = this.database.GetOrder(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void GetPosition_WhenNoPositionExists_ReturnsNull()
        {
            // Arrange
            var positionId = new PositionId("P-123456");

            // Act
            var result = this.database.GetPosition(positionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        internal void AddOrder_WhenOrderAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            // Act
            var result = this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void AddOrder_WithNoOrdersInDatabase_CorrectlyAddsOrderWithIndexes()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            // Act
            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(order, this.database.GetOrder(order.Id));
            Assert.Equal(positionId, this.database.GetPositionId(order.Id));
            Assert.Equal(traderId, this.database.GetTraderId(order.Id));
            Assert.Single(this.database.GetOrders());
            Assert.Single(this.database.GetOrders(traderId));
            Assert.Single(this.database.GetOrders(traderId, strategyId));
            Assert.Contains(order.Id, this.database.GetOrders());
            Assert.Contains(order.Id, this.database.GetOrders(traderId));
            Assert.Contains(order.Id, this.database.GetOrders(traderId, strategyId));
            Assert.Contains(traderId, this.database.GetTraderIds());
            Assert.Contains(accountId, this.database.GetAccountIds());
            Assert.Contains(strategyId, this.database.GetStrategyIds(traderId));
        }

        [Fact]
        internal void GetOrders_WhenDoesNotExistInCache_ReturnsEmptyDictionary()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            // Act
            this.database.ClearCaches();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Empty(this.database.GetOrders());
            Assert.Empty(this.database.GetOrders(traderId));
            Assert.Empty(this.database.GetOrders(traderId, strategyId));
        }

        [Fact]
        internal void GetOrdersWorking_WhenDoesNotExistInCache_ReturnsEmptyDictionary()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);
            order.Apply(StubEventMessageProvider.OrderSubmittedEvent(order));
            this.database.UpdateOrder(order);

            // Act
            this.database.ClearCaches();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Empty(this.database.GetOrders());
            Assert.Empty(this.database.GetOrders(traderId));
            Assert.Empty(this.database.GetOrders(traderId, strategyId));
            Assert.Empty(this.database.GetOrdersWorking());
            Assert.Empty(this.database.GetOrdersWorking(traderId));
            Assert.Empty(this.database.GetOrdersWorking(traderId, strategyId));
            Assert.Empty(this.database.GetOrdersCompleted());
            Assert.Empty(this.database.GetOrdersCompleted(traderId));
            Assert.Empty(this.database.GetOrdersCompleted(traderId, strategyId));
        }

        [Fact]
        internal void GetOrdersCompleted_WhenDoesNotExistInCache_ReturnsEmptyDictionary()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            order.Apply(StubEventMessageProvider.OrderSubmittedEvent(order));
            this.database.UpdateOrder(order);

            order.Apply(StubEventMessageProvider.OrderRejectedEvent(order));
            this.database.UpdateOrder(order);

            // Act
            this.database.ClearCaches();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Empty(this.database.GetOrders());
            Assert.Empty(this.database.GetOrders(traderId));
            Assert.Empty(this.database.GetOrders(traderId, strategyId));
            Assert.Empty(this.database.GetOrdersWorking());
            Assert.Empty(this.database.GetOrdersWorking(traderId));
            Assert.Empty(this.database.GetOrdersWorking(traderId, strategyId));
            Assert.Empty(this.database.GetOrdersCompleted());
            Assert.Empty(this.database.GetOrdersCompleted(traderId));
            Assert.Empty(this.database.GetOrdersCompleted(traderId, strategyId));
        }

        [Fact]
        internal void ClearCaches_WithOrderInCaches_CorrectlyClearsCache()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            // Act
            this.database.ClearCaches();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Null(this.database.GetOrder(order.Id));
        }

        [Fact]
        internal void Flush_WithOrderInDatabase_FlushesData()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            // Act
            this.database.Flush();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Null(this.database.GetTraderId(order.Id));
            Assert.Empty(this.database.GetOrders(traderId));
            Assert.Empty(this.database.GetOrders(traderId, strategyId));
            Assert.DoesNotContain(order.Id, this.database.GetOrders(traderId));
            Assert.DoesNotContain(order.Id, this.database.GetOrders(traderId, strategyId));
            Assert.DoesNotContain(traderId, this.database.GetTraderIds());
            Assert.DoesNotContain(accountId, this.database.GetAccountIds());
            Assert.DoesNotContain(strategyId, this.database.GetStrategyIds(traderId));
        }

        [Fact]
        internal void AddAccount_WithNoAccountsInDatabase_CorrectlyAddsAccountWithIndexes()
        {
            // Arrange
            var account = StubAccountProvider.Create();

            // Act
            this.database.AddAccount(account);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(account, this.database.GetAccount(account.Id));
            Assert.Equal(account.Id, this.database.GetAccountIds().First());
        }

        [Fact]
        internal void AddAtomicOrder_WhenEntryOrderAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var atomicOrder = StubAtomicOrderProvider.Create();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(atomicOrder.Entry, traderId, accountId, strategyId, positionId);

            // Act
            var result = this.database.AddAtomicOrder(atomicOrder, traderId, accountId, strategyId, positionId);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void AddAtomicOrder_WhenStopLossOrderAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var atomicOrder = StubAtomicOrderProvider.Create();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(atomicOrder.StopLoss, traderId, accountId, strategyId, positionId);

            // Act
            var result = this.database.AddAtomicOrder(atomicOrder, traderId, accountId, strategyId, positionId);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void AddAtomicOrder_WhenTakeProfitOrderAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var atomicOrder = StubAtomicOrderProvider.Create();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            #pragma warning disable 8602
            #pragma warning disable 8604
            this.database.AddOrder(atomicOrder.TakeProfit, traderId, accountId, strategyId, positionId);

            // Act
            var result = this.database.AddAtomicOrder(atomicOrder, traderId, accountId, strategyId, positionId);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void AddAtomicOrder_WithTakeProfit_CorrectlyAddsOrdersWithIndexes()
        {
            // Arrange
            var atomicOrder = StubAtomicOrderProvider.Create();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            // Act
            this.database.AddAtomicOrder(atomicOrder, traderId, accountId, strategyId, positionId);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(atomicOrder.Entry, this.database.GetOrder(atomicOrder.Entry.Id));
            Assert.Equal(atomicOrder.StopLoss, this.database.GetOrder(atomicOrder.StopLoss.Id));
            Assert.Equal(atomicOrder.TakeProfit, this.database.GetOrder(atomicOrder.TakeProfit.Id));
            Assert.Equal(positionId, this.database.GetPositionId(atomicOrder.Entry.Id));
            Assert.Equal(positionId, this.database.GetPositionId(atomicOrder.StopLoss.Id));
            Assert.Equal(positionId, this.database.GetPositionId(atomicOrder.TakeProfit.Id));
            Assert.Equal(traderId, this.database.GetTraderId(atomicOrder.Entry.Id));
            Assert.Equal(traderId, this.database.GetTraderId(atomicOrder.StopLoss.Id));
            Assert.Equal(traderId, this.database.GetTraderId(atomicOrder.TakeProfit.Id));
            Assert.Equal(3, this.database.GetOrders().Count);
            Assert.Equal(3, this.database.GetOrders(traderId).Count);
            Assert.Equal(3, this.database.GetOrders(traderId, strategyId).Count);
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrderIds());
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrders());
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrders(traderId));
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrders(traderId, strategyId));
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrderWorkingIds());
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrderCompletedIds());
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrdersWorking());
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrdersCompleted());
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrderIds());
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrders());
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrders(traderId));
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrders(traderId, strategyId));
            Assert.Contains(atomicOrder.TakeProfit.Id, this.database.GetOrderIds());
            Assert.Contains(atomicOrder.TakeProfit.Id, this.database.GetOrders());
            Assert.Contains(atomicOrder.TakeProfit.Id, this.database.GetOrders(traderId));
            Assert.Contains(atomicOrder.TakeProfit.Id, this.database.GetOrders(traderId, strategyId));
        }

        [Fact]
        internal void AddAtomicOrder_WithNoTakeProfit_CorrectlyAddsOrdersWithIndexes()
        {
            // Arrange
            var atomicOrder = StubAtomicOrderProvider.Create(false);
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            // Act
            this.database.AddAtomicOrder(atomicOrder, traderId, accountId, strategyId, positionId);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(atomicOrder.Entry, this.database.GetOrder(atomicOrder.Entry.Id));
            Assert.Equal(atomicOrder.StopLoss, this.database.GetOrder(atomicOrder.StopLoss.Id));
            Assert.Equal(positionId, this.database.GetPositionId(atomicOrder.Entry.Id));
            Assert.Equal(positionId, this.database.GetPositionId(atomicOrder.StopLoss.Id));
            Assert.Equal(traderId, this.database.GetTraderId(atomicOrder.Entry.Id));
            Assert.Equal(traderId, this.database.GetTraderId(atomicOrder.StopLoss.Id));
            Assert.Equal(2, this.database.GetOrders().Count);
            Assert.Equal(2, this.database.GetOrders(traderId).Count);
            Assert.Equal(2, this.database.GetOrders(traderId, strategyId).Count);
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrderIds());
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrders());
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrders(traderId));
            Assert.Contains(atomicOrder.Entry.Id, this.database.GetOrders(traderId, strategyId));
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrderWorkingIds());
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrderCompletedIds());
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrdersWorking());
            Assert.DoesNotContain(atomicOrder.Entry.Id, this.database.GetOrdersCompleted());
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrderIds());
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrders());
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrders(traderId));
            Assert.Contains(atomicOrder.StopLoss.Id, this.database.GetOrders(traderId, strategyId));
        }

        [Fact]
        internal void GetPositions_WhenNotPositionsInCache_ReturnsEmptyDictionary()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderPartiallyFilledEvent(order, 50000, 50000));
            this.database.AddPosition(position);

            position.Apply(StubEventMessageProvider.OrderFilledEvent(order));
            this.database.UpdatePosition(position);

            // Act
            this.database.ClearCaches();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Empty(this.database.GetPositions());
            Assert.Empty(this.database.GetPositions(traderId));
            Assert.Empty(this.database.GetPositions(traderId, strategyId));
        }

        [Fact]
        internal void GetPositionsOpen_WhenNotPositionsInCache_ReturnsEmptyDictionary()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderPartiallyFilledEvent(order, 50000, 50000));
            this.database.AddPosition(position);

            position.Apply(StubEventMessageProvider.OrderFilledEvent(order));
            this.database.UpdatePosition(position);

            // Act
            this.database.ClearCaches();

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Empty(this.database.GetPositionsOpen());
            Assert.Empty(this.database.GetPositionsOpen(traderId));
            Assert.Empty(this.database.GetPositionsOpen(traderId, strategyId));
        }

        [Fact]
        internal void GetPositionsClosed_WhenNotPositionsInCache_ReturnsEmptyDictionary()
        {
            // Arrange
            var order1 = new StubOrderBuilder()
                .WithOrderId("O-123456-1")
                .BuildMarketOrder();

            var order2 = new StubOrderBuilder()
                .WithOrderId("O-123456-2")
                .WithOrderSide(OrderSide.SELL)
                .BuildMarketOrder();

            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order1, traderId, accountId, strategyId, positionId);
            this.database.AddOrder(order2, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderFilledEvent(order1));
            this.database.AddPosition(position);

            position.Apply(StubEventMessageProvider.OrderFilledEvent(order2));
            this.database.UpdatePosition(position);

            // Act
            this.database.ClearCaches();

            // Assert
            Assert.Empty(this.database.GetPositionsClosed());
            Assert.Empty(this.database.GetPositionsClosed(traderId));
            Assert.Empty(this.database.GetPositionsClosed(traderId, strategyId));
        }

        [Fact]
        internal void AddPosition_WhenPositionAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderFilledEvent(order));

            this.database.AddPosition(position);

            // Act
            var result = this.database.AddPosition(position);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void AddPosition_WithNoPositionsInDatabase_CorrectlyAddsPositionWithIndexes()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderFilledEvent(order));

            // Act
            this.database.AddPosition(position);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(position, this.database.GetPosition(positionId));
            Assert.Contains(position.Id, this.database.GetPositions());
            Assert.Contains(position.Id, this.database.GetPositions(traderId));
            Assert.Contains(position.Id, this.database.GetPositions(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionsOpen());
            Assert.Contains(position.Id, this.database.GetPositionsOpen(traderId));
            Assert.Contains(position.Id, this.database.GetPositionsOpen(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionIds());
            Assert.Contains(position.Id, this.database.GetPositionIds(traderId));
            Assert.Contains(position.Id, this.database.GetPositionIds(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionOpenIds());
            Assert.Contains(position.Id, this.database.GetPositionOpenIds(traderId));
            Assert.Contains(position.Id, this.database.GetPositionOpenIds(traderId, strategyId));
            Assert.DoesNotContain(position.Id, this.database.GetPositionClosedIds());
        }

        [Fact]
        internal void UpdateAccount_WhenAccountDoesNotYetExist_CorrectlyUpdatesAccount()
        {
            // Arrange
            var account = StubAccountProvider.Create();
            this.database.UpdateAccount(account);

            // Act
            this.database.UpdateAccount(account);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.True(true); // Does not throw
        }

        [Fact]
        internal void UpdateAccount_WhenAccountExists_CorrectlyUpdatesAccount()
        {
            // Arrange
            var account = StubAccountProvider.Create();
            this.database.UpdateAccount(account);

            var message = new AccountStateEvent(
                new AccountId("FXCM", "123456789", "SIMULATED"),
                Currency.AUD,
                Money.Create(150000m, Currency.AUD),
                Money.Create(150000m, Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            account.Apply(message);

            // Act
            this.database.UpdateAccount(account);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.True(true); // Does not throw
        }

        [Fact]
        internal void UpdateOrder_WhenOrderWorking_CorrectlyUpdatesIndexes()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            order.Apply(StubEventMessageProvider.OrderSubmittedEvent(order));
            this.database.UpdateOrder(order);

            order.Apply(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.database.UpdateOrder(order);

            order.Apply(StubEventMessageProvider.OrderWorkingEvent(order));

            // Act
            this.database.UpdateOrder(order);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Contains(order.Id, this.database.GetOrderIds());
            Assert.Contains(order.Id, this.database.GetOrderIds(traderId));
            Assert.Contains(order.Id, this.database.GetOrderIds(traderId, strategyId));
            Assert.Contains(order.Id, this.database.GetOrderWorkingIds());
            Assert.Contains(order.Id, this.database.GetOrderWorkingIds(traderId));
            Assert.Contains(order.Id, this.database.GetOrderWorkingIds(traderId, strategyId));
            Assert.DoesNotContain(order.Id, this.database.GetOrderCompletedIds());
            Assert.Contains(order.Id, this.database.GetOrders(traderId));
            Assert.Contains(order.Id, this.database.GetOrders(traderId, strategyId));
            Assert.Contains(order.Id, this.database.GetOrdersWorking(traderId));
            Assert.Contains(order.Id, this.database.GetOrdersWorking(traderId, strategyId));
        }

        [Fact]
        internal void UpdateOrder_WhenOrderCompleted_CorrectlyUpdatesIndexes()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            order.Apply(StubEventMessageProvider.OrderSubmittedEvent(order));
            this.database.UpdateOrder(order);

            order.Apply(StubEventMessageProvider.OrderRejectedEvent(order));

            // Act
            this.database.UpdateOrder(order);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Contains(order.Id, this.database.GetOrderIds());
            Assert.Contains(order.Id, this.database.GetOrderIds(traderId));
            Assert.Contains(order.Id, this.database.GetOrderIds(traderId, strategyId));
            Assert.Contains(order.Id, this.database.GetOrderCompletedIds());
            Assert.Contains(order.Id, this.database.GetOrderCompletedIds(traderId));
            Assert.Contains(order.Id, this.database.GetOrderCompletedIds(traderId, strategyId));
            Assert.DoesNotContain(order.Id, this.database.GetOrderWorkingIds());
            Assert.Contains(order.Id, this.database.GetOrders(traderId));
            Assert.Contains(order.Id, this.database.GetOrders(traderId, strategyId));
            Assert.Contains(order.Id, this.database.GetOrdersCompleted(traderId));
            Assert.Contains(order.Id, this.database.GetOrdersCompleted(traderId, strategyId));
        }

        [Fact]
        internal void UpdatePosition_WhenPositionOpen_CorrectlyUpdatesIndexes()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderPartiallyFilledEvent(order, 50000, 50000));
            this.database.AddPosition(position);

            // Act
            position.Apply(StubEventMessageProvider.OrderFilledEvent(order));
            this.database.UpdatePosition(position);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(position, this.database.GetPosition(positionId));
            Assert.Contains(position.Id, this.database.GetPositions());
            Assert.Contains(position.Id, this.database.GetPositions(traderId));
            Assert.Contains(position.Id, this.database.GetPositions(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionsOpen());
            Assert.Contains(position.Id, this.database.GetPositionsOpen(traderId));
            Assert.Contains(position.Id, this.database.GetPositionsOpen(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionIds());
            Assert.Contains(position.Id, this.database.GetPositionOpenIds());
            Assert.DoesNotContain(position.Id, this.database.GetPositionClosedIds());
        }

        [Fact]
        internal void UpdatePosition_WhenPositionClosed_CorrectlyUpdatesIndexes()
        {
            // Arrange
            var order1 = new StubOrderBuilder()
                .WithOrderId("O-123456-1")
                .BuildMarketOrder();

            var order2 = new StubOrderBuilder()
                .WithOrderId("O-123456-2")
                .WithOrderSide(OrderSide.SELL)
                .BuildMarketOrder();

            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order1, traderId, accountId, strategyId, positionId);
            this.database.AddOrder(order2, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderFilledEvent(order1));
            this.database.AddPosition(position);

            // Act
            position.Apply(StubEventMessageProvider.OrderFilledEvent(order2));
            this.database.UpdatePosition(position);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(position, this.database.GetPosition(positionId));
            Assert.Contains(position.Id, this.database.GetPositions());
            Assert.Contains(position.Id, this.database.GetPositions(traderId));
            Assert.Contains(position.Id, this.database.GetPositions(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionsClosed());
            Assert.Contains(position.Id, this.database.GetPositionsClosed(traderId));
            Assert.Contains(position.Id, this.database.GetPositionsClosed(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionIds());
            Assert.Contains(position.Id, this.database.GetPositionClosedIds());
            Assert.DoesNotContain(position.Id, this.database.GetPositionOpenIds());
        }

        [Fact]
        internal void UpdatePosition_WhenMultipleOrdersForPositionLeavingPositionOpen_CorrectlyUpdatesIndexes()
        {
            // Arrange
            var order1 = new StubOrderBuilder()
                .WithOrderId("O-123456-1")
                .BuildMarketOrder();

            var order2 = new StubOrderBuilder()
                .WithOrderId("O-123456-2")
                .BuildMarketOrder();

            var order3 = new StubOrderBuilder()
                .WithOrderId("O-123456-3")
                .BuildMarketOrder();

            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            this.database.AddOrder(order1, traderId, accountId, strategyId, positionId);
            this.database.AddOrder(order2, traderId, accountId, strategyId, positionId);
            this.database.AddOrder(order3, traderId, accountId, strategyId, positionId);

            var position = new Position(positionId, StubEventMessageProvider.OrderFilledEvent(order1));
            this.database.AddPosition(position);

            position.Apply(StubEventMessageProvider.OrderFilledEvent(order2));
            this.database.UpdatePosition(position);

            // Act
            position.Apply(StubEventMessageProvider.OrderFilledEvent(order3));
            this.database.UpdatePosition(position);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(position, this.database.GetPosition(positionId));
            Assert.Contains(position.Id, this.database.GetPositionsOpen());
            Assert.Contains(position.Id, this.database.GetPositionsOpen(traderId));
            Assert.Contains(position.Id, this.database.GetPositionsOpen(traderId, strategyId));
            Assert.Contains(position.Id, this.database.GetPositionIds());
            Assert.Contains(position.Id, this.database.GetPositionOpenIds());
            Assert.DoesNotContain(position.Id, this.database.GetPositionClosedIds());
        }
    }
}
