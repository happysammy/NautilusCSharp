namespace Nautilus.Scheduler
{
    internal class SchedulerRegistration
    {
        /// <summary>
        /// The cancellation handle, if any.
        /// </summary>
        public readonly ICancelable Cancellation;

        /// <summary>
        /// The task to be executed.
        /// </summary>
        public readonly IRunnable Action;

        /*
         * Linked list is only ever modified from the scheduler thread, so this
         * implementation does not need to be synchronized or implement CAS semantics.
         */
        public SchedulerRegistration Next;
        public SchedulerRegistration Prev;

        public long RemainingRounds;

        /// <summary>
        /// Used to determine the delay for repeatable sends
        /// </summary>
        public long Offset;

        /// <summary>
        /// The deadline for determining when to execute
        /// </summary>
        public long Deadline;

        public SchedulerRegistration(IRunnable action, ICancelable cancellation)
        {
            Action = action;
            Cancellation = cancellation;
        }

        /// <summary>
        /// Determines if this task will need to be re-scheduled according to its <see cref="Offset"/>.
        /// </summary>
        public bool Repeat => Offset > 0;

        /// <summary>
        /// If <c>true</c>, we skip execution of this task.
        /// </summary>
        public bool Cancelled => Cancellation?.IsCancellationRequested ?? false;

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

        public override string ToString()
        {
            return
                $"ScheduledWork(Deadline={this.Deadline}, RepeatEvery={this.Offset}, Cancelled={this.Cancelled}, Work={this.Action})";
        }
    }
}
