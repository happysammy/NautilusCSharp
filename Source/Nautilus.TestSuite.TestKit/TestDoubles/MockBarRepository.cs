//--------------------------------------------------------------------------------------------------
// <copyright file="MockBarRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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

        public CommandResult SnapshotDatabase()
        {
            throw new System.NotImplementedException();
        }
    }
}
