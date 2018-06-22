// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarClientTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.FixTests
{
    using System.Diagnostics.CodeAnalysis;
    using Akka.Event;
    using Akka.TestKit.Xunit2;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Processors;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class FixTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly IDataClient client;

        public FixTests(ITestOutputHelper output)
        {
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.logger = setupFactory.LoggingAdapter;
            var container = setupFactory.Create();

            var quoteProvider = new QuoteProvider(Exchange.FXCM);
            var tickSizeIndex = FxcmTickSizeProvider.GetIndex();
            var messagingAdapter = new MockMessagingAdapter(TestActor);
            var tickDataProcessor = new TickDataProcessor(
                container,
                tickSizeIndex,
                quoteProvider,
                new StandardOutLogger());

            var username = "D102412895";
            var password = "1234";
            var accountNumber = "02402856";

            var clientFactory = new FxcmFixClientFactory(username, password, accountNumber);

            this.client = clientFactory.DataClient(
                container,
                messagingAdapter,
                tickDataProcessor);
        }

        [Fact]
        internal void Test_can_connect()
        {
//            this.client.Connect();
//
//            LogDumper.Dump(this.logger, this.output, 5000);
        }
    }
}
