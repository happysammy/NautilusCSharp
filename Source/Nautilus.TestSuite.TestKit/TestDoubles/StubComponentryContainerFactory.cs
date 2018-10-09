//--------------------------------------------------------------------------------------------------
// <copyright file="StubComponentryContainerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Data.Aggregators;
    using Nautilus.DomainModel.Enums;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubComponentryContainerFactory
    {
        public MockLoggingAdapter LoggingAdapter { get; private set; }

        public IQuoteProvider QuoteProvider { get; private set; }

        public StubClock Clock { get; set; }

        public IComponentryContainer Create()
        {
            this.Clock = new StubClock();
            this.Clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            this.LoggingAdapter = new MockLoggingAdapter();
            var loggerFactory = new LoggerFactory(this.LoggingAdapter);

            var guidFactory = new GuidFactory();
            this.QuoteProvider = new QuoteProvider(Venue.FXCM);

            return new ComponentryContainer(
                this.Clock,
                guidFactory,
                loggerFactory);
        }
    }
}
