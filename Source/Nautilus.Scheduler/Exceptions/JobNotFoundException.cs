

namespace Nautilus.Scheduler.Exceptions
{
    using System;

    public class JobNotFoundException: Exception
    {
        public JobNotFoundException() : base("job not found")
        {
        }
    }
}
