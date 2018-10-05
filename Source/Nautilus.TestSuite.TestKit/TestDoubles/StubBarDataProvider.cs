//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarDataProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubBarDataProvider : IBarDataProvider
    {
        public string DateTimeParsePattern => "yyyy.MM.dd HH:mm:ss";

        public IReadOnlyCollection<BarType> SymbolBarDatas => this.GetSymbolBarDatas();

        public DirectoryInfo DataPath => new DirectoryInfo(TestKitConstants.TestDataDirectory);

        public string TimestampParsePattern => "yyyy.MM.dd HH:mm:ss";

        public int VolumeMultiple => 1000000;

        public bool IsBarDataCheckOn => false;

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
                                  "ZARJPY",
                              }.Distinct();

            var barTypes = new List<BarType>();

            foreach (var symbol in symbols)
            {
                barTypes.Add(new BarType(new Symbol(symbol, Venue.DUKASCOPY), new BarSpecification(QuoteType.Bid, Resolution.Minute, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.DUKASCOPY), new BarSpecification(QuoteType.Ask, Resolution.Minute, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.DUKASCOPY), new BarSpecification(QuoteType.Bid, Resolution.Hour, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.DUKASCOPY), new BarSpecification(QuoteType.Ask, Resolution.Hour, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.DUKASCOPY), new BarSpecification(QuoteType.Bid, Resolution.Day, 1)));
                barTypes.Add(new BarType(new Symbol(symbol, Venue.DUKASCOPY), new BarSpecification(QuoteType.Ask, Resolution.Day, 1)));
            }

            return barTypes.AsReadOnly();
        }
    }
}
