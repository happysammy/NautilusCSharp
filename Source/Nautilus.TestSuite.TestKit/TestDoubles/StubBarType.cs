// -------------------------------------------------------------------------------------------------
// <copyright file="StubbarTypeification.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class StubBarType
    {
        public static BarType AUDUSD()
        {
            return new BarType(
                new Symbol("AUDUSD", Venue.Dukascopy),
                new BarSpecification(
                    QuoteType.Ask,
                    Resolution.Minute,
                    1));
        }

        public static BarType GBPUSD()
        {
            return new BarType(
                new Symbol("GBPUSD", Venue.Dukascopy),
                new BarSpecification(
                    QuoteType.Bid,
                    Resolution.Minute,
                    1));
        }

        public static BarType GBPUSD_Second()
        {
            return new BarType(
                new Symbol("GBPUSD", Venue.Dukascopy),
                new BarSpecification(
                    QuoteType.Mid,
                    Resolution.Second,
                    1));
        }

        public static BarType USDJPY()
        {
            return new BarType(
                new Symbol("USDJPY", Venue.Dukascopy),
                new BarSpecification(
                    QuoteType.Bid,
                    Resolution.Minute,
                    1));
        }

        public static BarType CADHKD()
        {
            return new BarType(
                new Symbol("CADHKD", Venue.Dukascopy),
                new BarSpecification(
                    QuoteType.Bid,
                    Resolution.Minute,
                    1));
        }
    }
}
