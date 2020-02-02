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
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockBarRepository : IBarRepository
    {
        private readonly Dictionary<BarType, List<Bar>> database;

        public MockBarRepository()
        {
            this.database = new Dictionary<BarType, List<Bar>>();
        }

        public void Add(BarType barType, Bar bar)
        {
            if (!this.database.ContainsKey(barType))
            {
                this.database.Add(barType, new List<Bar>());
            }

            this.database[barType].Add(bar);
        }

        public void Add(BarType barType, Bar[] bars)
        {
            foreach (var bar in bars)
            {
                this.Add(barType, bar);
            }
        }

        public void Add(BarDataFrame barData)
        {
            foreach (var bar in barData.Bars)
            {
                this.Add(barData.BarType, bar);
            }
        }

        public int BarsCount(BarType barType)
        {
            return this.database[barType].Count;
        }

        public int AllBarsCount()
        {
            return this.database.Select(kvp => kvp.Value.Count).Sum();
        }

        public QueryResult<BarDataFrame> GetBars(BarType barType, int limit = 0)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BarDataFrame> GetBars(BarType barType, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<byte[][]> GetBarData(BarType barType, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BarDataFrame> GetBars(BarType barType)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BarDataFrame> GetBars(BarType barType, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            if (!this.database.ContainsKey(barType))
            {
                return QueryResult<BarDataFrame>.Fail($"No bars for {barType}");
            }

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(
                barType,
                this.database[barType].Where(b =>
                    b.Timestamp.IsGreaterThanOrEqualTo(fromDateTime) &&
                    b.Timestamp.IsLessThanOrEqualTo(toDateTime)).ToArray()));
        }

        public QueryResult<byte[][]> GetBarData(BarType barType, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            return !this.database.ContainsKey(barType)
                ? QueryResult<ZonedDateTime>.Fail($"No bars for {barType}")
                : QueryResult<ZonedDateTime>.Ok(this.database[barType].Last().Timestamp);
        }

        public void TrimToDays(BarStructure barStructure, int trimToDays)
        {
            // Not implemented yet
        }

        public void SnapshotDatabase()
        {
            // Not implemented yet
        }
    }
}
