// -------------------------------------------------------------------------------------------------
// <copyright file="StubBarDataProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NautechSystems.CSharp.CQS;
    using NodaTime;

    /// <summary>
    /// Provides the <see cref="Dukascopy"/> meta-data.
    /// </summary>
    public class StubBarDataProvider : IBarDataProvider
    {
        public string DateTimeParsePattern => "yyyy.MM.dd HH:mm:ss";

        public IReadOnlyCollection<SymbolBarData> SymbolBarDatas => this.GetSymbolBarDatas();

        public DirectoryInfo DataPath => new DirectoryInfo(TestKitConstants.TestDataDirectory);

        public string TimestampParsePattern => "yyyy.MM.dd HH:mm:ss";

        public int VolumeMultiple => 1000000;

        public bool IsBarDataCheckOn => false;

        // TODO: Temporary property. Remove once Dukascopy provider removed.
        public bool InitialFromDateSpecified => false;

        public string GetResolutionLabel(BarResolution resolution)
        {
            switch (resolution)
            {
                case BarResolution.Second:
                    return "Second";

                case BarResolution.Minute:
                    return "Min";

                case BarResolution.Hour:
                    return "Hour";

                case BarResolution.Day:
                    return "Day";

                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private IReadOnlyCollection<SymbolBarData> GetSymbolBarDatas()
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

            var symbolBarSpecs = new List<SymbolBarData>();

            foreach (var symbol in symbols)
            {
                symbolBarSpecs.Add(new SymbolBarData(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 1)));
                symbolBarSpecs.Add(new SymbolBarData(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Ask, BarResolution.Minute, 1)));
                symbolBarSpecs.Add(new SymbolBarData(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Bid, BarResolution.Hour, 1)));
                symbolBarSpecs.Add(new SymbolBarData(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Ask, BarResolution.Hour, 1)));
                symbolBarSpecs.Add(new SymbolBarData(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Bid, BarResolution.Day, 1)));
                symbolBarSpecs.Add(new SymbolBarData(new Symbol(symbol, Exchange.Dukascopy), new BarSpecification(BarQuoteType.Ask, BarResolution.Day, 1)));
            }

            return symbolBarSpecs.AsReadOnly();
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
