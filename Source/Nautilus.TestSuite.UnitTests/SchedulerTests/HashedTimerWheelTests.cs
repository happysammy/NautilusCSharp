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
    using System.Threading.Tasks;
    using Nautilus.Scheduler;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class HashedTimerWheelTests
    {
        private readonly ITestOutputHelper output;
        private readonly HashedWheelTimerScheduler scheduler;

        private int testActionCount = 0;

        public HashedTimerWheelTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
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

            Task.Delay(100).Wait(); // Wait for potential action to fire.

            // Assert
            Assert.Equal(1, this.testActionCount);
        }

        [Fact]
        internal void ScheduleOnceCancelable_WhenNotCancelled_ThenActionShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleOnceCancelable(TimeSpan.Zero, this.Run);

            Task.Delay(100).Wait(); // Wait for potential action to fire.

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

            Task.Delay(100).Wait(); // Wait for potential action to fire.

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
            Task.Delay(100).Wait(); // Wait for potential action to fire.

            // Assert
            Assert.Equal(5, this.testActionCount);
        }

        [Fact]
        internal void ScheduleRepeatedlyCancelable_WhenNotCancelled_ThenActionsShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.Run);

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(100).Wait(); // Wait for potential action to fire.

            // Assert
            Assert.Equal(5, this.testActionCount);
        }

        [Fact]
        internal void ScheduleRepeatedlyCancelable_WhenCancelled_ThenActionsShouldNotBeInvoked()
        {
            // Arrange
            // Act
            var cancelable = this.scheduler.ScheduleRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), this.Run);
            cancelable.Cancel();

            // Takes approx 50ms to spool up the scheduler.
            Task.Delay(100).Wait(); // Wait for potential action to fire.

            // Assert
            Assert.Equal(0, this.testActionCount);
        }

        private void Run()
        {
            this.testActionCount++;
        }
    }
}
