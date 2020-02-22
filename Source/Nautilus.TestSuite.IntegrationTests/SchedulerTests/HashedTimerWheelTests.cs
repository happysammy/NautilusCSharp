//--------------------------------------------------------------------------------------------------
// <copyright file="HashedTimerWheelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.SchedulerTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Scheduling;
    using Nautilus.Scheduling.Internal;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Mocks;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class HashedTimerWheelTests
    {
        private readonly MockComponent testReceiver;
        private readonly HashedWheelTimerScheduler scheduler;

        private int testActionCount;

        public HashedTimerWheelTests(ITestOutputHelper output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            this.testReceiver = new MockComponent(container);
            this.testReceiver.RegisterHandler<string>(this.testReceiver.OnMessage);
            this.scheduler = new HashedWheelTimerScheduler(container);
        }

        [Fact]
        internal void MonotonicClockRunsFromInstantiation()
        {
            // Arrange
            // Act
            var result = MonotonicClock.Elapsed.BclCompatibleTicks;

            // Assert
            Assert.True(result > 20);
        }

        [Fact]
        internal void ScheduleOnce_ThenActionShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleOnce(Duration.Zero, this.Run);

            Task.Delay(300).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(1, this.testActionCount);
        }

        [Fact]
        internal void ScheduleOnceCancelable_WhenNotCancelled_ThenActionShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleOnceCancelable(Duration.Zero, this.Run);

            Task.Delay(300).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(1, this.testActionCount);
        }

        [Fact]
        internal void ScheduleOnceCancelable_WhenCancelled_ThenActionShouldNotBeInvoked()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleOnceCancelable(Duration.Zero, this.Run);
            cancelable.Cancel();

            Task.Delay(300).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(0, this.testActionCount);
        }

        [Fact]
        internal void ScheduleRepeatedly_ThenActionsShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleRepeatedly(
                Duration.Zero,
                Duration.FromMilliseconds(10),
                this.Run);

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(300).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.True(this.testActionCount > 1);
        }

        [Fact]
        internal void ScheduleRepeatedlyCancelable_WhenNotCancelled_ThenActionsShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleRepeatedlyCancelable(
                Duration.Zero,
                Duration.FromMilliseconds(10),
                this.Run);

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(300).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.True(this.testActionCount > 1);
        }

        [Fact]
        internal void ScheduleRepeatedlyCancelable_WhenCancelled_ThenActionsShouldNotBeInvoked()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleRepeatedlyCancelable(
                Duration.Zero,
                Duration.FromMilliseconds(10),
                this.Run);
            cancelable.Cancel();

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(300).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(0, this.testActionCount);
        }

        [Fact]
        internal void ScheduleSendOnce_ThenMessageShouldBeSent()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendOnce(
                Duration.Zero,
                this.testReceiver.Endpoint,
                "TEST",
                this.scheduler.Endpoint);

            Task.Delay(300).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Single(this.testReceiver.Messages);
            Assert.Equal("TEST", this.testReceiver.Messages[0]);
        }

        [Fact]
        internal void ScheduleSendOnceCancelable_WhenNotCancelled_ThenMessageShouldBeSent()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendOnceCancelable(
                Duration.Zero,
                this.testReceiver.Endpoint,
                "TEST",
                this.scheduler.Endpoint);

            Task.Delay(300).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Single(this.testReceiver.Messages);
            Assert.Equal("TEST", this.testReceiver.Messages[0]);
        }

        [Fact]
        internal void ScheduleSendOnceCancelable_WhenCancelled_ThenMessageShouldNotBeSent()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleSendOnceCancelable(
                Duration.Zero,
                this.testReceiver.Endpoint,
                "TEST",
                this.scheduler.Endpoint);
            cancelable.Cancel();
            Task.Delay(300).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Empty(this.testReceiver.Messages);
        }

        [Fact]
        internal void ScheduleSendRepeatedly_ThenMessageShouldBeSentRepeatedly()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendRepeatedly(
                Duration.Zero,
                Duration.FromMilliseconds(10),
                this.testReceiver.Endpoint,
                "TEST",
                this.scheduler.Endpoint);

            Task.Delay(300).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.True(this.testReceiver.Messages.Count > 1);
            Assert.Equal("TEST", this.testReceiver.Messages[0]);
        }

        [Fact]
        internal void ScheduleSendRepeatedlyCancelable_WhenNotCancelled_ThenMessageShouldBeSentRepeatedly()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendRepeatedlyCancelable(
                Duration.Zero,
                Duration.FromMilliseconds(10),
                this.testReceiver.Endpoint,
                "TEST",
                this.scheduler.Endpoint);

            Task.Delay(300).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.True(this.testReceiver.Messages.Count > 1);
            Assert.Equal("TEST", this.testReceiver.Messages[0]);
        }

        [Fact]
        internal void ScheduleSendRepeatedlyCancelable_WhenCancelled_ThenMessageShouldNotBeSentRepeatedly()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleSendRepeatedlyCancelable(
                Duration.Zero,
                Duration.FromMilliseconds(10),
                this.testReceiver.Endpoint,
                "TEST",
                this.scheduler.Endpoint);
            cancelable.Cancel();
            Task.Delay(300).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Empty(this.testReceiver.Messages);
        }

        private void Run()
        {
            this.testActionCount++;
        }
    }
}
