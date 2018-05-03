namespace Nautilus.Database.Core
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;

    public class DatabaseSetupContainer : IComponentryContainer
    {
        public DatabaseSetupContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
        {
            this.Clock = clock;
            this.GuidFactory = guidFactory;
            this.LoggerFactory = loggerFactory;
        }

        public IZonedClock Clock { get; }
        public IGuidFactory GuidFactory { get; }
        public ILoggerFactory LoggerFactory { get; }
    }
}
