namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using NautechSystems.CSharp.CQS;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    public class MockMarketDataRepository : IMarketDataRepository
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

        public QueryResult<MarketDataFrame> Find(BarSpecification barSpec, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(SymbolBarSpec barSpec)
        {
            throw new System.NotImplementedException();
        }
    }
}
