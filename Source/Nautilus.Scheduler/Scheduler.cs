

namespace Nautilus.Scheduler
{
    using System;
    using System.Collections.Specialized;
    using Akka.Actor;
    using Quartz.Impl;
    using Akka;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using Nautilus.Scheduler.Exceptions;
    using IScheduler = Quartz.IScheduler;

    /// <summary>
    /// The base quartz scheduling actor. Handles a single quartz scheduler
    /// and processes Add and Remove messages.
    /// </summary>
    public class Scheduler : ActorBase
    {
        private readonly IScheduler quartzScheduler;

        private readonly bool externallySupplied;

        public Scheduler()
        {
            this.quartzScheduler = new StdSchedulerFactory().GetScheduler().Result;
        }

        public Scheduler(NameValueCollection props)
        {
            this.quartzScheduler = new StdSchedulerFactory(props).GetScheduler().Result;
        }

        public Scheduler(IScheduler quartzScheduler)
        {
            this.quartzScheduler = quartzScheduler;
            this.externallySupplied = true;
        }

        protected override bool Receive(object message)
        {
            return message.Match().With<CreateJob>(CreateJobCommand).With<RemoveJob>(RemoveJobCommand).WasHandled;
        }

        protected override void PreStart()
        {
            if (!this.externallySupplied)
            {
                this.quartzScheduler.Start();
            }
            base.PreStart();
        }

        protected override void PostStop()
        {
            if (!this.externallySupplied)
            {
                this.quartzScheduler.Shutdown();
            }
            base.PostStop();
        }

        protected virtual void CreateJobCommand(CreateJob createJob)
        {
            if (createJob.To == null)
            {
                Context.Sender.Tell(new CreateJobFail(null, null, new ArgumentNullException(nameof(createJob.To))));
            }
            if (createJob.Trigger == null)
            {
                Context.Sender.Tell(new CreateJobFail(null, null, new ArgumentNullException(nameof(createJob.Trigger))));
            }
            else
            {

                try
                {
                    var job =
                   Job.CreateBuilderWithData(createJob.To, createJob.Message)
                       .WithIdentity(createJob.Trigger.JobKey)
                       .Build();
                    quartzScheduler.ScheduleJob(job, createJob.Trigger);

                    Context.Sender.Tell(new JobCreated(createJob.Trigger.JobKey, createJob.Trigger.Key));
                }
                catch (Exception ex)
                {
                    Context.Sender.Tell(new CreateJobFail(createJob.Trigger.JobKey, createJob.Trigger.Key, ex));
                }
            }
        }

        protected virtual void RemoveJobCommand(RemoveJob removeJob)
        {
            try
            {
                var deleted = quartzScheduler.DeleteJob(removeJob.JobKey);
                if (deleted.Result)
                {
                    Context.Sender.Tell(new JobRemoved(removeJob.JobKey, removeJob.TriggerKey));
                }
                else
                {
                    Context.Sender.Tell(new RemoveJobFail(removeJob.JobKey, removeJob.TriggerKey, new JobNotFoundException()));
                }
            }
            catch (Exception ex)
            {
                Context.Sender.Tell(new RemoveJobFail(removeJob.JobKey, removeJob.TriggerKey, ex));
            }
        }
    }
}
