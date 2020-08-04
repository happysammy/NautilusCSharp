// -------------------------------------------------------------------------------------------------
// <copyright file="LZ4CompressorTests.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Messaging.Interfaces;
using Nautilus.Scheduling;
using Nautilus.Scheduling.Messages;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Mocks;
using Quartz;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.IntegrationTests.SchedulingTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SchedulerTests
    {
        private readonly ITestOutputHelper output;
        private readonly IZonedClock clock;
        private readonly IEndpoint receiver;
        private readonly Scheduler scheduler;

        public SchedulerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var container = TestComponentryContainer.Create(output);
            this.clock = container.Clock;
            this.receiver = new MockComponent(container).Endpoint;
            this.scheduler = new Scheduler(container, new MockMessageBusProvider(container).Adapter);
            this.scheduler.Start().Wait();
            Task.Delay(100);
        }

        [Fact]
        internal void GivenCreateJob_WithValidJob_SchedulesAndExecutes()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit-test-job", "unit-testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .StartNow()
                .Build();

            var createJob = new CreateJob(
                this.receiver,
                new ConnectSession(Guid.NewGuid(), this.clock.TimeNow()),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Endpoint.Send(createJob);

            // Assert
            // Assert.True(this.scheduler.DoesJobExist(jobKey));
        }

        [Fact]
        internal void Test_can_create_job_and_send_message_based_on_quartz_trigger()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit-test-job", "unit-testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.receiver,
                new ConnectSession(Guid.NewGuid(), this.clock.TimeNow()),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Endpoint.Send(createJob);

            // Assert
            // Assert inside scheduler.
        }

        [Fact]
        internal void Test_can_pause_a_created_job()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit-test-job", "unit-testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.receiver,
                new ConnectSession(Guid.NewGuid(), this.clock.TimeNow()),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            this.scheduler.Endpoint.Send(createJob);

            var pauseJob = new PauseJob(
                jobKey,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Endpoint.Send(pauseJob);

            // Assert

// CustomAssert.EventuallyContains(
//                $"Scheduler: Job paused successfully (JobKey={jobKey}).",
//                this.logger,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void Test_can_resume_a_paused_job()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit-test-job", "unit-testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.receiver,
                new ConnectSession(Guid.NewGuid(), this.clock.TimeNow()),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Used to get job key.
            this.scheduler.Endpoint.Send(createJob);

            var pauseJob = new PauseJob(
                jobKey,
                Guid.NewGuid(),
                this.clock.TimeNow());

            this.scheduler.Endpoint.Send(pauseJob);

            var resumeJob = new ResumeJob(
                jobKey,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Endpoint.Send(resumeJob);

            // Assert

// CustomAssert.EventuallyContains(
//                $"Scheduler: Job resumed successfully (JobKey={jobKey}).",
//                this.logger,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void Test_removing_a_job_which_does_not_exist_raises_exception()
        {
            // Arrange
            var removeJob = new RemoveJob(
                new JobKey("bogus-job-key"),
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Endpoint.Send(removeJob);

            // Assert

// CustomAssert.EventuallyContains(
//                "Scheduler: Job remove failed (JobKey=DEFAULT.bogus-job-key, TriggerKey=DEFAULT.bogus-trigger, Reason=JobNotFound).",
//                this.logger,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }
    }
}
