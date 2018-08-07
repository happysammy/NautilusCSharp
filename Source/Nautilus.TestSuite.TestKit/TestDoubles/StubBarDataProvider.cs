// -------------------------------------------------------------------------------------------------
// <copyright file="StubBarDataProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides the stub bar data provider meta-data.
    /// </summary>
    public class StubBarDataProvider : IBarDataProvider
    {
        public string DateTimeParsePattern => "yyyy.MM.dd HH:mm:ss";

        public IReadOnlyCollection<BarType> SymbolBarDatas => this.GetSymbolBarDatas();

        public DirectoryInfo DataPath => new DirectoryInfo(TestKitConstants.TestDataDirectory);

        public string TimestampParsePattern => "yyyy.MM.dd HH:mm:ss";

        public int VolumeMultiple => 1000000;

        public bool IsBarDataCheckOn => false;

        // TODO: Temporary property. Remove once Dukascopy provider removed.
        public bool InitialFromDateSpecified => false;

        public string GetResolutionLabel(Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution.Second:
                    return "Second";

                case Resolution.Minute:
                    return "Min";

                case Resolution.Hour:
                    return "Hour";

                case Resolution.Day:
                    return "Day";

                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private IReadOnlyCollection<BarType> GetSymbolBarDatas()
        {
            var symbols = new List<string>
                              {
                                  "AUDCAD",
                                  "AUDCHF",
                                  "AUDJPY",
                                  "AUDNZD",
                                  "AUDSGD",
                                  "AUDUSD",
                                  "CADCHF",
                                  "CADHKD",
                                  "CADJPY",
                                  "CHFJPY",
                                  "CHFSGD",
                                  "EURAUD",
                                  "EURCAD",
                                  "EURCHF",
                                  "EURCZK",
                                  "EURDKK",
                                  "EURGBP",
                                  "EURHKD",
                                  "EURHUF",
                                  "EURJPY",
                                  "EURNOK",
                                  "EURNZD",
                                  "EURPLN",
                                  "EURRUB",
                                  "EURSEK",
                                  "EURSGD",
                                  "EURTRY",
                                  "EURUSD",
                                  "GBPAUD",
                                  "GBPCAD",
                                  "GBPCHF",
                                  "GBPJPY",
                                  "GBPNZD",
                                  "GBPUSD",
                                  "HKDJPY",
                                  "NZDCAD",
                                  "NZDCHF",
                                  "NZDJPY",
                                  "NZDUSD",
                                  "SGDJPY",
                                  "TRYJPY",
                                  "USDCAD",
                                  "USDCHF",
                                  "USDCNH",
                                  "USDCZK",
                                  "USDDKK",
                                  "USDHKD",
                                  "USDHUF",
                                  "USDJPY",
                                  "USDMXN",
                                  "USDNOK",
                                  "USDPLN",
                                  "USDRON",
                                  "USDRUB",
                                  "USDSEK",
                                  "USDSGD",
                                  "USDTHB",
                                  "USDTRY",
                                  "USDZAR",
                                  "ZARJPY"
                              }.Distinct();

            var barTypes = new List<BarType>();

            foreach (var symbol in symbols)
            {
                barTypes.Add(new BarType(new Symbol(symbol, Venue.Dukascopy), new BarSpecification(QuoteType.Bid, Resolution.Minute, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.Dukascopy), new BarSpecification(QuoteType.Ask, Resolution.Minute, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.Dukascopy), new BarSpecification(QuoteType.Bid, Resolution.Hour, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.Dukascopy), new BarSpecification(QuoteType.Ask, Resolution.Hour, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.Dukascopy), new BarSpecification(QuoteType.Bid, Resolution.Day, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.Dukascopy), new BarSpecification(QuoteType.Ask, Resolution.Day, 1)));
            }

            return barTypes.AsReadOnly();
        }

        // TODO: Temporary method. Remove once Dukascopy provider removed.
        public CommandResult InitialFromDateConfigCsv(IReadOnlyList<string> currencyPairs, ZonedDateTime toDateTime)
        {
            throw new NotImplementedException();
        }

        // TODO: Temporary method. Remove once Dukascopy provider removed.
        public CommandResult UpdateConfigCsv(IReadOnlyList<string> currencyPairs, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
