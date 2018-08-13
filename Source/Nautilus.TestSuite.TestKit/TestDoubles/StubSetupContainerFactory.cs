//--------------------------------------------------------------------------------------------------
// <copyright file="StubSetupContainerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Moq;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Data.Aggregators;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubSetupContainerFactory
    {
        public MockLoggingAdapter LoggingAdapter { get; private set; }

        public IQuoteProvider QuoteProvider { get; private set; }

        public StubClock Clock { get; set; }

        public BlackBoxContainer Create()
        {
            var environment = BlackBoxEnvironment.Live;

            this.Clock = new StubClock();
            this.Clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            this.LoggingAdapter = new MockLoggingAdapter();
            var loggerFactory = new LoggerFactory(this.LoggingAdapter);

            var guidFactory = new GuidFactory();
            var instrumentRepository = new Mock<IInstrumentRepository>().Object;
            this.QuoteProvider = new QuoteProvider(Venue.FXCM);

            var riskModel = new RiskModel(
                new RiskModelId("None"),
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
