

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;

    public class StubSetupContainer : ComponentryContainer
    {
        public StubSetupContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
            : base(clock, guidFactory, loggerFactory)
        {
        }
    }
}
