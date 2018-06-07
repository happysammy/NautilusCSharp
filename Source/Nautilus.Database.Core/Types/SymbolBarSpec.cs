

namespace Nautilus.Database.Core.Types
{
    using Nautilus.DomainModel.ValueObjects;

    public class SymbolBarSpec
    {
        public SymbolBarSpec(
            Symbol symbol,
            BarSpecification barSpecification)
        {
            this.Symbol = symbol;
            this.BarSpecification = barSpecification;
        }

        public Symbol Symbol { get; }

        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Symbol} {this.BarSpecification.Period}-{this.BarSpecification.Resolution}[{this.BarSpecification.QuoteType}]";
        }
    }
}
