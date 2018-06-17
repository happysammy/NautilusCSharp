//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Quartz;

    /// <summary>
    /// The job command to create a new job..
    /// </summary>
    [Immutable]
    public sealed class CreateJob : IJobCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJob"/> class.
        /// </summary>
        /// <param name="destination">The actor to send the message to.</param>
        /// <param name="message">The job to message to send./</param>
        /// <param name="trigger">The job trigger.</param>
        public CreateJob(IActorRef destination, object message, ITrigger trigger)
        {
            Debug.NotNull(destination, nameof(destination));
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(trigger, nameof(trigger));

            this.Destination = destination;
            this.Message = message;
            this.Trigger = trigger;
        }

        /// <summary>
        /// Gets the jobs destination actor.
        /// </summary>
        public IActorRef Destination { get; }

        /// <summary>
        /// Gets the jobs message.
        /// </summary>
        public object Message { get; }

        /// <summary>
        /// Gets the jobs trigger.
        /// </summary>
        public ITrigger Trigger { get; }
    }
}
