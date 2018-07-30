//--------------------------------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;
    using System.Collections.Specialized;
    using Akka.Actor;
    using Quartz.Impl;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using Nautilus.Scheduler.Exceptions;

    using IScheduler = Quartz.IScheduler;

    /// <summary>
    /// Provides a system scheduling actor with an internal quartz scheduler which processes Add
    /// and Remove messages.
    /// </summary>
    public sealed class Scheduler : ActorComponentBase
    {
        private readonly IScheduler quartzScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        public Scheduler(IComponentryContainer container)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(Scheduler)),
                container)
        {
            Validate.NotNull(container, nameof(container));

            var properties = new NameValueCollection { {"quartz.threadPool.threadCount", "10"} };
            this.quartzScheduler = new StdSchedulerFactory(properties).GetScheduler().Result;

            this.Receive<CreateJob>(msg => this.OnMessage(msg));
            this.Receive<RemoveJob>(msg => this.OnMessage(msg));
            this.Receive<PauseJob>(msg => this.OnMessage(msg));
            this.Receive<ResumeJob>(msg => this.OnMessage(msg));
        }

        protected override void PreStart()
        {
            base.PreStart();
            this.quartzScheduler.Start();
        }

        protected override void PostStop()
        {
            this.quartzScheduler.Shutdown();
            base.PostStop();
        }

        private void OnMessage(CreateJob message)
        {
            Debug.NotNull(message, nameof(message));

            var destination = message.Destination;

            try
            {
                var job = Job.CreateBuilderWithData(
                        destination,
                        message.Message)
                    .WithIdentity(message.Trigger.JobKey)
                    .Build();

                quartzScheduler.ScheduleJob(job, message.Trigger);

                destination.Tell(new JobCreated(
                    message.Trigger.JobKey,
                    message.Trigger.Key,
                    message.Message));
            }
            catch (Exception ex) // TODO: Make this exception more specific.
            {
                destination.Tell(new CreateJobFail(
                    message.Trigger.JobKey,
                    message.Trigger.Key,
                    ex));
            }
        }

        private void OnMessage(PauseJob message)
        {
            Debug.NotNull(message, nameof(message));

            try
            {
                var paused = this.quartzScheduler.PauseJob(message.JobKey);
                if (paused.IsCompletedSuccessfully)
                {
                    this.Log.Information($"Job paused successfully {message.JobKey}.");
                }
                else
                {
                    this.Log.Warning($"Job pause failed for {message.JobKey}.");
                }
            }
            catch (Exception ex)  // TODO: Make this exception more specific.
            {
                this.Log.Error($"Job pause failed with error for {message.JobKey}.", ex);
            }
        }

        private void OnMessage(ResumeJob message)
        {
            Debug.NotNull(message, nameof(message));

            try
            {
                var resume = this.quartzScheduler.ResumeJob(message.JobKey);
                if (resume.IsCompletedSuccessfully)
                {
                    this.Log.Information($"Job resumed successfully {message.JobKey}.");
                }
                else
                {
                    this.Log.Warning($"Job resume failed for {message.JobKey}.");
                }
            }
            catch (Exception ex)  // TODO: Make this exception more specific.
            {
                this.Log.Error($"Job resume failed with error for {message.JobKey}.", ex);
            }
        }

        private void OnMessage(RemoveJob message)
        {
            Debug.NotNull(message, nameof(message));

            var sender = message.Sender;

            try
            {
                var deleted = quartzScheduler.DeleteJob(message.JobKey);
                if (deleted.IsCompletedSuccessfully)
                {
                    sender.Tell(new JobRemoved(
                        message.JobKey,
                        message.TriggerKey,
                        message.Job));
                }
                else
                {
                    sender.Tell(new RemoveJobFail(message.JobKey, message.TriggerKey, new JobNotFoundException()));
                }
            }
            catch (Exception ex)  // TODO: Make this exception more specific.
            {
                sender.Tell(new RemoveJobFail(message.JobKey, message.TriggerKey, ex));
            }
        }
    }
}
