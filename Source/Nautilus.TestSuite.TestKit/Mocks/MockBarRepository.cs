//--------------------------------------------------------------------------------------------------
// <copyright file="MockBarRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nautilus.Common.Interfaces;
using Nautilus.Core.CQS;
using Nautilus.Core.Extensions;
using Nautilus.Data.Interfaces;
using Nautilus.Data.Keys;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Frames;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.TestSuite.TestKit.Mocks
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockBarRepository : IBarRepository
    {
        private readonly Dictionary<BarType, List<Bar>> database;
        private readonly IDataSerializer<Bar> serializer;

        public MockBarRepository(IDataSerializer<Bar> serializer)
        {
            this.database = new Dictionary<BarType, List<Bar>>();
            this.serializer = serializer;
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

        public long BarsCount(BarType barType)
        {
            return this.database[barType].Count;
        }

        public long BarsCount()
        {
            return this.database.Select(kvp => kvp.Value.Count).Sum();
        }

        public QueryResult<BarDataFrame> GetBars(BarType barType, int limit = 0)
        {
            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barType, this.database[barType].ToArray()));
        }

        public QueryResult<BarDataFrame> GetBars(BarType barType, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            if (!this.database.ContainsKey(barType))
            {
                return QueryResult<BarDataFrame>.Fail($"No bars for {barType}");
            }

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(
                barType,
                this.database[barType].Where(b =>
                    b.Timestamp.IsGreaterThanOrEqualTo(fromDate.StartOfDay) &&
                    b.Timestamp.IsLessThanOrEqualTo(fromDate.StartOfDay)).ToArray()));
        }

        public QueryResult<byte[][]> GetBarData(BarType barType, int limit = 0)
        {
            return this.database.TryGetValue(barType, out var barList)
                ? QueryResult<byte[][]>.Ok(this.serializer.Serialize(barList.ToArray()))
                : QueryResult<byte[][]>.Fail($"Cannot find any bar data for {barType}");
        }

        public QueryResult<byte[][]> GetBarData(BarType barType, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            return this.database.TryGetValue(barType, out var barList)
                ? QueryResult<byte[][]>.Ok(this.serializer.Serialize(barList.ToArray()))
                : QueryResult<byte[][]>.Fail($"Cannot find any bar data for {barType}");
        }

        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            return !this.database.ContainsKey(barType)
                ? QueryResult<ZonedDateTime>.Fail($"No bars for {barType}")
                : QueryResult<ZonedDateTime>.Ok(this.database[barType][^1].Timestamp);
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
