//--------------------------------------------------------------------------------------------------
// <copyright file="MockBarRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockBarRepository : IBarRepository
    {
        private readonly Dictionary<BarType, List<Bar>> bars;

        public MockBarRepository()
        {
            this.bars = new Dictionary<BarType, List<Bar>>();
        }

        public int BarsCount(BarType barType)
        {
            return this.bars[barType].Count;
        }

        public int AllBarsCount()
        {
            return this.bars.Select(kvp => kvp.Value.Count).Sum();
        }

        public CommandResult Add(BarType barType, Bar bar)
        {
            if (!this.bars.ContainsKey(barType))
            {
                this.bars.Add(barType, new List<Bar>());
            }

            this.bars[barType].Add(bar);

            return CommandResult.Ok();
        }

        public CommandResult Add(BarDataFrame barData)
        {
            if (this.bars.ContainsKey(barData.BarType))
            {
                this.bars.Add(barData.BarType, new List<Bar>());
            }

            foreach (var bar in barData.Bars)
            {
                this.Add(barData.BarType, bar);
            }

            return CommandResult.Ok();
        }

        public QueryResult<BarDataFrame> Find(BarType barType, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            if (!this.bars.ContainsKey(barType))
            {
                return QueryResult<BarDataFrame>.Fail($"No bars for {barType}");
            }

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(
                barType,
                this.bars[barType].Where(b =>
                    b.Timestamp.IsGreaterThanOrEqualTo(fromDateTime) &&
                    b.Timestamp.IsLessThanOrEqualTo(toDateTime)).ToArray()));
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            return !this.bars.ContainsKey(barType)
                ? QueryResult<ZonedDateTime>.Fail($"No bars for {barType}")
                : QueryResult<ZonedDateTime>.Ok(this.bars[barType].Last().Timestamp);
        }

        public CommandResult TrimToDays(Resolution resolution, int trimToDays)
        {
            // Not implemented
            return CommandResult.Ok();
        }

        public CommandResult SnapshotDatabase()
        {
            // Not implemented
            return CommandResult.Ok();
        }
    }
}
