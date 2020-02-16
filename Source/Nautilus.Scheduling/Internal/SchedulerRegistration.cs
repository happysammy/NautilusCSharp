// -------------------------------------------------------------------------------------------------
// <copyright file="SchedulerRegistration.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Internal
{
    using Nautilus.Scheduling;
    using Nautilus.Scheduling.Internal;

    /// <summary>
    /// Represents a scheduler registration.
    /// </summary>
    internal sealed class SchedulerRegistration
    {
        private readonly ICancelable? cancellation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerRegistration"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellation">The cancellation.</param>
        internal SchedulerRegistration(IRunnable action, ICancelable? cancellation)
        {
            this.Action = action;
            this.cancellation = cancellation;
        }

        /// <summary>
        /// Gets the registrations action.
        /// </summary>
        internal IRunnable Action { get; }

        /// <summary>
        /// Gets or sets the next registration.
        /// </summary>
        internal SchedulerRegistration? Next { get; set; }

        /// <summary>
        /// Gets or sets the previous registration.
        /// </summary>
        internal SchedulerRegistration? Prev { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Bucket"/> to which this registration belongs.
        /// </summary>
        internal Bucket? Bucket { get; set; }

        /// <summary>
        /// Gets or sets the remaining rounds.
        /// </summary>
        internal long RemainingRounds { get; set; }

        /// <summary>
        /// Gets or sets the offset. Used to determine the delay for repeatable sends.
        /// </summary>
        internal long Offset { get; set; }

        /// <summary>
        /// Gets or sets the deadline. Used to determine when to execute.
        /// </summary>
        internal long Deadline { get; set; }

        /// <summary>
        /// Gets a value indicating whether this task will need to be re-scheduled according to its <see cref="Offset"/>.
        /// </summary>
        internal bool Repeat => this.Offset > 0;

        /// <summary>
        ///  Gets a value indicating whether the task is cancelled.
        /// </summary>
        internal bool Cancelled => this.cancellation?.IsCancellationRequested ?? false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                $"ScheduledWork(Deadline={this.Deadline}, RepeatEvery={this.Offset}, Cancelled={this.Cancelled}, Work={this.Action})";
        }

        /// <summary>
        /// Resets all of the fields so this registration object can be used again.
        /// </summary>
        internal void Reset()
        {
            this.Next = null;
            this.Prev = null;
            this.Bucket = null;
            this.Deadline = 0;
            this.RemainingRounds = 0;
        }
    }
}
