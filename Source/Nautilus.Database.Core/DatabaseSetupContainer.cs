namespace Nautilus.Database.Core
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;

    public class DatabaseSetupContainer : ComponentryContainer
    {
        public DatabaseSetupContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
            : base(clock, guidFactory, loggerFactory)
        {
        }
    }
}
