//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.FixTester
{
    using System;
    using Akka.Event;
    using Nautilus.BlackBox;
    using Nautilus.BlackBox.Brokerage;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Data.Instrument;
    using Nautilus.BlackBox.Risk;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Processors;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Serilog;
    using NodaTime;

    public static class Program
    {
        private static void Main()
        {
            var environment = BlackBoxEnvironment.Live;
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
            var username = "D102412895";
            var password = "1234";
            var accountNumber = "02402856";

            var account = new BrokerageAccount(
                broker,
                username,
                password,
                accountNumber,
                CurrencyCode.AUD,
                clock.TimeNow());

            var clientFactory = new FxcmFixClientFactory(username, password, accountNumber);

            var loggerFactory = new LoggerFactory(loggingAdatper);
            var guidFactory = new GuidFactory();

            var messagingAdapter = new MessagingAdapter(
                new StandardOutLogger(),
                new StandardOutLogger(),
                new StandardOutLogger());

            var container = new BlackBoxContainer(
                environment,
                clock,
                guidFactory,
                loggerFactory,
                instrumentRepository,
                quoteProvider,
                riskModel,
                account);

            var tickDataProcessor = new TickDataProcessor(
                container,
                FxcmTickSizeProvider.GetIndex(),
                quoteProvider,
                new StandardOutLogger());

            var client = clientFactory.DataClient(
                container,
                messagingAdapter,
                tickDataProcessor);

            Console.ReadLine();

            client.Connect();

            Console.ReadLine();

            client.Disconnect();

        }
    }
}
