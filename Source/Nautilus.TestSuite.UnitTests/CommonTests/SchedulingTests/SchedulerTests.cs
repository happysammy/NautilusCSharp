//--------------------------------------------------------------------------------------------------
// <copyright file="SchedulerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests.SchedulingTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Common.Scheduling;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Quartz;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SchedulerTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly IEndpoint scheduler;
        private readonly MockLoggingAdapter logger;
        private readonly IZonedClock clock;
        private readonly IEndpoint testActor;

        public SchedulerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            var setupContainer = setupFactory.Create();
            this.logger = setupFactory.LoggingAdapter;
            this.clock = setupContainer.Clock;
            this.testActor = new ActorEndpoint(this.TestActor);

            this.scheduler = new ActorEndpoint(
                this.Sys.ActorOf(Props.Create(
                    () => new Scheduler(setupContainer))));
        }

        [Fact]
        internal void Test_can_receive_create_job_command_with_valid_job()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit_test_job", "unit_testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                new TestJob(),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(createJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
        }

        [Fact]
        internal void Test_can_create_job_and_send_message_based_on_quartz_trigger()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit_test_job", "unit_testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                new TestJob(),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(createJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            this.ExpectMsg<TestJob>();
        }

        [Fact]
        internal void Test_can_pause_a_created_job()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit_test_job", "unit_testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                new TestJob(),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            this.scheduler.Send(createJob);

            var pauseJob = new PauseJob(
                jobKey,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(pauseJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            CustomAssert.EventuallyContains(
                $"Scheduler: Job paused successfully (JobKey={jobKey}).",
                this.logger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void Test_can_resume_a_paused_job()
        {
            // Arrange
            var schedule = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var jobKey = new JobKey("unit_test_job", "unit_testing");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                new TestJob(),
                jobKey,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Used to get job key.
            this.scheduler.Send(createJob);

            var pauseJob = new PauseJob(
                jobKey,
                Guid.NewGuid(),
                this.clock.TimeNow());

            this.scheduler.Send(pauseJob);

            var resumeJob = new ResumeJob(
                jobKey,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(resumeJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            CustomAssert.EventuallyContains(
                $"Scheduler: Job resumed successfully (JobKey={jobKey}).",
                this.logger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void Test_removing_a_job_which_does_not_exist_raises_exception()
        {
            // Arrange
            var removeJob = new RemoveJob(
                new JobKey("bogus-job-key"),
                new TriggerKey("bogus-trigger"),
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(removeJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            CustomAssert.EventuallyContains(
                "Scheduler: Job remove failed (JobKey=DEFAULT.bogus-job-key, TriggerKey=DEFAULT.bogus-trigger, Reason=JobNotFound).",
                this.logger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        // Only used within this class for testing purposes.
        private class TestJob : IScheduledJob
        {
            public override int GetHashCode() => 555;

            public override string ToString() => "test_job";
        }
    }
}
