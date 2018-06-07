//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Algorithms.Entry;
    using Nautilus.Algorithms.EntryStop;
    using Nautilus.Algorithms.ProfitTarget;
    using Nautilus.Algorithms.StopLoss;
    using Nautilus.Algorithms.TrailingStop;
    using Nautilus.BlackBox.AlphaModel;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Brokerage;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Data;
    using Nautilus.BlackBox.Data.Instrument;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Execution;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.BlackBox.Risk;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Serilog;
    using NodaTime;

    public static class Program
    {
        private static void Main()
        {
            var environment = NautilusEnvironment.Live;
            var clock = new Clock(DateTimeZone.Utc);
            var loggingAdatper = new SerilogLogger();
            var databaseAdapter = new DummyDatabase();
            var instrumentRepository = InstrumentRepositoryFactory.Create(clock, databaseAdapter);
            var quoteProvider = new QuoteProvider(Exchange.FXCM);

            var riskModel = new RiskModel(
                new EntityId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(0.1m),
                Quantity.Create(2),
                true,
                clock.TimeNow());

            var broker = Broker.FXCM;
            var username = "D102412895"; //"D102412895"; //ConfigReader.GetArgumentValue(ConfigurationManager.AppSettings, "Username"); // "D102412895";
            var password = "1234"; //"1234"; //ConfigReader.GetArgumentValue(ConfigurationManager.AppSettings, "Password"); // "1234";
            var accountNumber = "02402856"; //ConfigReader.GetArgumentValue(ConfigurationManager.AppSettings, "AccountNumber"); // "02402856";

            var account = new BrokerageAccount(
                broker,
                username,
                password,
                accountNumber,
                CurrencyCode.AUD,
                clock.TimeNow());

            var fixCredentials = new FixCredentials(username, password, accountNumber);

            var serviceFactory = new BlackBoxServicesFactory(
                new BrokerageGatewayFactory(),
                new FixClientFactory(Broker.FXCM, fixCredentials),
                new AlphaModelServiceFactory(),
                new DataServiceFactory(),
                new ExecutionServiceFactory(),
                new PortfolioServiceFactory(),
                new RiskServiceFactory());

            var blackBox = BlackBoxFactory.Create(
                environment,
                clock,
                loggingAdatper,
                databaseAdapter,
                instrumentRepository,
                quoteProvider,
                riskModel,
                account,
                serviceFactory);

            Console.ReadLine();

            blackBox.ConnectToBrokerage();

            Console.ReadLine();

            blackBox.InitializeSession();

            Console.ReadLine();

            var tradingSymbols = new List<Symbol>
            {
                new Symbol("AUDUSD", Exchange.FXCM),
                new Symbol("EURUSD", Exchange.FXCM),
                new Symbol("GBPUSD", Exchange.FXCM),
                new Symbol("USDCAD", Exchange.FXCM),
                // new Symbol("WTIUSD", Exchange.FXCM)

                // "AUDJPY",
                // "USDJPY",
                // "WTIUSD",
                // "AUS200",
                // "JPN225",
                // "SPX500"
            };

            foreach (var symbol in tradingSymbols)
            {
                var instrument = blackBox.GetInstrument(symbol).Value;
                var barProfile = new BarSpecification(
                    BarQuoteType.Bid,
                    BarResolution.Minute,
                    1);
                var tradePeriod = 20;

                var tradeProfile = new TradeProfile(
                    new TradeType("TestScalp"),
                    barProfile,
                    tradePeriod,
                    3,
                    1000,
                    180,
                    30,
                    2.0m,
                    1.0m,
                    1,
                    clock.TimeNow());

                var signalLogicFsm = new SignalLogic(true, true);

                var entryStopAlgorithm      = new BarStretchStop(tradeProfile, instrument);
                var stoplossAlgorithm       = new BarsBackWithBufferStop(tradeProfile, instrument, 3, 2.0m);
                var profitTargetAlgorithm   = new RiskMultiplesTarget(tradeProfile, instrument, tradeProfile.Units - 1, 1.0m);

                var entryAlgorithms         = new List<IEntryAlgorithm> { new CloseDirectionEntry(tradeProfile, instrument, entryStopAlgorithm) };
                var trailingStopAlgorithms  = new List<ITrailingStopAlgorithm> { new KeltnerChannelRatchetTrail(tradeProfile, instrument, 2.5m, 3, 2, 1, 0) };
                var exitAlgorithms          = new List<IExitAlgorithm>();

                var strategy = new AlphaStrategy(
                    instrument,
                    tradeProfile,
                    signalLogicFsm,
                    entryStopAlgorithm,
                    stoplossAlgorithm,
                    profitTargetAlgorithm,
                    entryAlgorithms,
                    trailingStopAlgorithms,
                    exitAlgorithms);

                blackBox.AddAlphaStrategyModule(strategy);
            }

            Console.ReadLine();

            blackBox.StartAlphaStrategyModulesAll();

            Console.ReadLine();

            blackBox.Terminate();
            blackBox.Dispose();

            Console.ReadLine();
        }
    }
}
