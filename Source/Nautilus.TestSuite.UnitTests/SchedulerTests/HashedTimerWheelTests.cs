//--------------------------------------------------------------------------------------------------
// <copyright file="HashedTimerWheelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SchedulerTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
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

        private bool didTestActionRun = false;

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
        internal void WhenScheduleOnce_ThenActionsShouldBeInvoked()
        {
            // Arrange
            // Act
            this.scheduler.ScheduleOnce(0, this.Run);

            Task.Delay(100).Wait(); // Wait for potential action to fire.

            // Assert
            Assert.True(this.didTestActionRun);
        }

        [Fact]
        internal void WhenScheduleOnceUsingCanceledCancelable_ThenTheirActionsShouldNotBeInvoked()
        {
            // Arrange
            var cancelable = Cancelable.CreateCanceled();

            // Act
            this.scheduler.ScheduleOnce(0, this.Run, cancelable);

            Task.Delay(100).Wait(); // Wait for potential action to fire.

            // Assert
            Assert.False(this.didTestActionRun);
        }

        private void Run()
        {
            this.didTestActionRun = true;
        }
    }
}
