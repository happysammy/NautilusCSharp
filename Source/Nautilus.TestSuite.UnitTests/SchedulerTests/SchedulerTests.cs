//--------------------------------------------------------------------------------------------------
// <copyright file="SchedulerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SchedulerTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Scheduler;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
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

            var setupFactory = new StubSetupContainerFactory();
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
            var scheduleBuilder = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"unit_test_job", "unit_testing")
                .WithSchedule(scheduleBuilder)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                this.testActor,
                "some_job",
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(createJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            this.ExpectMsg<JobCreated>();
        }

        [Fact]
        internal void Test_can_create_job_and_send_message_based_on_quartz_trigger()
        {
            // Arrange
            var scheduleBuilder = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"unit_test_job", "unit_testing")
                .WithSchedule(scheduleBuilder)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                this.testActor,
                "some_job",
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(createJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            this.ExpectMsg<JobCreated>();
            this.ExpectMsg<string>();
        }

        [Fact]
        internal void Test_can_pause_a_created_job()
        {
            // Arrange
            var scheduleBuilder = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"unit_test_job", "unit_testing")
                .WithSchedule(scheduleBuilder)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                this.testActor,
                "some_job",
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Used to get job key.
            this.scheduler.Send(createJob);
            var jobCreated = this.ExpectMsg<JobCreated>();

            var pauseJob = new PauseJob(
                jobCreated.JobKey,
                this.testActor,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(pauseJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            CustomAssert.EventuallyContains(
                $"Scheduler: Job paused successfully {jobCreated.JobKey}.",
                this.logger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void Test_can_resume_a_paused_job()
        {
            // Arrange
            var scheduleBuilder = SimpleScheduleBuilder
                .RepeatSecondlyForTotalCount(1)
                .WithMisfireHandlingInstructionFireNow();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"unit_test_job", "unit_testing")
                .WithSchedule(scheduleBuilder)
                .Build();

            var createJob = new CreateJob(
                this.testActor,
                this.testActor,
                "some_job",
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Used to get job key.
            this.scheduler.Send(createJob);
            var jobKey = this.ExpectMsg<JobCreated>().JobKey;

            var pauseJob = new PauseJob(
                jobKey,
                this.testActor,
                Guid.NewGuid(),
                this.clock.TimeNow());

            this.scheduler.Send(pauseJob);

            var resumeJob = new ResumeJob(
                jobKey,
                this.testActor,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(resumeJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            CustomAssert.EventuallyContains(
                $"Scheduler: Job resumed successfully {jobKey}.",
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
                "some_job",
                this.testActor,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(removeJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            this.ExpectMsg<RemoveJobFail>();
        }
    }
}
