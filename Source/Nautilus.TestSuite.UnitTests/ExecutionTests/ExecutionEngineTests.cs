//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionEngineTests.cs" company="Nautech Systems Pty Ltd">
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
using System.Linq;
using Nautilus.Core.Message;
using Nautilus.DomainModel.Commands;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Execution.Engine;
using Nautilus.Execution.Interfaces;
using Nautilus.Scheduling;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Mocks;
using Nautilus.TestSuite.TestKit.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class ExecutionEngineTests
    {
        private readonly MockTradingGateway tradingGateway;
        private readonly MockComponent receiver;
        private readonly IExecutionDatabase database;
        private readonly ExecutionEngine engine;

        public ExecutionEngineTests(ITestOutputHelper output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            var scheduler = new HashedWheelTimerScheduler(container);
            var messageBusAdapter = new MockMessageBusProvider(container).Adapter;
            this.tradingGateway = new MockTradingGateway(container);
            this.receiver = new MockComponent(container);
            this.receiver.RegisterHandler<Event>(this.receiver.OnMessage);

            this.database = new InMemoryExecutionDatabase(container);

            this.engine = new ExecutionEngine(
                container,
                scheduler,
                messageBusAdapter,
                this.database,
                this.tradingGateway,
                this.receiver.Endpoint);

            this.tradingGateway.Start().Wait();
            this.receiver.Start().Wait();
            this.engine.Start().Wait();
        }

        [Fact]
        internal void OnAccountInquiryCommand_SendsToGateway()
        {
            // Arrange
            var command = new AccountInquiry(
                TraderId.FromString("TESTER-000"),
                AccountId.FromString("NAUTILUS-000-SIMULATED"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(command).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(3, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Single(this.tradingGateway.CalledMethods);
            Assert.Equal("AccountInquiry", this.tradingGateway.CalledMethods[0]);
        }

        [Fact]
        internal void OnSubmitOrderCommand_WhenCommandValid_OperatesDatabaseAndSendsToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var command = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(command).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(3, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Single(this.tradingGateway.CalledMethods);
            Assert.Single(this.tradingGateway.ReceivedObjects);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal(order, this.tradingGateway.ReceivedObjects[0]);
        }

        [Fact]
        internal void OnSubmitOrderCommand_WhenDuplicateCommand_DoesNotSendToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var command = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(command);
            this.engine.Endpoint.SendAsync(command).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(4, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Single(this.tradingGateway.CalledMethods);
            Assert.Single(this.tradingGateway.ReceivedObjects);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal(order, this.tradingGateway.ReceivedObjects[0]);
        }

        [Fact]
        internal void OnSubmitBracketOrderCommand_WhenCommandValid_OperatesDatabaseAndSendsToGateway()
        {
            // Arrange
            var bracketOrder = StubBracketOrderProvider.Create();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var command = new SubmitBracketOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                bracketOrder,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(command).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(3, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Single(this.tradingGateway.CalledMethods);
            Assert.Single(this.tradingGateway.ReceivedObjects);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal(bracketOrder, this.tradingGateway.ReceivedObjects[0]);
        }

        [Fact]
        internal void OnSubmitBracketOrderCommand_WhenDuplicatedCommand_OperatesDatabaseAndSendsToGateway()
        {
            // Arrange
            var bracketOrder = StubBracketOrderProvider.Create();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var command = new SubmitBracketOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                bracketOrder,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(command);
            this.engine.Endpoint.SendAsync(command).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(4, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Single(this.tradingGateway.CalledMethods);
            Assert.Single(this.tradingGateway.ReceivedObjects);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal(bracketOrder, this.tradingGateway.ReceivedObjects[0]);
        }

        [Fact]
        internal void OnCancelOrderCommand_WhenOrderExists_DoesNotSendCommand()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");
            var positionId = new PositionId("P-123456");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var cancel = new CancelOrder(
                traderId,
                accountId,
                order.Id,
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submit);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(cancel).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(6, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Equal(2, this.tradingGateway.CalledMethods.Count);
            Assert.Equal(2, this.tradingGateway.ReceivedObjects.Count);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal("CancelOrder", this.tradingGateway.CalledMethods[1]);
            Assert.Equal(3, this.receiver.Messages.Count);
        }

        [Fact]
        internal void OnCancelOrderCommand_WhenNoOrderExists_DoesNotSendCommand()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var strategyId = new StrategyId("SCALPER", "001");

            var command = new CancelOrder(
                traderId,
                accountId,
                order.Id,
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(command).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(3, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Empty(this.tradingGateway.CalledMethods);
            Assert.Empty(this.tradingGateway.ReceivedObjects);
        }

        [Fact]
        internal void OnModifyOrderCommand_WhenOrderExists_SendsToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submit);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(modify).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(6, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(2, this.tradingGateway.CalledMethods.Count);
            Assert.Equal(2, this.tradingGateway.ReceivedObjects.Count);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal("ModifyOrder", this.tradingGateway.CalledMethods[1]);
            Assert.Equal(3, this.receiver.Messages.Count);
        }

        [Fact]
        internal void OnModifyOrderCommand_WhenNoOrderExists_DoesNotSendToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(modify).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(3, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(0, this.engine.EventCount);
            Assert.Empty(this.tradingGateway.CalledMethods);
            Assert.Empty(this.tradingGateway.ReceivedObjects);
            Assert.Empty(this.receiver.Messages);
        }

        [Fact]
        internal void OnModifyOrderCommand_WhenOrderNotYetWorking_DoesNotSendToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submit);
            this.engine.Endpoint.SendAsync(modify).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(4, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Single(this.tradingGateway.CalledMethods);
            Assert.Single(this.tradingGateway.ReceivedObjects);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
        }

        [Fact]
        internal void OnModifyOrderCommand_WhenOrderAlreadyBeingModified_DoesNotSendToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify1 = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify2 = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submit);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(modify1);
            this.engine.Endpoint.SendAsync(modify2).Wait();

            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(7, this.engine.ProcessedCount);
            Assert.Equal(3, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(2, this.tradingGateway.CalledMethods.Count);
            Assert.Equal(2, this.tradingGateway.ReceivedObjects.Count);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal("ModifyOrder", this.tradingGateway.CalledMethods[1]);
            Assert.Equal(3, this.receiver.Messages.Count);
        }

        [Fact]
        internal void OnAccountStateEvent_UpdatesAccountSendsToPublisher()
        {
            // Arrange
            var @event = StubEventMessageProvider.AccountStateEvent();

            // Act
            this.engine.Endpoint.SendAsync(@event);
            this.engine.Endpoint.SendAsync(@event);
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(4, this.engine.ProcessedCount);
            Assert.Equal(0, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(2, this.receiver.Messages.Count);
        }

        [Fact]
        internal void OnOrderSubmittedEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(3, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(0, this.engine.EventCount);
            Assert.Single(this.receiver.Messages);
            Assert.Equal(OrderState.Submitted, order.State);
        }

        [Fact]
        internal void OnOrderAcceptedEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(4, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(1, this.engine.EventCount);
            Assert.Equal(2, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Accepted, order.State);
        }

        [Fact]
        internal void OnOrderRejectedEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderRejectedEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(4, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(1, this.engine.EventCount);
            Assert.Equal(2, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Rejected, order.State);
        }

        [Fact]
        internal void OnOrderWorkingEvent_WhenModifyOrderCommandBuffered_SendsCommandToGateway()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submit);
            this.engine.Endpoint.SendAsync(modify);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(6, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(2, this.tradingGateway.CalledMethods.Count);
            Assert.Equal(2, this.tradingGateway.ReceivedObjects.Count);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal("ModifyOrder", this.tradingGateway.CalledMethods[1]);
            Assert.Equal(3, this.receiver.Messages.Count);
        }

        [Fact]
        internal void OnOrderWorkingEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(5, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(3, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Working, order.State);
        }

        [Fact]
        internal void OnOrderModifiedEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submit = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var modify = new ModifyOrder(
                traderId,
                accountId,
                order.Id,
                order.Quantity,
                Price.Create(1.00010m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submit);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(modify);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderModifiedEvent(order, Price.Create(1.00010m, 5))).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(7, this.engine.ProcessedCount);
            Assert.Equal(2, this.engine.CommandCount);
            Assert.Equal(3, this.engine.EventCount);
            Assert.Equal(2, this.tradingGateway.CalledMethods.Count);
            Assert.Equal(2, this.tradingGateway.ReceivedObjects.Count);
            Assert.Equal("SubmitOrder", this.tradingGateway.CalledMethods[0]);
            Assert.Equal("ModifyOrder", this.tradingGateway.CalledMethods[1]);
            Assert.Equal(4, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Working, order.State);
        }

        [Fact]
        internal void OnOrderExpiredEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderExpiredEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(6, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(3, this.engine.EventCount);
            Assert.Equal(4, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Expired, order.State);
        }

        [Fact]
        internal void OnOrderCancelledEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderCancelledEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(6, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(3, this.engine.EventCount);
            Assert.Equal(4, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Cancelled, order.State);
        }

        [Fact]
        internal void OnOrderCancelRejectEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildStopMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderWorkingEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderCancelRejectEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(6, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(3, this.engine.EventCount);
            Assert.Equal(4, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Working, order.State);
        }

        [Fact]
        internal void OnOrderFilledEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderFilledEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(5, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(3, this.receiver.Messages.Count);
            Assert.Equal(OrderState.Filled, order.State);
        }

        [Fact]
        internal void OnOrderPartiallyFilledEvent_UpdatesOrderSendsToPublisher()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderPartiallyFilledEvent(
                order,
                Quantity.Create(50000),
                Quantity.Create(50000))).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Null(this.engine.UnhandledMessages.FirstOrDefault());
            Assert.Equal(5, this.engine.ProcessedCount);
            Assert.Equal(1, this.engine.CommandCount);
            Assert.Equal(2, this.engine.EventCount);
            Assert.Equal(3, this.receiver.Messages.Count);
            Assert.Equal(OrderState.PartiallyFilled, order.State);
        }

        [Fact]
        internal void OnOrderFilledEvent_WithNoPosition_OpensPosition()
        {
            // Arrange
            var order = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.engine.Endpoint.SendAsync(submitOrder);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderSubmittedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderFilledEvent(order)).Wait();
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Single(this.database.GetPositions());
        }

        [Fact]
        internal void OnOrderFilledEvent_WithPosition_UpdatesPosition()
        {
            // Arrange
            var order1 = new StubOrderBuilder().EntryOrder("O-123456-1").BuildMarketOrder();
            var order2 = new StubOrderBuilder().EntryOrder("O-123456-2").BuildMarketOrder();
            var traderId = TraderId.FromString("TESTER-000");
            var accountId = AccountId.FromString("NAUTILUS-000-SIMULATED");
            var positionId = new PositionId("P-123456");
            var strategyId = new StrategyId("SCALPER", "001");

            var submitOrder1 = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var submitOrder2 = new SubmitOrder(
                traderId,
                accountId,
                strategyId,
                positionId,
                order2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.engine.Endpoint.SendAsync(submitOrder1);
            this.engine.Endpoint.SendAsync(submitOrder2);
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderSubmittedEvent(order1));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderSubmittedEvent(order2));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order1));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderAcceptedEvent(order2));
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderFilledEvent(order1)).Wait();

            // Act
            this.engine.Endpoint.SendAsync(StubEventMessageProvider.OrderFilledEvent(order2));
            this.engine.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Single(this.database.GetPositions());
        }
    }
}
