// -------------------------------------------------------------------------------------------------
// <copyright file="SchedulerRegistration.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    /// <summary>
    /// Represents a scheduler registration.
    /// </summary>
    internal class SchedulerRegistration
    {
        private readonly ICancelable cancellation;

        /// <summary>
        /// gets the next registration.
        /// </summary>
        public SchedulerRegistration Next;

        public SchedulerRegistration Prev;

        public long RemainingRounds;

        /// <summary>
        /// Used to determine the delay for repeatable sends.
        /// </summary>
        public long Offset;

        /// <summary>
        /// The deadline for determining when to execute.
        /// </summary>
        public long Deadline;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerRegistration"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellation">The cancellation.</param>
        internal SchedulerRegistration(IRunnable action, ICancelable cancellation)
        {
            this.Action = action;
            this.cancellation = cancellation;
        }

        /// <summary>
        /// Gets the registrations action.
        /// </summary>
        public IRunnable Action { get; }

        /// <summary>
        /// Gets a value indicating whether this task will need to be re-scheduled according to its <see cref="Offset"/>.
        /// </summary>
        public bool Repeat => this.Offset > 0;

        /// <summary>
        ///  Gets a value indicating whether the task is cancelled.
        /// </summary>
        public bool Cancelled => this.cancellation?.IsCancellationRequested ?? false;

        /// <summary>
        /// The <see cref="Bucket"/> to which this registration belongs.
        /// </summary>
        public Bucket Bucket;

        /// <summary>
        /// Resets all of the fields so this registration object can be used again
        /// </summary>
        public void Reset()
        {
            this.Next = null;
            this.Prev = null;
            this.Bucket = null;
            this.Deadline = 0;
            this.RemainingRounds = 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                $"ScheduledWork(Deadline={this.Deadline}, RepeatEvery={this.Offset}, Cancelled={this.Cancelled}, Work={this.Action})";
        }
    }
}
