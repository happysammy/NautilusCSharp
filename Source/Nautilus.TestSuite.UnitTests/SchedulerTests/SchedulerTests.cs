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

            var message = "some_job";

            var createJob = new CreateJob(
                this.testActor,
                this.testActor,
                message,
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
        internal void Test_can_receive_create_job_command_with()
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

            var message = "some_job";

            var createJob = new CreateJob(
                this.testActor,
                this.testActor,
                message,
                trigger,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            this.scheduler.Send(createJob);

            // Assert
            LogDumper.Dump(this.logger, this.output);
            this.ExpectMsg<JobCreated>();
        }
    }
}
