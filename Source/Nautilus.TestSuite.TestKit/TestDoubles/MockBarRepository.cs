namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Core.CQS;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Types;
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

        public CommandResult Add(MarketDataFrame marketData)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<MarketDataFrame> Find(SymbolBarSpec barSpec, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(SymbolBarSpec barSpec)
        {
            throw new System.NotImplementedException();
        }
    }
}
