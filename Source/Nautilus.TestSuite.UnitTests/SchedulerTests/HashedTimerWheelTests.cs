//--------------------------------------------------------------------------------------------------
// <copyright file="HashedTimerWheelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SchedulerTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Nautilus.Scheduler;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class HashedTimerWheelTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MockMessagingAgent testReceiver;
        private readonly HashedWheelTimerScheduler scheduler;

        private int testActionCount = 0;

        public HashedTimerWheelTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.testReceiver = new MockMessagingAgent();
            this.testReceiver.RegisterHandler<string>(this.testReceiver.OnMessage);
            this.scheduler = new HashedWheelTimerScheduler(container);
        }

        [Fact]
        internal void MonotonicClockRunsFromInstantiation()
        {
            // Arrange
            // Act
            var result = this.scheduler.Elapsed;

            // Assert
            Assert.True(result > 20);
            this.output.WriteLine(result.ToString());
        }

        [Fact]
        internal void ScheduleOnce_ThenActionShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleOnce(TimeSpan.Zero, this.Run);

            Task.Delay(100).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(1, this.testActionCount);
        }

        [Fact]
        internal void ScheduleOnceCancelable_WhenNotCancelled_ThenActionShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleOnceCancelable(TimeSpan.Zero, this.Run);

            Task.Delay(100).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(1, this.testActionCount);
        }

        [Fact]
        internal void ScheduleOnceCancelable_WhenCancelled_ThenActionShouldNotBeInvoked()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleOnceCancelable(TimeSpan.Zero, this.Run);
            cancelable.Cancel();

            Task.Delay(100).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(0, this.testActionCount);
        }

        [Fact]
        internal void ScheduleRepeatedly_ThenActionsShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.Run);

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(100).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.True(this.testActionCount > 4);
        }

        [Fact]
        internal void ScheduleRepeatedlyCancelable_WhenNotCancelled_ThenActionsShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.Run);

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(100).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.True(this.testActionCount > 4);
        }

        [Fact]
        internal void ScheduleRepeatedlyCancelable_WhenCancelled_ThenActionsShouldNotBeInvoked()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.Run);
            cancelable.Cancel();

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(100).Wait(); // Wait for potential action(s) to fire.

            // Assert
            Assert.Equal(0, this.testActionCount);
        }

        [Fact]
        internal void ScheduleSendOnce_ThenMessageShouldBeSent()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendOnce(TimeSpan.Zero, this.testReceiver.Endpoint, "TEST", this.scheduler.Endpoint);

            Task.Delay(100).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Single(this.testReceiver.Messages);
            Assert.Equal("TEST", this.testReceiver.Messages[0]);
        }

        [Fact]
        internal void ScheduleSendOnceCancelable_WhenNotCancelled_ThenMessageShouldBeSent()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendOnceCancelable(TimeSpan.Zero, this.testReceiver.Endpoint, "TEST", this.scheduler.Endpoint);

            Task.Delay(100).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Single(this.testReceiver.Messages);
            Assert.Equal("TEST", this.testReceiver.Messages[0]);
        }

        [Fact]
        internal void ScheduleSendOnceCancelable_WhenCancelled_ThenMessageShouldNotBeSent()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleSendOnceCancelable(TimeSpan.Zero, this.testReceiver.Endpoint, "TEST", this.scheduler.Endpoint);
            cancelable.Cancel();
            Task.Delay(100).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Empty(this.testReceiver.Messages);
        }

        [Fact]
        internal void ScheduleSendRepeatedly_ThenMessageShouldBeSentRepeatedly()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleSendRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.testReceiver.Endpoint, "TEST", this.scheduler.Endpoint);

            Task.Delay(100).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.True(this.testReceiver.Messages.Count > 4);
            Assert.Equal("TEST", this.testReceiver.Messages[4]);
        }

        [Fact]
        internal void ScheduleSendRepeatedlyCancelable_WhenNotCancelled_ThenMessageShouldBeSentRepeatedly()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleSendRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.testReceiver.Endpoint, "TEST", this.scheduler.Endpoint);

            Task.Delay(100).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.True(this.testReceiver.Messages.Count > 4);
            Assert.Equal("TEST", this.testReceiver.Messages[4]);
        }

        [Fact]
        internal void ScheduleSendRepeatedlyCancelable_WhenCancelled_ThenMessageShouldNotBeSentRepeatedly()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleSendRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.testReceiver.Endpoint, "TEST", this.scheduler.Endpoint);
            cancelable.Cancel();
            Task.Delay(100).Wait(); // Wait for potential message(s) to send.

            // Assert
            Assert.Empty(this.testReceiver.Messages);
        }

        private void Run()
        {
            this.testActionCount++;
        }
    }
}
