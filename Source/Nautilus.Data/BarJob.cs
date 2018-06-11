namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;

    public class BarJob : ValueObject<BarJob>, IEquatable<BarJob>
    {
        public BarJob(Symbol symbol, BarSpecification barSpecification)
        {
            Symbol = symbol;
            BarSpecification = barSpecification;
        }

        public Symbol Symbol { get; }

        public BarSpecification BarSpecification { get; }

        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
            {
                this.Symbol,
                this.BarSpecification
            };
        }

        public override string ToString()
        {
            return $"{nameof(BarJob)}-{this.Symbol}-{this.BarSpecification}";
        }
    }
}
