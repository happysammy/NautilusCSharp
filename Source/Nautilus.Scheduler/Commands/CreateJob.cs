//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// The job command to create a new job..
    /// </summary>
    [Immutable]
    public sealed class CreateJob : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJob"/> class.
        /// </summary>
        /// <param name="sender">The sender of the job.</param>
        /// <param name="receiver">The receiver for the job.</param>
        /// <param name="message">The job message to send.</param>
        /// <param name="trigger">The job trigger.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public CreateJob(
            IEndpoint sender,
            IEndpoint receiver,
            object message,
            ITrigger trigger,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(sender, nameof(sender));
            Debug.NotNull(receiver, nameof(receiver));
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(trigger, nameof(trigger));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Sender = sender;
            this.Receiver = receiver;
            this.Message = message;
            this.Trigger = trigger;
        }

        /// <summary>
        /// Gets the jobs sender.
        /// </summary>
        public IEndpoint Sender { get; }

        /// <summary>
        /// Gets the jobs destination actor.
        /// </summary>
        public IEndpoint Receiver { get; }

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
