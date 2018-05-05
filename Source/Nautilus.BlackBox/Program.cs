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
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Data;
    using Nautilus.BlackBox.Data.Instrument;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Execution;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.BlackBox.Risk;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Logging;
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
            var loggingAdapter = new SerilogLogger();
            var loggerFactory = new LoggerFactory(loggingAdapter);
            var guidFactory = new GuidFactory();

            //var ravenDbAdapter = new RavenDbAdapter("Instruments");
            var database = new DummyDatabase();
            var instrumentRepository = InstrumentRepositoryFactory.Create(clock, database);

            var quoteProvider = new QuoteProvider(Exchange.FXCM);

            var riskModel = new RiskModel(
                new EntityId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(0.1m),
                Quantity.Create(2),
                true,
                clock.TimeNow());

            var broker = Broker.FXCM;
            var username = "D102862129"; //"D102412895"; //ConfigReader.GetArgumentValue(ConfigurationManager.AppSettings, "Username"); // "D102412895";
            var password = "demo"; //"1234"; //ConfigReader.GetArgumentValue(ConfigurationManager.AppSettings, "Password"); // "1234";
            var accountNumber = "02851908"; //ConfigReader.GetArgumentValue(ConfigurationManager.AppSettings, "AccountNumber"); // "02402856";

            var account = new BrokerageAccount(
                broker,
                username,
                password,
                accountNumber,
                CurrencyCode.AUD,
                clock.TimeNow());

            var container = new BlackBoxSetupContainer(
                environment,
                clock,
                guidFactory,
                loggerFactory,
                instrumentRepository,
                quoteProvider,
                riskModel,
                account);

            var brokerageServiceFactory = new BrokerageGatewayFactory();
            var alphaModelServiceFactory = new AlphaModelServiceFactory();
            var dataServiceFactory = new DataServiceFactory();
            var executionServiceFactory = new ExecutionServiceFactory();
            var portfolioServiceFactory = new PortfolioServiceFactory();
            var riskServiceFactory = new RiskServiceFactory();

            var serviceFactory = new BlackBoxServicesFactory(
                brokerageServiceFactory,
                alphaModelServiceFactory,
                dataServiceFactory,
                executionServiceFactory,
                portfolioServiceFactory,
                riskServiceFactory);

            var credentials = new FixCredentials(username, password, accountNumber);
            var fixClient = new FixClient(container, credentials, broker);

            var nautilusSystem = new BlackBox(
                new Label("NautilusActorSystem"),
                container,
                serviceFactory,
                fixClient,
                account,
                riskModel);

            // var forexConnect = new FxcmForexConnect();
            // forexConnect.Connect();
            // forexConnect.RequestHistoricalBars("AUD/USD", "Min", NautilusTrader.TimeNow() - TimePeriod.FromDays(1), NautilusTrader.TimeNow());

            // Console.ReadLine();

            // forexConnect.Disconnect();

            // Console.ReadLine();

            Console.ReadLine();

            nautilusSystem.ConnectToBrokerage();

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
                var instrument = nautilusSystem.GetInstrument(symbol).Value;
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

                nautilusSystem.AddAlphaStrategyModule(strategy);
            }

            Console.ReadLine();

            nautilusSystem.StartAlphaStrategyModulesAll();

            Console.ReadLine();

            nautilusSystem.Terminate();
            nautilusSystem.Dispose();

            Console.ReadLine();
        }
    }
}
