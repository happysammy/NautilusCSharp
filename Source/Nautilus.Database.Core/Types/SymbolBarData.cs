

namespace Nautilus.Database.Core.Types
{
    using Nautilus.DomainModel.ValueObjects;

    public class SymbolBarData
    {
        public SymbolBarData(
            Symbol symbol,
            BarSpecification barSpecification)
        {
            this.Symbol = symbol;
            this.BarSpecification = barSpecification;
        }

        public Symbol Symbol { get; }

        public BarSpecification BarSpecification { get; }
    }
}
