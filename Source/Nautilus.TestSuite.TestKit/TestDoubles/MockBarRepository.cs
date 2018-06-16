namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Core.CQS;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    public class MockBarRepository : IBarRepository
    {
        public long BarsCount(SymbolBarSpec symbolBarSpec)
        {
            throw new System.NotImplementedException();
        }

        public long AllBarsCount()
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(BarDataFrame barData)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BarDataFrame> Find(SymbolBarSpec barSpec, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(SymbolBarSpec barSpec)
        {
            throw new System.NotImplementedException();
        }
    }
}
