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
    using System.Threading;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Scheduler;
    using Quartz;
    using Xunit;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using Nautilus.Scheduler.Exceptions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SchedulerTests : TestKit
    {
//        [Fact]
//        public void Shceduler_Should_Create_Job()
//        {
//            var probe = CreateTestProbe(Sys);
//            var quartzActor = Sys.ActorOf(Props.Create(() => new Scheduler()), "QuartzActor");
//            quartzActor.Tell(new CreateJob(probe, "Hello", TriggerBuilder.Create().WithCronSchedule("*0/10 * * * * ?").Build()));
//            ExpectMsg<JobCreated>();
//            probe.ExpectMsg("Hello", TimeSpan.FromSeconds(11));
//            Thread.Sleep(TimeSpan.FromSeconds(10));
//            probe.ExpectMsg("Hello");
//            Sys.Stop(quartzActor);
//        }

//        [Fact]
//        public void Scheduler_Should_Remove_Job()
//        {
//            // Arrange
//            var probe = CreateTestProbe(Sys);
//            var scheduler = Sys.ActorOf(Props.Create(() => new Scheduler()), "Scheduler");
//
//            // Act
//            scheduler.Tell(new CreateJob(probe, "Hello remove", TriggerBuilder.Create().WithCronSchedule("0/1 * * * * ?").Build()));
//
//            // Assert
//            var jobCreated = ExpectMsg<JobCreated>();
//            probe.ExpectMsg("Hello remove", TimeSpan.FromSeconds(1));
//            scheduler.Tell(new RemoveJob(jobCreated.JobKey, jobCreated.TriggerKey, "A job"));
//            ExpectMsg<JobRemoved>();
//
//            Task.Delay(1000);
//            probe.ExpectNoMsg(TimeSpan.FromSeconds(1));
//            Sys.Stop(scheduler);
//        }

//        [Fact]
//        public void Scheduler_Should_Not_Remove_UnExisting_Job()
//        {
//            // Arrange
//            var probe = CreateTestProbe(Sys);
//            var scheduler = Sys.ActorOf(Props.Create(() => new Scheduler()), "Scheduler");
//
//            // Act
//            // Assert
//            scheduler.Tell(new RemoveJob(new JobKey("key"), new TriggerKey("key"), "A job"));
//            Task.Delay(1000);
//
//            // Assert
//            var failure=ExpectMsg<RemoveJobFail>();
//            Assert.IsType<JobNotFoundException>(failure.Reason);
//            Sys.Stop(scheduler);
//        }
    }
}
