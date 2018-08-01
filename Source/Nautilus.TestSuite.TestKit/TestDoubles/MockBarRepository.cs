namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    public class MockBarRepository : IBarRepository
    {
        public long BarsCount(BarType barType)
        {
            throw new System.NotImplementedException();
        }

        public long AllBarsCount()
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(BarType barType, Bar bar)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(BarDataFrame barData)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BarDataFrame> Find(BarType barType, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult TrimToDays(Resolution resolution, int trimToDays)
        {
            throw new System.NotImplementedException();
        }
    }
}
