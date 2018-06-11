// -------------------------------------------------------------------------------------------------
// <copyright file="StubSetupContainerFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Moq;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Risk;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Logging;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The stub nautilus setup container.
    /// </summary>
    public class StubSetupContainerFactory
    {
        /// <summary>
        /// Gets the containers logger.
        /// </summary>
        public MockLoggingAdapter LoggingAdapter { get; private set; }

        /// <summary>
        /// Gets the containers quote provider.
        /// </summary>
        public IQuoteProvider QuoteProvider { get; private set; }

        /// <summary>
        /// Gets the containers quote provider.
        /// </summary>
        public StubClock Clock { get; private set; }

        /// <summary>
        /// Creates a new <see cref="BlackBoxContainer"/>.
        /// </summary>
        /// <returns>The <see cref="BlackBoxContainer"/>.</returns>
        public BlackBoxContainer Create()
        {
            var environment = NautilusEnvironment.Live;

            this.Clock = new StubClock();;
            this.Clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            this.LoggingAdapter = new MockLoggingAdapter();
            var loggerFactory = new LoggerFactory(this.LoggingAdapter);

            var guidFactory = new GuidFactory();
            var instrumentRepository = new Mock<IInstrumentRepository>().Object;
            this.QuoteProvider = new QuoteProvider(Exchange.FXCM);

            var riskModel = new RiskModel(
                new EntityId("None"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                this.Clock.TimeNow());

            var account = StubAccountFactory.Create();

            return new BlackBoxContainer(
                environment,
                this.Clock,
                guidFactory,
                loggerFactory,
                instrumentRepository,
                this.QuoteProvider,
                riskModel,
                account);
        }
    }
}
